using FiddlerHandle.Properties;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FiddlerHandle.Common
{
	public class SeleniumCdpHelper
	{
		public OpenQA.Selenium.Chrome.ChromeDriver ChromeDriver
		{
			get;
		}

		public SeleniumCdpHelper(IWebDriver chromeDriver)
		{
			this.ChromeDriver = (OpenQA.Selenium.Chrome.ChromeDriver)chromeDriver;
		}

		public SeleniumCdpHelper(OpenQA.Selenium.Chrome.ChromeDriver chromeDriver)
		{
			this.ChromeDriver = chromeDriver;
		}

		public void AddScriptToEvaluateOnNewDocument(string jsStr)
		{
			this.ExecuteChromeCommand("Page.addScriptToEvaluateOnNewDocument", new Dictionary<string, object>()
			{
				{ "source", jsStr }
			});
		}

		public void ExecuteChromeCommand(string commandName, Dictionary<string, object> commandParameters)
		{
			OpenQA.Selenium.Chrome.ChromeDriver chromeDriver = this.ChromeDriver;
			if (chromeDriver != null)
			{
				chromeDriver.ExecuteChromeCommand(commandName, commandParameters);
			}
			else
			{
			}
		}

		public object ExecuteChromeCommandWithResult(string commandName, Dictionary<string, object> commandParameters)
		{
			object obj;
			OpenQA.Selenium.Chrome.ChromeDriver chromeDriver = this.ChromeDriver;
			if (chromeDriver != null)
			{
				obj = chromeDriver.ExecuteChromeCommandWithResult(commandName, commandParameters);
			}
			else
			{
				obj = null;
			}
			return obj;
		}

		public void WebdriverPingBiJianChe()
		{
			this.AddScriptToEvaluateOnNewDocument(Resources.webdriverPingBi);
		}
	}
}