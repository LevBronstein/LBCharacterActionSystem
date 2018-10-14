using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBoxCast : MonoBehaviour
{

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
		Gizmos.DrawCube (c.bounds.center - new Vector3 (0, c.bounds.extents.y, 0), c.bounds.extents + new Vector3 (0, 0.05f, 0));
	}
}
