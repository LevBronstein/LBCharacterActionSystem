using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewMovementAction", menuName = "LBActionSystem/MovementAction")]
	public class LBMovementAction : LBTransitiveAction
	{
		protected Rigidbody rigidbody;

		public Vector3 MovementDir;
		public float MovementSpeed;

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;

			rigidbody = parent.GetComponent<Rigidbody> ();

			if (rigidbody == null)
				return false;

			return true;
		}
			
		protected virtual void PerformMovement ()
		{
			rigidbody.velocity = MovementDir.normalized * MovementSpeed;
			rigidbody.rotation = Quaternion.LookRotation (MovementDir);
		}

//		public override void Tick ()
//		{
//			base.Tick ();
//
//			PerformMovement ();
//		}

		protected override void TickActive ()
		{
			// we can move only when our action is active
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

//		protected LBVectorDirectionTypes GetParentLocomotionType()
//		{
//			float mov_hor, mov_vert;
//
//			mov_hor = Mathf.Sqrt(rigidbody.velocity.x * rigidbody.velocity.x + rigidbody.velocity.y * rigidbody.velocity.y);
//			mov_vert = Mathf.Abs(rigidbody.velocity.z);
//
//			if (Mathf.Abs(mov_hor) > 0 && Mathf.Approximately(mov_vert, 0.0f)) 
//			{
//				return MM_LocomotionTypes.Moving_Horizontal;
//			}
//			else if (Mathf.Approximately(mov_hor, 0.0f) && Mathf.Abs(mov_vert) > 0)
//			{
//				return MM_LocomotionTypes.Moving_Vertical;
//			}
//			else if (Mathf.Approximately(mov_hor, 0.0f) && Mathf.Approximately(mov_vert, 0.0f))
//			{
//				return MM_LocomotionTypes.Static;
//			}
//			else
//				return MM_LocomotionTypes.Moving_AnyDirection;
//		}

		public override LBActionTickTypes ActionTick 
		{
			get 
			{
				return LBActionTickTypes.PhysicsTick;
			}
		}

		public override LBAction Duplicate ()
		{
			LBMovementAction dup;

			dup = (LBMovementAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBMovementAction)dup).MovementDir = MovementDir;
			((LBMovementAction)dup).MovementSpeed = MovementSpeed;
		}

	}
}
