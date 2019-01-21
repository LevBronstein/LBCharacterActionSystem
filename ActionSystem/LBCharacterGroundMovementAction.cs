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
		public bool bStrictMax;
		public bool OuterInterval;
	}

	[System.Serializable]
	public struct LBDirectionRestraint
	{
		public float MinDir;
		public bool bStrictMin;
		public float MaxDir;
		public bool bStricMax;
		public bool OuterInterval;
	}


	public class LBCharacterGroundMovementAction : LBCharacterMovementAction
	{
		protected bool CheckSpeedRestraint(float spd, LBSpeedRestraint rest)
		{
			float min, max;

			min = Mathf.Min (rest.MinSpeed, rest.MaxSpeed);
			max = Mathf.Max (rest.MinSpeed, rest.MaxSpeed);

			if (!rest.OuterInterval)
			{
				if (spd >= min && !rest.bStrictMin || spd > min && rest.bStrictMin)
				{
					if (spd <= max && !rest.bStrictMin || spd < max && rest.bStrictMax)
						return true;
				}
			}
			else
			{
				if (spd <= min && !rest.bStrictMin || spd < min && rest.bStrictMin)
					return true;

				if (spd >= max && !rest.bStrictMin || spd > max && rest.bStrictMax)
					return true;
			}

			return false;
		}

		protected bool CheckDirectionRestraint(float sang, LBDirectionRestraint rest)
		{
			float min, max;

			min = Mathf.Min (rest.MinDir, rest.MaxDir);
			max = Mathf.Max (rest.MinDir, rest.MaxDir);

			if (!rest.OuterInterval)
			{
				if (sang >= min && !rest.bStrictMin || sang > min && rest.bStrictMin)
				{
					if (sang <= max && !rest.bStrictMin || sang < max && rest.bStricMax)
						return true;
				}
			}
			else
			{
				if (sang <= min && !rest.bStrictMin || sang < min && rest.bStrictMin)
					return true;

				if (sang >= max && !rest.bStrictMin || sang > max && rest.bStricMax)
					return true;
			}

			return false;
		}

		public float DirectionDifference
		{
			get
			{
				return Vector3.SignedAngle  (new Vector3 (RBForwardDir.x, 0, RBForwardDir.z), new Vector3 (MovementDir.x, 0, MovementDir.z), Vector3.up);
			}
		}

		public float SpeedDifference
		{
			get
			{
				return Mathf.Abs(MovementSpeed - RBSpeed);
			}
		}
	}

}