using System;
using System.IO;
using GraphsDataManager.Helpers;

namespace GraphsDataManager.DataFolderSystems
{
	public class DataFolderManager
	{
		public string PathToDataDirectory { get; private set; }

		private string[] LogPaths { get; set; }

		public void TryGetDataFromFolder (string[] arguments)
		{
			string errorMessage = TryGetPathToDataDirectory(arguments);

			if (errorMessage != null)
			{
				Console.WriteLine(errorMessage);
				return;
			}

			GetAllLogFilesInfo();
			PrintAllLogsFilesInFolder();
		}

		public string GetLogPathByPositionIndex (int index)
		{
			return CheckIsLogPathPositionIndexIsValid(index) == true ? LogPaths[index] : null;
		}

		private string TryGetPathToDataDirectory (string[] arguments)
		{
			string errorMessage = null;

			if (arguments.Length < 2)
			{
				errorMessage = DataFolderManagerDatabase.INVALID_DATA_FOLDER_COMMAND_ARGUMENTS;
			}
			else
			{
				PathToDataDirectory = arguments[1].Trim(DataFolderManagerDatabase.PATH_WRAPPER_SYMBOL);

				if (ValidatePathToFolder(PathToDataDirectory) == false)
				{
					errorMessage = DataFolderManagerDatabase.INVALID_FOLDER_PATH;
				}
			}

			return errorMessage;
		}

		private bool ValidatePathToFolder (string pathToFolder)
		{
			return (pathToFolder.CheckIfStringIsValid() == true) && Directory.Exists(pathToFolder);
		}

		private void GetAllLogFilesInfo ()
		{
			string[] allLogPathsInFolder = Directory.GetFiles(PathToDataDirectory, DataFolderManagerDatabase.SEARCH_FILE_PREFIX + DataFolderManagerDatabase.LOG_DATA_EXTENSION);
			LogPaths = new string [allLogPathsInFolder.Length];

			for (int filePathPointer = 0; filePathPointer < allLogPathsInFolder.Length; filePathPointer++)
			{
				LogPaths[filePathPointer] = allLogPathsInFolder[filePathPointer];
			}
		}

		private void PrintAllLogsFilesInFolder ()
		{
			for (int fileInfoPointer = 0; fileInfoPointer < LogPaths.Length; fileInfoPointer++)
			{
				Console.WriteLine(DataFolderManagerDatabase.LOG_FILES_TABLE_ROW_TEMPLATE, fileInfoPointer, Path.GetFileNameWithoutExtension(LogPaths[fileInfoPointer]));
			}
		}

		private bool CheckIsLogPathPositionIndexIsValid (int index)
		{
			return (index >= 0) && (index < LogPaths.Length);
		}
	}
}