using System;

public interface IAITransition
{
	AIState MoveTo { get; }
}

public class LambdaAITransition : IAITransition
{
	private readonly AIState target;
	private readonly Func<bool> shouldMove;

	public LambdaAITransition(AIState target, Func<bool> shouldMove)
	{
		this.target = target;
		this.shouldMove = shouldMove;
	}

	public AIState MoveTo => shouldMove() ? target : null;
}