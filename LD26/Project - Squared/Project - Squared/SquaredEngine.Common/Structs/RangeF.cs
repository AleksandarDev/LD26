using System;


namespace SquaredEngine.Common {

	[Serializable]
	public struct RangeF {

		[NonSerialized]
		private static RangeF empty;
		public static RangeF Empty { get { return RangeF.empty; } }

		[NonSerialized]
		private string toString;

		private PositionF upperLeft;
		public PositionF UpperLeft {
			get { return upperLeft; }
			set {
				upperLeft = value;
				UpdateToString();
			}
		}
		
		public PositionF UpperRight
		{
			get { return new PositionF(lowerRight.X, upperLeft.Y); }
			set {
				upperLeft.Y = value.Y;
				lowerRight.X = value.X;		
		
				UpdateToString();
			}
		}
		
		public PositionF LowerLeft
		{
			get { return new PositionF(upperLeft.X, lowerRight.Y); }
			set {
				upperLeft.X = value.X;
				lowerRight.Y = value.Y;

				UpdateToString();
			}
		}

		private PositionF lowerRight;
		public PositionF LowerRight
		{
			get { return lowerRight; }
			set {
				lowerRight = value;
				UpdateToString();
			}
		}

		public float Width {
			get { return lowerRight.X - upperLeft.X; }
		}
		public float Height {
			get { return lowerRight.Y - upperLeft.Y; }
		}


		public RangeF(float size) : this(size, size) { }
		public RangeF(float width, float height) : this(PositionF.Zero, width, height) { }
		public RangeF(PositionF upperLeft, PositionF lowerRight) {
			this.upperLeft = upperLeft;
			this.lowerRight = lowerRight;

			toString = String.Empty;
			UpdateToString();
		}
		public RangeF(PositionF startPositionF, float width, float height) {
			upperLeft = startPositionF;
			lowerRight = new PositionF(startPositionF.X + width, startPositionF.Y + height);

			toString = String.Empty;
			UpdateToString();
		}


		public bool Contains(PositionF PositionF) {
			return PositionF.X >= upperLeft.X && PositionF.Y < lowerRight.Y &&
			       PositionF.X < lowerRight.X && PositionF.Y >= upperLeft.Y;
		}
		public bool Contains(RangeF RangeF) {
			return RangeF.UpperLeft >= upperLeft &&
			       RangeF.LowerRight <= lowerRight;
		}

		public bool floatersects(RangeF RangeF) {
			throw new NotImplementedException();
		}

		public static bool operator ==(RangeF a, RangeF b) {
			return a.Equals(b);
		}

		public static bool operator !=(RangeF a, RangeF b) {
			return !a.Equals(b);
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}

			if (!(obj is RangeF)) {
				return false;
			} else {
				return ((RangeF)obj).upperLeft == upperLeft &&
					   ((RangeF)obj).lowerRight == lowerRight;
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
			this.toString = String.Format("RangeF from {0} to {1}", UpperLeft, LowerRight);
		}
	}
}