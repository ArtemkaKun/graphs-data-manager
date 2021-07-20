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
				//TODO invalid commands arguments message
			}
			else
			{
				string selectedFiles = arguments[1];

				if (selectedFiles.CheckIfStringIsValid() == false)
				{
					//TODO invalig selected files
				}

				SelectedLogIDs = selectedFiles.Split(",");
			}

			return errorMessage;
		}

		private void StartConversion ()
		{
			//TODO add check for data directory path and files info

			for (int selectedFileIDPointer = 0; selectedFileIDPointer < SelectedLogIDs.Length; selectedFileIDPointer++)
			{
				string selectedIDInStringForm = SelectedLogIDs[selectedFileIDPointer];

				if (int.TryParse(selectedIDInStringForm, out int selectedID) == false)
				{
					//TODO is not a number error
					continue;
				}

				string selectedLogFile = Program.FolderManager.GetLogPathByPositionIndex(selectedID);

				if (Program.FolderManager.GetLogPathByPositionIndex(selectedID) == null)
				{
					//TODO no logs with that id
					continue;
				}

				List<LogData> logRecords = ReadLogData(selectedLogFile);

				if (logRecords.Count == 0)
				{
					//TODO empty table message
					return;
				}

				Queue<List<double>> timeSliceFrameTimesMatrix = ProceedLogDataWithStep(logRecords, 1.0d);
				Queue<double> averageFPSCollection = CalculateAverageFPSForSlices(timeSliceFrameTimesMatrix);
				WriteResults(Program.FolderManager.PathToDataDirectory, averageFPSCollection);

				Console.WriteLine("Conversion was done");
			}
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

		private Queue<double> CalculateAverageFPSForSlices (Queue<List<double>> timeSliceFrameTimesMatrix)
		{
			Queue<double> averageFPSCollection = new(timeSliceFrameTimesMatrix.Count);

			while (timeSliceFrameTimesMatrix.Count > 0)
			{
				averageFPSCollection.Enqueue(1000.0d / timeSliceFrameTimesMatrix.Dequeue().Average()); //TODO average or median?
			}

			return averageFPSCollection;
		}

		private void WriteResults (string pathToStoreResult, Queue<double> averageFPSCollection)
		{
			string pathToResultsFile = Path.Combine(pathToStoreResult, $"convert_{DateTime.Now.ToString("dd-MM-yy")}.csv");
			bool isPathAlreadyExists = File.Exists(pathToResultsFile);
			using StreamWriter writer = new(pathToResultsFile, true);
			using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

			if (isPathAlreadyExists == false)
			{
				WriteFirstDataLine(averageFPSCollection, csv);
			}

			WriteSecondDataLine(averageFPSCollection, csv, pathToResultsFile);
		}

		private void WriteFirstDataLine (Queue<double> averageFPSCollection, CsvWriter csv)
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

		private void InsertTimeSteps (Queue<double> averageFPSCollection, CsvWriter csv)
		{
			double stepNumber = 0.0d;

			for (int fpsFPSNumberPointer = 0; fpsFPSNumberPointer < averageFPSCollection.Count; fpsFPSNumberPointer++)
			{
				stepNumber += 1.0d;
				csv.WriteField(stepNumber);
			}
		}

		private void WriteSecondDataLine (Queue<double> averageFPSCollection, CsvWriter csv, string resultsName)
		{
			csv.NextRecord();
			csv.WriteField(resultsName);
			InsertEmptyColumns(csv, 2);

			while (averageFPSCollection.Count > 0)
			{
				csv.WriteField(averageFPSCollection.Dequeue());
			}
		}
	}
}