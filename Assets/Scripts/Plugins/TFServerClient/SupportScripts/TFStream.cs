/*	# TFStream
	Allows a simple way to add a bunch of data of different types into
	a stream of bytes (which can them be turned into an array). Easy down
	conversion of numbers to less bytes.
	The names of the functions in this class are based on the names of the
	functions in the Buffer class in Node.js. Everything here is little endian.
*/

using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class TFStream {
	MemoryStream stream = new MemoryStream();

	public byte[] toBytes() {
		return stream.ToArray();
	}

	public int getLength() {
		return (int)stream.Length;
	}

	public int writeString(string data) {
		byte[] bytedata = System.Text.Encoding.UTF8.GetBytes(data);
		stream.Write(bytedata, 0, data.Length);
		return data.Length;
	}

	public void writeInt32(int value) {
		byte[] bytedata = BitConverter.GetBytes((int)value);
		stream.Write(bytedata, 0, 4);
	}

	public void writeInt16(int value) {
		byte[] bytedata = BitConverter.GetBytes((short)value);
		stream.Write(bytedata, 0, 2);
	}

	public void writeUInt16(int value) {
		byte[] bytedata = BitConverter.GetBytes((ushort)value);
		stream.Write(bytedata, 0, 2);
	}

	public void writeUInt8(int value) {
		byte bytedata = Convert.ToByte((int)value);
		stream.WriteByte(bytedata);
	}

	public void writeFloat(float value) {
		byte[] bytedata = BitConverter.GetBytes((float)value);
		stream.Write(bytedata, 0, 4);
	}
	
}
