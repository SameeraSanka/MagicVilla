namespace MagicVilla_VillaAPI.Logger
{
	public class LoggerCustom : ILoggerCustom
	{
		public void Log(string message, string type)
		{
			if(type == "error")
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.WriteLine("Error - " + message);
				Console.ForegroundColor = ConsoleColor.Black;
			}
			else
			{
				if(type == "warning")
				{
					Console.BackgroundColor = ConsoleColor.Yellow;
					Console.WriteLine("Error - " + message);
					Console.ForegroundColor = ConsoleColor.Black;
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.Green;
					Console.WriteLine(message);
					Console.BackgroundColor = ConsoleColor.Black;
				}
			}
		}
	}
}
//colors dala lassna krala thiynne.
