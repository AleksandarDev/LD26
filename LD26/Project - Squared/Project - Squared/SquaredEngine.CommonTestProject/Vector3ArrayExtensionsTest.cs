using SquaredEngine.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xna.Framework;

namespace SquaredEngine.CommonTestProject
{    
	/// <summary>
	///This is a test class for Vector3ArrayExtensionsTest and is intended
	///to contain all Vector3ArrayExtensionsTest Unit Tests
	///</summary>
	[TestClass()]
	public class Vector3ArrayExtensionsTest {


		private TestContext testContextInstance;

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

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		/// A test for SearchPointRadius
		///</summary>
		[TestMethod()]
		[DeploymentItem("SquaredEngine.Common.dll")]
		public void SearchPointRadiusTest() {
			Vector3[] points = new Vector3[] { new Vector3(100, 100, 100), new Vector3(105, 105, 105), new Vector3(102, 150, 100) };
			Vector3 position = new Vector3(101, 101, 101);
			float inRadius = 3.0f;

			int expected = 0;
			
			int actual;
			actual = Vector3ArrayExtensions_Accessor.SearchPointRadius(points, position, inRadius);
			
			Assert.AreEqual(expected, actual);
		}
	}
}
