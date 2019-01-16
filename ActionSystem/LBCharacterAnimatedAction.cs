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

	[System.Serializable]
	public struct LBAnimationInfo
	{
		public string AnimationName;
		public int AnimationLayer;

		public LBAnimationInfo (string animationn_name, int animation_layer)
		{
			AnimationName = animationn_name;
			AnimationLayer = animation_layer;
		}
	}

	[CreateAssetMenu (fileName = "NewCharacterAnimatedAction", menuName = "LBActionSystem/CharacterAnimatedAction")]
	public class LBCharacterAnimatedAction : LBTransitiveAction
	{
		protected Animator animator = null;

		protected string curanimname;
		protected int curanimlayer;
		protected float startanimtime;


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

			ActivateAnimation ();
		}

		protected virtual void ActivateAnimation()
		{
			PlayAnimation (AnimationName, AnimationLayer, 0, AnimationBlendTime);
		}

		protected void PlayAnimation(string anim, int layer = 0, float offset = 0, float blend = 0.1f)
		{
			startanimtime = animator.GetCurrentAnimatorStateInfo (AnimationLayer).normalizedTime;

			if (anim == string.Empty)
				return;
			
			curanimname = AnimationName;
			curanimlayer = AnimationLayer;

			if (AnimationTrasnitionType == LBAnimationTransitionTypes.Play)
			{
				animator.Play (anim, layer, offset);
			}
			else
			{
				animator.CrossFade (anim, blend, layer);
			}
		}
			
		protected override void TickActive ()
		{
			if (animator.GetCurrentAnimatorStateInfo (curanimlayer).normalizedTime < startanimtime)
				startanimtime = 0;
			
			base.TickActive ();
		}

		public float AnimationTime
		{
			get 
			{
				if (animator != null) // We don't know, which animation we're playing currently
				{
					//animator.GetCurrentAnimatorStateInfo (AnimationLayer).ToString ();
					if (animator.GetCurrentAnimatorStateInfo (curanimlayer).normalizedTime > startanimtime)
						return animator.GetCurrentAnimatorStateInfo (curanimlayer).normalizedTime;
					else
						return 0;
				}
				else
					return startanimtime+2;
			}
		}

		public int AnimationExtraLoops
		{
			get 
			{
				return (int) (animator.GetCurrentAnimatorStateInfo (curanimlayer).normalizedTime - startanimtime);
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
			animator.Play (curanimname, curanimlayer, 0);
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