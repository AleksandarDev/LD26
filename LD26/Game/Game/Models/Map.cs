using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SquaredEngine.Common;
using SquaredEngine.Graphics;
using SquaredEngine.Utils.Trees.QuadTree;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Game.Models
{
	public class Map : DrawableGameComponent {
		public const int MapGridSize = 50;
		public const float MapGridSizeOver2 = MapGridSize / 2f;
		public const int MapSize = 64;

		public new GameStart Game { get; private set; }

		public Range mapRange;
		public Texture2D GrassTexture;

		public Node<StaticTower> towersTree;
		public Node<Enemy> enemiesTree; 
		public List<Position> checkForCollision;
		public List<Enemy> EnemiesPassed;

		public static SoundEffect newSpawnSF;
		public List<Position> enemySpawns;
		public Position protectBlock;


		public Map(GameStart game) : base(game) {
			this.Game = game;
		}


		protected override void LoadContent() {
			base.LoadContent();

			this.mapRange = new Range(Map.MapSize, Map.MapSize);
			Random random = new Random();
			this.protectBlock = new Position(random.Next(Map.MapSize - 20) + 10,
				random.Next(Map.MapSize - 20) + 10);

			this.Game.camera.Position =
				new Vector2(
					this.protectBlock.X * Map.MapSize -
					(this.GraphicsDevice.Viewport.Width / 2f + Map.MapGridSizeOver2) /
					this.Game.camera.Zoom
					,
					this.protectBlock.Y * Map.MapSize -
					(this.GraphicsDevice.Viewport.Height / 2f + Map.MapGridSizeOver2) /
					this.Game.camera.Zoom);

			this.EnemiesPassed = new List<Enemy>();
			this.enemiesTree = new Node<Enemy>(null, this.mapRange, 1, 10);
			this.enemiesTree.Initialize();
			this.enemySpawns = new List<Position>();

			this.checkForCollision = new List<Position>();
			this.towersTree = new Node<StaticTower>(null, this.mapRange, 1, 1);
			this.towersTree.Initialize();

			this.GrassTexture = this.Game.Content.Load<Texture2D>("Textures/Grass");
		}

		private int counter = 0;

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			if (this.enemySpawns.Count == 0 || 
				Enemy.CurrentLevel / (25 * this.enemySpawns.Count) >= this.enemySpawns.Count)
			{
				Map.newSpawnSF.Play(0.5f, 0, 0);
				Random random = new Random();
				Position position;
				do {
					position = new Position(random.Next(Map.MapSize - 10) + 5,
						random.Next(Map.MapSize - 10) + 5);
				} while (Position.Distance(position, protectBlock) < 10);
				this.enemySpawns.Add(position);
			}

			// Change path on collision
			foreach (var position in this.checkForCollision) {
				foreach (var enemy in this.enemiesTree.RootNode.GetAllComponents()) {
					if (!enemy.IsFindingPath && !enemy.HasReachedDestination &&
						enemy.PassingTrough(position))
						enemy.FindPath();
				}
			}
			this.checkForCollision.Clear();

			// Create random enemies
			if (counter++ / Math.Max(60, 200-Enemy.CurrentLevel) > 0) {
				counter = 0;
				for (int index = 0; index < this.enemySpawns.Count; index++) {
					this.enemiesTree.AddComponent(new Enemy(this) {
						Position = enemySpawns[index],
						Destination = this.protectBlock
					});
				}
			}

			// Update enemies
			foreach (var enemy in this.enemiesTree.GetAllComponents())
				enemy.Update(this.Game.timeController.Time);

			// Update towers
			foreach (var tower in this.towersTree.GetAllComponents()) {
				tower.Update(this.Game.timeController.Time);
			}

			foreach (var enemy in this.EnemiesPassed) {
				this.Game.GameOver();
				enemy.Kill();
			}
		}

		public override void Draw(GameTime gameTime) {
			base.Draw(gameTime);

			// Draw camera dependent elements
			this.Game.drawer.Begin(samplerState: SamplerState.PointClamp,
				transformMatrix:
					Matrix.CreateTranslation(-this.Game.camera.Position.X,
						-this.Game.camera.Position.Y, 0) *
					Matrix.CreateScale(this.Game.camera.Zoom));
			this.Game.drawer.Draw(this.GrassTexture, this.mapRange * Map.MapGridSize, Color.White);
			this.Game.drawer.End();

			this.Game.drawer.Begin(transformMatrix: this.Game.camera.TransformMatrix);

			this.Game.drawer.Draw(
				new SquaredEngine.Graphics.Rectangle(this.protectBlock * Map.MapGridSize,
					Map.MapGridSize, Color.Red));
			foreach (var enemySpawn in enemySpawns) {
				this.Game.drawer.Draw(
				new SquaredEngine.Graphics.Rectangle(enemySpawn * Map.MapGridSize,
					Map.MapGridSize, Color.Green));
			}

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
