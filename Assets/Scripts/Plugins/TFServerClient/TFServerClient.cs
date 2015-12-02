using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using SimpleJSON;

namespace TFServer {

	public class Client : MonoBehaviour {

		public delegate void ConnectionCallback();
		
		public delegate void OnCallback(Hashtable data);

		private static Hashtable settings;
		private static UdpClient udpClient;

		private static JSONNode rpcList;
		private static Dictionary<string, int> rpcDict;

		private static Dictionary<string, OnCallback> rpcs = new Dictionary<string, OnCallback>();

		class UdpState {
            public UdpClient u;
            public IPEndPoint e;
        }

		/*private int byteCount = 0;
		
		private static TcpClient tcpClient;
		private static NetworkStream networkStream;*/

		public static void Connect(Hashtable s, ConnectionCallback callback) {
			settings = s;
			if (s.Contains("json")) {
				TextAsset json = (TextAsset)Resources.Load((string)s["json"], typeof(TextAsset));
				rpcList = JSONNode.Parse(json.text);
				rpcDict = new Dictionary<string, int>();
				for (int i=0; i<rpcList.Count; i++) {
					rpcDict[rpcList[i]["name"]] = i;
				}
			}

			udpClient = new UdpClient();
			IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);

			UdpState ustate = new UdpState();
			ustate.e = remote;
			ustate.u = udpClient;

			udpClient.BeginReceive(new AsyncCallback(GotData), ustate);

			callback();
		}

		public static void Disconnect(string address, int port) {
			udpClient.Close();
		}

		public static void On(string functionName, OnCallback callback) {
			rpcs[functionName] = callback;
		}

		public static void SendRPC(string functionName, params object[] list) {
			byte[] data = Core.EncodePacket.call(list, rpcDict[functionName], rpcList[rpcDict[functionName]]);
			//byte[] bs = Encoding.ASCII.GetBytes("aaaa"+(string)list[0]);
			//bs[0] = 0x01; bs[1] = 0x00; bs[2] = 0x05; bs[2] = 0x00;
			udpClient.Send(data, data.Length, (string)settings["address"], (int)settings["serverport"]);
		}

		public static void GotData(IAsyncResult ar) {
			UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
			IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

			Byte[] data = u.EndReceive(ar, ref e);

			Hashtable res = Core.ParsePacket.call(data, rpcList);
			rpcs["UpdatePosition"](res);

			u.BeginReceive(new AsyncCallback(GotData), ar.AsyncState);
		}
	}
}