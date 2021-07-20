using CsvHelper.Configuration.Attributes;

namespace GraphsDataManager.LogConversionSystems.ResultsDataCalculationSystems
{
	public class LogDataRecord
	{
		[Name("TimeInSeconds")]
		public double TimeInSeconds { get; set; }
		[Name("MsBetweenDisplayChange")]
		public double MsBetweenDisplayChange { get; set; }
	}
}