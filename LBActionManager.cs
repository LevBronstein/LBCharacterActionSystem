using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	public class LBActionManager : MonoBehaviour
	{
		public LBAction[] Actions;

		void Start ()
		{
			InitActions ();
		}

		void Update ()
		{
			//Tick ();
		}

		protected virtual void InitActions()
		{
			int i;

			for (i = 0; i < Actions.Length; i++) 
			{
				if (Actions [i] != null) 
				{
					Actions [i] = Actions [i].Duplicate (); 
					Actions [i].Init (gameObject, this);
				} 
				else 
				{
					Actions [i] = new DummyAction ();
					Actions [i].Init (gameObject, this);
				}
			}
		}

//		protected virtual void Tick()
//		{
//			int i;
//
//			for (i = 0; i < Actions.Length; i++) 
//			{
//				Actions [i].Tick ();
//			}
//		}

//		protected bool CheckActivationTransitive (LBTransitiveAction action)
//		{
//
//			LBAction act;
//
//			act = FindSwitchableAction (action);
//
//			if (act == null)
//				return false;
//
//			if (action.CanActivateAction (act.ActionName))
//				return true;
//			else
//				return false;
//		}
			
//		protected void ActivateTransitive (LBTransitiveAction action)
//		{
//			LBAction act;
//
//			act = FindSwitchableAction (action);
//
//			if (act == null)
//				return;
//
//			action.ActivateAction ();
//
//			act.DeactivateAction ();
//		}

//		public virtual bool ManualActivateAction(string action)
//		{
//			LBAction a;
//
//			a = FindAction (action);
//
//			if (a == null)
//				return false;
//
//			bool b = a.GetType() == typeof(LBAction);////!!!!
//
//			if (a.ActionActivation == LBActionActivationTypes.Manual || a.ActionActivation == LBActionActivationTypes.ConditionalManual) 
//			{
//				if (a.GetType ().IsSubclassOf(typeof(LBTransitiveAction)) || a.GetType () == typeof(LBTransitiveAction)) 
//				{
//					if (CheckActivationTransitive ((LBTransitiveAction) a))
//						ActivateTransitive ((LBTransitiveAction) a);
//				}
//
//				if (a.GetType () == typeof(LBAction)) 
//				{
////					if (CheckActivationTransitive (a))
////						ActivateTransitive (a);
//				}
//			}
//
//			return false;
//		}

		public LBAction[] AllActions
		{
			get
			{
				return Actions;
			}
		}

		public LBAction[] ActiveActions
		{
			get 
			{
				LBAction[] acts = new LBAction[0];
				int i, k;

				k = 0;

				for (i = 0; i < Actions.Length; i++) 
				{
					if (Actions [i].ActionState == LBActionStates.Active) 
					{
						k++;
						Array.Resize (ref acts, k); 
						acts [k - 1] = Actions [i];
					}
				}

				return acts;
			}
		}

		public string[] AllActionNames
		{
			get 
			{
				string[] str;
				int i;

				str = new string[Actions.Length];

				for (i = 0; i < Actions.Length; i++) 
				{
					str [i] = Actions [i].ActionName;
				}

				return str;
			}
		}

		public string[] ActiveActionNames
		{
			get 
			{
				string[] str = new string[0];
				int i, k;

				k = 0;

				for (i = 0; i < Actions.Length; i++) 
				{
					if (Actions [i].ActionState == LBActionStates.Active) 
					{
						k++;
						Array.Resize (ref str, k);
						str [k - 1] = Actions [i].ActionName;
					}
				}

				return str;
			}
		}

//		LBAction FindSwitchableAction(LBTransitiveAction act)
//		{
//			int i, j;
//
//			LBAction[] active;
//
//			active = ActiveActions;
//
//			for (i = 0; i < active.Length; i++) 
//			{
//				for (j = 0; j < act.TransfersFrom.Length; j++) 
//				{
//					if (act.TransfersFrom [j] == active [i].ActionName) 
//					{
//						if (act.TransferType == LBActionTransitTypes.Interrupt)
//							return active [i];
//						
//						if (act.TransferType == LBActionTransitTypes.Switch) 
//						{
//							if (active [i].CanDeactivateAction ())
//								return active [i];
//						}
//					}
//				}
//			}
//
//			return null;
//		}
			
		LBAction FindAction(string action)
		{
			int i;

			for (i = 0; i < Actions.Length; i++) 
			{
				if (Actions [i].ActionName == action)
					return Actions [i];
			}

			return null;
		}
	}
}