using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SquaredEngine.Common;
using SquaredEngine.Diagnostics.Debug;
using SquaredEngine.Graphics;
using SquaredEngine.Input;
using IDrawable = SquaredEngine.Graphics.IDrawable;


namespace Game.Models {
	public class StaticTower {
		// Game-play
		public double HealthMax;
		public double HealthCurrent;
		public int Cost;

		// Draw
		public Vector3 Position;
		public Color Color;
		public float Size;
		public bool IsDropped;
		public bool CanBeDropped;
		private IDrawable Drawable;

		public void Rebuild() {
			this.Drawable = new Ellipse(
				this.Position, this.Size, this.Size, this.Color);
		}

		public void Draw(GraphicsDrawer drawer, Time time) {
			if (this.Drawable == null) this.Rebuild();

			drawer.Draw(this.Drawable);
		}
	}

	public class Player {
		public int Resources;
		public string Name;
	}
}
