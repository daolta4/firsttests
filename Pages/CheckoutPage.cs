using OpenQA.Selenium;

namespace FirstTests.Pages;

public class CheckoutPage : BasePage
{
    // Locator private — file test KHÔNG thấy, KHÔNG cần biết
    private readonly By nutCheckout   = By.Id("checkout");
    private readonly By oTen          = By.Id("first-name");
    private readonly By oHo           = By.Id("last-name");
    private readonly By oMaBuuChinh   = By.Id("postal-code");
    private readonly By nutContinue   = By.Id("continue");
    private readonly By nutFinish     = By.Id("finish");
    private readonly By tieuDeXacNhan = By.CssSelector("span.title");
    private readonly By loi           = By.CssSelector("h3[data-test='error']");

    public CheckoutPage(IWebDriver d) : base(d) { }

    /// <summary>Bấm Checkout. Hiệu ứng: form nhập thông tin đã hiện.</summary>
    public CheckoutPage BatDau()
    {
        ClickChoDenKhi(nutCheckout, d => d.FindElements(oTen).Count > 0);
        return this;
    }

    public CheckoutPage NhapThongTin(string ten, string ho, string maBuuChinh)
    {
        if (!string.IsNullOrEmpty(ten))         NhapChu(oTen, ten);
        if (!string.IsNullOrEmpty(ho))          NhapChu(oHo, ho);
        if (!string.IsNullOrEmpty(maBuuChinh))  NhapChu(oMaBuuChinh, maBuuChinh);

        // Bấm Continue. Hiệu ứng hợp lệ có HAI khả năng:
        //  - sang bước tổng kết (nút Finish hiện), hoặc
        //  - ở lại và báo lỗi thiếu trường.
        // Ca nghịch cũng là KẾT QUẢ, không phải sự cố -> phải nằm trong điều kiện chờ.
        ClickChoDenKhi(nutContinue,
                       d => d.FindElements(nutFinish).Count > 0 || d.FindElements(loi).Count > 0);
        return this;
    }

    public string LayThongBaoLoi() => Tim(loi).Text;

    /// <summary>Bấm Finish. Hiệu ứng: đã sang trang hoàn tất.</summary>
    public string HoanTatVaLayTieuDe()
    {
        ClickChoDenKhi(nutFinish,
                       d => d.Url.Contains("checkout-complete", StringComparison.OrdinalIgnoreCase));
        return Tim(tieuDeXacNhan).Text;
    }
}