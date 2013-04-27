using System;
using Microsoft.Xna.Framework;


namespace SquaredEngine.Graphics {

	public class Camera2DEventArgs : EventArgs {
		public Vector2 Position;
		public Vector3 Position3;
		public Matrix TransformMatrix;
	}
}
