using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBActionSystem;

namespace LBAControlSystem
{
	[System.Serializable]
	public struct KeyActionLink
	{
		public KeyCode Key;
		public string ActionName;
	}

	public class LBPlayerInputGenericControl : LBPlayerInputControlBase 
	{
		public KeyActionLink[] ActivatableActions;

		protected override void PerformControl()
		{
			int i;
			LBAction a;

			for (i = 0; i < ActivatableActions.Length; i++)
			{
				if (Input.GetKey (ActivatableActions [i].Key))
				{
					a = m.FindAction (ActivatableActions [i].ActionName);

					if (a != null)
						a.ActivateAction ();
				}
			}
		}
	}
}
