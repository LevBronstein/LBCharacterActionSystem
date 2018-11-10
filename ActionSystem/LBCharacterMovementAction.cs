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
	public class LBCharacterMovementAction : LBCharacterPhysicsAction 
	{
//		public LBMovementTransferCondition[] InputConditions;
//		public LBMovementTransferCondition[] OutputConditions;

//		protected Animator animator = null;
//
//		public string AnimationName = string.Empty;
//		public int AnimationLayer = 0;
//		public float AnimationBlendTime = 0.1f;
//		public LBActionAnimationTypes AnimationType = LBActionAnimationTypes.Playback;

		public Vector3 MovementDir;
		public float MovementSpeed;

		protected Vector3 orig_velocity;

		protected virtual void PerformMovement ()
		{
			rigidbody.velocity = MovementDir.normalized * MovementSpeed;
			if (MovementDir != Vector3.zero)
				rigidbody.rotation = Quaternion.LookRotation (MovementDir);
		}

		protected override void TickActive ()
		{
			// we can move only when our action is active
			base.TickActive ();
			PerformMovement ();
		}

		public virtual void SetMovementSpeed(float _speed)
		{
			MovementSpeed = _speed;
		}

		public virtual void SetMovementDir(Vector3 _dir)
		{
			MovementDir = _dir;
		}
			
		protected bool bHasWalkableFloor()
		{
			Collider c;
			RaycastHit hit;

			c = parent.GetComponent<Collider>();

			if (c == null)
				return false;

			if (Physics.BoxCast (c.bounds.center, new Vector3(c.bounds.extents.x, 0.15f, c.bounds.extents.z), -c.transform.up, out hit, Quaternion.LookRotation(-c.transform.up), c.bounds.extents.y))
			{
				if (hit.transform.gameObject.name != parent.name)
					return true;
			}

			return false;
		}

		public override LBAnimationTransitionTypes AnimationTrasnitionType
		{
			get
			{
				return LBAnimationTransitionTypes.Crossfade;
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

			((LBCharacterMovementAction)dup).MovementDir = MovementDir;
			((LBCharacterMovementAction)dup).MovementSpeed = MovementSpeed;
		}

//		protected override void DuplicateProperties(LBAction dup)
//		{
//			base.DuplicateProperties (dup);
//
//			((LBCharacterMovementAction)dup).AnimationName = AnimationName;
//			((LBCharacterMovementAction)dup).AnimationLayer = AnimationLayer;
//			((LBCharacterMovementAction)dup).AnimationBlendTime = AnimationBlendTime;
//			((LBCharacterMovementAction)dup).AnimationType = AnimationType;
//		}
	}
}