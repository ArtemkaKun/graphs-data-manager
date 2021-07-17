using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace GraphsDataManager
{
	public class TestResultsMaintainer
	{
		private string PathToDataDirectory { get; set; }
		private FileInfo[] LogFilesInfo { get; set; }

		public void SetPathToDataDirectory (string newPathToData, FileInfo[] logFilesInfo)
		{
			PathToDataDirectory = newPathToData;
			LogFilesInfo = logFilesInfo;
		}
	
		public void StartConversion (string[] selectedFileIDs)
		{
			//TODO add check for data directory path and files info
			
			for (int selectedFileIDPointer = 0; selectedFileIDPointer < selectedFileIDs.Length; selectedFileIDPointer++)
			{
				string selectedIDInStringForm = selectedFileIDs[selectedFileIDPointer];

				if ((int.TryParse(selectedIDInStringForm, out int selectedID) == true) && (CheckIsSelectedIDValid(selectedID) == true))
				{
					List<LogData> logRecords = ReadLogData(LogFilesInfo[selectedID].FullName);

					if (logRecords.Count == 0)
					{
						//TODO empty table message
						return;
					}

					Queue<List<double>> timeSliceFrameTimesMatrix = ProceedLogDataWithStep(logRecords, 1.0d);
					Queue<double> averageFPSCollection = CalculateAverageFPSForSlices(timeSliceFrameTimesMatrix);
					WriteResults(PathToDataDirectory, averageFPSCollection);

					Console.WriteLine("Conversion was done");
				}
				else
				{
					//TODO add invalid id message
					continue;
				}
			}
		}

		private bool CheckIsSelectedIDValid (int selectedID)
		{
			return (selectedID >= 0) && (selectedID < LogFilesInfo.Length);
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
			using StreamWriter writer = new(pathToResultsFile, true);
			using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
			WriteFirstDataLine(averageFPSCollection, csv);
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