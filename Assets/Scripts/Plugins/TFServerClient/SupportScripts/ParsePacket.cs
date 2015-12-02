using UnityEngine;
using System.Text;
using System.Collections;
using SimpleJSON;

namespace TFServer {
	namespace Core {
		public static class ParsePacket {
			public static Hashtable call(byte[] data, JSONNode rpcs) {
				// If an RPC List JSON is used we are sent a 2 byte number identifying the RPC.
				int rpcid = System.BitConverter.ToInt16(data, 0);
				// Check if the number received refers to a valid RPC...
				if (rpcs.Count <= rpcid) {
					Debug.Log("Invalid");
					return null;
				}
				// Get the name of the RPC.
				JSONNode rpc = rpcs[rpcid];
				int readPos = 2;

				// Get all the parameter values
				Hashtable packetData = new Hashtable();
				packetData["_name"] = rpc["name"];
				JSONNode parameters = rpc["parameters"];
				for (int i=0; i<parameters.Count; i++) {
					object val = (object)ReadParameter(parameters[i]["type"], data, ref readPos);
					packetData.Add((string)parameters[i]["name"], val);
				}
				return packetData;
			}

			private static object ReadParameter(string tp, byte[] data, ref int readPos) {
				object val = null;
				if (tp == "String") {
					// Get the length of the string.
					int stringLen = System.BitConverter.ToInt16(data, readPos);
					// Get the string.
					val = Encoding.UTF8.GetString(data, readPos+2, stringLen);
					readPos +=  2+stringLen;
				} else if (tp == "Int16") {
					val = System.BitConverter.ToInt16(data, readPos);
					readPos += 2;
				} else if (tp == "UInt16") {
					val = System.BitConverter.ToUInt16(data, readPos);
					readPos += 2;
				} else if (tp == "Int32" || tp == "Int") {
					val = System.BitConverter.ToInt32(data, readPos);
					readPos += 4;
				} else if (tp == "UInt32" || tp == "UInt") {
					val = System.BitConverter.ToUInt32(data, readPos);
					readPos += 4;
				} else if (tp == "Float") {
					val = System.BitConverter.ToSingle(data, readPos);
					readPos += 4;
				} else if (tp == "Double") {
					val = System.BitConverter.ToDouble(data, readPos);
					readPos += 8;
				}
				return val;
			}
		}
	}
}
