using System;
using System.Collections.Generic;
using System.IO;

namespace GraphsDataManager
{
	public class ConsoleUI
	{
		private const string WELCOME_MESSAGE = "Programm was started successfully. Waiting for user commands.";
		private const string COMMAND_ARGUMENT_PREFIX = "-";
		private const string CONVERT_COMMAND = COMMAND_ARGUMENT_PREFIX + "c";

		private Dictionary<string, Action<string[]>> CommandActionMap { get; set; }

		//TODO Create Command attribute to mark command methods
		//TODO create description attribute for commands
		//TODO create HelpClass object

		public ConsoleUI ()
		{
			CommandActionMap = new Dictionary<string, Action<string[]>>
			{
				{CONVERT_COMMAND, TryConvertLogsIntoResults}
			};
		}

		public void Start ()
		{
			Console.WriteLine(WELCOME_MESSAGE);

			while (true)
			{
				string inputString = Console.ReadLine();
				string[] commandArgument = SplitStringOnArguments(inputString);

				if (IsCommandArgumentsValid(commandArgument) == false)
				{
					//TODO return invalid command message
					continue;
				}

				CommandActionMap[commandArgument[0]].Invoke(commandArgument);
			}
		}

		private string[] SplitStringOnArguments (string stringToSplit)
		{
			return stringToSplit.Split(" ");
		}

		private bool IsCommandArgumentsValid (string[] arguments)
		{
			return (arguments.Length > 1) && arguments[0].Contains(COMMAND_ARGUMENT_PREFIX);
		}

		private void TryConvertLogsIntoResults (string[] arguments)
		{
			if (arguments.Length < 3)
			{
				//TODO invalid commands arguments message
				return;
			}

			string logsPath = arguments[1].Trim('"');

			if ((IsStringValid(logsPath) == false) || (ValidatePathToFile(logsPath) == false))
			{
				//TODO invalig path to log message
				return;
			}

			string resultsPath = arguments[2].Trim('"');

			if (IsStringValid(resultsPath) == false)
			{
				//TODO invalid path to store results message
				return;
			}

			Program.ResultsMaintainer.StartConversion(logsPath, resultsPath);
		}

		private bool IsStringValid (string stringToValidate)
		{
			return (string.IsNullOrEmpty(stringToValidate) == false) && (string.IsNullOrWhiteSpace(stringToValidate) == false);
		}

		private bool ValidatePathToFile (string pathToFile)
		{
			return File.Exists(pathToFile);
		}
	}
}