using GraphsDataManager.ConsoleSystems;

namespace GraphsDataManager.LogConversionSystems
{
	public static class LogConverterDatabase
	{
		public const string CONVERT_COMMAND = ConsoleUIDatabase.COMMAND_ARGUMENT_PREFIX + "c";
		public const string INVALID_CONVERT_COMMAND_MESSAGE = "Convert command has invalid arguments. Should be -c <numbers_of_logs_to_convers>. Example -c 1,3,2";
		public const string LOG_IDS_SEPARATOR = ",";
		public const string IS_NOT_AS_NUMBER_MESSAGE = "{0} is not a number";
	}
}