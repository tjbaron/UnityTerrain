using UnityEngine;
using System;
using System.Collections;
using System.IO;
using SimpleJSON;

namespace TFServer {
	namespace Core {
		public static class EncodePacket {
			public static byte[] call(object[] parameters, int specNum, JSONNode spec) {
				MemoryStream stream = new MemoryStream();
				byte[] bytedata = BitConverter.GetBytes((ushort)specNum);//spec["id"].AsInt);
				stream.Write(bytedata, 0, 2);

				for (int i=0; i<spec["parameters"].Count; i++) {
					JSONNode p = spec["parameters"][i];
					if ((string)p["type"] == "String") {
						bytedata = BitConverter.GetBytes((ushort)((string)parameters[i]).Length);
						stream.Write(bytedata, 0, 2);
						bytedata = System.Text.Encoding.UTF8.GetBytes((string)parameters[i]);
						stream.Write(bytedata, 0, bytedata.Length);
					} else if ((string)p["type"] == "Int8") {
						bytedata = BitConverter.GetBytes((byte)parameters[i]);
						stream.Write(bytedata, 0, 4);
					} else if ((string)p["type"] == "UInt8") {
						bytedata = BitConverter.GetBytes((byte)parameters[i]);
						stream.Write(bytedata, 0, 4);
					} else if ((string)p["type"] == "Int16") {
						bytedata = BitConverter.GetBytes((short)parameters[i]);
						stream.Write(bytedata, 0, 2);
					} else if ((string)p["type"] == "UInt16") {
						bytedata = BitConverter.GetBytes((ushort)parameters[i]);
						stream.Write(bytedata, 0, 2);
					} else if ((string)p["type"] == "Int32" || (string)p["type"] == "Int") {
						bytedata = BitConverter.GetBytes((int)parameters[i]);
						stream.Write(bytedata, 0, 4);
					} else if ((string)p["type"] == "UInt32" || (string)p["type"] == "UInt") {
						bytedata = BitConverter.GetBytes((uint)parameters[i]);
						stream.Write(bytedata, 0, 4);
					} else if ((string)p["type"] == "Float") {
						bytedata = BitConverter.GetBytes((float)parameters[i]);
						stream.Write(bytedata, 0, 4);
					} else if ((string)p["type"] == "Double") {
						bytedata = BitConverter.GetBytes((double)parameters[i]);
						stream.Write(bytedata, 0, 8);
					}
				}

				return stream.ToArray();
			}
		}
	}
}
