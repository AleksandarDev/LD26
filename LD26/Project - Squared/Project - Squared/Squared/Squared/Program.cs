namespace Squared {
#if WINDOWS || XBOX
	internal static class Program {

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		private static void Main(string[] args) {
			using (GameStart game = new GameStart())
			{
				game.Run();
			}
		}
	}
#endif
}