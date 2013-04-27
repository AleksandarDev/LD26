using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SquaredEngine.Graphics {

	public partial class GraphicsDrawer {
		public struct ConcavePolygonOutline : IComplex, IDrawable {
			private readonly IPrimitive[] primitives;
			private readonly int primitivesCount;

			IPrimitive[] IComplex.Primitives {
				get { return this.primitives; }
			}
			int IComplex.PrimitivesCount {
				get { return this.primitivesCount; }
			}


			public ConcavePolygonOutline(Vector3 position, Vector3[] points, Color[] pointColors) {
				throw new NotImplementedException();

				VertexPositionColor[] vertices = new VertexPositionColor[points.Length];
				for (int index = 0; index < points.Length; index++) {
					vertices[index] = new VertexPositionColor(points[index], pointColors[index]);
				}

				List<VertexPositionColor> concavePoints = new List<VertexPositionColor>();
				bool isConcave = ConcavePolygonGeometry.Process(vertices, ref concavePoints);

				if (!isConcave) {
					IComplex convex = new ConvexPolygonOutline(position, points, pointColors);
					this.primitives = convex.Primitives;
					this.primitivesCount = convex.PrimitivesCount;
					//? throw new InvalidOperationException("Given polygon isn't concave!");
				}
				else {
					this.primitives = new IPrimitive[concavePoints.Count];
					this.primitivesCount = this.primitives.Length;

					for (int index = 0; index < this.primitivesCount - 1; index++) {
						this.primitives[index] = new Line(concavePoints[index].Position + position, concavePoints[index + 1].Position + position, concavePoints[index].Color, concavePoints[index + 1].Color);
					}
					this.primitives[this.primitivesCount - 1] = new Line(concavePoints[0].Position + position, concavePoints[this.primitivesCount - 1].Position + position, concavePoints[0].Color, concavePoints[this.primitivesCount - 1].Color);
				}
			}


			IEnumerable<IPrimitive> IDrawable.Primitives {
				get { return (this as IComplex).Primitives; }
			}
			int IDrawable.PrimitivesCount {
				get { return (this as IComplex).PrimitivesCount; }
			}
		}
	}
}
