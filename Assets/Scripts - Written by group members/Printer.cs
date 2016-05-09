using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class Printer : NetworkBehaviour{

	private const string pacman_tag = "pacman";
	private const string ghost_tag = "enemy";
	GameObject canvas;
	private int numOfFruits = 0;

	void Start(){
		struct_Time tm = new struct_Time();
		tm.initTime(2,00);
		canvas = Utility.createCanvas();
		PrintTime(tm);
		if(isServer){
			PrintScore("0");
			numOfFruits = GameObject.FindGameObjectsWithTag("bean").Length;
		}
	}

	void PrintScore(string scr){

		GameObject s = GameObject.FindWithTag("score");
		if(s == null){
			TextStruct txt_struct = new TextStruct();
			txt_struct.setupTextStruct("Score: " + scr, 40,new Color(0.9F,0.46F,0.1F,1.0F),new Vector2(0,0),new Vector2(0,0),TextAnchor.MiddleLeft,"Shumi");

			GameObject text = Utility.initTextWithRelationship(txt_struct,canvas.transform);
			text.tag = "score";
			Outline out1 = text.AddComponent<Outline>();
			out1.effectColor = new Color(0.4F,0.11F,0.13F,1.0F);
			out1.effectDistance = new Vector2(3.0f,2.0f);
		}else{
			Text txt = s.GetComponent<Text>();
			string[] words = txt.text.Split(' ');
			txt.text = words[0] + " " + scr;

		}
	}

	// For debugging purposes
	void printName(string x){
		if(canvas == null){
			canvas = Utility.createCanvas();
		}

		if(GameObject.FindWithTag("asdqwe") != null){
			Text txt = GameObject.FindWithTag("asdqwe").GetComponent<Text>();
			txt.text = txt.text +";"+ "\n" + x;
		}else{
			TextStruct txt_struct = new TextStruct();
			txt_struct.setupTextStruct(x, 30,new Color(0.9F,0.46F,0.1F,1.0F),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),TextAnchor.MiddleCenter,"Shumi");
			GameObject text_to_display = Utility.initTextWithRelationship(txt_struct,canvas.transform);
			text_to_display.tag = "asdqwe";
			text_to_display.GetComponent<RectTransform>().sizeDelta = new Vector2(100,800);
			text_to_display.GetComponent<RectTransform>().localPosition = new Vector3(0f,0f,0f);
		}
	}

	void clearCanvasElement(){
		foreach(Transform child in canvas.transform){
			child.gameObject.SetActive(false);
		}
	}


	void printCongrats(string text){
		TextStruct txt_struct = new TextStruct();
		txt_struct.setupTextStruct(text, 35,new Color(1.0F,1.0F,1.0F,1.0F),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),TextAnchor.MiddleCenter,"Moon");
		GameObject text_to_display = Utility.initTextWithRelationship(txt_struct,canvas.transform);
		text_to_display.GetComponent<Text>().fontStyle = FontStyle.Bold;
		text_to_display.transform.SetAsLastSibling();
		text_to_display.GetComponent<RectTransform>().localPosition = new Vector3(0f,60.0f,0f);
	}

	void PrintStateToGui(string text){

		if(canvas == null){
			canvas = Utility.createCanvas();
		}

		Utility.createPanel(canvas.transform,new Color(0.5f,0.47f,0.47f,0.7f));

		if(text.Split(' ')[1] == "Wins!!"){
			printCongrats("Congratulations");
		}

		TextStruct txt_struct = new TextStruct();
		txt_struct.setupTextStruct(text, 70,new Color(0.9F,0.46F,0.1F,1.0F),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),TextAnchor.MiddleCenter,"Shumi");
		GameObject text_to_display = Utility.initTextWithRelationship(txt_struct,canvas.transform);
		text_to_display.transform.SetAsLastSibling();
		text_to_display.GetComponent<RectTransform>().localPosition = new Vector3(0f,0f,0f);
	}

	void PrintBullet(string num){

		GameObject s = GameObject.FindWithTag("bullet");
		if(s == null){
			TextStruct txt_struct = new TextStruct();
			txt_struct.setupTextStruct("Bullet: " + "0"+num, 40,new Color(0.58F,0.82F,0.89F,1.0F),new Vector2(0,0),new Vector2(0,0),TextAnchor.MiddleLeft,"Shumi");

			GameObject text = Utility.initTextWithRelationship(txt_struct,canvas.transform);
			text.tag = "bullet";
			text.GetComponent<RectTransform>().anchoredPosition = new Vector3(68.0f,70.0f,0);
			Outline out1 = text.AddComponent<Outline>();
			out1.effectColor = new Color(0.04F,0.42F,0.55F,1.0F);
			out1.effectDistance = new Vector2(3.0f,2.0f);
		}else{
			Text txt = s.GetComponent<Text>();
			string[] words = txt.text.Split(' ');
			txt.text = words[0] + " " + "0" + num;

		}
	}

	void addBombImage(){

		GameObject img = Utility.loadImage("bomb_small");
		img.tag = "bombImage";
		img.SetActive(false);
		img.transform.SetParent(canvas.transform);
		RectTransform trans = img.GetComponent<RectTransform>();
		trans.localPosition = new Vector3(0.0f,0.0f,0.0f);
		trans.sizeDelta = new Vector2(70,70);
		Animator anim = img.AddComponent<Animator>();
		RuntimeAnimatorController zxc = (RuntimeAnimatorController)Resources.Load("Image1",typeof(RuntimeAnimatorController));
		anim.runtimeAnimatorController = zxc;
		img.SetActive(true);
		anim.SetBool("eatBomb",true);
	}

	void removeBombImage(){
		GameObject b = GameObject.FindWithTag("bombImage");
		if(b != null){
			Destroy(b);
		}
	}

	void DisableMovement(string tag_name){
		GameObject[] objs = GameObject.FindGameObjectsWithTag(tag_name);
		foreach(GameObject obj in objs){
			obj.GetComponent<CharacterController>().enabled = false;
		}
	}

	void PrintTime(struct_Time time){

		string seconds = time.sec / 10 < 1 ? "0"+time.sec.ToString() : time.sec.ToString();
		string time1 = time.minute.ToString()+":"+ seconds.ToString();
		TextStruct txt_struct = new TextStruct();
		txt_struct.setupTextStruct(time1, 40,new Color(0.9F,0.46F,0.1F,1.0F),new Vector2(0.5f,0),new Vector2(0.5f,0),TextAnchor.MiddleLeft,"Shumi");
		//GameObject canvas = GameObject.Find("canvas");

		GameObject txt_timer = GameObject.FindWithTag("time");
		if(txt_timer != null) { 
			Destroy(txt_timer);
		}
	
		GameObject text_component = Utility.createTextObject(txt_struct);
		text_component.tag = "time";
		text_component.transform.SetParent(canvas.transform);
		text_component.GetComponent<RectTransform>().position = new Vector3(500.0f,25.0f,0.0f);
	}
}
