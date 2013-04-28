using System;
using System.Collections.Generic;
using System.Linq;
using SquaredEngine.Common;

namespace SquaredEngine.Utils.Trees.QuadTree {
	public class Node<K> : INode<K>
		where K : IComponent {
		public int Index { get; protected set; }

		public event QuadTreeNodeEventHandler<INode<K>, K> OnComponentAddRequest;
		public event QuadTreeNodeEventHandler<INode<K>, K> OnComponentAdded;
		public event QuadTreeNodeEventHandler<INode<K>, K> OnComponentRemoveRequest;
		public event QuadTreeNodeEventHandler<INode<K>, K> OnComponentRemoved;
		public event QuadTreeNodeEventHandler<INode<K>, K> OnClear;
		public event QuadTreeNodeEventHandler<INode<K>, K> OnInitialize;

		public bool IsRootNode { get; protected set; }
		public bool IsEndNode { get; protected set; }
		public INode<K> RootNode { get; protected set; }
		public INode<K> Parent { get; protected set; }

		public int ComponentThreshold { get; protected set; }
		public int MinNodeSize { get; protected set; }

		protected Range range;
		public Range Range {
			get { return this.range; }
		}

		protected NodeCollection<INode<K>, K> children;
		public bool HasChildren {
			get { return !Children.IsEmpty; }
		}
		public NodeCollection<INode<K>, K> Children {
			get { return this.children; }
		}

		protected List<K> components;
		public bool HasComponents {
			get { return Components.Count != 0; }
		}
		public List<K> Components {
			get { return this.components; }
		}


		public Node(INode<K> parent, Range nodeRange, int minNodeSize = -1, int componentThreshold = -1) {
			Parent = parent;
			MinNodeSize = minNodeSize;
			ComponentThreshold = componentThreshold;

			this.range = nodeRange;


			System.Diagnostics.Debug.WriteLine(String.Format("{0}Node created {1}", IsRootNode ? "Root" : String.Empty, Range));
		}


		public virtual void Initialize() {
			IsRootNode = Parent == null;

			if (IsRootNode) {
				ConfigureAsRoot();
			}
			else {
				ConfigureAsChild();
			}

			IsEndNode = (this.range.Width == RootNode.MinNodeSize || this.range.Height == RootNode.MinNodeSize);

			Clear(true);

			if (OnInitialize != null) {
				OnInitialize(this, new NodeEventArgs<K>());
			}
		}
		private void ConfigureAsChild() {
			RootNode = Parent.RootNode;

			MinNodeSize = RootNode.MinNodeSize;
			ComponentThreshold = RootNode.ComponentThreshold;
		}
		private void ConfigureAsRoot() {
			if (MinNodeSize <= 0) throw new InvalidOperationException("MinNodeSize must be greater than zero ( > 0 )!");
			if (this.range.Width % MinNodeSize != 0) throw new InvalidOperationException("Tree width must be dividable by minNodeSize!");
			if (this.range.Height % MinNodeSize != 0) throw new InvalidOperationException("Tree height must be dividable by minNodeSize!");

			if (ComponentThreshold <= 0) throw new InvalidOperationException("ComponentThreshold must be greater than zero ( > 0 )!");

			RootNode = this;
		}

		public virtual void AddComponent(K component, bool checkTreshold = true) {
			if (this.HasChildren) {
				// Trazi smjer u koje djete treba dodati komponentu
				Directions direction = NodeCollection<INode<K>, K>.GetDirection(Range, component.Position);

				// Dodaje komponentu u djete
				this.Children.GetNode(direction).AddComponent(component, checkTreshold);	
			}
			else {
				// Dodaje komponentu u ovaj cvor
				Components.Add(component);

				if (OnComponentAdded != null) {
					OnComponentAdded(this, new NodeEventArgs<K>() {
						Component = component
					});
				}

				System.Diagnostics.Debug.WriteLine(
					String.Format("Component added to node {0}", Range));
			}

			// Provjerava da li je granica komponenti prijedena te da li nije trenutni cvor krajnji cvor
			if (!IsEndNode && checkTreshold && Components.Count >= ComponentThreshold)
			{
				// Kreira djecu cvora ukoliko ona vec ne postoje
				CreateChildNodes();

				foreach (var comp in this.Components) {
					// Trazi smjer u koje djete treba dodati komponentu
					Directions direction = NodeCollection<INode<K>, K>.GetDirection(Range, comp.Position);

					// Dodaje komponentu u djete
					Children.GetNode(direction).AddComponent(comp, checkTreshold);	
				}
				this.components.Clear();
			}

			if (OnComponentAddRequest != null) {
				OnComponentAddRequest(this, new NodeEventArgs<K>() {
					Component = component
				});
			}
		}

		public void UpdateComponent(K component) {
			RemoveComponent(component, false);
			AddComponent(component);
			return;
			//if (this.HasChildren) {
			//    var direction = NodeCollection<INode<K>, K>.GetDirection(Range,
			//        component.Position);
			//    this.children.GetNode(direction).UpdateComponent(component, newPosition);
			//    return;
			//}

			//// Parent of component can update component position
			//if (!this.range.Contains(newPosition)) {
			//    this.RootNode.RemoveComponent(component, false);
			//    component.Position = newPosition;
			//    this.RootNode.AddComponent(component);
			//}
		}

		public bool RemoveComponent(K component, bool checkDirection = true) {
			bool removed = false;
			removed = this.components.Remove(component);
			if (removed && this.Parent != null)
				Parent.CheckChildrenComponents();

			if (!removed && this.HasChildren) {
				if (checkDirection) {
					var direction = NodeCollection<INode<K>, K>.GetDirection(this.range,
						component.Position);
					removed = Children.GetNode(direction)
						.RemoveComponent(component, checkDirection);
				}
				else {
					foreach (var child in this.children) {
						removed = child.RemoveComponent(component, checkDirection);
						if (removed) break;
					}
				}
			}

			return removed;
		}
		public void RemoveComponent(int componentKey) {
			K component = this.components.Find(comp => comp.Key == componentKey);

			if (component != null) {
				this.components.Remove(component);

				if (this.components.Count == 0) {
					Parent.CheckChildrenComponents();
				}
			}
		}
		public void RemoveComponents(IEnumerable<K> componentsToRemove) {
			foreach (K component in componentsToRemove) {
				RemoveComponent(component, false);
			}
		}
		public void RemoveComponents(int nodeIndex) {
			throw new NotImplementedException();
		}

		protected virtual void CreateChildNodes() {
			if (Children.IsEmpty) {
				for (int directionIndex = 0; directionIndex < 4; directionIndex++) {
					Directions direction = (Directions)directionIndex;
					Range childRange = NodeCollection<INode<K>, K>.GetRange(Range, direction);

					Children.SetNode(direction, new Node<K>(this, childRange));
					Children.GetNode(direction).Initialize();
				}
			}
		}
		public bool CheckChildrenComponents(bool clearIfEmpty = true) {
			if (!this.HasChildren && this.Components.Count == 0)
				return true;

			if (this.HasChildren) {
				if (Children.All(child => child.CheckChildrenComponents())) {
					if (clearIfEmpty) {
						this.children = new NodeCollection<INode<K>, K>();
					}

					if (this.Parent != null)
						this.Parent.CheckChildrenComponents(clearIfEmpty);

					return true;
				}
			}

			return false;
		}

		public virtual void Clear(bool clearAll = false) {
			this.components = new List<K>();

			if (clearAll) {
				this.children = new NodeCollection<INode<K>, K>();
			}
		}

		public IEnumerable<K> GetComponents() {
			return this.components;
		}

		public IEnumerable<K> GetAllComponents() {
			List<K> requestedComponents = new List<K>();

			requestedComponents.AddRange(this.GetComponents());
			foreach (var child in Children)
				requestedComponents.AddRange(child.GetAllComponents());

			return requestedComponents;
		}

		public IEnumerable<K> GetComponents(Position position) {
			if (!this.range.Contains(position))
				return new List<K>();

			if (this.HasComponents)
				return this.GetComponents();
			if (this.HasChildren) {
				var components = new List<K>();
				foreach (var child in this.children)
					components.AddRange(child.GetComponents(position));
				return components;
			}

			return new List<K>();
		}
	}
}