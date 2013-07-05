using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs2ps
{
	public class Path
	{
		public Path()
		{
			codes = new List<string>();
		}

		public string this[int i] 
		{
			get
			{
				if (i >= codes.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return codes[i]; 
			}
		}

		public int Count { get { return codes.Count; } }

		public void AddCode(string code)
		{
			codes.Add(code);
		}

		public void Clear()
		{
			codes.Clear();
		}

		public void ClosePath()
		{
			codes.Add("closepath");
		}

		private List<string> codes;
	}
}
