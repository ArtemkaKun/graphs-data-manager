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
		private const string RESULTS_FILE_NAME_TEMPLATE = "convert_{0}.csv";
		private const string DD_MM_YY_HH_MM_DATETIME_FORMAT = "dd-MM-yy_hh-mm";

		private Dictionary<string, List<double>> ResultsDataCollection { get; set; }
		private List<double> ComparisonDataSequence { get; set; }

		public OutputDataManager (Dictionary<string, List<double>> resultsDataCollection, List<double> comparisonDataSequence)
		{
			ResultsDataCollection = resultsDataCollection;
			ComparisonDataSequence = comparisonDataSequence;
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
			string pathToResultsFile = Path.Combine(Program.FolderManager.PathToDataDirectory, string.Format(RESULTS_FILE_NAME_TEMPLATE, DateTime.Now.ToString(DD_MM_YY_HH_MM_DATETIME_FORMAT)));
			using StreamWriter writerStream = new(pathToResultsFile, false);
			using CsvWriter outputCsvWriter = new(writerStream, CultureInfo.InvariantCulture);

			WriteFirstDataLine(ResultsDataCollection.First().Value, outputCsvWriter);

			foreach ((string logFileName, List<double> value) in ResultsDataCollection)
			{
				WriteSecondDataLine(value, outputCsvWriter, logFileName);
			}

			if (ComparisonDataSequence != null)
			{
				WriteVersusLine(outputCsvWriter);
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

		private void WriteVersusLine (CsvWriter csv)
		{
			csv.NextRecord();
			InsertEmptyColumns(csv, 3);

			for (int comparisonRecordPointer = 0; comparisonRecordPointer < ComparisonDataSequence.Count; comparisonRecordPointer++)
			{
				csv.WriteField(ComparisonDataSequence[comparisonRecordPointer]);
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