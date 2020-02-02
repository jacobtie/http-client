using System;
using System.Text;
using System.Collections.Generic;

namespace http_client.http
{
	public class HttpRequestMessage
	{
		public HttpMethod Method { get; set; }
		public string Route { get; set; }
		public string Version { get; set; }
		public Dictionary<string, string> Headers { get; set; }
		public string? Body { get; set; }

		public HttpRequestMessage(HttpMethod method, string host, string route, string? body)
		{
			this.Method = method;
			this.Route = route;
			this.Version = "HTTP/1.1";
			this.Body = body;
			this.Headers = new Dictionary<string, string>();
			_initDefaultHeaders(host);
		}

		private void _initDefaultHeaders(string host)
		{
			this.SetHeaderValue("User-Agent", "KrevatAgent/0.1.0");
			this.SetHeaderValue("Accept", "*/*");
			this.SetHeaderValue("Cache-Control", "no-cache");
			this.SetHeaderValue("Host", host);
			this.SetHeaderValue("Connection", "keep-alive");
		}

		public void SetHeaderValue(string header, string value)
		{
			if (this.Headers.ContainsKey(header))
			{
				this.Headers[header] = value;
			}
			else
			{
				this.Headers.Add(header, value);
			}
		}

		public string GetHeaderValue(string header)
		{
			if (this.Headers.ContainsKey(header))
			{
				return this.Headers[header];
			}
			else
			{
				throw new KeyNotFoundException("Header not found");
			}
		}

		public string AsString()
		{
			var sb = new StringBuilder();
			sb.Append(this.Method.ToString("G"));
			sb.Append(" ");
			sb.Append(this.Route);
			sb.Append(" ");
			sb.Append(this.Version);
			sb.Append("\r\n");

			foreach (var (header, value) in this.Headers)
			{
				sb.Append($"{header}: {value}\r\n");
			}

			sb.Append("\r\n");
			if (!(this.Body is null))
			{
				sb.Append(this.Body);
			}

			return sb.ToString();
		}
	}
}
