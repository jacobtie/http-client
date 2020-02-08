using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace http_client.http
{
	public class HttpRequest
	{
		private const int RESPONSE_BUFFER_SIZE = 512;
		private const string FAILED_MESSAGE = "Connection failed";
		private string _host;
		private int _port;
		private string _route;
		private HttpMethod _method;
		private string? _body;
		private string? _contentType;

		public static string MakeHttpRequest(string host, int port, string route, HttpMethod method, string? body, string? contentType)
		{
			return new HttpRequest(host, port, route, method, body, contentType)._makeRequest();
		}

		private HttpRequest(string host, int port, string route, HttpMethod method, string? body, string? contentType)
		{
			this._host = host;
			this._port = port;
			this._route = route;
			this._method = method;
			this._body = body;
			this._contentType = contentType;
		}

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
				throw new Exception("An error occurred");
			}
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
				attemptedSocket.Connect(address, _port);
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
			var requestMessage = new HttpRequestMessage(_method, _host, _route, _body);
			if (!(_contentType is null))
			{
				requestMessage.SetHeaderValue("Content-Type", _contentType);
				if (!(_body is null))
				{
					requestMessage.SetHeaderValue("Content-Length", Encoding.ASCII.GetByteCount(_body).ToString());
				}
			}
			return Encoding.ASCII.GetBytes(requestMessage.AsString());
		}

		private string _receiveFromSocket(Socket socket)
		{
			var bytesReceived = new List<byte>();
			int bytes = 0;
			var responseBuffer = new byte[RESPONSE_BUFFER_SIZE];

			do
			{
				bytes = socket.Receive(responseBuffer, RESPONSE_BUFFER_SIZE, SocketFlags.None);
				bytesReceived.AddRange(responseBuffer.Take(bytes));
				Thread.Sleep(2);
			}
			while (socket.Available > 0 && bytes > 0);

			return Encoding.ASCII.GetString(bytesReceived.ToArray());
		}
	}
}
