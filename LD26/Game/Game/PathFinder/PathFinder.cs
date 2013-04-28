using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SquaredEngine.Utils.Trees.QuadTree;

namespace Game.PathFinder {
	// Source: http://blogs.msdn.com/b/ericlippert/archive/2007/10/10/path-finding-using-a-in-c-3-0-part-four.aspx
	public partial class PathFinder<Node> where Node : PathFinder<Node>.IHasNeighbours<Node> {
		public volatile HashSet<Node> closed;
		public PriorityQueue<double, Path<Node>> queue; 

		public Path<Node> FindPath(Node start, Node destination, Func<Node, Node, double> distance, Func<Node, double> estimate) {			
			this.closed = new HashSet<Node>();
			this.queue = new PriorityQueue<double, Path<Node>>();
			
			queue.Enqueue(0, new Path<Node>(start));
			
			while (!queue.IsEmpty) {
				var path = queue.Dequeue();
				if (closed.Contains(path.LastStep))
					continue;
				if (path.LastStep.Equals(destination))
					return path;
				
				closed.Add(path.LastStep);
				
				foreach (Node n in path.LastStep.Neighbours) {
					double d = distance(path.LastStep, n);
					var newPath = path.AddStep(n, d);
					queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
				}
			}
			return null;
		}
	}
}