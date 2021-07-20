﻿using GraphsDataManager.ConsoleSystems;

namespace GraphsDataManager.LogConversionSystems
{
	public static class LogConverterDatabase
	{
		public const string CONVERT_COMMAND = ConsoleUIDatabase.COMMAND_ARGUMENT_PREFIX + "c";
		public const string INVALID_CONVERT_COMMAND_MESSAGE = "Convert command has invalid arguments. Should be -c <numbers_of_logs_to_convers>. Example -c 1,3,2";
	}
}