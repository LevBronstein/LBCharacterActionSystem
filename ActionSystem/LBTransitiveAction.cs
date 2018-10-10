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

		public string[] Input;
		public string[] Output;

		public LBActionTransitTypes TransferType = LBActionTransitTypes.Switch; // only works for @TransfersFrom

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;

			input = FindInputTransfers ();
			output = FindOutputTransfers ();
		
			return true;
		}

		protected virtual void Activate(LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate ();
		}

		protected virtual void Activate(LBAction[] _prevs, LBActionTransitTypes _transit)
		{
			base.Activate ();
		}

		protected virtual void Deactivate(LBAction _next, LBActionTransitTypes _transit)
		{
			base.Deactivate ();
		}
			
		protected virtual void Deactivate(LBAction[] _nexts, LBActionTransitTypes _transit)
		{
			base.Deactivate ();
		}

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

		// selecting first availaiable input
		protected virtual LBAction[] SelectInputs()
		{
			int i;

			LBAction[] res = new LBAction[0];

			for (i = 0; i < input.Length; i++) 
			{
				if (this.CanTransferAction (input [i], TransferType, LBActionTransitDirection.In))
				{
					Array.Resize (ref res, res.Length + 1);
					res [res.Length - 1] = input [i];
					return res;
				}
			}

			return res;
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
		public override bool ActivateAction () //это 100% внешняя активация
		{		
			LBAction[] inputs = SelectInputs ();

			if (inputs.Length == 0 && input.Length > 0)
				return false;

			if (inputs.Length > 1)
			{
				if (this.TransferMultiAction (inputs, TransferType, LBActionTransitDirection.In))
					return true;
			}
			else if (inputs.Length == 1)
			{
				if (this.TransferAction (inputs [0], TransferType, LBActionTransitDirection.In))
					return true;
			}
			else
			{
				// bug!	
			}

			return false;
		}

		// selecting first available output
		protected virtual LBAction[] SelectOutputs()
		{
			int i;

			LBAction[] res = new LBAction[0];

			for (i = 0; i < output.Length; i++) 
			{
				if (this.CanTransferAction (output [i], TransferType, LBActionTransitDirection.Out))
				{
					Array.Resize (ref res, res.Length + 1);
					res [res.Length - 1] = output [i];
					return res;
				}
			}

			return res;
		}

		public override bool DeactivateAction () // извне можем только переключить
		{
			LBAction[] outputs = SelectOutputs ();

			if (outputs.Length == 0 && output.Length > 0)
				return false;

			if (outputs.Length > 1)
			{
				if (this.TransferMultiAction (outputs, TransferType, LBActionTransitDirection.Out))
					return true;
			}
			else if (outputs.Length == 1)
			{
				if (this.TransferAction (outputs[0], TransferType, LBActionTransitDirection.Out))
					return true;
			}
			else
			{
				// bug!	
			}

			return false;
		}
			
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

				if (this.IsConditionalActivation) // if we need to check some conditions to activate
				{
					if (!this.CheckTransferConditions (_other, _transit, _dir))
						return false;
				}

				if (_other is LBTransitiveAction)
				{
					LBTransitiveAction other = (LBTransitiveAction)_other;

					if (!other.HasOutputConn (this)) // if _other action is not connected to this
						return false;

					if (other.IsConditionalDeactivation && other.CheckTransferConditions (this, _transit, LBActionTransitDirection.Out )  || !other.IsConditionalDeactivation)
						return true;
				}
				else
				{
					if (_other.CanDeactivateAction ())
						return true;
				}
			}
			else // trying to transit from this to _other (OUT of this)
			{
				if (this.IsConditionalDeactivation) // if we need to check some conditions to deactvate
				{
					if (!this.CheckTransferConditions (_other, _transit, _dir))
						return false;
				}

				if (_other is LBTransitiveAction)
				{
					LBTransitiveAction other = (LBTransitiveAction)_other;

					if (!other.HasInputConn (this)) // if _other action is not connected to this
						return false;

					if (_transit == LBActionTransitTypes.Switch && !HasOutputConn (_other))
						return false;

					if (other.IsConditionalActivation && other.CheckTransferConditions (this, _transit, LBActionTransitDirection.In )  || !other.IsConditionalActivation)
						return true;
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

		protected virtual bool TransferMultiAction (LBAction[] _others, LBActionTransitTypes _transit, LBActionTransitDirection _dir)
		{
			int i;
			bool b;

			b = false;

			if (_dir == LBActionTransitDirection.In) // transition from _other to this (IN this)
			{
				for (i = 0; i < _others.Length; i++)
				{
					if (!this.CanTransferAction (_others [i], _transit, LBActionTransitDirection.In))
						return false;

					if (_others [i] is LBTransitiveAction)
					{
						LBTransitiveAction other = (LBTransitiveAction)(_others [i]);

						if (other.CanTransferAction (this, _transit, LBActionTransitDirection.Out))
						{
							other.Deactivate (this, _transit);
							//this.Activate (other, _transit);
							b = true;

							//return true;
						}
					}
					else
					{
						if (_others [i].CanDeactivateAction ()) // here we are deactivating with an external call
						{
							_others [i].DeactivateAction ();
							///this.Activate (_others[i], _transit);
							b = true;

							//return true;
						}
					}
				}

				if (b == true)
				{
					this.Activate (_others, _transit);
					return true;
				}
			}
			else // transition from this to _other (OUT of this)
			{
				for (i = 0; i < _others.Length; i++)
				{
					if (!this.CanTransferAction (_others [i], _transit, LBActionTransitDirection.Out))
						return false;

					if (_others [i] is LBTransitiveAction)
					{
						LBTransitiveAction other = (LBTransitiveAction)(_others [i]);

						if (other.CanTransferAction (this, _transit, LBActionTransitDirection.In))
						{
							other.Activate (this, _transit);
							//this.Deactivate (other, _transit);
							b=true;

							//return true;
						}
					}
					else
					{
						if (_others [i].CanActivateAction ()) // here we are activating with an external call
						{
							_others [i].ActivateAction ();
							//this.Deactivate (_other, _transit);
							b=true;

							//return true;
						}
					}
				}

				if (b == true)
				{
					this.Deactivate (_others, _transit);
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
			
		protected virtual bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
//			if (_dir == LBActionTransitDirection.In)
//				return base.CanActivateAction (true);
//			else 
//				return base.CanDeactivateAction(true);

			return true; //no conditions in this class
		}

		protected int FindInputIndex(LBAction action)
		{
			int i;

			for (i = 0; i < input.Length; i++) 
			{
				if (input [i] == action)
					return i;
			}

			return -1;
		}

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

		protected int FindOutputIndex(LBAction action)
		{
			int i;

			for (i = 0; i < output.Length; i++) 
			{
				if (output [i] == action)
					return i;
			}

			return -1;
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
