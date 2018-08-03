using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	public enum LBActionTransitTypes
	{
		/// <summary>
		/// Transition is only performed after action has ended.
		/// </summary>
		Switch,
		/// <summary>
		/// Transit is performed at any moment.
		/// </summary>
		Interrupt, // 
	}
		
	public class LBActionTransitionEventArgs : LBActionEventArgs
	{
		protected LBAction old_action, new_action;
		LBActionTransitTypes transit_type;

		public LBActionTransitionEventArgs (LBAction _old_action, LBAction _new_action, LBActionTransitTypes _transit_type)
		{
			old_action = _old_action;
			new_action = _new_action;
			transit_type = _transit_type;
		}

		public LBAction OldAction
		{
			get
			{
				return old_action;
			}
		}
			
		public LBAction NewAction
		{
			get
			{
				return new_action;
			}
		}

		public LBActionTransitTypes TransitType
		{
			get
			{
				return transit_type;
			}
		}
	}

	public abstract class LBTransitiveAction : LBAction
	{
		protected LBAction[] input = new LBAction[0];
		protected LBAction output; // что делать, если output не сможет включиться?

		public LBActionTransitTypes TransferType = LBActionTransitTypes.Switch; // only works for @TransfersFrom
		public string[] TransfersFrom = new string[0];
		public string TransfersTo;

//		protected LBTransitiveAction()
//		{}
//
//		public LBTransitiveAction (string _name) :
//		base (_name)
//		{}
//
//		public LBTransitiveAction (string _name, LBActionActivationTypes _activation, LBActionActivationTypes _deactivation) : 
//		base (_name, _activation, _deactivation)
//		{}
//
//		public LBTransitiveAction (string _name, LBActionActivationTypes _activation, LBActionActivationTypes _deactivation, LBActionTransitTypes _transfer) : 
//		base (_name, _activation, _deactivation)
//		{
//			TransferType = _transfer;
//		}
//
//		public LBTransitiveAction (string _name, LBActionActivationTypes _activation, LBActionActivationTypes _deactivation, LBActionTransitTypes _transfer, string [] _transfer_from, string _transfer_to) : 
//		base (_name, _activation, _deactivation)
//		{
//			TransferType = _transfer;
//			ActionDeactivation = _deactivation;
//			TransfersFrom = _transfer_from;
//			TransfersTo = _transfer_to;
//		}

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;

			input = FindInputTransfers ();
			output = FindOutputTransfer ();

			//if (input == null || output == null)
				//return false;

			return true;
		}
			
		public override bool CanActivateAction (bool _isinternal)
		{
			if (!base.CanActivateAction (_isinternal)) // check conditions in the base class
				return false;

			int i;

			// if any of input actions can be transfered
			for (i = 0; i < input.Length; i++) 
			{
				if (CanInputTransferAction (input [i]))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Performs activation, however, it now works with <c>LBTransitiveAction</c> and <c>LBAction</c> in different manner.
		/// <para>For <c>LBAction</c>: activation is performed only if this action can be deactivated, otherwise -- activation fails.</para>
		/// <para>For <c>LBTransitiveAction</c>: activations is perfromed by <c>CanSwitchAction</c> considering <c>TransferType</c> value, it can be immediately stopped (interrupted)
		/// gently asked for transfer (switch).</para>
		/// </para>Transfer is performed from the first availiable action, to activate from exact action -- use overload.</para>
		/// <remarks> 
		/// Switch is performed when transfering action has <c>TransferType == TransferType.Switch</c>, interrupt is performed when <c>TransferType == TransferType.Interrupt</c>.
		/// </remarks>
		/// </summary>
		public override bool ActivateAction ()
		{		
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (InputTransferAction (input [i]))
				{
					if (base.ActivateAction ()) //if transfer was a sucsess, activate current action
						return true; // transfer only from one action
				}
			}

			return false;
		}

		public bool ActivateAction (LBAction prev_action)
		{	
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (input[i] == prev_action) 
				{
					if (InputTransferAction (input [i]))
					{
						base.ActivateAction (); //if transfer was a sucsess, activate current action
						return true; // transfer only from one action (first found!)
					}
				}
			}

			return false;
		}

		protected virtual bool CanInputTransferAction(LBAction old_action)
		{
			if (old_action == null)
				return false;

			if (TransfersFrom == null || TransfersFrom.Length == 0)
				return false;

			if (!CanTransferFrom (old_action))
				return false;

			if (old_action is LBTransitiveAction) 
			{		
				if (((LBTransitiveAction)old_action).CanOutputTransferAction (this, TransferType))
					return true; // here we commit that we have at least one possible option			
			} 
			else 
			{
				if (old_action.ActionState == LBActionStates.Active) 
				{
					if (old_action.CanDeactivateAction (true))
						return true;
				}
			}

			return false;
		}
			
		/// <summary>
		/// Determines whether this action can be transfered to the specified action with a given transit type. This condition is checked <c>SwitchAction</c>.
		/// </summary>
		/// <returns><c>true</c> if switching is is possible, otherwise -- <c>false</c>.</returns>
		/// <param name="new_action">An action, which is activated (switched or interrupted) after this one is deactivated.</param>
		/// <param name="transit">Transit type (switch or interrupt).</param>
		protected virtual bool CanOutputTransferAction(LBAction new_action, LBActionTransitTypes transit = LBActionTransitTypes.Switch)
		{
			if (transit == LBActionTransitTypes.Interrupt)
			{
				if (!CheckInterruptAction (new_action))
					return false;
			}
			else
			{
				if (CheckSwitchAction (new_action))
					return false;
			}

			if (new_action is LBTransitiveAction) 
			{		
				if (((LBTransitiveAction)new_action).CanInputTransferAction (this))
					return true; // here we commit that we have at least one possible option			
			} 
			else 
			{
				if (new_action.ActionState == LBActionStates.Inactive) 
				{
					if (new_action.CanActivateAction (true))
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// This function is called from <c>CanSwitchAction</c>, when <c>transit == LBActionTransitTypes.Interrupt</c>.
		/// </summary>
		/// <remarks>
		/// Overrided in derived classes.
		/// </remarks>
		/// <returns><c>true</c> if this action can be switched; otherwise, <c>false</c>.</returns>
		protected virtual bool CheckInterruptAction(LBAction new_action)
		{
			return true;
		}

		/// <summary>
		/// This function is called from <c>CanSwitchAction</c>, when <c>transit == LBActionTransitTypes.Switch</c>. 
		/// </summary>
		/// <remarks>
		/// Overrided in derived classes.
		/// </remarks>
		/// <returns><c>true</c> if this instance can switch action; otherwise, <c>false</c>.</returns>
		protected virtual bool CheckSwitchAction(LBAction new_action)
		{
			return true;
		}

		protected virtual bool InputTransferAction(LBAction old_action)
		{
			if (CanInputTransferAction((old_action)))
			{
				if (old_action is LBTransitiveAction)
				{
					ActionActivatedArgs = new LBActionTransitionEventArgs (old_action, this, TransferType); // we set event args, because @base.ActivateAction riases the event
					((LBTransitiveAction)old_action).OutputTransferAction (this, TransferType); // transfer from this action
					return true;
				}
				else
				{
					if (old_action.CanDeactivateAction(true))
					{
						ActionActivatedArgs = new LBActionTransitionEventArgs (old_action, this, TransferType);	// we set event args, because @base.ActivateAction riases the event
						old_action.DeactivateAction ();
						return true;
					}
				}
			}

			ActionActivatedArgs = null; // is it ok?

			return false;
		}

		/// <summary>
		/// Completion of current action with transition to the specified action.
		/// </summary>
		/// <remarks> 
		/// For LBTransitiveAction type only, this subroutine is called from <c>ActivateAction</c>.
		/// </remarks> 
		/// <param name="new_action">An action to transfer to.</param>
		/// <param name="transit">Transit type (switch or interrupt).</param>
		protected virtual bool OutputTransferAction(LBAction new_action, LBActionTransitTypes transit = LBActionTransitTypes.Switch)
		{
			if (transit == LBActionTransitTypes.Switch) 
			{
				if (base.CanDeactivateAction (true) && output.CanActivateAction (true)) // here we have to check all conditions, etc to be able to switch 
				{
					ActionDeactivatedArgs = new LBActionTransitionEventArgs (this, new_action, TransferType); // we set event args, because @base.DeactivateAction riases the event
					base.DeactivateAction ();
					OutputTransferAction (output, LBActionTransitTypes.Switch);
					return true;
				}
				else
					return false;
			}
			else
			{
				return true; //interrupt is always performed
			}
		}

		protected override bool DeactivateActionInternal()
		{
			if (!base.DeactivateActionInternal())
				return false;

			if (CanOutputTransferAction (output, TransferType))
				OutputTransferAction (output, TransferType);

			return false;
		}

		public override bool DeactivateAction ()
		{
			if (!base.DeactivateAction())
				return false;

			if (CanOutputTransferAction (output, TransferType))
				OutputTransferAction (output, TransferType);

			return false;
		}

		protected virtual void ForceDeactivateAction (LBAction new_action)
		{
			action_state = LBActionStates.Inactive;
			ActionDeactivatedArgs = new LBActionTransitionEventArgs (this, new_action, TransferType); // we set event args, because @base.DeactivateAction riases the event
			RaiseEvenet_OnActionDeactivated();
		}
			
		protected bool CanTransferFrom(LBAction prev_action)
		{
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (input [i] == prev_action)
					return true;
			}

			return false;
		}

		protected bool CanTransferFrom(string prev_action)
		{
			int i;

			for (i = 0; i < TransfersFrom.Length; i++) 
			{
				if (TransfersFrom [i] == prev_action)
					return true;
			}

			return false;
		}

		LBAction[] FindInputTransfers()
		{
			int i, j, k;

			LBAction[] actions;
			LBAction[] res = new LBAction[0];

			actions = manager.AllActions;

			k = 0;

			for (i = 0; i < actions.Length; i++) 
			{
				for (j = 0; j < TransfersFrom.Length; j++) 
				{
					if (actions [i].ActionName == TransfersFrom [j]) 
					{
						k++;
						Array.Resize (ref res, k);

						res[k-1] = actions[i];
					}
				}
			}

			return res;
		}

		LBAction FindOutputTransfer()
		{
			int i;

			LBAction[] actions;

			actions = manager.AllActions;

			for (i = 0; i < actions.Length; i++) 
			{
				if (actions [i].ActionName == TransfersTo) 
				{
					return actions [i];
				}
			}
		
			return null;
		}
	
		public override LBAction Duplicate ()
		{
			LBTransitiveAction dup;

			dup = (LBTransitiveAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);
	
			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBTransitiveAction)dup).TransfersFrom = TransfersFrom;
			((LBTransitiveAction)dup).TransfersTo = TransfersTo;
			((LBTransitiveAction)dup).TransferType = TransferType;
		}
	}
}
