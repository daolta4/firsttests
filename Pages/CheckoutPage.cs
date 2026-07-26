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

    public CheckoutPage BatDau()
    {
        Tim(nutCheckout).Click();
        return this;
    }

    public CheckoutPage NhapThongTin(string ten, string ho, string maBuuChinh)
    {
        // var o1 = Tim(oTen);
        // var o2 = Tim(oHo);
        // var o3 = Tim(oMaBuuChinh);

        // Tim(nutContinue).Click();
        // if(!string.IsNullOrEmpty(ten)) o1.SendKeys(ten);
        // if(!string.IsNullOrEmpty(ho)) o1.SendKeys(ho);
        // if(!string.IsNullOrEmpty(maBuuChinh)) o1.SendKeys(maBuuChinh);
        
        // return this;

        if (!string.IsNullOrEmpty(ten))
        {
            var o1 = Tim(oTen);
            o1.Clear();
            o1.SendKeys(ten);
        }

        if (!string.IsNullOrEmpty(ho))
        {
            var o2 = Tim(oHo);
            o2.Clear();
            o2.SendKeys(ho);
        }

        if (!string.IsNullOrEmpty(maBuuChinh))
        {
            var o3 = Tim(oMaBuuChinh);
            o3.Clear();
            o3.SendKeys(maBuuChinh);
        }

        Tim(nutContinue).Click();
        return this;
    }

    public string LayThongBaoLoi() => Tim(loi).Text;

    public string HoanTatVaLayTieuDe()
    {
        Tim(nutFinish).Click();
        return Tim(tieuDeXacNhan).Text;
    }
}