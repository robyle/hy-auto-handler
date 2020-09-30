using CsharpHttpHelper;
using Fiddler;
using Huya.Data.Entity;
using HyBase.VplayerUi.Net;
using NewLife.Collections;
using NewLife.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using XS.Helper.Http;

namespace FiddlerHandle
{
	public class HookMgr
	{
		public static Dictionary<int, Session> WebSokcetDic;

		public static string curCookie
		{
			get;
			set;
		}

		public static JiQiMa jiQiMa
		{
			get;
			set;
		}

		public static bool NeedQuickReg
		{
			get;
			set;
		}

		public static bool NeedWeiBoYanZhengMa
		{
			get;
			set;
		}

		public static bool PhoneIsReg
		{
			get;
			set;
		}

		public static bool UpCookie
		{
			get;
			set;
		}

		public static byte[] WeiBoYanZhengMa
		{
			get;
			set;
		}

		public static object WeiBoYanZhengMaLock
		{
			get;
			set;
		}

		static HookMgr()
		{
			HookMgr.WeiBoYanZhengMaLock = new object();
			HookMgr.WebSokcetDic = new Dictionary<int, Session>();
		}

		public HookMgr()
		{
		}

		public static byte[] DecodeMask(WebSocketMessage oWSM)
		{
			byte[] payloadData;
			if (oWSM.MaskingKey == null)
			{
				payloadData = oWSM.PayloadData;
			}
			else
			{
				byte[] MaskingKey = oWSM.MaskingKey;
				int maskLen = (int)MaskingKey.Length;
				byte[] payload = oWSM.PayloadData;
				byte[] s = new byte[oWSM.PayloadLength];
				int i = 0;
				for (int j = 0; j < (int)payload.Length; j++)
				{
					int num = i;
					i = num + 1;
					s[j] = (byte)(payload[j] ^ MaskingKey[num % maskLen]);
				}
				payloadData = s.ToArray<byte>(0);
			}
			return payloadData;
		}

		public static bool IsWebSocket(Session session)
		{
			bool flag;
			XTrace.WriteLine(string.Concat("BeforeRequest ", session.fullUrl, " "));
			if (!session.bHasWebSocketMessages)
			{
				flag = false;
			}
			else
			{
				lock (HookMgr.WebSokcetDic)
				{
					HookMgr.WebSokcetDic[session.id] = session;
				}
				flag = true;
			}
			return flag;
		}

		public static string LockByteArray(byte[] data, string beizhu = "")
		{
			StringBuilder sb = new StringBuilder();
			byte[] numArray = data;
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				byte d = numArray[i];
				sb.Append(string.Format("{0},", d));
			}
			string str = sb.ToString();
			XTrace.WriteLine(string.Format("LockByteArray {0} 长度[{1}]{2}", beizhu, (int)data.Length, str));
			return str;
		}

		private static void NeedImmediatelyReturn(Session ReqSession)
		{
			if ((ReqSession.fullUrl.IndexOf("?wsSecret=") == -1 ? false : ReqSession.fullUrl.IndexOf("https://") != -1))
			{
			}
		}

		public static void OnWebSocketMessage(object sender, WebSocketMessageEventArgs e)
		{
			Session session = (Session)sender;
			if ((session.fullUrl.IndexOf("-ws.va.huya.com") != -1 ? true : session.fullUrl.IndexOf("wsapi.huya.com") != -1))
			{
				HookMgr.LockByteArray(e.oWSM.PayloadData, string.Format("Mask={0} Fiddler原数据    ", e.oWSM.MaskingKey != null));
				byte[] arr = e.oWSM.PayloadAsBytes();
				HookMgr.LockByteArray(arr, string.Concat(string.Format("{0}  控制:{1} {2}", session.fullUrl, e.oWSM.FrameType, e.oWSM.IsFinalFrame), string.Format(" {0} Mask={1} Fiddler解Mask的    ", (e.oWSM.IsOutbound ? "发送" : "接收"), e.oWSM.MaskingKey != null)));
				WebSocket webs = session.__oTunnel as WebSocket;
				if (arr.Length != 0)
				{
					try
					{
						XTrace.WriteLine(string.Format("消息列表 {0} 个", webs.listMessages.Count));
						if (!e.oWSM.IsOutbound)
						{
							HookHyWebSocketNet.RecvData(arr);
						}
						else
						{
							HookHyWebSocketNet.SendData(arr);
						}
					}
					catch (Exception exception)
					{
						XTrace.WriteLine(string.Concat("序列化包出错 ", exception.ToString()));
					}
				}
			}
		}

		private static void PhoneIsRegFun(Session session)
		{
			if (HookMgr.NeedQuickReg)
			{
				if (session.fullUrl.IndexOf("udbreg.huya.com/sms/v2/send/reg") != -1)
				{
					session.utilDecodeResponse();
					if (session.utilFindInResponse("用户已存在", false) != -1)
					{
						HookMgr.PhoneIsReg = true;
					}
				}
			}
		}

		public static void ReplaceHyDeviceJs(Session ReqSession)
		{
			if (ReqSession.fullUrl.IndexOf("google.com") == -1)
			{
				if ((!HookMgr.UpCookie ? false : ReqSession.fullUrl.IndexOf(":443") == -1))
				{
					ReqSession.bBufferResponse = true;
					string old = ReqSession.oRequest.headers["Cookie"];
					string UserCookie = HttpHelper.GetMergeCookie(HookMgr.curCookie, old);
					ReqSession.oRequest.headers["Cookie"] = UserCookie;
				}
				HookMgr.NeedImmediatelyReturn(ReqSession);
			}
			else
			{
				ReqSession.bBufferResponse = true;
				ReqSession.utilCreateResponseAndBypassServer();
				ReqSession.oResponse.headers.SetStatus(502, "Failed");
				ReqSession.oResponse.headers.HTTPResponseCode = 502;
				ReqSession.oResponse.headers.HTTPVersion = "HTTP/1.1";
			}
		}

		public static void RspWanCheng(Session session)
		{
			if (session.bHasResponse)
			{
				if (HookMgr.NeedWeiBoYanZhengMa)
				{
					if (session.fullUrl.IndexOf("login.sina.com.cn/cgi/pin.php?r=") != -1)
					{
						session.utilDecodeResponse();
						lock (HookMgr.WeiBoYanZhengMaLock)
						{
							HookMgr.WeiBoYanZhengMa = session.responseBodyBytes;
						}
					}
				}
				HookMgr.PhoneIsRegFun(session);
				HookMgr.ylog(session);
			}
		}

		private static void ylog(Session session)
		{
			string str;
			string str1;
			if (session.url.IndexOf("ylog.huya.com/g.gif") != -1)
			{
				try
				{
					if (session.RequestHeaders.HTTPMethod != "GET")
					{
						object[] objArray = JsonConvert.DeserializeObject<object[]>(session.GetRequestBodyAsString());
						for (int i = 0; i < (int)objArray.Length; i++)
						{
							dynamic item = objArray[i];
							if (item.eid_desc != (dynamic)null)
							{
								XTrace.WriteLine(string.Format("POST ylog  eid=[{0}] eid_desc=[{1}]", item.eid, HttpHelper.URLDecode(item.eid_desc.ToString(), Encoding.UTF8)));
							}
							else if (item.eid != (dynamic)null)
							{
								XTrace.WriteLine(string.Format("POST ylog  eid=[{0}]", item.eid));
							}
							else if (item.rid == (dynamic)null)
							{
								XTrace.WriteLine(string.Format("POST ylog  {0}", JsonConvert.SerializeObject(item)));
							}
							else
							{
								XTrace.WriteLine(string.Format("POST ylog  rid=[{0}]", item.rid));
							}
						}
					}
					else
					{
						Tuple<string, IEnumerable<KeyValuePair<string, string>>> data = XHttpHelper.UrlToData(session.fullUrl);
						IEnumerable<KeyValuePair<string, string>> pa = data.Item2;
						if (pa == null)
						{
							XTrace.WriteLine(string.Concat("GET ylog 参数为空  ", data.Item1));
						}
						else
						{
							NullableDictionary<string, string> dic = XHttpHelper.EumKV2Dic(pa);
							if (dic["eid_desc"] != null)
							{
								string[] strArrays = new string[] { "Get ylog  eid=[", dic["eid"], "] eid_desc=[", null, null };
								string str2 = dic["eid_desc"];
								if (str2 != null)
								{
									str1 = str2.ToString();
								}
								else
								{
									str1 = null;
								}
								strArrays[3] = HttpHelper.URLDecode(str1, Encoding.UTF8);
								strArrays[4] = "]";
								XTrace.WriteLine(string.Concat(strArrays));
							}
							else if (dic["eid"] != null)
							{
								XTrace.WriteLine(string.Concat("GET ylog  eid=[", dic["eid"], "]"));
							}
							else if (dic["rid"] == null)
							{
								XTrace.WriteLine(string.Concat("GET ylog  ", string.Join<KeyValuePair<string, string>>(" ", pa)));
							}
							else
							{
								XTrace.WriteLine(string.Concat("GET ylog  rid=[", dic["rid"], "]"));
							}
						}
						return;
					}
				}
				catch (Exception exception)
				{
					Exception ex = exception;
					XTrace.WriteLine(string.Concat("分析虎牙记录请求出错 ", ex.Message, " ", session.url));
				}
			}
			else if (session.url.IndexOf("huya.com/d.gif") != -1)
			{
				if (session.RequestHeaders.HTTPMethod == "GET")
				{
					Tuple<string, IEnumerable<KeyValuePair<string, string>>> data = XHttpHelper.UrlToData(session.fullUrl);
					IEnumerable<KeyValuePair<string, string>> pa = data.Item2;
					if (pa == null)
					{
						XTrace.WriteLine(string.Concat("GET ylog 参数为空  ", data.Item1));
					}
					else
					{
						NullableDictionary<string, string> dic = XHttpHelper.EumKV2Dic(pa);
						if (dic["eid_desc"] != null)
						{
							string[] strArrays1 = new string[] { "Get ylog  eid=[", dic["eid"], "] eid_desc=[", null, null };
							string str3 = dic["eid_desc"];
							if (str3 != null)
							{
								str = str3.ToString();
							}
							else
							{
								str = null;
							}
							strArrays1[3] = HttpHelper.URLDecode(str, Encoding.UTF8);
							strArrays1[4] = "]";
							XTrace.WriteLine(string.Concat(strArrays1));
						}
						else if (dic["eid"] != null)
						{
							XTrace.WriteLine(string.Concat("GET ylog  eid=[", dic["eid"], "]"));
						}
						else if (dic["rid"] == null)
						{
							XTrace.WriteLine(string.Concat("GET ylog  ", string.Join<KeyValuePair<string, string>>(" ", pa)));
						}
						else
						{
							XTrace.WriteLine(string.Concat("GET ylog  rid=[", dic["rid"], "]"));
						}
					}
				}
			}
		}
	}
}