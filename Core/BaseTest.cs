using FirstTests.Config;
using FirstTests.Utilities;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

namespace FirstTests.Core;

public abstract class BaseTest
{
    protected IWebDriver driver = null!;
    protected TestConfig cfg    = null!;

    [SetUp]
	public void BaseSetup()
	{
		var ten = TestContext.CurrentContext.Test.Name;
		Log.Info($"===== BẮT ĐẦU: {ten} =====");

		cfg    = ConfigReader.Load();
		driver = DriverFactory.Create(cfg);
		Log.Info($"Mở {cfg.Browser}, tới {cfg.BaseUrl}");

		driver.Navigate().GoToUrl(cfg.BaseUrl);
	}

	[TearDown]
	public void BaseTearDown()
	{
		try
		{
			if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
			{
				Log.Error($"RỚT: {TestContext.CurrentContext.Test.Name}");
				ScreenshotHelper.Chup(driver);
			}
			else
			{
				Log.Info("XONG: OK");
			}
		}
		finally
		{
			driver?.Quit();
			driver?.Dispose();
		}
	}
}