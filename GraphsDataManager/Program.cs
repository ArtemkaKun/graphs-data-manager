namespace GraphsDataManager
{
	internal static class Program
	{
		public static TestResultsMaintainer ResultsMaintainer { get; } = new();
		
		private static ConsoleUI UI { get; } = new();

		private static void Main ()
		{
			UI.Start();
		}
	}
}