using UnityEngine;
using UnityEngine.AI;

public class CombatAI : AIState
{
	[SerializeField] private float pathInterval = 0.3f;
	[SerializeField] private float speed = 2f;
	[SerializeField] private float angularSpeed = 180f;

	private NavMeshPath path;
	private PlayerController target;
	private float pathValidFor;

	protected override IAITransition[] CreateTransitions()
	{
		return new[]
		{
			// TODO: Transition to CombatAI
			new LambdaAITransition(null, () => false)
		};
	}

	public override void OnInit()
	{
		path = new NavMeshPath();
	}

	public override void OnEnter(AIState previous)
	{
		Agent.speed = speed;
		Agent.angularSpeed = angularSpeed;
	}

	public override void OnUpdate(float dt)
	{
		if (pathValidFor > 0)
			pathValidFor -= dt;
		if (pathValidFor > 0)
			return;

		// TODO: Target player

		Agent.SetPath(path);
		pathValidFor = pathInterval;
	}

	public override void OnExit(AIState next)
	{
		Agent.SetPath(null);
		pathValidFor = 0;
	}
}
