using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterTurnInPlaceAction", menuName = "LBActionSystem/CharacterTurnInPlaceAction")]
	public class LBCharacterTurnInPlaceAction : LBCharacterGenericAction  
	{
		public string TrunLeftAnim90deg;
		public string TrunRightAnim90deg;

		public bool bUseRestraintDirection;
		public LBDirectionRestraint DirectionRestraint;

		public float BaseRotationSpeed = 5.0f;
		public float ThresholdAngle = 10.0f; // A threshold, after which this action is activated (difference between desired rotation and current RB's rotation)

		protected bool bCanSetNewDir = true;
		protected Vector3 TargetDir;

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

			TargetDir = MovementDir; // We should remember this value, because MovementDir may change
			bCanSetNewDir = false;
		}
			
		protected override void Deactivate (LBAction _next, LBActionTransitTypes _transit)
		{
			base.Deactivate (_next, _transit);
			bCanSetNewDir = true;
		}

//		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
//		{
//			base.Activate (_prev, _transit);
//
//			ActivateAnimation ();
//		}

		protected override void ActivateAnimation()
		{
			if (DirectionDifference > 0)
				PlayAnimation (TrunLeftAnim90deg, AnimationLayer, 0, AnimationBlendTime);
			else
				PlayAnimation (TrunRightAnim90deg, AnimationLayer, 0, AnimationBlendTime);
		}

//		protected override void ActivateAnimation()
//		{
//			
//			if (AnimationTrasnitionType == LBAnimationTransitionTypes.Play)
//			{
//				PlayAnimation(AnimationName, AnimationLayer, 0);
//			}
//			else
//			{
//				CrossfadeAnimation(AnimationName, AnimationLayer, AnimationBlendTime);
//			}
//		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) 
		{
			if (_dir == LBActionTransitDirection.In)
			{
				if (bCanTurnInPlace ())
					return true;
				else
					return false;
			}
			else
			{
				if (!bCanTurnInPlace ())
					return true;
				else
					return false;
			}
		}

		protected override bool CheckSelfDeactivationCondtions ()
		{
			if (!bCanTurnInPlace ())
				return true;
			else
				return false;
		}

		protected bool bCanTurnInPlace()
		{
			bool b;

			b = true;

			b = b && bHasWalkableFloor ();

			b = b && bHasDirDifference ();

			if (bUseRestraintDirection)
				b = b && bHasPropperTransferDirection ();

			return b;
		}
			
		public bool bHasPropperTransferDirection()
		{
			if (CheckDirectionRestraint (DirectionDifference, DirectionRestraint))
				return true;

			return false;
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

		protected bool bDirHasChanged()
		{
			if (TargetDir != MovementDir)
				return true;
			else
				return false;
		}

		protected bool bHasDirDifference()
		{
			float ang = Vector3.SignedAngle (new Vector3 (RBForwardDir.x, 0, RBForwardDir.z), new Vector3 (MovementDir.x, 0, MovementDir.z), Vector3.up); // What about coord system?

			if (Mathf.Abs (ang) >= Mathf.Abs (ThresholdAngle))
				return true;

			return false;
		}

		public override LBActionPerformanceTypes ActionPerfomacneType
		{
			get 
			{
				return LBActionPerformanceTypes.PerformOnce;
			}
		}

		public override void SetMovementDir(Vector3 _dir)
		{
			if (bCanSetNewDir)
				MovementDir = _dir;
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

			((LBCharacterTurnInPlaceAction)dup).BaseRotationSpeed = BaseRotationSpeed;
			((LBCharacterTurnInPlaceAction)dup).ThresholdAngle = ThresholdAngle;
			((LBCharacterTurnInPlaceAction)dup).TrunLeftAnim90deg = TrunLeftAnim90deg;
			((LBCharacterTurnInPlaceAction)dup).TrunRightAnim90deg = TrunRightAnim90deg;
		}

	}
}
