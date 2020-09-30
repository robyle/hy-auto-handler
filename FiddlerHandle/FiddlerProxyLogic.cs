using Base.Model.Proxy;
using FiddlerHandle.AutoWeb;
using Helper;
using Helper.Http.HttpProxys;
using NewLife.Xml;
using SuperSocket.ClientEngine;
using SuperSocket.ClientEngine.Proxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using ThirdParty.ProxyMgr.Proxy;

namespace FiddlerHandle
{
	public class FiddlerProxyLogic
	{
		private AutoResetEvent WaitRes = new AutoResetEvent(false);

		protected static string Password
		{
			get;
			private set;
		}

		protected ProxyConnectorBase Proxy
		{
			get;
			set;
		}

		protected bool ProxyConnected
		{
			get;
			private set;
		}

		private static Dictionary<string, ProxyInfo> ProxyDic
		{
			get;
			set;
		}

		protected static EndPoint ProxyEndPoint
		{
			get;
			private set;
		}

		private static IList<ProxyInfo> ProxyList
		{
			get;
			set;
		}

		protected System.Net.Sockets.Socket Socket
		{
			get;
			private set;
		}

		protected static string Username
		{
			get;
			private set;
		}

		public FiddlerProxyLogic()
		{
		}

		protected virtual bool ConnectProxy(EndPoint endPoint, int Timeout = 20000)
		{
			this.SetProxy();
			this.Proxy.Completed += new EventHandler<ProxyEventArgs>(this.Proxy_Completed);
			this.Proxy.Connect(endPoint);
			Debug.WriteLine("ConnectProxy 等待 ");
			this.WaitRes.WaitOne(Timeout);
			Debug.WriteLine(string.Format("ConnectProxy 返回 {0}", this.ProxyConnected));
			return this.ProxyConnected;
		}

		public static void GetProxy(string ip = null)
		{
			if (XmlConfig<SeleniumConfig>.Current.UseProxy)
			{
				ProxyInfo proxy = null;
				if (!ip.IsNullOrWhiteSpace())
				{
					if (FiddlerProxyLogic.ProxyDic.ContainsKey(ip))
					{
						proxy = FiddlerProxyLogic.ProxyDic[ip];
					}
				}
				if (proxy == null)
				{
					int index = HelperGeneral.Random.Next(0, FiddlerProxyLogic.ProxyList.Count - 1);
					proxy = FiddlerProxyLogic.ProxyList[index];
				}
				EndPoint ipEnd = HttpProxyBase.GetEndPoint(proxy.IP, proxy.Port);
				FiddlerProxyLogic.UpProxy(ipEnd, proxy.ProxyUser, proxy.ProxyPass);
			}
		}

		public static void LoadProxy()
		{
			ProxyMgr.Instance.ReadProxy4PatchOrUrl(XmlConfig<SeleniumConfig>.Current.ProxyPatch, true);
			FiddlerProxyLogic.ProxyList = ProxyMgr.Instance.Proxylist;
			FiddlerProxyLogic.ProxyDic = ProxyMgr.Instance.ProxyDic;
			if (FiddlerProxyLogic.ProxyList.Count <= 0)
			{
				throw new Exception("代理加载不到,个数为0");
			}
		}

		private void Proxy_Completed(object sender, ProxyEventArgs e)
		{
			string str;
			this.Proxy.Completed -= new EventHandler<ProxyEventArgs>(this.Proxy_Completed);
			this.ProxyConnected = e.Connected;
			string targetHostName = e.TargetHostName;
			Exception exception = e.Exception;
			if (exception != null)
			{
				str = exception.ToString();
			}
			else
			{
				str = null;
			}
			Debug.WriteLine(string.Concat("Proxy_Completed ", targetHostName, " ", str));
			if (e.Connected)
			{
				this.Socket = e.Socket;
			}
			this.WaitRes.Set();
		}

		private static System.Net.Sockets.Socket ProxyConnect(IPEndPoint iPEndPoint)
		{
			FiddlerProxyLogic p = new FiddlerProxyLogic();
			p.ConnectProxy(iPEndPoint, 20000);
			return p.Socket;
		}

		public static void SetFindderUseS5Proxy()
		{
			if (XmlConfig<SeleniumConfig>.Current.UseProxy)
			{
				FiddlerProxyLogic.LoadProxy();
			}
		}

		private void SetProxy()
		{
			this.Proxy = new TSocks5Connector(FiddlerProxyLogic.ProxyEndPoint, FiddlerProxyLogic.Username, FiddlerProxyLogic.Password);
		}

		private static void UpProxy(EndPoint proxyEndPoint, string username, string password)
		{
			FiddlerProxyLogic.ProxyEndPoint = proxyEndPoint;
			FiddlerProxyLogic.Username = username;
			FiddlerProxyLogic.Password = password;
		}
	}
}