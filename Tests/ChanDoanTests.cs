using FirstTests.Core;
using FirstTests.Pages;
using OpenQA.Selenium;

namespace FirstTests.Tests;

/// <summary>
/// TẠM THỜI — chỉ để soi xem trên CI cú click "rơi" ở đâu. Xoá sau khi tìm ra nguyên nhân.
/// </summary>
public class ChanDoanTests : BaseTest
{
    private const string JsSoiDiemClick = """
        var e = arguments[0];
        e.scrollIntoView({block:'center'});
        var r = e.getBoundingClientRect();
        var cx = r.left + r.width/2, cy = r.top + r.height/2;
        var t = document.elementFromPoint(cx, cy);
        return JSON.stringify({
          viewport: [window.innerWidth, window.innerHeight],
          scroll:   [window.scrollX, window.scrollY],
          rect:     [Math.round(r.left), Math.round(r.top), Math.round(r.width), Math.round(r.height)],
          diem:     [Math.round(cx), Math.round(cy)],
          phanTuTaiDiem: t ? (t.tagName + '#' + t.id + '.' + t.className) : null,
          dungPhanTu:    t ? (t === e || e.contains(t) || t.contains(e)) : false
        });
        """;

    private void Soi(string nhan, By by)
    {
        var js = (IJavaScriptExecutor)driver;
        var els = driver.FindElements(by);
        TestContext.Out.WriteLine($"[SOI] {nhan}: tim thay {els.Count} phan tu");
        if (els.Count == 0) return;

        var e = els[0];
        TestContext.Out.WriteLine($"[SOI] {nhan}: displayed={e.Displayed} enabled={e.Enabled}");
        TestContext.Out.WriteLine($"[SOI] {nhan}: html={e.GetAttribute("outerHTML")}");
        TestContext.Out.WriteLine($"[SOI] {nhan}: {js.ExecuteScript(JsSoiDiemClick, e)}");
    }

    [Test]
    [Category("ChanDoan")]
    public void Soi_Vi_Sao_Click_Bi_Roi()
    {
        var js = (IJavaScriptExecutor)driver;

        new LoginPage(driver).DangNhap("standard_user", "secret_sauce");
        new InventoryPage(driver).DaHienThi();

        TestContext.Out.WriteLine($"[SOI] url={driver.Url}");
        TestContext.Out.WriteLine($"[SOI] readyState={js.ExecuteScript("return document.readyState")}");
        TestContext.Out.WriteLine($"[SOI] userAgent={js.ExecuteScript("return navigator.userAgent")}");

        var nutThem = By.Id("add-to-cart-sauce-labs-backpack");
        var nutBo   = By.Id("remove-sauce-labs-backpack");
        var gio     = By.ClassName("shopping_cart_link");

        // ---- Bước 1: THÊM VÀO GIỎ ----
        Soi("nutThem", nutThem);

        driver.FindElement(nutThem).Click();
        Thread.Sleep(3000);
        var themOk = driver.FindElements(nutBo).Count > 0;
        TestContext.Out.WriteLine($"[SOI] click THUONG vao nutThem -> an? {themOk}");

        if (!themOk)
        {
            js.ExecuteScript("arguments[0].click();", driver.FindElement(nutThem));
            Thread.Sleep(3000);
            TestContext.Out.WriteLine(
                $"[SOI] click JS vao nutThem -> an? {driver.FindElements(nutBo).Count > 0}");
        }

        // ---- Bước 2: MỞ GIỎ ----
        Soi("gio", gio);

        driver.FindElement(gio).Click();
        Thread.Sleep(3000);
        TestContext.Out.WriteLine($"[SOI] click THUONG vao gio -> url={driver.Url}");

        if (!driver.Url.Contains("cart"))
        {
            js.ExecuteScript("arguments[0].click();", driver.FindElement(gio));
            Thread.Sleep(3000);
            TestContext.Out.WriteLine($"[SOI] click JS vao gio -> url={driver.Url}");
        }

        if (!driver.Url.Contains("cart"))
        {
            js.ExecuteScript("window.location.href='/cart.html';");
            Thread.Sleep(3000);
            TestContext.Out.WriteLine($"[SOI] ep chuyen url -> url={driver.Url}");
            TestContext.Out.WriteLine(
                $"[SOI] o trang cart co #checkout? {driver.FindElements(By.Id("checkout")).Count}");
        }

        // ---- Log lỗi JS của trình duyệt (nếu bundle React chết thì lộ ra ở đây) ----
        try
        {
            foreach (var l in driver.Manage().Logs.GetLog(LogType.Browser))
                TestContext.Out.WriteLine($"[SOI][console] {l.Level} {l.Message}");
        }
        catch (Exception ex)
        {
            TestContext.Out.WriteLine($"[SOI] khong doc duoc console log: {ex.Message}");
        }

        Assert.Pass("Chi de soi, khong assert gi.");
    }
}
