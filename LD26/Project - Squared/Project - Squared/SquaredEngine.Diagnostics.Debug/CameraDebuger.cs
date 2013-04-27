using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquaredEngine.Common;
using SquaredEngine.Common.Extensions;
using SquaredEngine.Graphics;
using SquaredEngine.Diagnostics.Log;
using Rectangle = SquaredEngine.Graphics.Rectangle;

namespace SquaredEngine.Diagnostics.Debug {
	public class CameraDebuger : DrawableGameComponent {
		private Range boardRange;
		private readonly Camera2D camera;

		// TODO: Kreiraj svojstvo sirine komponente
		// TODO: Omoguciti promjenu sirine komponente
		public const int ComponentWidth = 250;
		private GraphicsDrawer gd;

		public String ID { get; private set; }
		private LoggerInstance log;

		private Vector2 position;
		private Vector2 cameraPositionComponent;
		private Vector2 componentBoardRatio = Vector2.One;

		private Rectangle backgroundQuad;
		private Rectangle cameraViewRectangle;

		private Color backgroundColor;
		private Color cameraColor;

		public Vector2 Position {
			get { return this.position; }
			set {
				this.position = value;
				GenerateContent();
			}
		}


		public CameraDebuger(Game game, Camera2D camera, Range boardRange)
			: base(game) {
			this.camera = camera;
			this.boardRange = boardRange;

			ID = Guid.NewGuid().ToString();

			this.log = new LoggerInstance(typeof(CameraDebuger), ID);
		}


		protected override void LoadContent() {
			this.log.WriteInformation("Loading content...");

			GraphicsDevice.DeviceReset += (s, e) => LoadContent();

			camera.OnMoved += (cameraSender, cameraEvents) => RecalculateCameraPosition(cameraEvents.Position);

			gd = new GraphicsDrawer(GraphicsDevice) {
				DebugFont = Game.Content.Load<SpriteFont>("Fonts/DebugFont")
			};

			GenerateContent();

			camera.MoveTo(Vector3.One);

			base.LoadContent();

			this.log.WriteInformation("Content loaded!");
		}

		private void RecalculateCameraPosition(Vector2 cameraPosition) {
			cameraPositionComponent = position + cameraPosition * componentBoardRatio;
			cameraViewRectangle = new Rectangle(
				cameraPositionComponent.ToVector3(), 
				(cameraPositionComponent + camera.ViewSize * componentBoardRatio).ToVector3(), 
				cameraColor);
		}

		private void GenerateContent() {
			this.log.WriteInformation("Generating content...");

			// Kreiranje boje pozadine
			backgroundColor = Color.Black;
			backgroundColor.A = 150;
			cameraColor = backgroundColor;

			// Generiranje teksure pogleda na mapu
			float ratio = (float)boardRange.Height / (float)boardRange.Width;
			backgroundQuad = new Rectangle(Vector3.Zero + position.ToVector3(),
															   new Vector3(ComponentWidth, ComponentWidth * ratio, 0f) + position.ToVector3(),
															   backgroundColor);

			// Generiranje teksture pogleda kamere
			componentBoardRatio = new Vector2(
			   ComponentWidth / (float)boardRange.Height,
			   (ComponentWidth * ratio) / boardRange.Width);

			RecalculateCameraPosition(this.camera.Position);


			this.log.WriteInformation("Content generated!");
		}

		public override void Draw(GameTime gameTime) {
			if (Enabled) {
				gd.Begin();
				
				gd.Draw(backgroundQuad);
				gd.Write(boardRange.ToString(), Position);

				gd.Draw(cameraViewRectangle);
				gd.Write(camera.GetDebug(), cameraPositionComponent);
				
				gd.End();

				base.Draw(gameTime);
			}
		}
	}
}