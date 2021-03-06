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

		protected override void Start()
		{
			base.Start ();
			if (TransformBase == null)
				TransformBase = gameObject;
		}

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

			//StartWalk(v);

			WalkCharacter (v.magnitude, v.normalized);

			if (Input.GetKey(KeyCode.Q))
				TurnInPlace (TransformBase.transform.right);

			if (Input.GetKey (KeyCode.E))
				TurnInPlace (-TransformBase.transform.right);

			if (Input.GetButton ("Jump"))
			{
				JumpInPlace ();
				JumpInMotion ();
			}

			v = Vector3.zero;
			v.x = Input.GetAxisRaw ("RotationHorAux");
			v.y = Input.GetAxisRaw ("RotationVerAux");

			RotateHead (v * 4);
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
	
		void WalkCharacter(float spd, Vector3 dir)
		{
			int i;
			LBCharacterMovementAction mov;
			LBCharacterStartWalkAction start;

			start = (LBCharacterStartWalkAction)(m.FindAction ("StartWalk"));

			for (i = 0; i < m.AllActions.Length; i++)
			{
				if (m.AllActions [i].GetType() != (typeof(LBCharacterMovementAction)) && !(m.AllActions [i].GetType().IsSubclassOf(typeof(LBCharacterMovementAction))))
					continue;
				
				mov = (LBCharacterMovementAction) (m.AllActions [i]);

				if (mov != null)
				{
					mov.SetMovementSpeed (spd);

					if (dir != Vector3.zero)
						mov.SetMovementDir (dir);
				}
			}

			if (start != null && spd != 0)
				start.ActivateAction ();
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

		void JumpInPlace()
		{
			LBCharacterJumpAction jmp;

			jmp = (LBCharacterJumpAction)m.FindAction ("JumpInPlace");

			if (jmp != null)
				jmp.ActivateAction ();
		}

		void JumpInMotion()
		{
			LBCharacterJumpAction jmp;

			jmp = (LBCharacterJumpAction)m.FindAction ("JumpInMotion");

			if (jmp != null)
				jmp.ActivateAction ();
		}

		void RotateHead(Vector3 v)
		{
			LBPhysicsOffset offs;

			offs = (LBPhysicsOffset)m.FindAction ("HeadRotation");

			if (offs != null)
				offs.OffsetRotation += v;
		}
	
	}
}