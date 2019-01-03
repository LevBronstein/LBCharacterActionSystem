using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterTurnInPlaceAction", menuName = "LBActionSystem/CharacterTurnInPlaceAction")]
	public class LBCharacterTurnInPlaceAction : LBCharacterGenericAction  
	{
		public float BaseRotationSpeed = 5.0f;
		public float ThresholdAngle = 10.0f; // A threshold, after which this action is activated (difference between desired rotation and current RB's rotation)

		protected Vector3 TargetDir;

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

			TargetDir = MovementDir; // We should remember this value, because MovementDir may change
		}
			
		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) 
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return base.CheckTransferConditions (_other, _transit, LBActionTransitDirection.In) && bHasWalkableFloor () && bHasDirDifference ();
			}
			else
			{
				return base.CheckTransferConditions (_other, _transit, LBActionTransitDirection.Out) && !bHasDirDifference ();
			}
		}

		protected override bool CheckSelfDeactivationCondtions ()
		{
			return base.CheckSelfDeactivationCondtions () || !bHasWalkableFloor () || !bHasDirDifference ();
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
			float destrot = Vector3.SignedAngle  (new Vector3 (RBForwardDir.x, 0, RBForwardDir.z), new Vector3 (MovementDir.x, 0, MovementDir.z), Vector3.up);
			float currot = LerpFloat (0, TruncFloat(destrot,1), BaseRotationSpeed, Time.fixedTime);

			rigidbody.rotation = Quaternion.LookRotation (Quaternion.AngleAxis (TruncFloat(currot), Vector3.up) * RBForwardDir);

//			if (Mathf.Abs (ang) >= Mathf.Abs (ThresholdAngle))
//			{
//				currot = LerpFloat (0, TruncFloat(ang,1), BaseRotationSpeed, Time.fixedTime);
//				rigidbody.rotation = Quaternion.LookRotation (Quaternion.AngleAxis (TruncFloat(currot), Vector3.up) * RBForwardDir);
//			}
		}

		protected bool bHasDirDifference()
		{
			float ang = Vector3.SignedAngle (new Vector3 (RBForwardDir.x, 0, RBForwardDir.z), new Vector3 (MovementDir.x, 0, MovementDir.z), Vector3.up); // What about coord system?

			if (Mathf.Abs (ang) >= Mathf.Abs (ThresholdAngle))
				return true;

			return false;
		}

		public override LBAction Duplicate ()
		{
			LBCharacterTurnInPlaceAction dup;

			dup = (LBCharacterTurnInPlaceAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterTurnInPlaceAction)dup).ThresholdAngle = ThresholdAngle;
		}

	}
}
