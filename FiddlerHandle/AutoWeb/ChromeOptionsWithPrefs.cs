using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FiddlerHandle.AutoWeb
{
	public class ChromeOptionsWithPrefs : ChromeOptions
	{
		public Dictionary<string, object> prefs
		{
			get;
			set;
		}

		public ChromeOptionsWithPrefs()
		{
		}
	}
}