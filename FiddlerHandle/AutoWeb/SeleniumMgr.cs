using FiddlerHandle;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace FiddlerHandle.AutoWeb
{
	public class SeleniumMgr
	{
		public SeleniumMgr()
		{
		}

		private void Clos(IWebDriver webDriver)
		{
			if (webDriver != null)
			{
				webDriver.Quit();
			}
		}

		private static IWebDriver CreateChromeDriver()
		{
			IWebDriver webDriver;
			try
			{
				ChromeDriverService service = ChromeDriverService.CreateDefaultService();
				service.HideCommandPromptWindow = true;
				ChromeOptions option = new ChromeOptions();
				option.AddUserProfilePreference("excludeSwitches", new string[] { "enable-automation" });
				ChromeDriver driver = new ChromeDriver(service, option, TimeSpan.FromSeconds(40));
				try
				{
				}
				catch (Exception exception)
				{
					driver.Close();
					driver.Dispose();
				}
				webDriver = driver;
				return webDriver;
			}
			catch (Exception exception1)
			{
			}
			webDriver = null;
			return webDriver;
		}

		public static IWebDriver OpenSetting(IWebDriver webDriver, string url = "chrome://settings/")
		{
			webDriver.Navigate().GoToUrl(url);
			Thread.Sleep(3000);
			return null;
		}

		public static void SetAbout(IWebDriver webDriver)
		{
			IWebElement webElement;
			IWebElement webElement1;
			IWebElement webElement2;
			SeleniumMgr.OpenSetting(webDriver, "chrome://settings/onStartup");
			IWebDriver settingsDriver = webDriver;
			((IJavaScriptExecutor)settingsDriver).ExecuteScript("window.scrollBy(0, 1000000)", Array.Empty<object>());
			Thread.Sleep(2500);
			IWebElement dianji = webDriver.FindElement(By.XPath("/html/body/settings-ui//div[2]/settings-main//settings-basic-page//div[1]/settings-section[6]/settings-on-startup-page//div/settings-radio-group/controlled-radio-button[3]//div[1]/div[1]"));
			if (dianji != null)
			{
				dianji.Click();
			}
			if (settingsDriver != null)
			{
				webElement = settingsDriver.FinElement(By.XPath("//*[@id=\"onStartupRadioGroup\"]"));
			}
			else
			{
				webElement = null;
			}
			IWebElement StartupEl = webElement;
			if (StartupEl != null)
			{
				webElement1 = StartupEl.FinElementByAttr(By.TagName("controlled-radio-button"), "label", "打开特定网页或一组网页");
			}
			else
			{
				webElement1 = null;
			}
			IWebElement teDingEl = webElement1;
			if (teDingEl != null)
			{
				IWebElement teEL = teDingEl.FinElement(By.ClassName("disc-border"));
				if (teEL != null)
				{
					teEL.Click();
					Thread.Sleep(2500);
					if (StartupEl != null)
					{
						webElement2 = StartupEl.FinElement(By.ClassName("list-button"));
					}
					else
					{
						webElement2 = null;
					}
					IWebElement but = webElement2;
					if (but != null)
					{
						but.Click();
						Thread.Sleep(2000);
						IWebElement inputEl = settingsDriver.FinElement(By.Id("input"));
						if (inputEl != null)
						{
							inputEl.Click();
							inputEl.SendKeys("about:blank");
							Thread.Sleep(500);
							IWebElement actButEl = settingsDriver.FindElement(By.Id("actionButton"));
							if (actButEl != null)
							{
								actButEl.Click();
								Thread.Sleep(500);
							}
						}
					}
				}
			}
		}

		private void SetConfig()
		{
			ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
			driverService.HideCommandPromptWindow = true;
			ChromeOptions options = new ChromeOptions();
			options.AddArgument("--disable-gpu");
			options.AddArgument("user-agent=Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1");
			options.AddArgument("--window-size=414,736");
			options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
			IWebDriver webDriver = new ChromeDriver(driverService, options);
			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
			webDriver.Navigate().GoToUrl("https://www.baidu.com");
			((IJavaScriptExecutor)webDriver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", Array.Empty<object>());
			webDriver.FindElements(By.CssSelector("[class='item goWork']"));
		}
	}
}