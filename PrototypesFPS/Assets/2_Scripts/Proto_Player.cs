using UnityEngine;
using System.Collections;

public class Proto_Player : MonoBehaviour {

	Transform cam;
	public Transform prefab;

	void Start () 
	{
		cam = Camera.main.transform;
	}

	void Update () 
	{
		RaycastHit hit;
		if(Physics.Raycast (cam.position, cam.forward, out hit) )//&& hit.collider.tag == "StreetArt" && prefab != null
		{
//			print (hit.collider);
			prefab.position = hit.collider.transform.position;
			prefab.rotation = hit.collider.transform.rotation;
			prefab.gameObject.SetActive(true);
		}	
		else
			prefab.gameObject.SetActive(false);
	}
}
