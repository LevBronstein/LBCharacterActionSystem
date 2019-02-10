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
	/// 
	public enum LBAxisTypes
	{
		XAxis,
		YAxis,
		ZAxis
	}


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

		//Animation paramteres' names in the animator
		public string CharVelocityParamName = "CharVelocityNorm";
		public string CharDeltaRotXParamName = "CharDeltaRotXNorm";
		public string CharDeltaRotYParamName = "CharDeltaRotYNorm";
		public string CharDeltaRotZParamName = "CharDeltaRotYNorm";

		protected Vector3 orig_velocity;

		protected Vector3 LastVel;

		protected virtual void PerformMovement ()
		{
			rigidbody.velocity = MovementDir.normalized * MovementSpeed;
			if (MovementDir != Vector3.zero)
				rigidbody.rotation = Quaternion.LookRotation (MovementDir);
		}

		protected virtual void UpdateSliders()
		{
			SetVelocityParam (RBSpeedVector.magnitude);
			SetDeltaRotParam (Vector3.SignedAngle (LastVel, RBSpeedVector, Vector3.right), LBAxisTypes.XAxis);
			SetDeltaRotParam (Vector3.SignedAngle (LastVel, RBSpeedVector, Vector3.up), LBAxisTypes.YAxis);
			SetDeltaRotParam (Vector3.SignedAngle (LastVel, RBSpeedVector, Vector3.forward), LBAxisTypes.ZAxis);
		}

		protected override void TickActive ()
		{
			// we can move only when our action is active
			base.TickActive ();
			PerformMovement ();
			UpdateSliders ();
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

		protected void SetVelocityParam(float value)
		{
			if (bParamExists(CharVelocityParamName))
				animator.SetFloat (CharVelocityParamName, value);
		}

		protected float GetVelocityParam()
		{
			if (bParamExists (CharVelocityParamName))
				return animator.GetFloat (CharVelocityParamName);
			else
				return 0;
		}
			
		protected void SetDeltaRotParam(float value, LBAxisTypes axis = LBAxisTypes.YAxis)
		{
			switch (axis)
			{
				case LBAxisTypes.XAxis:
					if (bParamExists (CharDeltaRotXParamName))
						animator.SetFloat (CharDeltaRotXParamName, value);
					break;
				case LBAxisTypes.YAxis:
					if (bParamExists (CharDeltaRotYParamName))
						animator.SetFloat (CharDeltaRotYParamName, value);
					break;
				case LBAxisTypes.ZAxis:
					if (bParamExists (CharDeltaRotZParamName))
						animator.SetFloat (CharDeltaRotZParamName, value);
					break;
				default:
					break;
			}
		}

		protected float GetDeltaRotParam(LBAxisTypes axis = LBAxisTypes.YAxis)
		{
			switch (axis)
			{
			case LBAxisTypes.XAxis:
				if (bParamExists (CharDeltaRotXParamName))
					return animator.GetFloat (CharDeltaRotXParamName);
				break;
			case LBAxisTypes.YAxis:
				if (bParamExists (CharDeltaRotYParamName))
					return animator.GetFloat (CharDeltaRotYParamName);
				break;
			case LBAxisTypes.ZAxis:
				if (bParamExists (CharDeltaRotZParamName))
					return animator.GetFloat (CharDeltaRotZParamName);
				break;
			default:
				break;
			}

			return 0;
		}

		bool bParamExists(string param)
		{
			int i;

			if (animator != null)
			{
				for (i = 0; i < animator.parameters.Length; i++)
				{
					if (animator.parameters [i].name == param)
						return true;
				}
			}

			return false;
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
			((LBCharacterMovementAction)dup).CharVelocityParamName = CharVelocityParamName;
			((LBCharacterMovementAction)dup).CharDeltaRotXParamName = CharDeltaRotXParamName;
			((LBCharacterMovementAction)dup).CharDeltaRotYParamName = CharDeltaRotYParamName;
			((LBCharacterMovementAction)dup).CharDeltaRotZParamName = CharDeltaRotZParamName;
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