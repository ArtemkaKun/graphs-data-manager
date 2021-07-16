﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace GraphsDataManager
{
	public class TestResultsMaintainer
	{
		public void StartConversion (string pathToLog, string pathToStoreResult)
		{
			List<LogData> logRecords = ReadLogData(pathToLog);

			if (logRecords.Count == 0)
			{
				//TODO empty table message
				return;
			}

			Queue<List<double>> timeSliceFrameTimesMatrix = ProceedLogDataWithStep(logRecords, 1.0d);
			Queue<double> averageFPSCollection = CalculateAverageFPSForSlices(timeSliceFrameTimesMatrix);
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
				averageFPSCollection.Enqueue(1.0d / timeSliceFrameTimesMatrix.Dequeue().Average()); //TODO average or median?
			}

			return averageFPSCollection;
		}
	}
}