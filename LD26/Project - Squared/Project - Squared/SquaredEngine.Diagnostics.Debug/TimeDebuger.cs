using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SquaredEngine.Common;
using SquaredEngine.Common.Extensions;
using SquaredEngine.Graphics;
using SquaredEngine.Diagnostics.Log;
using IDrawable = SquaredEngine.Graphics.IDrawable;
using Rectangle = SquaredEngine.Graphics.Rectangle;

namespace SquaredEngine.Diagnostics.Debug {
	public class TimeDebuger : DrawableGameComponent {
		public const Int32 GraphWidth = 320;
		public const Int32 GraphHeight = 180;
		private const float ColorChange = GraphWidth / 80000f;

		public String ID { get; private set; }
		private LoggerInstance log;

		private GraphicsDrawer gd;

		private Rectangle backgroundQuad;
		private TimeLine[] graphLines;

		private float millisecondsPassed;
		private int currentFrame;
		private int drawsCount;
		private int fps;

		private Vector3 position;
		private Vector2 position2;

		public Vector3 Position {
			get { return position; }
			set { ChangePosition(value); }
		}

		public int FPS {
			get { return fps; }
		}


		public TimeDebuger(Game game)
			: base(game) {
				ID = Guid.NewGuid().ToString();
				this.log = new LoggerInstance(typeof(TimeDebuger), ID);
		}


		protected override void LoadContent() {
			this.log.WriteInformation("Loading content...");

			GraphicsDevice.DeviceReset += (s, e) => LoadContent();

			gd = new GraphicsDrawer(GraphicsDevice) {
				DebugFont = Game.Content.Load<SpriteFont>("Fonts/DebugFont")
			};

			GenerateContent();

			base.LoadContent();

			this.log.WriteInformation("Content loaded!");
		}

		private void GenerateContent() {
			this.log.WriteInformation("Generating content...");

			// Kreira pozadinu za graf
			Color blackTrans = Color.Black;
			blackTrans.A = 150;
			
			Range backgroundRange = new Range(GraphWidth, GraphHeight);
			backgroundQuad = new Rectangle(position + backgroundRange.UpperLeft,
			                                                   position + backgroundRange.LowerRight, blackTrans);

			// Ucitava linije
			if (graphLines == null) graphLines = new TimeLine[GraphWidth];
			for (int index = 0; index < GraphWidth; index++) {
				if (graphLines[index] == null) {
					graphLines[index] = new TimeLine {
						PointA = new Vector3(position.X + index, position.Y + GraphHeight, position.Z),
						PointB = new Vector3(position.X + index, position.Y + GraphHeight, position.Z),
						ColorA = Color.DarkGreen,
						ColorB = Color.Green,
					};
				}
				else {
					graphLines[index].PointA = new Vector3(position.X + index, position.Y + GraphHeight, position.Z);
					graphLines[index].PointB = new Vector3(graphLines[index].PointA.X,
					                                       graphLines[index].PointA.Y - graphLines[index].Height, position.Z);
				}
			}
			this.log.WriteInformation("{0} TimeLines generated!", this.graphLines.Length);

			this.log.WriteInformation("Content generated!");
		}

		private void ChangePosition(Vector3 newPosition) {
			this.log.WriteInformation("Position changed from {0} to {1}!", this.position, newPosition);

			this.position = newPosition;
			this.position2 = this.position.ToVector2();

			GenerateContent();
		}

		public override void Update(GameTime gameTime) {
			if (Enabled) {
				// Zatamni sve linije grafa kako bi dobili efekt prolaza
				for (int index = 0; index < GraphWidth; ++index) {
					graphLines[index].DarkenColor(ColorChange);
				}

				// Izracunava FPS tako da trenutnom zbroju vremena doda
				// vrijeme od prijasnjeg osvjezavanja te ukoliko je prosla 1
				// sekunda onda je trenutni broj iscrtanih slika iznos FPSa
				millisecondsPassed += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
				if (millisecondsPassed >= 1000) {
					millisecondsPassed -= 1000;
					fps = drawsCount;
					drawsCount = 0;
				}

				base.Update(gameTime);
			}
		}

		public override void Draw(GameTime gameTime) {
			if (Enabled) {
				// Mijenja visinu linije grafa ovisno o vremenu potrebnom za iscrtavanja
				graphLines[currentFrame++].ChangeLineHeight((float) gameTime.ElapsedGameTime.TotalMilliseconds);

				// Brine se da trenutna linija nije veca od sirine grafa
				if (currentFrame >= GraphWidth) {
					currentFrame = 0;
				}

				// Uvezava broj iscrtavanja za jedan (1)
				drawsCount++;

				gd.Begin();

				// Iscrtava pozadinu grafa
				gd.Draw(backgroundQuad);

				// Iscrtava sve linije grafa);
				for (int index = 0; index < GraphWidth; index++) {
					gd.Draw(graphLines[index]);
				}

				// Ispisuje informacije o grafu (FPS i naziv komponente, GetDebug())
				gd.Write(GetDebug(), position2);

				gd.End();


				base.Draw(gameTime);
			}
		}

		public string GetDebug() {
			return String.Format("TimeDebuger: {0} FPS", FPS);
		}

		private class TimeLine : IPrimitive, IDrawable {
			private readonly VertexPositionColor[] vertices;
			private float height;
			
			public Color ColorA {
				get { return this.vertices[0].Color; }
				set { this.vertices[0].Color = value; }
			}
			public Color ColorB {
				get { return this.vertices[1].Color; }
				set { this.vertices[1].Color = value; }
			}
			public Vector3 PointA {
				get { return this.vertices[0].Position; } 
				set { this.vertices[0].Position = value; }
			}
			public Vector3 PointB {
				get { return this.vertices[1].Position; } 
				set { this.vertices[1].Position = value; }
			}

			public float Height {
				get { return this.height; }
			}

			VertexPositionColor[] IPrimitive.Vertices {
				get { return this.vertices; }
			}
			PrimitiveType IPrimitive.Type {
				get { return PrimitiveType.LineList; }
			}
			int IPrimitive.VerticesCount {
				get { return 2; }
			}


			public TimeLine() {
				vertices = new VertexPositionColor[2] {
					new VertexPositionColor(Vector3.Zero, Color.Black),
					new VertexPositionColor(Vector3.Zero, Color.Black)
				};
			}


			public void DarkenColor(float percent) {
				// Zatamnjuje liniju (obije boje) za odredeni postotak
				var darkAmount = (byte)(255 * percent);
				this.vertices[0].Color.R = (byte)Math.Max(0, this.vertices[0].Color.R - darkAmount);
				this.vertices[0].Color.G = (byte)Math.Max(0, this.vertices[0].Color.G - darkAmount);
				this.vertices[0].Color.B = (byte)Math.Max(0, this.vertices[0].Color.B - darkAmount);
				this.vertices[1].Color.R = (byte)Math.Max(0, this.vertices[1].Color.R - darkAmount);
				this.vertices[1].Color.G = (byte)Math.Max(0, this.vertices[1].Color.G - darkAmount);
				this.vertices[1].Color.B = (byte)Math.Max(0, this.vertices[1].Color.B - darkAmount);
			}

			public void ChangeLineHeight(float value) {
				// Postavlja visinu linije
				height = value * 2f;

				// Postavlja visinu na kojoj zavrsava linija,
				// posto je pocetna visina veca od zavrsne (origin je dole)
				// od pocetne visine se oduzima zadana visina linije
				this.vertices[1].Position.Y = this.vertices[0].Position.Y - height;

				// Ovisno o visini linije mijenja boju
				if (value >= 40) {
					this.vertices[0].Color = Color.DarkRed;
					this.vertices[1].Color = Color.Red;
				}
				else if (value >= 25) {
					this.vertices[0].Color = Color.YellowGreen;
					this.vertices[1].Color = Color.Yellow;
				}
				else {
					this.vertices[0].Color = Color.DarkGreen;
					this.vertices[1].Color = Color.Green;
				}
			}


			System.Collections.Generic.IEnumerable<IPrimitive> IDrawable.Primitives {
				get { yield return this; }
			}
			int IDrawable.PrimitivesCount {
				get { return 1; }
			}
		}
	}
}