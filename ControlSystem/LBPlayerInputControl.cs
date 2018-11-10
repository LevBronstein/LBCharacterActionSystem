using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBActionSystem;

namespace LBAControlSystem
{
	public class LBPlayerInputControl : LBPlayerInputControlBase
	{ 
		public GameObject TransformBase;

		Transform coords;

		protected override void PerformControl()
		{
			Vector3 v, x, z;

			v = Vector3.zero;

			if (TransformBase != null && TransformBase.transform != null)
			{
				x = TransformBase.transform.right;
				z = TransformBase.transform.forward;
				x.y = 0; x = x.normalized * Input.GetAxisRaw ("Horizontal");
				z.y = 0; z = z.normalized * Input.GetAxisRaw ("Vertical");
				v = x + z;
			}
				//v = TransformBase.transform.InverseTransformVector (v);

			StartWalk(v);

//			if (Input.GetKey (KeyCode.Space))
//				StartJump ();
		}
	
		void StartWalk(Vector3 v)
		{
			LBCharacterMovementAction mov;
			int i;

			for (i = 0; i < m.AllActions.Length; i++)
			{
				if (m.AllActions [i].ActionName == "Walk")
				{
					mov = (LBCharacterMovementAction)m.AllActions [i];

					if (mov != null)
					{
						
						mov.SetMovementSpeed(v.magnitude);

						if (v != Vector3.zero)
						{
							mov.SetMovementDir (v);
							mov.ActivateAction ();
						}
					}

//					if (mov != null)
//					{
//						mov.SetMovementSpeed(v.magnitude);
//
//						if (v != Vector3.zero)
//						{
//							mov.SetMovementDir (v);
//							mov.ActivateAction ();
//						}
//					}
						
				}
			}
		}
	
//		void StartJump()
//		{
//			LBCharacterGenericAction jmp;
//
//			jmp = (LBCharacterGenericAction)m.FindAction ("Jump");
//
//			if (jmp != null)
//				jmp.ActivateAction ();
//		}
	
	}
}