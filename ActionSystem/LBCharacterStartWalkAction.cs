using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[System.Serializable]
	public struct LBGenericTransferOutCondition
	{
		public string ActionName;
		public bool Toggle1;
		public bool bHasToBeFinished; // only transfer if action has successfully finished
	}

	[CreateAssetMenu (fileName = "NewCharacterStartWalkAction", menuName = "LBActionSystem/CharacterStartWalkAction")]
	public class LBCharacterStartWalkAction : LBCharacterGenericAction
	{
		// only two internally activated action
		//public string Succsess; // when the action was performed, switch to this
		//public string Failure; // when the action was not performed, switch to this

		public LBGenericTransferOutCondition[] Conditions;

		//protected LBAction succsess;
		//protected LBAction failure;

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;
			
			//succsess = FindOutputAction (Succsess);
			//failure = FindOutputAction (Failure);

			return true;
		}

		//by default succeeds after animations has finished
		protected virtual bool CheckIfActionSucceed ()
		{
			if (AnimationName == string.Empty)
				return true;

			if ((AnimationTime >= 1) &&  (ActionPerfomacneType == LBActionPerformanceTypes.PerformOnce || ActionPerfomacneType == LBActionPerformanceTypes.PerformOnceModal))
				return  true;

			return false;
		}

		protected bool CheckTransferOutCondition(LBAction _other)
		{
			int id;

			id = FindConditionIndexForOutput (_other);

			// if we don't have any conditions, we're free to transfer
			if (id < 0)
				return true;

			if (CheckCondition (Conditions [id].Toggle1, Conditions [id].bHasToBeFinished, CheckIfActionSucceed ()))
				return true;
			else
				return false;
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return true;
			}
			else
			{
				if (CheckTransferOutCondition (_other))
					return true;
			}

			return false;
		}

//		protected override LBAction[] SelectOutputs()
//		{
//			int i;
//			LBAction[] actions = base.SelectOutputs ();
//
//			if (CheckIfActionSucceed () == true)
//			{
//				for (i = 0; i < actions.Length; i++)
//				{
//					if (actions [i] == succsess)
//						return new LBAction[] { succsess };
//				}
//			}
//
//			return actions;
//		}

		//checks condition to be value if toggle is true
		protected bool CheckCondition(bool toggle, bool condition, bool value)
		{
			return ((value == condition) && toggle) || (!toggle);
		}

		protected int FindConditionIndexForOutput(LBAction action)
		{
			int i;

			for (i = 0; i < Conditions.Length; i++)
			{
				if (Conditions [i].ActionName == action.ActionName)
					return i;
			}

			return -1;
		}

		public override LBAction Duplicate ()
		{
			LBCharacterStartWalkAction dup;

			dup = (LBCharacterStartWalkAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterStartWalkAction)dup).Conditions = new LBGenericTransferOutCondition[Conditions.Length];
			Array.Copy(Conditions, ((LBCharacterStartWalkAction)dup).Conditions, Conditions.Length);
			//((LBCharacterStartWalkAction)dup).Conditions = System.Array.Copy(Conditions, ;
		}
	}
}
