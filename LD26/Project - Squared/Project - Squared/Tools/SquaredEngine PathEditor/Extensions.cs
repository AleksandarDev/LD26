using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredEngine.PathEditor {
	public static class Extensions {
		public static System.Drawing.Point ToPoint(this System.Windows.Point @this) {
			return new System.Drawing.Point((int)@this.X, (int)@this.Y);
		}
	}
}
