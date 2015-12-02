using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public struct RPCFormats {
	public JSONNode rpcJson;
	public Dictionary<string, JSONNode> rpcDict;
}

namespace TFServer {
	namespace Core {
		public static class LoadJson {
			public delegate void callbackDelegate(int x);
			
			public static RPCFormats call(string filename) {
				TextAsset contents = Resources.Load("MyTexts/text") as TextAsset;
				JSONNode rpcJson = JSONNode.Parse(contents.text);
				Dictionary<string, JSONNode> rpcDict = new Dictionary<string, JSONNode>();
				for (int i=0; i<rpcJson.Count; i++) {
					rpcDict[rpcJson[i]["name"]] = rpcJson[i];
				}
				RPCFormats returnVal = new RPCFormats();
				returnVal.rpcJson = rpcJson;
				returnVal.rpcDict = rpcDict;
				return returnVal;
			}
		}
	}
}
