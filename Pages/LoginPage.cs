using OpenQA.Selenium;

namespace FirstTests.Pages;

public class LoginPage : BasePage
{
    // Locator private — file test KHÔNG thấy, KHÔNG cần biết
    private readonly By oTaiKhoan = By.Id("user-name");
    private readonly By oMatKhau  = By.Id("password");
    private readonly By nutLogin  = By.Id("login-button");
    private readonly By loi       = By.CssSelector("h3[data-test='error']");

    public LoginPage(IWebDriver d) : base(d) { }

    // Bài 4 có "public const string Url" — Bài 5 BỎ, vì URL đã về appsettings.json

    public InventoryPage DangNhap(string tk, string mk)
    {
        var o1 = Tim(oTaiKhoan); o1.Clear(); o1.SendKeys(tk);
        var o2 = Tim(oMatKhau);  o2.Clear(); o2.SendKeys(mk);
        Tim(nutLogin).Click();
        return new InventoryPage(driver);   // trả về TRANG TIẾP THEO
    }

    public string LayThongBaoLoi() => Tim(loi).Text;
}