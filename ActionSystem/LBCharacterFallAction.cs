using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterFallAction", menuName = "LBActionSystem/CharacterFallAction")]
	public class LBCharacterFallAction : LBCharacterMovementAction 
	{

		protected override void PerformMovement ()
		{
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return bHasNoFloor ();
			}
			else
			{
				return !bHasNoFloor ();
			}
		}

		protected override void CheckTransferIn()
		{
			if (bHasNoFloor ())
			{
				ActivateAction ();
			}
		}

		protected override void CheckTransferOut()
		{
			if (!bHasNoFloor ())
			{
				DeactivateAction ();
			}
		}

		protected bool bHasNoFloor()
		{
			Collider c;
			Ray r;
			RaycastHit hit;

			c = parent.GetComponent<Collider>();

			if (c == null)
				return true;

			r = new Ray (c.bounds.center, Vector3.down);

			Debug.DrawRay (r.origin, r.direction, Color.green);

			if (Physics.Raycast (r.origin, r.direction, out hit, c.bounds.extents.y+0.05f)) 
			{
				//Debug.Log (hit.transform.gameObject.name);
				if (hit.transform.gameObject.name != parent.name)
					return false;
			}

			return true;
		}

	}
}
