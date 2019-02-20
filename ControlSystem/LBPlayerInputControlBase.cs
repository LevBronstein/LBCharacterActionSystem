using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBActionSystem;

namespace LBAControlSystem
{
	[RequireComponent (typeof(LBActionManager))]
	public class LBPlayerInputControlBase : MonoBehaviour 
	{
		protected LBActionManager m;

		protected virtual void Start ()
		{
			m = GetComponent<LBActionManager> ();
		}
		
		void Update ()
		{
			PerformControl ();
		}

		protected virtual void PerformControl()
		{
			
		}

	}
}
