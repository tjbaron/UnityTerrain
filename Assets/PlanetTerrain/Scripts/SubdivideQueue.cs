using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SubdivideQueue {
	// Levels: Player Standing On It - Adjacent To Player - Visible - Hidden
	private static List<PlanetTerrainSegment>[] priorityLevels = new List<PlanetTerrainSegment>[]{
		new List<PlanetTerrainSegment>(), new List<PlanetTerrainSegment>(), 
		new List<PlanetTerrainSegment>(), new List<PlanetTerrainSegment>()
	};
	private static bool busy = false;

	public static void Add(int priority, PlanetTerrainSegment seg) {
		if (!busy) {
			busy = true;
			//seg.Subdivide();
		} else {
			priorityLevels[priority].Add(seg);
		}
	}

	public static void Remove(PlanetTerrainSegment seg) {
		for (var i=0; i<priorityLevels.Length; i++) {
			for (var j=0; i<priorityLevels[i].Count; i++) {
				if (priorityLevels[i][j] == seg) {
					priorityLevels[i].RemoveAt(j);
					return;
				}
			}
		}
	}

	public static void Release() {
		for (var i=0; i<priorityLevels.Length; i++) {
			if (priorityLevels[i].Count > 0) {
				//priorityLevels[i][0].Subdivide();
				priorityLevels[i].RemoveAt(0);
				return;
			}
		}
	}
}
