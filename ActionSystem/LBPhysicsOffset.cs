using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewPhysicsOffset", menuName = "LBActionSystem/PhysicsOffset")]
	public class LBPhysicsOffset : LBPhysicsTransform
	{
		public string OverridenParent = "";

		public bool bApplyPosition;
		public Vector3 Position;

		public bool bApplyRotation;
		public Vector3 Rotation;

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			if (!base.Init (parentgameobject, manager))
				return false;

			if (OverridenParent == string.Empty)
			{
				rigidbody = parent.GetComponent<Rigidbody> ();
			}
			else
			{
				GameObject p = FindChild (parent, OverridenParent);

				if (p!=null)
					rigidbody = p.GetComponent<Rigidbody> ();
			}

			if (rigidbody == null)
				return false;

			return true;
		}

		protected override void TickActive ()
		{
			base.TickActive ();
			if (bApplyPosition)
				PerformMovement ();
			if (bApplyRotation)
				PerformRotation ();
		}

		protected virtual void PerformMovement ()
		{
			if (rigidbody != null)
			{
				switch (Coordinates) 
				{
					case LBCoordinateSystem.Local:
						rigidbody.MovePosition (rigidbody.transform.TransformPoint (Position));
						break;
					case LBCoordinateSystem.Parent:
						rigidbody.MovePosition (rigidbody.transform.parent.TransformPoint (Position));
						break;
					case LBCoordinateSystem.World:
					default:
						rigidbody.MovePosition (Position);
						break;
				}	
			}
		}

		protected virtual void PerformRotation ()
		{
			if (rigidbody != null)
			{
				switch (Coordinates) 
				{
					case LBCoordinateSystem.Local:
						rigidbody.transform.gameObject.transform.rotation = (Quaternion.LookRotation(rigidbody.transform.TransformDirection(Rotation)));
						break;
					case LBCoordinateSystem.Parent:
						rigidbody.transform.gameObject.transform.rotation = Quaternion.LookRotation(rigidbody.transform.parent.TransformDirection(Rotation));
						break;
					case LBCoordinateSystem.World:
					default:
						rigidbody.transform.gameObject.transform.rotation =  Quaternion.LookRotation(Rotation);
						break;
				}	
			}
		}

		protected GameObject FindChild(GameObject p, string name)
		{
			int i;
			GameObject c;

			if (p.name != name)
			{
				for (i = 0; i < p.transform.childCount; i++)
				{
					c = FindChild (p.transform.GetChild (i).gameObject, name);

					if (c != null)
						return c;
				}
			}
			else
				return p;

			return null;
		}

		public override LBActionTickTypes TickType 
		{
			get 
			{
				return LBActionTickTypes.LateTick;
			}
		}

		public override LBAction Duplicate ()
		{
			LBPhysicsOffset dup;

			dup = (LBPhysicsOffset)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBPhysicsOffset)dup).OverridenParent = OverridenParent;
			((LBPhysicsOffset)dup).bApplyPosition = bApplyPosition;
			((LBPhysicsOffset)dup).Position = Position;
			((LBPhysicsOffset)dup).bApplyRotation = bApplyRotation;
			((LBPhysicsOffset)dup).Rotation = Rotation;
		} 
	}
}
