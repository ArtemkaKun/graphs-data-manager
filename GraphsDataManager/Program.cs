namespace GraphsDataManager
{
	internal static class Program
	{
		private static ConsoleUI UI { get; } = new();
		private static TestResultsMaintainer ResultsMaintainer { get; } = new();

		private static void Main ()
		{
			UI.Start();
		}
	}
}