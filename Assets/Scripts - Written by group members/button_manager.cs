using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class button_manager : MonoBehaviour {

	public void HightLight(Button target){
		target.transform.GetChild(0).GetComponent<Text>().color = new Color(0.87f,0.87f,0.87f,1.0f);
	}

	public void noHightLight(Button target){
		target.transform.GetChild(0).GetComponent<Text>().color = new Color(1f,1,1,1f);
	}

	public void clickButton(Button btn){
		GameObject.Find("audio").GetComponent<AudioSource>().Play();
		StartCoroutine(moveScene(btn.tag, GameObject.Find("audio").GetComponent<AudioSource>().clip.length));
	}


	IEnumerator moveScene(string tag, float delay){
		yield return new WaitForSeconds(delay);
		if(tag == "startButton"){
			GlobalData.option = 0;
			SceneManager.LoadScene ("nickname");
		}else if(tag == "optionButton"){
			SceneManager.LoadScene("options");
		}else if(tag == "helpButton"){
			SceneManager.LoadScene ("help"); 
		}else if(tag == "lobbyButton"){
			SceneManager.LoadScene ("lobby");
		}else if(tag == "cancelButton"){
			SceneManager.LoadScene ("Main_menu");
		}else if(tag == "joinButton"){
			GameObject list = GameObject.Find("Canvas").transform.Find("gameList").gameObject;
			list.SetActive(true);
			GlobalData.option = 1;
		}else if(tag == "quitButton"){
			Utility.createPanel(GameObject.Find("Canvas").transform,new Color(0.5f,0.47f,0.47f,0.96f));
			createExitBlock(GameObject.Find("Canvas").transform);
		}
	}


	private void createExitBlock(Transform parent){
		GameObject m = new GameObject("exit");
		RectTransform trans = m.AddComponent<RectTransform>();
		trans.sizeDelta = new Vector2(500,220);
		trans.anchoredPosition3D = new Vector3(0, 0, 0);
		trans.anchoredPosition = new Vector2(0, 0);
		trans.anchorMax = new Vector2(0.5f,0.5f);
		trans.anchorMin = new Vector2(0.5f,0.5f);
		trans.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		m.AddComponent<CanvasRenderer>();
		Image img = m.AddComponent<Image>();
		img.sprite = (Sprite)Resources.Load("images/exitrect",typeof(Sprite));

		TextStruct txt = new TextStruct();
		txt.setupTextStruct("Are you sure you want to Quit?",30,new Color(1.0f,1.0f,1.0f,1.0f),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),TextAnchor.MiddleCenter,"Langdon");
		GameObject txt3 = Utility.initTextWithRelationship(txt,m.transform);
		txt3.GetComponent<RectTransform>().localPosition = new Vector3(0.0f,30f,0.0f);
		txt3.GetComponent<Text>().fontStyle = FontStyle.Bold;

		TextStruct txt1 = new TextStruct();
		txt1.setupTextStruct("Yes",22,new Color(1.0f,1.0f,1.0f,1.0f),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),TextAnchor.MiddleCenter);

		TextStruct txt4 = new TextStruct();
		txt4.setupTextStruct("No",22,new Color(1.0f,1.0f,1.0f,1.0f),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),TextAnchor.MiddleCenter);

		ButtonStruct bStruct = new ButtonStruct();
		bStruct.createButton(new Color(1.0f,1.0f,1.0f,0.0f));

		GameObject bt1 = Utility.createButton(bStruct,txt1,FontStyle.Bold);
		GameObject bt2 = Utility.createButton(bStruct,txt4,FontStyle.Bold);

		Button b = bt1.GetComponent<Button>();
		b.onClick.AddListener(delegate{
			Application.Quit();
		});

		Button b2 = bt2.GetComponent<Button>();
		b2.onClick.AddListener(delegate{
			Destroy(GameObject.Find("exit"));
			Destroy(GameObject.Find("panel"));
		});

		bt1.transform.SetParent(m.transform);
		bt1.GetComponent<RectTransform>().localPosition = new Vector3(-60,-24,0);
		bt2.transform.SetParent(m.transform);
		bt2.GetComponent<RectTransform>().localPosition = new Vector3(55,-24,0);

		m.transform.SetParent(parent);
		trans.localPosition = new Vector3 (0, 0, 0);
	}




}
