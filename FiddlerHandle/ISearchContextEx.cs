using Helper;
using NewLife.Log;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FiddlerHandle
{
	public static class ISearchContextEx
	{
		public static Random Rd;

		static ISearchContextEx()
		{
			Guid guid = Guid.NewGuid();
			ISearchContextEx.Rd = new Random(guid.GetHashCode());
		}

		public static IWebElement FinAndSetAttrVal(this ISearchContext ISearch, IWebDriver jsExec, By by, string attr, string val)
		{
			IWebElement webElement;
			try
			{
				IWebElement el = ISearch.FindElement(by);
				((IJavaScriptExecutor)jsExec).ExecuteScript(string.Concat(new string[] { "arguments[0].setAttribute('", attr, "','", val, "');" }), new object[] { el });
				webElement = el;
				return webElement;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XTrace.WriteLine(string.Concat("FinAndSetAttrVal ", by.ToString(), " ", ex.Message));
			}
			webElement = null;
			return webElement;
		}

		public static IWebElement FinAndUpload(this ISearchContext ISearch, By by, string pach)
		{
			IWebElement webElement;
			try
			{
				IWebElement el = ISearch.FindElement(by);
				el.SendKeys(pach);
				Thread.Sleep(3000);
				webElement = el;
				return webElement;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XTrace.WriteLine(string.Concat("FinAndUpload ", by.ToString(), " ", ex.Message));
			}
			webElement = null;
			return webElement;
		}

		public static void FindAllAndClickElement(this ISearchContext ISearch, By by)
		{
			try
			{
				foreach (IWebElement el in ISearch.FindElements(by))
				{
					try
					{
						if ((!el.Displayed || !el.Enabled ? false : el.Location.X > 0))
						{
							el.Click();
							Thread.Sleep(1500);
						}
					}
					catch (Exception exception)
					{
						Exception ex = exception;
						XTrace.WriteLine(string.Concat(new string[] { "FindAllAndClickElement ", by.ToString(), "  点击出错 ", ex.Message, " " }));
					}
				}
			}
			catch (Exception exception1)
			{
				Exception ex = exception1;
				XTrace.WriteLine(string.Concat(new string[] { "FindAllAndClickElement ", by.ToString(), "  出错 ", ex.Message, " " }));
			}
		}

		public static IWebElement FindAndClickElement(this ISearchContext ISearch, By by)
		{
			IWebElement webElement;
			try
			{
				IWebElement el = ISearch.FindElement(by);
				el.Click();
				Thread.Sleep(1500);
				webElement = el;
				return webElement;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XTrace.WriteLine(string.Concat("FindAndClickElement ", by.ToString(), " 出错 ", ex.Message));
			}
			webElement = null;
			return webElement;
		}

		public static IWebElement FindAndRandomClickElement(this ISearchContext ISearch, IWebDriver webDriver, By by)
		{
			IWebElement webElement;
			try
			{
				IWebElement el = ISearch.FindElement(by);
				el.RandomClick(webDriver);
				Thread.Sleep(2000);
				webElement = el;
				return webElement;
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat("FinElement ", by.ToString(), " 元素找不到"));
			}
			webElement = null;
			return webElement;
		}

		public static IWebElement FindElement(this ISearchContext ISearch, By by, string keyZi)
		{
			IWebElement webElement;
			try
			{
				foreach (IWebElement item in ISearch.FindElements(by))
				{
					if (item.Text.IndexOf(keyZi) != -1)
					{
						webElement = item;
						return webElement;
					}
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat(new string[] { "SelementHelper.FinElement ", by.ToString(), " 元素查找不到 关键字[", keyZi, "] " }));
			}
			webElement = null;
			return webElement;
		}

		public static IWebElement FinElement(this ISearchContext ISearch, By by)
		{
			IWebElement webElement;
			try
			{
				webElement = ISearch.FindElement(by);
				return webElement;
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat("FinElement ", by.ToString(), " 元素找不到"));
			}
			webElement = null;
			return webElement;
		}

		public static IWebElement FinElementAndInput(this ISearchContext ISearch, By by, string str = "", Func<string> GetInputStr = null)
		{
			IWebElement webElement;
			try
			{
				IWebElement el = ISearch.FindElement(by);
				el.Clear();
				Thread.Sleep(300);
				el.Click();
				Thread.Sleep(300);
				if (GetInputStr != null)
				{
					str = GetInputStr();
				}
				el.SendKeys(str);
				Thread.Sleep(2000);
				webElement = el;
				return webElement;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XTrace.WriteLine(string.Concat("FinElement ", by.ToString(), " 元素找不到  ", ex.Message));
			}
			webElement = null;
			return webElement;
		}

		public static IWebElement FinElementAndInput(this ISearchContext ISearch, IWebDriver webDriver, By by, string str = "", Func<string> GetInputStr = null)
		{
			Size size;
			IWebElement webElement;
			int? nullable;
			int? nullable1;
			int? nullable2;
			dynamic py = null;
			IWebElement el = null;
			try
			{
				el = ISearch.FindElement(by);
				el.Clear();
				Thread.Sleep(300);
				if (GetInputStr != null)
				{
					str = GetInputStr();
				}
				Actions actionChains = new Actions(webDriver);
				Random random = HelperGeneral.Random;
				size = el.Size;
				int num = random.Next(3, size.Width);
				Random random1 = HelperGeneral.Random;
				size = el.Size;
				py = new { X = num, Y = random1.Next(3, size.Height) };
				actionChains.MoveToElement(el, py.X, py.Y).Click().SendKeys(str).Perform();
				Thread.Sleep(2000);
				webElement = el;
				return webElement;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				object[] objArray = new object[] { by.ToString(), ex.Message, null, null, null, null };
				dynamic obj = py;
				objArray[2] = (obj != null ? obj.X : null);
				obj = py;
				objArray[3] = (obj != null ? obj.Y : null);
				if (el != null)
				{
					size = el.Size;
					nullable1 = new int?(size.Width);
				}
				else
				{
					nullable = null;
					nullable1 = nullable;
				}
				objArray[4] = nullable1;
				if (el != null)
				{
					size = el.Size;
					nullable2 = new int?(size.Height);
				}
				else
				{
					nullable = null;
					nullable2 = nullable;
				}
				objArray[5] = nullable2;
				XTrace.WriteLine(string.Format("FinElementAndInput {0}  {1} 点击偏移{2},{3} 元素大小{4},{5}", objArray));
			}
			webElement = null;
			return webElement;
		}

		public static IList<IWebElement> FinElementByArrtPartVal(this ISearchContext webDriver, By by, string attribute, string val)
		{
			IList<IWebElement> webElements;
			try
			{
				ReadOnlyCollection<IWebElement> list = webDriver.FindElements(by);
				val = val.Trim();
				IList<IWebElement> listR = new List<IWebElement>();
				foreach (IWebElement item in list)
				{
					string v = item.GetAttribute(attribute);
					if ((v != null ? v.IndexOf(val) != -1 : true))
					{
						listR.Add(item);
					}
				}
				webElements = listR;
				return webElements;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XTrace.WriteLine(string.Concat("FinElementByArrtPartVal ", by.ToString(), " ", ex.Message));
			}
			webElements = null;
			return webElements;
		}

		public static IWebElement FinElementByAttr(this ISearchContext webDriver, By by, string attribute, string val)
		{
			IWebElement webElement;
			try
			{
				ReadOnlyCollection<IWebElement> list = webDriver.FindElements(by);
				val = val.Trim();
				if ((list == null ? true : list.Count <= 0))
				{
					XTrace.WriteLine(string.Concat("FinElementByAttr 元素[", by.ToString(), "] 找不到"));
				}
				else
				{
					foreach (IWebElement item in list)
					{
						if (item.GetAttribute(attribute).Trim() == val)
						{
							webElement = item;
							return webElement;
						}
					}
					XTrace.WriteLine(string.Concat(new string[] { "FinElementByAttr 元素[", by.ToString(), "] 对应属性找不到[", attribute, "] [", val, "] 找不到" }));
				}
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XTrace.WriteLine(string.Concat("FinElementByAttr 元素[", by.ToString(), "] 找不到 ", ex.Message));
			}
			webElement = null;
			return webElement;
		}

		public static IWebElement FinElementByText(this ISearchContext ISearch, By by, string innerText, bool isBuFen = false, By byZi = null)
		{
			IWebElement webElement;
			bool flag;
			bool flag1;
			try
			{
				IWebElement el = ISearch.FindElement(by);
				string test = el.Text.Trim();
				if (test == innerText)
				{
					flag = true;
				}
				else
				{
					flag = (!isBuFen ? false : test.IndexOf(innerText) != -1);
				}
				if (flag)
				{
					webElement = el;
					return webElement;
				}
				else if (byZi != null)
				{
					IWebElement elZi = el.FindElement(byZi);
					test = elZi.Text.Trim();
					if (test == innerText)
					{
						flag1 = true;
					}
					else
					{
						flag1 = (!isBuFen ? false : test.IndexOf(innerText) != -1);
					}
					if (flag1)
					{
						webElement = elZi;
						return webElement;
					}
				}
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XTrace.WriteLine(string.Concat("FinElementByText ", by.ToString(), " ", ex.Message));
			}
			webElement = null;
			return webElement;
		}

		public static IList<IWebElement> FinElements(this ISearchContext webDriver, By by)
		{
			IList<IWebElement> webElements;
			try
			{
				webElements = webDriver.FindElements(by);
				return webElements;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XTrace.WriteLine(string.Concat("FinElementByArrtPartVal ", by.ToString(), " ", ex.Message));
			}
			webElements = null;
			return webElements;
		}

		public static IList<IWebElement> FinElementsByAttr(this ISearchContext webDriver, By by, string attribute, string val)
		{
			ReadOnlyCollection<IWebElement> list = webDriver.FindElements(by);
			val = val.Trim();
			IList<IWebElement> listR = new List<IWebElement>();
			if ((list == null ? true : list.Count <= 0))
			{
				XTrace.WriteLine(string.Concat("FinElementByAttr 元素[", by.ToString(), "] 找不到"));
			}
			else
			{
				foreach (IWebElement item in list)
				{
					if (item.GetAttribute(attribute).Trim() == val)
					{
						listR.Add(item);
					}
				}
				if (listR.Count == 0)
				{
					XTrace.WriteLine(string.Concat(new string[] { "FinElementByAttr 元素[", by.ToString(), "] 对应属性找不到[", attribute, "] [", val, "] 找不到" }));
				}
			}
			return listR;
		}

		public static IWebElement FinHasAttr(this ISearchContext sc, By by, string attr)
		{
			IWebElement webElement;
			try
			{
				IWebElement el = sc.FindElement(by);
				if (!el.GetAttribute(attr).IsNullOrWhiteSpace())
				{
					webElement = el;
					return webElement;
				}
				else
				{
					webElement = null;
					return webElement;
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat("FinHasAttrVal ", exception.Message));
			}
			webElement = null;
			return webElement;
		}

		public static bool FinHasAttrVal(this ISearchContext sc, By by, string attr, string val)
		{
			bool flag;
			try
			{
				if (sc.FindElement(by).GetAttribute(attr).Trim() == val.Trim())
				{
					flag = true;
					return flag;
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat("FinHasAttrVal ", exception.Message));
			}
			flag = false;
			return flag;
		}

		public static string GetTextByElement(this ISearchContext ISearch, By by, By byZi = null)
		{
			string str;
			try
			{
				IWebElement el = ISearch.FindElement(by);
				string test = el.Text.Trim();
				if (test.IsNullOrWhiteSpace())
				{
					if (byZi != null)
					{
						test = el.FindElement(byZi).Text.Trim();
					}
					str = test;
					return str;
				}
				else
				{
					str = test;
					return str;
				}
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XTrace.WriteLine(string.Concat("GetTextByElement ", by.ToString(), " ", ex.Message));
			}
			str = null;
			return str;
		}

		public static bool HasCookie(this IWebDriver webDriver, string key)
		{
			ICookieJar cookies = webDriver.Manage().Cookies;
			Cookie cookie = cookies.GetCookieNamed(key.Trim());
			return ((cookie == null ? true : cookie.Value.IsNullOrWhiteSpace()) ? false : true);
		}

		public static bool RandomClick(this IWebElement element, IWebDriver webDriver)
		{
			bool flag;
			int i = 0;
			while (true)
			{
				if (i < 3)
				{
					try
					{
						Size kg = element.Size;
						var py = new { X = HelperGeneral.Random.Next(0, kg.Width), Y = HelperGeneral.Random.Next(0, kg.Height) };
						Actions act = new Actions(webDriver);
						act.MoveToElement(element, py.X, py.Y).Click().Perform();
						Thread.Sleep(2000);
						XTrace.WriteLine(string.Format("随机点击偏移 {0},{1} 宽高{2} ", py.X, py.Y, kg.ToString()));
						flag = true;
						break;
					}
					catch (Exception exception)
					{
						Exception ex = exception;
						XTrace.WriteLine(string.Concat("RandomClick 错误: ", ex.Message, " "));
					}
					i++;
				}
				else
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		public static void SwitchPageByTitle(this IWebDriver webDriver, string eidTitle)
		{
			string currentWindow = webDriver.CurrentWindowHandle;
			string title = webDriver.Title;
			if (title.IndexOf(eidTitle) == -1)
			{
				ReadOnlyCollection<string> windowsHandles = webDriver.WindowHandles;
				if (windowsHandles != null)
				{
					foreach (string item in windowsHandles.Reverse<string>())
					{
						IWebDriver window = webDriver.SwitchTo().Window(item);
						Thread.Sleep(2000);
						title = window.Title;
						if (title.IndexOf(eidTitle) != -1)
						{
							XTrace.WriteLine(string.Concat("切换到 目标标签页 ", title, " ", item));
							Thread.Sleep(2000);
							break;
						}
					}
				}
			}
			else
			{
				XTrace.WriteLine(string.Concat("已经切换到 目标标签页", title, " ", currentWindow));
			}
		}

		public static bool SwitchPageByUrl(this IWebDriver webDriver, string targetUrl, bool isBuFen = false)
		{
			bool flag;
			bool flag1;
			bool flag2;
			string url = webDriver.Url.Trim();
			if (url == targetUrl)
			{
				flag1 = true;
			}
			else
			{
				flag1 = (!isBuFen ? false : url.IndexOf(targetUrl) != -1);
			}
			if (!flag1)
			{
				ReadOnlyCollection<string> windowsHandles = webDriver.WindowHandles;
				if (windowsHandles != null)
				{
					foreach (string item in windowsHandles.Reverse<string>())
					{
						IWebDriver window = webDriver.SwitchTo().Window(item);
						Thread.Sleep(2000);
						url = window.Url.Trim();
						if (url == targetUrl)
						{
							flag2 = true;
						}
						else
						{
							flag2 = (!isBuFen ? false : url.IndexOf(targetUrl) != -1);
						}
						if (flag2)
						{
							XTrace.WriteLine(string.Concat(new string[] { "切换到 目标标签页 ", item, " ", url, " ", targetUrl }));
							Thread.Sleep(2000);
							flag = true;
							return flag;
						}
					}
				}
				flag = false;
			}
			else
			{
				XTrace.WriteLine(string.Concat("已经切换到 目标标签页 ", webDriver.CurrentWindowHandle));
				flag = true;
			}
			return flag;
		}

		public static TResult Wait<TResult>(this IWebDriver webDriver, Func<IWebDriver, TResult> condition)
		{
			WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
			return wait.Until<TResult>(condition);
		}

		public static IWebElement Wait(this IWebDriver webDriver, Func<By, IWebElement> func, By by)
		{
			IWebElement webElement = webDriver.Wait<IWebElement>((IWebDriver webDr) => func(by));
			return webElement;
		}
	}
}