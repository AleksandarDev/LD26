using System;

namespace SquaredEngine.GameBoard.Helpers {
	public static class KeyGenerator {
		private const UInt64 DefaultKey = 10001;
		private static UInt64 currentKey = DefaultKey;


		public static UInt64 GetKeyUInt64() {
			return currentKey++;
		}
		public static UInt32 GetKeyUInt32() {
			return (UInt32)GetKeyUInt64();
		}
		public static Int64 GetKeyInt64() {
			return (Int64)GetKeyUInt64();
		}
		public static Int32 GetKeyInt32() {
			return (Int32)GetKeyUInt32();
		}
	}
}
