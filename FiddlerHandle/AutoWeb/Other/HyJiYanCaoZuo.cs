using FastVerCode;
using FiddlerHandle;
using FiddlerHandle.AutoWeb;
using Helper;
using Helper.TuPian;
using NewLife.Log;
using NewLife.Xml;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using PicDaTi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using XS.Helper;
using XS.Helper.StringY;

namespace FiddlerHandle.AutoWeb.Other
{
	public class HyJiYanCaoZuo
	{
		private const string GetCannasBase64PicJs = "return document.getElementById('canvas').toDataURL()";

		public static long id;

		public HyJiYanCaoZuo()
		{
		}

		private double GetDragBtuttonElLeft(IWebElement dragBtuttonEl)
		{
			string drBtLeft = dragBtuttonEl.GetAttribute("style");
			string leftstr = HelperText.getMiddleStr(drBtLeft, "left:", "px", 0);
			return leftstr.ToDouble(0);
		}

		public PointX GetPoint(string bigPicSrc, string smallPicSrc, int smallShiJiY, out PointX bigYuan, out PointX samllYuan)
		{
			PointX pointX;
			PointX p = this.GetPoint4MyServer(bigPicSrc, smallPicSrc, smallShiJiY, out bigYuan, out samllYuan);
			if ((p == null || bigYuan == null ? false : samllYuan != null))
			{
				p.X = p.X + 18;
				p.Y = p.Y + 18;
				pointX = p;
			}
			else
			{
				pointX = null;
			}
			return pointX;
		}

		public PointX GetPoint4LianZhong(string bigPicSrc, string smallPicSrc, out PointX bigYuan, out PointX samllYuan)
		{
			int yuanWidth;
			int yuanHidth;
			int smW;
			int smH;
			byte[] bigPicByte = ImageHelper.Base64ToImage(bigPicSrc, out yuanWidth, out yuanHidth);
			bigYuan = new PointX()
			{
				X = yuanWidth,
				Y = yuanHidth
			};
			byte[] smallPicByte = ImageHelper.Base64ToImage(smallPicSrc, out smW, out smH);
			samllYuan = new PointX()
			{
				X = smW,
				Y = smH
			};
			PointX p = null;
			if ((bigPicByte == null || (int)bigPicByte.Length <= 10 || smallPicByte == null ? false : (int)smallPicByte.Length > 10))
			{
				JsDaTi dtMgr = new JsDaTi();
				if (dtMgr.GetZuoBiao(bigPicByte, 1318))
				{
					p = new PointX()
					{
						X = dtMgr.offsetX,
						Y = dtMgr.offsetY
					};
				}
			}
			int rd = HelperGeneral.Random.Next(1, 9999999);
			PicDaTiHelper.SavePic(bigPicByte, "null", string.Format("big_{0}_{1}_{2}", p.X, p.Y, rd), "bmp");
			PicDaTiHelper.SavePic(smallPicByte, "null", string.Format("small_{0}_{1}_{2}", p.X, p.Y, rd), "bmp");
			return p;
		}

		public PointX GetPoint4MyServer(string bigPicSrc, string smallPicSrc, int smallShiJiY, out PointX bigYuan, out PointX samllYuan)
		{
			string bigBase64 = ImageHelper.TakeOutHeand(bigPicSrc);
			string smallBase64 = ImageHelper.TakeOutHeand(smallPicSrc);
			PointX p = HuaKuai.GetXY(XmlConfig<SeleniumConfig>.Current.HuaKuaiServer, bigBase64, smallBase64, smallShiJiY, out bigYuan, out samllYuan);
			long num = HyJiYanCaoZuo.id;
			HyJiYanCaoZuo.id = num + (long)1;
			HyJiYanCaoZuo.id = num;
			ImageHelper.Base64ToImage(bigPicSrc);
			ImageHelper.Base64ToImage(smallPicSrc);
			return p;
		}

		private double GetSmallElLeft(IWebElement smallEl)
		{
			string drBtLeft = smallEl.GetAttribute("style");
			string leftstr = HelperText.getMiddleStr(drBtLeft, "left:", "px", 0);
			return leftstr.ToDouble(0);
		}

		private bool MouseMoveTaget(int x, int y, Actions actions, IWebElement dragBtuttonEl, double tageX_left, double dianJianP_X, IWebElement slideEl, IWebElement bgEl, IWebElement dragTrackEl, IWebDriver webDriver)
		{
			List<int> track = (new MouseMove()).get_track(x);
			if ((track == null ? false : track.Count > 0))
			{
				int last = 0;
				int i = 0;
				while (i < track.Count)
				{
					int realy = y + HelperGeneral.Random.Next(-3, 3);
					int cur = track[i] + last;
					actions.MoveToElement(bgEl, cur, realy).Perform();
					last = cur;
					XTrace.WriteLine(string.Format("鼠标前进 x{0}", track[i]));
					Thread.Sleep(100);
					double dragLeft = this.GetDragBtuttonElLeft(dragBtuttonEl);
					double smalLeft = this.GetSmallElLeft(slideEl);
					if (Math.Abs((double)x - dianJianP_X - dragLeft) > 5)
					{
						XTrace.WriteLine(string.Format("按钮位置 {0} 小图位置{1}  目的地:{2}", dragLeft, smalLeft, tageX_left));
						Thread.Sleep(300);
						i++;
					}
					else
					{
						XTrace.WriteLine(string.Format("按钮位置 {0} 接近目的地 {1} 跳出 ", dragLeft, tageX_left));
						break;
					}
				}
			}
			return false;
		}

		public bool Run(IWebDriver IframeElWebDriver)
		{
			PointX bigYuan;
			PointX samllYuan;
			bool isSuc = false;
			IWebElement bgEl = IframeElWebDriver.FinElement(By.ClassName("bg-placeholder"));
			IWebElement slideEl = IframeElWebDriver.FinElement(By.Id("slide-block"));
			IWebElement smallE1 = IframeElWebDriver.FinElement(By.XPath("//div[@id=\"slide-block\"]/img")) ?? IframeElWebDriver.FinElement(By.XPath("/html/body/div[1]/div[2]/div[1]/div[2]/img"));
			IWebElement bgCanvasEl = IframeElWebDriver.FinElement(By.Id("canvas"));
			IWebElement dragBtuttonEl = IframeElWebDriver.FinElement(By.ClassName("drag-button"));
			IWebElement dragTrackEl = IframeElWebDriver.FinElement(By.ClassName("drag-track"));
			if ((bgEl == null || smallE1 == null || dragBtuttonEl == null ? false : bgCanvasEl != null))
			{
				string smallPicSrc = smallE1.GetAttribute("src");
				string bigPicSrc = SelementHelper.ExecJs(IframeElWebDriver, "return document.getElementById('canvas').toDataURL()", Array.Empty<object>()) as string;
				if ((bigPicSrc == null ? false : smallPicSrc != null))
				{
					int samllLocationY = smallE1.Location.Y - bgCanvasEl.Location.Y;
					int smallShiJiY = (int)((double)samllLocationY * 1.20805369127517 - 1);
					PointX p = this.GetPoint(bigPicSrc, smallPicSrc, smallShiJiY, out bigYuan, out samllYuan);
					if (p != null)
					{
						double x = (double)bigYuan.X;
						Size size = bgEl.Size;
						double elWidthP = x / (double)size.Width;
						size = bgEl.Size;
						double smallYinBig = (double)size.Height / (double)bigYuan.Y;
						int sjX = HelperGeneral.Random.Next(13, 53);
						int sjY = HelperGeneral.Random.Next(13, 53);
						int ysX = sjX;
						int ysY = sjY;
						double dianJianP_X = (double)ysX;
						double tageX_left = (double)p.X / (double)elWidthP;
						double tageX = tageX_left + dianJianP_X;
						double k = 0.938;
						double d = 256 + dianJianP_X - tageX;
						double realX = tageX - (1 - k) / k * d;
						int realY = p.Y + sjY;
						Actions actions = new Actions(IframeElWebDriver);
						SelementHelper.MoveTo(IframeElWebDriver, smallE1, ysX, ysY);
						actions.ClickAndHold().Perform();
						XTrace.WriteLine("开始移动到{tageX}");
						this.MouseMoveTaget((int)realX, realY, actions, dragBtuttonEl, tageX_left, dianJianP_X, slideEl, bgEl, dragTrackEl, IframeElWebDriver);
						actions.MoveToElement(bgEl, (int)realX, realY).Perform();
						Thread.Sleep(2000);
						double smallLeft = this.GetSmallElLeft(slideEl);
						double dragLeft = this.GetDragBtuttonElLeft(dragBtuttonEl);
						if (Math.Abs(realX - dianJianP_X - dragLeft) > 1)
						{
							XTrace.WriteLine(string.Format("当前模块位置 {0} 目标位置{1}    小图片当前左上角位置{2}", dragLeft, realX - dianJianP_X, smallLeft));
							actions.MoveToElement(bgEl, (int)realX, 20).Perform();
							Thread.Sleep(300);
						}
						else
						{
							XTrace.WriteLine(string.Format("到达目标点{0} 当前按钮位置{1}   小图片当前左上角位置{2}  目的左上角{3}", new object[] { realX - dianJianP_X, dragLeft, smallLeft, tageX_left }));
							Thread.Sleep(1000);
						}
						XTrace.WriteLine("松开鼠标...");
						actions.Release().Perform();
						Thread.Sleep(2000);
						isSuc = true;
						Thread.Sleep(1000);
					}
				}
			}
			return isSuc;
		}

		public void Test()
		{
		}
	}
}