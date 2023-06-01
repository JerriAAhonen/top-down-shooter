using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIState : MonoBehaviour
{
	private IAITransition[] transitions;

	private void Awake()
	{
		Agent = GetComponent<NavMeshAgent>();
		OnInit();
	}

	public IReadOnlyList<IAITransition> Transitions
	{
		get
		{
			transitions ??= CreateTransitions();
			return transitions;
		}
	}

	protected NavMeshAgent Agent { get; private set; }
	protected virtual IAITransition[] CreateTransitions() => Array.Empty<IAITransition>();

	public virtual void OnInit() { }
	public virtual void OnEnter(AIState previous) { }
	public virtual void OnUpdate(float dt) { }
	public virtual void OnExit(AIState next) { }
}
