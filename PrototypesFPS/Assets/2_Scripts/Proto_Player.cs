using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class Proto_Player : MonoBehaviour {

	Transform cam;
	public Transform prefab;
	public Image iconLoading;
	public Image paintingToShow;

//	private bool loaded = false;
	private bool isPaintingShown = false;
	public Blur blur;

	void Start () 
	{
		cam = Camera.main.transform;
	}

	void Update () 
	{
		RaycastHit hit;
//		if(!isPaintingShown)
//		{
			if(Physics.Raycast (cam.position, cam.forward, out hit) && hit.collider.tag == "StreetArt" && prefab != null)
			{
	//			print (hit.collider);
				Vector3 direction = hit.collider.transform.position - cam.position ;
				prefab.position = hit.collider.transform.position + (hit.normal * 0.1F) ;
				prefab.rotation = hit.collider.transform.rotation;
				prefab.gameObject.SetActive(true);
				iconLoading.fillAmount +=  Time.deltaTime;
				if(iconLoading.fillAmount == 1)
				{
//					loaded = true;
					isPaintingShown = true;
					ShowPaintingInfo(hit.collider.GetComponent<MeshRenderer>().material.mainTexture);
					
//					paintingToShow.material = hit.collider.GetComponent<MeshRenderer>().material;
				}
			}	
			else
			{
				if(isPaintingShown)
					RemovePaintingInfo();
				prefab.gameObject.SetActive(false);
				iconLoading.fillAmount = 0;
			}
//		}
	}

	void ShowPaintingInfo(Texture painting)
	{
//		blur.enabled = true;
		paintingToShow.enabled = true;
//		paintingToShow.sprite = Sprite.Create(painting,new Rect(),new Vector2(0.5f, 0.5f));
//		paintingToShow.material.mainTexture = painting;
		//Pause: Stop player Input
	}

	void RemovePaintingInfo()
	{
		blur.enabled = false;
		paintingToShow.enabled = false;
		//Disable Blur effect
		//Unpause Player
		isPaintingShown = false;
	}
}
