using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SquaredEngine.PathEditor.Library {
	/// <summary>
	/// Class containing <see cref="Microsoft.Xna.Framework.Input.Mouse"/> extension methods.
	/// </summary>
	public static class MouseExtensions {
		#region Mouse extensions

		/// <summary>
		/// Sets mouse position.
		/// </summary>
		/// <param name="position">Position to set mouse to.</param>
		public static void SetMousePosition(Vector2 position) {
			Mouse.SetPosition((int)position.X, (int)position.Y);
		}

		/// <summary>
		/// Sets mouse position.
		/// </summary>
		/// <param name="position">Position to set mouse to, Z coordinate is not used by this method.</param>
		public static void SetMousePosition(Vector3 position) {
			Mouse.SetPosition((int)position.X, (int)position.Y);
		}

		#endregion

	}
}
