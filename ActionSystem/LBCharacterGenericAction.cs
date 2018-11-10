using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	public enum LBActionAnimationTypes
	{
		Playback, // Transit only after action has ended
		Loop, // Transit during action exectution
	}

	public enum LBActionPerformanceTypes
	{
		PerformOnce, 			
		PerformOnceModal,		
		PerformContinious
	}

	[CreateAssetMenu (fileName = "NewCharacterGenericAction", menuName = "LBActionSystem/CharacterGenericAction")]
	public class LBCharacterGenericAction : LBCharacterMovementAction 
	{
		//public LBActionAnimationTypes AnimationType = LBActionAnimationTypes.Playback;

		public LBActionPerformanceTypes PerformaceType;

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

			//RewindAnimation ();
		}

		// for a generic action -- always true
		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return true;
			}
			else
			{
				if (ActionPerfomacneType == LBActionPerformanceTypes.PerformOnceModal)
					return (AnimationTime >= 1);
			}

			return true;
		}

		protected override bool CheckSelfDeactivationCondtions ()
		{
			return (AnimationName == string.Empty) || (AnimationTime >= 1) &&  (ActionPerfomacneType == LBActionPerformanceTypes.PerformOnce || ActionPerfomacneType == LBActionPerformanceTypes.PerformOnceModal);
		}

//		protected override void TrySelfDeactivate()
//		{
//			if (AnimationName == string.Empty)
//			{
//				DeactivateAction ();
//				return;
//			}
//
//			if (AnimationTime >= 1)
//			{
//				// We need to self-deactivate only if we
//				if (ActionPerfomacneType == LBActionPerformanceTypes.PerformOnce || ActionPerfomacneType == LBActionPerformanceTypes.PerformOnceModal)
//				{
//					DeactivateAction ();
//				}
////				else
////				{
////					RewindAnimation ();
////				}
//			}
//		}

		public LBActionPerformanceTypes ActionPerfomacneType
		{
			get 
			{
				return PerformaceType;
			}
		}

		public override LBAnimationTransitionTypes AnimationTrasnitionType
		{
			get
			{
				return LBAnimationTransitionTypes.Play;
			}
		}

		protected override void PerformMovement () {}

		public override LBAction Duplicate ()
		{
			LBCharacterAnimatedAction dup;

			dup = (LBCharacterGenericAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterGenericAction)dup).PerformaceType = PerformaceType;
		}
	}
}
