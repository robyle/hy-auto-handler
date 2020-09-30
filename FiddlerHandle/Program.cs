using EasyHookSocket;
using FiddlerHandle.AutoWeb;
using FiddlerHandle.Tasks;
using HyBase.HyFile;
using HyBase.Web;
using NewLife.Log;
using NewLife.Xml;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using TaskBase;

namespace FiddlerHandle
{
	internal class Program
	{
		private static Program.ControlCtrlDelegate cancelHandler;

		private static AutoCaoZuoBase AutoCaoZuo
		{
			get;
			set;
		}

		private static FiddlerMgr FMgr
		{
			get;
			set;
		}

		private static WinSockConnectController HookPrpxy
		{
			get;
			set;
		}

		static Program()
		{
			Program.cancelHandler = new Program.ControlCtrlDelegate(Program.HandlerRoutine);
		}

		public Program()
		{
		}

		public static void ClearProcess()
		{
			Process curP = Process.GetCurrentProcess();
			Process[] processes = Process.GetProcesses();
			for (int i = 0; i < (int)processes.Length; i++)
			{
				Process p = processes[i];
				string info = "";
				try
				{
					info = string.Concat(new object[] { p.Id, "  ", p.ProcessName, "  ", p.MainWindowTitle, "  ", p.StartTime });
					if (curP.Id == p.Id)
					{
						continue;
					}
					else if ((p.ProcessName.IndexOf("chromedriver") != -1 ? true : p.ProcessName.IndexOf("FiddlerHandle") != -1))
					{
						XTrace.WriteLine(info);
						p.Kill();
						p.WaitForExit();
						p.Close();
					}
				}
				catch (Exception exception)
				{
					info = exception.Message;
				}
			}
		}

		public static void EndFidder()
		{
			FiddlerMgr.Quit();
		}

		public static bool HandlerRoutine(int CtrlType)
		{
			AutoCaoZuoBase autoCaoZuo = Program.AutoCaoZuo;
			if (autoCaoZuo != null)
			{
				autoCaoZuo.Close();
			}
			else
			{
			}
			FiddlerMgr.Quit();
			int ctrlType = CtrlType;
			if (ctrlType == 0)
			{
				Console.WriteLine("0工具被强制关闭");
			}
			else if (ctrlType == 2)
			{
				Console.WriteLine("2工具被强制关闭");
			}
			Program.ClearProcess();
			Thread.Sleep(100);
			return false;
		}

		private static void Main(string[] args)
		{
			XTrace.UseConsole(true, true);
			XTrace.WriteLine("浏览器操作");
			int p = XmlConfig<SeleniumConfig>.Current.PiHao;
			//string ph = DataFile.WtingRead("请输入配置文件 ", "输入的配置路径:", (string path) => (path.IndexOf(".config") == -1 ? false : XmlConfig<SeleniumConfig>.Current.Load(path)));
				string ph = @"Config\Selenium.config";
				XTrace.WriteLine($"加载的配置地址:" + ph);
				XmlConfig<SeleniumConfig>.Current.Load(ph);
				XmlConfig<SeleniumConfig>.Current.ConfigFile = ph;
				XmlConfig<SeleniumConfig>.Current.Load(ph);

			
			XmlConfig<SeleniumConfig>.Current.ConfigFile = ph;
			XmlConfig<SeleniumConfig>.Current.Load(ph);
			TaskTool.AddHyDataConneStr(XmlConfig<SeleniumConfig>.Current.HyAccountDb);
			TaskTool.AddJiQiMaConnStr(XmlConfig<SeleniumConfig>.Current.JiQiMaDb);
			Program.ClearProcess();
			Program.SetConsoleCtrlHandler(Program.cancelHandler, true);
			ModifyHead.LoadPicFileInfo(XmlConfig<SeleniumConfig>.Current.TouXiang);
			switch (DataFile.WtingReadInt("任务类型 0=刷活跃 1=快速注册 2=文本账号保存到数据库", "输入的任务类型:"))
			{
				case 0:
				{
					AutoHuoYueCaoZuo huoyue = new AutoHuoYueCaoZuo();
					Program.AutoCaoZuo = huoyue;
					huoyue.Start();
					break;
				}
				case 1:
				{
					int needCount = DataFile.WtingReadInt("需要注册的数量:", "输入的注册数量:");
					AutoQuickReg reg = new AutoQuickReg();
					Program.AutoCaoZuo = reg;
					reg.Start(needCount);
					break;
				}
				case 2:
				{
					AutoLoginTask Login = new AutoLoginTask();
					Program.AutoCaoZuo = Login;
					Login.Start();
					break;
				}
			}
			DataFile.WatingOut();
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool SetConsoleCtrlHandler(Program.ControlCtrlDelegate HandlerRoutine, bool Add);

		public static void StartFidder()
		{
			Program.FMgr = new FiddlerMgr();
			HySeleniumMgr.OpenProxy = true;
		}

		public static void TestPhone()
		{
		}

		public delegate bool ControlCtrlDelegate(int CtrlType);
	}
}