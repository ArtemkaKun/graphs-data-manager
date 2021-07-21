using System;
using System.Collections.Generic;
using GraphsDataManager.Helpers;
using GraphsDataManager.LogConversionSystems.ResultsDataCalculationSystems;

namespace GraphsDataManager.LogConversionSystems
{
	public class LogConverter
	{
		private string[] SelectedLogIDs { get; set; }

		public void TryConvertLogsIntoResults (string[] arguments)
		{
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

			if (arguments[2] != "-vs")
			{
				return ($"Unknown argument {arguments[2]}", false);
			}

			return SelectedLogIDs.Length == 2 ? (null, true) : ("Invalid count of logs to compare. Should be 2", false);
		}

		private void StartConversion (bool isVersusModeActive)
		{
			OutputResultsData(GetResultsData(), isVersusModeActive);
			Console.WriteLine("Conversion was done");
		}

		private Dictionary<string, List<double>> GetResultsData ()
		{
			return new ResultsDataCalculator(SelectedLogIDs).ProceedDataFromLogs();
		}

		private void OutputResultsData (Dictionary<string, List<double>> resultsData, bool isVersusModeActive)
		{
			new OutputDataManager(resultsData, isVersusModeActive).OutputResultsDataToFile();
		}
	}
}