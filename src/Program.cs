using System;

namespace http_client
{
	class Program
	{
		// Parses arguments to be sent to the HTTP client
		static void Main(string[] args)
		{
			// If the 4 arguments were not given
			if (args.Length == 4)
			{
				// Try to store the port from the second argument
				int port;
				if (Int32.TryParse(args[1], out port))
				{
					// Get the other requirements from the arguments
					var host = args[0];
					var command = args[2];
					var filename = args[3];

					// Run the HTTP client
					HttpClientRunner.Run(host, port, filename, command);
				}
				else
				{
					Console.WriteLine("Port is not valid");
				}
			}
			else if (args.Length == 0)
			{
				HttpClientRunner.Run();
			}
			else
			{
				Console.WriteLine("Incorrect number of arguments");
			}
		}
	}
}
