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

        // QUAN TRỌNG: trong lúc chờ, BỎ QUA NoSuchElement.
        // Nếu không, wait.Until sẽ văng ngay ở lần tìm đầu tiên (khi phần tử
        // chưa kịp render) thay vì chờ đủ thời gian — đây là nguyên nhân test
        // "trên máy thì xanh, trên CI thì đỏ".
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
    }

    /// <summary>Tìm phần tử KÈM chờ. Mọi Page class dùng chung.</summary>
    protected IWebElement Tim(By by) => wait.Until(d => d.FindElement(by));
}