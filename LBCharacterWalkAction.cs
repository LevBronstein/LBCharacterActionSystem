using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	/// <summary>
	/// Handles walk action of an arbitary character, which is performed in a forward direction on a flat surface.
	/// </summary>
	[CreateAssetMenu (fileName = "NewCharacterWalkAction", menuName = "LBActionSystem/CharacterWalkAction")]
	public class LBCharacterWalkAction : LBCharacterMovementAction 
	{
		public float MinSpeedLimit = 0.0f; // Min amount of forward speed
		public float MaxSpeedLimit = 3.0f;

		protected override void PerformMovement ()
		{
			rigidbody.velocity = MovementDir.normalized * Mathf.Clamp (MovementSpeed, MinSpeedLimit, MaxSpeedLimit);
			rigidbody.rotation = Quaternion.LookRotation (MovementDir);
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return bHasWalkableFloor () && bHasPropperSpeed();
			}
			else
			{
				return true;
			}
		}

		protected override void CheckTransferOut()
		{
			if (!bHasWalkableFloor () || !bHasPropperSpeed())
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
	
		protected bool bHasPropperSpeed()
		{
			if (Vector3.Angle (rigidbody.transform.forward, rigidbody.velocity) < 5.0f)
			{
				if (rigidbody.velocity.magnitude >= MinSpeedLimit && rigidbody.velocity.magnitude <= MaxSpeedLimit)
					return true;
				return false;
			}

			return false;
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

			((LBCharacterWalkAction)dup).MinSpeedLimit = MinSpeedLimit;
			((LBCharacterWalkAction)dup).MaxSpeedLimit = MaxSpeedLimit;
		}
	
	}
}
