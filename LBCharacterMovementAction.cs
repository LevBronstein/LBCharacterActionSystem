using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{

//	public enum LBRBBaseTypes
//	{
//		Grounded,
//		Airbourne
//	}

	/// <summary>
	/// This enum briefly describes velocity vector direction in local system
	/// </summary>
//	public enum LBVectorDirectionTypes
//	{
//		Zero,
//		ZAxis,
//		XAxis,
//		YAxis
//	}

//	public struct LBMovementTransferCondition
//	{
//		public bool use_VelocityDirectionFilter;
//		public LBVectorDirectionTypes VelocityDirectionFilter;
//		public bool use_RBVelocityLimit;
//		public float RBVelocityLimit;
//		public bool use_RBBasement;
//		public LBRBBaseTypes RBBasement;
//	}

	[CreateAssetMenu (fileName = "NewCharacterMovementAction", menuName = "LBActionSystem/CharacterMovementAction")]
	public class LBCharacterMovementAction : LBMovementAction 
	{
//		public LBMovementTransferCondition[] InputConditions;
//		public LBMovementTransferCondition[] OutputConditions;

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

//		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
//		{
//			int id;
//
//			if (_dir == LBActionTransitDirection.In)
//			{
//				id = FindInputIndex (_other);
//
//				if (id < 0)
//					return false;
//
//				if (id > InputConditions.Length) // if we don't have condition set for this input
//					return true;
//
//				if (CheckMovementTransferConditions(InputConditions[id]))
//					return true;
//				else
//					return false;
//			}
//			else
//			{
//			}
//			return true; //no conditions in this class
//		}
//
//		protected bool CheckMovementTransferConditions(LBMovementTransferCondition _cond)
//		{
//			if (rigidbody.velocity.magnitude > _cond.RBVelocityLimit)
//				return false;
//
//			// trace down to basement
//
//			if (GetParentBasementType () != _cond.RBBasement)
//				return false;
//
//			return true;
//		}
//
//		LBRBBaseTypes GetParentBasementType()
//		{
//			Collider c;
//			Ray r;
//			RaycastHit hit;
//
//			c = parent.GetComponent<Collider>();
//
//			if (c == null)
//				return LBRBBaseTypes.Airbourne;
//
//			r = new Ray (c.bounds.center, Vector3.down);
//
//			Debug.DrawRay (r.origin, r.direction, Color.green);
//
//			if (Physics.Raycast (r.origin, r.direction, out hit, c.bounds.extents.y+0.05f)) 
//			{
//				//Debug.Log (hit.transform.gameObject.name);
//				if (hit.transform.gameObject.name != parent.name)
//					return LBRBBaseTypes.Grounded;
//			}
//
//			return LBRBBaseTypes.Airbourne;
//		}


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