﻿using System.Collections;
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

			if (Input.GetKey(KeyCode.Q))
				TurnInPlace (TransformBase.transform.right);

			if (Input.GetKey (KeyCode.E))
				TurnInPlace (-TransformBase.transform.right);

			if (Input.GetButton ("Jump"))
				StartJump ();
		}
	
		void StartWalk(Vector3 v)
		{
			LBCharacterMovementAction mov;
			LBCharacterStartWalkAction step;
		
			mov = (LBCharacterMovementAction)(m.FindAction ("Walk"));
			step = (LBCharacterStartWalkAction)(m.FindAction ("StartWalk"));

			if (mov != null)
			{
				mov.SetMovementSpeed (v.magnitude);

				if (v != Vector3.zero)
				{
					mov.SetMovementDir (v);

					if (step != null)
					{
						step.ActivateAction ();
					}
					else
						mov.ActivateAction ();
				}
			}

//			for (i = 0; i < m.AllActions.Length; i++)
//			{
//				if (m.AllActions [i].ActionName == "Walk")
//				{
//					mov = (LBCharacterMovementAction)m.AllActions [i];
//
//					if (mov != null)
//					{
//						
//						mov.SetMovementSpeed(v.magnitude);
//
//						if (v != Vector3.zero)
//						{
//							mov.SetMovementDir (v);
//							mov.ActivateAction ();
//						}
//					}						
//				}
//			}
		}
	
		void TurnInPlace(Vector3 v)
		{
			LBCharacterTurnInPlaceAction t;

			t = (LBCharacterTurnInPlaceAction)m.FindAction ("TurnInPlace");

			if (t != null)
			{
				t.SetMovementDir(v);
				t.ActivateAction ();
			}
		}

		void StartJump()
		{
			LBCharacterJumpAction jmp;

			jmp = (LBCharacterJumpAction)m.FindAction ("Jump");

			if (jmp != null)
				jmp.ActivateAction ();
		}
	
	}
}