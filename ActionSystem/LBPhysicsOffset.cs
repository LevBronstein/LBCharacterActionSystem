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

				if (p != null)
				{
					if (TransformTool == LBTransformTool.RigidBody)
						rigidbody = p.GetComponent<Rigidbody> ();
					else
						gameobject = p;
				}
			}

			if (TransformTool == LBTransformTool.RigidBody && rigidbody == null)
				return false;

			if (TransformTool == LBTransformTool.Transform && gameobject == null)
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
			if (TransformTool == LBTransformTool.Transform)
			{
				MoveByTransform ();
			}
		}

		protected virtual void MoveByTransform()
		{			
			switch (Coordinates)
			{
				case LBCoordinateSystem.Local:
					gameobject.transform.position = gameobject.transform.TransformPoint (Position);
					break;
				case LBCoordinateSystem.Parent:
					gameobject.transform.position = gameobject.transform.parent.TransformPoint (Position);
					break;
				case LBCoordinateSystem.World:
				default:
					gameobject.transform.position = Position;
					break;
			}	
		}
			
		protected virtual void MoveByRigidBody()
		{	
			switch (Coordinates)
			{
				case LBCoordinateSystem.Local:
					rigidbody.MovePosition (gameobject.transform.TransformPoint (Position));
					break;
				case LBCoordinateSystem.Parent:
					rigidbody.MovePosition (gameobject.transform.parent.TransformPoint (Position));
					break;
				case LBCoordinateSystem.World:
				default:
					rigidbody.MovePosition (Position);
					break;
			}	
		}

		protected virtual void PerformRotation ()
		{
			if (TransformTool == LBTransformTool.Transform)
			{
				RotateByTransform ();
			}
		}

		protected virtual void RotateByTransform()
		{
			switch (Coordinates) 
			{
				case LBCoordinateSystem.Local:
					gameobject.transform.rotation = (Quaternion.LookRotation(gameobject.transform.TransformDirection(Rotation)));
					break;
				case LBCoordinateSystem.Parent:
					gameobject.transform.localRotation = Quaternion.Euler(Rotation);
					break;
				case LBCoordinateSystem.World:
				default:
					gameobject.transform.rotation = Quaternion.Euler(Rotation);
					break;
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

		public Vector3 OffsetPosition
		{
			get
			{
				return Position;
			}
			set
			{
				Position = value;
			}
		}

		public Vector3 OffsetRotation
		{
			get
			{
				return Rotation;
			}
			set
			{
				Rotation = value;
			}
		}

		public bool TogglePosition
		{
			get
			{
				return bApplyPosition;
			}
			set
			{
				bApplyPosition = value;
			}
		}

		public bool ToggleRotation
		{
			get 
			{
				return bApplyRotation;
			}
			set 
			{
				bApplyRotation = value;
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
