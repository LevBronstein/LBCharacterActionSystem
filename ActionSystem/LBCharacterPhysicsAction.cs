using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	//[CreateAssetMenu (fileName = "NewMovementAction", menuName = "LBActionSystem/MovementAction")]
	public class LBCharacterPhysicsAction : LBCharacterAnimatedAction
	{
		protected Rigidbody rigidbody;

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;

			rigidbody = parent.GetComponent<Rigidbody> ();

			if (rigidbody == null)
				return false;

			return true;
		}

		protected virtual void TrySelfActivate()
		{
			if (CheckSelfActivationCondtions ())
			{
				ActivateAction ();
			}
		}

		protected virtual void TrySelfDeactivate()
		{
			if (CheckSelfDeactivationCondtions ())
			{
				DeactivateAction ();
			}
		}

		protected virtual bool CheckSelfActivationCondtions()
		{
			return false;
		}

		protected virtual bool CheckSelfDeactivationCondtions()
		{
			return false;
		}

		protected override void TickActive ()
		{
			base.TickActive ();

			TrySelfDeactivate();
		}

		protected override void TickInactive ()
		{
			base.TickInactive ();

			TrySelfActivate();
		}

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

			protected set
			{
				rigidbody.velocity = rigidbody.velocity.normalized * value;
			}
		}

		public Vector3 RBSpeedDir
		{
			get 
			{
				return rigidbody.velocity.normalized;	
			}

			protected set
			{
				rigidbody.velocity = value.normalized * rigidbody.velocity.magnitude;
			}
		}

		public Vector3 RBSpeedVector
		{
			get 
			{
				return rigidbody.velocity;	
			}

			protected set
			{
				rigidbody.velocity = value;
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
			LBCharacterPhysicsAction dup;

			dup = (LBCharacterPhysicsAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

//			((LBTransPhysicsAction)dup).MovementDir = MovementDir;
//			((LBTransPhysicsAction)dup).MovementSpeed = MovementSpeed;
		}

	}
}
