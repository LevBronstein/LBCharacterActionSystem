using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterMovementAction", menuName = "LBActionSystem/CharacterMovementAction")]
	public class LBCharacterMovementAction : LBMovementAction 
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

		public override LBAction Duplicate ()
		{
			LBCharacterMovementAction dup;

			dup = (LBCharacterMovementAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterMovementAction)dup).AnimationName = AnimationName;
			((LBCharacterMovementAction)dup).AnimationLayer = AnimationLayer;
			((LBCharacterMovementAction)dup).AnimationBlendTime = AnimationBlendTime;
			((LBCharacterMovementAction)dup).AnimationType = AnimationType;
		}
	}
}