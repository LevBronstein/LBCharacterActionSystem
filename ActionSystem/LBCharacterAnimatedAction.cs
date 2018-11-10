using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LBActionSystem
{
	public enum LBAnimationTransitionTypes
	{
		Play,
		Crossfade
	}

	[CreateAssetMenu (fileName = "NewCharacterAnimatedAction", menuName = "LBActionSystem/CharacterAnimatedAction")]
	public class LBCharacterAnimatedAction : LBTransitiveAction
	{
		protected Animator animator = null;

		public string AnimationName = string.Empty;
		public int AnimationLayer = 0;
		public float AnimationBlendTime = 0.1f;

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
//				if (((LBCharacterAnimatedAction)_prev) != null)
//				{
//					string prevanim = ((LBCharacterAnimatedAction)_prev).AnimationName;
//					int prevanimlayer = ((LBCharacterAnimatedAction)_prev).AnimationLayer;
//
//					if (prevanim != string.Empty)
//					{
//						float prevanimtime = AnimationTime;
//						RewindAnimation (); // start new animation at zero frame
//						animator.Play (prevanim, prevanimlayer, prevanimtime); // return to the old animation at old time
//					}
//				}

				if (AnimationTrasnitionType == LBAnimationTransitionTypes.Play)
				{
					animator.Play (AnimationName, AnimationLayer, 0); // we'll just play the anim form the start
				}
				else
				{
					animator.CrossFade (AnimationName, AnimationBlendTime);
				}
			}
		}
			
		public float AnimationTime
		{
			get 
			{
				if (animator != null) // We don't know, which animation we're playing currently
					return Mathf.Clamp01 (animator.GetCurrentAnimatorStateInfo (AnimationLayer).normalizedTime);
				else
					return 1;
			}
		}

		public virtual LBAnimationTransitionTypes AnimationTrasnitionType
		{
			get
			{
				return LBAnimationTransitionTypes.Play;
			}
		}

		protected void RewindAnimation()
		{
			animator.Play (AnimationName, AnimationLayer, 0);
		}


		public override LBAction Duplicate ()
		{
			LBCharacterAnimatedAction dup;

			dup = (LBCharacterAnimatedAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterAnimatedAction)dup).AnimationName = AnimationName;
			((LBCharacterAnimatedAction)dup).AnimationLayer = AnimationLayer;
			((LBCharacterAnimatedAction)dup).AnimationBlendTime = AnimationBlendTime;
		}
	
	}
}