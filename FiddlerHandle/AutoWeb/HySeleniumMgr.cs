using DotNet.Utilities;
using FiddlerHandle;
using FiddlerHandle.AutoWeb.Other;
using FiddlerHandle.Common;
using Helper;
using Helper.TuPian;
using Huya.Data.Entity;
using HyBase.Web;
using NewLife.Log;
using NewLife.Xml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using PicDaTi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using XS.Helper.StringY;

namespace FiddlerHandle.AutoWeb
{
	public class HySeleniumMgr
	{
		public SeleniumCdpHelper ChromeDriverCdp
		{
			get;
			private set;
		}

		public OpenQA.Selenium.DriverService DriverService
		{
			get;
			set;
		}

		public Func<string, string, string> GetPhoneSms
		{
			get;
			set;
		}

		public static bool OpenProxy
		{
			private get;
			set;
		}

		private string Phone
		{
			get;
			set;
		}

		private string QuHao
		{
			get;
			set;
		}

		public IWebDriver WebDriver
		{
			get;
			set;
		}

		public HySeleniumMgr()
		{
		}

		public void BaoXiangShow(IWebDriver webDriver)
		{
			IWebElement giftElement = webDriver.FinElement(By.ClassName("room-player-gift-placeholder"));
			if (giftElement != null)
			{
				if (XmlConfig<SeleniumConfig>.Current.LingBaoXiangWaitingTime > 0)
				{
					XTrace.WriteLine(string.Format("等待[{0}]分钟,领取宝箱", XmlConfig<SeleniumConfig>.Current.LingBaoXiangWaitingTime));
					Thread.Sleep(XmlConfig<SeleniumConfig>.Current.LingBaoXiangWaitingTime * 60 * 1000);
					giftElement = webDriver.FinElement(By.ClassName("room-player-gift-placeholder"));
					if (giftElement == null)
					{
						return;
					}
				}
				SelementHelper.MoveTo(webDriver, giftElement, 33, 31);
				Thread.Sleep(3000);
				if (webDriver.FinElement(By.ClassName("player-box-list")) != null)
				{
					Actions actions = new Actions(webDriver);
					IList<IWebElement> listEl = webDriver.FinElementsByAttr(By.ClassName("player-box-stat3"), "style", "visibility: visible;");
					foreach (IWebElement item in listEl)
					{
						actions.MoveToElement(item).Click().Perform();
						Thread.Sleep(1000);
					}
					SelementHelper.MoveTo(webDriver, giftElement);
					Thread.Sleep(500);
				}
			}
		}

		private bool BtnAccountLoginTilte(IWebDriver UDBiframeDriver)
		{
			bool flag;
			try
			{
				UDBiframeDriver.FindAndClickElement(By.ClassName("input-login"));
				IWebElement element = UDBiframeDriver.FinElementByText(By.Id("login-head-nav"), "账号登录", false, By.TagName("li"));
				if (element != null)
				{
					element.Click();
					Thread.Sleep(2000);
					flag = true;
					return flag;
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat("BtnAccountLoginTilte 点击 登录 弹窗的 账号登陆 标签 ", exception.Message));
			}
			flag = false;
			return flag;
		}

		private bool BtnSMSLoginTilte(IWebDriver UDBiframeDriver)
		{
			bool flag;
			IWebElement element = UDBiframeDriver.FinElement(By.LinkText("手机验证码登录"));
			if (element == null)
			{
				flag = false;
			}
			else
			{
				element.Click();
				flag = true;
			}
			return flag;
		}

		private void CachConfig(ChromeOptions options, string zh)
		{
			string cache = string.Concat(XmlConfig<SeleniumConfig>.Current.GoogleUserCache, "\\", zh);
			FileOperateHelper.FolderCreate(cache);
			options.AddArgument(string.Concat("--user-data-dir=", cache, "\\cookie"));
			options.AddArgument(string.Concat("--disk-cache-dir=", cache));
		}

		public void ClearCach(string zh)
		{
			string cache = string.Concat(XmlConfig<SeleniumConfig>.Current.GoogleUserCache, "\\", zh);
			FileOperateHelper.DeleteFolder(cache);
		}

		internal void Close()
		{
			try
			{
				if (this.WebDriver != null)
				{
					IWebDriver webDriver = this.WebDriver;
					if (webDriver != null)
					{
						webDriver.Close();
					}
					else
					{
					}
					Thread.Sleep(1000);
				}
			}
			catch (Exception exception)
			{
			}
			try
			{
				IWebDriver webDriver1 = this.WebDriver;
				if (webDriver1 != null)
				{
					webDriver1.Quit();
				}
				else
				{
				}
				IWebDriver webDriver2 = this.WebDriver;
				if (webDriver2 != null)
				{
					webDriver2.Dispose();
				}
				else
				{
				}
				OpenQA.Selenium.DriverService driverService = this.DriverService;
				if (driverService != null)
				{
					driverService.Dispose();
				}
				else
				{
				}
			}
			catch (Exception exception1)
			{
			}
		}

		private void Cookie(IWebDriver webDriver)
		{
			ICookieJar listCookies = webDriver.Manage().Cookies;
		}

		private void GetTask(IWebDriver webDriver)
		{
			IWebElement taskEl = webDriver.FinElement(By.ClassName("tasks"));
			if (taskEl != null)
			{
				ReadOnlyCollection<IWebElement> getEl = taskEl.FindElements(By.ClassName("J_get"));
				if (getEl == null)
				{
					XTrace.WriteLine("没有可以领取的任务");
				}
				else
				{
					foreach (IWebElement item in getEl)
					{
						(new Actions(webDriver)).MoveToElement(item).Click().Perform();
						Thread.Sleep(1000);
					}
				}
			}
		}

		public bool HasLogin(IWebDriver webDriver)
		{
			bool flag;
			webDriver.SwitchTo().DefaultContent();
			bool ret = webDriver.HasCookie("udb_biztoken");
			if (!ret)
			{
				flag = ret;
			}
			else if (webDriver.FinHasAttr(By.Id("login-username"), "title") == null)
			{
				flag = false;
			}
			else
			{
				XTrace.WriteLine("找到 udb_biztoken 和 名字  证明已经登录成功的");
				flag = true;
			}
			return flag;
		}

		public void InRoom(IWebDriver webDriver)
		{
			IWebElement list = webDriver.FinElement(By.Id("js-live-list"));
			if (list != null)
			{
				IList<IWebElement> listRoomElement = list.FinElementByArrtPartVal(By.ClassName("new-clickstat"), "report", "click/position");
				if ((listRoomElement == null ? true : listRoomElement.Count == 0))
				{
					listRoomElement = list.FinElements(By.ClassName("video-info"));
				}
				if ((listRoomElement == null ? true : listRoomElement.Count <= 0))
				{
					XTrace.WriteLine("没有房间列表数据,请联系开发");
				}
				else
				{
					int i = HelperGeneral.Random.Next(0, listRoomElement.Count - 1);
					string targetHref = listRoomElement[i].GetAttribute("href").Trim();
					listRoomElement[i].Click();
					Thread.Sleep(12000);
					webDriver.SwitchPageByUrl(targetHref, false);
				}
			}
		}

		protected bool JiYanYanZheng(IWebDriver webDriver, ref IWebDriver IframeElWebDriver)
		{
			bool flag;
			bool isSuc = false;
			int i = 0;
			while (true)
			{
				if (i >= 20)
				{
					flag = isSuc;
					break;
				}
				else if (IframeElWebDriver.FinElementByText(By.ClassName("captcha-title"), "请拖动下方滑块完成拼图", true, null) != null)
				{
					isSuc = (new HyJiYanCaoZuo()).Run(IframeElWebDriver);
					i++;
				}
				else
				{
					flag = isSuc;
					break;
				}
			}
			return flag;
		}

		public bool LoginHy(string zh, string mima, IWebDriver webDriver)
		{
			bool flag;
			IWebElement webElement;
			IWebDriver UDBiframeDriver = this.TanChuang(webDriver);
			if (UDBiframeDriver != null)
			{
				this.BtnAccountLoginTilte(UDBiframeDriver);
				UDBiframeDriver.FinElementAndInput(By.ClassName("udb-input-account"), zh, null);
				UDBiframeDriver.FinElementAndInput(By.ClassName("udb-input-pw"), mima, null);
				if (UDBiframeDriver.FindAndRandomClickElement(webDriver, By.Id("login-btn")) != null)
				{
					Thread.Sleep(8000);
					if (!this.PicCodeProcess(webDriver, true))
					{
						this.WanShangZhiLiao(webDriver);
						flag = true;
					}
					else
					{
						UDBiframeDriver = this.TanChuang(webDriver);
						if (UDBiframeDriver != null)
						{
							webElement = UDBiframeDriver.FindAndRandomClickElement(webDriver, By.Id("login-btn"));
						}
						else
						{
							webElement = null;
						}
						if (webElement == null)
						{
							XTrace.WriteLine("虎牙.账号登录.再次点击登录按钮找不到");
						}
						Thread.Sleep(3000);
						this.WanShangZhiLiao(webDriver);
						flag = true;
					}
				}
				else
				{
					XTrace.WriteLine("虎牙.账号登录.登录按钮找不到");
					flag = false;
				}
			}
			else
			{
				XTrace.WriteLine("登录弹窗定位不到");
				flag = false;
			}
			return flag;
		}

		private bool mobileCodeAuth(IWebDriver webDriver)
		{
			bool flag;
			string phone;
			IWebElement el = webDriver.FindAndClickElement(By.ClassName("get-tbcode"));
			if (el != null)
			{
				Thread.Sleep(3000);
				el = webDriver.FindAndClickElement(By.ClassName("get-tbcode"));
				if (el == null)
				{
					flag = false;
				}
				else if (el.Text.IndexOf("重新获取") != -1)
				{
					Func<string, string, string> getPhoneSms = this.GetPhoneSms;
					if (getPhoneSms != null)
					{
						phone = getPhoneSms(this.Phone, this.QuHao);
					}
					else
					{
						phone = null;
					}
					string code = phone;
					if (!code.IsNullOrWhiteSpace())
					{
						int ii = 0;
						while (ii < 10)
						{
							if (webDriver.FinElementAndInput(webDriver, By.ClassName("input-security"), code, null) == null)
							{
								XTrace.WriteLine("手机验证码.输入验证码 找不到");
							}
							if (webDriver.FindAndRandomClickElement(webDriver, By.ClassName("form-btn")) != null)
							{
								Thread.Sleep(5000);
							}
							else
							{
								XTrace.WriteLine("手机验证码.确定 找不到");
							}
							if (webDriver.FinElement(By.Id("layui-layer1")) != null)
							{
								ii++;
							}
							else
							{
								flag = true;
								return flag;
							}
						}
						flag = false;
						return flag;
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
				return flag;
			}
			return flag;
		}

		public IWebDriver OpenFirst(string zh, string Url, string proxyIpAndPort = "127.0.0.1:8877")
		{
			ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
			this.DriverService = driverService;
			driverService.HideCommandPromptWindow = true;
			ChromeOptionsWithPrefs options = new ChromeOptionsWithPrefs();
			options.AddArgument("--start-maximized");
			this.CachConfig(options, zh);
			if (HySeleniumMgr.OpenProxy)
			{
				this.SetProxy(options, proxyIpAndPort);
			}
			options.AddUserProfilePreference("credentials_enable_service", false);
			options.AddUserProfilePreference("profile.password_manager_enabled", false);
			options.AddArgument("disable-sync-passwords");
			options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
			options.AddExcludedArgument("enable-automation");
			options.AddAdditionalCapability("useAutomationExtension", false);
			ChromeDriver chromeDriver = new ChromeDriver(driverService, options);
			IWebDriver webDriver1 = chromeDriver;
			this.WebDriver = chromeDriver;
			IWebDriver webDriver = webDriver1;
			SeleniumCdpHelper seleniumCdpHelper = new SeleniumCdpHelper(webDriver);
			SeleniumCdpHelper seleniumCdpHelper1 = seleniumCdpHelper;
			this.ChromeDriverCdp = seleniumCdpHelper;
			seleniumCdpHelper1.WebdriverPingBiJianChe();
			webDriver.Navigate().GoToUrl(Url);
			Thread.Sleep(5000);
			SelementHelper.CloseExecHuiFu();
			return webDriver;
		}

		public bool OpenLogin(IWebDriver webDriver)
		{
			bool flag;
			webDriver.SwitchTo().DefaultContent();
			if (webDriver.FindAndRandomClickElement(webDriver, By.Id("nav-login")) == null)
			{
				flag = false;
			}
			else
			{
				Thread.Sleep(3000);
				flag = true;
			}
			return flag;
		}

		private bool PicCodeProcess(IWebDriver webDriver, bool hasDuanXin)
		{
			bool flag;
			IWebDriver UDBiframeDriver = null;
			int hasNoCount = 0;
			int w = 0;
			while (true)
			{
				if (w < 20)
				{
					UDBiframeDriver = this.TanChuang(webDriver);
					IWebElement elemFram = null;
					elemFram = webDriver.FinElement(By.Id("layui-layer-iframe1")) ?? webDriver.FinElement(By.Id("layui-layer-iframe2")) ?? webDriver.FinElement(By.Id("tcaptcha_iframe")) ?? webDriver.FinElement(By.Id("layui-layer-iframe1"));
					if (elemFram != null)
					{
						UDBiframeDriver = webDriver.SwitchTo().Frame(elemFram);
						Thread.Sleep(5000);
					}
					this.JiYanYanZheng(webDriver, ref UDBiframeDriver);
					QQVTTYanZhengServer.Ins.VTTShiBieGuoCheng(webDriver);
					HyTuBiaoYanZhengServer.Ins.ShiBieGuoCheng(webDriver);
					if (UDBiframeDriver != null)
					{
						if ((UDBiframeDriver.FinElement(By.Id("mobile-mask")) != null ? false : UDBiframeDriver.FinElement(By.ClassName("sms-wrap")) == null))
						{
							if (hasDuanXin)
							{
								this.mobileCodeAuth(webDriver);
							}
							IWebElement picCodeElem = UDBiframeDriver.FinElement(By.Id("pic-code"));
							if (picCodeElem != null)
							{
								string code = "";
								for (int i = 0; i < 5; i++)
								{
									picCodeElem = UDBiframeDriver.FinElement(By.Id("pic-code"));
									if (picCodeElem != null)
									{
										byte[] tupianByte = ImageHelper.Base64ToImage(picCodeElem.GetAttribute("src"));
										if (tupianByte != null)
										{
											code = this.PicDati(tupianByte);
											if ((code != null ? code.Length == 4 : false))
											{
												break;
											}
										}
										picCodeElem.Click();
										Thread.Sleep(5000);
									}
								}
								if (UDBiframeDriver.FinElementAndInput(UDBiframeDriver, By.ClassName("input-security"), code, null) == null)
								{
									XTrace.WriteLine("图片验证码输入框找不到");
								}
								if (UDBiframeDriver.FindAndRandomClickElement(UDBiframeDriver, By.ClassName("form-btn")) == null)
								{
									XTrace.WriteLine("图片验证码.确定按钮找不到");
								}
								else
								{
									Thread.Sleep(2000);
								}
							}
						}
						else
						{
							HookMgr.PhoneIsReg = true;
							flag = false;
							break;
						}
					}
					if (UDBiframeDriver.FinElement(By.ClassName("input-security")) == null)
					{
						if (hasNoCount <= 1)
						{
							Thread.Sleep(3000);
							hasNoCount++;
						}
						else
						{
							webDriver.SwitchTo().DefaultContent();
							flag = true;
							break;
						}
					}
					Thread.Sleep(100);
					w++;
				}
				else
				{
					webDriver.SwitchTo().DefaultContent();
					flag = false;
					break;
				}
			}
			return flag;
		}

		protected string PicDati(byte[] picbyte)
		{
			string str;
			try
			{
				int i = 0;
				while (i < 2)
				{
					string orcSms = PicDaTiHelper.OcrTuPic(picbyte);
					XTrace.WriteLine(string.Concat("答题结果 [", orcSms, "]"));
					if (orcSms == "-10")
					{
						i++;
					}
					else
					{
						Thread.Sleep(2000);
						str = orcSms;
						return str;
					}
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteException(exception);
			}
			str = null;
			return str;
		}
		public bool QuickRegisterHy(string phone, string mima, string quHao)
		{
			IWebDriver webDriver = this.OpenFirst(phone, XmlConfig<SeleniumConfig>.Current.FirstUrl, "127.0.0.1:8877");
			IWebDriver UDBiframeDriver = this.TanChuang(webDriver);
			bool flag = UDBiframeDriver == null;
			bool result;
			if (flag)
			{
				XTrace.WriteLine("弹窗定位不到");
				result = false;
			}
			else
			{
				this.BtnAccountLoginTilte(UDBiframeDriver);
				IWebElement regTitleElement = UDBiframeDriver.FinElementByAttr(By.ClassName("go-register"), "eid", "Click/AccountLogin/Register");
				bool flag2 = regTitleElement != null && regTitleElement.Displayed;
				if (flag2)
				{
					if (regTitleElement != null)
					{
						regTitleElement.Click();
					}
					Thread.Sleep(3000);
				}
				else
				{
					regTitleElement = UDBiframeDriver.FinElementByAttr(By.ClassName("go-register"), "eid", "Click/SMSLogin/Register");
					bool flag3 = regTitleElement != null && regTitleElement.Displayed;
					if (!flag3)
					{
						XTrace.WriteLine("跳转到快速注册元素找不到");
						return false;
					}
					if (regTitleElement != null)
					{
						regTitleElement.Click();
					}
					Thread.Sleep(3000);
				}
				IWebElement registerFormElem = UDBiframeDriver.FinElement(By.Id("register-form"));
				bool flag4 = registerFormElem == null;
				if (flag4)
				{
					XTrace.WriteLine("快速注册窗口表单找不到");
					result = false;
				}
				else
				{
					IWebElement quHaoElement = registerFormElem.FinElementByAttr(By.ClassName("udb-form-sel"), "eid", "Click/Register/AreaCode");
					bool flag5 = quHaoElement == null;
					if (flag5)
					{
						XTrace.WriteLine("快速注册窗口,国家区号下拉按钮找不到");
						result = false;
					}
					else
					{
						quHaoElement.RandomClick(webDriver);
						Thread.Sleep(2000);
						this.SelectQuHao(registerFormElem, quHao);
						IWebElement inputPhoen = registerFormElem.FinElement(By.ClassName("udb-input-tel"));
						bool flag6 = inputPhoen != null;
						if (flag6)
						{
							inputPhoen.Click();
							Thread.Sleep(1000);
							inputPhoen.SendKeys(phone);
							HookMgr.NeedQuickReg = true;
							IWebElement smsElement = registerFormElem.FinElement(By.ClassName("get-sms-code"));
							bool flag7 = smsElement != null;
							if (flag7)
							{
								smsElement.RandomClick(webDriver);
								Thread.Sleep(5000);
								this.PicCodeProcess(webDriver, false);
								HookMgr.NeedQuickReg = false;
								bool phoneIsReg = HookMgr.PhoneIsReg;
								if (phoneIsReg)
								{
									XTrace.WriteLine("手机已被注册 " + phone);
									HookMgr.PhoneIsReg = false;
									return false;
								}
								Func<string, string, string> getPhoneSms = this.GetPhoneSms;
								string sms = (getPhoneSms != null) ? getPhoneSms(phone, quHao) : null;
								bool flag8 = string.IsNullOrWhiteSpace(sms);
								if (flag8)
								{
									XLog.Info("获取不到手机验证码验证码,返回", Array.Empty<object>());
									return false;
								}
								for (int i = 0; i < 5; i++)
								{
									UDBiframeDriver = this.TanChuang(webDriver);
									bool flag9 = UDBiframeDriver == null;
									if (flag9)
									{
										XTrace.WriteLine("登录弹窗定位不到");
										Thread.Sleep(1000);
									}
									else
									{
										registerFormElem = UDBiframeDriver.FinElement(By.Id("register-form"));
										IWebElement inputCodeElement = (registerFormElem != null) ? registerFormElem.FinElement(By.ClassName("udb-input-code")) : null;
										bool flag10 = inputCodeElement != null;
										if (flag10)
										{
											inputCodeElement.Click();
											Thread.Sleep(1000);
											inputCodeElement.SendKeys(sms);
										}
										IWebElement pwElement = (registerFormElem != null) ? registerFormElem.FinElement(By.ClassName("udb-input-pw")) : null;
										bool flag11 = pwElement != null;
										if (flag11)
										{
											if (pwElement != null)
											{
												pwElement.Click();
											}
											Thread.Sleep(1000);
											pwElement.SendKeys(mima);
											Thread.Sleep(2000);
										}
										IWebElement agreenElem = (registerFormElem != null) ? registerFormElem.FinElement(By.Name("agreen")) : null;
										bool flag12 = agreenElem != null;
										if (flag12)
										{
											if (agreenElem != null)
											{
												agreenElem.Click();
											}
											Thread.Sleep(2000);
										}
										IWebElement btnRegElement = (registerFormElem != null) ? registerFormElem.FinElement(By.Id("reg-btn")) : null;
										bool flag13 = btnRegElement != null;
										if (flag13)
										{
											btnRegElement.RandomClick(webDriver);
											Thread.Sleep(9000);
											IWebElement reg = UDBiframeDriver.FinElement(By.Id("register-form"));
											bool flag14 = reg == null;
											if (flag14)
											{
												return true;
											}
										}
									}
								}
							}
							else
							{
								HookMgr.NeedQuickReg = false;
							}
						}
						result = false;
					}
				}
			}
			return result;
		}
		public void RandomOpenFeiLei(IWebDriver webDriver)
		{
			int i = 0;
			while (i < 3)
			{
				if (!this.RandomOpenFeiLeiAction(webDriver))
				{
					i++;
				}
				else
				{
					break;
				}
			}
		}

		public bool RandomOpenFeiLeiAction(IWebDriver webDriver)
		{
			bool flag;
			string attribute;
			IWebElement categoryElement = webDriver.FinElement(By.Id("hy-nav-category"));
			if (categoryElement != null)
			{
				SelementHelper.MoveTo(webDriver, categoryElement);
				Thread.Sleep(2000);
				string clickstat = "clickstat";
				int i = HelperGeneral.Random.Next(1, 24);
				string val = string.Format("click/navi/game/game{0}", i);
				try
				{
					IWebElement eidElement = webDriver.FinElementByAttr(By.ClassName(clickstat), "eid", val);
					if (eidElement != null)
					{
						if (eidElement != null)
						{
							attribute = eidElement.GetAttribute("title");
						}
						else
						{
							attribute = null;
						}
						string eidTitle = attribute;
						if ((!eidElement.Enabled ? false : eidElement.Displayed))
						{
							if (eidElement != null)
							{
								eidElement.Click();
							}
							Thread.Sleep(7000);
							webDriver.SwitchPageByTitle(eidTitle);
							flag = true;
							return flag;
						}
					}
				}
				catch (Exception exception)
				{
					XTrace.WriteLine(string.Concat("RandomOpenFeiLei ", exception.Message));
				}
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		protected string Remove0(string all)
		{
			string ret = "";
			bool yao = false;
			for (int i = 0; i < all.Length; i++)
			{
				if ((yao ? true : all[i] != '0'))
				{
					yao = true;
					char chr = all[i];
					ret = string.Concat(ret, chr.ToString());
				}
			}
			return ret;
		}

		protected bool SelectQuHao(IWebElement element, string quHao)
		{
			bool flag;
			try
			{
				string qh = this.Remove0(quHao);
				IWebElement e = element.FinElementByAttr(By.ClassName("areasNumber"), "areascode", qh ?? "");
				if (e != null)
				{
					if (e != null)
					{
						e.Click();
					}
					Thread.Sleep(1000);
					flag = true;
					return flag;
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat("注册,选择下拉列表对应的国家区号 出错 ", exception.Message));
			}
			flag = false;
			return flag;
		}

		public void SendDanWu(IWebDriver webDriver, string msg = "666660")
		{
			try
			{
				IWebElement inputElement = webDriver.FinElement(By.Id("pub_msg_input"));
				if (inputElement != null)
				{
					inputElement.Clear();
					inputElement.Click();
					inputElement.SendKeys(msg);
					webDriver.FinElement(By.Id("msg_send_bt")).Click();
					Thread.Sleep(500);
				}
			}
			catch (Exception exception)
			{
			}
		}

		public void SendGift(IWebDriver webDriver, int GiftId)
		{
			IWebElement faceEl = webDriver.FinElement(By.Id("player-face"));
			if (faceEl != null)
			{
				IWebElement gift = faceEl.FinElementByAttr(By.ClassName("player-face-gift"), "propsid", GiftId.ToString());
				if (gift != null)
				{
					Actions ac = new Actions(webDriver);
					ac.MoveToElement(gift, 5, 5).Click().Perform();
					Thread.Sleep(2000);
					IWebElement dialogEl = webDriver.FinElement(By.Id("player-gift-dialog"));
					if (dialogEl != null)
					{
						dialogEl.FindAndClickElement(By.ClassName("confirm"));
						dialogEl.FindAndClickElement(By.ClassName("close"));
					}
					webDriver.FindAndClickElement(By.ClassName("close-create-layer"));
				}
			}
		}

		private void SetProxy(ChromeOptions options, string proxyIpAndPort = "127.0.0.1:8877")
		{
			Proxy proxy = new Proxy()
			{
				Kind = ProxyKind.Manual,
				HttpProxy = proxyIpAndPort,
				SslProxy = proxyIpAndPort,
				FtpProxy = proxyIpAndPort,
				IsAutoDetect = false,
				NoProxy = "None"
			};
			options.Proxy = proxy;
		}

		public bool SmsLoginOrReg(string phone, ref string mima, string quHao, bool isReg = false)
		{
			bool flag;
			string str;
			IWebDriver webDriver = this.OpenFirst(phone, XmlConfig<SeleniumConfig>.Current.FirstUrl, "127.0.0.1:8877");
			IWebDriver UDBiframeDriver = this.TanChuang(webDriver);
			if (UDBiframeDriver != null)
			{
				this.BtnSMSLoginTilte(UDBiframeDriver);
				IWebElement phoneLoginDivElement = UDBiframeDriver.FinElement(By.Id("phone-login-form"));
				if (phoneLoginDivElement != null)
				{
					IWebElement quhaoElem = phoneLoginDivElement.FinElementByAttr(By.ClassName("udb-form-sel"), "eid", "Click/SMSLogin/AreaCode");
					if (quhaoElem != null)
					{
						quhaoElem.Click();
					}
					Thread.Sleep(2000);
					if (this.SelectQuHao(phoneLoginDivElement, quHao))
					{
						IWebElement phoenInputElement = phoneLoginDivElement.FinElement(By.ClassName("udb-input-tel"));
						if (phoenInputElement != null)
						{
							phoenInputElement.Click();
							phoenInputElement.SendKeys(phone);
							Thread.Sleep(1000);
							IWebElement smsElement = phoneLoginDivElement.FinElement(By.ClassName("get-sms-code"));
							if (smsElement != null)
							{
								if (smsElement != null)
								{
									smsElement.Click();
								}
								Thread.Sleep(1000);
								Func<string, string, string> getPhoneSms = this.GetPhoneSms;
								if (getPhoneSms != null)
								{
									str = getPhoneSms(phone, quHao);
								}
								else
								{
									str = null;
								}
								string sms = str;
								if (!string.IsNullOrWhiteSpace(sms))
								{
									IWebElement inputCodeElement = phoneLoginDivElement.FinElement(By.ClassName("udb-input-code"));
									if (inputCodeElement != null)
									{
										inputCodeElement.Click();
										Thread.Sleep(1000);
										inputCodeElement.SendKeys(sms);
									}
									IWebElement btnRegElement = phoneLoginDivElement.FinElement(By.Id("phone-login-btn"));
									if (btnRegElement != null)
									{
										if (btnRegElement != null)
										{
											btnRegElement.Click();
										}
										Thread.Sleep(2000);
										flag = true;
										return flag;
									}
								}
								else
								{
									flag = false;
									return flag;
								}
							}
						}
					}
					flag = false;
				}
				else
				{
					XTrace.WriteLine("验证码登录窗口表单元素找不到");
					flag = false;
				}
			}
			else
			{
				XTrace.WriteLine("登录弹窗定位不到");
				flag = false;
			}
			return flag;
		}

		public void Subscribe(IWebDriver webDriver)
		{
			IWebElement subscribeEl = webDriver.FinElement(By.ClassName("subscribe-entrance"));
			if (subscribeEl != null)
			{
				Actions actions = new Actions(webDriver);
				actions.MoveToElement(subscribeEl, 30, 20).Click().Perform();
				Thread.Sleep(3000);
				webDriver.FindAndClickElement(By.ClassName("J_sub_x"));
				webDriver.FindAndClickElement(By.ClassName("btn-close"));
				webDriver.FindAndClickElement(By.ClassName("dlg-close"));
			}
		}

		protected IWebDriver TanChuang(IWebDriver webDriver)
		{
			IWebDriver webDriver1;
			try
			{
				webDriver.SwitchTo().DefaultContent();
				int i = 0;
				while (i < 2)
				{
					if (!this.HasLogin(webDriver))
					{
						IWebElement elem = webDriver.FinElement(By.Id("UDBSdkLgn_iframe"));
						if (elem == null)
						{
							this.OpenLogin(webDriver);
							i++;
						}
						else
						{
							webDriver1 = webDriver.SwitchTo().Frame(elem);
							return webDriver1;
						}
					}
					else
					{
						break;
					}
				}
			}
			catch (Exception exception)
			{
				XTrace.WriteLine(string.Concat("弹窗定位不到 ", exception.Message));
			}
			webDriver1 = null;
			return webDriver1;
		}

		public static void Test()
		{
			HookMgr.jiQiMa = HyDeviceMgr.Ins.GetRandomJiQiMa();
			string phone = "0123456789";
			FiddlerHandle.AutoWeb.HySeleniumMgr HySeleniumMgr = new FiddlerHandle.AutoWeb.HySeleniumMgr();
			HySeleniumMgr.ClearCach(phone);
			HySeleniumMgr.OpenFirst(phone, XmlConfig<SeleniumConfig>.Current.FirstUrl, "127.0.0.1:8877");
		}

		protected void TouXiangDingWei(IWebDriver webDriver)
		{
			IWebElement img_widthEl = webDriver.FinElement(By.Id("img_width"));
			IWebElement img_heightEl = webDriver.FinElement(By.Id("img_height"));
			int img_width = img_widthEl.GetAttribute("value").ToInt(0);
			int img_height = img_heightEl.GetAttribute("value").ToInt(0);
			if ((img_width <= 0 ? false : img_height > 0))
			{
				int limit = (img_width < img_height ? img_width : img_height);
				int lenMin = (limit < 100 ? limit : 100);
				int len = HelperGeneral.Random.Next(lenMin, (limit > 100 ? limit : 100));
				int x1 = 0;
				int y1 = 0;
				int x2 = 0;
				int y2 = 0;
				if (limit > 100)
				{
					x1 = HelperGeneral.Random.Next(0, limit - len);
					x2 = x1 + len;
					y1 = HelperGeneral.Random.Next(0, limit - len);
					y2 = y1 + len;
				}
				else
				{
					x1 = 0;
					y1 = 0;
					x2 = len;
					y2 = len;
				}
				webDriver.FinAndSetAttrVal(webDriver, By.Id("x1"), "value", x1.ToString());
				webDriver.FinAndSetAttrVal(webDriver, By.Id("y1"), "value", y1.ToString());
				webDriver.FinAndSetAttrVal(webDriver, By.Id("x2"), "value", x2.ToString());
				webDriver.FinAndSetAttrVal(webDriver, By.Id("y2"), "value", y2.ToString());
			}
		}

		public void TouXiangTask(IWebDriver webDriver)
		{
			if (!webDriver.FinHasAttrVal(By.ClassName("un-login"), "style", "display: block;"))
			{
				int rd = HelperGeneral.Random.Next(1, 2);
				IWebElement tx = null;
				tx = (rd != 1 ? webDriver.FinElement(By.Id("login-username")) : webDriver.FinElement(By.Id("login-userAvatar")));
				if (tx != null)
				{
					Actions ac = new Actions(webDriver);
					ac.MoveToElement(tx).Perform();
					Thread.Sleep(2000);
					this.GetTask(webDriver);
					ac.MoveToElement(tx, 200, 200).Perform();
					Thread.Sleep(100);
				}
			}
		}

		public void WanShangZhiLiao(IWebDriver webDriver)
		{
			webDriver.SwitchTo().DefaultContent();
			IWebElement iframeEl = webDriver.FinElement(By.Id("uploadInformation"));
			if (iframeEl != null)
			{
				webDriver.SwitchTo().Frame(iframeEl);
				XTrace.WriteLine("有完成资料弹窗,开始 修改昵称和上传头像");
				string nick = HelperText.RdMaJia(XmlConfig<SeleniumConfig>.Current.RdName, XmlConfig<SeleniumConfig>.Current.RdNameAndNum);
				webDriver.FinElementAndInput(By.ClassName("nick-input"), nick, null);
				try
				{
					FileInfo pic = ModifyHead.GetHeadRandomPathNotSpace();
					if (null == null)
					{
						XTrace.WriteLine("使用AutoIt3上传");
						if (webDriver.FindAndClickElement(By.ClassName("avatar-upload")) != null)
						{
							Thread.Sleep(2000);
							SelementHelper.Upload(pic.FullName);
							Thread.Sleep(2000);
						}
					}
					Thread.Sleep(3000);
				}
				catch (Exception exception)
				{
					XTrace.WriteLine(string.Concat("完善资料  操作修改头像失败", exception.Message));
				}
				webDriver.FindAndRandomClickElement(webDriver, By.ClassName("submit-complete"));
				Thread.Sleep(2000);
				webDriver.FindAndRandomClickElement(webDriver, By.ClassName("j_complete"));
			}
		}

		private bool XiuGaiMiMa(IWebDriver webDriver, string phone, ref string quHao, string mima)
		{
			bool flag;
			IWebElement homeElement = webDriver.FinElement(By.LinkText("我的信息"));
			if (homeElement == null)
			{
				homeElement.Click();
				Thread.Sleep(9000);
			}
			IWebElement xiuGaiElement = webDriver.FinElement(By.LinkText("修改密码"));
			if (xiuGaiElement != null)
			{
				xiuGaiElement.Click();
				Thread.Sleep(9000);
			}
			IWebElement modify = webDriver.FinElement(By.XPath("/html/body/div[3]/div[3]/div/div/div[2]/iframe"));
			if (modify != null)
			{
				if (modify.GetAttribute("src") == "https://aq.huya.com/i/modify.html")
				{
					webDriver.SwitchTo().Frame(modify);
					IWebElement smsElement = webDriver.FinElement(By.LinkText("获取验证码"));
					if (smsElement != null)
					{
						smsElement.Click();
						string sms = this.GetPhoneSms(phone, quHao);
						if (string.IsNullOrWhiteSpace(sms))
						{
							flag = false;
							return flag;
						}
						IWebElement codeInputElement = webDriver.FinElement(By.Name("code"));
						if (codeInputElement != null)
						{
							codeInputElement.Click();
							codeInputElement.SendKeys(sms);
							Thread.Sleep(1000);
						}
					}
					IWebElement passwdElement = webDriver.FinElement(By.Name("passwd"));
					if (passwdElement != null)
					{
						passwdElement.Click();
						passwdElement.SendKeys(mima);
					}
					IWebElement wanChengElement = webDriver.FinElement(By.ClassName("UDBSdkReg-button focusnot"));
					if (wanChengElement != null)
					{
						wanChengElement.Click();
						Thread.Sleep(9000);
						flag = true;
						return flag;
					}
				}
			}
			flag = false;
			return flag;
		}

		public bool XiuGaiNick(IWebDriver webDriver, string nick)
		{
			bool flag;
			bool flag1;
			string phone;
			IWebElement e = webDriver.FinElement(By.Id("edit_nick"));
			if (e == null)
			{
				if (webDriver.FindAndClickElement(By.LinkText("我的信息")) != null)
				{
					Thread.Sleep(2000);
				}
				e = webDriver.FinElement(By.Id("edit_nick"));
			}
			IWebElement nickEl = webDriver.FinElement(By.ClassName("uesr_n"));
			if (nickEl != null)
			{
				string nick_n = nickEl.Text.Trim();
				if ((nick_n.IsNullOrWhiteSpace() ? false : nick_n.IndexOf("我是一颗小虎牙") == -1))
				{
					flag = true;
					return flag;
				}
			}
			if (e != null)
			{
				e.Click();
				Thread.Sleep(9000);
				IWebElement mianfei = webDriver.FinElement(By.XPath("/html/body/div[3]/div[3]/div/div/div[2]/div[1]/div/p[1]/span"));
				if (mianfei == null)
				{
					flag1 = false;
				}
				else
				{
					string text = mianfei.Text;
					if (text != null)
					{
						flag1 = text.IndexOf("免费") != -1;
					}
					else
					{
						flag1 = true;
					}
				}
				if (flag1)
				{
					IWebElement nickInputElement = webDriver.FinElement(By.Id("new-nick"));
					if (nickInputElement != null)
					{
						nickInputElement.Click();
						nickInputElement.SendKeys(nick);
						Thread.Sleep(1000);
						IWebElement sureElement = webDriver.FinElement(By.Id("sure-code"));
						if (sureElement == null)
						{
							flag = false;
							return flag;
						}
						else
						{
							sureElement.Click();
							Thread.Sleep(9000);
							IWebElement tipsElem = webDriver.FinElement(By.Id("sure-box"));
							if (tipsElem != null)
							{
								IWebElement mianfeiElem = tipsElem.FinElement(By.Id("modifyTimesDesc"));
								if ((mianfeiElem == null ? true : mianfeiElem.Text.Trim() != "免费"))
								{
									tipsElem.FindAndClickElement(By.ClassName("close_box"));
								}
								else if (tipsElem.FindAndClickElement(By.ClassName("sure-btn")) != null)
								{
									Thread.Sleep(5000);
									IWebElement feifaNickEl = webDriver.FinElement(By.XPath("//*[@id=\"tips-box-content\"]/p"));
									if ((feifaNickEl == null ? false : feifaNickEl.Text.IndexOf("修改后重试") != -1))
									{
										XTrace.WriteLine(string.Concat("昵称有问题   ", nick, "  ", feifaNickEl.Text));
										webDriver.FindAndClickElement(By.ClassName("close_box"));
										nick = HelperText.RdMaJia(5, false);
										bool jg = this.XiuGaiNick(webDriver, nick);
										webDriver.SwitchTo().DefaultContent();
										flag = jg;
										return flag;
									}
									IWebElement iframeEl = webDriver.FinElement(By.XPath("//*[@id=\"payPhoneBox\"]/div[2]/div/div/iframe"));
									if (iframeEl != null)
									{
										IWebDriver iframeWebDriver = webDriver.SwitchTo().Frame(iframeEl);
										Func<string, string, string> getPhoneSms = this.GetPhoneSms;
										if (getPhoneSms != null)
										{
											phone = getPhoneSms(this.Phone, this.QuHao);
										}
										else
										{
											phone = null;
										}
										string sms = phone;
										if (!sms.IsNullOrWhiteSpace())
										{
											for (int i = 0; i < 5; i++)
											{
												if (iframeWebDriver.FinElementAndInput(iframeWebDriver, By.ClassName("input-security"), sms, null) != null)
												{
													if (iframeWebDriver.FindAndRandomClickElement(iframeWebDriver, By.ClassName("next-step-btn")) != null)
													{
														Thread.Sleep(1000);
													}
													else
													{
														break;
													}
												}
											}
										}
									}
									webDriver.SwitchTo().DefaultContent();
								}
							}
						}
					}
				}
			}
			for (int i = 0; i < 3; i++)
			{
				webDriver.FindAllAndClickElement(By.ClassName("btn-close"));
				webDriver.FindAllAndClickElement(By.ClassName("J_btnClose"));
				webDriver.FindAllAndClickElement(By.ClassName("close_box"));
			}
			flag = false;
			return flag;
		}

		public bool XiuGaiTouXiang(IWebDriver webDriver, string picPatch)
		{
			bool flag;
			IWebElement imgElement = webDriver.FinElement(By.ClassName("img_hover"));
			if (imgElement == null)
			{
				if (webDriver.FindAndClickElement(By.LinkText("我的信息")) != null)
				{
					Thread.Sleep(2000);
				}
				imgElement = webDriver.FinElement(By.ClassName("img_hover"));
			}
			if (imgElement != null)
			{
				Thread.Sleep(2000);
				IWebElement fileElement = webDriver.FinElement(By.ClassName("input_file_box"));
				if (fileElement != null)
				{
					fileElement.SendKeys(picPatch);
					Thread.Sleep(9000);
					this.TouXiangDingWei(webDriver);
					IWebElement btn = webDriver.FinElement(By.ClassName("sure_img"));
					if (btn != null)
					{
						btn.Click();
						Thread.Sleep(9000);
						flag = true;
						return flag;
					}
				}
			}
			flag = false;
			return flag;
		}
	}
}