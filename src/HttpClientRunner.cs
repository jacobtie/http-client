using System;
using System.IO;
using http_client.http;

namespace http_client
{
	public class HttpClientRunner
	{
		private string _host;
		private int _port;
		private string _route;
		private HttpMethod _method;
		private string? _body;
		private string? _contentType;

		public static void Run(string host, int port, string filename, string command)
		{
			new HttpClientRunner(host, port, filename, command);
		}

		private HttpClientRunner(string host, int port, string filename, string command)
		{
			this._host = host;
			this._port = port;
			this._route = filename[0] == '/' ? filename : $"/{filename}";
			HttpMethod method;
			if (command.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
			{
				method = HttpMethod.GET;
			}
			else if (command.Equals("PUT", StringComparison.InvariantCultureIgnoreCase))
			{
				method = HttpMethod.PUT;
			}
			else
			{
				throw new ArgumentException("Invalid HTTP method");
			}

			this._method = method;

			if (method == HttpMethod.PUT)
			{
				this._body = _readFile(filename);
			}

			var response = _doRequest();

			Console.WriteLine(response);
		}

		private string _readFile(string filename)
		{
			var fileLines = File.ReadAllLines($"files/{filename}");
			var contents = string.Join("\n", fileLines);

			return contents;
		}

		public static void Run()
		{
			new HttpClientRunner();
		}

		private HttpClientRunner()
		{
			this._host = "";
			this._port = -1;
			this._route = "";
			this._method = HttpMethod.GET;
			this._body = null;
			this._contentType = null;

			bool playNextRound;
			do
			{
				playNextRound = _playRound();
			}
			while (playNextRound);
		}

		private bool _playRound()
		{
			try
			{
				_getInput();
				var response = _doRequest();
				Console.WriteLine($"\n{response}\n");
			}
			catch
			{
				Console.WriteLine("An error occurred");
			}
			return _askPlayNextRound();
		}

		private void _getInput()
		{
			_host = _getHostInput();
			_port = _getPortInput();
			_route = _getRouteInput();
			_method = _getMethodInput();
			if (_method == HttpMethod.PUT)
			{
				_contentType = _getContentTypeInput();
				_body = _getBodyInput();
			}
			else
			{
				_body = null;
			}
		}

		private string _getHostInput()
		{
			Console.Write("Enter host name (no '/' at end, use www. if necessary): ");
			return Console.ReadLine();
		}

		private int _getPortInput()
		{
			int port = 0;

			do
			{
				Console.Write("Enter port number (ex. 80, 5000, etc): ");
				string portAsString = Console.ReadLine();
				if (!Int32.TryParse(portAsString, out port) || port <= 0)
				{
					Console.WriteLine("Invalid port number");
				}
			}
			while (port <= 0);

			return port;
		}

		private string _getRouteInput()
		{
			Console.Write("Enter route (starting with '/'): ");
			return Console.ReadLine();
		}

		private HttpMethod _getMethodInput()
		{
			HttpMethod? method = null;
			do
			{
				string methodRaw;

				Console.Write("Enter HTTP Method (Either GET or PUT): ");
				methodRaw = Console.ReadLine();

				if (methodRaw.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
				{
					method = HttpMethod.GET;
				}
				else if (methodRaw.Equals("PUT", StringComparison.InvariantCultureIgnoreCase))
				{
					method = HttpMethod.PUT;
				}
				else
				{
					Console.WriteLine("Invalid HTTP method");
				}
			}
			while (method == null);

			return (HttpMethod)method;
		}

		private string _getBodyInput()
		{
			Console.Write("Enter request method body (one line ex. filename.ext or JSON): ");
			return Console.ReadLine();
		}

		private string _getContentTypeInput()
		{
			Console.Write("Enter Content Type (ex. application/json or text/plain): ");
			return Console.ReadLine();
		}

		private string _doRequest()
		{
			var response = HttpRequest.MakeHttpRequest(_host, _port, _route, _method, _body, _contentType);
			return response;
		}

		private bool _askPlayNextRound()
		{
			Console.Write("Do you want to run again? Enter 'yes' to run the system again, enter anything else to quit: ");
			var answer = Console.ReadLine();

			bool playAgain = false;
			if (answer.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
			{
				playAgain = true;
			}

			return playAgain;
		}
	}
}
