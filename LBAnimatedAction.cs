using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LBActionSystem
{
	public enum LBActionAnimationTypes
	{
		Playback, // Transit only after action has ended
		Loop, // Transit during action exectution
	}

	[CreateAssetMenu (fileName = "NewAnimatedAction", menuName = "LBActionSystem/AnimatedAction")]
	public class LBAnimatedAction : LBTransitiveAction
	{
		protected Animator animator = null;

		public string AnimationName = string.Empty;
		public int AnimationLayer = 0;
		public float AnimationBlendTime = 0.1f;
		public LBActionAnimationTypes AnimationType = LBActionAnimationTypes.Playback;

		protected LBAnimatedAction()
		{}

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init(parentgameobject, manager))
				return false;

			animator = parentgameobject.GetComponent<Animator> ();

			if (animator == null) 
			{
				ReportProblem ("animator in " + parentgameobject.ToString () + " not found!");
				return false;
			}

			return true;
		}

		public override bool CanDeactivateAction (bool _isinternal)
		{
//			if (!base.CanDeactivateAction (true))
//				return false;
//
//			if (AnimationType == LBActionAnimationTypes.Playback) 
//			{
//				if (ActionDeactivation == LBActionDectivationTypes.Automatic || ActionDeactivation == LBActionDectivationTypes.ConditionalAutomatic) 
//				{
//					if (animator.GetCurrentAnimatorStateInfo (AnimationLayer).normalizedTime < 1)
//						return false;
//					else
//						return true;
//				}
//			}

			return false;
		}


		protected override bool InputTransferAction(LBAction old_action)
		{
			if (!base.InputTransferAction (old_action))
				return false;

			if (AnimationName != string.Empty)
				animator.CrossFade(AnimationName, AnimationBlendTime);

			return true;
		}

		protected override bool CanOutputTransferAction(LBAction new_action, LBActionTransitTypes transit = LBActionTransitTypes.Switch)
		{
			if (!base.CanDeactivateAction (true)) // как быть с автоматически и вручную активирумыми действиями?
				return false; // if the action is deactivated or inactive, or it has some conditions

			if (transit == LBActionTransitTypes.Interrupt)
				return CheckInterruptAction ();
			else
				return CheckSwitchAction ();
		}

		public override void Tick ()
		{
//			if (AnimationType == LBActionAnimationTypes.Playback && (ActionDeactivation == LBActionDectivationTypes.Automatic || ActionDeactivation == LBActionDectivationTypes.ConditionalAutomatic))
//			{
//				
//			}
			//animator.GetCurrentAnimatorClipInfo()[0].clip.
		}
	}
}