using System.Collections.Generic;
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
		}

		private List<LogData> ReadLogData (string pathToLog)
		{
			using StreamReader reader = new(pathToLog);
			using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
			return csv.GetRecords<LogData>().ToList();
		}
	}
}