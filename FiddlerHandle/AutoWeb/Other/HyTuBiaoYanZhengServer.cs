using FastVerCode;
using FiddlerHandle;
using FiddlerHandle.AutoWeb;
using FiddlerHandle.Properties;
using Helper;
using Helper.TuPian;
using NewLife.Log;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using XS.Helper;

namespace FiddlerHandle.AutoWeb.Other
{
	public class HyTuBiaoYanZhengServer
	{
		private static HyTuBiaoYanZhengServer ins;

		private LianZhongV2 dtMgr { get; set; } = new LianZhongV2();

		public static HyTuBiaoYanZhengServer Ins
		{
			get
			{
				return HyTuBiaoYanZhengServer.ins;
			}
		}

		private string OldImgBase64
		{
			get;
			set;
		}

		static HyTuBiaoYanZhengServer()
		{
			HyTuBiaoYanZhengServer.ins = new HyTuBiaoYanZhengServer();
		}

		private HyTuBiaoYanZhengServer()
		{
		}

		private void DaTi(IWebDriver iframeElWebDriver)
		{
			IWebElement bigImgEl = iframeElWebDriver.FinElement(By.ClassName("bg-placeholder"));
			if (bigImgEl != null)
			{
				string base64img = this.GetPicBase64Data(iframeElWebDriver);
				if (!base64img.IsNullOrWhiteSpace())
				{
					string base64imgNoHeand = ImageHelper.TakeOutHeand(base64img);
					if (this.OldImgBase64 == base64imgNoHeand)
					{
						this.dtMgr.report_error();
						XLog.Error("Vtt验证码识别错误,报错", Array.Empty<object>());
					}
					this.OldImgBase64 = base64imgNoHeand;
					PointLianZhong[] zuobiaos = this.dtMgr.GetPoints(base64imgNoHeand, 3, 3);
					if (zuobiaos != null)
					{
						List<PointX> l = new List<PointX>();
						PointLianZhong[] pointLianZhongArray = zuobiaos;
						for (int i = 0; i < (int)pointLianZhongArray.Length; i++)
						{
							PointLianZhong item = pointLianZhongArray[i];
							l.Add(new PointX()
							{
								X = item.X,
								Y = item.Y
							});
						}
						ImageHelper.HuaDianAndSave(base64img, l);
						Actions actions = new Actions(iframeElWebDriver);
						PointLianZhong[] pointLianZhongArray1 = zuobiaos;
						for (int j = 0; j < (int)pointLianZhongArray1.Length; j++)
						{
							PointLianZhong zuobiao = pointLianZhongArray1[j];
							actions.MoveToElement(bigImgEl, zuobiao.X, zuobiao.Y).Perform();
							Thread.Sleep(HelperGeneral.Random.Next(1000, 2500));
							actions.MoveToElement(bigImgEl, zuobiao.X, zuobiao.Y).Click().Perform();
							Thread.Sleep(HelperGeneral.Random.Next(1000, 2500));
						}
						Thread.Sleep(6000);
					}
				}
				else
				{
					XLog.Error("图标点击-->js 注入取图片 获取失败", Array.Empty<object>());
				}
			}
			else
			{
				XLog.Debug("图标点击,大图元素找不到", Array.Empty<object>());
			}
		}

		private string GetPicBase64Data(IWebDriver iframeElWebDriver)
		{
			string str;
			try
			{
				string zhuRuJs = Resources.getBase64Image_tbdj;
				str = (string)SelementHelper.ExecJs(iframeElWebDriver, zhuRuJs, null);
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

		public bool ShiBieGuoCheng(IWebDriver IframeElWebDriver)
		{
			bool flag;
			bool isSuc = false;
			if (0 >= 20)
			{
				flag = isSuc;
			}
			else if (IframeElWebDriver.FinElementByText(By.ClassName("captcha-title"), "按顺序点击：", true, null) != null)
			{
				this.DaTi(IframeElWebDriver);
				flag = isSuc;
			}
			else
			{
				flag = isSuc;
			}
			return flag;
		}
	}
}