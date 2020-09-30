using FiddlerHandle;
using NewLife.Log;
using NewLife.Xml;
using OpenQA.Selenium;
using PicDaTi;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FiddlerHandle.AutoWeb
{
	public class HySeleniumWeiBoRegMgr : HySeleniumMgr
	{
		public HySeleniumWeiBoRegMgr()
		{
		}

		protected bool BingPhone(IWebDriver webDriver, string phone, string quhao, out bool has)
		{
			bool flag;
			string str;
			if (webDriver.FinElementAndInput(By.ClassName("input-tel"), phone, null) != null)
			{
				has = true;
				this.SelectAreaCode(webDriver, quhao);
				if (webDriver.FindAndRandomClickElement(webDriver, By.ClassName("get-tbcode")) != null)
				{
					Thread.Sleep(2000);
					Func<string, string, string> getPhoneSms = base.GetPhoneSms;
					if (getPhoneSms != null)
					{
						str = getPhoneSms(phone, quhao);
					}
					else
					{
						str = null;
					}
					if (webDriver.FinElementAndInput(By.ClassName("input-tbcode"), "", null) != null)
					{
					}
					IWebElement btnEl = webDriver.FinElementByAttr(By.ClassName("form-btn"), "eid", "usr/click/confirm/thirdparty_bindphone");
					if (btnEl != null)
					{
						btnEl.Click();
						Thread.Sleep(3000);
						flag = true;
						return flag;
					}
				}
			}
			has = false;
			flag = false;
			return flag;
		}

		public byte[] GetPicCodeData()
		{
			byte[] weiBoYanZhengMa;
			lock (HookMgr.WeiBoYanZhengMaLock)
			{
				weiBoYanZhengMa = HookMgr.WeiBoYanZhengMa;
			}
			return weiBoYanZhengMa;
		}

		private bool IsSetCookiePage(IWebDriver webDriver)
		{
			bool flag;
			flag = (webDriver.Url.IndexOf("udb3lgn.huya.com/web/v2/callback?state=") == -1 ? false : true);
			return flag;
		}

		private bool LoginWeiBo(IWebDriver webDriver, string wbzh, string wbmima)
		{
			bool flag;
			int i = 0;
			while (true)
			{
				if (i < 20)
				{
					webDriver.FinElementAndInput(By.Id("userId"), wbzh, null);
					webDriver.FinElementAndInput(By.Id("passwd"), wbmima, null);
					Thread.Sleep(3000);
					webDriver.FinElementAndInput(By.Id("vcode"), null, () => {
						byte[] tupianByte = this.GetPicCodeData();
						string picCode = base.PicDati(tupianByte);
						PicDaTiHelper.SavePic(tupianByte, "null", picCode, "bmp");
						return picCode;
					});
					if (webDriver.FindAndRandomClickElement(webDriver, By.ClassName("WB_btn_login")) != null)
					{
						if (webDriver.Wait(new Func<By, IWebElement>(webDriver.FindAndClickElement), By.ClassName("WB_btn_link")) == null)
						{
							XTrace.WriteLine("连接按钮找不到,可能已连接到虎牙");
						}
						if (webDriver.Wait(new Func<By, IWebElement>(webDriver.FinElement), By.Id("userId")) == null)
						{
							flag = true;
							break;
						}
						else
						{
							i++;
						}
					}
					else
					{
						XTrace.WriteLine("登录按钮找不到");
						flag = false;
						break;
					}
				}
				else
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		private bool SelectAreaCode(IWebDriver webDriver, string quhao)
		{
			bool flag;
			try
			{
				if (webDriver.FindAndRandomClickElement(webDriver, By.Id("sel-area")) != null)
				{
					string qh = base.Remove0(quhao);
					IWebElement areaEl = webDriver.FinElementByAttr(By.Id("sel-areas-list"), "area-code", qh);
					if (areaEl != null)
					{
						if (areaEl != null)
						{
							areaEl.Click();
						}
						Thread.Sleep(1000);
						flag = true;
						return flag;
					}
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat("选择区号出错 ", exception.Message));
			}
			flag = false;
			return flag;
		}

		public bool WeiBoRegHy(string wbzh, string wbmima, string phone, string quHao)
		{
			bool flag;
			bool hasBindPhone;
			IWebDriver webDriver = base.OpenFirst(phone, XmlConfig<SeleniumConfig>.Current.FirstUrl, "127.0.0.1:8877");
			if (base.TanChuang(webDriver) != null)
			{
				HookMgr.NeedWeiBoYanZhengMa = true;
				if (webDriver.FindAndRandomClickElement(webDriver, By.ClassName("weibo-icon")) != null)
				{
					if (!base.HasLogin(webDriver))
					{
						this.BingPhone(webDriver, phone, quHao, out hasBindPhone);
						if (!hasBindPhone)
						{
							webDriver.SwitchPageByUrl("api.weibo.com/oauth2/authorize?redirect_uri", true);
							this.LoginWeiBo(webDriver, wbzh, wbmima);
						}
					}
					else
					{
						XTrace.WriteLine(string.Concat("账号已经登录成功的 ", wbzh));
					}
					HookMgr.NeedWeiBoYanZhengMa = false;
					flag = false;
				}
				else
				{
					XTrace.WriteLine("第三方注册 微博图标找不到");
					flag = false;
				}
			}
			else
			{
				XTrace.WriteLine("登录弹窗找不到");
				flag = false;
			}
			return flag;
		}
	}
}