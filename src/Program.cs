using System;
using http_client.http;

namespace http_client
{
	class Program
	{
		static void Main(string[] args)
		{
			// var res = HttpRequest.MakeHttpRequest("www.google.com", "/", HttpMethod.GET, null);
			var res = HttpRequest.MakeHttpRequest("jsonplaceholder.typicode.com", "/posts", HttpMethod.GET, null);
			Console.WriteLine(res);

			Console.WriteLine("\nPress enter to exit...");
			Console.Read();
		}
	}
}
