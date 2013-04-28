using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SquaredEngine.Common;
using SquaredEngine.Common.Extensions;
using SquaredEngine.Diagnostics.Debug;
using SquaredEngine.Graphics;
using SquaredEngine.Input;
using SquaredEngine.Utils.Trees.QuadTree;
using IDrawable = SquaredEngine.Graphics.IDrawable;
using Rectangle = SquaredEngine.Graphics.Rectangle;

namespace Game {
	public class GameStart : Microsoft.Xna.Framework.Game {
		// TODO Minimalism
		// TODO Tower defense?

		public GraphicsDeviceManager graphics;
		public GraphicsDrawer drawer;
		public TimeDebuger timeDebuger;
		public KeyboardController keyboard;
		public MouseState previousMouseState;
		public MouseState currentMouseState;
		public TimeController timeController;

		public Camera2D camera;
		public CameraDebuger cameraDebugger;
		public const float zoomMax = 1f;
		public const float zoomMin = 0.2f;

		public bool isMenuActive;
		public Vector3 menuPosition;
		public int menuSelectedItemIndex;
		public float menuBottomSideSize = 25f;
		public float menuTopSideSize = 67.5f;
		public float menuItemHeight = 50;
		public Vector3 menuOffset = new Vector3(-12.5f, -31, 0);
		public float menuAngle = MathHelper.ToRadians(45f);
		public int menuItems = 8;

		public Map map;
		public Position pointerPosition;

		public Player player;
		public const int StartResources = 2000;

		public StaticTower buildTower;

		public GameStart() {
			this.Window.Title = "LD26 by AleksandarDev";

			// Set resolution
			this.graphics = new GraphicsDeviceManager(this);
			this.graphics.PreferredBackBufferWidth = 1280;
			this.graphics.PreferredBackBufferHeight = 720;
			this.graphics.IsFullScreen = false;

			// TODO Remove if mouse not used or masked
			this.IsMouseVisible = true;

			this.Content.RootDirectory = "Content";
		}

		protected override void Initialize() {
			this.timeDebuger = new TimeDebuger(this);
			this.Components.Add(this.timeDebuger);

			this.keyboard = new KeyboardController(this);
			this.Components.Add(this.keyboard);

			this.timeController = new TimeController(this, true);
			this.Components.Add(this.timeController);

			// TODO Limit camera view
			this.camera = new Camera2D(this);
			this.Components.Add(this.camera);

			this.map = new Map(this);
			this.Components.Add(this.map);

			this.cameraDebugger = new CameraDebuger(
				this, this.camera, this.map.mapRange * Map.MapGridSize);
			this.Components.Add(this.cameraDebugger);

			base.Initialize();
		}

		protected override void LoadContent() {
			// Create a new SpriteBatch, which can be used to draw textures.
			this.drawer = new GraphicsDrawer(this.GraphicsDevice);
			this.drawer.Initialize();
			this.drawer.DebugFont = this.Content.Load<SpriteFont>("Fonts/DebugFont");

			this.timeDebuger.Position = new Vector3(0f,
				GraphicsDevice.Viewport.Height - TimeDebuger.GraphHeight, 0f);

			// TODO Implement player name input
			this.player = new Player() {
				Name = "AleksandarDev",
				Resources = StartResources
			};
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);

			this.previousMouseState = this.currentMouseState;
			this.currentMouseState = Mouse.GetState();

			this.pointerPosition = new Position(
				(int)
					((this.currentMouseState.X / this.camera.Zoom + this.camera.Position.X) /
						Map.MapGridSize),
				(int)
					((this.currentMouseState.Y / this.camera.Zoom + this.camera.Position.Y) /
						Map.MapGridSize));

			var scrollDelta = this.currentMouseState.ScrollWheelValue -
				this.previousMouseState.ScrollWheelValue;
			if (this.keyboard.IsClicked(Keys.Add) || scrollDelta > 0)
				this.camera.Zoom = Math.Min(zoomMax, this.camera.Zoom + 0.1f);
			if (this.keyboard.IsClicked(Keys.Subtract) || scrollDelta < 0)
				this.camera.Zoom = Math.Max(zoomMin, this.camera.Zoom - 0.1f);

			// On mouse click
			if (this.currentMouseState.LeftButton == ButtonState.Pressed &&
				this.previousMouseState.LeftButton == ButtonState.Released) {

				// Check if build tower needs to be dropped
				if (this.buildTower != null && !this.buildTower.IsDropped &&
					this.buildTower.CanBeDropped) {
					this.buildTower.IsDropped = true;
					this.buildTower.Rebuild();
					this.map.towersTree.AddComponent(this.buildTower);
					this.player.Resources -= this.buildTower.Cost;
					this.map.checkForCollision.Add(this.buildTower.Position);
					this.buildTower = null;
				}
			}

			// On right mouse click
			if (this.currentMouseState.RightButton == ButtonState.Pressed &&
				this.previousMouseState.RightButton == ButtonState.Released) {

				// Activate menu
				if (!this.isMenuActive) {
					this.buildTower = null;
					this.isMenuActive = true;
					this.menuPosition = new Vector3(
						this.currentMouseState.X,
						this.currentMouseState.Y, 0);
				}
			}
			else if (this.currentMouseState.RightButton == ButtonState.Released &&
				this.previousMouseState.RightButton == ButtonState.Pressed) {
				if (this.isMenuActive) {
					this.isMenuActive = false;
					// Build new tower
					if (this.menuSelectedItemIndex == 0) {
						this.buildTower = new StaticTower(this.map);
					}
					// TODO Check released on menu
				}
			}

			// Move build tower around
			if (this.buildTower != null && !this.buildTower.IsDropped) {
				this.buildTower.Position = Position.Clamp(this.pointerPosition,
					Position.Zero, this.map.mapRange.LowerRight - 1);

				// Check collision
				this.buildTower.CanBeDropped =
					!this.map.towersTree.GetComponents(this.buildTower.Position).Any();

				this.buildTower.Rebuild();
			}
		}

		protected override void Draw(GameTime gameTime) {
			this.GraphicsDevice.Clear(Color.Black);

			// Draw camera dependent elements
			this.drawer.Begin(transformMatrix: this.camera.TransformMatrix);

			// Draw game board
			this.drawer.Draw(new Rectangle(Vector3.Zero,
				new Vector3(this.map.mapRange.Width * Map.MapGridSize,
					this.map.mapRange.Height * Map.MapGridSize, 0),
				new Color(96, 196, 116)));

			this.drawer.End();

			base.Draw(gameTime);

			// Draw camera dependent elements
			this.drawer.Begin(transformMatrix: this.camera.TransformMatrix);

			// Draw build tower
			if (this.buildTower != null)
				this.buildTower.Draw(this.drawer, this.timeController.Time);



#if DEBUG
			//DrawChildren(drawer, this.map.towersTree.RootNode);
			DrawChildren(drawer, this.map.enemiesTree.RootNode);
#endif
			this.drawer.End();

			// Draw camera independent elements
			this.drawer.Begin();

			this.drawer.Write(this.player.Name, new Vector2(20, 20));
			this.drawer.Write(this.player.Resources.ToString(), new Vector2(20, 40));

			if (this.isMenuActive)
				this.DrawMenu(drawer, this.timeController.Time);

			this.drawer.End();
		}

		private void DrawMenu(GraphicsDrawer drawer, Time time) {
			Vector3 currentOrigin = this.menuPosition + this.menuOffset;
			for (int index = 0; index < this.menuItems; index++) {
				Vector3[] points = new Vector3[4];
				points[0] = currentOrigin;
				points[1] = points[0] +
					new Vector3(-(this.menuTopSideSize - this.menuBottomSideSize) / 2f,
						-this.menuItemHeight, 0);
				points[2] = points[1] + new Vector3(this.menuTopSideSize, 0, 0);
				points[3] = points[0] + new Vector3(this.menuBottomSideSize, 0, 0);

				GraphicsDrawer.RotateAboutOrigin(currentOrigin, this.menuAngle * index, ref points);

				this.drawer.Draw(new Quad(
					points[0], points[1], points[2], points[3],
					this.menuSelectedItemIndex == index
						? Color.Red
						: new Color(0 + index * 30, 255 - index * 20, 200 - index * 50)));
				currentOrigin = points[3];
			}


#if DEBUG
			this.drawer.Draw(new Ellipse(this.menuPosition, 10, 10, Color.Red));
			this.drawer.Draw(new GraphicsDrawer.Line(this.menuPosition, new Vector3(
				this.currentMouseState.X, this.currentMouseState.Y, 0), Color.Red));
#endif

			// Get Selected item
			var angleSelection = (float) Math.Atan2(
				this.currentMouseState.X - this.menuPosition.X,
				this.currentMouseState.Y - this.menuPosition.Y) - (float) Math.PI;
			this.menuSelectedItemIndex =
				Math.Abs((int) Math.Round(MathHelper.ToDegrees(angleSelection) / 45f));
			if (this.menuSelectedItemIndex > 7)
				this.menuSelectedItemIndex = 0;
		}

		private static void DrawChildren<K>(GraphicsDrawer gd, INode<K> node)
			where K : IComponent {
			int mult = 50;

			if (!node.HasChildren) {
				if (!node.HasComponents) {
					gd.Draw(new Rectangle(node.Range.UpperLeft * mult, node.Range.Width * mult,
						new Color(255, 0, 0, 2)));
				}
				gd.Draw(new RectangleOutline(node.Range.UpperLeft * mult,
					node.Range.Width * mult,
					node.HasComponents ? Color.Blue : Color.Red));
			}
			foreach (var child in node.Children) {
				DrawChildren<K>(gd, child);
			}
		}
	}
}
