using System;
using Microsoft.Xna.Framework;


namespace SquaredEngine.Common {

	/// <summary>
	/// Klasa vremenskog razmaka aplikacije
	/// </summary>
	public class Time {
		private readonly float delta;
		private readonly float elapsed;

		private GameTime toGameTime;
		private bool isConvertedToGameTime;

		/// <summary>
		/// Constructs an Time object.
		/// </summary>
		/// <param name="elapsedSinceStart">Seconds from start of the application.</param>
		/// <param name="timeDeltaSeconds">Seconds from last update call.</param>
		public Time(float elapsedSinceStart = 0f, float timeDeltaSeconds = 0f) {
			elapsed = elapsedSinceStart;
			delta = timeDeltaSeconds;
		}

		/// <summary>
		/// Gets milliseconds from start of the application.
		/// </summary>
		public double Elapsed {
			get { return elapsed; }
		}

		/// <summary>
		/// Gets delta time from last update.
		/// </summary>
		public double Delta {
			get { return delta; }
		}

		/// <summary>
		/// Converts Time object to Microsoft.Xna.Framework.GameTime object.
		/// </summary>
		/// <returns>GameTime object of samo values as Time object.</returns>
		public GameTime ToGameTime() {
			// Kreira novu inacicu GameTime strukture, ukoliko ona vec nije kreirana, 
			// koja se koristi kad se poziva pretvorba iz Time u GameTime kako ne bi 
			// svaki put trebali ispocetka kreirati strukturu.
			if (!isConvertedToGameTime) {
				toGameTime = new GameTime(
					new TimeSpan(0, 0, 0, 0, (int) elapsed),
					new TimeSpan(0, 0, 0, 0, (int) delta));
				isConvertedToGameTime = true;
			}

			// Vraca vrijednost pohranjene strukture
			return toGameTime;
		}

		/// <summary>
		/// Implict conversion to Microsoft.Xna.Framework.GameTime.
		/// </summary>
		/// <param name="toConvert">Time object to cenvert.</param>
		/// <returns>Microsoft.Xna.Framework.GameTime object that cooresponds to current time.</returns>
		public static implicit operator GameTime(Time toConvert) {
			return toConvert.ToGameTime();
		}
	}
}