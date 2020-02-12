using System;
using System.IO;
using http_client.http;

namespace http_client
{
	// Class to run the HTTP Client
	public class HttpClientRunner
	{
		// Creation of fields to store the values for the client
		private string _host;
		private int _port;
		private string _route;
		private HttpMethod _method;
		private string? _body;
		private string? _contentType;

		// Static method to run a HTTP Client with the given parameters
		public static void Run(string host, int port, string filename, string command)
		{
			new HttpClientRunner(host, port, filename, command);
		}

		// Constructor to create the HTTP Client and wait for responses
		private HttpClientRunner(string host, int port, string filename, string command)
		{
			this._host = "";
			this._port = 0;
			this._route = "";
			this._method = 0;

			try
			{
				// Setting of the fields of the HTTP Client
				this._host = host;
				this._port = port;
				this._route = filename[0] == '/' ? filename : $"/{filename}";

				// Retrieve the type of method based on the provided command
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

				// If this command is a PUT command
				if (method == HttpMethod.PUT)
				{
					// Read the provided file
					this._body = _readFile(filename);
				}

				// Receive and print the response
				var response = _doRequest();
				Console.WriteLine(response);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		// Method to read the contents of the file
		private string _readFile(string filename)
		{
			string contents;
			try
			{
				contents = File.ReadAllText($"files/{filename}");
			}
			catch
			{
				throw new Exception("File does not exist in files/ directory");
			}
			return contents;
		}

		// Method to run a HTTP Client with no parameters
		public static void Run()
		{
			new HttpClientRunner();
		}

		// Default constructor to create a HTTP Client
		private HttpClientRunner()
		{
			// Initialize the fields to default values
			this._host = "";
			this._port = -1;
			this._route = "";
			this._method = HttpMethod.GET;
			this._body = null;
			this._contentType = null;

			// Do while the user wants to play another round
			bool playNextRound;
			do
			{
				playNextRound = _playRound();
			}
			while (playNextRound);
		}

		// Method to loop through user input and responses
		private bool _playRound()
		{
			// Try to get user input and print the response
			try
			{
				_getInput();
				var response = _doRequest();
				Console.WriteLine($"\n{response}\n");
			}
			// Catch any error that occurs
			catch
			{
				Console.WriteLine("An error occurred");
			}

			// Return if the user wants to run again
			return _askPlayNextRound();
		}

		// Method to get user input for the fields
		private void _getInput()
		{
			// Get the values for each field
			_host = _getHostInput();
			_port = _getPortInput();
			_route = _getRouteInput();
			_method = _getMethodInput();

			// If the method is a PUT command
			if (_method == HttpMethod.PUT)
			{
				// Get the content type and the body 
				_contentType = _getContentTypeInput();
				_body = _getBodyInput();
			}
			else
			{
				// Set the body to null
				_body = null;
			}
		}

		// Method to get the input for the host
		private string _getHostInput()
		{
			Console.Write("Enter host name (no '/' at end, use www. if necessary): ");
			return Console.ReadLine();
		}

		// Method to get the input for the port
		private int _getPortInput()
		{
			int port = 0;

			// Do while the user's input was not an integer
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

		// Method to get the input for the route
		private string _getRouteInput()
		{
			Console.Write("Enter route (starting with '/'): ");
			return Console.ReadLine();
		}

		// Method to get the input for the method
		private HttpMethod _getMethodInput()
		{
			HttpMethod? method = null;

			// Do while the user did not enter GET or PUT as the method
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

		// Method to get the input for the body
		private string _getBodyInput()
		{
			Console.Write("Enter request method body (one line ex. filename.ext or JSON): ");
			return Console.ReadLine();
		}

		// Method to get the input for the content type
		private string _getContentTypeInput()
		{
			Console.Write("Enter Content Type (ex. application/json or text/plain): ");
			return Console.ReadLine();
		}

		// Method to make a request and return the response
		private string _doRequest()
		{
			var response = HttpRequest.MakeHttpRequest(_host, _port, _route, _method, _body, _contentType);
			return response;
		}

		// Method to prompt the user if they would like to run again
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
