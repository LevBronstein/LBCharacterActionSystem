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

		protected virtual void TrySelfActivate()
		{
		}

		protected virtual void TrySelfDeactivate()
		{
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
			TrySelfDeactivate();
		}

		protected override void TickInactive ()
		{
			TrySelfActivate();
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

		protected float TruncFloat(float f, byte numbers=3)
		{
			return (float)System.Math.Truncate (f * (int)System.Math.Pow(10,numbers)) / (int)System.Math.Pow(10,numbers);
		}

		protected float LerpFloat(float current, float target, float step, float dt)
		{
			float value;

			if (Mathf.Abs(current - target) > Mathf.Abs(step))
			{
				if (current < target)
					value = current + Mathf.Abs (step);
				else
					value = current - Mathf.Abs (step);  
			}
			else
			{
				if (current < target)
					value = current + Mathf.Abs (current - target);
				else
					value = current - Mathf.Abs (current - target);     
			}

			return value;
		}

		protected float LerpAngle(float current, float target, float step, float dt)
		{
			float value;

			if (current < target)
			{
				if (Mathf.Abs(target-current) < 180)
				{
					if (Mathf.Abs (current - target) > Mathf.Abs (step))
						value = current + step;
					else
						value = current + Mathf.Abs (current - target);
				}
				else
				{
					if (Mathf.Abs (current - target) > Mathf.Abs (step))
						value = current - step;
					else
						value = current - Mathf.Abs (current - target);

					if (value < 0)
						value = value + 360;
				}    
			}
			else
			{
				if (Mathf.Abs (target-current) < 180)
				{
					if (Mathf.Abs (current - target) > Mathf.Abs (step))
						value = current - step;
					else
						value = current - Mathf.Abs (current - target);
				}
				else
				{
					if (Mathf.Abs (current - target) > Mathf.Abs (step))
						value = current + step;
					else
						value = current + Mathf.Abs (current - target);

					if (value > 360)
						value = value - 360;
				}        
			}

			return value;
		}
			
//		protected float SingedAngle(Vector3 v1, Vector3 v2, Vector3 n)
//		{
//			return Mathf.Atan2 (Vector3.Dot (n, Vector3.Cross (v1, v2)), Vector3.Dot (v1, v2) * Mathf.Rad2Deg);
//		}

		public float RBSpeed
		{
			get 
			{
				return rigidbody.velocity.magnitude;	
			}
		}

		public Vector3 RBSpeedDir
		{
			get 
			{
				return rigidbody.velocity.normalized;	
			}
		}

		public Vector3 RBForwardDir
		{
			get 
			{
				return rigidbody.transform.forward;	
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
