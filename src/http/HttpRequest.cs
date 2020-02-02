using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace http_client.http
{
	public class HttpRequest
	{
		private const int HTTP_PORT = 80;
		private const int RESPONSE_BUFFER_SIZE = 512;
		private string _host;
		private string _route;
		private HttpMethod _method;
		private string? _body;

		public static string MakeHttpRequest(string host, string route, HttpMethod method, string? body)
		{
			return new HttpRequest(host, route, method, body)._makeRequest();
		}

		private HttpRequest(string host, string route, HttpMethod method, string? body)
		{
			this._host = host;
			this._route = route;
			this._method = method;
			this._body = body;
		}

		private string _makeRequest()
		{
			string msg;

			using (var socket = _initSocket())
			{
				if (socket is null)
				{
					msg = "Connection failed";
				}
				else
				{
					msg = _sendRequest(socket);
				}
			}

			return msg;
		}

		private Socket? _initSocket()
		{
			Socket? socket = null;
			var addresses = _resolveIPAddressesFromHost();
			if (!(addresses is null))
			{
				socket = _getIPV4Socket(addresses);
			}

			return socket;
		}

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

		private Socket? _getIPV4Socket(IPAddress[] addresses)
		{
			Socket? socket = null;
			foreach (var address in addresses)
			{
				var attemptedSocket = _trySocket(address);
				if (!(attemptedSocket is null))
				{
					socket = attemptedSocket;
					break;
				}
			}

			return socket;
		}

		private Socket? _trySocket(IPAddress address)
		{
			var attemptedSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				attemptedSocket.Connect(address, HTTP_PORT);
			}
			catch
			{
				return null;
			}

			return attemptedSocket.Connected ? attemptedSocket : null;
		}

		private string _sendRequest(Socket socket)
		{
			var requestEncoded = _buildMessage();
			socket.Send(requestEncoded);
			var response = _receiveFromSocket(socket);

			return response;
		}

		private byte[] _buildMessage()
		{
			var requestMessage = new HttpRequestMessage(_method, _host, _route, _body).AsString();
			return Encoding.ASCII.GetBytes(requestMessage);
		}

		private string _receiveFromSocket(Socket socket)
		{
			var bytesReceived = new List<byte>();
			int bytes = 0;
			var responseBuffer = new byte[RESPONSE_BUFFER_SIZE];

			do
			{
				bytes = socket.Receive(responseBuffer, RESPONSE_BUFFER_SIZE, 0);
				bytesReceived.AddRange(responseBuffer.Take(bytes));
			}
			while (bytes == RESPONSE_BUFFER_SIZE);

			return Encoding.ASCII.GetString(bytesReceived.ToArray());
		}
	}
}
