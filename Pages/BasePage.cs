using FirstTests.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FirstTests.Pages;

public abstract class BasePage
{
    protected readonly IWebDriver    driver;
    protected readonly WebDriverWait wait;

    protected BasePage(IWebDriver driver)
    {
        this.driver = driver;
        // Timeout lấy từ config (đã nâng lên 30s cho môi trường CI chậm)
        wait = new WebDriverWait(driver,
                   TimeSpan.FromSeconds(ConfigReader.Load().TimeoutSeconds));

        // QUAN TRỌNG: trong lúc chờ, BỎ QUA NoSuchElement + StaleElement.
        // - NoSuchElement: nếu không bỏ qua, wait.Until văng ngay ở lần tìm đầu tiên
        //   (khi phần tử chưa kịp render) thay vì chờ đủ thời gian.
        // - StaleElement: saucedemo là ứng dụng React, nó VẼ LẠI (re-render) DOM liên
        //   tục. Phần tử tìm được ở giây trước có thể đã bị thay bằng node mới.
        // Đây là nguyên nhân test "trên máy thì xanh, trên CI thì đỏ".
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException),
                                  typeof(StaleElementReferenceException));
    }

    /// <summary>Tìm phần tử KÈM chờ. Mọi Page class dùng chung.</summary>
    protected IWebElement Tim(By by) => wait.Until(d => d.FindElement(by));

    /// <summary>Tìm phần tử ĐANG HIỆN và ĐANG BẬT — tức là click được thật.</summary>
    protected IWebElement TimCoTheClick(By by) =>
        wait.Until(d =>
        {
            var e = d.FindElement(by);
            return e.Displayed && e.Enabled ? e : null;   // null = chưa thoả -> chờ tiếp
        });

    /// <summary>
    /// Click RỒI KIỂM TRA hiệu ứng; nếu click bị "rơi" thì click lại.
    ///
    /// VÌ SAO CẦN: saucedemo là SPA (React). Giữa lúc FindElement và lúc Click,
    /// React có thể thay node cũ bằng node mới. Selenium click vào node đã bị
    /// tháo khỏi cây DOM -> KHÔNG có exception, nhưng cũng KHÔNG có gì xảy ra.
    /// Máy cá nhân nhanh nên hầu như không gặp; runner CI 2-4 vCPU chạy 3 Chrome
    /// song song thì gặp thường xuyên. Chỉ chờ "phần tử có tồn tại" là KHÔNG đủ —
    /// phải chờ KẾT QUẢ của cú click.
    /// </summary>
    /// <param name="by">Phần tử cần click.</param>
    /// <param name="hieuUng">Dấu hiệu chứng minh cú click đã ăn.</param>
    /// <param name="soLanThu">Số lần click lại tối đa.</param>
    protected void ClickChoDenKhi(By by, Func<IWebDriver, bool> hieuUng, int soLanThu = 3)
    {
        for (var lan = 1; lan <= soLanThu; lan++)
        {
            // Lần cuối mới hạ xuống click bằng JS. Ưu tiên click THẬT để test vẫn
            // mô phỏng đúng người dùng; JS chỉ là phao cứu sinh cho môi trường lỗi.
            var dungJs = lan == soLanThu;

            try
            {
                var e = TimCoTheClick(by);
                if (dungJs)
                {
                    TestContext.Out.WriteLine(
                        $"[CANH BAO] Click thật vào {by} không ăn -> chuyển sang click bằng JS.");
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", e);
                }
                else
                {
                    e.Click();
                }
            }
            catch (StaleElementReferenceException)
            {
                continue;                       // node bị thay ngay lúc click -> thử lại
            }
            catch (ElementClickInterceptedException)
            {
                continue;                       // có gì che mất -> thử lại
            }

            if (ChoHieuUng(hieuUng)) return;    // click đã ăn -> xong
        }

        throw new WebDriverTimeoutException(
            $"Đã click {soLanThu} lần vào {by} mà không thấy hiệu ứng mong đợi. " +
            "Có thể locator đúng nhưng trang chưa xử lý được cú click.");
    }

    /// <summary>Chờ ngắn (5s) xem cú click vừa rồi có tạo ra hiệu ứng hay không.</summary>
    private bool ChoHieuUng(Func<IWebDriver, bool> hieuUng)
    {
        var w = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        w.IgnoreExceptionTypes(typeof(NoSuchElementException),
                               typeof(StaleElementReferenceException));
        try   { return w.Until(hieuUng); }
        catch (WebDriverTimeoutException) { return false; }   // click bị rơi -> gọi thử lại
    }

    /// <summary>
    /// Nhập chữ RỒI KIỂM TRA ô đã nhận đúng giá trị. Cùng lý do như ClickChoDenKhi:
    /// React vẽ lại ô input thì SendKeys/Clear cũng có thể bị mất trắng, dẫn tới
    /// lỗi giả kiểu "Error: First Name is required" dù test đã gõ tên.
    /// </summary>
    protected void NhapChu(By by, string chu, int soLanThu = 3)
    {
        for (var lan = 1; lan <= soLanThu; lan++)
        {
            try
            {
                var o = TimCoTheClick(by);

                if (lan == soLanThu)
                {
                    TestContext.Out.WriteLine(
                        $"[CANH BAO] Gõ thật vào {by} không ăn -> chuyển sang gán bằng JS.");
                    ((IJavaScriptExecutor)driver).ExecuteScript(JsGanGiaTriKieuReact, o, chu);
                }
                else
                {
                    o.Clear();
                    o.SendKeys(chu);
                }

                if (o.GetDomProperty("value") == chu) return;
            }
            catch (StaleElementReferenceException)
            {
                // node bị thay giữa lúc gõ -> thử lại
            }
        }

        throw new WebDriverException($"Không nhập được '{chu}' vào {by} sau {soLanThu} lần thử.");
    }

    /// <summary>
    /// Gán giá trị cho ô input của React. KHÔNG thể gán thẳng element.value: React giữ
    /// state riêng nên sẽ ghi đè lại. Phải gọi setter gốc rồi tự bắn sự kiện input/change
    /// để React biết mà cập nhật state.
    /// </summary>
    private const string JsGanGiaTriKieuReact = """
        var o = arguments[0], v = arguments[1];
        var setter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
        setter.call(o, v);
        o.dispatchEvent(new Event('input',  { bubbles: true }));
        o.dispatchEvent(new Event('change', { bubbles: true }));
        """;
}
