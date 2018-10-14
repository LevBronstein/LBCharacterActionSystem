using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSphereCast : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	protected void OnDrawGizmos()
	{
		Collider c;

		c = GetComponent<Collider>();

		Gizmos.color = Color.red;
		Gizmos.DrawSphere (c.bounds.center, c.bounds.extents.x);
	}

}
