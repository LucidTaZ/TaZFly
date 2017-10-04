using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridMask {
	ICollection<GridCoordinates> contents;

	public GridMask (IEnumerable<GridCoordinates> contents) {
		this.contents = contents.ToList();
	}

	public static GridMask CreateSquare (int radius) {
		return new GridMask(enumerateSquare(radius));
	}

	static IEnumerable<GridCoordinates> enumerateSquare (int radius) {
		for (int x = -radius; x <= radius; x++) {
			for (int z = -radius; z <= radius; z++) {
				yield return new GridCoordinates(x, z);
			}
		}
	}

	public GridMask Translate (GridCoordinates delta) {
		return new GridMask(contents.Select(x => x + delta));
	}

	public GridMask Dilate (int radius) {
		int minX = int.MaxValue;
		int maxX = int.MinValue;
		int minZ = int.MaxValue;
		int maxZ = int.MinValue;
		foreach (GridCoordinates coords in contents) {
			minX = Mathf.Min(minX, coords.x);
			maxX = Mathf.Max(maxX, coords.x);
			minZ = Mathf.Min(minZ, coords.z);
			maxZ = Mathf.Max(maxZ, coords.z);
		}

		List<GridCoordinates> result = new List<GridCoordinates>();
		for (int x = minX - radius; x <= maxX + radius; x++) {
			for (int z = minZ - radius; z <= maxZ + radius; z++) {
				GridCoordinates coords = new GridCoordinates(x, z);
				if (containsWithinRadius(coords, radius)) {
					result.Add(coords);
				}
			}
		}
		return new GridMask(result);
	}

	bool containsWithinRadius (GridCoordinates coords, int radius) {
		for (int dx = -radius; dx <= radius; dx++) {
			for (int dz = -radius; dz <= radius; dz++) {
				GridCoordinates neighbor = coords + new GridCoordinates(dx, dz);
				if (contents.Contains(neighbor)) {
					return true;
				}
			}
		}
		return false;
	}

	public bool Contains (GridCoordinates coords) {
		return contents.Contains(coords);
	}

	public IEnumerator<GridCoordinates> GetEnumerator () {
		return contents.GetEnumerator();
	}
}
