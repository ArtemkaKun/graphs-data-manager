using System;
using System.Collections.Generic;
using System.IO;

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
				{ConsoleUIDatabase.DATA_FOLDER_COMMAND, TryGetDataFromFolder},
				{ConsoleUIDatabase.CONVERT_COMMAND, TryConvertLogsIntoResults}
			};
		}

		public void Start ()
		{
			Console.WriteLine(ConsoleUIDatabase.WELCOME_MESSAGE);

			while (true)
			{
				string inputString = Console.ReadLine();
				string[] commandArgument = SplitStringOnArguments(inputString);

				if (IsCommandArgumentsValid(commandArgument) == false)
				{
					Console.WriteLine(ConsoleUIDatabase.UNKNOWN_COMMAND_MESSAGE);
					continue;
				}

				CommandActionMap[commandArgument[0]].Invoke(commandArgument);
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

		private void TryGetDataFromFolder (string[] arguments)
		{
			if (arguments.Length < 2)
			{
				//TODO invalid commands arguments message
				return;
			}

			string dataFolderPath = arguments[1].Trim('"');

			if (ValidatePathToFolder(dataFolderPath) == false)
			{
				//TODO invalig path to data folder message
				return;
			}

			FileInfo[] filesInfo = GetAllLogFilesInfo(dataFolderPath);
			PrintAllLogsFilesInFolder(filesInfo);
			Program.ResultsMaintainer.SetPathToDataDirectory(dataFolderPath, filesInfo);
		}

		private FileInfo[] GetAllLogFilesInfo (string pathToFolder)
		{
			string[] allLogPathsInFolder = Directory.GetFiles(pathToFolder, ConsoleUIDatabase.SEARCH_FILE_PREFIX + ConsoleUIDatabase.LOG_DATA_EXTENSION);
			FileInfo[] filesInfo = new FileInfo[allLogPathsInFolder.Length];

			for (int filePathPointer = 0; filePathPointer < allLogPathsInFolder.Length; filePathPointer++)
			{
				filesInfo[filePathPointer] = new FileInfo(allLogPathsInFolder[filePathPointer]);
			}

			return filesInfo;
		}

		private void PrintAllLogsFilesInFolder (FileInfo[] filesInfo)
		{
			for (int fileInfoPointer = 0; fileInfoPointer < filesInfo.Length; fileInfoPointer++)
			{
				Console.WriteLine(ConsoleUIDatabase.LOG_FILES_TABLE_ROW_TEMPLATE, fileInfoPointer, filesInfo[fileInfoPointer].Name);
			}
		}

		private void TryConvertLogsIntoResults (string[] arguments)
		{
			if (arguments.Length < 2)
			{
				//TODO invalid commands arguments message
				return;
			}

			string selectedFiles = arguments[1];

			if (IsStringValid(selectedFiles) == false)
			{
				//TODO invalig selected files
				return;
			}

			string[] selectedFileIDs = selectedFiles.Split(",");
			Program.ResultsMaintainer.StartConversion(selectedFileIDs);
		}

		private bool ValidatePathToFolder (string pathToFolder)
		{
			return (IsStringValid(pathToFolder) == true) && Directory.Exists(pathToFolder);
		}

		private bool IsStringValid (string stringToValidate)
		{
			return (string.IsNullOrEmpty(stringToValidate) == false) && (string.IsNullOrWhiteSpace(stringToValidate) == false);
		}
	}
}