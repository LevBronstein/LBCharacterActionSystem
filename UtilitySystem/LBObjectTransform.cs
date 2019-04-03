using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBUtils
{
	public enum LBCoordinateSystem
	{
		World,
		Parent,
		Local
	}

	public enum LBTransformTool
	{
		Transform,
		RigidBody,
		Velocity,
		RBVelocity,
		RBForce
	}

	public enum LBAppendType
	{
		Overwrite,
		AppendToExisting,
		AppendToLast
	}

	public class LBObjectTransform : MonoBehaviour
	{
		protected Rigidbody _rigidbody;
		protected GameObject _gameobject;

		protected Vector3 _oldvel;
		protected Vector3 _oldrot;

		public LBTransformTool PositionTool;
		public LBTransformTool RotationTool;

		public LBAppendType PositionAppend;
		public LBAppendType RotationAppend;

		public ForceMode PositionForceMode;
		public ForceMode RotationForceMode;

		public LBCoordinateSystem Coordinates = LBCoordinateSystem.Local;

		public string OverridenParent = "";

		public bool bApplyPosition;
		public Vector3 Position;

		public bool bApplyRotation;
		public Vector3 Rotation;

		void Start()
		{
			if (OverridenParent == string.Empty)
			{
				_rigidbody = GetComponent<Rigidbody> ();
				_gameobject = gameObject;
			}
			else
			{
				GameObject p = FindChild (gameObject, OverridenParent);

				if (p != null)
				{
					if (PositionTool == LBTransformTool.RigidBody)
						_rigidbody = p.GetComponent<Rigidbody> ();
					else
						_gameobject = p;
				}
			}
		}

		void FixedUpdate()
		{
			if (bApplyPosition)
				PerformMovement ();
			if (bApplyRotation)
				PerformRotation ();
		}

		protected virtual void PerformMovement ()
		{
			if (PositionTool == LBTransformTool.Transform)
				MoveByTransform ();
			else if (PositionTool == LBTransformTool.RigidBody && _rigidbody != null)
				MoveByRigidBody ();
			else if (PositionTool == LBTransformTool.RBVelocity && _rigidbody != null)
				MoveByVelocity ();
			else if (PositionTool == LBTransformTool.RBForce && _rigidbody != null)
				MoveByRBForce ();
		}

		protected virtual void MoveByTransform()
		{			
			switch (Coordinates)
			{
				case LBCoordinateSystem.Local:
					_gameobject.transform.position = _gameobject.transform.TransformPoint (Position);
					break;
				case LBCoordinateSystem.Parent:
					_gameobject.transform.position = _gameobject.transform.parent.TransformPoint (Position);
					break;
				case LBCoordinateSystem.World:
				default:
					_gameobject.transform.position = Position;
					break;
			}	
		}

		protected virtual void MoveByRigidBody()
		{	
			switch (Coordinates)
			{
				case LBCoordinateSystem.Local:
					_rigidbody.MovePosition (_rigidbody.transform.TransformPoint (Position));
					break;
				case LBCoordinateSystem.Parent:
					_rigidbody.MovePosition (_gameobject.transform.parent.TransformPoint (Position));
					break;
				case LBCoordinateSystem.World:
				default:
					_rigidbody.MovePosition (Position);
					break;
			}	
		}

		protected virtual void MoveByVelocity()
		{	
			_rigidbody.velocity = Position;
		}

		protected virtual void MoveByRBForce()
		{	
			_rigidbody.AddForce (Position, PositionForceMode);
		}

		protected virtual void PerformRotation ()
		{
			if (RotationTool == LBTransformTool.Transform)
				RotateByTransform ();
			else if (RotationTool == LBTransformTool.RigidBody && _rigidbody != null)
				RotateByRigidBody ();
			else if (RotationTool == LBTransformTool.RBVelocity && _rigidbody != null)
				RotateByVelocity ();
			else if (RotationTool == LBTransformTool.RBForce && _rigidbody != null)
				RotateByRBForce ();
		}

		protected virtual void RotateByTransform()
		{
			switch (Coordinates) 
			{
			case LBCoordinateSystem.Local:
				_gameobject.transform.rotation = (Quaternion.LookRotation(_gameobject.transform.TransformDirection(Rotation)));
				break;
			case LBCoordinateSystem.Parent:
				_gameobject.transform.localRotation = Quaternion.Euler(Rotation);
				break;
			case LBCoordinateSystem.World:
			default:
				_gameobject.transform.rotation = Quaternion.Euler(Rotation);
				break;
			}	
		}

		protected virtual void RotateByRigidBody()
		{
			switch (Coordinates) 
			{
			case LBCoordinateSystem.Local:
				_rigidbody.MoveRotation(Quaternion.LookRotation(_rigidbody.transform.TransformDirection(Rotation)));
				break;
//			case LBCoordinateSystem.Parent:
//				_rigidbody.transform.localRotation = Quaternion.Euler(Rotation);
//				break;
			case LBCoordinateSystem.World:
			default:
				_rigidbody.MoveRotation(Quaternion.Euler(Rotation));
				break;
			}	
		}

		protected virtual void RotateByVelocity()
		{
			_rigidbody.angularVelocity = Rotation;
		}

		protected virtual void RotateByRBForce()
		{	
			_rigidbody.AddTorque (Rotation, RotationForceMode);
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
	}
}