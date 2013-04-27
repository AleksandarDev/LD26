using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SquaredEngine.Input;
using SquaredEngine.Diagnostics.Log;


namespace SquaredEngine.Graphics {

	public class Camera2D : GameComponent {
		// TODO: Dodati ogranicenje kretanja (samo unutar Rectangle polja)
		// TODO: Rijesiti problem view frustuma

		public event Camera2DEventHandler OnMoved;
		public event Camera2DEventHandler OnMoveRequested;
		public event Camera2DEventHandler OnMoveStarted;
		public event Camera2DEventHandler OnMoveEnded;

		private KeyboardController keyboardController;
		private Vector3 position = Vector3.One;
		private float speed = 1000f;
		private int speedMult = 1;
		private Matrix transformMatrix;
		private float zoom = 1f;

		private bool isMoveRequested;
		private bool isMoveRequestedPreviously;
		private Vector3 moveLocation;

		public String ID { get; private set; }
		private LoggerInstance log;

		public GraphicsDevice GraphicsDevice { get; private set; }

		public float Zoom {
			get { return zoom; }
			set { if (zoom >= 0) zoom = value; }
		}

		public Vector2 Position {
			get { return new Vector2(this.position.X, this.position.Y); }
			set {
				this.position.X = value.X;
				this.position.Y = value.Y;
				RecalculateParameters();
			}
		}

		public Vector3 Position3 {
			get { return position; }
			set {
				this.position = value;
				RecalculateParameters();
			}
		}

		public Matrix TransformMatrix {
			get { return transformMatrix; }
		}

		public Vector2 ViewSize { get; protected set; }

		public bool CustomControls { get; set; }

		public float Speed {
			get { return speed; }
			set { speed = value; }
		}


		public Camera2D(Game game)
			: base(game) {
			ID = Guid.NewGuid().ToString();
			this.log = new LoggerInstance(typeof(Camera2D), ID);

			if (game == null) {
				this.log.WriteFatalError("game is null!");
				throw new ArgumentNullException("game");
			}

			GraphicsDevice = game.GraphicsDevice;
			CustomControls = false;

			RecalculateParameters();
		}


		public override void Initialize() {
			this.log.WriteInformation("Initializing...");

			GraphicsDevice.DeviceReset += (o, ea) => Initialize();

			ViewSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) * Zoom;

			keyboardController = new KeyboardController(Game);
			keyboardController.Initialize();

			base.Initialize();

			this.log.WriteInformation("Initialized!");
		}

		public override void Update(GameTime gameTime) {
			if (!CustomControls) {
				keyboardController.Update(gameTime);

				var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

				speedMult = keyboardController.IsHeld(Keys.LeftShift) ? 10 : 1;

				Vector3 direction = Vector3.Zero;

				if (keyboardController.Keyboard.IsKeyDown(Keys.Right)) {
					direction += Vector3.Right;
				}
				if (keyboardController.Keyboard.IsKeyDown(Keys.Left)) {
					direction += Vector3.Left;
				}
				if (keyboardController.Keyboard.IsKeyDown(Keys.Up)) {
					direction -= Vector3.Up;
				}
				if (keyboardController.Keyboard.IsKeyDown(Keys.Down)) {
					direction -= Vector3.Down;
				}

				if (direction != Vector3.Zero) {
					direction.Normalize();
					Move(direction * speed * deltaTime * speedMult);
				}
			}

			if (!this.isMoveRequestedPreviously && this.isMoveRequested) {
				if (OnMoveStarted != null) {
					OnMoveStarted(this, GetEventArgs());
				}
			}

			if (this.isMoveRequestedPreviously && !this.isMoveRequested) {
				if (OnMoveEnded != null) {
					OnMoveEnded(this, GetEventArgs());
				}
			}

			this.isMoveRequestedPreviously = this.isMoveRequested;

			if (this.isMoveRequested) {
				MakeMove(this.moveLocation);
				this.isMoveRequested = false;
			}

			base.Update(gameTime);
		}


		public void Move(Vector2 amount) {
			Move(amount.X, amount.Y);
		}
		public void Move(Vector3 amount) {
			Move(amount.X, amount.Y);
		}
		public void Move(float x, float y) {
			MoveTo(this.position.X + x, this.position.Y + y, this.position.Z);
		}
		public void MoveTo(float x, float y, float z) {
			MoveTo(new Vector3(x, y, z));
		}
		public void MoveTo(Vector2 newPosition) {
			MoveTo(newPosition.X, newPosition.Y, 0f);
		}
		public void MoveTo(Vector3 newPosition) {
			this.moveLocation = newPosition;
			this.isMoveRequested = true;

			if (OnMoveRequested != null) {
				OnMoveRequested(this, GetEventArgs());
			}
		}

		protected virtual void MakeMove(Vector3 location) {
			this.position = location;

			RecalculateParameters();

			if (OnMoved != null) {
				OnMoved(this, GetEventArgs());
			}
		}

		private Camera2DEventArgs GetEventArgs() {
			return new Camera2DEventArgs() {
				Position = Position,
				Position3 = Position3,
				TransformMatrix = TransformMatrix
			};
		}

		private void RecalculateParameters() {
			transformMatrix = CalculateMatrix();
		}

		private Matrix CalculateMatrix() {
			Matrix tr = Matrix.Identity *
			            Matrix.CreateTranslation(-Position3) *
			            Matrix.CreateScale(Zoom);
			return tr;
		}

		public string GetDebug() {
			return String.Format("Camera2D: {0}\nat {1}", ViewSize, Position3);
		}
	}
}