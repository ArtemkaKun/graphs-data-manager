using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphsDataManager.LogConversionSystems.ResultsDataCalculationSystems
{
	public class ResultsDataCalculator
	{
		private LogParser ParserTool { get; } = new();
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
				ProceedLog(SelectedLogIDs[selectedFileIDPointer]);
			}

			return ResultsDataCollection;
		}

		private void ProceedLog (string selectedIDInStringForm)
		{
			(string errorMessage, LogData logData) = ParserTool.TryGetLogData(selectedIDInStringForm);

			if (errorMessage != null)
			{
				Console.WriteLine(errorMessage);
				return;
			}

			ResultsDataCollection.Add(logData.LogFileName, CalculateAverageFPSWithStep(logData));
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
					sliceTime -= step;
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

			return timeSliceFrameTimesMatrix;
		}

		private List<double> CalculateAverageFPSForSlices (Queue<List<double>> timeSliceFrameTimesMatrix)
		{
			List<double> averageFPSCollection = new(timeSliceFrameTimesMatrix.Count);

			while (timeSliceFrameTimesMatrix.Count > 0)
			{
				averageFPSCollection.Add(1000.0d / timeSliceFrameTimesMatrix.Dequeue().Average());
			}

			return averageFPSCollection;
		}
	}
}