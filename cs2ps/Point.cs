using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs2ps
{
	public class Point
	{
		public Point()
		{
			X = 0;
			Y = 0;
		}
		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}

		public static Point operator +(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}

		public double X { get; set; }
		public double Y { get; set; }

		public string ToString()
		{
			return X.ToString() + " " + Y.ToString() + " ";
		}

		public void RefrectX()
		{
			X *= -1;
		}
		public void RefrectY()
		{
			Y *= -1;
		}
	}
}
