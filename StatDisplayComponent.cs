using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LBActionSystem;

public class StatDisplayComponent : MonoBehaviour
{

	public GameObject character;
	LBActionManager m;
	Text t;

	// Use this for initialization
	void Start ()
	{
		m = character.GetComponent<LBActionManager> ();
		t = gameObject.GetComponent <Text> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		int i;
		LBAction[] actions;
		
		actions = m.ActiveActions;

		t.text = "";

		for (i = 0; i < actions.Length; i++)
		{
			t.text += actions [i].ToString () + "\n\n";
		}
	}
}
