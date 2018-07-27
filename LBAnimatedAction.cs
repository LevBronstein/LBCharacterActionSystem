using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu (fileName = "NewAnimatedAction", menuName = "LBActionSystem/AnimatedAction")]
public class LBAnimatedAction: LBAction
{
	public string Animation;

	protected override bool Init(GameObject parentgameobject)
	{
		if (!base.Init(parentgameobject))
			return false;

		Animator anm;

		anm = parent.GetComponent<Animator> ();

		return true;
	}
}

[CustomEditor(typeof(LBAnimatedAction))]
public class LBAnimatedActionCustomEditor: Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.Popup("MyDropdown", 0,new string [] {"one", "two", "three"},EditorStyles.popup);
		EditorGUILayout.EndHorizontal ();
	}
}
