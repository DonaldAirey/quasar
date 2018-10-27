namespace MarkThree
{

	using System;
	using System.Collections;
	using System.IO;

	public class CompressedStreamReader
	{

		// Private Members
		private const int bufferSize = 1024;

		/// <summary>
		/// Reads data out of a stream when the number of bytes in the stream isn't know.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static byte[] ReadBytes(Stream stream)
		{

			// During some kinds of IO, particularly IO involving compression/decompression, the number of bytes in the stream isn't
			// known nor can it be known until the stream is read.  This makes reserving a buffer for the data difficult.  This
			// algorithm will collect the chunks of data read from the stream in a variable length array.
			ArrayList arrayList = new ArrayList();

			// This loop will read data from the stream and place it in the a variable list of buffer until the stream is empty.
			int totalCount = 0;
			while (true)
			{
				byte[] buffer = new byte[bufferSize];
				int bytesRead = stream.Read(buffer, 0, buffer.Length);
				if (bytesRead == 0)
					break;
				arrayList.Add(buffer);
				totalCount += bytesRead;
			}

			// This will copy the data from the list off buffers into a large, contiguous buffer.
			byte[] streamBuffer = new byte[totalCount];
			int offset = 0;
			foreach (byte[] buffer in arrayList)
			{
				int length = offset + buffer.Length < totalCount ? buffer.Length : totalCount - offset;
				Array.Copy(buffer, 0, streamBuffer, offset, length);
				offset += buffer.Length;
			}

			// This contains a single, contiguous array of the bytes read from the stream.
			return streamBuffer;

		}

	}
}
