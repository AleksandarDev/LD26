using System;
using Microsoft.Xna.Framework;


namespace SquaredEngine.Common {

	/// <summary>
	/// Timestep control for Game component
	/// </summary>
	public class TimeController : GameComponent {
		#region Varijable

		private Common.Time currentTime;
		private Common.Time previousTime;
		private DateTime startTime;

		private float timeStep = 1.0f;

		#endregion

		#region Svojstva

		/// <summary>
		/// Gets object creation time, which represents start of application.
		/// </summary>
		public DateTime StartTime {
			get { return this.startTime; }
			protected set { this.startTime = value; }
		}

		/// <summary>
		/// Gets or sets current timestep of the application.
		/// </summary>
		public virtual float TimeStep {
			get { return this.timeStep; }
			set {
				// Ukoliko je omoguceno negativno stanje vremenskog pomaka ili
				// je zadani vremenski pomak veci ili jednak nuli onda se nastavlja
				// u suprotnom se (javlja greska) postavlja na nulu
				if (EnableNegativeTimeStep || value >= 0) {
					this.timeStep = value;
				}
				else {
					this.timeStep = 0;
				}
			}
		}

		/// <summary>
		/// Gets current frame application time.
		/// </summary>
		public Common.Time Time {
			get { return this.currentTime; }
		}
		/// <summary>
		/// Gets previous frame application time
		/// </summary>
		public Common.Time PreviousTime {
			get { return this.previousTime; }
		}

		/// <summary>
		/// Gets or sets ability of timestap to go negative.
		/// </summary>
		public bool EnableNegativeTimeStep { get; set; }

		#endregion

		#region Kontruktor

		/// <summary>
		/// Constructs an TimeController object.
		/// </summary>
		/// <param name="game">Game for which this object works.</param>
		/// <param name="enableNegativeTimeStep">Enables timestep to go negative.</param>
		public TimeController(Game game, bool enableNegativeTimeStep = false) : base(game) {
			startTime = DateTime.Now;
			EnableNegativeTimeStep = enableNegativeTimeStep;
		}

		#endregion

		#region Metode

		/// <summary>
		/// Updates application time.
		/// </summary>
		/// <param name="gameTime">Time from update method in Game class of the game.</param>
		public override void Update(GameTime gameTime) {
			// Postavlja vrijeme varijable trenutnog vremena kao prijasnje vrijeme
			previousTime = currentTime;

			// Izracunava trenutno vrijeme prema dobivenim parametrima
			currentTime = CalculateNewTime(gameTime);
		}

		protected virtual Common.Time CalculateNewTime(GameTime gameTime) {
			// Izracunava ukupno prodeno vrijeme od pocetka aplikacije do sad te iz prosljedenog vremena
			// uzima komponentu sekundi te je mnozi s pomakom vremena kako bi dobili ubrzanje ili
			// usporavanje vremena koje cemo koristiti u aplikaciji.
			return new Common.Time((float)(DateTime.Now - startTime).TotalSeconds, (float)(gameTime.ElapsedGameTime.TotalSeconds * TimeStep));
		}

		#endregion
	}
}