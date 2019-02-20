using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewPhysicsMovement", menuName = "LBActionSystem/PhysicsMovement")]
	public class LBPhysicsMovement : LBPhysicsTransform
	{
		public GameObject OverridenParent;

		public Vector3 MovementSpeed;
		public Vector3 RotationSpeed;

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;

			if (OverridenParent == null)
				rigidbody = parent.GetComponent<Rigidbody> ();
			else
				rigidbody = OverridenParent.GetComponent<Rigidbody> ();

			if (rigidbody == null)
				return false;

			return true;
		}

		protected override void TickActive ()
		{
			base.TickActive ();
			PerformMovement ();
			PerformRotation ();
		}

		protected virtual void PerformMovement ()
		{
		}

		protected virtual void PerformRotation ()
		{
		}
	}
}
