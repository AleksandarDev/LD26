using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquaredEngine.Common;


namespace SquaredEngine.Graphics {

	public static class ConcavePolygonGeometry {
		// C# port of C++ code from
		// Page: http://www.flipcode.com/archives/Efficient_Polygon_Triangulation.shtml
		// Source: http://www.vterrain.org/Implementation/Libs/triangulate.html


		public static bool Process(VertexPositionColor[] contour, ref List<VertexPositionColor> result) {
			throw new NotImplementedException();
			// allocate and initialize list of Vertices in polygon //

			int n = contour.Length;
			if (n < 3) return false;

			int[] V = new int[n];

			// we want a counter-clockwise polygon in V //

			if (0.0f < Area(contour))
				for (int v = 0; v < n; v++) V[v] = v;
			else
				for (int v = 0; v < n; v++) V[v] = (n - 1) - v;

			int nv = n;

			//  remove nv-2 Vertices, creating 1 triangle every time //
			int count = 2 * nv;   // error detection //

			for (int m = 0, v = nv - 1; nv > 2; ) {
				// if we loop, it is probably a non-simple polygon //
				if (0 >= (count--)) {
					//// Triangulate: ERROR - probable bad polygon!
					return false;
				}

				// three consecutive vertices in current polygon, <u,v,w> //
				int u = v; if (nv <= u) u = 0;     // previous //
				v = u + 1; if (nv <= v) v = 0;     // new v    //
				int w = v + 1; if (nv <= w) w = 0;     // next     //

				if (Snip(contour, u, v, w, nv, V)) {

					int a, b, c, s, t;


					// true names of the vertices //
					a = V[u]; b = V[v]; c = V[w];

					// output Triangle //
					result.Add(contour[a]);
					result.Add(contour[b]);
					result.Add(contour[c]);

					m++;

					// remove v from remaining polygon //
					for (s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t]; nv--;

					// resest error detection counter //
					count = 2 * nv;
				}
			}

			return true;
		}
		public static float Area(VertexPositionColor[] contour) {
			int n = contour.Length;
			float area = 0.0f;

			for (int p = n - 1, q = 0; q < n; p = q++) {
				area += contour[p].Position.X * contour[q].Position.Y - contour[q].Position.X * contour[p].Position.Y;
			}

			return area * 0.5f;
		}
		public static bool InsideTriangle(float Ax, float Ay, float Bx, float By, float Cx, float Cy, float Px, float Py) {
			
			float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
			float cCROSSap, bCROSScp, aCROSSbp;
			

			ax = Cx - Bx;
			ay = Cy - By;
			bx = Ax - Cx;
			by = Ay - Cy;
			cx = Bx - Ax;
			cy = By - Ay;

			apx = Px - Ax;
			apy = Py - Ay;
			bpx = Px - Bx;
			bpy = Py - By;
			cpx = Px - Cx;
			cpy = Py - Cy;

			aCROSSbp = ax * bpy - ay * bpx;
			cCROSSap = cx * apy - cy * apx;
			bCROSScp = bx * cpy - by * cpx;

			return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
		}

		private static bool Snip(VertexPositionColor[] contour, int u, int v, int w, int n, int[] V) {
			int p;

			float Ax, Ay, Bx, By, Cx, Cy, Px, Py;


			Ax = contour[V[u]].Position.X;
			Ay = contour[V[u]].Position.Y;

			Bx = contour[V[v]].Position.X;
			By = contour[V[v]].Position.Y;

			Cx = contour[V[w]].Position.X;
			Cy = contour[V[w]].Position.Y;

			if (Constants.EpsilonFGreater > (((Bx - Ax) * (Cy - Ay)) - ((By - Ay) * (Cx - Ax)))) return false;

			for (p = 0; p < n; p++) {
				if ((p == u) || (p == v) || (p == w)) continue;
				Px = contour[V[p]].Position.X;
				Py = contour[V[p]].Position.Y;
				if (InsideTriangle(Ax, Ay, Bx, By, Cx, Cy, Px, Py)) return false;
			}

			return true;
		}
	}
}
