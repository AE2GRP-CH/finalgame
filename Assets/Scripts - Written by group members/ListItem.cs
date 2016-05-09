using UnityEngine;
using UnityEngine.UI;

public class ListItem:MonoBehaviour{

	private GameObject btn;
	private GameObject item_str;

	public GameObject Button{
		get{
			return btn;
		}
		set{
			btn = value;
		}
	}

	public GameObject Text{
		get{
			return item_str;
		}
		set{
			item_str = value;
		}
	} 

}