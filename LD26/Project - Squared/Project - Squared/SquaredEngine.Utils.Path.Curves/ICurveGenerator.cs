using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SquaredEngine.Utils.Path.Curves {
	public interface ICurveGenerator {
		CurveNodesLayoutType Layout { get; set; }

		List<Vector3> Generate(List<Vector3> controlNodes);
	}
}
