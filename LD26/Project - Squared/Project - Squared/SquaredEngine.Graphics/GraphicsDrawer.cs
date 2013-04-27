using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquaredEngine.Diagnostics.Log;

namespace SquaredEngine.Graphics {
	/// <summary>
	/// Draws basic graphics.
	/// This is replacement for SpriteBatch as this also supports sprites.
	/// </summary>
	public partial class GraphicsDrawer {
		// NOTE: http://msdn.microsoft.com/en-us/library/bb196414.aspx
		// TODO: Document this class.
		// TODO: Dodati FontManager

		public String ID { get; private set; }
		private LoggerInstance log;

		#region Font manager

		public const int DebugFontID = int.MaxValue;
		private Dictionary<int, SpriteFont> fonts;

		public SpriteFont DebugFont {
			get { return GetFont(DebugFontID); }
			set { SetFont(DebugFontID, value); }
		}

		private void SetFont(int id, SpriteFont font) {
			if (!this.fonts.ContainsKey(id)) {
				this.fonts.Add(id, font);
			}
			else this.fonts[id] = font;
		}

		private SpriteFont GetFont(int id) {
			if (!this.fonts.ContainsKey(id)) {
				return null;
			}

			return this.fonts[id];
		}

		#endregion


		private List<IPrimitive> primitivesList;

		public GraphicsDevice GraphicsDevice { get; private set; }
		private SpriteBatch spriteBatch;

		public Matrix ViewMatrix {
			get { return this.basicEffect.View; }
			set { this.basicEffect.View = value; }
		}

		public Matrix WorldMatrix {
			get { return this.basicEffect.World; }
			set { this.basicEffect.World = value; }
		}

		public Matrix ProjectionMatrix {
			get { return this.basicEffect.Projection; }
			set { this.basicEffect.Projection = value; }
		}

		private BasicEffect basicEffect;
		private bool isBeginCalled;

		// TODO: Dodati novu klasu za VertexBuffer
		private bool hasCachedBuffers;
		//? private int buffersThreshold = 67108863;
		private const int MaxPrimitives = 21845;
		private Dictionary<int, KeyValuePair<int, VertexBuffer>> verticesTriangleListBuffer;
		private Dictionary<int, KeyValuePair<int, VertexBuffer>> verticesLineListBuffer;


		public GraphicsDrawer(GraphicsDevice graphicsDevice) {
			GraphicsDevice = graphicsDevice;

			ID = Guid.NewGuid().ToString();
			this.log = new LoggerInstance(typeof(GraphicsDrawer), ID);

			Initialize();
		}


		public void Initialize() {
			this.log.WriteInformation("Initializing...");

			GraphicsDevice.DeviceLost += (s, e) => Initialize();

			this.spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: Remove this after adding font manager
			this.fonts = new Dictionary<int, SpriteFont>() {
			                                          	{DebugFontID, null}
			                                          };

			this.primitivesList = new List<IPrimitive>();

			this.verticesLineListBuffer = new Dictionary<int, KeyValuePair<int, VertexBuffer>>();
			this.verticesTriangleListBuffer = new Dictionary<int, KeyValuePair<int, VertexBuffer>>();

			InitializeEffect();
			InitializeTransform();
		}

		private void InitializeTransform() {
			WorldMatrix = Matrix.Identity;
			ViewMatrix = Matrix.CreateLookAt(new Vector3(0f, 0f, 1f), Vector3.Zero, Vector3.Up);
			ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
																  GraphicsDevice.Viewport.Height, 0, 0f, 1f);

			this.log.WriteDebugInformation("Transform matrices initialized.");
		}

		private void InitializeEffect() {
			this.basicEffect = new BasicEffect(GraphicsDevice) {
				VertexColorEnabled = true,
			};

			this.log.WriteDebugInformation("Basic effect initialized.");
		}

		public void Begin(
			SpriteSortMode sortMode = SpriteSortMode.Deferred,
			BlendState blendState = null,
			SamplerState samplerState = null,
			DepthStencilState depthStencilState = null,
			RasterizerState rasterizerState = null,
			Effect effect = null,
			Matrix? transformMatrix = null) {
			CheckEndCall();
			this.isBeginCalled = true;

			// TODO: Promijeniti svojsta GraphicsDevice tako da iscrtava ispravno

			if (transformMatrix.HasValue) {
				ViewMatrix = transformMatrix.Value;
				spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, ViewMatrix);
			}
			else {
				spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect);
				ViewMatrix = Matrix.Identity;
			}
		}

		public void End() {
			CheckBeginCall();

			// Iscrtava sve osnovne oblike
			FlushPrimitives();

			// Iscrtava SpriteBatch pozive
			spriteBatch.End();

			isBeginCalled = false;
		}

		/// <summary>
		/// Draws primitives in list and clears that list.
		/// Use this in Draw.End call and when list has max number of allowed primitives.
		/// </summary>
		private void FlushPrimitives() {
			// If nothing to draw skip return
			if (primitivesList.Count == 0) return;

			// Apply effect
			this.basicEffect.CurrentTechnique.Passes[0].Apply();

			// Draw primitives from list
			foreach (IPrimitive primitive in primitivesList) {
				GraphicsDevice.DrawUserPrimitives(primitive.Type, primitive.Vertices, 0, 1);
			}

			// Clear drawn primitives
			this.primitivesList.Clear();
		}

		/// <summary>
		/// Creates new buffer for one drawable object.
		/// </summary>
		/// <typeparam name="TDrawable">A <see cref="IDrawable"/> object type to add to buffer.</typeparam>
		/// <param name="drawable">Object to add to buffer.</param>
		/// <returns>ID of just created buffer. Use this to call Draw method for this buffer.</returns>
		public int CreateBuffer<TDrawable>(TDrawable drawable, Int32? ID = null) where TDrawable : IDrawable {
			return CreateBuffer<TDrawable>(new List<TDrawable> { drawable }, ID);
		}

		/// <summary>
		/// Creates new buffer for collection of drawable objects.
		/// </summary>
		/// <typeparam name="TDrawable">A <see cref="IDrawable"/> object type to add to buffer.</typeparam>
		/// <param name="drawable">Collection of drawable object to add to buffer.</param>
		/// <param name="ID">Optional. ID of this buffer.</param>
		/// <returns>ID of just created buffer. Use this to call Draw method for this buffer.</returns>
		public int CreateBuffer<TDrawable>(IEnumerable<TDrawable> drawable, Int32? ID = null) where TDrawable : IDrawable {
			// TODO: Implementiraj optimizaciju buffera na neki nacin
			// NOTE: Optimizacija buffera ce se vrsiti preko GraphicsDrawer.PrimitivesBuffer klase);

			// Check if there is something to add to buffer
			if (drawable != null && drawable.Count() != 0) {
				List<VertexPositionColor> triangleList = new List<VertexPositionColor>();
				List<VertexPositionColor> lineList = new List<VertexPositionColor>();
				int lineCount = 0;
				int trianglesCount = 0;

				// Seperate triangles and lines
				foreach (TDrawable current in drawable) {
					foreach (IPrimitive primitive in current.Primitives) {
						if (primitive.Type == PrimitiveType.TriangleList) {
							triangleList.AddRange(primitive.Vertices);
							trianglesCount++;
						}
						else if (primitive.Type == PrimitiveType.LineList) {
							lineList.AddRange(primitive.Vertices);
							lineCount++;
						}
						else throw new InvalidOperationException("Only TrangleList and LineList are currently supported!");
					}
				}

				// Gets new random buffer ID if not passed by argument
				int bufferID = -1;
				if (ID.HasValue && ID.Value != null) {
					bufferID = ID.Value;
				}
				else bufferID = (new Random()).Next(100000, 999999);

				// Create triangles buffer
				if (triangleList.Count != 0) {
					VertexBuffer trianglesBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration,
																	triangleList.Count, BufferUsage.WriteOnly);
					trianglesBuffer.SetData<VertexPositionColor>(triangleList.ToArray());
					this.verticesTriangleListBuffer.Add(bufferID, new KeyValuePair<int, VertexBuffer>(trianglesCount, trianglesBuffer));

					this.hasCachedBuffers = true;
				}

				// Create lines buffer
				if (lineList.Count != 0) {
					VertexBuffer linesBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration,
																lineList.Count, BufferUsage.WriteOnly);
					linesBuffer.SetData<VertexPositionColor>(lineList.ToArray());
					this.verticesLineListBuffer.Add(bufferID, new KeyValuePair<int, VertexBuffer>(lineCount, linesBuffer));

					this.hasCachedBuffers = true;
				}

				this.log.WriteDebugInformation("New buffer created (" + bufferID + ") with " + (trianglesCount + lineCount) + " primitives");

				return bufferID;
			}
			return -1;
		}

		/// <summary>
		/// Removes buffer from cache.
		/// </summary>
		/// <param name="bufferID">ID of buffer to remove.</param>
		public void ClearBuffer(int bufferID) {
			this.log.WriteDebugInformation("Buffer (" + bufferID + ") cleared!");

			this.verticesLineListBuffer.Remove(bufferID);
			this.verticesLineListBuffer.Remove(bufferID);

			// Check if lists of buffers are empty, if so set variable hasCachedBuffer to false
			if (this.verticesLineListBuffer.Count == 0 && 
				this.verticesTriangleListBuffer.Count == 0) {
				this.hasCachedBuffers = false;

				this.log.WriteDebugInformation("No more cached buffers left!");
			}
		}

		#region Draw calls

		/// <summary>
		/// Draws buffer from cache.
		/// </summary>
		/// <param name="bufferID">ID of buffer to draw.</param>
		public void Draw(int bufferID) {
			// TODO: Promjeniti tako da se buffer ne iscrtava trenutno.

			// Check if there are any cached buffers.
			if (this.hasCachedBuffers) {
				this.basicEffect.CurrentTechnique.Passes[0].Apply();

				if (this.verticesTriangleListBuffer.ContainsKey(bufferID)) {
					GraphicsDevice.SetVertexBuffer(this.verticesTriangleListBuffer[bufferID].Value);
					GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.verticesTriangleListBuffer[bufferID].Key);
				}
				if (this.verticesLineListBuffer.ContainsKey(bufferID)) {
					GraphicsDevice.SetVertexBuffer(this.verticesLineListBuffer[bufferID].Value);
					GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, this.verticesLineListBuffer[bufferID].Key);
				}
			}
		}

		/// <summary>
		/// Draws drawable object.
		/// </summary>
		/// <typeparam name="T">A <see cref="IDrawable"/> object type to draw.</typeparam>
		/// <param name="drawable">Drawable object to draw.</param>
		public void Draw<T>(T drawable) where T : IDrawable {
			CheckBeginCall();

			// Check if this can be added to list or flush is needed.
			if (primitivesList.Count >= MaxPrimitives) {
				FlushPrimitives();
			}

			// If drawable contains primitives, add them to list.
			if (drawable != null && drawable.Primitives != null) {
				primitivesList.AddRange(drawable.Primitives);
			}
		}

		public void Draw(Texture2D texture, Vector2 position, Color color) {
			Draw(texture, position, null, color);
		}

		public void Draw(Texture2D texture, Vector2 position, Microsoft.Xna.Framework.Rectangle? sourceRectangle, Color color) {
			Draw(texture, position, sourceRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}

		public void Draw(Texture2D texture, Vector2 position, Microsoft.Xna.Framework.Rectangle? sourceRectangle, Color color,
						 float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
			Draw(texture, position, sourceRectangle, color, rotation, origin, Vector2.One * scale, effects, layerDepth);
		}

		public void Draw(Texture2D texture, Vector2 position, Microsoft.Xna.Framework.Rectangle? sourceRectangle, Color color,
						 float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
			CheckBeginCall();

			spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
		}

		public void Draw(Texture2D texture, Microsoft.Xna.Framework.Rectangle destinationRectangle, Color color) {
			Draw(texture, destinationRectangle, null, color);
		}

		public void Draw(Texture2D texture, Microsoft.Xna.Framework.Rectangle destinationRectangle,
						 Microsoft.Xna.Framework.Rectangle? sourceRectangle, Color color) {
			Draw(texture, destinationRectangle, sourceRectangle, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		}

		public void Draw(Texture2D texture, Microsoft.Xna.Framework.Rectangle destinationRectangle,
						 Microsoft.Xna.Framework.Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin,
						 SpriteEffects effects, float layerDepth) {
			CheckBeginCall();

			spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
		}

		#endregion

		#region Write calls

		public void Write(string text, Vector2 position) {
			Write(text, position, Color.LightGray);
		}

		public void Write(string text, Vector2 position, Color color) {
			Write(GetFont(DebugFontID), text, position, color);
		}

		public void Write(SpriteFont spriteFont, string text, Vector2 position, Color color) {
			Write(spriteFont, text, position, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}

		public void Write(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin,
						  float scale, SpriteEffects effects, float layerDepth) {
			Write(spriteFont, text, position, color, rotation, origin, Vector2.One * scale, effects, layerDepth);
		}

		public void Write(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin,
						  Vector2 scale, SpriteEffects effects, float layerDepth) {
			CheckBeginCall();

			spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
		}

		public void Write(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color) {
			Write(spriteFont, text, position, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}

		public void Write(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation,
						  Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
			Write(spriteFont, text, position, color, rotation, origin, Vector2.One * scale, effects, layerDepth);
		}

		public void Write(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation,
						  Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
			CheckBeginCall();

			spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
		}

		#endregion

		private void CheckBeginCall() {
			if (!isBeginCalled) {
				log.WriteFatalError("Begin call is needed before this call!");
				throw new InvalidOperationException("Begin call is needed before this call!");
			}
		}

		private void CheckEndCall() {
			if (this.isBeginCalled) {
				log.WriteFatalError("End call is needed before this call!");
				throw new InvalidOperationException("End call is needed before this call!");
			}
		}

		// TODO: Rotacija
		// NOTE: Rotacije i pomicanje objekata: http://forums.create.msdn.com/forums/t/7414.aspx?PageIndex=2
		// NOTE: Krug i elips: http://forums.create.msdn.com/forums/t/7414.aspx

		#region Primitives

		/// <summary>
		/// Basic line primitive.
		/// Line primitive doesn't have thickness (1px thick) and may only be colored using two colors.
		/// </summary>
		public struct Line : IPrimitive, IDrawable {
			private const Int32 PrimitivesCount = 1;
			private const Int32 VerticesCount = 2;
			private readonly VertexPositionColor[] vertices;

			VertexPositionColor[] IPrimitive.Vertices {
				get { return this.vertices; }
			}
			PrimitiveType IPrimitive.Type {
				get { return PrimitiveType.LineList; }
			}
			Int32 IPrimitive.VerticesCount {
				get { return VerticesCount; }
			}


			/// <summary>
			/// Initializes a new instance of the <see cref="Line"/> struct.
			/// </summary>
			/// <param name="pointA">The point A position.</param>
			/// <param name="pointB">The point B position.</param>
			/// <param name="color">The color of both point A and point B.</param>
			public Line(Vector3 pointA, Vector3 pointB, Color color)
				: this(pointA, pointB, color, color) {
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Line"/> struct.
			/// </summary>
			/// <param name="pointA">The point A position.</param>
			/// <param name="pointB">The point B position.</param>
			/// <param name="colorA">The color of point A.</param>
			/// <param name="colorB">The color of point B.</param>
			public Line(Vector3 pointA, Vector3 pointB, Color colorA, Color colorB) {
				this.vertices = new VertexPositionColor[VerticesCount] {
				                                                       	new VertexPositionColor(pointA, colorA),
				                                                       	new VertexPositionColor(pointB, colorB)
				                                                       };
			}


			IEnumerable<IPrimitive> IDrawable.Primitives {
				get { yield return this; }
			}

			int IDrawable.PrimitivesCount {
				get { return PrimitivesCount; }
			}
		}

		/// <summary>
		/// Basic triangle primitive.
		/// Triangle primitive doesn't have thickness (1px thick) and my only be colored using three colors.
		/// </summary>
		public struct Triangle : IPrimitive, IDrawable {
			private const Int32 PrimitivesCount = 1;
			private const Int32 VerticesCount = 3;
			private readonly VertexPositionColor[] vertices;

			VertexPositionColor[] IPrimitive.Vertices {
				get { return this.vertices; }
			}

			PrimitiveType IPrimitive.Type {
				get { return PrimitiveType.TriangleList; }
			}

			int IPrimitive.VerticesCount {
				get { return VerticesCount; }
			}


			/// <summary>
			/// Initializes a new instance of the <see cref="Triangle"/> struct.
			/// </summary>
			/// <param name="pointA">The point A position.</param>
			/// <param name="pointB">The point B position.</param>
			/// <param name="pointC">The point C position.</param>
			/// <param name="color">The color of all three points (point A, point B and point C).</param>
			public Triangle(Vector3 pointA, Vector3 pointB, Vector3 pointC, Color color)
				: this(pointA, pointB, pointC, color, color, color) {
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Triangle"/> struct.
			/// </summary>
			/// <param name="pointA">The point A position.</param>
			/// <param name="pointB">The point B position.</param>
			/// <param name="pointC">The point C position.</param>
			/// <param name="colorA">The color of point A.</param>
			/// <param name="colorB">The color of point B.</param>
			/// <param name="colorC">The color of point C.</param>
			public Triangle(Vector3 pointA, Vector3 pointB, Vector3 pointC, Color colorA, Color colorB, Color colorC) {
				this.vertices = new VertexPositionColor[VerticesCount] {
				                                                       	new VertexPositionColor(pointA, colorA),
				                                                       	new VertexPositionColor(pointB, colorB),
				                                                       	new VertexPositionColor(pointC, colorC)
				                                                       };
			}


			IEnumerable<IPrimitive> IDrawable.Primitives {
				get { return new IPrimitive[] { this }; }
			}

			Int32 IDrawable.PrimitivesCount {
				get { return PrimitivesCount; }
			}
		}

		#endregion
	}
}