using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	public enum LBCoordinateSystem
	{
		World,
		Parent,
		Local
	}

	public enum LBTransformTool
	{
		Transform,
		RigidBody,
		Velocity
	}

	public class LBPhysicsTransform : LBTransitiveAction
	{
		public LBActionStates InitialState = LBActionStates.Disabled;

		public LBCoordinateSystem Coordinates = LBCoordinateSystem.Local;

		public LBActionTickTypes TickOrder; 

		public LBTransformTool TransformTool;
		//protected GameObject transformbase;
		protected Rigidbody rigidbody;
		protected GameObject gameobject;

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;

			rigidbody = parent.GetComponent<Rigidbody> ();
			gameobject = parent;

			if (TransformTool == LBTransformTool.RigidBody && rigidbody == null)
				return false;

			if (TransformTool == LBTransformTool.Transform && gameobject == null)
				return false;

			if (InitialState == LBActionStates.Active)
				ActivateAction ();

			return true;
		}

//		protected Transform GetTransformBase()
//		{
//			switch (Coordinates)
//			{
//			case LBCoordinateSystem.Local:
//				transformbase = rigidbody.gameObject.transform;
//				break;
//			case LBCoordinateSystem.Parent: 
//				transformbase = rigidbody.gameObject.transform.parent;
//				break;
//			case LBCoordinateSystem.World:
//				transformbase = rigidbody.gameObject.transform.root;
//				default:
//					break;
//			}
//		}

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

		public override LBActionTickTypes TickType 
		{
			get 
			{
				return TickOrder;
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

		public float RBFlatSpeed
		{
			get 
			{
				return (new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z)).magnitude;	
			}

			protected set
			{
				Vector3 v; 
				v = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
				rigidbody.velocity = v.normalized * value;
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

		public Vector3 RBFlatSpeedDir
		{
			get 
			{
				return (new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z)).normalized;	
			}

			protected set
			{
				rigidbody.velocity = (new Vector3(value.x, 0 , value.z)).normalized * RBFlatSpeed + new Vector3(0, rigidbody.velocity.y,0);
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

			set 
			{
				rigidbody.rotation = Quaternion.LookRotation (value);
			}
		}

		public override LBAction Duplicate ()
		{
			LBPhysicsTransform dup;

			dup = (LBPhysicsTransform)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBPhysicsTransform)dup).InitialState = InitialState;
			((LBPhysicsTransform)dup).Coordinates = Coordinates;
			((LBPhysicsTransform)dup).TickOrder = TickOrder;
			((LBPhysicsTransform)dup).TransformTool = TransformTool;
		} 
	}
}