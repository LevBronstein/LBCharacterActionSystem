using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterStandAction", menuName = "LBActionSystem/CharacterStandAction")]
	public class LBCharacterStandAction : LBCharacterMovementAction 
	{
		protected override void PerformMovement () {}

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
			if (Mathf.Approximately (rigidbody.velocity.magnitude, 0))
				return true;
			else
				return false;
		}
	}
}
