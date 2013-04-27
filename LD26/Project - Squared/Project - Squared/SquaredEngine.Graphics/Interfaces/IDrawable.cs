using System;
using System.Collections.Generic;

namespace SquaredEngine.Graphics {
	public interface IDrawable {
		IEnumerable<IPrimitive> Primitives { get; }
		Int32 PrimitivesCount { get; }
	}
}
