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


		public Quad(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, Color color)
			: this(pointA, pointB, pointC, pointD, color, color, color, color) {
		}

		public Quad(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, Color colorA, Color colorB, Color colorC,
		            Color colorD)
			: this() {
			this.primitives = new IPrimitive[2] {
			                                    	new GraphicsDrawer.Triangle(pointA, pointB, pointD, colorA, colorB, colorD),
			                                    	new GraphicsDrawer.Triangle(pointB, pointC, pointD, colorB, colorC, colorD)
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
