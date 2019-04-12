using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBPhysics;

namespace LBActionSystem
{
	public enum LBBaseTypes
	{
		None,
		Static,
		RigidBody
	}

	[CreateAssetMenu (fileName = "NewCharacterStandExAction", menuName = "LBActionSystem/CharacterStandExAction")]
	public class LBCharacterStandExAction : LBCharacterStandAction
	{
		public bool bCheckBaseType; // Set to true to check base object's type
		public LBBaseTypes BaseType;
		public bool bCheckNormal; // Set to true to check base object's normal difference with character's up direction
		public float NormalDelta;

		protected Quaternion _oldrot;

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

			_oldrot = rigidbody.rotation;
		}

		protected override void PerformMovement ()
		{
			RaycastHit h;
			Vector3 r;

			h = GetBase ();
			//rigidbody.transform.up 
			r = h.rigidbody.rotation.eulerAngles + _oldrot.eulerAngles;
			r.x = 0;
			//r.y = r.y * Mathf.Floor(rigidbody.transform.up.normalized.y);
			r.z = 0;
			rigidbody.rotation = Quaternion.Euler(r);
		}

		protected override bool bHasWalkableFloor()
		{
			RaycastHit h;

			h = GetBase ();

			if (bCheckBaseType)
			{
				if (BaseType != GetBaseType (h))
					return false;
			}

			if (bCheckNormal)
			{
				if (Vector3.Angle (h.normal, rigidbody.transform.up) <= NormalDelta)
					return false;
			}

			return true;
		}

//		protected bool bHasRBBase()
//		{
//			Collider c;
//			RaycastHit hit;
//
//			c = parent.GetComponent<Collider>();
//
//			if (c == null)
//				return false;
//
//			if (Physics.BoxCast (c.bounds.center, new Vector3(c.bounds.extents.x, 0.15f, c.bounds.extents.z), -c.transform.up, out hit, Quaternion.LookRotation(-c.transform.up), c.bounds.extents.y))
//			{
//				if (hit.transform.gameObject.name != parent.name)				
//				{
//					if (hit.transform.gameObject.GetComponent<Rigidbody> () != null)
//						return true;
//				}
//			}
//
//			return false;
//		}

		protected RaycastHit GetBase()
		{
			Collider c;
			RaycastHit[] hit;
			RaycastHit h;
			int i;
			bool foundhit;

			c = parent.GetComponent<Collider>();

			if (c == null)
				return new RaycastHit();

			hit = Physics.BoxCastAll (c.bounds.center, new Vector3 (c.bounds.extents.x, 0.15f, c.bounds.extents.z), -rigidbody.transform.up, Quaternion.LookRotation (-rigidbody.transform.up), c.bounds.extents.y);

			if (hit == null || hit.Length == 0)
				return new RaycastHit();

			h = hit [0];
			foundhit = false;

			for (i = 0; i < hit.Length; i++)
			{
				if (hit [i].transform.gameObject != parent && !(hit [i].transform.IsChildOf (parent.transform)))
				{
					h = hit [i];
					foundhit = true;
					break;
				}
			}

			// haven't gotten any appropriate hit
			if (!foundhit)
				return new RaycastHit();

			// find closest hit
			for (i = 0; i < hit.Length; i++)
			{
				if (hit [i].transform.gameObject != parent && !hit [i].transform.IsChildOf (parent.transform) && hit [i].distance < h.distance)
				{
					h = hit [i];
				}
			}

//			if (Physics.BoxCast (c.bounds.center, new Vector3(c.bounds.extents.x, 0.15f, c.bounds.extents.z), -c.transform.up, out hit, Quaternion.LookRotation(-c.transform.up), c.bounds.extents.y))
//			{
//				if (hit.transform.gameObject.name != parent.name && !hit.transform.IsChildOf(parent.transform))				
//				{
//					return hit.transform.gameObject;
//				}
//			}

			return h;
		}

		protected virtual LBBaseTypes GetBaseType(RaycastHit hit)
		{
			if (hit.transform.gameObject == null)
				return LBBaseTypes.None;

			if (hit.transform.gameObject.GetComponent<Rigidbody> () != null)
				return LBBaseTypes.RigidBody;
			else 
				return LBBaseTypes.Static;
		}

		// We need just to ensure, that our RB is not moving
		protected override bool bHasPropperSpeed()
		{
			GameObject baseobj;
			Rigidbody baserb;

			baseobj = GetBase().transform.gameObject;
			baserb = baseobj.GetComponent<Rigidbody> ();

			if (BaseType != LBBaseTypes.None && baseobj == null)
				return false;

			// if we need only RB as a base
			if (BaseType == LBBaseTypes.RigidBody && baserb == null)
				return false;

			// if we have a static object as a base and we mean it -- we use an old scheme
			if (BaseType == LBBaseTypes.Static && baserb == null)
			{
				return base.bHasPropperSpeed();
			}

			if (baserb.velocity == rigidbody.velocity)
				return true;
				
			return false;
		}

		public override LBAction Duplicate ()
		{
			LBCharacterStandExAction dup;

			dup = (LBCharacterStandExAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterStandExAction)dup).bCheckBaseType = bCheckBaseType;
			((LBCharacterStandExAction)dup).BaseType = BaseType;
			((LBCharacterStandExAction)dup).bCheckNormal = bCheckNormal;
			((LBCharacterStandExAction)dup).NormalDelta = NormalDelta;
		}
	}
}