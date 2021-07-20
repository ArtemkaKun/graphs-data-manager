using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using GraphsDataManager.Helpers;

namespace GraphsDataManager.LogConversionSystems
{
	public class LogConverter
	{
		private string[] SelectedLogIDs { get; set; }
		private Dictionary<string, List<double>> ResultsDataCollection { get; set; }

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
			GetResultsData();
			PrepareResultsToOutput();
			WriteResults();
			Console.WriteLine("Conversion was done");
		}

		private void GetResultsData ()
		{
			ResultsDataCollection = new LogParser(SelectedLogIDs).ProceedDataFromLogs();
		}

		private void PrepareResultsToOutput ()
		{
			int shortestListLength = ResultsDataCollection.Values.Min(resultsCollection => resultsCollection.Count);

			foreach ((string logFileName, List<double> value) in ResultsDataCollection)
			{
				while (value.Count > shortestListLength)
				{
					value.RemoveAt(value.Count - 1);
				}
			}
		}

		private void WriteResults ()
		{
			string pathToResultsFile = Path.Combine(Program.FolderManager.PathToDataDirectory, $"convert_{DateTime.Now.ToString("dd-MM-yy")}.csv");
			using StreamWriter writer = new(pathToResultsFile, false);
			using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

			WriteFirstDataLine(ResultsDataCollection.First().Value, csv);

			foreach ((string logFileName, List<double> value) in ResultsDataCollection)
			{
				WriteSecondDataLine(value, csv, logFileName);
			}
		}

		private void WriteFirstDataLine (List<double> averageFPSCollection, CsvWriter csv)
		{
			InsertEmptyColumns(csv, 3);
			InsertTimeSteps(averageFPSCollection, csv);
		}

		private void InsertEmptyColumns (CsvWriter csv, int countOfEmptyColumns)
		{
			for (int emptyColumnPointer = 0; emptyColumnPointer < countOfEmptyColumns; emptyColumnPointer++)
			{
				csv.WriteField(null);
			}
		}

		private void InsertTimeSteps (List<double> averageFPSCollection, CsvWriter csv)
		{
			double stepNumber = 0.0d;

			for (int fpsFPSNumberPointer = 0; fpsFPSNumberPointer < averageFPSCollection.Count; fpsFPSNumberPointer++)
			{
				stepNumber += 1.0d;
				csv.WriteField(stepNumber);
			}
		}

		private void WriteSecondDataLine (List<double> averageFPSCollection, CsvWriter csv, string resultsName)
		{
			csv.NextRecord();
			csv.WriteField(resultsName);
			InsertEmptyColumns(csv, 2);

			for (int avgFPSValuePointer = 0; avgFPSValuePointer < averageFPSCollection.Count; avgFPSValuePointer++)
			{
				csv.WriteField(averageFPSCollection[avgFPSValuePointer]);
			}
		}
	}
}