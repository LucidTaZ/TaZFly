public struct GridCoordinates
{
	public int x;
	public int z;

	public GridCoordinates (int x, int z) {
		this.x = x;
		this.z = z;
	}

	override public string ToString () {
		return string.Format("({0}, {1})", x, z);
	}

	public GridCoordinates North {
		get {
			return new GridCoordinates(x, z + 1);
		}
	}

	public GridCoordinates South {
		get {
			return new GridCoordinates(x, z - 1);
		}
	}

	public GridCoordinates East {
		get {
			return new GridCoordinates(x + 1, z);
		}
	}

	public GridCoordinates West {
		get {
			return new GridCoordinates(x - 1, z);
		}
	}

	public static GridCoordinates operator + (GridCoordinates a, GridCoordinates b) {
		return new GridCoordinates(a.x + b.x, a.z + b.z);
	}
}
