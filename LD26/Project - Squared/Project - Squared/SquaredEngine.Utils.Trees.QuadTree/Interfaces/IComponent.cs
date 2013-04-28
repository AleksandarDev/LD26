using SquaredEngine.Common;


namespace SquaredEngine.Utils.Trees.QuadTree {

	public interface IComponent {
		int Key { get; set; }
		Position Position { get; set; }
	}
}