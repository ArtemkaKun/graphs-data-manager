using GraphsDataManager.ConsoleSystems;

namespace GraphsDataManager.LogConversionSystems
{
	public static class LogConverterDatabase
	{
		public const string CONVERT_COMMAND = ConsoleUIDatabase.COMMAND_ARGUMENT_PREFIX + "c";
		public const string INVALID_CONVERT_COMMAND_MESSAGE = "Convert command has invalid arguments. Should be -c <numbers_of_logs_to_convers>. Example -c 1,3,2";
		public const string LOG_IDS_SEPARATOR = ",";
		public const string VERSUS_COMMAND = ConsoleUIDatabase.COMMAND_ARGUMENT_PREFIX + "vs";
		public const string UNKNOWN_ARGUMENT_MESSAGE = "Unknown argument {0}";
		public const string INVALID_COUNT_OF_LOGS_TO_COMPARE = "Invalid count of logs to compare. Should be 2";
		public const double LOG_PROCEED_STEP = 1.0d;
	}
}