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
		private const string DATA_FOLDER_COMMAND = COMMAND_ARGUMENT_PREFIX + "df";
		private const string SEARCH_FILE_PREFIX = "*.";
		private const string LOG_DATA_EXTENSION = "csv";
		private const string LOG_FILES_TABLE_ROW_TEMPLATE = "{0} - {1}";

		private Dictionary<string, Action<string[]>> CommandActionMap { get; set; }

		//TODO Create Command attribute to mark command methods
		//TODO create description attribute for commands
		//TODO create HelpClass object

		public ConsoleUI ()
		{
			CommandActionMap = new Dictionary<string, Action<string[]>>
			{
				{CONVERT_COMMAND, TryConvertLogsIntoResults},
				{DATA_FOLDER_COMMAND, TryGetDataFromFolder}
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
			Program.ResultsMaintainer.SetPathToDataDirectory(dataFolderPath, filesInfo);
			PrintAllLogsFilesInFolder(filesInfo);
		}

		private bool ValidatePathToFolder (string pathToFolder)
		{
			return (IsStringValid(pathToFolder) == true) && Directory.Exists(pathToFolder);
		}

		private bool IsStringValid (string stringToValidate)
		{
			return (string.IsNullOrEmpty(stringToValidate) == false) && (string.IsNullOrWhiteSpace(stringToValidate) == false);
		}

		private FileInfo[] GetAllLogFilesInfo (string pathToFolder)
		{
			string[] allLogPathsInFolder = Directory.GetFiles(pathToFolder, SEARCH_FILE_PREFIX + LOG_DATA_EXTENSION);
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
				Console.WriteLine(LOG_FILES_TABLE_ROW_TEMPLATE, fileInfoPointer, filesInfo[fileInfoPointer].Name);
			}
		}
	}
}