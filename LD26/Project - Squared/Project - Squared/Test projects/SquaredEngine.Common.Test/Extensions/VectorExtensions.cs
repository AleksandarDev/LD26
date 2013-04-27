using System;
using Microsoft.Xna.Framework;
using SquaredEngine.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SquaredEngine.Common.Test.Extensions {
	/// <summary>
	/// Class containing SquaredEngine.Common.Extensions.VectorExtensions test methods
	/// </summary>
	[TestClass]
	public class VectorExtensions {
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext {
			get {
				return testContextInstance;
			}
			set {
				testContextInstance = value;
			}
		}
		private TestContext testContextInstance;

		#region Test methods

		/// <summary>
		/// Test for ToVector2 method
		/// public static Vector2 ToVector2(this Vector3 vector);
		/// </summary>
		[TestMethod]
		public void ToVector2Test() {
			const float TestValueA = 5.722222f;
			const float TestValueB = 7.5213f;
			const float TestValueC = -72.333e-7f;

			Vector3 vectorA = new Vector3(new Vector2(TestValueA, TestValueB), TestValueC);

			Vector2 resultA = vectorA.ToVector2();

			Assert.AreEqual(resultA.X, TestValueA);
			Assert.AreEqual(resultA.Y, TestValueB);
		}

		/// <summary>
		/// Test for ToVector3 method
		/// public static Vector2 ToVector2(this Vector3 vector);
		/// </summary>
		[TestMethod]
		public void ToVector3Test() {
			const float TestValueA = 5.722222f;
			const float TestValueB = -72.333e-7f;

			Vector2 vectorA = new Vector2(TestValueA, TestValueB);

			Vector3 resultA = vectorA.ToVector3();

			Assert.AreEqual(resultA.X, TestValueA);
			Assert.AreEqual(resultA.Y, TestValueB);
			Assert.AreEqual(resultA.Z, 0);
		}

		#endregion
	}
}
