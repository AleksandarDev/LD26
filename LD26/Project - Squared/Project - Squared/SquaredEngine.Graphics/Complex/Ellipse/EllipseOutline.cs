using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SquaredEngine.Common;

namespace SquaredEngine.Graphics {
	public struct EllipseOutline : IComplex, IDrawable {
		private readonly IPrimitive[] primitives;
		private readonly int primitivesCount;

		IPrimitive[] IComplex.Primitives {
			get { return this.primitives; }
		}

		int IComplex.PrimitivesCount {
			get { return this.primitivesCount; }
		}


		public EllipseOutline(Vector3 position, float width, float height, Color color)
			: this(position, width, height, 0f, color) {
		}

		public EllipseOutline(Vector3 position, float width, float height, float rotation, Color color)
			: this(position, width, height, rotation, (int) ((width + height)/2), color) {
		}

		public EllipseOutline(Vector3 position, float width, float height, float rotation, int sides, Color color) {
			this.primitives = new IPrimitive[sides];
			Vector3[] points = GetEllipsePoints(width, height, rotation, sides);

			for (int index = 0; index < sides - 1; index++) {
				this.primitives[index] = new GraphicsDrawer.Line(position + points[index], position + points[index + 1], color);
			}
			this.primitives[sides - 1] = new GraphicsDrawer.Line(position + points[sides - 1], position + points[0], color);

			this.primitivesCount = sides;
		}


		public static Vector3[] GetEllipsePoints(float width, float height, float rotation, int sides) {
			Vector3[] points = new Vector3[sides];

			float step = Constants.PI2 / sides;
			int currentPoint = 0;

			for (float t = 0f; currentPoint < sides; t += step) {
				points[currentPoint++] = new Vector3((float) (width*Math.Cos(t)), (float) (height*Math.Sin(t)), 0f);
			}

			if (Math.Abs(rotation - 0) > Constants.EpsilonFGreater) {
				Matrix transform = Matrix.CreateRotationZ(rotation);
				for (int i = 0; i < sides; i++) {
					points[i] = Vector3.Transform(points[i], transform);
				}
			}

			return points;
		}

		IEnumerable<IPrimitive> IDrawable.Primitives {
			get { return (this as IComplex).Primitives; }
		}

		int IDrawable.PrimitivesCount {
			get { return (this as IComplex).PrimitivesCount; }
		}
	}
}
