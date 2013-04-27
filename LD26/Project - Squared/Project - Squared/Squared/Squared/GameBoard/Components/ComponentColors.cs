using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SquaredEngine.GameBoard.Components {
	public static class ComponentColors {
		private static Random random = new Random();

		private static Dictionary<int, Color> colorsCatalog = new Dictionary<int, Color>() {
			{ 10001, new Color(0, 124, 0, 255) }, // Grass1
			{ 10002, new Color(0, 128, 0, 255) }, // Grass2
			{ 10003, new Color(0, 132, 0, 255) }  // Grass3
		};

		public static Color Color(int id) {
			return colorsCatalog[id];
		}
	}
}
