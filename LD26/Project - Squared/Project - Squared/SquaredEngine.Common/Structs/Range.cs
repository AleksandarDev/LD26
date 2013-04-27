using System;


namespace SquaredEngine.Common {

	[Serializable]
	public struct Range {

		[NonSerialized]
		private static Range empty;
		public static Range Empty { get { return Range.empty; } }

		[NonSerialized]
		private string toString;

		private Position upperLeft;
		public Position UpperLeft {
			get { return upperLeft; }
			set {
				upperLeft = value;
				UpdateToString();
			}
		}
		
		public Position UpperRight
		{
			get { return new Position(lowerRight.X, upperLeft.Y); }
			set {
				upperLeft.Y = value.Y;
				lowerRight.X = value.X;		
		
				UpdateToString();
			}
		}
		
		public Position LowerLeft
		{
			get { return new Position(upperLeft.X, lowerRight.Y); }
			set {
				upperLeft.X = value.X;
				lowerRight.Y = value.Y;

				UpdateToString();
			}
		}

		private Position lowerRight;
		public Position LowerRight
		{
			get { return lowerRight; }
			set {
				lowerRight = value;
				UpdateToString();
			}
		}

		public int Width {
			get { return lowerRight.X - upperLeft.X; }
		}
		public int Height {
			get { return lowerRight.Y - upperLeft.Y; }
		}


		public Range(int size) : this(size, size) { }
		public Range(int width, int height) : this(Position.Zero, width, height) { }
		public Range(Position upperLeft, Position lowerRight) {
			this.upperLeft = upperLeft;
			this.lowerRight = lowerRight;

			toString = String.Empty;
			UpdateToString();
		}
		public Range(Position startPosition, int width, int height) {
			upperLeft = startPosition;
			lowerRight = new Position(startPosition.X + width, startPosition.Y + height);

			toString = String.Empty;
			UpdateToString();
		}


		public bool Contains(Position position) {
			return position.X >= upperLeft.X && position.Y < lowerRight.Y &&
			       position.X < lowerRight.X && position.Y >= upperLeft.Y;
		}
		public bool Contains(Range range) {
			return range.UpperLeft >= upperLeft &&
			       range.LowerRight <= lowerRight;
		}

		public bool Intersects(Range range) {
			throw new NotImplementedException();
		}

		public static bool operator ==(Range a, Range b) {
			return a.Equals(b);
		}

		public static bool operator !=(Range a, Range b) {
			return !a.Equals(b);
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}

			if (!(obj is Range)) {
				return false;
			} else {
				return ((Range)obj).upperLeft == upperLeft &&
					   ((Range)obj).lowerRight == lowerRight;
			}
		}

		// NOTE: Implemented from http://www.eggheadcafe.com/software/aspnet/29483139/override-gethashcode.aspx
		public override int GetHashCode() {
			int hash = 23;
			hash = hash * 37 + this.lowerRight.GetHashCode();
			hash = hash * 37 + this.upperLeft.GetHashCode();
			return hash;
		}

		public override string ToString() {
			return this.toString;
		}
		private void UpdateToString() {
			this.toString = String.Format("Range from {0} to {1}", UpperLeft, LowerRight);
		}
	}
}