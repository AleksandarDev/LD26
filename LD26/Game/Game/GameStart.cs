using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
		// Bug close destination

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
		public Texture2D machineGunLogo;
		public Texture2D laserLogo;
		public Texture2D shotGunLogo;
		public Texture2D canonLogo;

		public IDrawable minimapBackground;
		public Vector3 minimapPosition;

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
#if DEBUG
			this.timeDebuger = new TimeDebuger(this);
			this.Components.Add(this.timeDebuger);
#endif

			this.keyboard = new KeyboardController(this);
			this.Components.Add(this.keyboard);

			this.timeController = new TimeController(this, true);
			this.Components.Add(this.timeController);

			// TODO Limit camera view
			this.camera = new Camera2D(this);
			this.camera.Zoom = 0.5f;
			this.Components.Add(this.camera);

			this.map = new Map(this);
			this.Components.Add(this.map);
#if DEBUG
			this.cameraDebugger = new CameraDebuger(
				this, this.camera, this.map.mapRange * Map.MapGridSize);
			this.Components.Add(this.cameraDebugger);
#endif
			base.Initialize();
		}

		protected override void LoadContent() {
			// Create a new SpriteBatch, which can be used to draw textures.
			this.drawer = new GraphicsDrawer(this.GraphicsDevice);
			this.drawer.Initialize();
			this.drawer.DebugFont = this.Content.Load<SpriteFont>("Fonts/DebugFont");

#if DEBUG
			this.timeDebuger.Position = new Vector3(0f,
				GraphicsDevice.Viewport.Height - TimeDebuger.GraphHeight, 0f);
#endif
			// TODO Implement player name input
			this.player = new Player() {
				Name = "AleksandarDev",
				Resources = StartResources
			};

			LaserTower.LaserBullet.effect =
				this.Content.Load<SoundEffect>("SF/LaserBullet");
			Bullet.effect =
				this.Content.Load<SoundEffect>("SF/Bullet");

			this.machineGunLogo = this.Content.Load<Texture2D>("Textures/MachineGun");
			this.laserLogo = this.Content.Load<Texture2D>("Textures/Laser");
			this.shotGunLogo = this.Content.Load<Texture2D>("Textures/ShotGun");
			this.canonLogo = this.Content.Load<Texture2D>("Textures/Canon");

			this.minimapPosition = new Vector3(
				this.GraphicsDevice.Viewport.Width - 256,
				this.GraphicsDevice.Viewport.Height - 256, 0);
			this.minimapBackground = new Rectangle(
				this.minimapPosition, 256, new Color(0, 0, 0, 128));
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
			float prevZoom = this.camera.Zoom;
			
			// Set zoom
			if (this.keyboard.IsClicked(Keys.Add) || scrollDelta > 0)
				this.camera.Zoom = Math.Min(zoomMax, this.camera.Zoom + 0.1f);
			if (this.keyboard.IsClicked(Keys.Subtract) || scrollDelta < 0)
				this.camera.Zoom = Math.Max(zoomMin, this.camera.Zoom - 0.1f);

			// Correct origin of zoom
			this.camera.Position = new Vector2(
									this.camera.Position.X -
									this.camera.ViewSize.X / this.camera.Zoom *
									(1 - this.camera.Zoom / prevZoom) * (this.currentMouseState.X / (this.GraphicsDevice.Viewport.Width / 2f)),
									this.camera.Position.Y -
									this.camera.ViewSize.Y / this.camera.Zoom *
									(1 - this.camera.Zoom / prevZoom) * (this.currentMouseState.Y / (this.GraphicsDevice.Viewport.Height / 2f)));

			if (this.currentMouseState.LeftButton == ButtonState.Pressed &&
				this.previousMouseState.LeftButton == ButtonState.Pressed) {
				this.camera.Move(-new Vector2(this.currentMouseState.X - this.previousMouseState.X, this.currentMouseState.Y - this.previousMouseState.Y) / this.camera.Zoom);
			}

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
					if (this.menuSelectedItemIndex == 0)
						this.buildTower = new MachineGunTower(this.map);
					else if (this.menuSelectedItemIndex == 1)
						this.buildTower = new ShotGunTower(this.map);
					else if (this.menuSelectedItemIndex == 5)
						this.buildTower = new LaserTower(this.map);
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
			this.GraphicsDevice.Clear(Color.BurlyWood);

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

			drawer.Draw(this.minimapBackground);
			foreach (var enemy in this.map.enemiesTree.GetAllComponents()) {
				drawer.Draw(new Rectangle(enemy.Position * 4 + this.minimapPosition, 4,
					enemy.Color));
			}

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

				GraphicsDrawer.RotateAboutOrigin(currentOrigin, this.menuAngle * index,
					ref points);

				this.drawer.Draw(new Quad(
					points[0], points[1], points[2], points[3],
					this.menuSelectedItemIndex == index
						? Color.Red
						: new Color(0 + index * 30, 255 - index * 20, 200 - index * 50)));
				currentOrigin = points[3];

				// Draw canon logo
				if (index == 0)
					drawer.Draw(this.machineGunLogo,
						points[1].ToVector2() +
						new Vector2((this.menuTopSideSize - this.menuBottomSideSize) / 3.5f, 
							this.menuItemHeight / 12), Color.White);
				else if (index == 1)
					drawer.Draw(this.shotGunLogo,
						points[1].ToVector2() +
						new Vector2(-(this.menuTopSideSize - this.menuBottomSideSize) / 5,
							this.menuItemHeight / 2), Color.White);
				else if (index == 2)
					drawer.Draw(this.canonLogo,
						points[3].ToVector2() +
						new Vector2(-(this.menuTopSideSize - this.menuBottomSideSize) / 6,
							this.menuItemHeight / 6), Color.White);
				else if (index == 4)
					drawer.Draw(this.canonLogo,
						points[3].ToVector2() +
						new Vector2(-(this.menuTopSideSize - this.menuBottomSideSize) / 6,
							this.menuItemHeight / 6), Color.White);
				else if (index == 5)
					drawer.Draw(this.laserLogo,
						points[2].ToVector2() +
						new Vector2((this.menuTopSideSize - this.menuBottomSideSize) / 3,
							-this.menuItemHeight / 4), Color.White);
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
