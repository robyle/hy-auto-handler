using FiddlerHandle.AutoWeb;
using Huya.Data.Entity;
using HyBase.HyFile;
using NewLife.Log;
using NewLife.Threading;
using NewLife.Xml;
using System;
using XCode;

namespace FiddlerHandle.Tasks
{
	public class AutoLoginTask : AutoCaoZuoBase
	{
		public AutoLoginTask()
		{
		}

		private void Process()
		{
			string txtPath = XmlConfig<SeleniumConfig>.Current.ZhangHaoTxtPath;
			string[] lines = DataFile.ReadLines(txtPath);
			XTrace.WriteLine(string.Concat("读入账号文本路径", txtPath));
			int i = 0;
			string[] strArrays = lines;
			for (int num = 0; num < (int)strArrays.Length; num++)
			{
				string item = strArrays[num];
				string[] line = item.Split(new string[] { "----" }, StringSplitOptions.RemoveEmptyEntries);
				if ((int)line.Length > 1)
				{
					HyAccount data = HyAccount.FindByUserName(line[0]) ?? new HyAccount();
					data.UserName = line[0];
					data.UserPass = line[1];
					data.Phone = line[2];
					data.PiHao = XmlConfig<SeleniumConfig>.Current.PiHao;
					data.Enable = true;
					data.Save();
					XTrace.WriteLine(string.Format("文本保存导入到数据库完成 导入 {0}", line));
					i++;
				}
			}
			XTrace.WriteLine(string.Format("文本保存导入到数据库完成 导入[{0}]个 完成", i));
		}

		internal void Start()
		{
			ThreadPoolX.QueueUserWorkItem(new Action(this.Process));
		}
	}
}