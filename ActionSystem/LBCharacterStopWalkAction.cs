using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterStopWalkAction", menuName = "LBActionSystem/CharacterStopWalkAction")]
	public class LBCharacterStopWalkAction : LBCharacterGenericAction 
	{
		//public LBGenericTransferOutCondition[] OutputConditions;
		//public LBGenericTransferOutCondition[] InputConditions;

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;

			//succsess = FindOutputAction (Succsess);
			//failure = FindOutputAction (Failure);

			return true;
		}

//		//by default succeeds after animations has finished
//		protected virtual bool CheckIfActionSucceed ()
//		{
//			if (AnimationName == string.Empty)
//				return true;
//
//			if ((AnimationTime >= 1) &&  (ActionPerfomacneType == LBActionPerformanceTypes.PerformOnce || ActionPerfomacneType == LBActionPerformanceTypes.PerformOnceModal))
//				return  true;
//
//			return false;
//		}

//		protected bool CheckTransferOutCondition(LBAction _other)
//		{
//			int id;
//
//			id = FindConditionIndexForOutput (_other);
//
//			// if we don't have any conditions, we're free to transfer
//			if (id < 0)
//				return true;
//
//			if (CheckCondition (OutputConditions [id].Toggle1, OutputConditions [id].bHasToBeFinished, CheckIfActionSucceed ()))
//				return true;
//			else
//				return false;
//		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return bCanStopWalk(_other) && bHasWalkableFloor();
			}
			else
			{
				return true;
			}
		}

		protected bool bCanStopWalk(LBAction _other)
		{
			if (_other is LBCharacterWalkAction)
			{
				// we want to start this action only if previous has lost control impulse
				if (!((LBCharacterWalkAction)_other).bHasControlImpulse ())
					return true;
			}
			else
				return true;

			return false;
		}

		//checks condition to be value if toggle is true
		protected bool CheckCondition(bool toggle, bool condition, bool value)
		{
			return ((value == condition) && toggle) || (!toggle);
		}

//		protected int FindConditionIndexForOutput(LBAction action)
//		{
//			int i;
//
//			for (i = 0; i < OutputConditions.Length; i++)
//			{
//				if (OutputConditions [i].ActionName == action.ActionName)
//					return i;
//			}
//
//			return -1;
//		}

//		protected int FindConditionIndexForInput(LBAction action)
//		{
//			int i;
//
//			for (i = 0; i < InputConditions.Length; i++)
//			{
//				if (InputConditions [i].ActionName == action.ActionName)
//					return i;
//			}
//
//			return -1;
//		}

//		public override LBAction Duplicate ()
//		{
//			LBCharacterStopWalkAction dup;
//
//			dup = (LBCharacterStopWalkAction)CreateInstance(this.GetType());
//			DuplicateProperties (dup);
//
//			return dup;
//		}
//
//		protected override void DuplicateProperties(LBAction dup)
//		{
//			base.DuplicateProperties (dup);
//
//			((LBCharacterStopWalkAction)dup).OutputConditions = new LBGenericTransferOutCondition[OutputConditions.Length];
//			Array.Copy(OutputConditions, ((LBCharacterStopWalkAction)dup).OutputConditions, OutputConditions.Length);
//			//((LBCharacterStartWalkAction)dup).Conditions = System.Array.Copy(Conditions, ;
//		}
	}
}
