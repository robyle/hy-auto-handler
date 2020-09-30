using NewLife.Xml;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThirdParty.Model;

namespace FiddlerHandle.AutoWeb
{
	[Description("操作浏览器配置")]
	[XmlConfigFile("Config\\Selenium.config", 15000)]
	public class SeleniumConfig : XmlConfig<SeleniumConfig>
	{
		[Description("开启宽带拨号 默认不开启")]
		public bool ADSLBoHao
		{
			get;
			internal set;
		}

		[Description("开启Fiddler记录文本输出  默认开启")]
		public bool FiddlerLogString { get; internal set; } = true;

		[Description("开启Fiddler用户通知文本输出 默认开启")]
		public bool FiddlerNotifyUser { get; internal set; } = true;

		[Description("首次打开页面 https://i.huya.com/ 或 https://www.huya.com/  或 https://www.huya.com/房间号 等等 不同的页面可能可以躲过 弹扫码登录")]
		public string FirstUrl { get; set; } = "https://i.huya.com/";

		[Description("服务名称 默认 320823")]
		public string FuWuName { get; internal set; } = "320823";

		[Description("谷歌浏览器,账号对应缓存信息保存路径 默认 C:\\cache ,如果跟换这个路径,请把原路径的账号的缓存一起复制,就可以保存用户的cookie等等信息了,这个文件夹有点大,请设置足够的空间")]
		public string GoogleUserCache { get; internal set; } = "C:\\cache";

		[Description("滑块服务器地址")]
		public string HuaKuaiServer { get; set; } = "http://47.99.98.15:8801";

		[Description("账号数据库全路径 例如 E:\\huya\\HyData.db")]
		public string HyAccountDb
		{
			get;
			set;
		}

		[Description("true=只获取登录成功的账号")]
		public bool IsLogin
		{
			get;
			internal set;
		}

		[Description("机器码信息数据库全路径")]
		public string JiQiMaDb
		{
			get;
			set;
		}

		[Description("是否执行任务 领取百宝箱 默认不执行")]
		public bool LingBaoXiang
		{
			get;
			internal set;
		}

		[Description("领宝箱等待时间,单位分钟 默认 3")]
		public int LingBaoXiangWaitingTime { get; internal set; } = 3;

		[Description("是否执行任务 领取任务奖励 默认不执行")]
		public bool LingTask
		{
			get;
			internal set;
		}

		[Description("注册账号使用的 短信接收平台类型 默认 MDS,美国=1")]
		public MobileType PhonePtType { get; internal set; } = MobileType.MDS;

		[Description("手动输入手机验证码")]
		public bool PhoneSmsShouDong
		{
			get;
			internal set;
		}

		[Description("批号,默认0,取全部,目前账号 目前只支持虎牙账号")]
		public int PiHao { get; internal set; } = 0;

		[Description("宽带密码 默认 320823")]
		public string PPOEMima { get; internal set; } = "320823";

		[Description("宽带账号 默认 052712301037")]
		public string PPOEZh { get; internal set; } = "052712301037";

		[Description("宽带连接名 默认 AutoADSL")]
		public string PPPOEname { get; internal set; } = "AutoADSL";

		[Description("代理读取路径 可以是http连接(连接中的 &替换成&amp; ) 或者本地文本路径,默认http://140.246.151.84:8022/call/proxy?id=e1a5a2c51aff56e8a898598ee144e254&amp;count=50")]
		public string ProxyPatch { get; internal set; } = "http://140.246.151.84:8022/call/proxy?id=e1a5a2c51aff56e8a898598ee144e254&amp;count=50";

		[Description("生成随机名字 0.随机 1.男 2.女 3个性名字   5.3000个,个性名字  默认5")]
		public int RdName { get; internal set; } = 5;

		[Description("生成随机名字 是否在随机位置 插入随机数字字母")]
		public bool RdNameAndNum { get; internal set; } = false;

		[Description("是否执行任务 发送虎粮礼物 默认不执行")]
		public bool SendHyLiangGift
		{
			get;
			internal set;
		}

		[Description("是否执行任务 订阅 默认不执行")]
		public bool Subscribe
		{
			get;
			internal set;
		}

		[Description("开启调试,做一些测试方面的公子 真开启")]
		public bool Test
		{
			get;
			internal set;
		}

		[Description("修改头像时,头像图片文件夹目录")]
		public string TouXiang
		{
			get;
			internal set;
		}

		[Description("执行任务之前是否设置cookie,一般其他任务比如领箱子等等,更新了cookie,这里刷活跃之前要先设置cookie 默认为真,有cookie信息,就先设置cookie")]
		public bool UpCookie { get; internal set; } = true;

		[Description("使用代理,true ,使用代理 ADSLBoHao应该设置为 false")]
		public bool UseProxy
		{
			get;
			internal set;
		}

		[Description("是否执行任务 修改昵称 默认不执行  true false")]
		public bool XiuGaiNick
		{
			get;
			internal set;
		}

		[Description("是否执行任务 修改头像 默认不执行")]
		public bool XiuGaiTouXiang
		{
			get;
			internal set;
		}

		[Description("文本账号[格式 账号----密码----电话号码(可空) (一行一个)]登录的txt文本全路径")]
		public string ZhangHaoTxtPath
		{
			get;
			internal set;
		}

		[Description("注册成功后 随机进入房间,并发送一条弹幕(一般是为了取guid),并根据配置 执行 比如 订阅,发送虎粮,领任务,领宝箱(上面的配置项决定)")]
		public bool ZhuCeInRoom
		{
			get;
			internal set;
		}

		public SeleniumConfig()
		{
		}
	}
}