using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	public enum LBActionTransitTypes
	{
		Switch, // Transit only after action has ended
		Interrupt, // Transit during action exectution
	}
		
	public abstract class LBTransitiveAction : LBAction
	{
		protected LBAction[] input = new LBAction[0];
		protected LBAction output;

		public LBActionTransitTypes TransferType = LBActionTransitTypes.Switch; // only works for @TransfersFrom
		public string[] TransfersFrom = new string[0];
		public string TransfersTo;

		public LBTransitiveAction ()
		{}

		public LBTransitiveAction (string _name, LBActionActivationTypes _activation, LBActionDectivationTypes _deactivation) : 
		base (_name, _activation, _deactivation)
		{}

		public LBTransitiveAction (string _name, LBActionActivationTypes _activation, LBActionDectivationTypes _deactivation, LBActionTransitTypes _transfer) : 
		base (_name, _activation, _deactivation)
		{
			TransferType = _transfer;
		}

		public LBTransitiveAction (string _name, LBActionActivationTypes _activation, LBActionDectivationTypes _deactivation, LBActionTransitTypes _transfer, string [] _transfer_from, string _transfer_to) : 
		base (_name, _activation, _deactivation)
		{
			TransferType = _transfer;
			ActionDeactivation = _deactivation;
			TransfersFrom = _transfer_from;
			TransfersTo = _transfer_to;
		}

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;

			input = FindTransferActions ();
			output = FindSwitchAction ();

			if (input == null || output == null)
				return false;

			return true;
		}
			
		public override bool CanActivateAction ()
		{
			if (TransfersFrom == null || TransfersFrom.Length == 0)
				return false;

			if (!base.CanActivateAction ())
				return false;

			int i;

			// If any of input actions are active and can be deactivated
			for (i = 0; i < input.Length; i++) 
			{
				if (input [i].ActionState == LBActionStates.Active) 
				{
					if (TransferType == LBActionTransitTypes.Interrupt)
						return true;
					else // TransferType == LBActionTransitTypes.Switch
					{
						if (input [i].CanDeactivateAction ())
							return true;
					}
				}
			}

			return false;
		}

//		protected bool CanTransfer(string[] active_actions)
//		{
//			int i;
//
//			for (i = 0; i < TransfersFrom.Length; i++) 
//			{
//				if (TransfersFrom [i] == prev_action)
//					return true;
//			}
//		}

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

		LBAction[] FindTransferActions()
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

		LBAction FindSwitchAction()
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
	}
}
