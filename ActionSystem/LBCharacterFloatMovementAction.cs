using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	public enum LBJitterType
	{
		Sinusoidal,
	}

	[CreateAssetMenu (fileName = "NewCharacterFloatMovementAction", menuName = "LBActionSystem/CharacterFloatMovementAction")]
	public class LBCharacterFloatMovementAction : LBCharacterMovementAction
	{
		public Vector3 Jitter;
		public float JitterAmp = 1;
		public LBJitterType JitterType;

		public override bool Init (GameObject parentgameobject, LBActionManager manager)
		{
			return base.Init (parentgameobject, manager);
		}

		protected override void Activate (LBAction _prev, LBActionTransitTypes _transit)
		{
			base.Activate (_prev, _transit);
		}

		protected override void PerformMovement ()
		{
			base.PerformMovement ();

			if (JitterType == LBJitterType.Sinusoidal)
				SinJitter ();
		}

		protected void SinJitter()
		{
			rigidbody.velocity = rigidbody.velocity + Mathf.Sin(Time.deltaTime * JitterAmp) * Jitter;
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

			((LBCharacterFloatMovementAction)dup).Jitter = Jitter;
			((LBCharacterFloatMovementAction)dup).JitterAmp = JitterAmp;
			((LBCharacterFloatMovementAction)dup).JitterType = JitterType;
		}
	}
}
