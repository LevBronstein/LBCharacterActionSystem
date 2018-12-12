using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewCharacterJumpAction", menuName = "LBActionSystem/CharacterJumpAction")]
	public class LBCharacterJumpAction : LBCharacterGenericAction 
	{
		protected override bool CheckTransferConditions(LBAction _other, LBActionTransitTypes _transit, LBActionTransitDirection _dir) 
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