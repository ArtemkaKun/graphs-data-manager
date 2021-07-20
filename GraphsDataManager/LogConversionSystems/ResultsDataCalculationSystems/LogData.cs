using System.Collections.Generic;

namespace GraphsDataManager.LogConversionSystems.ResultsDataCalculationSystems
{
	public class LogData
	{
		public string LogFileName { get; }
		public List<LogDataRecord> LogRecords { get; }

		public LogData (string logFileName, List<LogDataRecord> logRecords)
		{
			LogFileName = logFileName;
			LogRecords = logRecords;
		}
	}
}