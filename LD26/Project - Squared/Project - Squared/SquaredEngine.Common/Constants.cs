using System;

namespace SquaredEngine.Common {
	/// <summary>
	/// Collection of common constants used in engine
	/// </summary>
	public static class Constants {
		#region PI constants

		/// <summary>
		/// Cooresponds to value of two PI (6.28318530...)
		/// </summary>
		public const Single PI2 = (Single)(Math.PI * 2.0);

		/// <summary>
		/// Cooresponds to value of PI over two (1.570796326...)
		/// </summary>
		public const Single PIOver2 = (Single)(Math.PI / 2.0);

		#endregion

		#region Epsilons

		/// <summary>
		/// Epsilon constant with less tolerance used for Double comparison
		/// </summary>
		public const Double EpsilonLess = 4.94065645841247e-324;

		/// <summary>
		/// Epsilon constant with greater tolerance used for Double comparison
		/// </summary>
		public const Double EpsilonGreater = 1.0e-16;

		/// <summary>
		/// Epsilon constant with less tolerance used for Single comparison
		/// </summary>
		public const Single EpsilonFLess = (float) 1.4e-45;

		/// <summary>
		/// Epsilon constant with greater tolerance used for Single comparison
		/// </summary>
		public const Single EpsilonFGreater = (float) 1.0e-8;

		#endregion
	}
}
