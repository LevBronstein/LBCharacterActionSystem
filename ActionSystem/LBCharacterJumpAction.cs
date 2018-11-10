using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterJumpAction", menuName = "LBActionSystem/CharacterJumpAction")]
	public class LBCharacterJumpAction : LBCharacterMovementAction 
	{
		public float JumpInstantUpSpeed;

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

			Vector3 v = RBSpeedVector;

			v.y = JumpInstantUpSpeed;

			rigidbody.velocity = v;
		}

		protected override void PerformMovement ()
		{
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return bHasWalkableFloor ();
			}
			else
			{
				return bHasPropperTransferOutSpeed();
			}
		}

		protected override bool CheckSelfDeactivationCondtions ()
		{
			return bHasWalkableFloor () ||  bHasPropperTransferOutSpeed();
		}

		protected bool bHasControlImpulse()
		{
			if (TruncFloat (MovementSpeed) > 0)
				return true;
			else
				return false;
		}

		protected bool bHasPropperTransferOutSpeed()
		{
			if (RBSpeedVector.y <= 0)
				return true;
			
			return false;
		}

		public override LBAction Duplicate ()
		{
			LBCharacterJumpAction dup;

			dup = (LBCharacterJumpAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterJumpAction)dup).JumpInstantUpSpeed = JumpInstantUpSpeed;
		}
	
	}
}
