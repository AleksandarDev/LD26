using System.Collections;
using System.Collections.Generic;


namespace SquaredEngine.Utils.Trees.QuadTree {

	public class NodesEnumerator<T, K> : IEnumerator<T>
		where T : class, INode<K> where K : IComponent {
		private readonly NodeCollection<T, K> collection;
		private int currentIndex;
		private T currentNode;


		public T Current {
			get { return currentNode; }
		}

		object IEnumerator.Current {
			get { return Current; }
		}

		public NodesEnumerator(NodeCollection<T, K> collection) {
			this.collection = collection;

			Initialize();
		}


		private void Initialize() {
			Reset();
		}

		public void Dispose() { }

		public bool MoveNext() {
			if (++currentIndex >= collection.Count || collection.IsEmpty) {
				return false;
			}

			currentNode = collection.GetNode((Directions)currentIndex);
			return true;
		}

		public void Reset() {
			currentIndex = -1;
			currentNode = default(T);
		}
	}
}