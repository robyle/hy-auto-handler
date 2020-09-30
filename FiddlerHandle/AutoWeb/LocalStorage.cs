using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FiddlerHandle.AutoWeb
{
	public class LocalStorage
	{
		private IJavaScriptExecutor driver
		{
			get;
			set;
		}

		public LocalStorage(IWebDriver _driver)
		{
			this.driver = (IJavaScriptExecutor)_driver;
		}

		public void clear()
		{
			this.driver.ExecuteScript("window.localStorage.clear();", Array.Empty<object>());
		}

		public object @get(string key)
		{
			return this.driver.ExecuteScript("return window.localStorage.getItem(arguments[0]);", new object[] { key });
		}

		public bool has(string key)
		{
			bool flag;
			flag = (this.items()[key] == null ? false : true);
			return flag;
		}

		public Dictionary<string, object> items()
		{
			object JsonStr = this.driver.ExecuteScript("var ls = window.localStorage, items = {};for (var i = 0, k; i < ls.length; ++i) { items[k = ls.key(i)] = ls.getItem(k);} ;return items;", Array.Empty<object>());
			return (Dictionary<string, object>)JsonStr;
		}

		public IList<string> keys()
		{
			object list = this.driver.ExecuteScript("var ls = window.localStorage, keys = []; for (var i = 0; i < ls.length; ++i)  {keys[i] = ls.key(i);} return keys; ", Array.Empty<object>());
			return (IList<string>)list;
		}

		public long len()
		{
			object le = this.driver.ExecuteScript("return window.localStorage.length;", Array.Empty<object>());
			return le.ToLong((long)0);
		}

		public void @remove(string key)
		{
			this.driver.ExecuteScript("window.localStorage.removeItem(arguments[0]);", new object[] { key });
		}

		public void @set(string key, string value)
		{
			this.driver.ExecuteScript("window.localStorage.setItem(arguments[0], arguments[1]);", new object[] { key, value });
		}
	}
}