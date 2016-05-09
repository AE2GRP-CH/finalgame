using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameListItem:ListItem{

	private string ip;

	public string IP{
		get{
			return ip;
		}
		set{
			ip = value;
		}
	}

	void Start(){		
		Button btn = this.Button.GetComponent<Button>();
		btn.onClick.AddListener(delegate{
			GlobalData.connect_Ip = this.IP;
			SceneManager.LoadScene("nickname");
		});
	}

}