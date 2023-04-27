public class EnemySpawner : Spawner
{
	protected override void OnReady()
	{
		MainRpc.Instance.SpawnEnemy(this);
	}
}
