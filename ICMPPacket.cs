using System;

namespace Ping
{
	public class ICMPPacket
	{
		public readonly byte Type;
		public readonly byte Code;
		private readonly ushort checkSum;
		public readonly int MessageSize;
		public readonly byte[] Message = new byte[1024];


		public ICMPPacket(byte[] data, int size)
		{
			Type = data[20];
			Code = data[21];
			checkSum = BitConverter.ToUInt16(data, 22);
			MessageSize = size - 24;
			Buffer.BlockCopy(data, 24, Message, 0, MessageSize);
		}
		
		public ICMPPacket(
			byte type,
			byte code,
			short identifier,
			short sequenceNumber,
			byte[] messageData)
		{
			Type = type;
			Code = code;

			// Copy the data payload fields into the array of bytes in Message
			const int shortLengthInBytes = 2;
			Buffer.BlockCopy(BitConverter.GetBytes(identifier), 0, Message, 0, shortLengthInBytes);
			Buffer.BlockCopy(BitConverter.GetBytes(sequenceNumber), 0, Message, 2, shortLengthInBytes);
			Buffer.BlockCopy(messageData, 0, Message, 4, messageData.Length);

			MessageSize = messageData.Length + 4; // 4 from data payload, additional to existing 2 from identifiers

			checkSum = GetCheckSum();
		}

		public byte[] GetBytes()
		{
			byte[] data = new byte[MessageSize + 9];
			Buffer.BlockCopy(BitConverter.GetBytes(Type), 0, data, 0, 1);
			Buffer.BlockCopy(BitConverter.GetBytes(Code), 0, data, 1, 1);
			Buffer.BlockCopy(BitConverter.GetBytes(checkSum), 0, data, 2, 2);
			Buffer.BlockCopy(Message, 0, data, 4, MessageSize);
			return data;
		}

		// COPIED AND PASTED
		private ushort GetCheckSum()
		{
			UInt32 chcksm = 0;
			byte[] data = GetBytes();
			int packetsize = MessageSize + 8;
			int index = 0;
			while (index < packetsize)
			{
				chcksm += Convert.ToUInt32(BitConverter.ToUInt16(data, index));
				index += 2;
			}

			chcksm = (chcksm >> 16) + (chcksm & 0xffff);
			chcksm += (chcksm >> 16);
			return (UInt16) (~chcksm);
		}
	}
}
