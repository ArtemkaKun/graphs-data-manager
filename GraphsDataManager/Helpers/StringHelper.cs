namespace GraphsDataManager.Helpers
{
	public static class StringHelper
	{
		public static bool CheckIfStringIsValid (this string stringToValidate)
		{
			return (string.IsNullOrEmpty(stringToValidate) == false) && (string.IsNullOrWhiteSpace(stringToValidate) == false);
		}
	}
}