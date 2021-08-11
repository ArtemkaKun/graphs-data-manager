namespace GraphsDataManager.ConsoleSystems
{
	public static class ConsoleUIDatabase
	{
		public const string WELCOME_MESSAGE = "Programm was started successfully. Waiting for user commands.\n\nAvailable commands:\n\n"
		  + "-df 'PATH_TO_FOLDER' -> set path to folder with data to convert. Should be used as first command every time you launch the app. Usage example -df C:\\MyData\n\n"
		  + "-c 'IDS_OF_FILES_TO_CONVERT' -> convert one or more files to results. You will get IDs after providing data folder with -df command. Usage example -c 1,3,2\n\n"
		  + "-vs -> additional sub command for -c command. Will convert provided files to results and compare these results. Supports only 2 files. Usage example -c 1,2 -vs";
		public const string COMMAND_ARGUMENT_PREFIX = "-";
		public const string COMMAND_ARGUMENTS_SEPARATOR = " ";
		public const string UNKNOWN_COMMAND_MESSAGE = "Unknown command was provided";
	}
}