using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace GraphsDataManager.LogConversionSystems.ResultsDataCalculationSystems
{
	public class ResultsDataCalculator
	{
		private string[] SelectedLogIDs { get; set; }
		private Dictionary<string, List<double>> ResultsDataCollection { get; set; }

		public ResultsDataCalculator (string[] selectedLogIDs)
		{
			SelectedLogIDs = selectedLogIDs;
		}

		public Dictionary<string, List<double>> ProceedDataFromLogs ()
		{
			ResultsDataCollection = new Dictionary<string, List<double>>(SelectedLogIDs.Length);

			for (int selectedFileIDPointer = 0; selectedFileIDPointer < SelectedLogIDs.Length; selectedFileIDPointer++)
			{
				ProceedLog(selectedFileIDPointer);
			}

			return ResultsDataCollection;
		}

		private void ProceedLog (int selectedFileIDPointer)
		{
			(string errorMessage, LogData logData) = TryGetLogData(selectedFileIDPointer);

			if (errorMessage != null)
			{
				Console.WriteLine(errorMessage);
				return;
			}

			ResultsDataCollection.Add(logData.LogFileName, CalculateAverageFPSWithStep(logData));
		}

		private (string errorMessage, LogData logData) TryGetLogData (int selectedFileIDPointer)
		{
			LogData logData = null;
			(string errorMessage, string pathToLog) = TryGetPathToLogFile(selectedFileIDPointer);

			if (errorMessage == null)
			{
				List<LogDataRecord> logRecords = ReadLogData(pathToLog);
				string nameOfLogFile = Path.GetFileNameWithoutExtension(pathToLog);

				if (logRecords.Count == 0)
				{
					errorMessage = string.Format(ResultsDataCalculatorDatabase.EMPTY_LOG_MESSAGE, nameOfLogFile);
				}
				else
				{
					logData = new LogData(nameOfLogFile, logRecords);
				}
			}

			return (errorMessage, logData);
		}

		private (string errorMessage, string pathToLog) TryGetPathToLogFile (int selectedFileIDPointer)
		{
			string errorMessage = null;
			string pathToLog = null;
			string selectedIDInStringForm = SelectedLogIDs[selectedFileIDPointer];

			if (int.TryParse(selectedIDInStringForm, out int selectedID) == false)
			{
				errorMessage = string.Format(ResultsDataCalculatorDatabase.IS_NOT_AS_NUMBER_MESSAGE, selectedIDInStringForm);
			}
			else
			{
				pathToLog = Program.FolderManager.GetLogPathByPositionIndex(selectedID);

				if (pathToLog == null)
				{
					errorMessage = string.Format(ResultsDataCalculatorDatabase.NO_LOG_WITH_THIS_ID_MESSAGE, selectedID);
				}
			}

			return (errorMessage, pathToLog);
		}

		private List<LogDataRecord> ReadLogData (string pathToLog)
		{
			using StreamReader reader = new(pathToLog);
			using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
			return csv.GetRecords<LogDataRecord>().ToList();
		}

		private List<double> CalculateAverageFPSWithStep (LogData logData)
		{
			Queue<List<double>> timeSliceFrameTimesMatrix = ProceedLogDataWithStep(logData.LogRecords, 1.0d);
			List<double> averageFPSCollection = CalculateAverageFPSForSlices(timeSliceFrameTimesMatrix);
			return averageFPSCollection;
		}

		private Queue<List<double>> ProceedLogDataWithStep (List<LogDataRecord> logRecords, double step)
		{
			Queue<List<double>> timeSliceFrameTimesMatrix = new();
			List<double> frameTimesBuffer = new();
			double previousTime = 0.0d;
			double sliceTime = 0.0d;

			for (int recordPointer = 0; recordPointer < logRecords.Count; recordPointer++)
			{
				LogDataRecord currentLogDataRecord = logRecords[recordPointer];

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
						sliceTime += currentLogDataRecord.TimeInSeconds - previousTime;
					}

					frameTimesBuffer.Add(currentLogDataRecord.MsBetweenDisplayChange);
					previousTime = currentLogDataRecord.TimeInSeconds;
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