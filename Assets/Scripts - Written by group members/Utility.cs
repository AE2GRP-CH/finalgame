using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;


public struct TextStruct
{
   public string txt;
   public int size;
   public Color color;
   public Vector2 anchorMaximum;
   public Vector2 anchorMinimum;
   public string font;
   public TextAnchor align_ment;


   public void setupTextStruct(string content, int sz, Color clr, Vector2 max, Vector2 min,TextAnchor anchor,string fontname = null){
   		txt = content;
   		size = sz;
   		color = clr;
   		font = fontname;
   		anchorMaximum = max;
   		anchorMinimum = min;
   		align_ment = anchor;
   }
};

public struct ButtonStruct{
	public string imageName;
	public Color cl;

	public void createButton(Color c,string name = null){
		cl = c;
		imageName = name;
	}
}  

public static class Utility{

	public static GameObject createTextObject(TextStruct text){
	//public static GameObject createTextObject(string txt,int size,Color color,string font_name = null){

		GameObject obj = new GameObject();

		RectTransform trans = obj.AddComponent<RectTransform>();
		trans.sizeDelta.Set(100, 100);
		trans.anchoredPosition3D = new Vector3(0, 0, 0);
		trans.anchoredPosition = new Vector2(0, 0);
		trans.anchorMax = new Vector2(0,0);
		trans.anchorMax = text.anchorMaximum;
		trans.anchorMin = text.anchorMinimum;
		trans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		trans.position = new Vector3 (0, 0, 0);
		trans.localPosition = new Vector3(68.4f,24.3f,0.0f);
		obj.AddComponent<CanvasRenderer>();

		obj.layer = 5;
		Text text_component = obj.AddComponent<Text>();
		text_component.fontSize = text.size;
		text_component.text = text.txt;
		text_component.color = text.color;

		if(text.font == null){
			text_component.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		}else{
			text_component.font = (Font)Resources.Load("fonts/"+text.font,typeof(Font));
		}

		text_component.alignment = text.align_ment;
		text_component.horizontalOverflow = HorizontalWrapMode.Overflow;

		return obj;
	}

	public static GameObject initTextWithRelationship(TextStruct text, Transform parent){
		GameObject obj = Utility.createTextObject(text);
		obj.transform.SetParent(parent);
		return obj;
	}

	public static GameObject getChildWithTag(Transform parent,string child_tag){
		foreach(Transform child in parent){
			if(child.tag == child_tag){
				return child.gameObject;
			}
		}
		return null;
	}

	public static GameObject getChildWithName(Transform parent,string name){
		foreach(Transform child in parent){
			if(child.gameObject.name == name){
				return child.gameObject;
			}
		}
		return null;
	}

	public static void createPanel(Transform parent,Color clr){
		GameObject panel = new GameObject("panel");

		RectTransform trans = panel.AddComponent<RectTransform>();
		trans.sizeDelta.Set(100, 100);
		trans.anchoredPosition3D = new Vector3(0, 0, 0);
		trans.anchoredPosition = new Vector2(0, 0);
		trans.anchorMax = new Vector2(1,1);
		trans.anchorMin = new Vector2(0,0);
		trans.pivot = new Vector2(0.5f,0.5f);
		trans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		trans.position = new Vector3 (0, 0, 0);
		panel.AddComponent<CanvasRenderer>();

		Image img = panel.AddComponent<Image>();
		img.type = Image.Type.Sliced;
		img.color = clr;
	//new Color(0.5f,0.47f,0.47f,0.5f);
		panel.transform.SetParent(parent);
		trans.offsetMax = new Vector2(0.0f,0.0f);
		trans.offsetMin = new Vector2(0.0f,0.0f);
		trans.localPosition = new Vector3 (0, 0, 0);

	}

	public static GameObject createCanvas(){

			GameObject canvas = new GameObject("canvas");

			Canvas canvas_component = canvas.AddComponent<Canvas>();
			canvas_component.renderMode = RenderMode.ScreenSpaceOverlay;

			CanvasScaler scaller = canvas.AddComponent<CanvasScaler>();
			scaller.scaleFactor = 1;
			scaller.dynamicPixelsPerUnit = 10f;

			canvas.AddComponent<GraphicRaycaster>();
			return canvas;
	}

	public static List<GameObject> removeDuplicate(Collider[] c){

		List<GameObject> unique = new List<GameObject>();
		if(c.Length == 1){
			unique.Add(c[0].gameObject);
		}else{
			for(int j = 0; j < c.Length - 1; j++){
				Vector3 pos = c[j].gameObject.transform.position;
				unique.Add(c[j].gameObject);
				for(int i = j+1; i < c.Length ;i++){
					Vector3 pos1 = c[i].gameObject.transform.position;
					if(pos.x != pos1.x || pos.y != pos1.y || pos.z != pos1.z){
						unique.Add(c[i].gameObject);
					}
				}
			}
		}
		
		return unique;
	}

	public static GameObject createButton(ButtonStruct bStruct,TextStruct tStruct, FontStyle style){

		GameObject buttonObject = new GameObject("Button");
		buttonObject.layer = 5;

		// Add rect transform

		RectTransform trans = buttonObject.AddComponent<RectTransform>();
		trans.anchoredPosition3D = new Vector3(0, 0, 0);
		trans.anchoredPosition = new Vector2(0, 0);
		trans.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		// Add canvas rendered
		buttonObject.AddComponent<CanvasRenderer>();

		Image image = buttonObject.AddComponent<Image>();
		// Add image component
		if(bStruct.imageName != null){
			Texture2D tex = Resources.Load ("images/"+bStruct.imageName, typeof(Texture2D)) as Texture2D;
			image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
		}else{
			image.color = bStruct.cl;
		}
		//Add Button Text
		GameObject obj = initTextWithRelationship(tStruct,buttonObject.transform);
		obj.GetComponent<Text>().fontStyle = style;
		obj.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);

		//Add Button Component
		Button button = buttonObject.AddComponent<Button>();
		button.interactable = true;

		return buttonObject;
	}

	public static GameObject loadImage(string name,Transform parent = null){
			GameObject img = new GameObject("image");

			Image img1 = img.AddComponent<Image>();
			Texture2D tex = Resources.Load ("images/"+name, typeof(Texture2D)) as Texture2D;
			img1.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
			if(parent != null){
				img.transform.SetParent(parent);
			}
			return img;
	}






}