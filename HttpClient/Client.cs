using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpClient
{
	public class Client
	{
		public static string Get(string host, Dictionary<string, string> headers)
		{
			var hostEntries = Dns.GetHostEntry(host);
			var ipAddress = hostEntries.AddressList[0];
			const int port = 80;
			var ipEndPoint = new IPEndPoint(ipAddress, port);
			using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			socket.Connect(ipEndPoint);

			//Cache-Control: no-cache

			IEnumerable<string> parsedHeaders = headers.Select(header => header.Key + ":" + header.Value + "\r\n");
			if (parsedHeaders.Any())
			{
				// Send request
				var request = "GET / HTTP/1.1\r\n" +
				              "Host: " + host + "\r\n" +
				              string.Join("", parsedHeaders) + 
				              "Connection: Close\r\n\r\n";
				var requestBytes = Encoding.ASCII.GetBytes(request); //Accept-Encoding: gzip,deflate
				socket.Send(requestBytes, requestBytes.Length, 0);
			}
			else
			{
				var request = "GET / HTTP/1.1\r\n" +
				              "Host: " + host + "\r\n" +
				              "Connection: Close\r\n\r\n";
				var requestBytes = Encoding.ASCII.GetBytes(request);
				socket.Send(requestBytes, requestBytes.Length, 0);
			}
			

			// Receive response
			int bytes;
			var bytesReceived = new byte[256];
			var contents = new StringBuilder();
			do
			{
				bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
				contents.Append(Encoding.ASCII.GetString(bytesReceived, 0, bytes));
			}
			while (bytes > 0);

			return contents.ToString();
		}
	}
}
