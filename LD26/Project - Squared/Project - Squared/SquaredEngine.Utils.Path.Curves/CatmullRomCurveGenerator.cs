using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SquaredEngine.Utils.Path.Curves {
	/// <summary>
	/// Generates Catmul-Rom curve
	/// </summary>
	public class CatmullRomCurveGenerator : ICurveGenerator {
		// When using CurveNodesLayoutType.Linear
		public Int32 NumberNodes { get; set; }


		protected virtual List<Vector3> GenerateLinear(List<Vector3> controlNodes) {
			if (controlNodes.Count <= 3) {
				return new List<Vector3>(controlNodes);
			}

			//LinkedList<Vector3> curveNodes = new LinkedList<Vector3>(NumberNodes);

			//int curveNodesPerControlNode =
			//    Math.Ceiling((double)NumberNodes / (controlNodes.Count - 3));
			//float positionStep = 1d / curveNodesPerControlNode;
			//for (int nodeIndex = 0; nodeIndex < NumberNodes; nodeIndex++) {
			//    for (float position = 0f; position < curveNodesPerControlNode; position += positionStep) {
			//        throw new NotImplementedException();
			//    }
			//}
			throw new NotImplementedException();
		}

		protected virtual Vector3 GetNextNode(
			Vector3 node1, Vector3 node2, Vector3 node3, Vector3 node4,
			Single position) {
				return Vector3.CatmullRom(
					node1, node2, node3, node4,
					position);
		}


		CurveNodesLayoutType ICurveGenerator.Layout { get; set; }

		List<Vector3> ICurveGenerator.Generate(List<Vector3> controlNodes) {
			if ((this as ICurveGenerator).Layout == CurveNodesLayoutType.Linear)
				return GenerateLinear(controlNodes);
			else {
				// TODO Throw or write some exception or message of error "Type not unknown"
				return controlNodes;
			}
		}
	}
}
