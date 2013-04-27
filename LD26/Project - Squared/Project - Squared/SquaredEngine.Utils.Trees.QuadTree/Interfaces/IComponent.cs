using SquaredEngine.Common;


namespace SquaredEngine.Utils.Trees.QuadTree {

	public interface IComponent {
		int Key { get; }
		Position Position { get; set; }
	}
}