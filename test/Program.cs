using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cs2ps;

namespace test
{
	class Program
	{
		[STAThreadAttribute]
		static void Main(string[] args)
		{
			EncapsulatedPostscirpt fig = new EncapsulatedPostscirpt("a",200, 200);

			fig.SetLineWidth(1);
			fig.DrawLines(new Point(10, 30), new Point(50, 90), new Point(70, 50));
			fig.DrawPolygon(new Point(100, 80), new Point(125, 130), new Point(150, 130), new Point(150, 80));
			fig.SetBackColor(0.1);
			fig.FillPolygon(new Point(10, 10), new Point(60, 50), new Point(80, 20), new Point(120, 40), new Point(150, 10));
			fig.SetBackColor(0.4);
			fig.DrawFillPolygon(new Point(10, 190), new Point(120, 180), new Point(80, 90));
			fig.DrawCircle(new Point(130, 80), 60);
			fig.FillCircle(new Point(25, 100), 20);
			fig.DrawArc(new Point(15, 135), 10, 0, 240);
			fig.SetBackColor(0.8,0,0.8);
			fig.FillArc(new Point(15, 160), 10, -30, 240);
		//	fig.Print();

			double screen_w = 400;
			double screen_h = 400;
			double r1 = 60;
			double r2 = 80;
			double r_brush_innner = 48;
			int slot = 9;
			Point center = new Point(screen_w / 2, screen_h / 2);
			EncapsulatedPostscirpt fig2 = new EncapsulatedPostscirpt("PMDCmotor_winding_structure", (int)screen_w, (int)screen_h);
			fig2.MoveOrigin(center);

			fig2.SetLineWidth(3);
			fig2.DrawLines(new Point(-r_brush_innner, 0), new Point(-20, 0), new Point(-20, -150));
			fig2.DrawLines(new Point(r_brush_innner, 0), new Point(20, 0), new Point(20, -150));
			
			fig2.SetLineWidth(1);
			fig2.DrawCircle(new Point(), r1);
			fig2.DrawCircle(new Point(), r2);

			/*
			for (int i = 0; i < slot; i++)
			{
				Point p1 = new Point(r1 * Math.Cos(2 * Math.PI * i / slot), r1 * Math.Sin(2 * Math.PI * i / slot));
				Point p2 = new Point(r2 * Math.Cos(2 * Math.PI * i / slot), r2 * Math.Sin(2 * Math.PI * i / slot));
				fig2.DrawLine(p1, p2);
			}
			 * */

			double rate_theta_1 = 0.05;
			double rate_r_1 = 0.08;
			double r_commu_inner = r1 - (r2 - r1) * rate_r_1;
			double r_commu_outer = r2 + (r2 - r1) * rate_r_1;
			double theta_1 = 360 / slot * rate_theta_1;
			double theta_2 = 360 / slot * (1-rate_theta_1);
			
			Path path = new Path();
			path.AddCode(EncapsulatedPostscirpt.GetArcPath(new Point(), r_commu_inner, theta_1, theta_2, Direction.CCW));
			path.AddCode(EncapsulatedPostscirpt.GetArcPath(new Point(), r_commu_outer,theta_2,theta_1,Direction.CW));
			path.ClosePath();
			fig2.SetBackColor(0.8, 0.8, 0.8);
			for (int i = 0; i < slot; i++)
			{
				fig2.DrawFillPath(path);
				fig2.RotateCCW(360 / slot);
			}


			// ブラシの描画
			fig2.SetBackColor(0, 1, 1);
			Path annulus=new Path();
			annulus.AddCode(EncapsulatedPostscirpt.GetArcPath(new Point(), r_commu_inner, -12, 12, Direction.CCW));
			annulus.AddCode(EncapsulatedPostscirpt.GetArcPath(new Point(), r_brush_innner, 12, -12, Direction.CW));
			annulus.ClosePath();
			for (int i = 0; i < 2; i++)
			{
				fig2.DrawFillPath(annulus);
				fig2.RotateCW(180);
			}

			double core_width = 20;
			double r_core_outer = 180;
			double slot_width_rate = 0.15;
			double core_thick_rate = 0.3;
			double r_core_inner = r_commu_outer+(r_core_outer-r_commu_outer)*(1-core_thick_rate);


			path.Clear();
			Point p1 = new Point(Math.Sqrt(Pow(r_commu_outer, 2) - Pow(core_width / 2, 2)), core_width / 2);
			Point p2 = new Point(Math.Sqrt(Pow(r_core_inner, 2) - Pow(core_width / 2, 2)), core_width / 2);


			path.AddCode(EncapsulatedPostscirpt.GetMovetoPath(p1));
			path.AddCode(EncapsulatedPostscirpt.GetLineToPath(p2));
			path.AddCode(EncapsulatedPostscirpt.GetArcPath(new Point(), r_core_inner, Math.Atan(p2.Y / p2.X) * 360 / 2 / Math.PI, 360 / slot * (1 - slot_width_rate)/2, Direction.CCW));
			path.AddCode(EncapsulatedPostscirpt.GetArcPath(new Point(), r_core_outer, 360 / slot * (1 - slot_width_rate) / 2, -360 / slot * (1 - slot_width_rate) / 2, Direction.CW));
			path.AddCode(EncapsulatedPostscirpt.GetArcPath(new Point(), r_core_inner, -360 / slot * (1 - slot_width_rate) / 2, -Math.Atan(p2.Y / p2.X) * 360 / 2 / Math.PI, Direction.CCW));
			path.AddCode(EncapsulatedPostscirpt.GetLineToPath(new Point(p1.X,-p1.Y)));
		//	path.ClosePath();

			for (int i = 0; i < slot; i++)
			{
				fig2.DrawPath(path);
				fig2.RotateCCW(360 / slot);
			}

			
			fig2.Print();
		}

		private static double Pow(double x, int n)
		{
			double ret = 1;
			for (int i = 0; i < n; i++)
			{
				ret *= x;
			}
			return ret;
		}
	}
}
