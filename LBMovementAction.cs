using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{

	public enum LBRBBaseTypes
	{
		Grounded,
		Airbourne
	}

	/// <summary>
	/// This enum briefly describes velocity vector direction in local system
	/// </summary>
	public enum LBVectorDirectionTypes
	{
		Zero,
		ZAxis,
		XAxis,
		YAxis
	}

	public struct LBMovementTransferCondition
	{
		public bool use_VelocityDirectionFilter;
		public LBVectorDirectionTypes VelocityDirectionFilter;
		public bool use_RBVelocityLimit;
		public float RBVelocityLimit;
		public bool use_RBBasement;
		public LBRBBaseTypes RBBasement;
	}

	public class LBMovementAction : LBTransitiveAction
	{
		protected Rigidbody rigidbody;

		public Vector3 MovementDir;
		public float MovementSpeed;

		public LBMovementTransferCondition[] InputConditions;
		public LBMovementTransferCondition[] OutputConditions;

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;

			rigidbody = parent.GetComponent<Rigidbody> ();

			if (rigidbody == null)
				return false;

			return true;
		}
			
		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			int id;

			if (_dir == LBActionTransitDirection.In)
			{
				id = FindInputIndex (_other);

				if (id < 0)
					return false;

				if (id > InputConditions.Length) // if we don't have condition set for this input
					return true;

				if (CheckMovementTransferConditions(InputConditions[id]))
					return true;
				else
					return false;
			}
			else
			{
			}
			return true; //no conditions in this class
		}

		protected bool CheckMovementTransferConditions(LBMovementTransferCondition _cond)
		{
			if (rigidbody.velocity.magnitude > _cond.RBVelocityLimit)
				return false;

			// trace down to basement

			if (GetParentBasementType () != _cond.RBBasement)
				return false;

			return true;
		}
			
		LBRBBaseTypes GetParentBasementType()
		{
			Collider c;
			Ray r;
			RaycastHit hit;

			c = parent.GetComponent<Collider>();

			if (c == null)
				return LBRBBaseTypes.Airbourne;

			r = new Ray (c.bounds.center, Vector3.down);

			Debug.DrawRay (r.origin, r.direction, Color.green);

			if (Physics.Raycast (r.origin, r.direction, out hit, c.bounds.extents.y+0.05f)) 
			{
				//Debug.Log (hit.transform.gameObject.name);
				if (hit.transform.gameObject.name != parent.name)
					return LBRBBaseTypes.Grounded;
			}

			return LBRBBaseTypes.Airbourne;
		}
			
		protected virtual void PerformMovement ()
		{
			rigidbody.velocity = MovementDir.normalized * MovementSpeed;
			rigidbody.rotation = Quaternion.LookRotation (MovementDir);
		}

		public override void Tick ()
		{
			base.Tick ();

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

	}
}
