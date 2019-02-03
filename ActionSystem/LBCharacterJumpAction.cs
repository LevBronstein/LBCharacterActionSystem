using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterJumpAction", menuName = "LBActionSystem/CharacterJumpAction")]
	public class LBCharacterJumpAction : LBCharacterGenericAction 
	{
		public bool bPreserveSpeed;

		protected bool bhasimpulse;

		protected Vector3 origvel;

		public override bool ActivateAction () // гемор?
		{
			bHasImpulse = true;

			if (!base.ActivateAction ())
			{
				bHasImpulse = false;	
				return false;
			}

			return true;
		}

//		protected override bool TransferAction (LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // гемор?!
//		{
//			if (_dir == LBActionTransitDirection.Out)
//				bHasImpulse = false;
//			
//			return base.TransferAction (_other, _transit, _dir);
//		}

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			//LBCharacterMovementAction mov;
			base.Activate (_prev, _transit);

//			mov = (LBCharacterMovementAction)_prev;
//
//			if (mov != null)
//			{
//				SetMovementSpeed (RBSpeed);
//				SetMovementDir (RBSpeedDir);
//			}

			if (bPreserveSpeed)
			{
				origvel = RBSpeedVector;
			}
		}

		public override bool DeactivateAction ()
		{
			if (bPreserveSpeed)
			{
				RBSpeedVector = origvel;
			}

			return base.DeactivateAction ();
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) 
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return base.CheckTransferConditions (_other, _transit, LBActionTransitDirection.In) && bHasWalkableFloor () && bHasImpulse;
			}
			else
			{
				return base.CheckTransferConditions (_other, _transit, LBActionTransitDirection.Out);
			}
		}

		public override LBAnimationTransitionTypes AnimationTrasnitionType
		{
			get
			{
				return LBAnimationTransitionTypes.Crossfade;
			}
		}

		protected override void PerformMovement () 
		{
			if (bPreserveSpeed)
			{
				//rigidbody.velocity = origspeeddir.normalized * origspeed;
//				if (MovementDir != Vector3.zero)
//					rigidbody.rotation = Quaternion.LookRotation (MovementDir);
			}
		}

		public bool bHasImpulse
		{
			set 
			{
				bhasimpulse = value;
			}
			get 
			{
				return bhasimpulse;
			}
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

			((LBCharacterJumpAction)dup).bPreserveSpeed = bPreserveSpeed;
		}
	
	}
}