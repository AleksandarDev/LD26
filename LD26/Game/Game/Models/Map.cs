using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using SquaredEngine.Common;
using SquaredEngine.Utils.Trees.QuadTree;

namespace Game.Models
{
	public class Map : DrawableGameComponent {
		public const int MapGridSize = 50;
		public const float MapGridSizeOver2 = MapGridSize / 2f;
		public const int MapSize = 64;

		public new GameStart Game { get; private set; }

		public Range mapRange;

		public Node<StaticTower> towersTree;
		public Node<Enemy> enemiesTree; 
		public List<Position> checkForCollision; 


		public Map(GameStart game) : base(game) {
			this.Game = game;
		}


		protected override void LoadContent() {
			base.LoadContent();

			this.mapRange = new Range(Map.MapSize, Map.MapSize);

			this.enemiesTree = new Node<Enemy>(null, this.mapRange, 1, 10);
			this.enemiesTree.Initialize();

			this.checkForCollision = new List<Position>();
			this.towersTree = new Node<StaticTower>(null, this.mapRange, 1, 1);
			this.towersTree.Initialize();
		}

		private int counter = 0;

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			foreach (var position in this.checkForCollision) {
				foreach (var enemy in this.enemiesTree.RootNode.GetAllComponents()) {
					if (!enemy.IsFindingPath && !enemy.HasReachedDestination &&
						enemy.PassingTrough(position))
						enemy.FindPath();
				}
			}
			this.checkForCollision.Clear();


			if (counter++ % 60 == 0) {
				Random random = new Random();
				var newenemy = new Enemy(this);
				newenemy.Position = new Vector3(random.Next(Map.MapSize),
					random.Next(Map.MapSize), 0);
				newenemy.Destination = new Position(0, 0);
				newenemy.Rebuild();
				this.enemiesTree.AddComponent(newenemy);
			}

			// Update enemies
			foreach (var enemy in this.enemiesTree.GetAllComponents())
				enemy.Update(this.Game.timeController.Time);

			// Update towers
			foreach (var tower in this.towersTree.GetAllComponents()) {
				tower.Update(this.Game.timeController.Time);
			}
		}

		public override void Draw(GameTime gameTime) {
			base.Draw(gameTime);

			// Draw camera dependent elements
			this.Game.drawer.Begin(transformMatrix: this.Game.camera.TransformMatrix);

			// Draw all towers
			foreach (var tower in this.towersTree.RootNode.GetAllComponents())
				tower.Draw(this.Game.drawer, this.Game.timeController.Time);

			// Draw enemies
			foreach (var enemy in this.enemiesTree.GetAllComponents())
				enemy.Draw(this.Game.drawer, this.Game.timeController.Time);

			this.Game.drawer.End();
		}
	}
}
