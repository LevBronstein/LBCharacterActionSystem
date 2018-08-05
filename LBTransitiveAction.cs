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
			
		public bool CanActivateAction (LBAction _prev, bool _is_internal)
		{
			if (!base.CanActivateAction (_is_internal)) // check conditions in the base class (проверяеися несколько раз!)
				return false;

			if (_prev == null)
				return false;

			if (TransfersFrom == null || TransfersFrom.Length == 0)
				return false;

			if (!CanTransferFrom (_prev))
				return false;

			if (_prev is LBTransitiveAction) 
			{		
				if (((LBTransitiveAction)_prev).CanDeactivateAction (this, true, TransferType))
					return true; // here we commit that we have at least one possible option			
			} 
			else 
			{
				if (_prev.ActionState == LBActionStates.Active) 
				{
					if (_prev.CanDeactivateAction (true))
						return true;
				}
			}

			return false;
		}

		public override bool CanActivateAction (bool _is_internal)
		{
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (CanActivateAction (input [i], _is_internal))
					return true; //if any of input is availiable to transfer
			}

			return false;
		}


		protected virtual bool ActivateAction(LBAction _prev, bool _is_internal)
		{
//			if (!CanActivateAction (_prev, _is_internal))
//				return false;

			ActionActivatedArgs = new LBActionTransitionEventArgs (_prev, this, TransferType); // we set event args, because @base.ActivateAction riases the event

			if (!base.ActivateAction (_is_internal))
				return false;

			if (_prev is LBTransitiveAction)
			{
				if (((LBTransitiveAction)_prev).DeactivateAction (this, true, TransferType)) // transfer from this action
					return true;
			}
			else
			{
				if (_prev.DeactivateAction())
					return true;
			}

			return false;
		}

		protected override bool ActivateActionInternal()
		{
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (ActivateAction (input [i], true))
					return true;
			}

			return false;
		}

		public override bool ActivateAction ()
		{			
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (ActivateAction (input [i], false))
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
		protected override bool ActivateAction (bool _is_internal) //это 100% внешняя активация
		{		
//			if (!base.ActivateAction (_is_internal))
//				return false;

			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (ActivateAction (input [i], _is_internal))
					return true;
			}

			return false;
		}
					
		public virtual bool CanDeactivateAction (LBAction _next, bool _is_internal, LBActionTransitTypes transit = LBActionTransitTypes.Switch)
		{
			if (!base.CanDeactivateAction (_is_internal))
				return false;
			
			if (transit == LBActionTransitTypes.Interrupt)
			{
				if (!CheckInterruptAction (_next))
					return false;
			}
			else
			{
				if (!CheckSwitchAction (_next))
					return false;
			}

			if (_next is LBTransitiveAction) 
			{		
				if (((LBTransitiveAction)_next).CanActivateAction(this, _is_internal))
					return true; // here we commit that we have at least one possible option			
			} 
			else 
			{
				if (_next.ActionState == LBActionStates.Inactive) 
				{
					if (_next.CanActivateAction (true))
						return true;
				}
			}

			return false;
		}

		public override bool CanDeactivateAction (bool _is_internal)
		{
			if (output == null)
				return false;

			if (output is LBTransitiveAction)
			{
				if (CanDeactivateAction (output, _is_internal, ((LBTransitiveAction)output).TransferType))
					return true; 
			}
			else
			{
				if (CanDeactivateAction (output, _is_internal, LBActionTransitTypes.Switch))
					return true;
			}
				
			return false;
		}

		protected virtual bool DeactivateAction(LBAction _next, bool _is_internal, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
		{
			if (_next is LBTransitiveAction)
			{
				ActionDeactivatedArgs = new LBActionTransitionEventArgs (this, _next, _transfer); // we set event args, because @base.DeactivateAction riases the event

				if (base.DeactivateAction (_is_internal))
				{
					if (((LBTransitiveAction)_next).ActivateAction (this, true))
					{
						return true;
					}
				}
			}
			else
			{
				ActionDeactivatedArgs = new LBActionTransitionEventArgs (this, _next, _transfer); // we set event args, because @base.DeactivateAction riases the event

				if (base.DeactivateAction (_is_internal))
				{
					if (_next.ActivateAction ())
					{
						return true;
					}
				}
			}

			return false;
		}
			
		protected override bool DeactivateActionInternal()
		{
			if (output is LBTransitiveAction)
				return DeactivateAction (output, true, ((LBTransitiveAction)output).TransferType); //when deactivating from internal -- always switch
			else 
				return DeactivateAction (output, true, LBActionTransitTypes.Switch);
		}

		public override bool DeactivateAction ()
		{
			if (output is LBTransitiveAction)
				return DeactivateAction (output, false, ((LBTransitiveAction)output).TransferType); //when deactivating from internal -- always switch
			else 
				return DeactivateAction (output, false, LBActionTransitTypes.Switch);
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
