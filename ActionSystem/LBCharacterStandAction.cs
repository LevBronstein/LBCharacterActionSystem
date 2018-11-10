using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterStandAction", menuName = "LBActionSystem/CharacterStandAction")]
	public class LBCharacterStandAction : LBCharacterMovementAction 
	{
		protected override void PerformMovement () {}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return bHasWalkableFloor () && bHasPropperSpeed();
			}
			else
			{
				return true;
			}
		}

//		protected override void TrySelfDeactivate()
//		{
//			if (!bHasWalkableFloor () || !bHasPropperSpeed())
//			{
//				DeactivateAction ();
//			}
//		}

		protected bool bHasPropperSpeed()
		{
			if (TruncFloat(rigidbody.velocity.magnitude) == 0)
				return true;
			else
				return false;
		}
	}
}
