using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInputMove : MonoBehaviour {

	public bool isMove {get{if(zMoveSpeed != 0) return true;return false;}}

	private float maxZ;
	private float minZ;

	Vector2 mousePosition;
	float zMoveSpeed = 0;
	// Update is called once per frame
	void Update () 
	{
		if(transform.position.z <= maxZ && transform.position.z >= minZ)
		{
			if(Input.GetMouseButtonDown (0))
			{
				mousePosition = Input.mousePosition;
			}

			if(Input.GetMouseButton (0))
			{
				zMoveSpeed =  mousePosition.y - Input.mousePosition.y;
				mousePosition = Input.mousePosition;
			}

			if(zMoveSpeed != 0)
			{
				zMoveSpeed += (0 - zMoveSpeed)*Time.deltaTime * 4;
			}

			Vector3 nextPos = transform.position + new Vector3(0,0,zMoveSpeed*0.5f*Time.deltaTime);

			if(nextPos.z <= maxZ && nextPos.z >= minZ)
				transform.position = nextPos;
		}
		else
		{
			if(transform.position.z > maxZ)
				transform.position = new Vector3(0,transform.position.y,maxZ);
			if(transform.position.z < minZ)
				transform.position = new Vector3(0,transform.position.y,minZ);
			zMoveSpeed = 0;
		}
	}

	public void SetMaxMinZ (float setMaxZ,float setMinZ)
	{
		maxZ = setMaxZ;
		minZ = setMinZ;
	}

	public void CenterOn (Transform obj)
	{
		transform.position = new Vector3 (0,transform.position.y,obj.transform.position.z - 4);
	}
}
