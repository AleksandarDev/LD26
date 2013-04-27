using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SquaredEngine.Utils.Path.Curves {
	public interface ICurveNodesContainer {
		List<Vector3> ControlNodes { get; set; }
		List<Vector3> CurveNodes { get; }

		List<Vector3> GetCurveNodesFrom(ICurveGenerator generator);

		void AddLast(Vector3 node);
		void AddFirst(Vector3 node);
		Boolean AddAfter(Int32 index, Vector3 node);
		Boolean AddAfter(Vector3 searchNode, Vector3 node);

		Boolean AddRangeAfter(Int32 index, Vector3[] nodes);

		Boolean Remove(Vector3 node);
		Boolean Remove(Int32 index);

		void Clear();
	}
}
