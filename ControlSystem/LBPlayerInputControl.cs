using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBActionSystem;

namespace LBAControlSystem
{
	[RequireComponent (typeof(LBActionManager))]
	public class LBPlayerInputControl : MonoBehaviour
	{ 
		LBActionManager m;

		void Start ()
		{
			m = GetComponent<LBActionManager> ();
		}

		void Update ()
		{
			PerformControl ();
		}

		void PerformControl()
		{
			Vector3 v;

			v.x = Input.GetAxis ("Horizontal");
			v.z = Input.GetAxis ("Vertical");
			v.y = 0;
				
			StartWalk(v);
		}
	
		void StartWalk(Vector3 v)
		{
			LBMovementAction mov;
			int i;

			for (i = 0; i < m.AllActions.Length; i++)
			{
				if (m.AllActions [i].ActionName == "Walk")
				{
					mov = (LBMovementAction)m.AllActions [i];

					if (mov != null && v != Vector3.zero)
					{
						mov.ActivateAction ();
						mov.SetMovementDir (v);
						mov.SetMovementSpeed (v.magnitude);
					}
				}
			}
		}
	}

}