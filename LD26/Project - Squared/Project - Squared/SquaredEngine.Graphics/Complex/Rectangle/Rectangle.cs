﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SquaredEngine.Common;

namespace SquaredEngine.Graphics {
	public struct Rectangle : IComplex, IDrawable {
		private readonly IPrimitive[] primitives;

		IPrimitive[] IComplex.Primitives {
			get { return this.primitives; }
		}

		int IComplex.PrimitivesCount {
			get { return 2; }
		}

		public Rectangle(Vector3 upperLeft, float size, Color color, float angle = 0f, Vector3? origin = null)
			: this(upperLeft, size, color, color, color, color, angle, origin) {
		}

		public Rectangle(Vector3 upperLeft, float size, Color colorA, Color colorB, Color colorC, Color colorD, float angle = 0f, Vector3? origin = null)
			: this(upperLeft, new Vector3(upperLeft.X + size, upperLeft.Y + size, upperLeft.Z), colorA, colorB, colorC, colorD, angle, origin) {
		}

		public Rectangle(Vector3 upperLeft, Vector3 lowerRight, Color color, float angle = 0f, Vector3? origin = null)
			: this(upperLeft, lowerRight, color, color, color, color, angle, origin) {
		}

		public Rectangle(Vector3 upperLeft, Vector3 lowerRight, Color colorA, Color colorB, Color colorC, Color colorD, float angle = 0f, Vector3? origin = null)
		{
			IComplex rectangleQuad = new Quad(upperLeft,
			                                  new Vector3(lowerRight.X, upperLeft.Y,
			                                              Math.Abs(lowerRight.Z - 0) < Constants.EpsilonFLess && Math.Abs(upperLeft.Z - 0) < Constants.EpsilonFLess
			                                              	? 0
			                                              	: lowerRight.Z + (lowerRight.Z - upperLeft.Z)/2),
			                                  lowerRight,
			                                  new Vector3(upperLeft.X, lowerRight.Y,
			                                              Math.Abs(lowerRight.Z - 0) < Constants.EpsilonFLess && Math.Abs(upperLeft.Z - 0) < Constants.EpsilonFLess
			                                              	? 0
			                                              	: lowerRight.Z + (lowerRight.Z - upperLeft.Z)/2),
			                                  colorA, colorB, colorC, colorD, angle, origin);
			this.primitives = rectangleQuad.Primitives;
		}


		IEnumerable<IPrimitive> IDrawable.Primitives {
			get { return (this as IComplex).Primitives; }
		}

		int IDrawable.PrimitivesCount {
			get { return (this as IComplex).PrimitivesCount; }
		}
	}
}
