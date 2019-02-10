using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterFallAction", menuName = "LBActionSystem/CharacterFallAction")]
	public class LBCharacterFallAction : LBCharacterMovementAction 
	{
		public bool bPreserveFlatSpeed;
		public float MaxFlatSpeed = 1;

		protected Vector3 origvel;

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);

			if (bPreserveFlatSpeed)
			{
				origvel = new Vector3 (RBSpeedVector.x, 0, RBSpeedVector.z);
			}
		}
			
		protected override void PerformMovement ()
		{
			if (bPreserveFlatSpeed)
			{
				RBSpeedVector = origvel + Physics.gravity; // what if we have some other physic forces
			}
		}

		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return !bHasWalkableFloor ();
				//return true; //!bHasWalkableFloor ();
			}
			else
			{
				return bHasWalkableFloor ();
			}
		}

		protected override void TrySelfActivate()
		{
			if (!bHasWalkableFloor ())
			{
				ActivateAction ();
			}
		}

		protected override void TrySelfDeactivate()
		{
			if (bHasWalkableFloor ())
			{
				DeactivateAction ();
			}
		}

		protected override void UpdateSliders()
		{
			base.UpdateSliders ();

			if (MaxFlatSpeed !=0)
				SetVelocityParam ((new Vector3(RBSpeedVector.x, 0, RBSpeedVector.z)).magnitude / MaxFlatSpeed);
			else 
				SetVelocityParam (0);
		}

		public override LBAnimationTransitionTypes AnimationTrasnitionType
		{
			get
			{
				return LBAnimationTransitionTypes.Crossfade;
			}
		}

		public override LBAction Duplicate ()
		{
			LBCharacterFallAction dup;

			dup = (LBCharacterFallAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}
			
		protected override void DuplicateProperties(LBAction dup)
		{
			base.DuplicateProperties (dup);

			((LBCharacterFallAction)dup).bPreserveFlatSpeed = bPreserveFlatSpeed;
			((LBCharacterFallAction)dup).MaxFlatSpeed = MaxFlatSpeed;
		}

	}
}
