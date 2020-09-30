using BCCertMaker;
using Fiddler;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace FiddlerHandle
{
	public class FiddlerMgr
	{
		private const ushort fiddlerCoreListenPort = 8877;

		private readonly static ICollection<Session> sessions;

		private readonly static ReaderWriterLockSlim sessionsLock;

		private readonly static string assemblyDirectory;

		static FiddlerMgr()
		{
			FiddlerMgr.sessions = new List<Session>();
			FiddlerMgr.sessionsLock = new ReaderWriterLockSlim();
			FiddlerMgr.assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		public FiddlerMgr()
		{
			FiddlerMgr.AttachEventListeners();
			FiddlerMgr.EnsureRootCertificate();
			FiddlerMgr.StartupFiddlerCore();
		}

		private static void AttachEventListeners()
		{
			FiddlerApplication.OnNotification += new EventHandler<NotificationEventArgs>((object o, NotificationEventArgs nea) => {
			});
			FiddlerApplication.Log.OnLogString += new EventHandler<LogEventArgs>((object o, LogEventArgs lea) => {
			});
			FiddlerApplication.BeforeRequest += new SessionStateHandler((Session session) => HookMgr.ReplaceHyDeviceJs(session));
			FiddlerApplication.AfterSessionComplete += new SessionStateHandler((Session session) => HookMgr.RspWanCheng(session));
			Console.CancelKeyPress += new ConsoleCancelEventHandler((object o, ConsoleCancelEventArgs ccea) => FiddlerMgr.Quit());
		}

		private static byte charToByte(char c)
		{
			return (byte)"0123456789ABCDEF".IndexOf(c);
		}

		private static string Ellipsize(string text, int length)
		{
			string str;
			if (object.Equals(text, null))
			{
				throw new ArgumentNullException("text");
			}
			if (length < 3)
			{
				throw new ArgumentOutOfRangeException("length", string.Format("{0} cannot be less than {1}", "length", 3));
			}
			str = (text.Length > length ? string.Concat(text.Substring(0, length - 3), new string('.', 3)) : text);
			return str;
		}

		private static void EnsureRootCertificate()
		{
			BCCertMaker.BCCertMaker certProvider = new BCCertMaker.BCCertMaker();
			CertMaker.oCertProvider = certProvider;
			string rootCertificatePath = Path.Combine(FiddlerMgr.assemblyDirectory, "..", "..", "RootCertificate.p12");
			string rootCertificatePassword = "S0m3T0pS3cr3tP4ssw0rd";
			if (File.Exists(rootCertificatePath))
			{
				certProvider.ReadRootCertificateAndPrivateKeyFromPkcs12File(rootCertificatePath, rootCertificatePassword, null);
			}
			else
			{
				certProvider.CreateRootCertificate();
				certProvider.WriteRootCertificateAndPrivateKeyToPkcs12File(rootCertificatePath, rootCertificatePassword, null);
			}
			if (!CertMaker.rootCertIsTrusted())
			{
				CertMaker.trustRootCert();
			}
		}

		private static void ExecuteUserCommands()
		{
			int sessionsCount;
			bool done = false;
			do
			{
				Console.WriteLine("Enter a command [C=Clear; L=List; W=write SAZ; R=read SAZ; Q=Quit]:");
				Console.Write(">");
				ConsoleKeyInfo cki = Console.ReadKey();
				Console.WriteLine();
				char lower = char.ToLower(cki.KeyChar);
				if (lower <= 'l')
				{
					if (lower == 'c')
					{
						try
						{
							FiddlerMgr.sessionsLock.EnterWriteLock();
							FiddlerMgr.sessions.Clear();
						}
						finally
						{
							FiddlerMgr.sessionsLock.ExitWriteLock();
						}
						Console.Title = "Session list contains: 0 sessions";
						FiddlerMgr.WriteCommandResponse("Clear...");
						FiddlerApplication.Log.LogString("Cleared session list.");
					}
					else if (lower == 'l')
					{
						FiddlerMgr.WriteSessions(FiddlerMgr.sessions);
					}
				}
				else if (lower == 'q')
				{
					done = true;
				}
				else if (lower == 'r')
				{
					FiddlerMgr.ReadSessions(FiddlerMgr.sessions);
					try
					{
						FiddlerMgr.sessionsLock.EnterReadLock();
						sessionsCount = FiddlerMgr.sessions.Count;
					}
					finally
					{
						FiddlerMgr.sessionsLock.ExitReadLock();
					}
					Console.Title = string.Format("Session list contains: {0} sessions", sessionsCount);
				}
				else if (lower == 'w')
				{
					string password = null;
					Console.WriteLine("Password Protect this Archive (Y/N)?");
					ConsoleKeyInfo yesNo = Console.ReadKey();
					if ((yesNo.KeyChar == 'y' ? true : yesNo.KeyChar == 'Y'))
					{
						Console.WriteLine(string.Concat(Environment.NewLine, "Enter the password:"));
						password = Console.ReadLine();
					}
					Console.WriteLine();
					FiddlerMgr.SaveSessionsToDesktop(FiddlerMgr.sessions, password);
				}
			}
			while (!done);
		}

		public static byte[] hexStringToBytes(string hexString)
		{
			hexString = hexString.Replace("-", "");
			int length = hexString.Length / 2;
			char[] hexChars = hexString.ToCharArray();
			byte[] d = new byte[length];
			for (int i = 0; i < length; i++)
			{
				int pos = i * 2;
				d[i] = (byte)(FiddlerMgr.charToByte(hexChars[pos]) << 4 | FiddlerMgr.charToByte(hexChars[pos + 1]));
			}
			return d;
		}

		private static void OnWebSocketMessage(object sender, WebSocketMessageEventArgs e)
		{
		}

		public static void Quit()
		{
			FiddlerMgr.WriteCommandResponse("Shutting down...");
			FiddlerApplication.Shutdown();
		}

		private static void ReadSessions(ICollection<Session> sessions)
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			string sazFilename = string.Concat(folderPath, directorySeparatorChar.ToString(), "ToLoad.saz");
			Session[] loaded = Utilities.ReadSessionArchive(sazFilename, false, "", (string file, string part) => {
				Console.WriteLine(string.Concat("Enter the password for ", part, " (or just hit Enter to cancel):"));
				string sResult = Console.ReadLine();
				Console.WriteLine();
				return sResult;
			});
			if ((loaded == null ? false : loaded.Length != 0))
			{
				try
				{
					FiddlerMgr.sessionsLock.EnterWriteLock();
					for (int i = 0; i < (int)loaded.Length; i++)
					{
						sessions.Add(loaded[i]);
					}
				}
				finally
				{
					FiddlerMgr.sessionsLock.ExitWriteLock();
				}
				FiddlerMgr.WriteCommandResponse(string.Format("Loaded: {0} sessions.", (int)loaded.Length));
			}
			else
			{
				FiddlerMgr.WriteCommandResponse(string.Concat("Could not load sessions from ", sazFilename));
			}
		}

		private static void SaveSessionsToDesktop(IEnumerable<Session> sessions, string password)
		{
			string response;
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			string str = Path.DirectorySeparatorChar.ToString();
			DateTime now = DateTime.Now;
			string filename = string.Concat(folderPath, str, now.ToString("hh-mm-ss"), ".saz");
			try
			{
				try
				{
					FiddlerMgr.sessionsLock.EnterReadLock();
					response = (!sessions.Any<Session>() ? "No sessions have been captured." : string.Concat((Utilities.WriteSessionArchive(filename, sessions.ToArray<Session>(), password, false) ? "Wrote" : "Failed to save"), ": ", filename));
				}
				catch (Exception exception)
				{
					response = string.Concat("Save failed: ", exception.Message);
				}
			}
			finally
			{
				FiddlerMgr.sessionsLock.ExitReadLock();
			}
			FiddlerMgr.WriteCommandResponse(response);
		}

		public void SetProxy(Session oSession)
		{
			oSession["X-OverrideGateway"] = "someProxy:1234";
			string userCredentials = string.Format("{0}:{1}", "user", "password");
			string base64UserCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userCredentials));
			oSession.RequestHeaders["Proxy-Authorization"] = string.Concat("Basic ", base64UserCredentials);
		}

		private static void StartupFiddlerCore()
		{
			FiddlerCoreStartupSettings startupSettings = (new FiddlerCoreStartupSettingsBuilder()).ListenOnPort(8877).ChainToUpstreamGateway().DecryptSSL().OptimizeThreadPool().Build();
			FiddlerApplication.Startup(startupSettings);
			FiddlerApplication.Log.LogString(string.Format("Created endpoint listening on port {0}", CONFIG.ListenPort));
			XTrace.WriteLine("Fidder拦截启动成功");
		}

		private static void WriteCommandResponse(string s)
		{
			ConsoleColor oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(s);
			Console.ForegroundColor = oldColor;
		}

		private static void WriteSessions(IEnumerable<Session> sessions)
		{
			ConsoleColor oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			StringBuilder sb = new StringBuilder(string.Concat("Session list contains:", Environment.NewLine));
			try
			{
				FiddlerMgr.sessionsLock.EnterReadLock();
				foreach (Session s in sessions)
				{
					sb.AppendLine(string.Format("{0} {1} {2}", s.id, s.oRequest.headers.HTTPMethod, FiddlerMgr.Ellipsize(s.fullUrl, 60)));
					sb.AppendLine(string.Format("{0} {1}{2}", s.responseCode, s.oResponse.MIMEType, Environment.NewLine));
				}
			}
			finally
			{
				FiddlerMgr.sessionsLock.ExitReadLock();
			}
			Console.Write(sb.ToString());
			Console.ForegroundColor = oldColor;
		}
	}
}