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
        // Timeout lấy từ config, không viết cứng số 10 như Bài 4
        wait = new WebDriverWait(driver,
                   TimeSpan.FromSeconds(ConfigReader.Load().TimeoutSeconds));
    }

    /// <summary>Tìm phần tử KÈM chờ. Mọi Page class dùng chung.</summary>
    protected IWebElement Tim(By by) => wait.Until(d => d.FindElement(by));
}