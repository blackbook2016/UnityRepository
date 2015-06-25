using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class Proto_Player : MonoBehaviour {


	public Transform uiLoading;
	public Image iconLoading;
	public Image paintingToShow;	
	public Blur blur;

	private bool isPaintingShown = false;
	private bool isLoadingPainting = false;
	private Transform cam;

	void Awake()
	{		
		cam = Camera.main.transform;
	}
	void Start () 
	{
		RemovePaintingInfo();
	}

	void Update () 
	{
		RaycastHit hit;
		if(Physics.Raycast (cam.position, cam.forward, out hit) && hit.collider.tag == "StreetArt" && uiLoading != null)
		{
			if(!isPaintingShown)
			{
				uiLoading.position = hit.collider.transform.position + (hit.normal * 0.1F) ;
				uiLoading.rotation = hit.collider.transform.rotation;
				uiLoading.gameObject.SetActive(true);

				iconLoading.fillAmount +=  Time.deltaTime;
				isLoadingPainting = true;
			}		
			if(iconLoading.fillAmount == 1 || isPaintingShown)
			{
				isPaintingShown = true;
				ShowPaintingInfo(hit.collider.GetComponent<MeshRenderer>().material.mainTexture);
			}
		}	
		else
		{
			if(isPaintingShown || isLoadingPainting)
				RemovePaintingInfo();
		}

		if(Input.GetButton("Cancel"))
			RemovePaintingInfo();

	}

	void ShowPaintingInfo(Texture painting)
	{
		blur.enabled = true;
		paintingToShow.enabled = true;
	}

	void RemovePaintingInfo()
	{
		blur.enabled = false;
		paintingToShow.enabled = false;
		iconLoading.fillAmount = 0;
		
		uiLoading.gameObject.SetActive(false);

		isLoadingPainting = false;
		isPaintingShown = false;
	}
}
