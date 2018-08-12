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

	public enum LBActionTransitDirection
	{
		In,
		Out
	}
		
	public enum LBActionTransitPickTypes
	{
		All,
		FirstFound,
		ByCondition
	}

	public struct LBTransferInfo
	{
		public string ActionName;
		public LBActionTransitTypes TransitType;
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
		protected LBAction[] output = new LBAction[0]; // что делать, если output не сможет включиться?

//		protected LBAction[] input_switch;
//		protected LBAction[] input_interrupt;
//
//		protected LBAction[] output_switch;
//		protected LBAction[] output_interrupt;

		public string[] Input;
		public string[] Output;

		public LBActionTransitTypes TransferType = LBActionTransitTypes.Switch; // only works for @TransfersFrom
		//public string[] TransfersFrom = new string[0];
		//public string TransfersTo;

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
			output = FindOutputTransfers ();

			//if (input == null || output == null)
				//return false;

			return true;
		}

		protected virtual void Activate(LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate ();
		}

		protected virtual void Deactivate(LBAction _next, LBActionTransitTypes _transit)
		{
			base.Deactivate ();
		}

	#region CanActivate

//		public bool CanActivateAction (LBAction _prev, bool _is_internal, LBActionTransitTypes _transfer) // общий вызов 
//		{
//			if (!base.CanActivateAction (_is_internal)) // здесь проверяется с учётом _is_internal
//				return false;
//
//			if (!CanTransferIn(_prev)) // здесь проверяется специфичный для LBTransitiveAction переход
//				return false;
//
//			if (_prev is LBTransitiveAction) 
//			{		
//				if (((LBTransitiveAction)_prev).CanTransferOut (this, _transfer)) //достаточно простой проверки возможности переключения
//					return true; // here we commit that we have at least one possible option			
//			} 
//			else 
//			{
//				if (_prev.CanDeactivateAction ())
//					return true;
//			}
//
//			return false;
//		}

		protected override bool CanActivateAction (bool _is_internal)
		{
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (this.CanTransferAction (input [i], TransferType, LBActionTransitDirection.Out))
					return true;
			}

			return false;
		}
			
	#endregion
				
	#region CanDeactivate

//		public virtual bool CanDeactivateAction (LBAction _next, bool _is_internal, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
//		{
//			if (!base.CanDeactivateAction (_is_internal))
//				return false;
//
//			if (!CanTransferOut(_next, _transfer))
//				return false;
//
//			if (_next is LBTransitiveAction) 
//			{		
//				if (((LBTransitiveAction)_next).CanTransferIn(this))
//					return true; // here we commit that we have at least one possible option			
//			} 
//			else 
//			{
//				if (_next.CanActivateAction ())
//					return true;
//			}
//
//			return false;
//		}
			
		protected override bool CanDeactivateAction (bool _is_internal)
		{
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (this.CanTransferAction (input [i], TransferType, LBActionTransitDirection.In))
					return true;
			}

			return false;

		}


	#endregion

	#region Activation

//		protected virtual bool ActivateAction(LBAction _prev, bool _is_internal)
//		{
////			if (!CanActivateAction (_prev, _is_internal))
////					return false;
//
//			if (!CanTransferIn (_prev))
//				return false;
//
//			if (!TransferIn (_prev, TransferType))
//				return false;
//
//			if (_prev is LBTransitiveAction)
//			{
//				if (((LBTransitiveAction)_prev).TransferOut (this, _is_internal, TransferType)) // transfer from this action
//					return true;
//			}
//			else
//			{
//				if (_prev.DeactivateAction ())
//					return true;
//			}
//
//			return false;
//		}

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
		public override bool ActivateAction () //это 100% внешняя активация
		{		
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (this.TransferAction (input [i], TransferType, LBActionTransitDirection.In))
					return true;
			}

			return false;
		}

	#endregion

	#region Deactivation

		//switch, always internal
//		protected virtual bool DeactivateAction(LBAction _next, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
//		{
//			return true;
//		}

		public override bool DeactivateAction () // извне можем только переключить
		{
			int i;

			for (i = 0; i < output.Length; i++) 
			{
				if (this.TransferAction (output [i], TransferType, LBActionTransitDirection.Out))
					return true;
			}

			return false;
		}

//		protected virtual bool DeactivateAction(LBAction _next, bool _is_internal, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
//		{
////			if (!CanDeactivateAction (_next, _is_internal, _transfer))
////				return false;
//
////			if (!CanTransferOut (_next))
////				return false;
//
//			if (_next is LBTransitiveAction)
//			{
//				if (!((LBTransitiveAction)_next).CanTransferIn (this))
//					return false;
//				
//				if (((LBTransitiveAction)_next).TransferIn (this, _transfer)) // почему здесь _transfer наш? всегда ведь switch!
//				{
//					if (!CanTransferOut (_next, _transfer))
//						return false;
//
//					if (TransferOut (_next, _is_internal, _transfer))
//						return true;
//				}					
//			}
//			else
//			{
//				if (!_next.CanActivateAction ())
//					return false;
//				
//				if (_next.ActivateAction ())
//				{
//					ActionDeactivatedArgs = new LBActionTransitionEventArgs (this, _next, _transfer); // we set event args, because @base.DeactivateAction riases the event
//
//					if (!CanTransferOut (_next, _transfer))
//						return false;
//
//					if (TransferOut (_next, _is_internal, _transfer))
//						return true;
//				}
//			}				
//				
//			return false;
//		}

	#endregion

	#region TransferFunctions

		protected virtual bool CanTransferAction (LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir)
		{
			if (_dir == LBActionTransitDirection.In)
			{
				if (!base.CanActivateAction (true)) // if we cannot activate
					return false;
			}
			else
			{
				if (!base.CanDeactivateAction (true)) // if we cannot deactivate
					return false;
			}

			if (_dir == LBActionTransitDirection.In) // trying to transit from _other to this (IN this)
			{
				if (!HasInputConn (_other)) // if we don't have this action connected (doesn't matter switch or interrupt)
					return false; 

				if (_other is LBTransitiveAction)
				{
					LBTransitiveAction other = (LBTransitiveAction)_other;

					if (!other.HasOutputConn (this)) // if _other action is not connected to this
						return false;

					if (_transit == LBActionTransitTypes.Switch)
					{
						if (other.CanSwitch (this))
							return true;
					}
					else
					{
						if (other.CanInterrupt (this))
							return true;
					}
				}
				else
				{
					if (_other.CanDeactivateAction ())
						return true;
				}
			}
			else // trying to transit from this to _other (OUT of this)
			{
				if (_other is LBTransitiveAction)
				{
					LBTransitiveAction other = (LBTransitiveAction)_other;

					if (!other.HasInputConn (this)) // if _other action is not connected to this
						return false;

					if (_transit == LBActionTransitTypes.Switch)
					{
						if (!HasOutputConn (_other)) // if we don't have this action connected (doesn't matter switch or interrupt)
							return false; 
						
						if (CanSwitch (_other))
							return true;
					}
					else
					{
						if (CanInterrupt(_other))
							return true;
					}
				}
				else
				{
					if (_transit == LBActionTransitTypes.Switch)
					{
						if (!CanSwitch (_other))
							return false;
						
						if (_other.CanActivateAction ())
							return true;
					}
				}
			}

			return false;
		}

		protected virtual bool TransferAction (LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir)
		{
			if (_dir == LBActionTransitDirection.In) // transition from _other to this (IN this)
			{
				if (!this.CanTransferAction (_other, _transit, LBActionTransitDirection.In))
					return false;
				
				if (_other is LBTransitiveAction)
				{
					LBTransitiveAction other = (LBTransitiveAction)_other;

					if (other.CanTransferAction (this, _transit, LBActionTransitDirection.Out))
					{
						other.Deactivate (this, _transit);
						this.Activate (other, _transit);

						return true;
					}
				}
				else
				{
					if (_other.CanDeactivateAction ()) // here we are deactivating with an external call
					{
						_other.DeactivateAction ();
						this.Activate (_other, _transit);

						return true;
					}
				}
			}
			else // transition from this to _other (OUT of this)
			{
				if (!this.CanTransferAction (_other, _transit, LBActionTransitDirection.Out))
					return false;

				if (_other is LBTransitiveAction)
				{
					LBTransitiveAction other = (LBTransitiveAction)_other;

					if (other.CanTransferAction (this, _transit, LBActionTransitDirection.In))
					{
						other.Activate (this, _transit);
						this.Deactivate (other, _transit);

						return true;
					}
				}
				else
				{
					if (_other.CanActivateAction ()) // here we are activating with an external call
					{
						_other.ActivateAction ();
						this.Deactivate (_other, _transit);

						return true;
					}
				}
			}

			return false;
		}

		//switch?
//		protected virtual bool CanTransferIn(LBAction _out) // может ли указанный инпут осуществить переход в данный 
//		{
//			
//			
//			return true;
//		}
//
//		//checking for transfer, always internal
//		protected virtual bool CanTransferOut (LBAction _in, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
//		{
//			if (!base.CanDeactivateAction (true)) // проверка, включено ли действие итд
//				return false;
//
//			if (_transfer == LBActionTransitTypes.Interrupt)
//			{
//				if (!CanInterrupt (_in))
//					return false;
//			}
//			else
//			{
//				if (!HasOutputConn (_in))
//					return false;
//
//				if (!CanSwitch (_in))
//					return false;
//			}
//
//			return true;
//		}

//		protected virtual bool TransferIn(LBAction _out, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
//		{
//			if (_out is LBTransitiveAction)
//				ActionActivatedArgs = new LBActionTransitionEventArgs (_out, this, ((LBTransitiveAction)_out).TransferType); // we set event args, because @base.ActivateAction riases the event
//			else
//				ActionActivatedArgs = new LBActionTransitionEventArgs (_out, this, LBActionTransitTypes.Interrupt); // we set event args, because @base.ActivateAction riases the event
//
//			if (!base.ActivateAction (true))
//				return false;
//
//			return true;
//		}

//		protected virtual bool TransferOut(LBAction _in, bool _is_internal, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
//		{
//			ActionDeactivatedArgs = new LBActionTransitionEventArgs (this, _in, _transfer); // we set event args, because @base.DeactivateAction riases the event
//
//			if (base.DeactivateAction (_is_internal))
//				return true;
//
//			return false;
//		}

	#endregion

	#region TransferConditions

		/// <summary>
		/// This function is called from <c>CanSwitchAction</c>, when <c>transit == LBActionTransitTypes.Interrupt</c>.
		/// </summary>
		/// <remarks>
		/// Overrided in derived classes.
		/// </remarks>
		/// <returns><c>true</c> if this action can be switched; otherwise, <c>false</c>.</returns>
		protected virtual bool CanInterrupt(LBAction new_action)
		{
			return base.CanDeactivateAction (true);
		}

		/// <summary>
		/// This function is called from <c>CanSwitchAction</c>, when <c>transit == LBActionTransitTypes.Switch</c>. 
		/// </summary>
		/// <remarks>
		/// Overrided in derived classes.
		/// </remarks>
		/// <returns><c>true</c> if this instance can switch action; otherwise, <c>false</c>.</returns>
		protected virtual bool CanSwitch(LBAction new_action)
		{
			return base.CanDeactivateAction (true);
		}

	#endregion
			
		protected bool HasInputConn(LBAction action)
		{
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (input [i] == action)
					return true;
			}

			return false;
		}

		protected bool HasOutputConn(LBAction action)
		{
			int i;

			for (i = 0; i < output.Length; i++) 
			{
				if (output [i] == action)
					return true;
			}

			return false;
		}

//		protected bool CanTransferFrom(LBAction prev_action)
//		{
//			int i;
//
//			for (i = 0; i < input.Length; i++) 
//			{
//				if (input [i] == prev_action)
//					return true;
//			}
//
//			return false;
//		}

//		protected bool CanTransferFrom(string prev_action)
//		{
//			int i;
//
//			for (i = 0; i < TransfersFrom.Length; i++) 
//			{
//				if (TransfersFrom [i] == prev_action)
//					return true;
//			}
//
//			return false;
//		}
			
		LBAction[] FindInputTransfers(LBActionTransitTypes _transit)
		{
			int i, j, k;

			LBAction[] actions;
			LBAction[] res = new LBAction[0];

			actions = manager.AllActions;

			k = 0;

			for (i = 0; i < actions.Length; i++) 
			{
				for (j = 0; j < Input.Length; j++)
				{
					if (actions [i].ActionName == Input [j]) 
					{
						k++;
						Array.Resize (ref res, k);
						res [k - 1] = actions [i];
					}
				}
			}

			return res;
		}

		LBAction[] FindInputTransfers()
		{
			int i, j, k;

			LBAction[] actions;
			LBAction[] res = new LBAction[0];

			if (Input == null || Input.Length == 0)
				return res;

			actions = manager.AllActions;

			k = 0;

			for (i = 0; i < actions.Length; i++) 
			{
				for (j = 0; j < Input.Length; j++)
				{
					if (actions [i].ActionName == Input [j]) 
					{
						k++;
						Array.Resize (ref res, k);
						res [k - 1] = actions [i];
					}
				}
			}

			return res;
		}


		LBAction[] FindOutputTransfers()
		{
			int i, j, k;

			LBAction[] actions;
			LBAction[] res = new LBAction[0];

			if (Output == null || Output.Length == 0)
				return res;

			actions = manager.AllActions;

			k = 0;

			for (i = 0; i < actions.Length; i++) 
			{
				for (j = 0; j < Output.Length; j++)
				{
					if (actions [i].ActionName == Output [j]) 
					{
						k++;
						Array.Resize (ref res, k);
						res [k - 1] = actions [i];
					}
				}
			}

			return res;
		}


		LBAction[] FindOutputTransfers(LBActionTransitTypes _transit)
		{
			int i, j, k;

			LBAction[] actions;
			LBAction[] res = new LBAction[0];

			actions = manager.AllActions;

			k = 0;

			for (i = 0; i < actions.Length; i++) 
			{
				for (j = 0; j < Output.Length; j++)
				{
					if (actions [i].ActionName == Output [j]) 
					{
						k++;
						Array.Resize (ref res, k);
						res [k - 1] = actions [i];
					}
				}
			}

			return res;
		}

//		LBAction[] FindInputTransfers()
//		{
//			int i, j, k;
//
//			LBAction[] actions;
//			LBAction[] res = new LBAction[0];
//
//			actions = manager.AllActions;
//
//			k = 0;
//
//			for (i = 0; i < actions.Length; i++) 
//			{
//				for (j = 0; j < TransfersFrom.Length; j++) 
//				{
//					if (actions [i].ActionName == TransfersFrom [j]) 
//					{
//						k++;
//						Array.Resize (ref res, k);
//
//						res[k-1] = actions[i];
//					}
//				}
//			}
//
//			return res;
//		}

//		LBAction FindOutputTransfer()
//		{
//			int i;
//
//			LBAction[] actions;
//
//			actions = manager.AllActions;
//
//			for (i = 0; i < actions.Length; i++) 
//			{
//				if (actions [i].ActionName == TransfersTo) 
//				{
//					return actions [i];
//				}
//			}
//		
//			return null;
//		}
	
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

			((LBTransitiveAction)dup).Input = Input;
			((LBTransitiveAction)dup).Output = Output;
			((LBTransitiveAction)dup).TransferType = TransferType;
		}
	}
}
