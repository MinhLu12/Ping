using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Ping
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			var packet = new ICMPPacket(
				type: 0x08, // Value is Echo request (what kind of ICMP message is in the IP packet)
				code: 0x00, // Value is Echo reply (this further defines the type. This is what the Echo Request packet uses)
				
				// below is the payload of the packet
				identifier: 1, // 1-byte that uniquely identifies Echo Request IP packet
				sequenceNumber: 1, // 1-byte sequence number for identification of ICMP packets in a stream
				Encoding.ASCII.GetBytes(Guid.NewGuid().ToString())); // multi-byte array for payload

			var packetSize = packet.MessageSize + 4;

			var rawSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
			
			// Get remote device to send packet to
			var iep = new IPEndPoint(IPAddress.Parse(args[0]), 0); // ICMP doesn't use ports
			EndPoint ep = iep;
			
			rawSocket.SendTo(packet.GetBytes(), packetSize, SocketFlags.None, iep);

			int receivedSize;
			byte[] receivedData = new byte[1024];
			try
			{
				receivedData = new byte[1024];
				receivedSize = rawSocket.ReceiveFrom(receivedData, ref ep); // have the socket receive data back from the remote device
			}
			catch (SocketException)
			{
				Console.WriteLine("No response from remote host");
				return;
			}

			ICMPPacket response = new ICMPPacket(receivedData, receivedSize);
			Console.WriteLine("response from: {0}", ep);
			Console.WriteLine(" Type {0}", response.Type);
			Console.WriteLine(" Code: {0}", response.Code);
			int Identifier = BitConverter.ToInt16(response.Message, 0);
			int Sequence = BitConverter.ToInt16(response.Message, 2);
			Console.WriteLine(" Identifier: {0}", Identifier);
			Console.WriteLine(" Sequence: {0}", Sequence);
			string stringData = Encoding.ASCII.GetString(response.Message, 4, response.MessageSize - 4);
			Console.WriteLine(" data: {0}", stringData);
			rawSocket.Close();
		}
	}
}
