using UnityEngine;
using System.Collections;

public static class PTHelpers {
	private static int visibleTiles = 0;
	public static void Add() {
		visibleTiles++;
		//Debug.Log(visibleTiles);
	}
	public static void Remove() {
		visibleTiles--;
		//Debug.Log(visibleTiles);
	}
}
