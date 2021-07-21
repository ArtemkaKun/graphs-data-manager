using System;
using System.Collections.Generic;
using System.Linq;
using GraphsDataManager.Helpers;
using GraphsDataManager.LogConversionSystems.ResultsDataCalculationSystems;

namespace GraphsDataManager.LogConversionSystems
{
	public class LogConverter
	{
		private string[] SelectedLogIDs { get; set; }

		public void TryConvertLogsIntoResults (string[] arguments)
		{
			if (Program.FolderManager.PathToDataDirectory.CheckIfStringIsValid() == false)
			{
				Console.WriteLine(LogConverterDatabase.INVALID_DATA_ABOUT_DATA_FOLDER_MESSAGE);
				return;
			}

			string errorMessage = TryGetSelectedIDs(arguments);

			if (errorMessage != null)
			{
				Console.WriteLine(errorMessage);
				return;
			}

			(string versusModeErrorMessage, bool isVersusModeActive) = TryGetVersusModeState(arguments);

			if (versusModeErrorMessage != null)
			{
				Console.WriteLine(versusModeErrorMessage);
				return;
			}

			StartConversion(isVersusModeActive);
		}

		private string TryGetSelectedIDs (string[] arguments)
		{
			string errorMessage = null;

			if (arguments.Length < 2)
			{
				errorMessage = LogConverterDatabase.INVALID_CONVERT_COMMAND_MESSAGE;
			}
			else
			{
				string selectedFiles = arguments[1];

				if (selectedFiles.CheckIfStringIsValid() == false)
				{
					errorMessage = LogConverterDatabase.INVALID_CONVERT_COMMAND_MESSAGE;
				}

				SelectedLogIDs = selectedFiles.Split(LogConverterDatabase.LOG_IDS_SEPARATOR);
			}

			return errorMessage;
		}

		private (string, bool) TryGetVersusModeState (string[] arguments)
		{
			if (arguments.Length < 3)
			{
				return (null, false);
			}

			if (arguments[2] != LogConverterDatabase.VERSUS_COMMAND)
			{
				return (string.Format(LogConverterDatabase.UNKNOWN_ARGUMENT_MESSAGE, arguments[2]), false);
			}

			return SelectedLogIDs.Length == 2 ? (null, true) : (LogConverterDatabase.INVALID_COUNT_OF_LOGS_TO_COMPARE, false);
		}

		private void StartConversion (bool isVersusModeActive)
		{
			Dictionary<string, List<double>> resultsDataCollection = GetResultsData();
			List<double> comparisonDataSequence = null;

			if (isVersusModeActive == true)
			{
				comparisonDataSequence = CompareFirstResultsToSecond(resultsDataCollection);
			}

			OutputResultsData(resultsDataCollection, comparisonDataSequence);
			Console.WriteLine("Conversion was done");
		}

		private List<double> CompareFirstResultsToSecond (Dictionary<string, List<double>> resultsDataCollection)
		{
			List<double> firstDataSequence = resultsDataCollection.First().Value;
			List<double> secondDataSequence = resultsDataCollection.Last().Value;
			List<double> comparisonDataSequence = new(firstDataSequence.Count);

			for (int resultDataPointer = 0; resultDataPointer < firstDataSequence.Count; resultDataPointer++)
			{
				comparisonDataSequence.Add(((secondDataSequence[resultDataPointer] * 100.0d) / firstDataSequence[resultDataPointer]) - 100.0d);
			}

			return comparisonDataSequence;
		}

		private Dictionary<string, List<double>> GetResultsData ()
		{
			return new ResultsDataCalculator(SelectedLogIDs).ProceedDataFromLogs();
		}

		private void OutputResultsData (Dictionary<string, List<double>> resultsData, List<double> comparisonDataSequence)
		{
			new OutputDataManager(resultsData, comparisonDataSequence).OutputResultsDataToFile();
		}
	}
}