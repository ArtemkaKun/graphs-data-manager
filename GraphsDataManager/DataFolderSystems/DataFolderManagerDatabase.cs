using GraphsDataManager.ConsoleSystems;

namespace GraphsDataManager.DataFolderSystems
{
	public static class DataFolderManagerDatabase
	{
		public const string DATA_FOLDER_COMMAND = ConsoleUIDatabase.COMMAND_ARGUMENT_PREFIX + "df";
		public const string SEARCH_FILE_PREFIX = "*.";
		public const string LOG_DATA_EXTENSION = "csv";
		public const string LOG_FILES_TABLE_ROW_TEMPLATE = "{0} - {1}";
		public const string INVALID_DATA_FOLDER_COMMAND_ARGUMENTS = "Invalid -df command arguments. Should be -df <path_to_folder>. Example -df C:\\MyLogs";
		public const string INVALID_FOLDER_PATH = "Invalid path to folder";
		public const char PATH_WRAPPER_SYMBOL = '"';
	}
}