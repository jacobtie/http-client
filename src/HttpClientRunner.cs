using System;
using http_client.http;

namespace http_client
{
	public class HttpClientRunner
	{
		private string _host;
		private string _route;
		private HttpMethod _method;
		private string? _body;
		private string? _contentType;

		public static void Run()
		{
			new HttpClientRunner();
		}

		private HttpClientRunner()
		{
			this._host = "";
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
			_getInput();
			var response = _doRequest();
			Console.WriteLine($"\n{response}\n");
			return _askPlayNextRound();
		}

		private void _getInput()
		{
			_host = _getHostInput();
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

		private string _getRouteInput()
		{
			Console.Write("Enter route (starting with '/'): ");
			return Console.ReadLine();
		}

		private HttpMethod _getMethodInput()
		{
			string methodRaw;
			do
			{
				Console.Write("Enter HTTP Method (Either GET or PUT): ");
				methodRaw = Console.ReadLine();
			}
			while (!methodRaw.Equals("GET", StringComparison.InvariantCultureIgnoreCase)
				&& !methodRaw.Equals("PUT", StringComparison.InvariantCultureIgnoreCase));

			HttpMethod method;
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
				throw new Exception("HTTP Method not defined");
			}

			return method;
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
			var response = HttpRequest.MakeHttpRequest(_host, _route, _method, _body, _contentType);
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
