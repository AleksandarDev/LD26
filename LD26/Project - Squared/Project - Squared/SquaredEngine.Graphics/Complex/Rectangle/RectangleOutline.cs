using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SquaredEngine.Common;

namespace SquaredEngine.Graphics {
	public struct RectangleOutline : IComplex, IDrawable {
		private readonly IPrimitive[] primitives;

		IPrimitive[] IComplex.Primitives {
			get { return this.primitives; }
		}

		int IComplex.PrimitivesCount {
			get { return 4; }
		}


		public RectangleOutline(Vector3 upperLeft, float size, Color color)
			: this(upperLeft, size, color, color, color, color) {
		}

		public RectangleOutline(Vector3 upperLeft, float size, Color colorA, Color colorB, Color colorC, Color colorD)
			: this(upperLeft, new Vector3(upperLeft.X + size, upperLeft.Y + size, upperLeft.Z), colorA, colorB, colorC, colorD) {
		}

		public RectangleOutline(Vector3 upperLeft, Vector3 lowerRight, Color color)
			: this(upperLeft, lowerRight, color, color, color, color) {
		}

		public RectangleOutline(Vector3 upperLeft, Vector3 lowerRight, Color colorA, Color colorB, Color colorC, Color colorD) {
			IComplex rectangleQuadOutline = new QuadOutline(upperLeft,
			                                                new Vector3(lowerRight.X, upperLeft.Y,
			                                                            Math.Abs(lowerRight.Z - 0) < Constants.EpsilonFLess && Math.Abs(upperLeft.Z - 0) < Constants.EpsilonFLess
			                                                            	? 0
			                                                            	: lowerRight.Z + (lowerRight.Z - upperLeft.Z)/2),
			                                                lowerRight,
			                                                new Vector3(upperLeft.X, lowerRight.Y,
			                                                            Math.Abs(lowerRight.Z - 0) < Constants.EpsilonFLess && Math.Abs(upperLeft.Z - 0) < Constants.EpsilonFLess
			                                                            	? 0
			                                                            	: lowerRight.Z + (lowerRight.Z - upperLeft.Z)/2),
			                                                colorA, colorB, colorC, colorD);
			this.primitives = rectangleQuadOutline.Primitives;
		}


		IEnumerable<IPrimitive> IDrawable.Primitives {
			get { return (this as IComplex).Primitives; }
		}

		int IDrawable.PrimitivesCount {
			get { return (this as IComplex).PrimitivesCount; }
		}
	}
}
