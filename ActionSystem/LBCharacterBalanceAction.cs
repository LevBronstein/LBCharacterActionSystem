using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterBalanceAction", menuName = "LBActionSystem/CharacterBalanceAction")]
	public class LBCharacterBalanceAction : LBCharacterStandAction
	{
		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return bHasRBBase ();
			}
			else
			{
				return true;
			}
		}

		protected bool bHasRBBase()
		{
			Collider c;
			RaycastHit hit;

			c = parent.GetComponent<Collider>();

			if (c == null)
				return false;

			if (Physics.BoxCast (c.bounds.center, new Vector3(c.bounds.extents.x, 0.15f, c.bounds.extents.z), -c.transform.up, out hit, Quaternion.LookRotation(-c.transform.up), c.bounds.extents.y))
			{
				if (hit.transform.gameObject.name != parent.name)				
				{
					if (hit.transform.gameObject.GetComponent<Rigidbody> () != null)
						return true;
				}
			}

			return false;
		}
	}
}
