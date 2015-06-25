using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class Proto_Player : MonoBehaviour {


	public Transform uiLoading;
	public Image iconLoading;
	public Image paintingToShow;	
	public Blur blur;

	public float rayDistance = 10;
	public int blurMax = 3;

	private bool isPaintingShown = false;
	private bool isLoadingPainting = false;
	private Transform cam;

	void Awake()
	{		
		cam = Camera.main.transform;
	}
	void Start () 
	{
		blurMax = blur.iterations;
		RemovePaintingInfo();
	}

	void Update () 
	{
		if(Input.GetButton("joystickB"))
		{
			RaycastHit hit;
			if(Physics.Raycast (cam.position, cam.forward, out hit, rayDistance) && hit.collider.tag == "StreetArt" && uiLoading != null)
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
		}
		else
		{
			if(isPaintingShown || isLoadingPainting)
				RemovePaintingInfo();
		}
		
	}
	
	void ShowPaintingInfo(Texture painting)
	{
		blur.enabled = true;
		paintingToShow.enabled = true;
		paintingToShow.CrossFadeAlpha(Time.deltaTime * 355.0F,5.0f,true);
		if(blur.iterations < blurMax)
			blur.iterations += (int)(Time.deltaTime );
	}

	void RemovePaintingInfo()
	{
		blur.enabled = false;
		paintingToShow.enabled = false;	
		paintingToShow.CrossFadeAlpha(0F,2.0f,true);
		iconLoading.fillAmount = 0;	
		blur.iterations = 0;

		uiLoading.gameObject.SetActive(false);

		isLoadingPainting = false;
		isPaintingShown = false;
	}
}
