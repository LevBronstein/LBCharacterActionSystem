using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[System.Serializable]
	public struct LBSpeedRestraint
	{
		public float MinSpeed;
		public bool bStrictMin;
		public float MaxSpeed;
		public bool bStricMax;
		public bool OuterInterval;
	}

	/// <summary>
	/// Handles walk action of an arbitary character, which is performed in a forward direction on a flat surface.
	/// </summary>
	[CreateAssetMenu (fileName = "NewCharacterWalkAction", menuName = "LBActionSystem/CharacterWalkAction")]
	public class LBCharacterWalkAction : LBCharacterMovementAction 
	{
		/// <summary>
		/// A value of the maximum availiable movement speed for the character in this state in Unity units. Modified by a coefficient.
		/// </summary>
		public float BaseMovementSpeed = 3.0f;
		/// <summary>
		/// A value of the constant rotation speed for the character in this state.
		/// </summary>
		public float BaseRotationSpeed = 5.0f;
		/// <summary>
		/// A value of the constant acceleration during movement for the character in this state.
		/// </summary>
		public float MovementAcceleration = 0.15f;

		//Animation paramteres' names in the animator
		public string CharVelocityParamName = "CharVelocityNorm";
		public string CharDeltaRotParamName = "CharDeltaRotNorm";

		public LBSpeedRestraint SpeedRestraintIn;
		public LBSpeedRestraint SpeedRestraintOut;

		protected Vector3 idealvelocity;

//		public float MinSpeedLimit = 0.0f; // Min amount of forward speed
//		public float MaxSpeedLimit = 3.0f;

		protected virtual void Move() // Time.deltaTime ???
		{
			float curspd, destspd;

			destspd = Mathf.Clamp (BaseMovementSpeed * Mathf.Clamp01 (MovementSpeed), SpeedRestraintOut.MinSpeed, SpeedRestraintOut.MaxSpeed);
			curspd = LerpFloat (idealvelocity.magnitude, destspd, MovementAcceleration, Time.fixedDeltaTime);
			idealvelocity = RBForwardDir.normalized * curspd;
			rigidbody.velocity = idealvelocity;

			if (animator != null)
			{
				animator.SetFloat (CharVelocityParamName, curspd / BaseMovementSpeed);
			}
		}

		protected virtual void Rotate()
		{
			float currot, destrot;

			//Debug.Log (MovementDir);
			destrot = Vector3.SignedAngle (RBForwardDir, new Vector3 (MovementDir.x, 0, MovementDir.z), Vector3.up);
			//destrot = SingedAngle(RBForwardDir, MovementDir, Vector3.up);
			currot = LerpFloat (0, TruncFloat(destrot,1), BaseRotationSpeed, Time.fixedTime);
			rigidbody.rotation = Quaternion.LookRotation (Quaternion.AngleAxis (TruncFloat(currot), Vector3.up) * RBForwardDir);
			//Debug.Log (destrot);
			if (animator != null)
			{
				float lastrot, newrot;

				lastrot = animator.GetFloat (CharDeltaRotParamName) * 180;

				newrot = LerpFloat (lastrot, destrot, BaseRotationSpeed * 10, Time.fixedTime) / 180;

				animator.SetFloat (CharDeltaRotParamName, newrot);

//				if (Mathf.Sign (lastrot) == Mathf.Sign (newrot))
//					animator.SetFloat (CharDeltaRotParamName, newrot);
//				else
//				{
//					
//				}
			}
		}

		protected override void PerformMovement ()
		{
			Move ();
			Rotate ();
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return bHasControlImpulse() && bHasWalkableFloor () && bHasPropperTransferInSpeed();
			}
			else
			{
				return true;
			}
		}

//		protected override void TrySelfDeactivate()
//		{
//			if (!bHasWalkableFloor () || bHasPropperTransferOutSpeed())
//			{
//				DeactivateAction ();
//			}
//		}

		protected override bool CheckSelfDeactivationCondtions ()
		{
			return !bHasWalkableFloor () || bHasPropperTransferOutSpeed();
		}

//		protected bool bHasWalkableFloor()
//		{
//			Collider c;
//			Ray r;
//			RaycastHit hit;
//
//			c = parent.GetComponent<Collider>();
//
//			if (c == null)
//				return false;
//
//			r = new Ray (c.bounds.center, Vector3.down);
//
//			Debug.DrawRay (r.origin, r.direction, Color.green);
//
////			if (Physics.Raycast (r.origin, r.direction, out hit, c.bounds.extents.y+0.05f)) 
////			{
////				//Debug.Log (hit.transform.gameObject.name);
////				if (hit.transform.gameObject.name != parent.name)
////					return true;
////			}
//
//			if (Physics.SphereCast (r, c.bounds.extents.x*2, out hit, c.bounds.extents.y+0.05f))
//			{
//				if (hit.transform.gameObject.name != parent.name)
//					return true;
//			}
//
//			return false;
//		}
		protected bool bHasPropperTransferInSpeed()
		{
			if (rigidbody.velocity != Vector3.zero)
			{
				if (Vector3.Angle (rigidbody.transform.forward, rigidbody.velocity) < 5.0f && CheckSpeedRestraint (TruncFloat (rigidbody.velocity.magnitude), SpeedRestraintIn))
					return true;
			}
			else
			{
				if (CheckSpeedRestraint (TruncFloat (RBSpeed), SpeedRestraintIn))
					return true;
			}
			
			return false;
		}

		protected bool bHasPropperTransferOutSpeed()
		{
//			if (Vector3.Angle (rigidbody.transform.forward, RBSpeedDir) > 5.0f)
//				return true;
			if (CheckSpeedRestraint(TruncFloat(RBSpeed), SpeedRestraintOut))
				return true;

			return false;
		}

		protected bool bHasControlImpulse()
		{
			if (TruncFloat (MovementSpeed) > 0)
				return true;
			else
				return false;
		}

//		protected bool bHasPropperOutTransferSpeed()
//		{
//			if (Vector3.Angle (rigidbody.transform.forward, rigidbody.velocity) > 5.0f)
//				return true;
//			
//			if (CheckSpeedRestraint(rigidbody.velocity.magnitude, InTransferSpeed))
//				return true;
//
//			return false;
//		}

		protected bool CheckSpeedRestraint(float spd, LBSpeedRestraint rest)
		{
			float min, max;

			min = Mathf.Min (rest.MinSpeed, rest.MaxSpeed);
			max = Mathf.Max (rest.MinSpeed, rest.MaxSpeed);

			if (!rest.OuterInterval)
			{
				if (spd >= min && !rest.bStrictMin || spd > min && rest.bStrictMin)
				{
					if (spd <= max && !rest.bStrictMin || spd < max && rest.bStricMax)
						return true;
				}
			}
			else
			{
				if (spd <= min && !rest.bStrictMin || spd < min && rest.bStrictMin)
					return true;

				if (spd >= max && !rest.bStrictMin || spd > max && rest.bStricMax)
					return true;
			}

			return false;
		}

		public virtual void SetBaseMovementSpeed(float _basespd)
		{
			BaseMovementSpeed = _basespd;
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

			((LBCharacterWalkAction)dup).SpeedRestraintIn = SpeedRestraintIn;
			((LBCharacterWalkAction)dup).SpeedRestraintOut = SpeedRestraintOut;
			((LBCharacterWalkAction)dup).BaseMovementSpeed = BaseMovementSpeed;
			((LBCharacterWalkAction)dup).BaseRotationSpeed = BaseRotationSpeed;
			((LBCharacterWalkAction)dup).MovementAcceleration = MovementAcceleration;
		}
	
	}
}
