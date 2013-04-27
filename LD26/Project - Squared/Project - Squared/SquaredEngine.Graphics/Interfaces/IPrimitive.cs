using System;
using Microsoft.Xna.Framework.Graphics;

namespace SquaredEngine.Graphics {
	public interface IPrimitive {
		VertexPositionColor[] Vertices { get; }
		PrimitiveType Type { get; }
		Int32 VerticesCount { get; }
	}
}
