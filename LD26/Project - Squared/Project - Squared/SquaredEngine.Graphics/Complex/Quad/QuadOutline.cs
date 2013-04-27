using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace SquaredEngine.Graphics {
	public struct QuadOutline : IComplex, IDrawable {
		private readonly IPrimitive[] primitives;

		IPrimitive[] IComplex.Primitives {
			get { return this.primitives; }
		}

		int IComplex.PrimitivesCount {
			get { return 4; }
		}


		public QuadOutline(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, Color color)
			: this(pointA, pointB, pointC, pointD, color, color, color, color) {
		}

		public QuadOutline(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, Color colorA, Color colorB,
		                   Color colorC, Color colorD)
			: this() {
			this.primitives = new IPrimitive[4] {
			                                    	new GraphicsDrawer.Line(pointA, pointB, colorA, colorB),
			                                    	new GraphicsDrawer.Line(pointB, pointC, colorB, colorC),
			                                    	new GraphicsDrawer.Line(pointC, pointD, colorC, colorD),
			                                    	new GraphicsDrawer.Line(pointD, pointA, colorD, colorA)
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
