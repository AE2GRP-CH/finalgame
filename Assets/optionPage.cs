using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class optionPage : MonoBehaviour {

	public Slider slideSFX;
	public Slider slideMusic;

	// Use this for initialization
	void Start () {
		slideSFX.value = 1.0f;
		slideMusic.value = 1.0f;

		slideSFX.onValueChanged.AddListener(delegate{

		});

		slideMusic.onValueChanged.AddListener(delegate{

		});
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
