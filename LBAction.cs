using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	public enum LBActionStates
	{
		Active,
		Inactive,
		Disabled
	}

	public enum LBActionActivationTypes
	{
		Manual,
		// Action is activated by a call
		ConditionalManual,
		// Action is activated by a call, checking conditions
		Automatic,
		ConditionalAutomatic,
		// Action is activated when conditions are met
	}

	public enum LBActionDectivationTypes
	{
		Manual,
		// Action is stopped by a call
		ConditionalManual,
		// Action is stopped by a call, checking conditions
		Automatic,
		ConditionalAutomatic
	}

	public class LBActionEventArgs : EventArgs
	{
	}

	public abstract class LBAction : ScriptableObject
	{
		protected GameObject parent = null;
		protected LBActionManager manager = null;

		public string ActionName = "Dummy_Action";

		public LBActionActivationTypes ActionActivation = LBActionActivationTypes.Manual;
		public LBActionDectivationTypes ActionDeactivation = LBActionDectivationTypes.Manual;

		/// <summary>
		/// Occurs when this action is activated.
		/// </summary>
		public event EventHandler<LBActionEventArgs> ActionActivated;
		protected LBActionEventArgs ActionActivatedArgs;
		/// <summary>
		/// Occurs when this action is deactivated.
		/// </summary>
		public event EventHandler<LBActionEventArgs> ActionDeactivated;
		protected LBActionEventArgs ActionDeactivatedArgs;

		protected LBActionStates action_state = LBActionStates.Inactive;

		public LBAction ()
		{}
			
		public LBAction (string _name, LBActionActivationTypes _activation, LBActionDectivationTypes _deactivation)
		{
			ActionName = _name;
			ActionActivation = _activation;
			ActionDeactivation = _deactivation;
		}

		public virtual LBAction Duplicate ()
		{
			return null;
		}

		public virtual bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (parent == null || manager == null)
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

		protected virtual bool CheckActivationConditions ()
		{
			return true;
		}

		public virtual bool CanActivateAction ()
		{
			if (ActionActivation == LBActionActivationTypes.Manual || ActionActivation == LBActionActivationTypes.Automatic) {
				if (action_state == LBActionStates.Inactive)
					return true;
				else
					return false;
			}

			if (ActionActivation == LBActionActivationTypes.ConditionalManual || ActionActivation == LBActionActivationTypes.ConditionalAutomatic) {	
				if (action_state == LBActionStates.Inactive)
					return CheckActivationConditions ();
				else
					return false;
			}

			return false;
		}

		public virtual void ActivateAction ()
		{			
			if (CanActivateAction ()) {
				action_state = LBActionStates.Active;
				RaiseEvenet_OnActionActivated ();
			}
		}

		protected virtual bool CheckDeactivationConditions ()
		{
			return true;
		}

		public virtual bool CanDeactivateAction ()
		{
			if (ActionDeactivation == LBActionDectivationTypes.Manual || ActionDeactivation == LBActionDectivationTypes.Automatic) {
				if (action_state == LBActionStates.Active)
					return true;
				else
					return false;
			}

			if (ActionDeactivation == LBActionDectivationTypes.ConditionalManual || ActionDeactivation == LBActionDectivationTypes.ConditionalAutomatic) {	
				if (action_state == LBActionStates.Active)
					return CheckDeactivationConditions ();
				else
					return false;
			}

			return false;
		}

		public virtual void DeactivateAction ()
		{
			if (CanDeactivateAction ()) {
				action_state = LBActionStates.Inactive;
				RaiseEvenet_OnActionDeactivated ();
			}
		}

		public virtual void Tick ()
		{
		}

		protected void RaiseEvenet_OnActionActivated()
		{
			EventHandler<LBActionEventArgs> handler = ActionActivated;

			if (handler != null && ActionActivatedArgs != null) 
			{
				handler (this, ActionActivatedArgs);
			}
		}

		protected void RaiseEvenet_OnActionDeactivated()
		{
			EventHandler<LBActionEventArgs> handler = ActionDeactivated;

			if (handler != null && ActionDeactivatedArgs != null) 
			{
				handler (this, ActionDeactivatedArgs);
			}
		}
	}

	public class DummyAction : LBAction
	{
		public DummyAction()
		{
			ActionActivation = LBActionActivationTypes.Manual;
			ActionDeactivation = LBActionDectivationTypes.Manual;
			action_state = LBActionStates.Disabled;
		}

		public override LBAction Duplicate ()
		{
			DummyAction dup;

			dup = new DummyAction ();

			return dup;
		}
	}
}

