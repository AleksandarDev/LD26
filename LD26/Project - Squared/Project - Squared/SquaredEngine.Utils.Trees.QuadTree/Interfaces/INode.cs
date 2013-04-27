using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredEngine.Common;


namespace SquaredEngine.Utils.Trees.QuadTree {

	public interface INode<K>
		where K : IComponent {
		int Index { get; }

		event QuadTreeNodeEventHandler<INode<K>, K> OnComponentAddRequest;
		event QuadTreeNodeEventHandler<INode<K>, K> OnComponentAdded;
		event QuadTreeNodeEventHandler<INode<K>, K> OnComponentRemoveRequest;
		event QuadTreeNodeEventHandler<INode<K>, K> OnComponentRemoved;
		event QuadTreeNodeEventHandler<INode<K>, K> OnClear;
		event QuadTreeNodeEventHandler<INode<K>, K> OnInitialize;

		int ComponentThreshold { get; }
		int MinNodeSize { get; }

		bool IsRootNode { get; }
		bool IsEndNode { get; }
		INode<K> RootNode { get; }
		INode<K> Parent { get; }
		
		Range Range { get; }

		bool HasChildren { get; }
		NodeCollection<INode<K>, K> Children { get; }

		bool HasComponents { get; }
		List<K> Components { get; }


		void Initialize();
		
		void AddComponent(K component);

		void RemoveComponent(K component);
		void RemoveComponent(int componentKey);
		void RemoveComponents(IEnumerable<K> components);
		void RemoveComponents(int nodeIndex);

		bool CheckChildrenComponents(bool clearIfEmpty = true);

		IEnumerable<K> GetComponents();
		IEnumerable<K> GetAllComponents();
	
		void Clear(bool clearAll = false);
	}
}
