using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	/// <summary>
	/// This enum defines action states: an action can be <c>Active</c> (currently running), <c>Inactive</c> (not running) and <c>Disabled</c> (cannot be activated).
	/// </summary>
	public enum LBActionStates
	{
		Active,
		Inactive,
		Disabled
	}

	/// <summary>
	/// This enum defines activation procedure for each <c>LBAction</c> instance. This value 
	/// </summary>
	[Flags]
	public enum LBActionActivationTypes : byte
	{
		/// <summary>
		/// An action can be activated (deactivated) by an external call. I.e. from an <c>LBActionManager</c> instance, which contains this action.
		/// </summary>
		Internal = 1,										//00000001
		/// <summary>
		/// An action can be activated (deactivated) in automatic mode, for example, by itslef.
		/// </summary>
		External = 2, 										//00000010
		/// <summary>
		/// An action can be activated (deactivated) both ways.
		/// </summary>
		InternalOrExternal = 3, 							//00000011
		/// <summary>
		/// An action can be activated (deactivated) by an external call, checking conditions, which are defined by <c>CheckActivationConditions</c>.
		/// </summary>
		ConditionalInternal = 129,							//10000001
		/// <summary>
		/// An action can be activated (deactivated) in automatic mode, checking conditions. This particular variant may mean that this action is activated (deactivated) by itself.
		/// when some condition is met. 
		/// </summary>
		ConditionalExternal = 130,							//10000010
		/// <summary>
		/// An action can be activated (deactivated) both ways, however, conditions should be checked.
		/// </summary>
		ConditionalInternalOrExternal = 131					//10000011
	}
			
	public class LBActionEventArgs : EventArgs
	{
	}

	public abstract class LBAction : ScriptableObject
	{
		protected GameObject parent = null;
		protected LBActionManager manager = null;

		public string ActionName = "Dummy_Action";

		public LBActionActivationTypes ActionActivation = LBActionActivationTypes.External;
		public LBActionActivationTypes ActionDeactivation = LBActionActivationTypes.External;

		/// <summary>
		/// Occurs when this action is activated.
		/// </summary>
		public event EventHandler<LBActionEventArgs> ActionActivated;
		/// <summary>
		/// Occurs when this action is deactivated.
		/// </summary>
		protected LBActionEventArgs ActionActivatedArgs;

		/// <summary>
		/// Occurs when this action is deactivated.
		/// </summary>
		public event EventHandler<LBActionEventArgs> ActionDeactivated;

		protected LBActionEventArgs ActionDeactivatedArgs;

		protected LBActionStates action_state = LBActionStates.Inactive;

//		public LBAction()
//		{}

//		public LBAction (string _name)
//		{
//			ActionName = _name;
//		}
//
//		public LBAction (string _name, LBActionActivationTypes _activation, LBActionActivationTypes _deactivation)
//		{
//			ActionName = _name;
//			ActionActivation = _activation;
//			ActionDeactivation = _deactivation;
//		}

		public virtual LBAction Duplicate ()
		{
			return null;
		}

		protected virtual void DuplicateProperties(LBAction dup)
		{
			dup.ActionName = ActionName;
			dup.ActionActivation = ActionActivation;
			dup.ActionDeactivation = ActionDeactivation;
			dup.action_state = LBActionStates.Inactive;

			dup.name = ActionName;
		}

		public static LBAction Default ()
		{
			return null;
		}

		public virtual bool Init (GameObject _parent, LBActionManager _manager)
		{
			if (_parent == null || _manager == null)
			{
				ReportProblem ("parent or manager in " + _parent.ToString () + " cannot be set!");
				return false;
			}

			parent = _parent;
			manager = _manager;

			return true;
		}

		public LBActionStates ActionState
		{
			get 
			{
				return action_state;
			}
		}

		public GameObject Parent
		{
			get
			{
				return parent;
			}
		}

		public LBActionManager Manager
		{
			get 
			{
				return manager;
			}
		}

		protected void Activate()
		{
			action_state = LBActionStates.Active;
			RaiseEvenet_OnActionActivated ();
		}

		protected void Deactivate()
		{
			action_state = LBActionStates.Inactive;
			RaiseEvenet_OnActionDeactivated ();
		}

		protected virtual bool CheckActivationConditions ()
		{
			return true;
		}

		protected virtual bool CanActivateAction (bool _is_internal) // вызывается изнутри
		{
			if ((_is_internal && ((ActionActivation & LBActionActivationTypes.Internal) != 0)) || (!_is_internal && ((ActionActivation & LBActionActivationTypes.External) != 0)))
			{
				//if activation is performed from inside and it is internally-activated or activation is performed from outside and it is externally-activated
				if (action_state == LBActionStates.Inactive)
				{
					//if action state is inactive (ready to activate)
					if ((((byte)ActionActivation & (byte)128) != 0 && CheckActivationConditions ()) || ((byte)ActionActivation & (byte)128) == 0)
						//if action is non-conditional or conditions are met
						return true;
				}
			}

			return false;
		}

		public virtual bool CanActivateAction () // вызывается извне
		{
			return CanActivateAction (false);
		}

		public virtual bool ActivateAction ()
		{			
			if (CanActivateAction (false))
			{
				Activate ();
				return true;
			}
			else
				return false;
		}

//		protected virtual bool ActivateAction(bool _is_internal)
//		{
//			if (CanActivateAction (_is_internal))
//			{
//				action_state = LBActionStates.Active;
//				RaiseEvenet_OnActionActivated ();
//				return true;
//			}
//
//			return false;
//		}
			
		protected virtual bool CheckDeactivationConditions ()
		{
			return true;
		}

		protected virtual bool CanDeactivateAction (bool _is_internal)
		{
			if ((_is_internal && ((ActionDeactivation & LBActionActivationTypes.Internal) != 0)) || (!_is_internal && ((ActionDeactivation & LBActionActivationTypes.External) != 0)))
			{
				//if activation is performed from inside and it is internally-activated or activation is performed from outside and it is externally-activated
				if (action_state == LBActionStates.Active)
				{
					//if action state is inactive (ready to activate)
					if ((((byte)ActionDeactivation & (byte)128) != 0 && CheckDeactivationConditions ()) || ((byte)ActionDeactivation & (byte)128) == 0)
						//if action is non-conditional or conditions are met
						return true;
				}
			}

			return false;
		}

		public virtual bool CanDeactivateAction ()
		{
			return CanDeactivateAction (false);
		}
			
//		protected virtual bool DeactivateAction (bool _is_internal)
//		{
//			if (CanDeactivateAction (_is_internal))
//			{
//				action_state = LBActionStates.Inactive;
//				RaiseEvenet_OnActionDeactivated ();
//				return true;
//			}
//
//			return false;
//		}

		public virtual bool DeactivateAction ()
		{
			if (CanDeactivateAction (false))
			{
				Deactivate ();
				return true;
			}

			return false;
		}

		public virtual void Tick ()
		{
		}

		protected void RaiseEvenet_OnActionActivated ()
		{
			EventHandler<LBActionEventArgs> handler = ActionActivated;

			if (handler != null && ActionActivatedArgs != null)
			{
				handler (this, ActionActivatedArgs);
			}
		}

		protected void RaiseEvenet_OnActionDeactivated ()
		{
			EventHandler<LBActionEventArgs> handler = ActionDeactivated;

			if (handler != null && ActionDeactivatedArgs != null)
			{
				handler (this, ActionDeactivatedArgs);
			}
		}

		protected void ReportProblem (string message)
		{
			Debug.LogWarning ("Problem in action " + ActionName + " (" + this.GetType () + ") " + message);
		}

		public override string ToString ()
		{
			return string.Format ("[{0} ({1}): \n Action activation type is {2} \n" +
				"Action deactivation type is {3} \n Action is currently {4}]", ActionName, this.GetType().Name, ActionActivation, ActionDeactivation, ActionState);
		}
	}
}

