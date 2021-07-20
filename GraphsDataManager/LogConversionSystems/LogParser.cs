using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using GraphsDataManager.LogConversionSystems;

namespace GraphsDataManager
{
	public class LogParser
	{
		private string[] SelectedLogIDs { get; set; }

		public LogParser (string[] selectedLogIDs)
		{
			SelectedLogIDs = selectedLogIDs;
		}

		public Dictionary<string, List<double>> ProceedDataFromLogs ()
		{
			Dictionary<string, List<double>> resultsDataCollection = new(SelectedLogIDs.Length);

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
				resultsDataCollection.Add(nameOfLogFile, averageFPSCollection);
			}

			return resultsDataCollection;
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
	}
}