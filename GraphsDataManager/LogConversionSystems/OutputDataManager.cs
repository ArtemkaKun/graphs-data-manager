using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace GraphsDataManager.LogConversionSystems
{
	public class OutputDataManager
	{
		private Dictionary<string, List<double>> ResultsDataCollection { get; set; }

		public OutputDataManager (Dictionary<string, List<double>> resultsDataCollection)
		{
			ResultsDataCollection = resultsDataCollection;
		}

		public void OutputResultsDataToFile ()
		{
			PrepareResultsToOutput();
			WriteResults();
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
			string pathToResultsFile = Path.Combine(Program.FolderManager.PathToDataDirectory, $"convert_{DateTime.Now.ToString("dd-MM-yy_hh-mm")}.csv");
			using StreamWriter writerStream = new(pathToResultsFile, false);
			using CsvWriter OutputCSVWriter = new(writerStream, CultureInfo.InvariantCulture);

			WriteFirstDataLine(ResultsDataCollection.First().Value, OutputCSVWriter);

			foreach ((string logFileName, List<double> value) in ResultsDataCollection)
			{
				WriteSecondDataLine(value, OutputCSVWriter, logFileName);
			}
		}

		private void WriteFirstDataLine (List<double> averageFPSCollection, CsvWriter csv)
		{
			InsertEmptyColumns(csv, 3);
			InsertTimeSteps(averageFPSCollection, csv);
		}

		private void InsertTimeSteps (List<double> averageFPSCollection, CsvWriter csv)
		{
			double stepNumber = 0.0d;

			for (int fpsFPSNumberPointer = 0; fpsFPSNumberPointer < averageFPSCollection.Count; fpsFPSNumberPointer++)
			{
				stepNumber += LogConverterDatabase.LOG_PROCEED_STEP;
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

		private void InsertEmptyColumns (CsvWriter csv, int countOfEmptyColumns)
		{
			for (int emptyColumnPointer = 0; emptyColumnPointer < countOfEmptyColumns; emptyColumnPointer++)
			{
				csv.WriteField(null);
			}
		}
	}
}