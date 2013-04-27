using System;
using System.Collections.Generic;

namespace SquaredEngine.Graphics {
	public interface IComplex {
		IPrimitive[] Primitives { get; }
		Int32 PrimitivesCount { get; }
	}
}
