using System;
using System.Collections;
using System.Collections.Generic;
using SquaredEngine.Common;


namespace SquaredEngine.Utils.Trees.QuadTree {

	public class NodeCollection<T, K> : IEnumerable<T>
		where T : class, INode<K> where K : IComponent{
		public const int NodesCount = 4;
		protected T[] nodes;

		public int Count {
			get { return IsEmpty ? 0 : NodesCount; }
		}

		public bool IsEmpty {
			get { return NW == null || NE == null || SW == null || SE == null; }
		}

		public T[] Nodes {
			get { return nodes; }
		}

		public T NW {
			get { return GetNode(Directions.NW); }
			set { SetNode(Directions.NW, value); }
		}

		public T NE {
			get { return GetNode(Directions.NE); }
			set { SetNode(Directions.NE, value); }
		}

		public T SW {
			get { return GetNode(Directions.SW); }
			set { SetNode(Directions.SW, value); }
		}

		public T SE {
			get { return GetNode(Directions.SE); }
			set { SetNode(Directions.SE, value); }
		}


		public NodeCollection() {
			Initialize();
		}


		public IEnumerator<T> GetEnumerator() {
			return new NodesEnumerator<T, K>(this);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new NodesEnumerator<T, K>(this);
		}

		protected void Initialize() {
			Clear();
		}

		public T GetNode(Directions direction) {
			return nodes[(int)direction];
		}

		public void SetNode(Directions direction, T node) {
			// TODO: Provijeriti lock naredbu
			nodes[(int)direction] = node;
		}

		public void Clear() {
			nodes = new T[NodesCount];
		}

		public override string ToString() {
			return String.Format("{0} ({1})", base.ToString(), IsEmpty ? "Empty" : "Filled");
		}

		public static Directions GetDirection(Range nodeRange, Position componentPosition) {
			if (nodeRange.Width % 2 != 0) {
				throw new ArgumentException("Node width must be dividable by two (2)!", "nodeRange");
			}
			if (nodeRange.Height % 2 != 0) {
				throw new ArgumentException("Node height must be dividable by two (2)!", "nodeRange");
			}
			if (!nodeRange.Contains(componentPosition)) {
				throw new ArgumentException("This node doesn't contain position of given component!", "componentPosition");
			}

			for (int currentDirection = 0; currentDirection < NodesCount; currentDirection++) {
				// Dobavlja opseg pozicija koje obuhvaca trenutni smjer
				Range currentRange = GetRange(nodeRange, (Directions)currentDirection);

				// Provjerava da li se u opsegu nalazi trenutna komponenta
				if (currentRange.Contains(componentPosition)) {
					// Vraca prvi pronadeni smjer koji odgovara poziciji komponente
					return (Directions)currentDirection;
				}
			}

			return Directions.NE;
		}
		public static Range GetRange(Range nodeRange, Directions direction) {
			if (nodeRange.Width % 2 != 0 && nodeRange.Width != 1) {
				throw new ArgumentException("Node width must be dividable by two (2)!", "nodeRange");
			}
			if (nodeRange.Height % 2 != 0 && nodeRange.Height != 1) {
				throw new ArgumentException("Node height must be dividable by two (2)!", "nodeRange");
			}

			int widthBy2 = nodeRange.Width / 2;
			int heightBy2 = nodeRange.Height / 2;

			switch (direction) {
				case Directions.NW:
					return new Range(nodeRange.UpperLeft,
									 new Position(nodeRange.UpperLeft.X + widthBy2, nodeRange.UpperLeft.Y + heightBy2));
				case Directions.NE:
					return new Range(new Position(nodeRange.UpperLeft.X + widthBy2, nodeRange.UpperLeft.Y),
									 new Position(nodeRange.UpperLeft.X + nodeRange.Width, nodeRange.UpperLeft.Y + heightBy2));
				case Directions.SW:
					return new Range(new Position(nodeRange.UpperLeft.X, nodeRange.UpperLeft.Y + heightBy2),
									 new Position(nodeRange.UpperLeft.X + widthBy2, nodeRange.UpperLeft.Y + nodeRange.Height));
				case Directions.SE:
					return new Range(new Position(nodeRange.UpperLeft.X + widthBy2, nodeRange.UpperLeft.Y + heightBy2),
									 nodeRange.LowerRight);
				default:
					throw new ArgumentOutOfRangeException("direction");
			}
		}
	}
}