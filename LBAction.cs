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
	Manual, // Action is activated by a call
	ConditionalManual, // Action is activated by a call, checking conditions
	Automatic, 
	ConditionalAutomatic, // Action is activated when conditions are met
}

public enum LBActionDectivationTypes
{
	Manual, // Action is stopped by a call
	ConditionalManual, // Action is stopped by a call, checking conditions
	Automatic,
	ConditionalAutomatic
}

public abstract class LBAction : ScriptableObject
{
	protected GameObject parent = null;

	public LBActionActivationTypes ActionActivation = LBActionActivationTypes.Manual;
	public LBActionDectivationTypes ActionDeactivation = LBActionDectivationTypes.Manual;

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

	protected virtual bool CheckActivationConditions()
	{
		return true;
	}

	public virtual bool CanActivateAction ()
	{
		if (ActionActivation == LBActionActivationTypes.Manual || ActionActivation == LBActionActivationTypes.Automatic) 
		{
			if (action_state == LBActionStates.Inactive)
				return true;
			else
				return false;
		}

		if (ActionActivation == LBActionActivationTypes.ConditionalManual || ActionActivation == LBActionActivationTypes.ConditionalAutomatic)
		{	
			if (action_state == LBActionStates.Inactive)
				return CheckActivationConditions();
			else
				return false;
		}

		return false;
	}

	public virtual void ActivateAction ()
	{			
		if (CanActivateAction()) 
		{
			action_state = LBActionStates.Active;
		}
	}

	protected virtual bool CheckDeactivationConditions()
	{
		return true;
	}

	public virtual bool CanDeactivateAction ()
	{
		if (ActionDeactivation == LBActionDectivationTypes.Manual || ActionDeactivation ==LBActionDectivationTypes.Automatic) 
		{
			if (action_state == LBActionStates.Active)
				return true;
			else
				return false;
		}

		if (ActionDeactivation == LBActionDectivationTypes.ConditionalManual || ActionDeactivation ==LBActionDectivationTypes.ConditionalAutomatic)
		{	
			if (action_state == LBActionStates.Active)
				return CheckDeactivationConditions();
			else
				return false;
		}

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