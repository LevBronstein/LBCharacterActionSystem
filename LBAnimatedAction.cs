﻿using System.Collections;
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

		public override bool Init (GameObject _parent, LBActionManager _manager)
		{
			if (!base.Init(_parent, _manager))
				return false;

			animator = _parent.GetComponent<Animator> ();

			if (animator == null) 
			{
				ReportProblem ("animator in " + _parent.ToString () + " not found!");
				return false;
			}

			return true;
		}

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

			if (AnimationName != string.Empty)
			{
				animator.CrossFade (AnimationName, AnimationBlendTime);
				Debug.Log (string.Format ("Transfered to {0}", this.ActionName));
			}
		}

//		protected  override void Activate(LBAction[] _prevs, LBActionTransitTypes _transit)
//		{
//			base.Activate (_prevs, _transit);
//
//			if (AnimationName != string.Empty)
//			{
//				animator.CrossFade (AnimationName, AnimationBlendTime);
//				Debug.Log (string.Format ("Transfered to {0}", this.ActionName));
//			}
//		}

//		protected override bool TransferAction (LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir)
//		{
//			if (!base.TransferAction (_other, _transit, _dir))
//				return false;
//
//			if (_dir == LBActionTransitDirection.In)
//			{
//				animator.CrossFade (AnimationName, AnimationBlendTime);
//				Debug.Log (string.Format ("Transfered to {0}", this.ActionName));
//			}
//				
//			return true;
//		}
						
//		protected override bool DeactivateAction(LBAction _next, bool _is_internal, LBActionTransitTypes _transfer = LBActionTransitTypes.Switch)
//		{
//			if (base.DeactivateAction(_next, _is_internal, _transfer))
//			{
//				if (!(_next is LBAnimatedAction))
//					animator.CrossFade ("", AnimationBlendTime);
//				
//				return true;
//			}
//
//			return false;
//		}
//		protected override bool InputTransferAction(LBAction old_action, bool is_internal)
//		{
//			if (base.InputTransferAction (old_action, is_internal))
//			{
//				animator.CrossFade (AnimationName, AnimationBlendTime);
//				return true;
//			}
//			else
//				return false;
//		}
	
		public override LBAction Duplicate ()
		{
			LBAnimatedAction dup;

			dup = (LBAnimatedAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBAnimatedAction)dup).AnimationName = AnimationName;
			((LBAnimatedAction)dup).AnimationLayer = AnimationLayer;
			((LBAnimatedAction)dup).AnimationBlendTime = AnimationBlendTime;
			((LBAnimatedAction)dup).AnimationType = AnimationType;
		}
	
	}
}