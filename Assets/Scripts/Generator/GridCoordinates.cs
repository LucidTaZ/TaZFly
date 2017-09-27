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
}
