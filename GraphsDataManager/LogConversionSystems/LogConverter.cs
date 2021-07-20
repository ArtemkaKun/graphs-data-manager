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

			StartConversion();
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

		private void StartConversion ()
		{
			OutputResultsData(GetResultsData());
			Console.WriteLine("Conversion was done");
		}

		private Dictionary<string, List<double>> GetResultsData ()
		{
			return new ResultsDataCalculator(SelectedLogIDs).ProceedDataFromLogs();
		}

		private void OutputResultsData (Dictionary<string, List<double>> resultsData)
		{
			new OutputDataManager(resultsData).OutputResultsDataToFile();
		}
	}
}