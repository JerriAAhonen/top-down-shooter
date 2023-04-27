using UnityEngine;

public class EnemySpawner : Spawner
{
	protected override void OnReady()
	{
		MainRpc.Instance.SpawnEnemy(this);
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		using (new TemporaryHandlesMatrix(transform))
		{
			using (new TemporaryHandlesColor(Color.red.Opacity(0.03f)))
			{
				UnityEditor.Handles.DrawSolidDisc(Vector3.zero, Vector3.up, 0.5f);
			}

			using (new TemporaryHandlesColor(Color.red))
			{
				UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.up, 0.5f);
			}
		}
	}
#endif
}
