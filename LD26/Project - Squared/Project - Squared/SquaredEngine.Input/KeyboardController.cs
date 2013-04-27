using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SquaredEngine.Input {
	/// <summary>
	/// Keyboard input Component that updates keyboard states and adds some additional functionalityes
	/// </summary>
	public class KeyboardController : GameComponent {
		// TODO: Add XBOX and WP7 support
		// TODO: Add events for keys
		private Boolean[] isKeyCheckedCollection;
		private Boolean[] isKeyClickedCollection;
		private Boolean[] isKeyHeldCollection;

		private KeyboardState currentState;
		private KeyboardState previousState;

		/// <summary>
		/// Gets the current keyboard state.
		/// This is state from current Update call.
		/// </summary>
		public KeyboardState Keyboard {
			get { return currentState; }
		}

		/// <summary>
		/// Gets the previous keyboard state.
		/// This is state from last Update call.
		/// </summary>
		public KeyboardState KeyboardPrevious {
			get { return previousState; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="KeyboardController"/> class.
		/// </summary>
		/// <param name="game">Game that the game component should be attached to.</param>
		public KeyboardController(Game game)
			: base(game) {
		}


		/// <summary>
		/// Initializes controller;
		/// </summary>
		public override void Initialize() {
			ResetCollections();

			base.Initialize();
		}

		/// <summary>
		/// Updates controller.
		/// Gets current keyboard state and stores previous state.
		/// </summary>
		/// <param name="gameTime">Current game time</param>
		public override void Update(GameTime gameTime) {
			// Save previous state from variable currentState to previousState and
			// update currentState variable with real current keyboard state
			previousState = currentState;
			currentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

			// Reset collection of key states
			ResetCollections();

			base.Update(gameTime);
		}

		/// <summary>
		/// Resets collection of key states.
		/// Use this every time keyboard state chenges.
		/// </summary>
		private void ResetCollections() {
			isKeyCheckedCollection = new Boolean[255];
			isKeyClickedCollection = new Boolean[255];
			isKeyHeldCollection = new Boolean[255];
		}

		/// <summary>
		/// Determines whether the specified key is Down.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>
		///   <c>true</c> if the specified key is Down; otherwise, <c>false</c>.
		/// </returns>
		public Boolean IsDown(Keys key) {
			return Keyboard.IsKeyDown(key);
		}

		/// <summary>
		/// Determines whether the specified key is Up.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>
		///   <c>true</c> if the specified key is Up; otherwise, <c>false</c>.
		/// </returns>
		public Boolean IsUp(Keys key) {
			return !IsDown(key);
		}

		/// <summary>
		/// Determines whether the specified key was Down.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>
		///   <c>true</c> if the specified key was Down; otherwise, <c>false</c>.
		/// </returns>
		public Boolean WasDown(Keys key) {
			return KeyboardPrevious.IsKeyDown(key);
		}

		/// <summary>
		/// Determines whether the specified key was Up.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>
		///   <c>true</c> if the specified key was Up; otherwise, <c>false</c>.
		/// </returns>
		public Boolean WasUp(Keys key) {
			return !WasDown(key);
		}

		/// <summary>
		/// Determines whether the specified key is clicked.
		/// Clicked in this case means that key was Up on previous Update call and Down on current Update call .
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>
		///   <c>true</c> if the specified key is clicked; otherwise, <c>false</c>.
		/// </returns>
		public Boolean IsClicked(Keys key) {
			CheckIfNot(key);
			return isKeyClickedCollection[(Int32)key];
		}

		/// <summary>
		/// Determines whether the specified key is held.
		/// Held in this case means that key was Down in previous Update call and Down in current Update call.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>
		///   <c>true</c> if the specified key is held; otherwise, <c>false</c>.
		/// </returns>
		public Boolean IsHeld(Keys key) {
			CheckIfNot(key);
			return isKeyHeldCollection[(Int32)key];
		}

		/// <summary>
		/// Check if requested key was already processed, if not precess it.
		/// </summary>
		/// <param name="key">Key to check</param>
		private void CheckIfNot(Keys key) {
			if (!isKeyCheckedCollection[(Int32)key]) {
				isKeyClickedCollection[(Int32)key] = Keyboard.IsKeyDown(key) && KeyboardPrevious.IsKeyUp(key);
				isKeyHeldCollection[(Int32)key] = Keyboard.IsKeyDown(key) && KeyboardPrevious.IsKeyDown(key);
			}
		}
	}
}