using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace SquaredEngine.Graphics {
	public struct Net : IComplex, IDrawable {
		private readonly IPrimitive[] primitives;
		private readonly int primitivesCount;

		IPrimitive[] IComplex.Primitives {
			get { return this.primitives; }
		}

		int IComplex.PrimitivesCount {
			get { return this.primitivesCount; }
		}


		public Net(Vector3 position, int numBlocksX, int numBlocksY, float blockWidth, float blockHeight, Color color) {
			this.primitivesCount = numBlocksX + numBlocksY + 2;
			this.primitives = new IPrimitive[this.primitivesCount];

			GenerateNet(numBlocksX, numBlocksY, blockWidth, blockHeight, position, color);
		}

		private void GenerateNet(int numBlocksX, int numBlocksY, float blockWidth, float blockHeight, Vector3 position,
		                         Color color) {
			float netWidth = numBlocksX*blockWidth;
			float netHeight = numBlocksY*blockHeight;

			int currentPrimitive = 0;
			for (int currentY = 0; currentPrimitive < numBlocksY + 1; ++currentPrimitive, ++currentY) {
				float linePositionY = position.X + (currentY*blockHeight);
				this.primitives[currentPrimitive] = new GraphicsDrawer.Line(
					new Vector3(position.X, linePositionY, position.Z),
					new Vector3(position.X + netWidth, linePositionY, position.Z),
					color);
			}
			for (int currentX = 0; currentPrimitive < this.primitivesCount; ++currentPrimitive, ++currentX) {
				float linePositionX = position.Y + (currentX*blockWidth);
				this.primitives[currentPrimitive] = new GraphicsDrawer.Line(
					new Vector3(linePositionX, position.Y, position.Z),
					new Vector3(linePositionX, position.Y + netHeight, position.Z),
					color);
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
