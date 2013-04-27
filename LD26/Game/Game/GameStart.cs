using System;
using System.Collections.Generic;
using System.Linq;
using Game.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SquaredEngine.Common;
using SquaredEngine.Diagnostics.Debug;
using SquaredEngine.Graphics;
using SquaredEngine.Input;

namespace Game {
    public class GameStart : Microsoft.Xna.Framework.Game {
        // TODO Minimalism
		// TODO Tower defense?

        private GraphicsDeviceManager graphics;
        private GraphicsDrawer drawer;
        private TimeDebuger timeDebuger;
	    private KeyboardController keyboard;
	    private MouseState previousMouseState;
	    private MouseState currentMouseState;
	    private TimeController timeController;
	    private Camera2D camera;

	    private Player player;
	    private const int StartResources = 2000;

	    private List<StaticTower> towers;
	    private StaticTower buildTower;
	    private static Color TowerBuildCan = Color.ForestGreen;
	    private static Color TowerBuildObsticle = Color.Red;
	    private static Color TowerColor = Color.DarkOrange;
	    private const int TowerSize = 25;
	    private const int TowerSize2 = TowerSize * 2;

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

	        this.camera = new Camera2D(this);
	        this.Components.Add(this.camera);

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

	        this.towers = new List<StaticTower>();
        }

        protected override void Update(GameTime gameTime) {
	        this.previousMouseState = this.currentMouseState;
	        this.currentMouseState = Mouse.GetState();

			// On mouse click
	        if (this.currentMouseState.LeftButton == ButtonState.Pressed &&
		        this.previousMouseState.LeftButton == ButtonState.Released) {

		        // Check if build tower needs to be dropped
		        if (this.buildTower != null && !this.buildTower.IsDropped && this.buildTower.CanBeDropped) {
			        this.buildTower.IsDropped = true;
			        this.buildTower.Color = TowerColor;
					this.player.Resources -= this.buildTower.Cost;
			        this.towers.Add(this.buildTower);
			        this.buildTower = null;
		        }
	        }

			// On right mouse click
			if (this.currentMouseState.RightButton == ButtonState.Pressed &&
				this.previousMouseState.RightButton == ButtonState.Released) {
				
				// TODO Testing
				this.buildTower = new StaticTower() {
					Color = TowerBuildCan,
					Size = TowerSize,
					Cost = 100
				};
			}

			// Move build tower around
	        if (this.buildTower != null && !this.buildTower.IsDropped) {
		        this.buildTower.Position = new Vector3(
			        (this.currentMouseState.X / TowerSize2) * TowerSize2 + TowerSize,
				        (this.currentMouseState.Y / TowerSize2) * TowerSize2 + TowerSize, 0);

				// Check collision
		        if (this.towers.Any(t => t.Position == this.buildTower.Position)) {
			        this.buildTower.Color = TowerBuildObsticle;
			        this.buildTower.CanBeDropped = false;
		        }
		        else {
					this.buildTower.Color = TowerBuildCan;
					this.buildTower.CanBeDropped = true;
				}

		        this.buildTower.Rebuild();
	        }

	        base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
	        this.GraphicsDevice.Clear(new Color(96, 196, 116));

			this.drawer.Begin();
	        
			// Draw all towers
			foreach (var tower in towers) {
		        tower.Draw(drawer, this.timeController.Time);
	        }

			// Draw build tower
			if (this.buildTower != null)
				this.buildTower.Draw(this.drawer, this.timeController.Time);

	        this.drawer.Write(this.player.Name, new Vector2(20, 20));
	        this.drawer.Write(this.player.Resources.ToString(), new Vector2(20, 40));
	        
			this.drawer.Draw<ConvexPolygon>(new ConvexPolygon())

			this.drawer.End();

            base.Draw(gameTime);
        }
    }
}
