using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterFloatMovementAction", menuName = "LBActionSystem/CharacterFloatMovementAction")]
	public class LBCharacterFloatMovementAction : LBCharacterMovementAction
	{
		protected Quaternion _floatrot;

		public float MaxVelocity = 0.025f;

		public bool UseInitialFloatLocation = true;
		public Vector3 FloatLocation;

		public bool UseProjection;
		public Vector3 ProjectionAxis;

		public bool UseRestraintX;
		public LBAngleRestraint RotationRestraintX;
		public bool UseRestraintY;
		public LBAngleRestraint RotationRestraintY;
		public bool UseRestraintZ;
		public LBAngleRestraint RotationRestraintZ;


		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			_floatrot = rigidbody.rotation;

			if (UseInitialFloatLocation)
				FloatLocation = rigidbody.position;
			
			return base.Init (parentgameobject, manager);
		}

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);
		}

		protected override void PerformMovement()
		{
			base.PerformMovement ();

			Vector3 delta;
			Vector3 proj;

			delta = FloatLocation - rigidbody.position;
			proj = Vector3.Project (delta, Physics.gravity);

			if (UseProjection)
				delta = Vector3.Project (delta, ProjectionAxis);

			////if (Vector3.Dot (proj, Physics.gravity) > 0)
			//_rigidbody.AddForce (-Physics.gravity, ForceMode.Acceleration);

			rigidbody.AddForce (delta.normalized * Mathf.Clamp (delta.magnitude, 0, MaxVelocity), ForceMode.VelocityChange);

			Vector3 v = rigidbody.rotation.eulerAngles;

			if (UseRestraintX)
			{
				if (!CheckAngleLimit (rigidbody.rotation.eulerAngles.x, RotationRestraintX))
				{
					rigidbody.constraints = rigidbody.constraints | RigidbodyConstraints.FreezeRotationX;
					v.x = ClampAngle (rigidbody.rotation.eulerAngles.x, RotationRestraintX);
				}
				else
					rigidbody.constraints = rigidbody.constraints & ~RigidbodyConstraints.FreezeRotationX;
			}

			if (UseRestraintY)
			{
				if (!CheckAngleLimit (rigidbody.rotation.eulerAngles.y, RotationRestraintY))
				{
					rigidbody.constraints = rigidbody.constraints | RigidbodyConstraints.FreezeRotationY;
					v.y = ClampAngle (rigidbody.rotation.eulerAngles.y, RotationRestraintY);
				}
				else
					rigidbody.constraints = rigidbody.constraints & ~RigidbodyConstraints.FreezeRotationY;
			}

			if (UseRestraintZ)
			{
				if (!CheckAngleLimit (rigidbody.rotation.eulerAngles.z, RotationRestraintZ))
				{
					rigidbody.constraints = rigidbody.constraints | RigidbodyConstraints.FreezeRotationZ;
					v.z = ClampAngle (rigidbody.rotation.eulerAngles.z, RotationRestraintZ);
				}
				else 
					rigidbody.constraints = rigidbody.constraints & ~RigidbodyConstraints.FreezeRotationZ;
			}

			//_rigidbody.AddTorque (_rigidbody.rotation.eulerAngles - v, ForceMode.VelocityChange);
			rigidbody.MoveRotation(Quaternion.Euler(v));
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return bIsWeightless ();
				//return true; //!bHasWalkableFloor ();
			}
			else
			{
				return true;
			}
		}

		protected override void TrySelfActivate()
		{
			if (bIsWeightless())
			{
				ActivateAction ();
			}
		}

		protected bool bIsWeightless()
		{
			return !rigidbody.useGravity;
		}

		protected bool CheckAngleLimit(float value, LBAngleRestraint limit)
		{
			if (!limit.outer)
			{
				if (value > Mathf.Min (limit.min, limit.max) && value < Mathf.Max (limit.min, limit.max))
					return true;
				else
					return false;
			}
			else
			{
				if (value < Mathf.Min (limit.min, limit.max) || value > Mathf.Max (limit.min, limit.max))
					return true;
				else
					return false;
			}
		}

		protected float ClampAngle(float value, LBAngleRestraint limit)
		{
			if (!limit.outer)
			{
				return Mathf.Clamp (value, limit.min, limit.max);
			}
			else
			{
				float c, min, max;
				min = Mathf.Min (limit.min, limit.max);
				max = Mathf.Max (limit.min, limit.max);

				if (value >= min && value <= max)
				{
					c = max - min;

					if (value < c)
						return limit.min;
					else 
						return limit.max;
				}
			}

			return value;
		}


		public override LBAction Duplicate ()
		{
			LBCharacterFloatMovementAction dup;

			dup = (LBCharacterFloatMovementAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}
			
		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterFloatMovementAction)dup).MaxVelocity = MaxVelocity;
			((LBCharacterFloatMovementAction)dup).UseInitialFloatLocation = UseInitialFloatLocation;
			((LBCharacterFloatMovementAction)dup).FloatLocation = FloatLocation;
			((LBCharacterFloatMovementAction)dup).UseProjection = UseProjection;
			((LBCharacterFloatMovementAction)dup).ProjectionAxis = ProjectionAxis;
			((LBCharacterFloatMovementAction)dup).UseRestraintX = UseRestraintX;
			((LBCharacterFloatMovementAction)dup).RotationRestraintX = RotationRestraintX;
			((LBCharacterFloatMovementAction)dup).UseRestraintY = UseRestraintY;
			((LBCharacterFloatMovementAction)dup).RotationRestraintY = RotationRestraintY;
			((LBCharacterFloatMovementAction)dup).UseRestraintZ = UseRestraintZ;
			((LBCharacterFloatMovementAction)dup).RotationRestraintZ = RotationRestraintZ;
		}
	}
}
