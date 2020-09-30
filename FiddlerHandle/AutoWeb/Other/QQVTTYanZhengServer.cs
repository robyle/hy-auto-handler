using FastVerCode;
using FiddlerHandle;
using FiddlerHandle.AutoWeb;
using FiddlerHandle.Properties;
using Helper.TuPian;
using NewLife.Log;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FiddlerHandle.AutoWeb.Other
{
	public class QQVTTYanZhengServer
	{
		private static QQVTTYanZhengServer ins;

		private LianZhongV2 dtMgr { get; set; } = new LianZhongV2();

		public static QQVTTYanZhengServer Ins
		{
			get
			{
				return QQVTTYanZhengServer.ins;
			}
		}

		private string OldImgBase64
		{
			get;
			set;
		}

		static QQVTTYanZhengServer()
		{
			QQVTTYanZhengServer.ins = new QQVTTYanZhengServer();
		}

		private QQVTTYanZhengServer()
		{
		}

		private string GetPicBase64Data(IWebDriver iframeElWebDriver, string tcaptcha_titleStr)
		{
			string str;
			try
			{
				string zhuRuJs = Resources.getBase64Image_Vtt;
				str = (string)SelementHelper.ExecJs(iframeElWebDriver, zhuRuJs, new object[] { tcaptcha_titleStr });
				return str;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				XLog.Debug(string.Concat("GetPicBase64Data ", ex.ToString()), Array.Empty<object>());
			}
			str = null;
			return str;
		}

		internal bool VTTShiBieGuoCheng(IWebDriver IframeElWebDriver)
		{
			bool flag;
			bool isSuc = false;
			if (0 < 20)
			{
				IWebElement tcaptcha_vtt0Elem = IframeElWebDriver.FinElement(By.Id("tcaptcha_vtt0"));
				if (tcaptcha_vtt0Elem != null)
				{
					this.VTTShiBieGuoChengDaTi(IframeElWebDriver, tcaptcha_vtt0Elem);
					flag = isSuc;
				}
				else
				{
					flag = isSuc;
				}
			}
			else
			{
				flag = isSuc;
			}
			return flag;
		}

		private void VTTShiBieGuoChengDaTi(IWebDriver iframeElWebDriver, IWebElement tcaptcha_vtt0Elem)
		{
			XLog.Debug("VTTShiBieGuoChengDaTi 开始", Array.Empty<object>());
			string tcaptcha_titleStr = iframeElWebDriver.GetTextByElement(By.ClassName("tcaptcha-title"), null);
			IWebElement tcaptcha_imgEl = iframeElWebDriver.FinElement(By.Id("tcaptcha-img"));
			if (tcaptcha_imgEl != null)
			{
				(new Actions(iframeElWebDriver)).Click();
				string base64img = this.GetPicBase64Data(iframeElWebDriver, tcaptcha_titleStr);
				if (!base64img.IsNullOrWhiteSpace())
				{
					string base64imgNoHeand = ImageHelper.TakeOutHeand(base64img);
					if (this.OldImgBase64 == base64imgNoHeand)
					{
						this.dtMgr.report_error();
						XLog.Error("Vtt验证码识别错误,报错", Array.Empty<object>());
					}
					this.OldImgBase64 = base64imgNoHeand;
					PointLianZhong zuobiao = this.dtMgr.GetPoint(base64imgNoHeand, 1, 1);
					if (zuobiao != null)
					{
						ImageHelper.HuaDianAndSave(base64img, zuobiao.X, zuobiao.Y);
						Actions actions = new Actions(iframeElWebDriver);
						actions.MoveToElement(tcaptcha_imgEl, zuobiao.X, zuobiao.Y).Perform();
						Thread.Sleep(1000);
						actions.MoveToElement(tcaptcha_imgEl, zuobiao.X, zuobiao.Y).Click().Perform();
						Thread.Sleep(6000);
					}
				}
				else
				{
					XLog.Error("Vtt验证码-->获取失败", Array.Empty<object>());
				}
			}
		}
	}
}