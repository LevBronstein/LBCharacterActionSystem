using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterFallAction", menuName = "LBActionSystem/CharacterFallAction")]
	public class LBCharacterFallAction : LBCharacterMovementAction 
	{

		protected override void PerformMovement ()
		{
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return true; //!bHasWalkableFloor ();
			}
			else
			{
				return bHasWalkableFloor ();
			}
		}

		protected override void TrySelfActivate()
		{
			if (!bHasWalkableFloor ())
			{
				ActivateAction ();
			}
		}

		protected override void TrySelfDeactivate()
		{
			if (bHasWalkableFloor ())
			{
				DeactivateAction ();
			}
		}

	}
}
