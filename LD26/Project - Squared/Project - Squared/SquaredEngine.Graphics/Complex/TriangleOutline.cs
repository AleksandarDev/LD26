using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace SquaredEngine.Graphics {
	public struct TriangleOutline : IComplex, IDrawable {
		private const Int32 PrimitivesCount = 3;
		private readonly IPrimitive[] primitives;

		IPrimitive[] IComplex.Primitives {
			get { return this.primitives; }
		}

		int IComplex.PrimitivesCount {
			get { return PrimitivesCount; }
		}


		public TriangleOutline(Vector3 pointA, Vector3 pointB, Vector3 pointC, Color color)
			: this(pointA, pointB, pointC, color, color, color) {
		}

		public TriangleOutline(Vector3 pointA, Vector3 pointB, Vector3 pointC, Color colorA, Color colorB, Color colorC)
			: this() {
			this.primitives = new IPrimitive[PrimitivesCount] {
			                                                  	new GraphicsDrawer.Line(pointA, pointB, colorA, colorB),
			                                                  	new GraphicsDrawer.Line(pointB, pointC, colorB, colorC),
			                                                  	new GraphicsDrawer.Line(pointC, pointA, colorC, colorA)
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
