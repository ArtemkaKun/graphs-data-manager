using GraphsDataManager.ConsoleSystems;
using GraphsDataManager.DataFolderSystems;
using GraphsDataManager.LogConversionSystems;

namespace GraphsDataManager
{
	internal static class Program
	{
		public static LogConverter ResultsMaintainer { get; } = new();
		public static DataFolderManager FolderManager { get; } = new();
		
		private static ConsoleUI UI { get; } = new();

		private static void Main ()
		{
			UI.Start();
		}
	}
}