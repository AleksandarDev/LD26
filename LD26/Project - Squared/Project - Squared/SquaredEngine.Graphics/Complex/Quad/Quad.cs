using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace SquaredEngine.Graphics {
	public struct Quad : IComplex, IDrawable {
		private readonly IPrimitive[] primitives;

		IPrimitive[] IComplex.Primitives {
			get { return this.primitives; }
		}

		int IComplex.PrimitivesCount {
			get { return 2; }
		}


		public Quad(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, Color color, float angle = 0f, Vector3? origin = null)
			: this(pointA, pointB, pointC, pointD, color, color, color, color, angle, origin) {
		}

		public Quad(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, Color colorA, Color colorB, Color colorC,
		            Color colorD, float angle = 0f, Vector3? origin = null)
			: this() {
			Vector3[] points = {pointA, pointB, pointC, pointD};
			if (origin.HasValue && angle != 0f)
				GraphicsDrawer.RotateAboutOrigin(origin.Value, angle, ref points);
			this.primitives = new IPrimitive[2] {
			                                    	new GraphicsDrawer.Triangle(points[0], points[1], points[3], colorA, colorB, colorD),
			                                    	new GraphicsDrawer.Triangle(points[1], points[2], points[3], colorB, colorC, colorD)
			                                    };
		}


		IEnumerable<IPrimitive> IDrawable.Primitives {
			get { return (this as IComplex).Primitives; }
		}

		int IDrawable.PrimitivesCount {
			get { return (this as IComplex).PrimitivesCount; }
		}
	}
}
