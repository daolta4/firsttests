using FirstTests.Core;
using FirstTests.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace FirstTests.Tests;

/// <summary>
/// TẠM THỜI — so sánh các cách click xem cách nào thực sự ăn trên CI. Xoá sau khi chốt.
/// </summary>
public class ChanDoanTests : BaseTest
{
    private static void Ghi(string s) => TestContext.Out.WriteLine($"[SOI] {s}");

    private void VeTrangSanPham()
    {
        driver.Navigate().GoToUrl("https://www.saucedemo.com/inventory.html");
        new InventoryPage(driver).DaHienThi();
    }

    /// <summary>Thử 1 cách click vào giỏ hàng, trả về true nếu URL đã sang trang giỏ.</summary>
    private bool ThuClickGio(string ten, Action<IWebElement> cachClick)
    {
        VeTrangSanPham();
        var gio = driver.FindElement(By.ClassName("shopping_cart_link"));

        try   { cachClick(gio); }
        catch (Exception ex) { Ghi($"{ten}: NEM {ex.GetType().Name}: {ex.Message.Split('\n')[0]}"); return false; }

        Thread.Sleep(2500);
        var ok = driver.Url.Contains("cart");
        Ghi($"{ten}: {(ok ? "AN" : "ROI")}  (url={driver.Url})");
        return ok;
    }

    [Test]
    [Category("ChanDoan")]
    public void So_Sanh_Cac_Cach_Click()
    {
        var js = (IJavaScriptExecutor)driver;

        new LoginPage(driver).DangNhap("standard_user", "secret_sauce");
        new InventoryPage(driver).DaHienThi();
        Ghi($"userAgent={js.ExecuteScript("return navigator.userAgent")}");
        Ghi($"devicePixelRatio={js.ExecuteScript("return window.devicePixelRatio")}");
        Ghi($"hasFocus={js.ExecuteScript("return document.hasFocus()")}");

        ThuClickGio("1-Click thuong", e => e.Click());

        ThuClickGio("2-Scroll roi click", e =>
        {
            js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", e);
            Thread.Sleep(300);
            e.Click();
        });

        ThuClickGio("3-Actions MoveToElement", e =>
            new Actions(driver).MoveToElement(e).Pause(TimeSpan.FromMilliseconds(200)).Click().Perform());

        ThuClickGio("4-Click JS", e => js.ExecuteScript("arguments[0].click();", e));

        Assert.Pass("Chi de soi.");
    }
}
