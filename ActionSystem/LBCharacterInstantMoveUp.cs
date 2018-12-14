using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterInstantMoveUp", menuName = "LBActionSystem/CharacterInstantMoveUp")]
	public class LBCharacterInstantMoveUp : LBCharacterMovementAction 
	{
		//public float JumpInstantUpSpeed;

		public float JumpHeight; // How high will can we jump
		public float JumpSpeed; // How fast will we jump

		protected float jumpheight;

		protected bool bUseGravity;

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

//			Vector3 v = RBSpeedVector;
//
//			v.y = JumpInstantUpSpeed;
//
//			rigidbody.velocity = v;

			jumpheight = 0;
			bUseGravity = rigidbody.useGravity;
			rigidbody.useGravity = false;
		}

		protected override void Deactivate(LBAction _next, LBActionTransitTypes _transit)
		{
			base.Deactivate ();

			rigidbody.useGravity = bUseGravity;
		}

		protected override void PerformMovement ()
		{
			if (!bHasFinishedJump ())
			{
				jumpheight += Mathf.Abs (JumpSpeed) * Time.fixedDeltaTime;
				Vector3 v = RBSpeedVector;
				v.y = JumpSpeed;
				rigidbody.velocity = v;
			}
			else
				rigidbody.useGravity = true;
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return bHasWalkableFloor ();
			}
			else
			{
				return bHasFinishedJump ();
			}
		}

		protected override bool CheckSelfDeactivationCondtions ()
		{
			return bHasFinishedJump ();
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

		protected bool bHasFinishedJump ()
		{
			if (jumpheight >= JumpHeight)
				return true;
			else
				return false;
		}

		public override LBAction Duplicate ()
		{
			LBCharacterInstantMoveUp dup;

			dup = (LBCharacterInstantMoveUp)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterInstantMoveUp)dup).JumpHeight = JumpHeight;
			((LBCharacterInstantMoveUp)dup).JumpSpeed = JumpSpeed;
		}
	
	}
}
