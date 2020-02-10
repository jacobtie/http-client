using System;

namespace http_client
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 4)
			{
				Console.WriteLine("Incorrect number of arguments");
			}
			else
			{
				int port;
				if (Int32.TryParse(args[1], out port))
				{
					var host = args[0];
					var command = args[2];
					var filename = args[3];
					HttpClientRunner.Run(host, port, filename, command);
				}
				else
				{
					Console.WriteLine("Port is not valid");
				}
			}
		}
	}
}
