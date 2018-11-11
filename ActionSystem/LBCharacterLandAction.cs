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
		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) // нужно добавить проверку на наличие связи?
		{
			if (_dir == LBActionTransitDirection.In)
			{
				return base.CheckTransferConditions(_other, _transit, LBActionTransitDirection.In) && bHasWalkableFloor();
			}
			else
			{
				return base.CheckTransferConditions (_other, _transit, LBActionTransitDirection.Out);
			}
		}

		public override LBAnimationTransitionTypes AnimationTrasnitionType
		{
			get
			{
				return LBAnimationTransitionTypes.Crossfade;
			}
		}
	}
}
