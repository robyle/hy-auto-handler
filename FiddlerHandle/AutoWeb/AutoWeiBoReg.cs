using Huya.Data.Entity;
using HyModel;
using NewLife.Log;
using NewLife.Threading;
using NewLife.Xml;
using System;
using System.Runtime.CompilerServices;
using ThirdParty.Mobiles;
using ThirdParty.Model;
using XCode;

namespace FiddlerHandle.AutoWeb
{
	public class AutoWeiBoReg : AutoCaoZuoBase
	{
		private FiddlerHandle.AutoWeb.HySeleniumWeiBoRegMgr HySeleniumWeiBoRegMgr
		{
			get;
			set;
		}

		public MobileType ptType { get; private set; } = MobileType.MDS;

		public AutoWeiBoReg()
		{
		}

		private bool OneReg()
		{
			bool flag;
			base.GeRegJqm();
			FiddlerHandle.AutoWeb.HySeleniumWeiBoRegMgr hySeleniumWeiBoRegMgr = new FiddlerHandle.AutoWeb.HySeleniumWeiBoRegMgr();
			FiddlerHandle.AutoWeb.HySeleniumWeiBoRegMgr hySeleniumWeiBoRegMgr1 = hySeleniumWeiBoRegMgr;
			this.HySeleniumWeiBoRegMgr = hySeleniumWeiBoRegMgr;
			base.HySeleniumMgr = hySeleniumWeiBoRegMgr1;
			MobileMgr mobileMgr = new MobileMgr(this.ptType);
			MobileModel mobileModel = mobileMgr.GetMobilenum(null);
			base.SetPhoneCodeCallBack(mobileMgr, mobileModel);
			base.HySeleniumMgr.ClearCach(mobileModel.PhoneNumber);
			string password = base.CratePasswor();
			bool ret = this.HySeleniumWeiBoRegMgr.WeiBoRegHy("", password, mobileModel.PhoneNumber, mobileModel.CountryCode);
			if (ret)
			{
				HyAccount zh = new HyAccount()
				{
					UserName = mobileModel.PhoneNumber,
					UserPass = password,
					PiHao = XmlConfig<SeleniumConfig>.Current.PiHao,
					Enable = true,
					RegType = RegType.Weibo,
					BeiZhu = "谷歌浏览器自动化操作微博注册"
				};
				zh.SaveModel(mobileModel);
				DateTime now = TimerX.Now;
				zh.CreateTime = now;
				string jqmStr = (string)SelementHelper.ExecJs(base.HySeleniumMgr.WebDriver, "return hyDecode(getCommmonInfo());", Array.Empty<object>());
				zh.JiQiMa = jqmStr;
				zh.UpdateTime = now.AddDays(-1);
				zh.Save();
				flag = ret;
			}
			else
			{
				XTrace.WriteLine("注册失败");
				flag = false;
			}
			return flag;
		}
	}
}