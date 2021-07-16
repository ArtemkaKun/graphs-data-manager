using CsvHelper.Configuration.Attributes;

namespace GraphsDataManager
{
	public class LogData
	{
		[Name("TimeInSeconds")]
		public double TimeInSeconds { get; set; }
		[Name("MsBetweenDisplayChange")]
		public double MsBetweenDisplayChange { get; set; }
	}
}