using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SquaredEngine.Diagnostics.Log;

namespace SquaredEngine.Input {
	/// <summary>
	/// Xbox controller input Component that updates controller states and adds some additional functionality
	/// </summary>
	public class GamePadController : GameComponent {
		public String ID { get; private set; }
		private LoggerInstance log;

		private Boolean[] isGamePadActive;
		private GamePadState[] gamePads;
		private GamePadState[] gamePadsPrevious;
		private GamePadDeadZone[] gamePadDeadZones;

		//private Boolean[] isGamePadChecked;
		//private GamePadDPad[] isDPadClickedCollection;
		//private GamePadButtons[] isButtonClickedCollection;
		//private GamePadDPad[] isDPadHeldCollection;
		//private GamePadButtons[] isButtonHeldCollection;

		#region Properties

		/// <summary>
		/// Gets the game pad of player one.
		/// </summary>
		public GamePadState GamePadOne {
			get { return this.gamePads[0]; }
		}

		/// <summary>
		/// Gets the game pad of player two.
		/// </summary>
		public GamePadState GamePadTwo {
			get { return this.gamePads[1]; }
		}

		/// <summary>
		/// Gets the game pad of player three.
		/// </summary>
		public GamePadState GamePadThree {
			get { return this.gamePads[2]; }
		}

		/// <summary>
		/// Gets the game pad of player four.
		/// </summary>
		public GamePadState GamePadFour {
			get { return this.gamePads[3]; }
		}

		/// <summary>
		/// Gets the game pad of player one from last update.
		/// </summary>
		public GamePadState GamePadOnePrevious {
			get { return this.gamePadsPrevious[0]; }
		}

		/// <summary>
		/// Gets the game pad of player two  from last update.
		/// </summary>
		public GamePadState GamePadTwoPrevious {
			get { return this.gamePadsPrevious[1]; }
		}

		/// <summary>
		/// Gets the game pad of player three  from last update.
		/// </summary>
		public GamePadState GamePadThreePrevious {
			get { return this.gamePadsPrevious[2]; }
		}

		/// <summary>
		/// Gets the game pad of player four  from last update.
		/// </summary>
		public GamePadState GamePadFourPrevious {
			get { return this.gamePadsPrevious[3]; }
		}

		#endregion



		/// <summary>
		/// Initializes a new instance of the <see cref="GamePadController"/> class.
		/// </summary>
		/// <param name="game">Game that the game component should be attached to.</param>
		public GamePadController(Game game)
			: base(game) {
			ID = Guid.NewGuid().ToString();
			log = new LoggerInstance(typeof (GamePadController), ID);

			this.isGamePadActive = new Boolean[4];
			this.gamePads = new GamePadState[4];
			this.gamePadsPrevious = new GamePadState[4];
			this.gamePadDeadZones = new GamePadDeadZone[4];

			//ResetCollections();
		}


		public override void Initialize() {
			base.Initialize();
		}

		/// <summary>
		/// Called when the GameComponent needs to be updated. Updates connected game pad states.
		/// </summary>
		/// <param name="gameTime">Time elapsed since the last call to Update</param>
		public override void Update(GameTime gameTime) {
			// Gets state for each player's game pad
			for (int playerIndex = 0; playerIndex < (Int32)PlayerIndex.Four; playerIndex++) {
				GamePadState state = GamePad.GetState((PlayerIndex)playerIndex, this.gamePadDeadZones[playerIndex]);

				// Check if game pad is connected and if it was connected on last update, if so then
				// just update game pad status but if game pad isn't connected and it was connected
				// on last update then we have to unregister gamepad, this means that if game pad is
				// connected and none of abobe cases were not true then we have to register new game pad.
				if (state.IsConnected && this.isGamePadActive[playerIndex]) {
					UpdateGamePadState(playerIndex, state);
				}
				else if (!state.IsConnected && this.isGamePadActive[playerIndex]) {
					UnregisterGamePad(playerIndex);
				}
				else if (state.IsConnected) {
					RegisterNewGamePad(playerIndex, state);
				}
			}

			base.Update(gameTime);
		}

		private void UpdateGamePadState(Int32 playerIndex, GamePadState state) {
			this.gamePadsPrevious[playerIndex] = this.gamePads[playerIndex];
			this.gamePads[playerIndex] = state;

			//ResetCollections();
		}

		private void RegisterNewGamePad(Int32 playerIndex, GamePadState state) {
			this.isGamePadActive[playerIndex] = true;
			// TODO Add event

			this.log.WriteInformation("New GamePad Player{0} is registered!", (PlayerIndex)playerIndex);
		}

		private void UnregisterGamePad(Int32 playerIndex) {
			this.isGamePadActive[playerIndex] = false;
			// TODO Add event

			this.log.WriteInformation("GamePad as Player{0} is unregistered!", (PlayerIndex)playerIndex);
		}

		//private void ResetCollections() {
		//    this.isGamePadChecked = new Boolean[4];

		//    this.isDPadClickedCollection = new GamePadDPad[4];
		//    this.isDPadHeldCollection = new GamePadDPad[4];

		//    this.isButtonClickedCollection = new GamePadButtons[4];
		//    this.isButtonHeldCollection = new GamePadButtons[4];
		//}


		/// <summary>
		/// Gets the current state of the game pad.
		/// </summary>
		/// <param name="index">The player index (index of game pad)..</param>
		/// <returns>Current state of specified player's game pad.</returns>
		public GamePadState GetState(PlayerIndex index) {
			return this.gamePads[(Int32)index];
		}

		/// <summary>
		/// Gets the previous state of the game pad..
		/// </summary>
		/// <param name="index">The player index (index of game pad)..</param>
		/// <returns>Previous state of specified player's game pad.</returns>
		public GamePadState GetPreviousState(PlayerIndex index) {
			return this.gamePadsPrevious[(Int32)index];
		}


		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="detailed">if set to <c>true</c> string returned contains detailed information about game pad states.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public new string ToString(Boolean detailed = false) {
			if (!detailed) {
				return "GamePadController (" + ID + ")";
			}
			else {
				string returnString = String.Empty;
				for (int playerIndex = 0; playerIndex <= (UInt32)PlayerIndex.Four; playerIndex++) {
					GamePadState state = GetState((PlayerIndex)playerIndex);

					string dPadState = "None";
					if (state.DPad.Right == ButtonState.Pressed) dPadState = "Right";
					else if (state.DPad.Left == ButtonState.Pressed) dPadState = "Left";
					else if (state.DPad.Up == ButtonState.Pressed) dPadState = "Up";
					else if (state.DPad.Down == ButtonState.Pressed) dPadState = "Down";

					string buttonsState = String.Empty;
					if (state.Buttons.A == ButtonState.Pressed) buttonsState += "A "; else buttonsState += "- ";
					if (state.Buttons.B == ButtonState.Pressed) buttonsState += "B "; else buttonsState += "- ";
					if (state.Buttons.X == ButtonState.Pressed) buttonsState += "X "; else buttonsState += "- ";
					if (state.Buttons.Y == ButtonState.Pressed) buttonsState += "Y "; else buttonsState += "- ";

					string controlButtonsState = String.Empty;
					if (state.Buttons.Back == ButtonState.Pressed) buttonsState += "Back "; else buttonsState += "- ";
					if (state.Buttons.BigButton == ButtonState.Pressed) buttonsState += "BigButton "; else buttonsState += "- ";
					if (state.Buttons.Start == ButtonState.Pressed) buttonsState += "Start "; else buttonsState += "- ";

					string stickButtonsState = String.Empty;
					if (state.Buttons.LeftStick == ButtonState.Pressed) buttonsState += "LeftStick "; else buttonsState += "- ";
					if (state.Buttons.RightStick == ButtonState.Pressed) buttonsState += "RightStick "; else buttonsState += "- ";

					string shoulderButtonsState = String.Empty;
					if (state.Buttons.LeftShoulder == ButtonState.Pressed) buttonsState += "LeftShoulder "; else buttonsState += "- ";
					if (state.Buttons.RightShoulder == ButtonState.Pressed) buttonsState += "RightShoulder "; else buttonsState += "- ";

					returnString +=
						String.Format(
							"GamePad of player {0}\n" + 
							"    {1}\n" + 
							"    DPad:       {2}\n" + 
							"    Buttons:    {3} {4} {5} {6}\n" + 
							"    Triggers:   {7}\n" + 
							"                {8}\n" + 
							"    ThumbStics: {9}\n" + 
							"                {10}\n\n",
							(PlayerIndex)playerIndex,
							state.IsConnected ? "GamePad is connected!" : "GamePad Not connected!",
							dPadState,
							buttonsState,
							controlButtonsState,
							stickButtonsState,
							shoulderButtonsState,
							state.Triggers.Left, state.Triggers.Right, 
							state.ThumbSticks.Left, state.ThumbSticks.Right);
				}

				return returnString;
			}
		}
	}
}
