using System;
using http_client.http;

namespace http_client
{
	class Program
	{
		static void Main(string[] args)
		{
			HttpClientRunner.Run();

			Console.WriteLine("\nPress enter to exit...");
			Console.Read();
		}
	}
}
