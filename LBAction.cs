using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LBActionStates
{
	Active,
	Inactive,
	Disabled
}

public enum LBActionActivationTypes
{
	Mandatory, // Action is activated by a call
	ConditionalMandatory, // Action is activated by a call, checking conditions
	Automatic, 
	ConditionalAutomatic, // Action is activated when conditions are met
}

public enum LBActionDectivationTypes
{
	Mandatory, // Action is stopped by a call
	ConditionalMandatory, // Action is stopped by a call, checking conditions
	Automatic,
	ConditionalAutomatic
}

public abstract class LBAction : ScriptableObject
{
	protected GameObject parent = null;

	public LBActionActivationTypes ActionActivation = LBActionActivationTypes.Mandatory;
	public LBActionDectivationTypes ActionDeactivation = LBActionDectivationTypes.Mandatory;

	protected LBActionStates action_state = LBActionStates.Inactive;

	protected virtual bool Init (GameObject parentgameobject)
	{
		if (parent == null)
			return false;
		
		parent = parentgameobject;

		return true;
	}

	public LBActionStates ActionState 
	{
		get 
		{
			return action_state;
		}
	}

	public virtual bool CanActivateAction ()
	{
		if (action_state == LBActionStates.Inactive)
			return true;
		else
			return false;
	}

	public virtual void ActivateAction ()
	{			
		if (CanActivateAction()) 
		{
			action_state = LBActionStates.Active;
		}
	}

	public virtual bool CanDeactivateAction ()
	{
		if (action_state == LBActionStates.Active)
			return true;
		else
			return false;
	}

	public virtual void DeactivateAction ()
	{
		if (CanDeactivateAction()) 
		{
			action_state = LBActionStates.Inactive;
		}
	}

	protected virtual void Tick ()
	{
	}
}