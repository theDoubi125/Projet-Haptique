using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BrushUI : MonoBehaviour {

	public GameObject cursor;	
	[HideInInspector] private RaycastCursor rayCastScript;

	public Text brushSizeText;	
	public Slider brushSizeSlider;	

	public Text toolTypeText;	
	public Button paintButton;	
	public Button eraseButton;	

	public Text brushShapeText;	
	public Slider brushShapeSlider;

	public bool isOpen = true;

	public Animator animator;

	public ColorPicker colorPicker;	

	// Use this for initialization
	void Awake () {

		rayCastScript = (RaycastCursor) cursor.GetComponent(typeof(RaycastCursor));
		SetBrushSize (5);
		SetOutil (1);
		SetBrushShape (0);
		animator.SetBool ("isOpen", true);

		OnSetColor(rayCastScript.currentColor);
		/*if (rayCastScript && colorPicker is ColorPicker)
			((ColorPicker)colorPicker).NotifyColor (rayCastScript.currentColor);*/
	}

	public void SetBrushSize(float brushSize)
	{
		int finalBrushSize = (int)brushSize;
		if(rayCastScript)
			rayCastScript.brushSize = finalBrushSize;
		cursor.transform.localScale = new Vector3(finalBrushSize,finalBrushSize,finalBrushSize);
		brushSizeText.text = "Taille : " + finalBrushSize;
		brushSizeSlider.value = finalBrushSize;

	}
	public void ChangeOutil()
	{
		if (rayCastScript) {
			if (rayCastScript.currentVoxelValue == 1)
				SetOutil (0);
			else
				SetOutil (1);
		}
	}

	public void ToogleInterface()
	{
		if (isOpen) {
			animator.SetBool ("isOpen", false);
			isOpen = false;
			//colorPicker.enabled = false;
		} else {
			animator.SetBool ("isOpen", true);
			isOpen = true;
			//colorPicker.enabled = true;
		}
	}


	public void SetOutil(int outil)
	{
		if (rayCastScript)
		{
			rayCastScript.currentVoxelValue = outil;

			if (rayCastScript.currentVoxelValue == 0) {
				toolTypeText.text = "Outil : Gomme";
				eraseButton.interactable = false;
				paintButton.interactable = true;
			} else {
				toolTypeText.text = "Outil : Pinceau";
				paintButton.interactable = false;
				eraseButton.interactable = true;
			}
		}
	}

	public void SetBrushShape(float brushShape)
	{
		int finalBrushShape = (int)brushShape;
		if(rayCastScript)
			rayCastScript.brushIndex = finalBrushShape;
		brushShapeText.text = "Forme : " + GetBrushNameByIndex(finalBrushShape);
		brushShapeSlider.value = finalBrushShape;

	}
		
	void OnSetColor(Color color)
	{
		if(rayCastScript)
			rayCastScript.OnSetColor(color);

		cursor.GetComponent<Renderer> ().material.color = color;
	}

	void OnGetColor(ColorPicker picker)
	{
		if(rayCastScript)
			picker.NotifyColor(rayCastScript.currentColor);
	}

	private string GetBrushNameByIndex(int brushShape)
	{
		switch (brushShape) {
		case 0:
			return "Cubique";
		case 1:
			return "Sphérique";
		case 2:
			return "Etoilïque";
		default:
			return "Inconnue";
		}
	}
}
