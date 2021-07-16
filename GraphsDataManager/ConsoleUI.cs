using System;
using System.Collections.Generic;

namespace GraphsDataManager
{
	public class ConsoleUI
	{
		private const string WELCOME_MESSAGE = "Programm was started successfully. Waiting for user commands.";
		private const string COMMAND_ARGUMENT_PREFIX = "--";

		private Dictionary<string, Action<string[]>> CommandActionMap { get; set; }
		
		//TODO Create Command attribute to mark command methods
		//TODO create description attribute for commands
		//TODO create HelpClass object

		public ConsoleUI ()
		{
			CommandActionMap = new Dictionary<string, Action<string[]>>
			{
				{"--convert", TryConvertLogsIntoResults}
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
					return;
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
			
		}
	}
}