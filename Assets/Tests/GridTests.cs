using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GridTests
{
	private Grid<int> grid;

	[SetUp]
	public void Setup()
	{
		grid = new Grid<int>(new Vector2Int(5, 5), 1, Vector3.zero);
	}

	[TestCase(0, 0, true)]
	[TestCase(4, 4, true)]
	[TestCase(5, 0, false)]
	public void IsInside(int x, int y, bool expected)
	{
		var gridPos = new Vector2Int(x,y);
		Assert.AreEqual(grid.IsInside(gridPos), expected);
	}
}
