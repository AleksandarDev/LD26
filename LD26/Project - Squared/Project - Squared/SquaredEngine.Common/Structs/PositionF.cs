using System;


namespace SquaredEngine.Common {

	[Serializable]
	public struct PositionF {

		[NonSerialized]
		private string toString;

		private float x;
		public float X {
			get { return this.x; }
			set {
				this.x = value;
				UpdateToString();
			}
		}

		private float y;
		public float Y {
			get { return this.y; }
			set {
				this.y = value;
				UpdateToString();
			}
		}

		[NonSerialized]
		private static PositionF zero;
		public static PositionF Zero { get { return PositionF.zero; } }


		public PositionF(float x, float y) {
			this.x = x;
			this.y = y;

			toString = String.Empty;
			UpdateToString();
		}

		#region Operator overloads

		public static bool operator ==(PositionF a, PositionF b) {
			return a.Equals(b);
		}

		public static bool operator !=(PositionF a, PositionF b) {
			return !a.Equals(b);
		}

		public static bool operator >(PositionF a, PositionF b) {
			return a.X > b.X && a.Y > b.Y;
		}

		public static bool operator <(PositionF a, PositionF b) {
			return a.X < b.X && a.Y < b.Y;
		}

		public static bool operator >=(PositionF a, PositionF b) {
			return a.X >= b.X && a.Y >= b.Y;
		}

		public static bool operator <=(PositionF a, PositionF b) {
			return a.X <= b.X && a.Y <= b.Y;
		}

		public static PositionF operator +(PositionF a, float b) {
			return new PositionF(a.X + b, a.Y + b);
		}

		public static PositionF operator -(PositionF a, float b) {
			return new PositionF(a.X - b, a.Y - b);
		}

		public static PositionF operator +(PositionF a, PositionF b) {
			return new PositionF(a.X + b.X, a.Y + b.Y);
		}

		public static PositionF operator -(PositionF a, PositionF b) {
			return new PositionF(a.X - b.X, a.Y - b.Y);
		}

		public static PositionF operator *(PositionF a, float b) {
			return new PositionF(a.X * b, a.Y * b);
		}

		public static PositionF operator /(PositionF a, float b) {
			return new PositionF(a.X / b, a.Y / b);
		}

		public static PositionF operator *(PositionF a, PositionF b) {
			return new PositionF(a.X * b.X, a.Y * b.Y);
		}

		public static PositionF operator /(PositionF a, PositionF b) {
			return new PositionF(a.X / b.X, a.Y / b.Y);
		}

		public static explicit operator Microsoft.Xna.Framework.Point(PositionF a) {
			return new Microsoft.Xna.Framework.Point((int)a.X, (int)a.Y);
		}

		public static implicit operator PositionF(Microsoft.Xna.Framework.Point a) {
			return new PositionF(a.X, a.Y);
		}

		public static implicit operator Microsoft.Xna.Framework.Vector2(PositionF a) {
			return new Microsoft.Xna.Framework.Vector2(a.X, a.Y);
		}

		public static implicit operator PositionF(Microsoft.Xna.Framework.Vector2 a) {
			return new PositionF(a.X, a.Y);
		}

		public static implicit operator Microsoft.Xna.Framework.Vector3(PositionF a) {
			return new Microsoft.Xna.Framework.Vector3(a.X, a.Y, 0f);
		}

		public static implicit operator PositionF(Microsoft.Xna.Framework.Vector3 a) {
			return new PositionF(a.X, a.Y);
		}

		#endregion

		public override bool Equals(object obj) {
			if (obj == null || !(obj is PositionF)) {
				return false;
			}
			PositionF objAsPos = (PositionF)obj;
			return objAsPos.X == this.X &&
				   objAsPos.Y == this.Y;
		}

		// NOTE: Implemented from http://www.eggheadcafe.com/software/aspnet/29483139/override-gethashcode.aspx
		public override int GetHashCode() {
			int hash = 23;
			hash = hash * 37 + (int)this.X;
			hash = hash * 37 + (int)this.Y;
			return hash;
		}

		public override string ToString() {
			return this.toString;
		}
		private void UpdateToString() {
			this.toString = "{X:" + this.x + " Y:" + this.y + "}";
		}
	}
}