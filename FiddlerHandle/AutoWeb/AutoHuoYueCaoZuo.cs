using System;
using System.Collections.Generic;
using System.Threading;
using Huya.Data.Entity;
using HyBase.HyFile;
using HyModel;
using NewLife.Log;
using NewLife.Threading;
using NewLife.Xml;
using OpenQA.Selenium;
using ThirdParty.Mobiles;
using ThirdParty.Model;
using XS.Helper.Net;

namespace FiddlerHandle.AutoWeb
{
	// Token: 0x0200000C RID: 12
	public class AutoHuoYueCaoZuo : AutoCaoZuoBase
	{
		// Token: 0x0600009D RID: 157 RVA: 0x00005BAA File Offset: 0x00003DAA
		public void Start()
		{
			ThreadPoolX.QueueUserWorkItem(new Action(this.Process));
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005BC0 File Offset: 0x00003DC0
		private void Process()
		{
			int ZhiXingCount = 0;
			bool isLogin = XmlConfig<SeleniumConfig>.Current.IsLogin;
			IList<HyAccount> zhs = HyAccount.GetHyAccountsByRegType(RegType.HyKuaiSu, XmlConfig<SeleniumConfig>.Current.PiHao, isLogin);
			IList<HyAccount> adds = HyAccount.GetHyAccountsByRegType(RegType.SMS, XmlConfig<SeleniumConfig>.Current.PiHao, isLogin);
			List<HyAccount> AllZhs = new List<HyAccount>();
			AllZhs.AddRange(zhs);
			AllZhs.AddRange(adds);
			int oneDaySeconds = 86400;
			FiddlerProxyLogic.SetFindderUseS5Proxy();
			foreach (HyAccount item in AllZhs)
			{
				DateTime now = TimerX.Now;
				bool flag = (now - item.UpdateTime).TotalSeconds < (double)oneDaySeconds;
				if (flag)
				{
					XTrace.WriteLine("账号[" + item.UserName + "]上次操作间隔时间还没有24小时");
				}
				try
				{
					bool adslboHao = XmlConfig<SeleniumConfig>.Current.ADSLBoHao;
					if (adslboHao)
					{
						XTrace.WriteLine(string.Concat(new string[]
						{
							"准备执行ADSL拨号...",
							XmlConfig<SeleniumConfig>.Current.PPPOEname,
							" ",
							XmlConfig<SeleniumConfig>.Current.PPOEZh,
							" ",
							XmlConfig<SeleniumConfig>.Current.PPOEMima,
							" ",
							XmlConfig<SeleniumConfig>.Current.FuWuName
						}));
						ADSLx.ChongXinBoHao(XmlConfig<SeleniumConfig>.Current.PPPOEname, XmlConfig<SeleniumConfig>.Current.PPOEZh, XmlConfig<SeleniumConfig>.Current.PPOEMima, XmlConfig<SeleniumConfig>.Current.FuWuName, delegate (string msg)
						{
							XTrace.WriteLine("执行ADSL拨号: [" + XmlConfig<SeleniumConfig>.Current.PPPOEname + "] ->  " + msg);
						});
					}
					ZhiXingCount++;
					Console.Title = string.Format("当前执行第[{0}]次 ", ZhiXingCount);
					XTrace.WriteLine(string.Format("执行次数 {0}", ZhiXingCount));
					Action startAction = base.StartAction;
					if (startAction != null)
					{
						startAction();
					}
					this.ProcessOneZh(item);
					bool test = XmlConfig<SeleniumConfig>.Current.Test;
					if (test)
					{
						XTrace.WriteLine("测试模式,已退出流程!");
						return;
					}
				}
				catch (Exception ex)
				{
					XTrace.WriteLine("执行任务过程出错 " + ex.ToString());
				}
				finally
				{
					base.Close();
					Thread.Sleep(2000);
					Action endAction = base.EndAction;
					if (endAction != null)
					{
						endAction();
					}
					Thread.Sleep(3000);
				}
			}
			XTrace.WriteLine("再次检测没有guid的,再进行一次操作");
			bool hasCaozuo = false;
			for (int i = 0; i < 3; i++)
			{
				foreach (HyAccount item2 in AllZhs)
				{
					try
					{
						bool flag2 = string.IsNullOrWhiteSpace(item2.Guid);
						if (flag2)
						{
							hasCaozuo = true;
							this.InRoomGetGuid(item2);
						}
					}
					catch (Exception ex2)
					{
						XTrace.WriteLine("再次检测没有guid的 执行任务过程出错 " + ex2.ToString());
					}
					finally
					{
						bool flag3 = hasCaozuo;
						if (flag3)
						{
							base.Close();
							Thread.Sleep(2000);
							Action endAction2 = base.EndAction;
							if (endAction2 != null)
							{
								endAction2();
							}
							Thread.Sleep(3000);
						}
					}
					hasCaozuo = false;
				}
				Thread.Sleep(10000);
			}
			XTrace.WriteLine("全部任务运行完成");
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005FBC File Offset: 0x000041BC
		private void ProcessOneZh(HyAccount account)
		{
			this.Prepace(account);
			base.ZHInitCaoZuo();
			Console.Title = "随机点击一个分类 " + Console.Title;
			base.OneByOneTaskTryErr(new Action<IWebDriver>(base.HySeleniumMgr.RandomOpenFeiLei), base.HySeleniumMgr);
			Console.Title = "到房间逛一下 " + Console.Title;
			base.OneByOneTaskTryErr(new Action<IWebDriver>(base.HySeleniumMgr.InRoom), base.HySeleniumMgr);
			Console.Title = "发送弹幕 " + Console.Title;
			base.OneByOneTaskTryErr(new Action<IWebDriver, string>(base.HySeleniumMgr.SendDanWu), base.HySeleniumMgr, "我来拉 6666");
			bool subscribe = XmlConfig<SeleniumConfig>.Current.Subscribe;
			if (subscribe)
			{
				Console.Title = "订阅 " + Console.Title;
				base.OneByOneTaskTryErr(new Action<IWebDriver>(base.HySeleniumMgr.Subscribe), base.HySeleniumMgr);
			}
			bool sendHyLiangGift = XmlConfig<SeleniumConfig>.Current.SendHyLiangGift;
			if (sendHyLiangGift)
			{
				Console.Title = "发送虎粮 " + Console.Title;
				base.OneByOneTaskTryErr(new Action<IWebDriver, int>(base.HySeleniumMgr.SendGift), base.HySeleniumMgr, 4);
			}
			bool lingTask = XmlConfig<SeleniumConfig>.Current.LingTask;
			if (lingTask)
			{
				Console.Title = "领任务 " + Console.Title;
				base.OneByOneTaskTryErr(new Action<IWebDriver>(base.HySeleniumMgr.TouXiangTask), base.HySeleniumMgr);
			}
			bool lingBaoXiang = XmlConfig<SeleniumConfig>.Current.LingBaoXiang;
			if (lingBaoXiang)
			{
				Console.Title = "领宝箱 " + Console.Title;
				base.OneByOneTaskTryErr(new Action<IWebDriver>(base.HySeleniumMgr.BaoXiangShow), base.HySeleniumMgr);
			}
			base.GetCookiesAndSaveToDb(account);
			string jqmStr = SelementHelper.GetJqmStr(base.HySeleniumMgr.WebDriver, Array.Empty<object>());
			XTrace.WriteLine("获取到的 机器码" + jqmStr);
			account.JiQiMa = (jqmStr ?? account.JiQiMa);
			account.Save();
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000061D0 File Offset: 0x000043D0
		private void Prepace(HyAccount account)
		{
			FiddlerProxyLogic.GetProxy(null);
			bool flag = !account.JiQiMa.IsNullOrWhiteSpace();
			if (flag)
			{
				JiQiMa jqm = new JiQiMa();
				jqm.SetVal4ComOldJson(account.JiQiMa);
				HookMgr.jiQiMa = jqm;
			}
			string zh = account.UserName;
			string mima = account.UserPass;
			Console.Title = string.Concat(new string[]
			{
				"当前账号 [",
				zh,
				"] [",
				mima,
				"] ",
				Console.Title
			});
			base.HySeleniumMgr = new HySeleniumMgr();
			base.HySeleniumMgr.GetPhoneSms = delegate (string phone, string quHao)
			{
				bool phoneSmsShouDong = XmlConfig<SeleniumConfig>.Current.PhoneSmsShouDong;
				string result;
				if (phoneSmsShouDong)
				{
					string sm = DataFile.WtingRead("请输入手机号 [" + phone + "]的验证码", "你输入的手机号 [{phone}]的验证码是", delegate (string s)
					{
						bool flag7 = s.Length == 6;
						return !flag7;
					});
					result = sm;
				}
				else
				{
					MobileType ptType = MobileTypeTool.Ex2ToMobileType(account.PhoneEx2);
					MobileMgr MobileMgr = new MobileMgr(ptType);
					MobileModel mobe = MobileMgr.GetMobilenum(account.Phone);
					bool flag4 = mobe == null || mobe.PhoneNumber.IsNullOrWhiteSpace();
					if (flag4)
					{
						bool flag5 = mobe != null || (!mobe.Ex1.IsNullOrWhiteSpace() && mobe.Ex1.IndexOf("没有可用号码") != -1);
						if (flag5)
						{
							account.PhoneEx1 = mobe.Ex1;
							account.Save();
						}
						result = null;
					}
					else
					{
						string smsBody = MobileMgr.getVcodeAndHoldMobilenum(mobe);
						string sms = MobileMgr.GetYzm(smsBody, 6);
						bool flag6 = sms.IsNullOrWhiteSpace();
						if (flag6)
						{
							MobileMgr.cancelSMSRecv(mobe.PhoneNumber);
						}
						result = sms;
					}
				}
				return result;
			};
			bool flag2;
			if (XmlConfig<SeleniumConfig>.Current.UpCookie && !account.WebCookie.IsNullOrWhiteSpace())
			{
				string webCookie = account.WebCookie;
				flag2 = (webCookie == null || webCookie.IndexOf("udb_biztoken") != -1);
			}
			else
			{
				flag2 = false;
			}
			bool flag3 = flag2;
			if (flag3)
			{
				XTrace.WriteLine("修改cookie 为: " + account.WebCookie);
				HookMgr.curCookie = account.WebCookie;
				HookMgr.UpCookie = XmlConfig<SeleniumConfig>.Current.UpCookie;
				try
				{
					IWebDriver webDriver = base.HySeleniumMgr.OpenFirst(zh, XmlConfig<SeleniumConfig>.Current.FirstUrl, "127.0.0.1:8877");
					SelementHelper.AddCookies(webDriver, account.WebCookie);
					Thread.Sleep(500);
				}
				catch (Exception ex)
				{
					XTrace.WriteLine("修改cookie失败 " + ex.ToString());
					Thread.Sleep(10000);
					return;
				}
				finally
				{
					base.Close();
					HookMgr.curCookie = null;
					HookMgr.UpCookie = false;
				}
			}
			try
			{
				IWebDriver webDriver2 = base.HySeleniumMgr.OpenFirst(zh, XmlConfig<SeleniumConfig>.Current.FirstUrl, "127.0.0.1:8877");
				base.HySeleniumMgr.LoginHy(zh, mima, webDriver2);
			}
			catch (Exception ex2)
			{
				XTrace.WriteLine("登录虎牙账号出错 " + ex2.ToString());
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006424 File Offset: 0x00004624
		private void InRoomGetGuid(HyAccount account)
		{
			this.Prepace(account);
			Console.Title = "到房间逛一下 " + Console.Title;
			base.OneByOneTaskTryErr(new Action<IWebDriver>(base.HySeleniumMgr.InRoom), base.HySeleniumMgr);
			Console.Title = "发送弹幕 " + Console.Title;
			base.OneByOneTaskTryErr(new Action<IWebDriver, string>(base.HySeleniumMgr.SendDanWu), base.HySeleniumMgr, "我来拉 6666");
			base.GetCookiesAndSaveToDb(account);
			string jqmStr = SelementHelper.GetJqmStr(base.HySeleniumMgr.WebDriver, Array.Empty<object>());
			XTrace.WriteLine("获取到的 机器码" + jqmStr);
			account.JiQiMa = (jqmStr ?? account.JiQiMa);
			account.Save();
		}
	}
}
