using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpSocketClient
{
	internal static class Program
	{
		private static void Main()
		{
			// Connect to server
			const string server = "www.google.com";
			var hostEntries = Dns.GetHostEntry(server);
			var ipAddress = hostEntries.AddressList[0];
			const int port = 80;
			var ipEndPoint = new IPEndPoint(ipAddress, port);
			using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			socket.Connect(ipEndPoint);

			// Send request
			const string request = "GET / HTTP/1.1\r\n" +
			                       "Host: " + server +
			                       "\r\nConnection: Close\r\n\r\n";
			var requestBytes = Encoding.ASCII.GetBytes(request);
			socket.Send(requestBytes, requestBytes.Length, 0);

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

			Console.WriteLine(contents.ToString());
		}
	}
}
