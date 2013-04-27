using System;


namespace SquaredEngine.Common {

	[Serializable]
	public struct Position {

		[NonSerialized]
		private string toString;

		/// <summary>
		/// Gets or Sets X coordinate.
		/// </summary>
		public int X {
			get { return this.x; }
			set {
				this.x = value;
				this.toString = null;
			}
		}
		private int x;

		/// <summary>
		/// Gets or Sets Y coordinate.
		/// </summary>
		public int Y {
			get { return this.y; }
			set {
				this.y = value;
				this.toString = null;
			}
		}
		private int y;

		/// <summary>
		/// Gets zero Position object {X:0 Y:0}
		/// </summary>
		public static Position Zero { get { return Position.zero; } }
		[NonSerialized]
		private static readonly Position zero;


		/// <summary>
		/// Creates new Position object.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		public Position(int x, int y) {
			this.x = x;
			this.y = y;

			toString = String.Empty;
			UpdateToString();
		}

		#region Operator overloads

		public static bool operator ==(Position a, Position b) {
			return a.Equals(b);
		}

		public static bool operator !=(Position a, Position b) {
			return !a.Equals(b);
		}

		public static bool operator >(Position a, Position b) {
			return a.X > b.X && a.Y > b.Y;
		}

		public static bool operator <(Position a, Position b) {
			return a.X < b.X && a.Y < b.Y;
		}

		public static bool operator >=(Position a, Position b) {
			return a.X >= b.X && a.Y >= b.Y;
		}

		public static bool operator <=(Position a, Position b) {
			return a.X <= b.X && a.Y <= b.Y;
		}

		public static Position operator +(Position a, int b) {
			return new Position(a.X + b, a.Y + b);
		}

		public static Position operator -(Position a, int b) {
			return new Position(a.X - b, a.Y - b);
		}

		public static Position operator +(Position a, Position b) {
			return new Position(a.X + b.X, a.Y + b.Y);
		}

		public static Position operator -(Position a, Position b) {
			return new Position(a.X - b.X, a.Y - b.Y);
		}

		public static Position operator *(Position a, int b) {
			return new Position(a.X * b, a.Y * b);
		}

		public static Position operator /(Position a, int b) {
			return new Position(a.X / b, a.Y / b);
		}

		public static Position operator *(Position a, Position b) {
			return new Position(a.X * b.X, a.Y * b.Y);
		}

		public static Position operator /(Position a, Position b) {
			return new Position(a.X / b.X, a.Y / b.Y);
		}

		public static implicit operator Microsoft.Xna.Framework.Point(Position a) {
			return new Microsoft.Xna.Framework.Point(a.X, a.Y);
		}

		public static implicit operator Position(Microsoft.Xna.Framework.Point a) {
			return new Position(a.X, a.Y);
		}

		public static implicit operator Microsoft.Xna.Framework.Vector2(Position a) {
			return new Microsoft.Xna.Framework.Vector2(a.X, a.Y);
		}

		public static explicit operator Position(Microsoft.Xna.Framework.Vector2 a) {
			return new Position((int)a.X, (int)a.Y);
		}

		public static implicit operator Microsoft.Xna.Framework.Vector3(Position a) {
			return new Microsoft.Xna.Framework.Vector3(a.X, a.Y, 0f);
		}

		public static explicit operator Position(Microsoft.Xna.Framework.Vector3 a) {
			return new Position((int)a.X, (int)a.Y);
		}

		#endregion

		/// <summary>
		/// Compares two Position object if equal.
		/// </summary>
		/// <param name="obj">Position object to compare to this object.</param>
		/// <returns>True of objects are equal.</returns>
		public override bool Equals(object obj) {
			if (obj == null || !(obj is Position)) {
				return false;
			}
			Position objAsPos = (Position)obj;
			return objAsPos.X == this.X &&
				   objAsPos.Y == this.Y;
		}

		/// <summary>
		/// Creates hash code for this object using X and Y coodrinates.
		/// </summary>
		/// <returns>Integer that represents this object in hash table.</returns>
		public override int GetHashCode() {
			// NOTE: Implemented from http://www.eggheadcafe.com/software/aspnet/29483139/override-gethashcode.aspx
			int hash = 23;
			hash = 851 + this.X;		// 851 = hash * 37 where hash is 23 as defined
			hash = hash * 37 + this.Y;
			return hash;
		}

		/// <summary>
		/// X and Y coordinates in string format.
		/// </summary>
		/// <returns>String containing ths object information.</returns>
		public override string ToString() {
			if (this.toString == null) {
				UpdateToString();
			}

			return this.toString;
		}

		/// <summary>
		/// Saves current ToString call to variable to reduce ToString calls, 
		/// call this if any value that is in a string changes.
		/// </summary>
		private void UpdateToString() {
			// Format: {X:{0} Y:{1}}
			this.toString = "{X:" + this.x + " Y:" + this.y + "}";
		}
	}
}