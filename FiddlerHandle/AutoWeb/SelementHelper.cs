using CsharpHttpHelper;
using FiddlerHandle.Properties;
using Helper;
using NewLife.Log;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace FiddlerHandle.AutoWeb
{
	public class SelementHelper
	{
		public SelementHelper()
		{
		}

		public static void AddCookies(IWebDriver webDriver, CookieCollection Coookie)
		{
			DateTime now;
			ICookieJar listCookie = webDriver.Manage().Cookies;
			foreach (System.Net.Cookie item in Coookie)
			{
				OpenQA.Selenium.Cookie yuan = listCookie.GetCookieNamed(item.Name);
				if (yuan == null)
				{
					item.Domain = ".huya.com";
					now = DateTime.Now;
					item.Expires = now.AddDays(365);
				}
				else
				{
					if (!yuan.Expiry.HasValue)
					{
						now = DateTime.Now;
						item.Expires = now.AddDays(365);
					}
					else
					{
						item.Expires = yuan.Expiry.Value;
					}
					item.Domain = yuan.Domain;
					listCookie.DeleteCookieNamed(yuan.Name);
				}
				try
				{
					OpenQA.Selenium.Cookie newCookie = new OpenQA.Selenium.Cookie(item.Name, item.Value, item.Domain, "/", new DateTime?(item.Expires));
					listCookie.AddCookie(newCookie);
				}
				catch (Exception exception)
				{
					XTrace.WriteLine(string.Concat("设置cookie 出错 ", exception.Message));
				}
			}
		}

		public static void AddCookies(IWebDriver webDriver, string Coookie)
		{
			SelementHelper.AddCookies(webDriver, HttpHelper.StrCookieToCookieCollection(Coookie));
		}

		public static void ClearCookies(IWebDriver webDriver)
		{
			ICookieJar list = webDriver.Manage().Cookies;
			foreach (OpenQA.Selenium.Cookie item in list.AllCookies)
			{
				list.DeleteCookieNamed(item.Name);
			}
			XTrace.WriteLine(string.Format("清空cookie {0}", list.AllCookies.Count));
		}

		public static void CloseExecHuiFu()
		{
			Process process = new Process();
			process.StartInfo.FileName = string.Concat(Environment.CurrentDirectory, "\\AutoIT3\\google要恢复页面吗.exe");
			process.StartInfo.CreateNoWindow = true;
			process.Start();
		}

		public static object ExecJs(IWebDriver webDriver, string script, params object[] args)
		{
			return ((IJavaScriptExecutor)webDriver).ExecuteScript(script, args);
		}

		public static IWebElement FinElementByClassName(IWebDriver webDriver, string className, string attribute, string val)
		{
			IWebElement webElement;
			ReadOnlyCollection<IWebElement> list = webDriver.FindElements(By.ClassName(className));
			val = val.Trim();
			foreach (IWebElement item in list)
			{
				if (item.GetAttribute(attribute).Trim() == val)
				{
					webElement = item;
					return webElement;
				}
			}
			webElement = null;
			return webElement;
		}

		public static IWebElement FinElementByClassName(IWebElement webDriver, string className, string attribute, string val)
		{
			IWebElement webElement;
			ReadOnlyCollection<IWebElement> list = webDriver.FindElements(By.ClassName(className));
			val = val.Trim();
			foreach (IWebElement item in list)
			{
				if (item.GetAttribute(attribute).Trim() == val)
				{
					webElement = item;
					return webElement;
				}
			}
			webElement = null;
			return webElement;
		}

		public static string GetCookiesSmallStr(IWebDriver webDriver)
		{
			string str;
			try
			{
				webDriver.SwitchTo().DefaultContent();
				ICookieJar list = webDriver.Manage().Cookies;
				if (list != null)
				{
					string cookiesStr = "";
					foreach (OpenQA.Selenium.Cookie item in list.AllCookies)
					{
						cookiesStr = string.Concat(new string[] { cookiesStr, item.Name, "=", item.Value, ";" });
					}
					XTrace.WriteLine(string.Concat("取得cookie ", cookiesStr));
					str = cookiesStr;
					return str;
				}
				else
				{
					str = null;
					return str;
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat("取cookies出错", exception.ToString()));
			}
			str = null;
			return str;
		}

		public static string GetJqmStr(IWebDriver webDriver, params object[] args)
		{
			string str;
			try
			{
				webDriver.SwitchTo().DefaultContent();
				Thread.Sleep(1000);
				SelementHelper.ExecJs(webDriver, Resources.hydevice_8290851_Get, Array.Empty<object>());
				str = (string)SelementHelper.ExecJs(webDriver, "return getCommmonInfo2();", Array.Empty<object>());
				return str;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XTrace.WriteLine(string.Concat("执行脚本获取机器码信息出错 ", ex.Message, "   有可能时脚本还没初始化,执行流程不要太快"));
			}
			str = null;
			return str;
		}

		public static void MoveTo(IWebDriver webDriver, IWebElement element)
		{
			try
			{
				Actions actions = new Actions(webDriver);
				if (element != null)
				{
					actions.MoveToElement(element).Perform();
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat("鼠标移动到元素 ", exception.Message));
			}
		}

		public static void MoveTo(IWebDriver webDriver, IWebElement element, int offsetX, int offsetY)
		{
			if (element != null)
			{
				(new Actions(webDriver)).MoveToElement(element, offsetX, offsetY).Perform();
			}
		}

		public static void MoveTo(IWebDriver webDriver, IWebElement element, IWebElement toElement, int offsetX, int offsetY)
		{
			if (element != null)
			{
				Actions actions = new Actions(webDriver);
				Console.WriteLine("请.....");
				Console.ReadLine();
				int X = offsetX;
				actions.ClickAndHold(element).MoveToElement(toElement, X, offsetY).Release().Perform();
				actions.ClickAndHold(element).MoveToElement(toElement, X, offsetY).Perform();
			}
		}

		public static void MoveTo(IWebDriver webDriver, IWebElement element, IWebElement toElement, int ysX, int ysY, int sX, int sY)
		{
			if (element != null)
			{
				Actions actions = new Actions(webDriver);
				actions.MoveToElement(element, ysX, ysY).Perform();
				actions.ClickAndHold().Perform();
				Thread.Sleep(HelperGeneral.Random.Next(200, 500));
				actions.MoveToElement(toElement, sX, sY).Perform();
				Thread.Sleep(HelperGeneral.Random.Next(200, 500));
				actions.Release().Perform();
			}
		}

		public static void ShowCookies(ICookieJar listCookies)
		{
			ReadOnlyCollection<OpenQA.Selenium.Cookie> allCookies = listCookies.AllCookies;
			XTrace.WriteLine(string.Format("当前Cookie数量{0}", allCookies.Count));
			foreach (OpenQA.Selenium.Cookie item in allCookies)
			{
				XTrace.WriteLine(string.Concat(new string[] { "可以cookies ", item.Name, " ", item.Value, " ", item.Domain, " ", item.Path }));
			}
		}

		public static void Upload(string pic)
		{
			Process process = new Process();
			process.StartInfo.FileName = string.Concat(Environment.CurrentDirectory, "\\AutoIT3\\goolge上传图片.exe");
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.Arguments = string.Concat("chrome ", pic);
			process.Start();
		}
	}
}