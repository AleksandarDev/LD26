using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SquaredEngine.Common;
using SquaredEngine.Diagnostics.Debug;
using SquaredEngine.GameBoard.Components;
using SquaredEngine.Graphics;
using SquaredEngine.Input;
using SquaredEngine.Utils.Trees.QuadTree;
using SquaredEngine.Diagnostics.Log;
using IDrawable = SquaredEngine.Graphics.IDrawable;


namespace Squared {
	public class GameStart : Game {
		private readonly GraphicsDeviceManager graphics;

		public String ID { get; private set; }
		private LoggerInstance log;

		private Camera2D camera;
		private CameraDebuger cameraDebuger;
		private GraphicsDrawer graphicsDrawer;

		private MouseState prevState;

		private KeyboardController keyboardController;
		private GamePadController gamePadController;
		private int previousZoom;
		private TimeController time;
		private TimeDebuger timeDebuger;

		public GameStart() {
			ID = Guid.NewGuid().ToString();
			this.log = new LoggerInstance(typeof(GameStart), ID);

			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Data";

			IsFixedTimeStep = false;
			graphics.SynchronizeWithVerticalRetrace = false;

			graphics.PreferredBackBufferHeight = 768;
			graphics.PreferredBackBufferWidth = 1024;
			graphics.ApplyChanges();
		}

		protected override void Initialize() {
			this.log.WriteInformation("Initializing...");

			GraphicsDevice.DeviceReset += (s, e) => LoadContent();

			IsMouseVisible = true;

			time = new TimeController(this);
			timeDebuger = new TimeDebuger(this);
			
			camera = new Camera2D(this);
			camera.OnMoveStarted += (s, e) => this.log.WriteDebugInformation("Camera move started...");
			camera.OnMoveEnded += (s, e) => this.log.WriteDebugInformation("Camera move ended!");

			cameraDebuger = new CameraDebuger(this, camera, new Range(5000, 5000));

			keyboardController = new KeyboardController(this);
			gamePadController = new GamePadController(this);

			// ------------ TEST --------------
			GenerateCurve();
			// ------------ TEST --------------

			RegisterComponents();

			base.Initialize();


			this.log.WriteInformation("Initialized...");
		}

		private void RegisterComponents() {
			Components.Add(time);
			Components.Add(timeDebuger);
			Components.Add(camera);
			Components.Add(cameraDebuger);
			Components.Add(keyboardController);
			Components.Add(gamePadController);
		}

		protected override void LoadContent() {
			graphicsDrawer = new GraphicsDrawer(GraphicsDevice) {
				DebugFont = Content.Load<SpriteFont>("Fonts/DebugFont")
			};
			
			timeDebuger.Position = new Vector3(0f, GraphicsDevice.Viewport.Height - TimeDebuger.GraphHeight, 0f);
			cameraDebuger.Position = new Vector2(GraphicsDevice.Viewport.Width - CameraDebuger.ComponentWidth, 0f);
		}

		protected override void Update(GameTime gameTime) {
			// TODO: Dodati InputController za mis
			// TODO: Objediniti kontrolere za mis i tipkovnicu
			// NOTE: Kasnije bi trebalo dodati InputManager.Touch za WP7 platformu

			if (keyboardController.Keyboard.IsKeyDown(Keys.Escape)) {
				Exit();
			}

			MouseState state = Mouse.GetState();

			int delta = previousZoom - state.ScrollWheelValue;
			previousZoom = state.ScrollWheelValue;

			prevState = state;

#if DEBUG
			if (keyboardController.IsClicked(Keys.F11)) {
				if (graphics.IsFullScreen) {
					graphics.PreferredBackBufferHeight = 768;
					graphics.PreferredBackBufferWidth = 1024;
				}
				else {
					graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
					graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
				}
				graphics.IsFullScreen = !graphics.IsFullScreen;
				graphics.ApplyChanges();
			}

			if (keyboardController.IsHeld(Keys.F12)) {
				if (keyboardController.IsClicked(Keys.C)) {
					cameraDebuger.Enabled = !cameraDebuger.Enabled;
				}
				if (keyboardController.IsClicked(Keys.T)) {
					timeDebuger.Enabled = !timeDebuger.Enabled;
				}
			}
#endif

			base.Update(gameTime);

#if DEBUG
			//Console.WriteLine(gamePadController.ToString(true));
#endif
		}

		List<Vector3> points = new List<Vector3>() {
			new Vector3(50, 50, 0),
			new Vector3(50, 50, 0),
			new Vector3(70, 70, 0),
			new Vector3(120, 120, 0),
			new Vector3(300, 25, 0),
			new Vector3(500, 125, 0),
			new Vector3(500, 125, 0)
		};
		private Vector3 GetNextPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float amt) {
			return new Vector3(
				MathHelper.CatmullRom(p0.X, p1.X, p2.X, p3.X, amt),
				MathHelper.CatmullRom(p0.Y, p1.Y, p2.Y, p3.Y, amt),
				MathHelper.CatmullRom(p0.Z, p1.Z, p2.Z, p3.Z, amt));
		}
		List<Vector3> curvedPoints = new List<Vector3>();
		private void GenerateCurve() {
			//curvedPoints.AddRange(points);
			for (int m = 0; m < points.Count - 3; m++) {
				
				Vector3 a = points[m + 1];
				Vector3 b = points[m + 2];
				float amount;
				Vector3.Distance(ref a, ref b, out amount);
				amount = 1 / (amount / 10) / 1.5f;

				Console.WriteLine(amount);

				for (float i = 0; i < 1; i += amount) {
					curvedPoints.Add(GetNextPoint(points[m], points[m + 1], points[m + 2], points[m + 3], i));
				}
			}
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Dodati GetDebug za svaku komponentu
			graphicsDrawer.Begin();

			Vector2 position;
			String text;

			position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
			text = (new Vector2(Mouse.GetState().X, Mouse.GetState().Y)).ToString();
			graphicsDrawer.Write(text, position);

			graphicsDrawer.End();

			graphicsDrawer.Begin();
			foreach (var point in curvedPoints) {
				graphicsDrawer.Draw(new Ellipse(point, 3f, 3f, Color.Red));
			}
			for (int i = 0; i < curvedPoints.Count - 1; i++) {
				graphicsDrawer.Draw(new GraphicsDrawer.Line(curvedPoints[i], curvedPoints[i + 1], Color.Red));
			}
			foreach (var point in points) {
				graphicsDrawer.Draw(new Ellipse(point, 2f, 2f, Color.Blue));
			}
			for (int i = 0; i < points.Count - 1; i++) {
				graphicsDrawer.Draw(new GraphicsDrawer.Line(points[i], points[i + 1], Color.Blue));
			}
			//x DrawChildren(graphicsDrawer, groundBoard.RootNode);
			graphicsDrawer.End();

			graphicsDrawer.Begin();
			graphicsDrawer.Write(gamePadController.ToString(true), new Vector2(20));
			graphicsDrawer.End();

			base.Draw(gameTime);
		}

		private static void DrawChildren<K>(GraphicsDrawer gd, INode<K> node)
			where K : IComponent {
			int mult = 32;
			IDrawable nodeQuad = new RectangleOutline(node.Range.UpperLeft * mult, node.Range.Width * mult, node.HasComponents ? Color.Blue : Color.Red);
			gd.Draw(nodeQuad);

			foreach (var child in node.Children) {
				DrawChildren<K>(gd, child);
			}
		}
	}
}