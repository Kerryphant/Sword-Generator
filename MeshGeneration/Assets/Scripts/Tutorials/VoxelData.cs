using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { NORTH, EAST, SOUTH, WEST, UP, DOWN }

public class VoxelData
{

	struct DataCoord
	{
		public int x;
		public int y;
		public int z;

		public DataCoord(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	DataCoord[] offsets =
	{
		new DataCoord(0, 0, 1),
		new DataCoord(1, 0, 0),
		new DataCoord(0, 0, -1),
		new DataCoord(-1, 0, 0),
		new DataCoord(0, 1, 0),
		new DataCoord(0, -1, 0),
	};


	int[,] data = new int[,]
	{
		{0, 1, 1 },
		{1, 1, 1 },
		{1, 1, 0 }
	};


	public int Width
	{
		get { return data.GetLength(0); }
	}

	public int Depth
	{
		get { return data.GetLength(1); }
	}

	public int GetCell (int x, int z)
	{
		return data[x, z];
	}

	public int GetNeighbour(int x, int y, int z, Direction dir)
	{
		DataCoord offsetToCheck = offsets[(int)dir];
		DataCoord neighbourCoord = new DataCoord(x + offsetToCheck.x, y + offsetToCheck.y, z + offsetToCheck.z);

		//if out of bounds
		if(neighbourCoord.x < 0 || neighbourCoord.x >= Width || neighbourCoord.y != 0 || neighbourCoord.z < 0 || neighbourCoord.z >= Depth)
		{
			return 0;
		}
	
		return GetCell(neighbourCoord.x, neighbourCoord.z);
	}

}