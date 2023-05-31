using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightTarget : MonoBehaviour
{
	[SerializeField] private bool shouldShootThis;

	public bool ShouldShootThis => shouldShootThis;
}
