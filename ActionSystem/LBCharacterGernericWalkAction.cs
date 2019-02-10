using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterGernericWalkAction", menuName = "LBActionSystem/CharacterGernericWalkAction")]
	public class LBCharacterGernericWalkAction : LBCharacterGenericAction
	{

		public LBActionPerformanceTypes PerformanceType;
		public LBAnimationTransitionTypes AnimTransitionType;

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

		public bool bNeedsControlImpulse;

		public bool bPreserveSpeed;
		public bool bAutoDetectInputSpeed;
		public bool bBlockExternalDir;
		public bool bBlockExternalSpeed;

		public bool bUseRestraintSpeedIn;
		public LBSpeedRestraint SpeedRestraintIn;
		public bool bUseRestraintSpeedOut;
		public LBSpeedRestraint SpeedRestraintOut;

		public bool bUseRestraintDirectionIn;
		public LBDirectionRestraint DirectionRestraintIn;
		public bool bUseRestraintDirectionOut;
		public LBDirectionRestraint DirectionRestraintOut;

		protected Vector3 idealvelocity;

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

			if (bPreserveSpeed)
			{
//				MovementSpeed = RBFlatSpeed;
				MovementDir = RBFlatSpeedDir;
				idealvelocity = new Vector3 (RBSpeedVector.x, 0, RBSpeedVector.z);

				if (bAutoDetectInputSpeed)
					base.SetMovementSpeed (idealvelocity.magnitude / BaseMovementSpeed);
			}
		}

		protected virtual void Move() 
		{
			float curspd, destspd;

			if (bUseRestraintSpeedOut)
				destspd = Mathf.Clamp (BaseMovementSpeed * Mathf.Clamp01 (MovementSpeed), SpeedRestraintOut.MinSpeed, SpeedRestraintOut.MaxSpeed);
			else
				destspd = BaseMovementSpeed * Mathf.Clamp01 (MovementSpeed);
			
			curspd = LerpFloat (idealvelocity.magnitude, destspd, MovementAcceleration, Time.fixedDeltaTime);
			///curspd = LerpFloat (RBFlatSpeed, destspd, MovementAcceleration, Time.fixedDeltaTime);

			idealvelocity = (new Vector3(RBForwardDir.x, 0, RBForwardDir.z)).normalized * curspd;
			RBSpeedVector = idealvelocity + new Vector3(0, RBSpeedVector.y, 0);
			//RBSpeedVector = RBForwardDir * curspd + Physics.gravity;

			SetVelocityParam(curspd / BaseMovementSpeed);
		}

		protected virtual void Rotate()
		{
			float currot, destrot;

			destrot = Vector3.SignedAngle (RBForwardDir, new Vector3 (MovementDir.x, 0, MovementDir.z), Vector3.up);
			currot = LerpFloat (0, TruncFloat(destrot,1), BaseRotationSpeed, Time.fixedTime);
			rigidbody.rotation = Quaternion.LookRotation (Quaternion.AngleAxis (TruncFloat(currot), Vector3.up) * RBForwardDir);
			if (animator != null)
			{
				float lastrot, newrot;
				lastrot = GetDeltaRotParam(LBAxisTypes.YAxis) * 180;
				newrot = LerpFloat (lastrot, destrot, BaseRotationSpeed * 10, Time.fixedTime) / 180;
				SetDeltaRotParam(newrot, LBAxisTypes.YAxis);
			}
		}

		protected override void PerformMovement ()
		{
			Move ();
			Rotate ();
		}

		protected override void UpdateSliders() {}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				if (bCanWalk ())
					return true;
				else
					return false;
			}
			else
			{
				return base.CheckTransferConditions(_other, _transit, _dir) || bCanStopWalk();
			}
		}

		protected override bool CheckSelfDeactivationCondtions ()
		{
			if (base.CheckSelfDeactivationCondtions() || bCanStopWalk ())
				return true;
			else
				return false;
		}

		protected bool bCanWalk()
		{
			bool b;

			b = true;

			b = b && bHasWalkableFloor ();

			if (bNeedsControlImpulse)
				b = b && bHasControlImpulse ();

			if (bUseRestraintSpeedIn)
				b = b && bHasPropperTransferInSpeed ();

			if (bUseRestraintDirectionIn)
				b = b && bHasPropperTransferInDirection ();

			return b;
		}

		protected bool bCanStopWalk()
		{
			bool b;

			b = false;

			b = b || !bHasWalkableFloor ();

			if (bUseRestraintSpeedOut)
				b = b || bHasPropperTransferOutSpeed ();

			if (bUseRestraintDirectionOut)
				b = b || bHasPropperTransferOutDirection ();

			return b;
		}
			
		public bool bHasPropperTransferInSpeed()
		{
			if (CheckSpeedRestraint (TruncFloat (RBFlatSpeed), SpeedRestraintIn))
				return true;
			else
				return false;
		}

		public bool bHasPropperTransferOutSpeed()
		{
			if (CheckSpeedRestraint(TruncFloat(RBFlatSpeed), SpeedRestraintOut))
				return true;

			return false;
		}

		public bool bHasPropperTransferInDirection()
		{
			if (CheckDirectionRestraint (DirectionDifference, DirectionRestraintIn))
				return true;

			return false;
		}

		public bool bHasPropperTransferOutDirection()
		{
			if (CheckDirectionRestraint (DirectionDifference, DirectionRestraintOut))
				return true;

			return false;
		}
			
		public bool bHasControlImpulse()
		{
			if (TruncFloat (MovementSpeed) > 0)
				return true;
			else
				return false;
		}

		public override void SetMovementDir (Vector3 _dir)
		{
			if (!bBlockExternalDir)
				base.SetMovementDir (_dir);
		}

		public override void SetMovementSpeed (float _speed)
		{
			if (!bBlockExternalSpeed)
				base.SetMovementSpeed (_speed);
		}

		public virtual void SetBaseMovementSpeed(float _basespd)
		{
			BaseMovementSpeed = _basespd;
		}

		public override LBActionPerformanceTypes ActionPerfomacneType
		{
			get 
			{
				return PerformanceType;
			}
		}

		public override LBAnimationTransitionTypes AnimationTrasnitionType
		{
			get
			{
				return AnimTransitionType;
			}
		}

		public override LBAction Duplicate ()
		{
			LBCharacterGernericWalkAction dup;

			dup = (LBCharacterGernericWalkAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterGernericWalkAction)dup).PerformanceType = PerformanceType;
			((LBCharacterGernericWalkAction)dup).AnimTransitionType = AnimTransitionType;

			((LBCharacterGernericWalkAction)dup).bUseRestraintSpeedIn = bUseRestraintSpeedIn;
			((LBCharacterGernericWalkAction)dup).SpeedRestraintIn = SpeedRestraintIn;
			((LBCharacterGernericWalkAction)dup).bUseRestraintSpeedOut = bUseRestraintSpeedOut;
			((LBCharacterGernericWalkAction)dup).SpeedRestraintOut = SpeedRestraintOut;

			((LBCharacterGernericWalkAction)dup).bUseRestraintDirectionIn = bUseRestraintDirectionIn;
			((LBCharacterGernericWalkAction)dup).DirectionRestraintIn = DirectionRestraintIn;
			((LBCharacterGernericWalkAction)dup).bUseRestraintDirectionOut = bUseRestraintDirectionOut;
			((LBCharacterGernericWalkAction)dup).DirectionRestraintOut = DirectionRestraintOut;

			((LBCharacterGernericWalkAction)dup).BaseMovementSpeed = BaseMovementSpeed;
			((LBCharacterGernericWalkAction)dup).BaseRotationSpeed = BaseRotationSpeed;
			((LBCharacterGernericWalkAction)dup).MovementAcceleration = MovementAcceleration;

			((LBCharacterGernericWalkAction)dup).bBlockExternalSpeed = bBlockExternalSpeed;
			((LBCharacterGernericWalkAction)dup).bBlockExternalDir = bBlockExternalDir;

			((LBCharacterGernericWalkAction)dup).bNeedsControlImpulse = bNeedsControlImpulse;
			((LBCharacterGernericWalkAction)dup).bPreserveSpeed = bPreserveSpeed;
			((LBCharacterGernericWalkAction)dup).bAutoDetectInputSpeed = bAutoDetectInputSpeed;
		}
	
	
	}
}
