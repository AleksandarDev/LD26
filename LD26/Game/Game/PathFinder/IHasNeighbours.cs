using System.Collections.Generic;

namespace Game.PathFinder {
	public partial class PathFinder<Node> where Node : PathFinder<Node>.IHasNeighbours<Node> {
		public interface IHasNeighbours<N> {
			IEnumerable<N> Neighbours { get; }
		}
	}
}