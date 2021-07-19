namespace GraphsDataManager.ConsoleSystems
{
	public static class ConsoleUIDatabase
	{
		public const string WELCOME_MESSAGE = "Programm was started successfully. Waiting for user commands.";
		public const string COMMAND_ARGUMENT_PREFIX = "-";
		public const string CONVERT_COMMAND = COMMAND_ARGUMENT_PREFIX + "c";
		public const string DATA_FOLDER_COMMAND = COMMAND_ARGUMENT_PREFIX + "df";
		public const string SEARCH_FILE_PREFIX = "*.";
		public const string LOG_DATA_EXTENSION = "csv";
		public const string LOG_FILES_TABLE_ROW_TEMPLATE = "{0} - {1}";
		public const string COMMAND_ARGUMENTS_SEPARATOR = " ";
		public const string UNKNOWN_COMMAND_MESSAGE = "Unknown command was provided";
		public const string INVALID_DATA_FOLDER_COMMAND_ARGUMENTS = "Invalid -df command arguments. Should be -df <path_to_folder>. Example -df C:\\MyLogs";
		public const char PATH_WRAPPER_SYMBOL = '"';
	}
}