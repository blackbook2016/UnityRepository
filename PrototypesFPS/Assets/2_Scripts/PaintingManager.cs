namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class PaintingManager : MonoBehaviour 
	{
		private static PaintingManager _instance;
		public static PaintingManager instance
		{
			get
			{
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<PaintingManager>();
				return _instance;
			}
		}

		public List<Sprite> paintingsList = new List<Sprite>();

		private TextAsset paintingInfo;

		void Start() 
		{
			paintingInfo = Resources.Load("PaintingInfo",typeof(TextAsset))as TextAsset;
		}

		public Sprite PaintingSprite(string paintingName)
		{
			return paintingsList.Find(x => x.name.Equals(paintingName +"_Sprite"));
		}
	}
}
