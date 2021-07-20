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
		private Dictionary<string, List<double>> ResultsDataCollection = new();

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
			for (int selectedFileIDPointer = 0; selectedFileIDPointer < SelectedLogIDs.Length; selectedFileIDPointer++)
			{
				(string errorMessage, string pathToLog) = TryGetPathToLogFile(selectedFileIDPointer);

				if (errorMessage != null)
				{
					Console.WriteLine(errorMessage);
					continue;
				}

				List<LogData> logRecords = ReadLogData(pathToLog);
				string nameOfLogFile = Path.GetFileNameWithoutExtension(pathToLog);

				if (logRecords.Count == 0)
				{
					Console.WriteLine(LogConverterDatabase.EMPTY_LOG_MESSAGE, nameOfLogFile);
					continue;
				}

				Queue<List<double>> timeSliceFrameTimesMatrix = ProceedLogDataWithStep(logRecords, 1.0d);
				List<double> averageFPSCollection = CalculateAverageFPSForSlices(timeSliceFrameTimesMatrix);
				ResultsDataCollection.Add(nameOfLogFile, averageFPSCollection);
			}

			PrepareResultsToOutput();
			WriteResults();
			Console.WriteLine("Conversion was done");
		}

		private (string errorMessage, string pathToLog) TryGetPathToLogFile (int selectedFileIDPointer)
		{
			string errorMessage = null;
			string pathToLog = null;
			string selectedIDInStringForm = SelectedLogIDs[selectedFileIDPointer];

			if (int.TryParse(selectedIDInStringForm, out int selectedID) == false)
			{
				errorMessage = string.Format(LogConverterDatabase.IS_NOT_AS_NUMBER_MESSAGE, selectedIDInStringForm);
			}
			else
			{
				pathToLog = Program.FolderManager.GetLogPathByPositionIndex(selectedID);

				if (pathToLog == null)
				{
					errorMessage = string.Format(LogConverterDatabase.NO_LOG_WITH_THIS_ID_MESSAGE, selectedID);
				}
			}

			return (errorMessage, pathToLog);
		}

		private List<LogData> ReadLogData (string pathToLog)
		{
			using StreamReader reader = new(pathToLog);
			using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
			return csv.GetRecords<LogData>().ToList();
		}

		private Queue<List<double>> ProceedLogDataWithStep (List<LogData> logRecords, double step)
		{
			Queue<List<double>> timeSliceFrameTimesMatrix = new();
			List<double> frameTimesBuffer = new();
			double previousTime = 0.0d;
			double sliceTime = 0.0d;

			for (int recordPointer = 0; recordPointer < logRecords.Count; recordPointer++)
			{
				LogData currentLogData = logRecords[recordPointer];

				if (sliceTime >= step)
				{
					timeSliceFrameTimesMatrix.Enqueue(new List<double>(frameTimesBuffer));
					frameTimesBuffer.Clear();
					sliceTime -= step; //TODO or sliceTime = 0? What should I do in a situation when with previous frame sliceTime was 0.98 and with the next frame it already 1.05 and step is 1?
				}
				else
				{
					if (recordPointer != 0)
					{
						sliceTime += currentLogData.TimeInSeconds - previousTime;
					}

					frameTimesBuffer.Add(currentLogData.MsBetweenDisplayChange);
					previousTime = currentLogData.TimeInSeconds;
				}
			}

			if (frameTimesBuffer.Count > 0)
			{
				//TODO for example with the last data record sliceTime didn't be bigger or equal then timestap and counted frame times wasn't added to the matrix. Should I add them or leave?
				//TODO this can probably be annihilated with smaller time step
			}

			return timeSliceFrameTimesMatrix;
		}

		private List<double> CalculateAverageFPSForSlices (Queue<List<double>> timeSliceFrameTimesMatrix)
		{
			List<double> averageFPSCollection = new(timeSliceFrameTimesMatrix.Count);

			while (timeSliceFrameTimesMatrix.Count > 0)
			{
				averageFPSCollection.Add(1000.0d / timeSliceFrameTimesMatrix.Dequeue().Average()); //TODO average or median?
			}

			return averageFPSCollection;
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