using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace GraphsDataManager.LogConversionSystems.ResultsDataCalculationSystems
{
	public class LogParser
	{
		public (string errorMessage, LogData logData) TryGetLogData (string selectedIDInStringForm)
		{
			LogData logData = null;
			(string errorMessage, string pathToLog) = TryGetPathToLogFile(selectedIDInStringForm);

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

		private (string errorMessage, string pathToLog) TryGetPathToLogFile (string selectedIDInStringForm)
		{
			string errorMessage = null;
			string pathToLog = null;

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
	}
}