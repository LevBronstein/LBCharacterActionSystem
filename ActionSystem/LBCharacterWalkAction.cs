using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[System.Serializable]
	public struct LBSpeedRestraint
	{
		public float MinSpeed;
		public bool bStrictMin;
		public float MaxSpeed;
		public bool bStricMax;
		public bool Inside;
	}

	/// <summary>
	/// Handles walk action of an arbitary character, which is performed in a forward direction on a flat surface.
	/// </summary>
	[CreateAssetMenu (fileName = "NewCharacterWalkAction", menuName = "LBActionSystem/CharacterWalkAction")]
	public class LBCharacterWalkAction : LBCharacterMovementAction 
	{
		public LBSpeedRestraint SpeedRestraintIn;
		public LBSpeedRestraint SpeedRestraintOut;

//		public float MinSpeedLimit = 0.0f; // Min amount of forward speed
//		public float MaxSpeedLimit = 3.0f;

		protected override void PerformMovement ()
		{
			rigidbody.velocity = MovementDir.normalized * Mathf.Clamp (MovementSpeed, SpeedRestraintIn.MinSpeed, SpeedRestraintIn.MaxSpeed);
			rigidbody.rotation = Quaternion.LookRotation (MovementDir);
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return bHasWalkableFloor () && bHasPropperTransferInSpeed();
			}
			else
			{
				return true;
			}
		}

		protected override void CheckTransferOut()
		{
			if (!bHasWalkableFloor () || bHasPropperTransferOutSpeed())
			{
				DeactivateAction ();
			}
		}

		protected bool bHasWalkableFloor()
		{
			Collider c;
			Ray r;
			RaycastHit hit;

			c = parent.GetComponent<Collider>();

			if (c == null)
				return false;

			r = new Ray (c.bounds.center, Vector3.down);

			Debug.DrawRay (r.origin, r.direction, Color.green);

			if (Physics.Raycast (r.origin, r.direction, out hit, c.bounds.extents.y+0.05f)) 
			{
				//Debug.Log (hit.transform.gameObject.name);
				if (hit.transform.gameObject.name != parent.name)
					return true;
			}

			return false;
		}
	
		protected bool bHasPropperTransferInSpeed()
		{
			if (Vector3.Angle (rigidbody.transform.forward, rigidbody.velocity) < 5.0f)
			{
				if (CheckSpeedRestraint(rigidbody.velocity.magnitude, SpeedRestraintIn))
					return true;
			}

			return false;
		}

		protected bool bHasPropperTransferOutSpeed()
		{
			if (Vector3.Angle (rigidbody.transform.forward, rigidbody.velocity) > 5.0f)
				return true;
			
			if (CheckSpeedRestraint(rigidbody.velocity.magnitude, SpeedRestraintOut))
				return true;

			return false;
		}

//		protected bool bHasPropperOutTransferSpeed()
//		{
//			if (Vector3.Angle (rigidbody.transform.forward, rigidbody.velocity) > 5.0f)
//				return true;
//			
//			if (CheckSpeedRestraint(rigidbody.velocity.magnitude, InTransferSpeed))
//				return true;
//
//			return false;
//		}

		protected bool CheckSpeedRestraintInside(float spd, LBSpeedRestraint rest)
		{
			float min, max;

			min = Mathf.Min (rest.MinSpeed, rest.MaxSpeed);
			max = Mathf.Max (rest.MinSpeed, rest.MaxSpeed);

			if ((spd <= min && rest.bStrictMin) || (spd < min && !rest.bStrictMin))
				return false;

			if ((spd >= max && rest.bStricMax) || (spd > max && !rest.bStricMax))
				return false;

			return true;
		}

		protected bool CheckSpeedRestraintOutside(float spd, LBSpeedRestraint rest)
		{
			float min, max;

			min = Mathf.Min (rest.MinSpeed, rest.MaxSpeed);
			max = Mathf.Max (rest.MinSpeed, rest.MaxSpeed);

			if ((spd >= min && rest.bStrictMin) || (spd > min && !rest.bStrictMin))
				return false;

			if ((spd <= max && rest.bStricMax) || (spd < max && !rest.bStricMax))
				return false;

			return true;
		}

		public override LBAction Duplicate ()
		{
			LBCharacterMovementAction dup;

			dup = (LBCharacterWalkAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterWalkAction)dup).SpeedRestraintIn = SpeedRestraintIn;
			((LBCharacterWalkAction)dup).SpeedRestraintOut = SpeedRestraintOut;
		}
	
	}
}
