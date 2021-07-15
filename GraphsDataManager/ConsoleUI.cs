using System;

namespace GraphsDataManager
{
	public class ConsoleUI
	{
		private const string WELCOME_MESSAGE = "Programm was started successfully. Waiting for user commands.";
		//TODO DICTIONARY COMMAND-Action
		//TODO Create Command attribute to mark command methods
		//TODO create description attribute for commands
		//TODO create HelpClass object
		
		public void Start ()
		{
			Console.WriteLine(WELCOME_MESSAGE);
			
			while (true)
			{
				//TODO implement commands handling
			}
		}
	}
}