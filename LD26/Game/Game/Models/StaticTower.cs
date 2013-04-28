using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Game.PathFinder;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SquaredEngine.Common;
using SquaredEngine.Diagnostics.Debug;
using SquaredEngine.Graphics;
using SquaredEngine.Input;
using SquaredEngine.Utils.Trees.QuadTree;
using IDrawable = SquaredEngine.Graphics.IDrawable;
using Rectangle = SquaredEngine.Graphics.Rectangle;


namespace Game.Models {
	public class PathNode :
		PathFinder.PathFinder<PathNode>.IHasNeighbours<PathNode> {
		public readonly Map Map;
		public readonly PathNode Parent;
		public readonly int X, Y;
		public readonly Vector3 Position;


		public PathNode(Map map, PathNode parentNode, int x, int y) {
			this.Map = map;
			this.Parent = parentNode;
			this.X = x;
			this.Y = y;
			this.Position = new Vector3(this.X, this.Y, 0);
		}


		public IEnumerable<PathNode> GetSuccessors() {
			for (int y = -1; y < 2; y++) {
				for (int x = -1; x < 2; x++) {
					if ((y == 0 && x == 0) || y + this.Y < 0 || x + this.X < 0)
						continue;
					if (Parent != null && (this.Y + y == Parent.Y && this.X + x == Parent.X))
						continue;
					if (this.Map != null &&
						this.Map.towersTree.GetComponents(new Position(this.X + x, this.Y + y))
							.Any())
						continue;
					// TODO Check if not blocked

					yield return new PathNode(this.Map, this, this.X + x, this.Y + y);
				}
			}
		}

		public IEnumerable<PathNode> Neighbours {
			get { return this.GetSuccessors(); }
		}

		public override bool Equals(object obj) {
			var path = obj as PathNode;
			if (path == null) return false;

			return path.X == this.X && path.Y == this.Y;
		}

		/// <summary>
		/// Creates hash code for this object using X and Y coodrinates.
		/// </summary>
		/// <returns>Integer that represents this object in hash table.</returns>
		public override int GetHashCode() {
			// NOTE: Implemented from http://www.eggheadcafe.com/software/aspnet/29483139/override-gethashcode.aspx
			int hash = 23;
			hash = 851 + this.X; // 851 = hash * 37 where hash is 23 as defined
			hash = hash * 37 + this.Y;
			return hash;
		}

		public override string ToString() {
			return "{X: " + this.X + " Y: " + this.Y + "}";
		}
	}

	public class Enemy : IComponent {
		private static readonly Color EnemyColor = Color.Blue;


		int IComponent.Key { get; set; }

		Position IComponent.Position {
			get { return (Position) this.Position; }
			set { this.Position = value; }
		}

		// Game-play
		public Map Map;
		public double HealthMax;
		public double HealthCurrent;
		public PositionF Position;
		private Position previousPosition;
		public Position Destination;
		private Position prevDestination;
		public bool IsFindingPath;
		public bool HasReachedDestination;

		// Health bar
		public Vector3 barOffset;
		public Vector3 barUL;
		public Vector3 barDROffset;
		private IDrawable hpbar;
		private IDrawable hpBarOverlay;

		// Draw
		public Vector3 AbsoutePosition;
		public Color Color;
		private IDrawable drawable;

		public PathFinder.PathFinder<PathNode> finder;
		public Stack<PathNode> movePath;
		public PathNode currentPathNode;


		public Enemy(Map map) {
			this.Map = map;
			this.Rebuild();
			this.finder = new PathFinder<PathNode>();
			this.prevDestination = new Position(-1, -1);
			this.movePath = new Stack<PathNode>();
			this.IsFindingPath = true;

			this.HealthMax = 150.0;
			this.HealthCurrent = this.HealthMax;

			this.barOffset = new Vector3(-Map.MapGridSizeOver2, -Map.MapGridSizeOver2 - 10, 0);
			this.barDROffset = new Vector3(Map.MapGridSize, 5, 0);
		}


		public void Update(Time time) {
			if (this.Destination != this.prevDestination)
				this.FindPath();
			this.prevDestination = this.Destination;

			if (!this.IsFindingPath) {
				if ((int) Math.Round(this.Position.X) == this.currentPathNode.X &&
					(int) Math.Round(this.Position.Y) == this.currentPathNode.Y) {

					// Pop current position and move to another
					if (this.movePath.Count > 0) {
						this.currentPathNode = this.movePath.Pop();
					}
					else this.HasReachedDestination = true;
					// TODO else reached destination
				}

				// Move toward next destination
				if (!this.HasReachedDestination) {
					var d = Vector3.Normalize(new Vector3(
						this.currentPathNode.Position.X - this.Position.X,
						this.currentPathNode.Position.Y - this.Position.Y, 0));
					this.Position += d * (float) time.Delta;

					if (this.previousPosition.X != (int) this.Position.X ||
						this.previousPosition.Y != (int) this.Position.Y) {
						this.Map.enemiesTree.RootNode.UpdateComponent(this);
						this.previousPosition = (Position) this.Position;
					}
				}
			}

			this.Rebuild();

			// Check if enemy should be dead
			if (this.HealthCurrent <= 0)
				this.Kill();
		}

		public void FindPath() {
			this.HasReachedDestination = false;
			var pathThread = new Thread(() => {
				this.IsFindingPath = true;
				var startNode = new PathNode(this.Map, null,
					(int) this.Position.X, (int) this.Position.Y);
				var endNode = new PathNode(this.Map, null,
					Math.Max(0, Math.Min(Map.MapSize, this.Destination.X)),
					Math.Max(0, Math.Min(Map.MapSize, this.Destination.Y)));
				var path = this.finder.FindPath(
					startNode, endNode, Distance2Nodes,
					p => Distance2Nodes(p, endNode));
				if (path != null) {
					this.movePath.Clear();
					foreach (var node in path) {
						this.movePath.Push(node);
					}
					this.currentPathNode = this.movePath.Pop();
				}
				this.IsFindingPath = false;
			});
			pathThread.Start();
		}

		public bool PassingTrough(Position p) {
			return this.movePath.Any(pn => pn.X == p.X && pn.Y == p.Y);
		}

		public static double Distance2Nodes(PathNode a, PathNode b) {
			return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
		}

		public void Rebuild() {
			this.Color = EnemyColor;

			// Absolute position
			this.AbsoutePosition = this.Position * Map.MapGridSize +
				new Vector3(Map.MapGridSizeOver2, Map.MapGridSizeOver2, 0);

			Vector3[] points = new Vector3[3];
			points[0] = this.AbsoutePosition + new Vector3(0, -Map.MapGridSizeOver2, 0);
			points[1] = this.AbsoutePosition +
				new Vector3(Map.MapGridSizeOver2, Map.MapGridSizeOver2, 0);
			points[2] = this.AbsoutePosition +
				new Vector3(-Map.MapGridSizeOver2, Map.MapGridSizeOver2, 0);

			float rotation = 0;
			if (this.currentPathNode != null) {
				rotation =
					(float)
						(Math.PI -
							Math.Atan2(this.currentPathNode.X - this.Position.X,
								this.currentPathNode.Y - this.Position.Y));
			}
			else
				rotation =
					(float)
						(Math.PI -
							Math.Atan2(this.Destination.X - this.Position.X,
								this.Destination.Y - this.Position.Y));

			GraphicsDrawer.RotateAboutOrigin(this.AbsoutePosition,
				rotation, ref points);

			// Draw primitive
			this.drawable = new GraphicsDrawer.Triangle(
				points[0], points[1], points[2], this.Color);

			this.barUL = this.barOffset / this.Map.Game.camera.Zoom + this.AbsoutePosition;
			this.hpbar = new Rectangle(this.barUL, this.barUL + this.barDROffset / this.Map.Game.camera.Zoom,
				Color.Black);
			var overlayDROffset =
				new Vector3((float)(this.barDROffset.X * (this.HealthCurrent / this.HealthMax)),
					this.barDROffset.Y, 0) / this.Map.Game.camera.Zoom;
			this.hpBarOverlay = new Rectangle(this.barUL, this.barUL + overlayDROffset,
				Color.Green);
		}

		public void Draw(GraphicsDrawer drawer, Time time) {
			if (this.drawable == null) this.Rebuild();

			drawer.Draw(this.drawable);

			// Draw health bar
			drawer.Draw(this.hpbar);
			drawer.Draw(this.hpBarOverlay);

#if DEBUG
			foreach (var pathNode in this.movePath) {
				if (pathNode != null && pathNode.Parent != null)
					drawer.Draw(new GraphicsDrawer.Line(
						new Vector3(pathNode.Parent.X, pathNode.Parent.Y, 0) * Map.MapGridSize +
						new Vector3(Map.MapGridSizeOver2, Map.MapGridSizeOver2, 0),
						new Vector3(pathNode.X, pathNode.Y, 0) * Map.MapGridSize +
						new Vector3(Map.MapGridSizeOver2, Map.MapGridSizeOver2, 0), Color.White));
			}
#endif
		}

		public void Kill() {
			this.Map.enemiesTree.RemoveComponent(this);
		}

		public void DoDamage(double damage) {
			this.HealthCurrent -= damage;
		}
	}

	public class StaticTower : IComponent {
		public const float TowerSize = Map.MapGridSizeOver2;
		public const float TowerSizeD = TowerSize * 2f;

		private static readonly Color TowerBuildCan = Color.ForestGreen;
		private static readonly Color TowerBuildObsticle = Color.Red;
		private static readonly Color TowerColor = Color.DarkOrange;

		public Map Map;

		// Quad tree componenets
		public int Key { get; set; }
		public Position Position { get; set; }

		// Game-play
		public double HealthMax;
		public double HealthCurrent;
		public int Cost;
		public bool IsDropped;
		public bool CanBeDropped;

		public double Range;
		public double SafeRange;
		public double Damage;
		public double FireRateDelay;
		public double FireReadyNext;

		// Draw
		public Vector3 AbsoutePosition;
		public Color Color;
		private IDrawable drawable;


		public StaticTower(Map map) {
			this.Map = map;

			this.Range = 10;
			this.SafeRange = 4;
			this.Damage = 20;
			this.FireRateDelay = 100;

			this.Rebuild();
		}


		public void Update(Time time) {
			if (this.FireReadyNext > 0)
				this.FireReadyNext -= time.Delta * 1000;
			else {
				this.FireReadyNext = 0;
				var enemiesInRange = this.Map.enemiesTree.GetAllComponents();
				double minDistance = Double.MaxValue;
				Enemy closestEnemy = null;
				foreach (var enemy in enemiesInRange) {
					var distance = Position.Distance((Position) enemy.Position, this.Position);
					if (distance <= this.Range && distance < minDistance) {
						minDistance = distance;
						closestEnemy = enemy;
					}
				}
				if (closestEnemy != null) {
					if (minDistance <= this.SafeRange)
						closestEnemy.DoDamage(this.Damage);
					else closestEnemy.DoDamage(this.Damage * (1 - (minDistance / this.Range)));
					this.FireReadyNext = this.FireRateDelay;
				}
			}
		}

		public void Rebuild() {
			// Tower colors
			if (this.IsDropped)
				this.Color = TowerColor;
			else if (this.CanBeDropped)
				this.Color = TowerBuildCan;
			else this.Color = TowerBuildObsticle;

			// Absolute position
			this.AbsoutePosition = (Vector3) this.Position * TowerSizeD +
				new Vector3(TowerSize, TowerSize, 0);

			// Draw primitive
			this.drawable = new Ellipse(
				this.AbsoutePosition, TowerSize, TowerSize, this.Color);
		}

		public void Draw(GraphicsDrawer drawer, Time time) {
			if (this.drawable == null) this.Rebuild();

#if DEBUG
			// Draw tower range
			drawer.Draw(
				new EllipseOutline(this.AbsoutePosition, (float)(this.Range * Map.MapGridSize),
					(float)(this.Range * Map.MapGridSize), Color.Red));
			
			// Draw tower safe range
			drawer.Draw(
				new EllipseOutline(this.AbsoutePosition, (float)(this.SafeRange * Map.MapGridSize),
					(float)(this.SafeRange * Map.MapGridSize), Color.Blue));
#endif

			drawer.Draw(this.drawable);
		}
	}
}
