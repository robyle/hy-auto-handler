using FiddlerHandle;
using Helper;
using Huya.Data.Entity;
using HyModel;
using NewLife.Log;
using NewLife.Threading;
using NewLife.Xml;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using ThirdParty.Mobiles;
using ThirdParty.Model;
using XCode;
using XS.Helper.Net;

namespace FiddlerHandle.AutoWeb
{
	public class AutoQuickReg : AutoCaoZuoBase
	{
		public static int[] NotPhone;

		public MobileType ptType { get; private set; } = MobileType.MDS;

		static AutoQuickReg()
		{
			AutoQuickReg.NotPhone = new int[] { 165, 173, 175, 176, 177, 178 };
		}

		public AutoQuickReg()
		{
		}

		private bool OneReg()
		{
			bool flag;
			FiddlerProxyLogic.GetProxy(null);
			base.GeRegJqm();
			base.HySeleniumMgr = new FiddlerHandle.AutoWeb.HySeleniumMgr();
			MobileMgr mobileMgr = new MobileMgr(this.ptType);
			MobileModel mobilenum = null;
			while (true)
			{
				mobilenum = mobileMgr.GetMobilenum(null);
				if ((mobilenum == null ? false : !mobilenum.PhoneNumber.IsNullOrWhiteSpace()))
				{
					break;
				}
				XTrace.WriteLine("手机号码获取不到!~~");
			}
			ConvertMy.toInt(mobilenum.PhoneNumber.Cut(3, null));
			XTrace.WriteLine(string.Concat("合适的手机号码 ", mobilenum.PhoneNumber));
			HyAccount acount = HyAccount.FindByPhone(mobilenum.PhoneNumber);
			if ((acount == null ? true : acount.Phone.IsNullOrWhiteSpace()))
			{
				base.SetPhoneCodeCallBack(mobileMgr, mobilenum);
				base.HySeleniumMgr.ClearCach(mobilenum.PhoneNumber);
				string password = base.CratePasswor();
				bool ret = base.HySeleniumMgr.QuickRegisterHy(mobilenum.PhoneNumber, password, mobilenum.CountryCode);
				if (ret)
				{
					HyAccount hyAccount = new HyAccount()
					{
						UserName = mobilenum.PhoneNumber,
						UserPass = password,
						PiHao = XmlConfig<SeleniumConfig>.Current.PiHao,
						Enable = true,
						RegType = RegType.HyKuaiSu,
						BeiZhu = "测试第六批->www.huya.com 页面登录成功后自动弹出完善资料页面,谷歌浏览器自动化操作快速注册,过极验验证,过检测驱动"
					};
					hyAccount.SaveModel(mobilenum);
					DateTime now = TimerX.Now;
					hyAccount.CreateTime = now;
					hyAccount.Save();
					string jqmStr = SelementHelper.GetJqmStr(base.HySeleniumMgr.WebDriver, Array.Empty<object>());//机器码处理
					hyAccount.JiQiMa = jqmStr;
					hyAccount.UpdateTime = now.AddDays(-1);
					hyAccount.Save();
					mobileMgr.reslut(mobilenum.PhoneNumber, true);
					XTrace.WriteLine(string.Concat(new string[] { "快速注册成功 ", hyAccount.UserName, " 密码:", hyAccount.UserPass, "  还没登录信息,需要运行另一个任务取得登录信息" }));
					base.HySeleniumMgr.GetPhoneSms = (string phone, string quHao) => {
						string str;
						MobileModel mobe = mobileMgr.GetMobilenum(hyAccount.Phone);
						if ((mobe == null ? false : !mobe.PhoneNumber.IsNullOrWhiteSpace()))
						{
							string sms = MobileMgr.GetYzm(mobileMgr.getVcodeAndHoldMobilenum(mobe), 6);
							if (sms.IsNullOrWhiteSpace())
							{
								mobileMgr.cancelSMSRecv(mobilenum.PhoneNumber);
							}
							str = sms;
						}
						else
						{
							if ((mobe != null ? true : (mobe.Ex1.IsNullOrWhiteSpace() ? false : mobe.Ex1.IndexOf("没有可用号码") != -1)))
							{
								hyAccount.PhoneEx1 = mobe.Ex1;
								hyAccount.Save();
							}
							str = null;
						}
						return str;
					};
					try
					{
						int i = 0;
						while (i < 5)
						{
							Thread.Sleep(5000);
							base.HySeleniumMgr.WanShangZhiLiao(base.HySeleniumMgr.WebDriver);
							if (!base.HySeleniumMgr.HasLogin(base.HySeleniumMgr.WebDriver))
							{
								base.HySeleniumMgr.LoginHy(hyAccount.UserName, hyAccount.UserPass, base.HySeleniumMgr.WebDriver);
								i++;
							}
							else
							{
								break;
							}
						}
					}
					catch (Exception exception)
					{
						XTrace.WriteLine(string.Concat("执行登录出错 ", exception.ToString()));
					}
					base.ZHInitCaoZuo();
					if (XmlConfig<SeleniumConfig>.Current.ZhuCeInRoom)
					{
						base.ZhuCeLoginAfter();
					}
					base.GetCookiesAndSaveToDb(hyAccount);
					flag = ret;
				}
				else
				{
					Thread.Sleep(900);
					mobileMgr.GetMobilenum(mobilenum.PhoneNumber);
					mobileMgr.addIgnoreList(mobilenum);
					mobileMgr.cancelSMSRecv(mobilenum.PhoneNumber);
					XTrace.WriteLine("注册失败");
					mobileMgr.reslut(mobilenum.PhoneNumber, false);
					string jqmStr1 = SelementHelper.GetJqmStr(base.HySeleniumMgr.WebDriver, Array.Empty<object>());
					XTrace.WriteLine(string.Concat("测试 获取到的机器码为 ", jqmStr1));
					flag = false;
				}
			}
			else
			{
				mobileMgr.addIgnoreList(mobilenum);
				mobileMgr.cancelSMSRecv(mobilenum.PhoneNumber);
				XTrace.WriteLine("手机号码已经注册!~~");
				mobileMgr.reslut(mobilenum.PhoneNumber, false);
				flag = false;
			}
			return flag;
		}

		private void Process(int needCount)
		{
			int ZhiXingCount = 0;
			int SuccecdNum = 0;
			FiddlerProxyLogic.SetFindderUseS5Proxy();
			while (true)
			{
				if (SuccecdNum >= needCount)
				{
					break;
				}
				try
				{
					try
					{
						if (XmlConfig<SeleniumConfig>.Current.ADSLBoHao)
						{
							XTrace.WriteLine(string.Concat(new string[] { "准备执行ADSL拨号...", XmlConfig<SeleniumConfig>.Current.PPPOEname, " ", XmlConfig<SeleniumConfig>.Current.PPOEZh, " ", XmlConfig<SeleniumConfig>.Current.PPOEMima, " ", XmlConfig<SeleniumConfig>.Current.FuWuName }));
							ADSLx.ChongXinBoHao(XmlConfig<SeleniumConfig>.Current.PPPOEname, XmlConfig<SeleniumConfig>.Current.PPOEZh, XmlConfig<SeleniumConfig>.Current.PPOEMima, XmlConfig<SeleniumConfig>.Current.FuWuName, (string msg) => XTrace.WriteLine(string.Concat("执行ADSL拨号: [", XmlConfig<SeleniumConfig>.Current.PPPOEname, "] ->  ", msg)));
						}
						ZhiXingCount++;
						Console.Title = string.Format("当前执行第[{0}]次 注册成功[{1}]个", ZhiXingCount, SuccecdNum);
						XTrace.WriteLine(string.Format("执行次数 {0}", ZhiXingCount));
						Action startAction = base.StartAction;
						if (startAction != null)
						{
							startAction();
						}
						else
						{
						}
						if (this.OneReg())
						{
							SuccecdNum++;
						}
					}
					catch (Exception exception)
					{
						XTrace.WriteLine(string.Concat("执行任务过程出错 ", exception.ToString()));
					}
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
					else
					{
					}
					Thread.Sleep(3000);
				}
			}
			XTrace.WriteLine(string.Format("注册完成:总共数量 [{0}]", SuccecdNum));
		}

		public void Start(int needCount)
		{
			this.ptType = XmlConfig<SeleniumConfig>.Current.PhonePtType;
			ThreadPoolX.QueueUserWorkItem<int>(new Action<int>(this.Process), needCount);
		}
	}
}