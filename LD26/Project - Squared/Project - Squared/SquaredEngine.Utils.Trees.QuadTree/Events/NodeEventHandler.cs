
namespace SquaredEngine.Utils.Trees.QuadTree {

	/// <summary>
	/// Handles events for Node
	/// </summary>
	/// <typeparam name="T">Class that derives from INode<![CDATA[<K>]]></typeparam>
	/// <typeparam name="K">Class or interface that derives from IComponent</typeparam>
	/// <param name="node">A QuadTree node which fired the event</param>
	/// <param name="eventArgs">QuadTree node event arguments</param>
	public delegate void QuadTreeNodeEventHandler<in T, K>(T node, NodeEventArgs<K> eventArgs)
		where T : class, INode<K> where K : IComponent;
}
