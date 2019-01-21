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

//	public enum LBActionMovmenetOverride
//	{
//		DisableMovement, // No movement is performed during action (outer forces still work)	
//		OverrideMovement, // Movement is performed during action, however @PerformOverridenMovement is used
//		KeepMovement // The acton's class base @PerformMovement is used
//	}

	[CreateAssetMenu (fileName = "NewCharacterGenericAction", menuName = "LBActionSystem/CharacterGenericAction")]
	public class LBCharacterGenericAction : LBCharacterGroundMovementAction 
	{
		//public LBActionAnimationTypes AnimationType = LBActionAnimationTypes.Playback;

		//public LBActionPerformanceTypes PerformaceType;

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
				if (ActionPerfomacneType == LBActionPerformanceTypes.PerformOnceModal && AnimationExtraLoops >= 1)
					return true;
				else if (ActionPerfomacneType == LBActionPerformanceTypes.PerformOnce)
					return true;
			}

			return false;
		}

		protected override bool CheckSelfDeactivationCondtions ()
		{
			if (AnimationName == string.Empty)
				return true;

			if ((AnimationExtraLoops >= 1) &&  (ActionPerfomacneType == LBActionPerformanceTypes.PerformOnce || ActionPerfomacneType == LBActionPerformanceTypes.PerformOnceModal))
				return  true;

			return false;
		}

		protected override void TickActive ()
		{
			Debug.Log ("Current animation: "+ AnimationName + " Current time: " + AnimationTime + " Started at: "+ startanimtime);
			base.TickActive ();
		}

		protected override void PerformMovement () { }

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

		public virtual LBActionPerformanceTypes ActionPerfomacneType
		{
			get 
			{
				return LBActionPerformanceTypes.PerformOnceModal;
			}
		}

		public override LBAnimationTransitionTypes AnimationTrasnitionType
		{
			get
			{
				return LBAnimationTransitionTypes.Play;
			}
		}

//		public virtual LBActionMovmenetOverride MovmenetOverride
//		{
//			get 
//			{
//				return LBActionMovmenetOverride.DisableMovement;
//			}
//		}
//
//		protected override void PerformMovement () 
//		{
//			if (MovmenetOverride == LBActionMovmenetOverride.KeepMovement)
//				base.PerformMovement ();
//			else if (MovmenetOverride == LBActionMovmenetOverride.OverrideMovement)
//				Per
//		}

//		protected virtual void PerformOverridenMovement()
//		{
//			
//		}

//		public override LBAction Duplicate ()
//		{
//			LBCharacterAnimatedAction dup;
//
//			dup = (LBCharacterGenericAction)CreateInstance(this.GetType());
//			DuplicateProperties (dup);
//
//			return dup;
//		}
//
//		protected override void DuplicateProperties(LBAction dup)
//		{
//			base.DuplicateProperties (dup);
//
//			((LBCharacterGenericAction)dup).PerformaceType = PerformaceType;
//		}
	}
}
