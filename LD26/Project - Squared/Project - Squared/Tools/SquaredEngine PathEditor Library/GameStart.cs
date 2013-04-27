#define XNA

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using SquaredEngine.Graphics;


namespace SquaredEngine.PathEditor.Library {
	public class GameStart : Microsoft.Xna.Framework.Game {
		private GraphicsDeviceManager graphics;
		private GraphicsDrawer gd;

		private List<Vector3> controlPoints;
		private Vector3[] curvePoints;

		private MouseState currentMouseState;
		private MouseState previousMouseState;


		private PathEditorTools currentTool = PathEditorTools.Add;

		// Selection tool variables
		private int selectedPointIndex = -1;
		private bool isPointSelected;


		private int endPointIndex = -1;
		private bool isEndPoint;

		private bool isEndLine;
		private int indexPointA, indexPointB;

		#region Properties

		#region Drawing

		public Color BackgroundColor { get; set; }
		public Color ForegroundColor { get; set; }

		public Boolean IsControlPointsDrawing { get; set; }
		public Boolean IsControlLinesDrawing { get; set; }
		public Color ControlPointColor { get; set; }
		public Color ControlLineColor { get; set; }
		public Color ControlPointSelectedColor { get; set; }
		public Color ControlLineSelectedColor { get; set; }

		public Boolean IsCurvePointsDrawing { get; set; }
		public Boolean IsCurveLinesDrawing { get; set; }
		public Color CurvePointColor { get; set; }
		public Color CurveLineColor { get; set; }

		#endregion

		#endregion

		public GameStart() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}


		protected override void Initialize() {
			IsMouseVisible = true;

			graphics.PreferredBackBufferHeight = 600;
			graphics.PreferredBackBufferWidth = 800;
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();

			gd = new GraphicsDrawer(GraphicsDevice);

			controlPoints = new List<Vector3>();
			curvePoints = new Vector3[0];

			base.Initialize();

			#region Properties initialization

			#region Drawing properties

			// General properties
			BackgroundColor = Color.Black;
			ForegroundColor = Color.White;

			// Control line properties
			IsControlLinesDrawing = true;
			ControlLineColor = Color.LightGray;
			ControlLineSelectedColor = Color.Orange;

			// Control point properties
			IsControlPointsDrawing = true;
			ControlPointColor = Color.Gray;
			ControlPointSelectedColor = Color.Orange;

			// Curve line properties
			IsCurveLinesDrawing = true;
			CurveLineColor = Color.YellowGreen;

			// Curve point properties
			IsCurvePointsDrawing = true;
			CurvePointColor = Color.DarkGreen;

			#endregion

			#endregion
		}

		protected override void LoadContent() {

		}

		protected override void UnloadContent() {

		}
		
		public void ChangeTool(PathEditorTools toTools) {
			currentTool = toTools;
		}

		protected override void Update(GameTime gameTime) {
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				this.Exit();

#if XNA
			if (Keyboard.GetState().IsKeyDown(Keys.R)) ChangeTool(PathEditorTools.Remove);
			if (Keyboard.GetState().IsKeyDown(Keys.M)) ChangeTool(PathEditorTools.Move);
			if (Keyboard.GetState().IsKeyDown(Keys.A)) ChangeTool(PathEditorTools.Add);
			if (Keyboard.GetState().IsKeyDown(Keys.S)) ChangeTool(PathEditorTools.Split);
#endif

			previousMouseState = currentMouseState;
			currentMouseState = Mouse.GetState();

			if (currentMouseState.X >= 0 && currentMouseState.X <= GraphicsDevice.Viewport.Width &&
				currentMouseState.Y >= 0 && currentMouseState.Y <= GraphicsDevice.Viewport.Height) {

				if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && IsOnPoint(currentMouseState.X, currentMouseState.Y, 10)) {
					// BUG: random ArgumentOutOfRangeException
					MouseExtensions.SetMousePosition(!isEndPoint
											? controlPoints[GetOnPoint(currentMouseState.X, currentMouseState.Y, 10)]
											: controlPoints[GetOnPoint(currentMouseState.X, currentMouseState.Y, 4)]);
				}

				isEndLine = false;
				if (currentTool == PathEditorTools.Split) {
					for (int index = 0; index < controlPoints.Count - 1; index++) {
						if (DistanceToPoint(controlPoints[index], controlPoints[index + 1], currentMouseState.X, currentMouseState.Y) <= 10) {
							isEndLine = true;
							indexPointA = index;
							indexPointB = index + 1;
						}
					}
				}

				if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released) {
					if (IsOnPoint(currentMouseState.X, currentMouseState.Y, 4.0f)) {
						if (currentTool == PathEditorTools.Move) {
							isPointSelected = true;
							selectedPointIndex = GetOnPoint(currentMouseState.X, currentMouseState.Y, 4.0f);
						}
						else if (currentTool == PathEditorTools.Remove) {
							controlPoints.RemoveAt(GetOnPoint(currentMouseState.X, currentMouseState.Y, 4.0f));
							RecalculateCurve(controlPoints.ToArray(), 10, out curvePoints);
						}
					}
					else if (currentTool == PathEditorTools.Add) {
						this.controlPoints.Add(new Vector3(currentMouseState.X, currentMouseState.Y, 0f));
						RecalculateCurve(controlPoints.ToArray(), 10, out this.curvePoints);
					}
					else if (isEndLine && currentTool == PathEditorTools.Split) {
						float newX = (controlPoints[indexPointA].X + controlPoints[indexPointB].X) / 2;
						float newY = (controlPoints[indexPointA].Y + controlPoints[indexPointB].Y) / 2;
						float newZ = (controlPoints[indexPointA].Z + controlPoints[indexPointB].Z) / 2;

						controlPoints.Insert(indexPointA + 1, new Vector3(newX, newY, newZ));

						RecalculateCurve(controlPoints.ToArray(), 10, out curvePoints);
					}
				}
				if (currentTool == PathEditorTools.Move) {
					if (isPointSelected && currentMouseState.LeftButton == ButtonState.Pressed) {
						controlPoints[selectedPointIndex] = new Vector3(currentMouseState.X, currentMouseState.Y, controlPoints[selectedPointIndex].Z);
						RecalculateCurve(controlPoints.ToArray(), 10, out curvePoints);
					}
					else if (isPointSelected && currentMouseState.LeftButton == ButtonState.Released) {
						isPointSelected = false;
					}
				}

				if (IsOnPoint(currentMouseState.X, currentMouseState.Y, 4.0f)) {
					endPointIndex = GetOnPoint(currentMouseState.X, currentMouseState.Y, 4.0f);
					isEndPoint = true;
				}
				else isEndPoint = false;
			}

			base.Update(gameTime);
		}

		private static float DistanceToPoint(Vector2 a, Vector2 b, float x, float y) {
			return DistanceToPoint(new Vector3(a, 0f), new Vector3(b, 0f), x, y);
		}
		private static float DistanceToPoint(Vector3 a, Vector3 b, float x, float y) {
			float vx = a.X - x;
			float vy = a.Y - y;
			float ux = b.X - a.X;
			float uy = b.Y - a.Y;
			float length = ux * ux + uy * uy;

			float det = (-vx * ux) + (-vy * uy);
			if (det < 0 || det > length) {
				ux = b.X - x;
				uy = b.Y - y;
				return Math.Min(vx * vx + vy * vy, ux * ux + uy * uy);
			}

			det = ux * vy - uy * vx;
			return (det * det) / length;
		}

		private int GetOnPoint(int x, int y, float pointSize) {
			for (int index = 0; index < controlPoints.Count; index++) {
				if (Math.Abs(controlPoints[index].X - x) <= pointSize && 
					Math.Abs(controlPoints[index].Y - y) <= pointSize) {
					return index;
				}
			}
			return -1;
		}
		private bool IsOnPoint(int x, int y, float pointSize) {
			return controlPoints.Any(point => Math.Abs(point.X - x) <= pointSize && Math.Abs(point.Y - y) <= pointSize);
		}

		private static void RecalculateCurve(Vector3[] controlPoints, int instancesPerControl, out Vector3[] curvePoints) {
			if (controlPoints.Length <= 3) {
				curvePoints = new Vector3[0];
				return;
			}

			curvePoints = new Vector3[(controlPoints.Length - 3) * instancesPerControl];

			for (int index = 0; index < controlPoints.Length - 3; index++) {
				for (int position = 0; position < instancesPerControl; position++) {
					curvePoints[index*instancesPerControl + position] =
						Vector3.CatmullRom(controlPoints[index], controlPoints[index + 1], controlPoints[index + 2], controlPoints[index + 3],
						                   (float)position/instancesPerControl);
				}
			}	
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(BackgroundColor);

			this.gd.Begin();

			DrawControlNodes();
			DrawCurveNodes();

			this.gd.End();


			base.Draw(gameTime);
		}

		private void DrawCurveNodes() {
			if (IsCurveLinesDrawing) {
				for (int index = 0; index < this.curvePoints.Length - 1; index++) {
					GraphicsDrawer.Line curveLine = new GraphicsDrawer.Line(
						this.curvePoints[index], this.curvePoints[index + 1], CurveLineColor);
					this.gd.Draw<GraphicsDrawer.Line>(curveLine);
				}
			}

			if (IsCurvePointsDrawing) {
				foreach (Vector3 point in this.curvePoints) {
					Ellipse curvePoint = 
						new Ellipse(point, 2, 2, CurvePointColor);
					this.gd.Draw<Ellipse>(curvePoint);
				}
			}
		}

		private void DrawControlNodes() {
			// Draw Control lines
			if (IsControlLinesDrawing) {
				for (int index = 0; index < this.controlPoints.Count - 1; index++) {
					GraphicsDrawer.Line controlLine = new GraphicsDrawer.Line(
						this.controlPoints[index],
						this.controlPoints[index + 1],
						this.isEndLine && index == this.indexPointA ? ControlLineSelectedColor : ControlLineColor);
					this.gd.Draw<GraphicsDrawer.Line>(controlLine);
				}
			}

			// Draw Control points
			if (IsControlPointsDrawing) {
				for (int index = 0; index < this.controlPoints.Count; index++) {
					// Draw Control point selection outline (if exists)
					if (this.isEndPoint && index == this.endPointIndex) {
						EllipseOutline selectionOutline = 
							new EllipseOutline(this.controlPoints[index], 5, 5, ControlPointSelectedColor);
						this.gd.Draw(selectionOutline);
					}
					Ellipse controlPoint = 
						new Ellipse(this.controlPoints[index], 2, 2, ControlPointColor);
					this.gd.Draw<Ellipse>(controlPoint);
				}
			}
		}
	}
}
