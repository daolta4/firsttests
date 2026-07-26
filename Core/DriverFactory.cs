using FirstTests.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace FirstTests.Core;

public static class DriverFactory
{
    /// <summary>Trả về IWebDriver (KHÔNG phải ChromeDriver) — đây là chỗ đa hình phát huy.</summary>
    public static IWebDriver Create(TestConfig cfg) => cfg.Browser.Trim().ToLower() switch
    {
        "chrome"  => new ChromeDriver(ChromeOpts(cfg)),
        "firefox" => new FirefoxDriver(FirefoxOpts(cfg)),
        "edge"    => new EdgeDriver(EdgeOpts(cfg)),
        _ => throw new ArgumentException(
                 $"Trình duyệt không hỗ trợ: '{cfg.Browser}'. Hợp lệ: chrome | firefox | edge")
    };

    private static ChromeOptions ChromeOpts(TestConfig cfg)
    {
		var o = new ChromeOptions();
       	bool headless = cfg.Headless
			|| Environment.GetEnvironmentVariable("CI") == "true";
		if (headless)
		{
			o.AddArgument("--headless=new");
			o.AddArgument("--window-size=1920,1080");
			o.AddArgument("--no-sandbox");
			o.AddArgument("--disable-dev-shm-usage");
			o.AddArgument("--disable-gpu");

			// Chay nhieu Chrome song song tren runner: cua so bi coi la "bi che khuat"
			// -> Chrome ha muc uu tien renderer, su kien chuot that co the bi bo qua.
			o.AddArgument("--disable-backgrounding-occluded-windows");
			o.AddArgument("--disable-renderer-backgrounding");
			o.AddArgument("--disable-background-timer-throttling");
		}
        o.AddArgument("--disable-notifications");
        o.SetLoggingPreference(LogType.Browser, LogLevel.All);   // TẠM: để đọc lỗi JS trên CI
        return o;
    }

    private static FirefoxOptions FirefoxOpts(TestConfig cfg)
    {
        var o = new FirefoxOptions();
        if (cfg.Headless) o.AddArgument("--headless");       // Firefox: KHÔNG có "=new"
        o.AddArgument($"--width={cfg.WindowWidth}");
        o.AddArgument($"--height={cfg.WindowHeight}");
        return o;
    }

    private static EdgeOptions EdgeOpts(TestConfig cfg)
    {
        var o = new EdgeOptions();
        if (cfg.Headless) o.AddArgument("--headless=new");
        o.AddArgument($"--window-size={cfg.WindowWidth},{cfg.WindowHeight}");
        return o;
    }
}