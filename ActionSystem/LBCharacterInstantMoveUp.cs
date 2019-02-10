using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterInstantMoveUp", menuName = "LBActionSystem/CharacterInstantMoveUp")]
	public class LBCharacterInstantMoveUp : LBCharacterGenericAction 
	{
		//public float JumpInstantUpSpeed;

		public float JumpHeight; // How high will can we jump
		public float JumpSpeed; // How fast will we jump
		public float FlatMotionModifier = 1;
		public float MaxFlatSpeed = 1;

		protected float jumpheight;

		protected bool bUseGravity;

		protected Vector3 lastrbvel;

		protected bool bhasrestoredvel;

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

//			Vector3 v = RBSpeedVector;
//
//			v.y = JumpInstantUpSpeed;
//
//			rigidbody.velocity = v;
			lastrbvel = RBSpeedVector;
			jumpheight = 0;
			bUseGravity = rigidbody.useGravity;
			rigidbody.useGravity = false;
			bhasrestoredvel = false;
		}

		protected override void Deactivate(LBAction _next, LBActionTransitTypes _transit)
		{
			base.Deactivate ();

			RestoreOrigVel ();
		}

		protected override void PerformMovement ()
		{
			if (!bHasFinishedJump ())
			{
				jumpheight += Mathf.Abs (JumpSpeed) * Time.fixedDeltaTime;
				rigidbody.velocity = new Vector3 (lastrbvel.x * FlatMotionModifier, JumpSpeed, lastrbvel.z * FlatMotionModifier);
			}
			else
			{
				if (!bhasrestoredvel)
					RestoreOrigVel ();
			}
		}

		protected void RestoreOrigVel()
		{
			if (!bhasrestoredvel)
			{
				rigidbody.useGravity = bUseGravity;
				RBSpeedVector = new Vector3 (lastrbvel.x, 0, lastrbvel.z) + Physics.gravity;
				bhasrestoredvel = true;
			}
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

		protected override void UpdateSliders()
		{
			base.UpdateSliders ();

			if (MaxFlatSpeed !=0)
				SetVelocityParam ((new Vector3(RBSpeedVector.x, 0, RBSpeedVector.z)).magnitude / MaxFlatSpeed);
			else 
				SetVelocityParam (0);
		}

		public override LBAnimationTransitionTypes AnimationTrasnitionType
		{
			get
			{
				return LBAnimationTransitionTypes.Crossfade;
			}
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
			((LBCharacterInstantMoveUp)dup).FlatMotionModifier = FlatMotionModifier;
			((LBCharacterInstantMoveUp)dup).MaxFlatSpeed = MaxFlatSpeed;
		}
	
	}
}
