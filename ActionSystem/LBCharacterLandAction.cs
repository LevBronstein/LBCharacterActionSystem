using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	/// <summary>
	/// Activates when the character lands (on the floor or somewhere else). A default output of the LBCharacterFallAction action
	/// </summary>
	[CreateAssetMenu (fileName = "NewCharacterLandAction", menuName = "LBActionSystem/CharacterLandAction")]
	public class LBCharacterLandAction : LBCharacterGenericAction
	{
		public bool bPreserveFlatSpeed;
		public bool bUseRestraintSpeedIn;
		public LBSpeedRestraint SpeedRestraintIn;

		protected Vector3 origvel;

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

			if (bPreserveFlatSpeed)
			{
				origvel = RBSpeedVector;
			}
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return base.CheckTransferConditions(_other, _transit, LBActionTransitDirection.In) && bCanLand();
			}
			else
			{
				return base.CheckTransferConditions (_other, _transit, LBActionTransitDirection.Out);
			}
		}

		protected override void PerformMovement ()
		{
			if (bPreserveFlatSpeed)
			{
				RBSpeedVector = origvel; // what if we have some other physic forces
			}
		}

		public override LBAnimationTransitionTypes AnimationTrasnitionType
		{
			get
			{
				return LBAnimationTransitionTypes.Crossfade;
			}
		}

		public bool bHasPropperTransferInSpeed()
		{
			if (CheckSpeedRestraint (TruncFloat (RBFlatSpeed), SpeedRestraintIn))
				return true;
			else
				return false;
		}
			
		protected bool bCanLand()
		{
			bool b;

			b = true;

			b = b && bHasWalkableFloor();

			if (bUseRestraintSpeedIn)
				b = b && bHasPropperTransferInSpeed ();

			return b;
		}

		public override LBAction Duplicate ()
		{
			LBCharacterLandAction dup;

			dup = (LBCharacterLandAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterLandAction)dup).bUseRestraintSpeedIn = bUseRestraintSpeedIn;
			((LBCharacterLandAction)dup).bPreserveFlatSpeed = bPreserveFlatSpeed;
		}

	}
}
