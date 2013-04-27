using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SquaredEngine.Graphics {
	public struct ConvexPolygonOutline : IComplex, IDrawable {
		private readonly IPrimitive[] primitives;
		private readonly int primitivesCount;

		IPrimitive[] IComplex.Primitives {
			get { return this.primitives; }
		}

		int IComplex.PrimitivesCount {
			get { return this.primitivesCount; }
		}


		public ConvexPolygonOutline(Vector3 position, Color color, params Vector3[] points)
			: this(position, points, color) {
		}

		public ConvexPolygonOutline(Vector3 position, Vector3[] points, Color color)
			: this(position, points, Enumerable.Repeat(color, points.Length).ToArray()) {
		}

		public ConvexPolygonOutline(Vector3 position, Vector3[] points, Color[] pointColors) {
			VertexPositionColor[] vertices = new VertexPositionColor[points.Length];
			for (int index = 0; index < points.Length; index++) {
				vertices[index] = new VertexPositionColor(points[index], pointColors[index]);
			}

			VertexPositionColor[] convexHullVertices = ConvexPolygonHelper.GetConvexHullVertices(vertices);

			this.primitives = new IPrimitive[convexHullVertices.Length];
			this.primitivesCount = this.primitives.Length;

			for (int index = 0; index < this.primitivesCount - 1; index++) {
				this.primitives[index] = new GraphicsDrawer.Line(convexHullVertices[index].Position + position,
				                                  convexHullVertices[index + 1].Position + position, convexHullVertices[index].Color,
				                                  convexHullVertices[index + 1].Color);
			}
			this.primitives[this.primitivesCount - 1] = new GraphicsDrawer.Line(convexHullVertices[0].Position + position,
			                                                     convexHullVertices[this.primitivesCount - 1].Position + position,
			                                                     convexHullVertices[0].Color,
			                                                     convexHullVertices[this.primitivesCount - 1].Color);
		}


		IEnumerable<IPrimitive> IDrawable.Primitives {
			get { return (this as IComplex).Primitives; }
		}

		int IDrawable.PrimitivesCount {
			get { return (this as IComplex).PrimitivesCount; }
		}
	}
}
