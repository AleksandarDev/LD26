using System;
using Microsoft.Xna.Framework;

namespace SquaredEngine.Common.Extensions {
	/// <summary>
	/// Class containing Microsoft.Xna.Framework.Vector2D and Microsoft.Xna.Framework.Vector3D extension methods
	/// </summary>
	public static class VectorExtansions {
		/// <summary>
		/// Converts Vector3 to Vector2 by losing the z coordinate.
		/// </summary>
		/// <param name="vector">Vector3 to convert.</param>
		/// <returns>New Vector2 object equal to given Vector3 object but with excluded z coordinate.</returns>
		public static Vector2 ToVector2(this Vector3 vector) {
			return new Vector2(vector.X, vector.Y);
		}

		/// <summary>
		/// Converts Vector2 to Vector3 by adding the z coordinate.
		/// </summary>
		/// <param name="vector">Vector2 to convert.</param>
		/// <returns>New Vector3 object equal to given Vector2 object with z coordinate equal to zero (0).</returns>
		public static Vector3 ToVector3(this Vector2 vector) {
			return new Vector3(vector, 0f);
		}

		/// <summary>
		/// Adds x and y values to Vector2 coordinates.
		/// </summary>
		/// <param name="vector">Vector to add to.</param>
		/// <param name="x">Value to add to vector X coordinate.</param>
		/// <param name="y">Value to add to vector Y coordinate.</param>
		/// <returns>New Vector2 object with sum of previous coordinates and given values.</returns>
		public static Vector2 Add(this Vector2 vector, float x, float y) {
			return new Vector2(vector.X + x, vector.Y + y);
		}

		/// <summary>
		/// Adds xy value to Vector2 coordinates.
		/// </summary>
		/// <param name="vector">Vector to add to.</param>
		/// <param name="xy">Value to add to vector X and Y coordinates.</param>
		/// <returns>New Vector2 object with sum of previous coordinates and given value.</returns>
		public static Vector2 Add(this Vector2 vector, float xy) {
			return new Vector2(vector.X + xy, vector.Y + xy);
		}

		
		/// <summary>
		/// Adds x, y and z values to Vector3 coordinates.
		/// </summary>
		/// <param name="vector">Vector to add to.</param>
		/// <param name="valueX">Value to add to vector X coordinate.</param>
		/// <param name="valueY">Value to add to vector Y coordinate.</param>
		/// <param name="valueZ">Value to add to vector Z coordinate.</param>
		/// <returns>New Vector3 object with sum of previous coordinates and given values.</returns>
		public static Vector3 Add(this Vector3 vector, float valueX, float valueY, float valueZ) {
			return new Vector3(vector.X + valueX, vector.Y + valueY, vector.Z + valueZ);
		}

		/// <summary>
		/// Adds x, y and z values to Vector3 coordinates.
		/// </summary>
		/// <param name="vector">Vector to add to.</param>
		/// <param name="xy">Value to add to vector X and Y coordinates.</param>
		/// <param name="z">Value to add to vector Z coordinate.</param>
		/// <returns>New Vector3 object with sum of previous coordinates and given values.</returns>
		public static Vector3 Add(this Vector3 vector, float xy, float z = 0) {
			return new Vector3(vector.X + xy, vector.Y + xy, vector.Z + z);
		}

		/// <summary>
		/// Adds x, y and z values to Vector3 coordinates.
		/// </summary>
		/// <param name="vector">Vector to add to.</param>
		/// <param name="xyz">Value to add to vector X, Y and Z coordinates.</param>
		/// <returns>New Vector3 object with sum of previous coordinates and given values.</returns>
		public static Vector3 Add(this Vector3 vector, float xyz) {
			return new Vector3(vector.X + xyz, vector.Y + xyz, vector.Z + xyz);
		}

		/// <summary>
		/// Searches for first point in radius around given position
		/// </summary>
		/// <param name="points">Array of searchable points</param>
		/// <param name="position">Around which position to search for point</param>
		/// <param name="inRadius">In which radius from position to search for point</param>
		/// <returns>Returns -1 if no point is found, if point is found then returns index of point from array</returns>
		private static int SearchPointRadius(this Vector3[] points, Vector3 position, Single inRadius) {
			if (float.IsNaN(inRadius))
				throw new NotFiniteNumberException("inRadius");
			if (points == null)
				throw new NullReferenceException("points");

			int resultIndex = -1;

			// Paralelno prelazi kroz sve dobivene tocke
			System.Threading.Tasks.Parallel.For(0, points.Length, (index, state) => {
				// Oduzima poziciju (X, Y, Z zasebno) tocke i zadanu poziciju te 
				// razliku usporeduje s dozvoljenim radiusom, ukoliko je uvjet 
				// zadvoljen sprema se i vraca indeks pronadene tocke
				if (Math.Abs(points[index].X - position.X) <= inRadius &&
					Math.Abs(points[index].Y - position.Y) <= inRadius &&
					Math.Abs(points[index].Z - position.Z) <= inRadius) {
					if (!state.IsStopped) {
						resultIndex = index;
						state.Stop();
					}
				}
			});

			return resultIndex;
		}
	}
}