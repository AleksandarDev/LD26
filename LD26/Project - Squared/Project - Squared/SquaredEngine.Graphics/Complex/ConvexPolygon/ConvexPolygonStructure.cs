using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquaredEngine.Common;


namespace SquaredEngine.Graphics {
	public static class ConvexPolygonHelper {
		public static VertexPositionColor[] GetConvexHullVertices(VertexPositionColor[] vertices) {
			// BUG: When you place two vertices to the samo place algorithm fails and shows concave polygon
			// NOTE: http://csourcesearch.net/csharp/fid45550898653ECFA798D5B324FF9E0FEAF4EF3840.aspx?s=mdef%3Acompute

			vertices = SortGrahmScan(vertices);

			VertexPositionColor p;
			Stack<VertexPositionColor> ps = new Stack<VertexPositionColor>(vertices.Length);

			ps.Push(vertices[0]);
			ps.Push(vertices[1]);
			ps.Push(vertices[2]);

			for (int i = 3; i < vertices.Length; i++) {
				p = ps.Pop();
				while (CalculateAngle(ps.Peek().Position, p.Position, vertices[i].Position) > 0)
					p = ps.Pop();
				ps.Push(p);
				ps.Push(vertices[i]);
			}

			ps.Push(vertices[0]);

			return ps.ToArray();
		}

		private static float CalculateAngle(Vector3 p1, Vector3 p2, Vector3 p3) {
			return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
		}

		private static VertexPositionColor[] SortGrahmScan(VertexPositionColor[] vertices) {
			for (int i = 1; i < vertices.Length; i++) {
				if ((vertices[i].Position.Y < vertices[0].Position.Y) || ((Math.Abs(vertices[i].Position.Y - vertices[0].Position.Y) < Constants.EpsilonFLess) && (vertices[i].Position.X < vertices[0].Position.X))) {
					VertexPositionColor t = vertices[0];
					vertices[0] = vertices[i];
					vertices[i] = t;
				}
			}

			Array.Sort(vertices, 1, vertices.Length - 1, new RadialComparator(vertices[0]));
			return vertices;
		}


		private class RadialComparator : IComparer<VertexPositionColor> {
			private readonly VertexPositionColor origin = default(VertexPositionColor);

			public RadialComparator(VertexPositionColor origin) {
				this.origin = origin;
			}

			public int Compare(VertexPositionColor p1, VertexPositionColor p2) {
				return PolarCompare(origin, p1, p2);
			}

			private static int PolarCompare(VertexPositionColor o, VertexPositionColor p, VertexPositionColor q) {
				double dxp = p.Position.X - o.Position.X;
				double dyp = p.Position.Y - o.Position.Y;
				double dxq = q.Position.X - o.Position.X;
				double dyq = q.Position.Y - o.Position.Y;

				int orient = (int)CalculateAngle(o.Position, p.Position, q.Position);

				if (orient > 0)
					return 1;
				if (orient < 0)
					return -1;

				double op = dxp * dxp + dyp * dyp;
				double oq = dxq * dxq + dyq * dyq;
				if (op < oq)
					return -1;
				if (op > oq)
					return 1;
				return 0;
			}

		}
	}
}
