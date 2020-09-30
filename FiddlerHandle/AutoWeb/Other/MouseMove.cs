using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using XS.Helper;

namespace FiddlerHandle.AutoWeb.Other
{
	public class MouseMove
	{
		public MouseMove()
		{
		}

		private double ease_out_bounce(double x)
		{
			double num;
			double n1 = 7.5625;
			double d1 = 2.75;
			if (x < 1 / d1)
			{
				num = n1 * x * x;
			}
			else if (x < 2 / d1)
			{
				x = x - 1.5 / d1;
				num = n1 * x * x + 0.75;
			}
			else if (x >= 2.5 / d1)
			{
				x = x - 2.625 / d1;
				num = n1 * x * x + 0.984375;
			}
			else
			{
				x = x - 2.25 / d1;
				num = n1 * x * x + 0.9375;
			}
			return num;
		}

		private double ease_out_expo(double x)
		{
			double num;
			num = (x != 1 ? 1 - Math.Pow(2, -10 * x) : 1);
			return num;
		}

		private double ease_out_quad(double x)
		{
			double num = 1 - (1 - x) * (1 - x);
			return num;
		}

		private double ease_out_quart(double x)
		{
			double num = 1 - Math.Pow(1 - x, 4);
			return num;
		}

		public List<int> get_track(int distance)
		{
			return this.get_track_4Pid(distance);
		}

		private List<int> get_track_4jiasudu(double distance)
		{
			List<int> track = new List<int>();
			double current = 0;
			double mid = distance * 4 / 5;
			double t = 0.2;
			double v = 0;
			double a = 0;
			while (current < distance)
			{
				if (current >= mid)
				{
					a = -3;
					double v0 = v;
					v = v0 + a * t;
					double move = v0 * t + 0 * a * t * t;
					current += move;
					track.Add((int)Math.Round(move));
				}
				else
				{
					a = 2;
				}
			}
			int trackSum = track.Sum();
			if ((double)trackSum != distance)
			{
				XTrace.WriteLine(string.Format("移动总数{0}  和 目标距离 {1}  不符合", trackSum, distance));
			}
			return track;
		}

		private List<int> get_track_4Pid(int distance)
		{
			List<int> list;
			while (true)
			{
				list = PositionalPID.get_pid_track(distance);
				if (list.Count < 15)
				{
					break;
				}
				XTrace.WriteLine(string.Format("数组过大 {0} {1}", list.Count, 15));
			}
			return list;
		}

		private List<int> get_tracks_4easeOut(int distance, int seconds, Func<double, double> ease_func)
		{
			List<int> tracks = new List<int>();
			List<double> offsets = new List<double>();
			double lastOffsets = 0;
			for (int t = 0; t < seconds; t += 100)
			{
				double offset = Math.Round(ease_func((double)(t / seconds)));
				tracks.Add((int)(offset - lastOffsets));
				lastOffsets = offset;
				offsets.Add(offset);
			}
			int trackSum = tracks.Sum();
			if (trackSum != distance)
			{
				XTrace.WriteLine(string.Format("移动总数{0}  和 目标距离 {1}  不符合", trackSum, distance));
			}
			return tracks;
		}
	}
}