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

	#region CanActivate

		public bool CanActivateAction (LBAction _prev, bool _is_internal)
		{
			if (!base.CanActivateAction (_is_internal)) // здесь проверяется с учётом _is_internal
				return false;

			if (!CanTransferIn(_prev)) // здесь проверяется специфичный для LBTransitiveAction переход
				return false;

			if (_prev is LBTransitiveAction) 
			{		
				if (((LBTransitiveAction)_prev).CanTransferOut (this, TransferType)) //достаточно простой проверки возможности переключения
					return true; // here we commit that we have at least one possible option			
			} 
			else 
			{
				if (_prev.CanDeactivateAction (true))
					return true;
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
			
	#endregion
				
	#region CanDeactivate

		public virtual bool CanDeactivateAction (LBAction _next, bool _is_internal, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
		{
			if (!base.CanDeactivateAction (_is_internal))
				return false;

			if (!CanTransferOut(_next, _transfer))
				return false;

			if (_next is LBTransitiveAction) 
			{		
				if (((LBTransitiveAction)_next).CanTransferIn(this))
					return true; // here we commit that we have at least one possible option			
			} 
			else 
			{
				if (_next.CanActivateAction (true))
					return true;
			}

			return false;
		}

		public override bool CanDeactivateAction (bool _is_internal)
		{
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

	#endregion

	#region Activation

		protected virtual bool ActivateAction(LBAction _prev, bool _is_internal)
		{
//			if (!CanActivateAction (_prev, _is_internal))
//					return false;

			if (!CanTransferIn (_prev))
				return false;

			if (_prev is LBTransitiveAction)
			{
				if (((LBTransitiveAction)_prev).TransferOut (this, _is_internal, TransferType)) // transfer from this action
				{
					TransferIn (_prev, TransferType);
					return true;
				}
			}
			else
			{
				if (_prev.DeactivateAction ())
				{
					TransferIn (_prev, LBActionTransitTypes.Switch);
					return true;
				}
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
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (ActivateAction (input [i], _is_internal))
					return true;
			}

			return false;
		}

		protected virtual bool TransferIn(LBAction _out, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
		{
			if (_out is LBTransitiveAction)
				ActionActivatedArgs = new LBActionTransitionEventArgs (_out, this, ((LBTransitiveAction)_out).TransferType); // we set event args, because @base.ActivateAction riases the event
			else
				ActionActivatedArgs = new LBActionTransitionEventArgs (_out, this, LBActionTransitTypes.Switch); // we set event args, because @base.ActivateAction riases the event

			if (!base.ActivateAction (true))
				return false;

			return true;
		}

	#endregion

	#region Deactivation

		//switch, always internal
		protected virtual bool DeactivateAction(LBAction _next, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
		{
			return true;
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

		protected virtual bool DeactivateAction(LBAction _next, bool _is_internal, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
		{
			if (!CanTransferOut (_next, _transfer))
				return false;

			if (_next is LBTransitiveAction)
			{
				if (TransferOut (_next, _is_internal, _transfer))
				{
					if (((LBTransitiveAction)_next).TransferIn (this, _transfer))
					{
						return true;
					}
				}
			}
			else
			{
				if (TransferOut (_next, _is_internal, _transfer))
				{
					if (_next.ActivateAction ())
					{
						ActionDeactivatedArgs = new LBActionTransitionEventArgs (this, _next, _transfer); // we set event args, because @base.DeactivateAction riases the event

						if (base.DeactivateAction (_is_internal))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		protected virtual bool TransferOut(LBAction _in, bool _is_internal, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
		{
			ActionDeactivatedArgs = new LBActionTransitionEventArgs (this, _in, _transfer); // we set event args, because @base.DeactivateAction riases the event

			if (base.DeactivateAction (_is_internal))
				return true;

			return false;
		}

	#endregion

	#region TransferFunctions

		//switch?
		protected virtual bool CanTransferIn(LBAction _in)
		{
			if (!base.CanActivateAction (true)) // проверка, включено ли действие итд
				return false;

			if (!CanTransferFrom (_in)) // проверка, связано ли с данным действием предыдущее
				return false;

			return true;
		}

		//checking for transfer, always internal
		protected virtual bool CanTransferOut (LBAction _out, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
		{
			if (_out != output)
				return false;

			if (_transfer == LBActionTransitTypes.Interrupt)
			{
				if (!CheckInterruptAction (_out))
					return false;
			}
			else
			{
				if (!CheckSwitchAction (_out))
					return false;
			}

			return true;
		}


	#endregion

	#region TransferConditions

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

	#endregion
				
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
