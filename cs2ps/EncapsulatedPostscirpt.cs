using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace cs2ps
{
	public enum Direction:int
	{
		CW,
		CCW
	}

    public class EncapsulatedPostscirpt
    {
		public EncapsulatedPostscirpt(string filename, int bb_width,int bb_height)
		{
			eps_str = new List<string>();
			Filename = filename;

			FrontRGBColor = new double[] { 0, 0, 0 };
			BackRGBColor = new double[] { 0.5,0.5,0.5};

			eps_str.Add("%!PS-Adobe-3.0 EPSF-3.0");
			eps_str.Add("%%BoundingBox: 0 0 " + bb_width.ToString() + " " + bb_height.ToString());

			eps_str.Add("gsave");
		}

		/// <summary>
		/// 原点を移動する．
		/// </summary>
		/// <param name="o">移動先の座標</param>
		public void MoveOrigin(Point o)
		{
			eps_str.Add(o.ToString() +TRANSLATE);
		}

		public void RotateCW(double angle)
		{
			eps_str.Add(angle.ToString() + " " + ROTATE);
		}
		public void RotateCCW(double angle)
		{
			angle *= -1;
			eps_str.Add(angle.ToString() + " " + ROTATE);
		}
		

		/// <summary>
		/// 線の太さを設定する
		/// </summary>
		/// <param name="w"></param>
		public void SetLineWidth(double w)
		{
			LineWidth = w;
			eps_str.Add(w.ToString() + " " + SETLINEWIDTH);
		}

		public void SetFrontColor(double r, double g, double b)
		{
			FrontRGBColor = new double[] { r, g, b };
			eps_str.Add(r.ToString() + " " + g.ToString() + " " + b.ToString() + " " + SETRGBCOLOR);
		}
		public void SetFrontColor(double density)
		{
			FrontRGBColor = new double[] { density, density, density };
			eps_str.Add(density.ToString() + " " + SETGRAY);
		}

		public void SetBackColor(double r, double g, double b)
		{
			BackRGBColor = new double[] { r, g, b };
		}
		public void SetBackColor(double density)
		{
			BackRGBColor = new double[] { density, density, density };
		}


		public void SetRawPostScript(string code)
		{
			eps_str.Add(code);
		}

		public void SetRawPostScript(string[] codes)
		{
			foreach (string line in codes)
			{
				eps_str.Add(line);
			}
		}

		



		public void Return2SolidLine()
		{
			eps_str.Add("[] 0 setdash");
		}

		public void DrawLine(Point p1, Point p2)
		{
			eps_str.Add(p1.ToString() + MOVETO);
			eps_str.Add(p2.ToString() + LINETO);
			eps_str.Add(CURRENTPOINT);
			eps_str.Add(STROKE);
			eps_str.Add(MOVETO);		// カレントポイントをp2に
		}

		public void DrawLines(params Point[] path)
		{
			int n = path.Length;
			if (n < 2)
			{
				throw new ArgumentException();
			}
			else
			{
				setPath(path);
				eps_str.Add(CURRENTPOINT);
				eps_str.Add(STROKE);
				eps_str.Add(MOVETO);
			}
			
		}

		public void DrawPolygon(params Point[] path)
		{
			int n = path.Length;
			if (n < 3)
			{
				throw new ArgumentException();
			}
			else
			{
				setPath(path);
				eps_str.Add(CLOSEPATH);
				eps_str.Add(CURRENTPOINT);
				eps_str.Add(STROKE);
				eps_str.Add(MOVETO);
			}
		}

		public void FillPolygon(params Point[] path)
		{
			int n = path.Length;
			if (n < 3)
			{
				throw new ArgumentException();
			}
			else
			{
				setPath(path);
				eps_str.Add(CLOSEPATH);
				eps_str.Add(GSAVE);
				eps_str.Add(BackRGBColor[0].ToString() + " " + BackRGBColor[1].ToString() + " " + BackRGBColor[2].ToString() + " " + SETRGBCOLOR);
		//		eps_str.Add(density.ToString() + " setgray");
				eps_str.Add(FILL);
				eps_str.Add(GRESTORE);
				eps_str.Add(NEWPATH);
			}
		}

		public void DrawFillPolygon(params Point[] path)
		{
			int n = path.Length;
			if (n < 3)
			{
				throw new ArgumentException();
			}
			else
			{
				setPath(path);
				eps_str.Add(CLOSEPATH);
				eps_str.Add(GSAVE);
				eps_str.Add(BackRGBColor[0].ToString() + " " + BackRGBColor[1].ToString() + " " + BackRGBColor[2].ToString() + " " + SETRGBCOLOR);
				eps_str.Add(FILL);
				eps_str.Add(GRESTORE);
				eps_str.Add(STROKE);
			}
		}

		private void setPath(Point[] path)
		{
			int n = path.Length;
			eps_str.Add(path[0].ToString() + MOVETO);
			for (int i = 1; i < n; i++)
			{
				eps_str.Add(path[i].ToString() + LINETO);
			}
		}

		public void DrawCircle(Point center, double r)
		{
			eps_str.Add(NEWPATH);
			eps_str.Add(center.X.ToString() + " " + center.Y.ToString() + " " + r.ToString() + " 0 360 arc stroke");
		}

		public void FillCircle(Point center, double r)
		{
			eps_str.Add(NEWPATH);
			eps_str.Add(center.X.ToString() + " " + center.Y.ToString() + " " + r.ToString() + " 0 360 arc");
			eps_str.Add(GSAVE);
			eps_str.Add(BackRGBColor[0].ToString() + " " + BackRGBColor[1].ToString() + " " + BackRGBColor[2].ToString() + " " + SETRGBCOLOR);
			eps_str.Add(FILL);
			eps_str.Add(GRESTORE);
		}

		public void DrawArc(Point center, double r, double start_angle, double end_angle)
		{
			eps_str.Add(NEWPATH);
			eps_str.Add(center.X.ToString() + " " + center.Y.ToString() + " " + r.ToString() + " " + start_angle.ToString() + " " + end_angle.ToString() + " " + "arc stroke");
		}

		public void FillArc(Point center, double r, double start_angle, double end_angle)
		{
			eps_str.Add(NEWPATH);
			eps_str.Add(center.X.ToString() + " " + center.Y.ToString() + " " + r.ToString() + " " + start_angle.ToString() + " " + end_angle.ToString() + " arc");
			eps_str.Add(center.X.ToString() + " " + center.Y.ToString() + " " + LINETO);
			eps_str.Add(CLOSEPATH);
			eps_str.Add(GSAVE);
			eps_str.Add(BackRGBColor[0].ToString() + " " + BackRGBColor[1].ToString() + " " + BackRGBColor[2].ToString() + " " + SETRGBCOLOR);
			eps_str.Add(FILL);
			eps_str.Add(GRESTORE);
		}

		

		public void DrawPath(Path path)
		{
			eps_str.Add(NEWPATH);
			for (int i = 0; i < path.Count; i++)
			{
				eps_str.Add(path[i]);
			}
			eps_str.Add(STROKE);
		}

		public void FillPath(Path path)
		{
			eps_str.Add(NEWPATH);
			for (int i = 0; i < path.Count; i++)
			{
				eps_str.Add(path[i]);
			}
			eps_str.Add(GSAVE);
			eps_str.Add(BackRGBColor[0].ToString() + " " + BackRGBColor[1].ToString() + " " + BackRGBColor[2].ToString() + " " + SETRGBCOLOR);
			eps_str.Add(FILL);
			eps_str.Add(GRESTORE);
		}

		public void DrawFillPath(Path path)
		{
			eps_str.Add(NEWPATH);
			for (int i = 0; i < path.Count; i++)
			{
				eps_str.Add(path[i]);
			}
			eps_str.Add(GSAVE);
			eps_str.Add(BackRGBColor[0].ToString() + " " + BackRGBColor[1].ToString() + " " + BackRGBColor[2].ToString() + " " + SETRGBCOLOR);
			eps_str.Add(FILL);
			eps_str.Add(GRESTORE);
			eps_str.Add(STROKE);
		}

		public void FillPath()
		{

		}

		public void DrawFillPath()
		{

		}

		public static string GetMovetoPath(Point p)
		{
			return p.ToString() + MOVETO;
		}

		public static string GetLineToPath(Point p)
		{
			return p.ToString() + LINETO;
		}

		public static string GetArcPath(Point center, double r, double start_angle, double end_angle,Direction direction)
		{
			if (direction == Direction.CCW)
			{
				return (center.ToString() + r.ToString() + " " + start_angle.ToString() + " " + end_angle.ToString() + " " + ARC);
			}
			else
			{
				return (center.ToString() + r.ToString() + " " + start_angle.ToString() + " " + end_angle.ToString() + " " + ARCN);
			}
			
		}

		public void Print()
		{

			eps_str.Add("showpage");
			eps_str.Add(GRESTORE);


			FolderBrowserDialog fbd = new FolderBrowserDialog()
			{
				Description = "ファイルを保存するフォルダを選択して下さい．",
				RootFolder = System.Environment.SpecialFolder.Desktop,
				SelectedPath = @Directory.GetCurrentDirectory(),
				ShowNewFolderButton = true
			};
			fbd.ShowDialog();

			using (StreamWriter sw = new StreamWriter(@fbd.SelectedPath + "\\" + Filename + ".eps"))
			{
				foreach(string str in eps_str){
					sw.WriteLine(str);
				}
			}
		}


		private List<string> eps_str;
		private string Filename { get; set; }
		private double LineWidth { get; set; }

		private double[] FrontRGBColor { get; set; }	//stroke
		private double[] BackRGBColor { get; set; }		//fill

		private static readonly string MOVETO = "moveto";
		private static readonly string LINETO = "lineto";
		private static readonly string CLOSEPATH = "closepath";
		private static readonly string GSAVE = "gsave";
		private static readonly string GRESTORE = "grestore";
		private static readonly string FILL = "fill";
		private static readonly string STROKE = "stroke";
		private static readonly string NEWPATH = "newpath";
		private static readonly string CURRENTPOINT = "currentpoint";
		private static readonly string ROTATE = "rotate";
		private static readonly string TRANSLATE = "translate";
		private static readonly string SETLINEWIDTH = "setlinewidth";
		private static readonly string SETRGBCOLOR = "setrgbcolor";
		private static readonly string SETGRAY = "setgray";
		private static readonly string ARC = "arc";
		private static readonly string ARCN = "arcn";
    }
}
