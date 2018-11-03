using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterAnimAction", menuName = "LBActionSystem/CharacterAnimAction")]
	public class LBCharacterAnimAction : LBCharacterMovementAction 
	{
		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

			RewindAnimation ();
		}

		protected override void PerformMovement ()
		{
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			return true;
		}

		protected override void TrySelfDeactivate()
		{
			if (AnimationTime >= 1)
			{
				if (AnimationType == LBActionAnimationTypes.Playback)
				{
					DeactivateAction ();
				}
				else
				{
					RewindAnimation ();
				}
			}
		}
	}
}
