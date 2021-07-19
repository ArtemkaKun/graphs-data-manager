using System;
using System.Collections.Generic;
using System.IO;
using GraphsDataManager.DataFolderSystems;
using GraphsDataManager.Helpers;

namespace GraphsDataManager.ConsoleSystems
{
	public class ConsoleUI
	{
		private Dictionary<string, Action<string[]>> CommandActionMap { get; }

		//TODO Create Command attribute to mark command methods
		//TODO create description attribute for commands
		//TODO create HelpClass object

		public ConsoleUI ()
		{
			CommandActionMap = new Dictionary<string, Action<string[]>>
			{
				{DataFolderManagerDatabase.DATA_FOLDER_COMMAND, Program.FolderManager.TryGetDataFromFolder},
				{ConsoleUIDatabase.CONVERT_COMMAND, TryConvertLogsIntoResults}
			};
		}

		public void Start ()
		{
			Console.WriteLine(ConsoleUIDatabase.WELCOME_MESSAGE);
			StartUILoop();
		}

		private void StartUILoop ()
		{
			while (true)
			{
				string[] commandArguments = GetProvidedCommandArguments();
				TryInvokeProvidedCommand(commandArguments);
			}
		}

		private string[] GetProvidedCommandArguments ()
		{
			string inputString = Console.ReadLine();
			string[] commandArguments = SplitStringOnArguments(inputString);

			return commandArguments;
		}

		private void TryInvokeProvidedCommand (string[] commandArguments)
		{
			if (IsCommandArgumentsValid(commandArguments) == true)
			{
				CommandActionMap[commandArguments[0]].Invoke(commandArguments);
			}
			else
			{
				Console.WriteLine(ConsoleUIDatabase.UNKNOWN_COMMAND_MESSAGE);
			}
		}

		private string[] SplitStringOnArguments (string stringToSplit)
		{
			return stringToSplit.Split(ConsoleUIDatabase.COMMAND_ARGUMENTS_SEPARATOR);
		}

		private bool IsCommandArgumentsValid (string[] arguments)
		{
			return (arguments.Length > 1) && arguments[0].Contains(ConsoleUIDatabase.COMMAND_ARGUMENT_PREFIX);
		}

		private void TryConvertLogsIntoResults (string[] arguments)
		{
			if (arguments.Length < 2)
			{
				//TODO invalid commands arguments message
				return;
			}

			string selectedFiles = arguments[1];

			if (selectedFiles.CheckIfStringIsValid() == false)
			{
				//TODO invalig selected files
				return;
			}

			string[] selectedFileIDs = selectedFiles.Split(",");
			Program.ResultsMaintainer.StartConversion(selectedFileIDs);
		}
	}
}