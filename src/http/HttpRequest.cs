using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace http_client.http
{
	// Class to send a request to the server and get a response
	public class HttpRequest
	{
		// Creation of the fields for the request message
		private const int RESPONSE_BUFFER_SIZE = 512;
		private const string FAILED_MESSAGE = "Connection failed";
		private string _host;
		private int _port;
		private string _route;
		private HttpMethod _method;
		private string? _body;
		private string? _contentType;

		// Method to make a new HTTP request with the given parameters
		public static string MakeHttpRequest(string host, int port, string route, HttpMethod method, string? body, string? contentType)
		{
			return new HttpRequest(host, port, route, method, body, contentType)._makeRequest();
		}

		// Constructor to create HTTP Request with the given parameters
		private HttpRequest(string host, int port, string route, HttpMethod method, string? body, string? contentType)
		{
			this._host = host;
			this._port = port;
			this._route = route;
			this._method = method;
			this._body = body;
			this._contentType = contentType;
		}

		// Method to attempt to send a request and return a response
		private string _makeRequest()
		{
			try
			{
				string msg;

				using (var socket = _initSocket())
				{
					if (socket is null)
					{
						msg = FAILED_MESSAGE;
					}
					else
					{
						msg = _sendRequest(socket);
					}
				}

				return msg;
			}
			catch
			{
				throw new Exception("A connection error occurred");
			}
		}

		// Method to create a socket with the addresses from the host
		private Socket? _initSocket()
		{
			Socket? socket = null;

			// Get the list of valid addresses from the host
			var addresses = _resolveIPAddressesFromHost();

			// If there are valid addresses
			if (!(addresses is null))
			{
				// Initialize the socket with the list of valid IPv4 addresses
				socket = _getIPV4Socket(addresses);
			}

			return socket;
		}

		// Method to attempt to get the valid IP addresses from the host
		private IPAddress[]? _resolveIPAddressesFromHost()
		{
			IPAddress[]? addresses;
			try
			{
				addresses = Dns.GetHostEntry(_host).AddressList;
			}
			catch
			{
				addresses = null;
			}

			return addresses;
		}

		// Method to get the socket from the list of IPv4 addresses
		private Socket? _getIPV4Socket(IPAddress[] addresses)
		{
			Socket? socket = null;

			// Get only the IPv4 addresses 
			IPAddress? ipv4Address = addresses.FirstOrDefault(
				address => address.AddressFamily == AddressFamily.InterNetwork);

			// If there are valid IPv4 addresses
			if (ipv4Address != null)
			{
				// Create a new socket and connect it to the IPv4 address through the given socket
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect(ipv4Address, _port);
			}

			return socket;
		}

		// Method to send the encoded request to the server
		private string _sendRequest(Socket socket)
		{
			// Get the encoded request
			var requestEncoded = _buildMessage();

			// Send the encoded request over the socket
			socket.Send(requestEncoded);

			// Get the response from the server over the socket
			var response = _receiveFromSocket(socket);

			return response;
		}

		// Method to build the message to be sent over the socket
		private byte[] _buildMessage()
		{
			// Create a new request message with the provided fields
			var requestMessage = new HttpRequestMessage(_method, _host, _route, _body);

			// If there is supposed to be content
			if (!(_contentType is null))
			{
				// Set the content type of the request message
				requestMessage.SetHeaderValue("Content-Type", _contentType);

				// If the body is present
				if (!(_body is null))
				{
					// Set the content length of the request message
					requestMessage.SetHeaderValue("Content-Length", Encoding.ASCII.GetByteCount(_body).ToString());
				}
			}

			return Encoding.ASCII.GetBytes(requestMessage.AsString());
		}

		// Receive and convert the response message from the server over the socket
		private string _receiveFromSocket(Socket socket)
		{
			// Create variables to receive and convert the response
			var bytesReceived = new List<byte>();
			int bytes = 0;
			var responseBuffer = new byte[RESPONSE_BUFFER_SIZE];

			// Do while there are still bytes to be received over the socket
			do
			{
				// Get the number of bytes to take from the socket
				bytes = socket.Receive(responseBuffer, RESPONSE_BUFFER_SIZE, SocketFlags.None);

				// Get the bytes from the socket
				bytesReceived.AddRange(responseBuffer.Take(bytes));

				// Sleep to allow enough time for the bytes to be received
				Thread.Sleep(2);
			}
			while (socket.Available > 0 && bytes > 0);

			return Encoding.ASCII.GetString(bytesReceived.ToArray());
		}
	}
}
