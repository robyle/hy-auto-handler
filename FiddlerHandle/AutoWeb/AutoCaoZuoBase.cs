using FiddlerHandle;
using Helper;
using Huya.Data.Entity;
using HyBase.Web;
using NewLife.Log;
using NewLife.Xml;
using OpenQA.Selenium;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using ThirdParty.Mobiles;
using ThirdParty.Model;
using XCode;
using XS.Helper.StringY;

namespace FiddlerHandle.AutoWeb
{
	public class AutoCaoZuoBase
	{
		public Action EndAction
		{
			get;
			set;
		}

		protected FiddlerHandle.AutoWeb.HySeleniumMgr HySeleniumMgr
		{
			get;
			set;
		}

		public Action StartAction
		{
			get;
			set;
		}

		public AutoCaoZuoBase()
		{
		}

		public void Close()
		{
			FiddlerHandle.AutoWeb.HySeleniumMgr hySeleniumMgr = this.HySeleniumMgr;
			if (hySeleniumMgr != null)
			{
				hySeleniumMgr.Close();
			}
			else
			{
			}
		}

		public string CratePasswor()
		{
			string password = string.Concat(HelperText.CreateRandomString(HelperText.UPCase, 2), string.Format("{0}{1}", HelperText.CreateRandomString(HelperText.LowerCase, 8), HelperGeneral.Random.Next(999, 89999)));
			return password;
		}

		public void GeRegJqm()
		{
			HookMgr.jiQiMa = HyDeviceMgr.Ins.GetRandomJiQiMa();
		}

		protected void GetCookiesAndSaveToDb(HyAccount account)
		{
			string str;
			Console.Title = string.Concat("取cookie ", Console.Title);
			string cookiesStr = SelementHelper.GetCookiesSmallStr(this.HySeleniumMgr.WebDriver);
			if ((cookiesStr != null ? cookiesStr.IndexOf("udb_biztoken") == -1 : false))
			{
				account.WebCookie = account.WebCookie ?? cookiesStr;
			}
			else
			{
				account.LoginSucceed = true;
				account.Uid = HelperText.getMiddleStr(string.Concat(cookiesStr, ";"), "yyuid=", ";", 0).ToLong((long)0);
				string middleStr = HelperText.getMiddleStr(string.Concat(cookiesStr, ";"), "guid=", ";", 0);
				if (middleStr != null)
				{
					str = middleStr.Trim();
				}
				else
				{
					str = null;
				}
				account.Guid = str ?? account.Guid;
				account.WebCookie = cookiesStr ?? account.WebCookie;
				XTrace.WriteLine(string.Concat(account.UserName, " ", account.UserPass, " 有登录成功状态"));
			}
			account.Save();
		}

		public void OneByOneTaskTryErr(Action<IWebDriver> task, FiddlerHandle.AutoWeb.HySeleniumMgr hySeleniumMgr)
		{
			try
			{
				if (task != null)
				{
					task(hySeleniumMgr.WebDriver);
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(exception.ToString());
			}
		}

		public void OneByOneTaskTryErr(Action<IWebDriver, int> task, FiddlerHandle.AutoWeb.HySeleniumMgr hySeleniumMgr, int arg2)
		{
			try
			{
				if (task != null)
				{
					task(hySeleniumMgr.WebDriver, arg2);
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(exception.ToString());
			}
		}

		public void OneByOneTaskTryErr(Action<IWebDriver, string> task, FiddlerHandle.AutoWeb.HySeleniumMgr hySeleniumMgr, string arg2)
		{
			try
			{
				if (task != null)
				{
					task(hySeleniumMgr.WebDriver, arg2);
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(exception.ToString());
			}
		}

		public void OneByOneTaskTryErrF(Func<IWebDriver, string, bool> task, FiddlerHandle.AutoWeb.HySeleniumMgr hySeleniumMgr, string arg2)
		{
			try
			{
				if (task != null)
				{
					task(hySeleniumMgr.WebDriver, arg2);
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(exception.ToString());
			}
		}

		public void SetPhoneCodeCallBack(MobileMgr mobileMgr, MobileModel mobileModel)
		{
			this.HySeleniumMgr.GetPhoneSms = (string phone, string quHao) => {
				string sms = MobileMgr.GetYzm(mobileMgr.getVcodeAndHoldMobilenum(mobileModel), 6);
				if (sms.IsNullOrWhiteSpace())
				{
					mobileMgr.cancelSMSRecv(mobileModel.PhoneNumber);
				}
				return sms;
			};
		}

		protected void ToGeRenZhongXin(IWebDriver webDriver)
		{
			if (webDriver.Url.Trim().IndexOf("i.huya.com") == -1)
			{
				if (webDriver.FindAndRandomClickElement(webDriver, By.ClassName("nav-user-title")) == null)
				{
					webDriver.Navigate().GoToUrl("https://i.huya.com");
				}
				Thread.Sleep(5000);
				webDriver.SwitchPageByUrl("i.huya.com", true);
			}
		}

		protected void ZHInitCaoZuo()
		{
			if (XmlConfig<SeleniumConfig>.Current.XiuGaiNick)
			{
				this.ToGeRenZhongXin(this.HySeleniumMgr.WebDriver);
				string nick = HelperText.RdMaJia(5, false);
				this.OneByOneTaskTryErrF(new Func<IWebDriver, string, bool>(this.HySeleniumMgr.XiuGaiNick), this.HySeleniumMgr, nick);
			}
			if (XmlConfig<SeleniumConfig>.Current.XiuGaiTouXiang)
			{
				this.ToGeRenZhongXin(this.HySeleniumMgr.WebDriver);
				FileInfo pic = ModifyHead.GetHeadRandomPath();
				if (pic != null)
				{
					this.OneByOneTaskTryErrF(new Func<IWebDriver, string, bool>(this.HySeleniumMgr.XiuGaiTouXiang), this.HySeleniumMgr, pic.FullName);
				}
			}
		}

		protected void ZhuCeLoginAfter()
		{
			Console.Title = string.Concat("随机点击一个分类 ", Console.Title);
			this.OneByOneTaskTryErr(new Action<IWebDriver>(this.HySeleniumMgr.RandomOpenFeiLei), this.HySeleniumMgr);
			Console.Title = string.Concat("到房间逛一下 ", Console.Title);
			this.OneByOneTaskTryErr(new Action<IWebDriver>(this.HySeleniumMgr.InRoom), this.HySeleniumMgr);
			Console.Title = string.Concat("发送弹幕 ", Console.Title);
			this.OneByOneTaskTryErr(new Action<IWebDriver, string>(this.HySeleniumMgr.SendDanWu), this.HySeleniumMgr, "我来拉 6666");
			if (XmlConfig<SeleniumConfig>.Current.Subscribe)
			{
				Console.Title = string.Concat("订阅 ", Console.Title);
				this.OneByOneTaskTryErr(new Action<IWebDriver>(this.HySeleniumMgr.Subscribe), this.HySeleniumMgr);
			}
			if (XmlConfig<SeleniumConfig>.Current.SendHyLiangGift)
			{
				Console.Title = string.Concat("发送虎粮 ", Console.Title);
				this.OneByOneTaskTryErr(new Action<IWebDriver, int>(this.HySeleniumMgr.SendGift), this.HySeleniumMgr, 4);
			}
			if (XmlConfig<SeleniumConfig>.Current.LingTask)
			{
				Console.Title = string.Concat("领任务 ", Console.Title);
				this.OneByOneTaskTryErr(new Action<IWebDriver>(this.HySeleniumMgr.TouXiangTask), this.HySeleniumMgr);
			}
			if (XmlConfig<SeleniumConfig>.Current.LingBaoXiang)
			{
				Console.Title = string.Concat("领宝箱 ", Console.Title);
				this.OneByOneTaskTryErr(new Action<IWebDriver>(this.HySeleniumMgr.BaoXiangShow), this.HySeleniumMgr);
			}
		}
	}
}