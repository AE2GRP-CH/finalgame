using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HelpScene : MonoBehaviour {

	public GameObject playerBlock;
	public GameObject itemBlock;
	public GameObject prevButton;
	public GameObject nextButton;
	private GameObject[] blocks = new GameObject[2];
	int start = 0;
	private GameObject activeBlock;
	float mvLeftDuration = 0;
	float moveRightFromActiveDuration = 0;

	void Start(){
		prevButton.SetActive(false);
		blocks[0] = playerBlock;
		blocks[1] = itemBlock;
		AnimationClip h = (AnimationClip)Resources.Load("animation/FlyLeft",typeof(AnimationClip));
		mvLeftDuration = h.length;

		AnimationClip h1 = (AnimationClip)Resources.Load("animation/moveRight",typeof(AnimationClip));
		moveRightFromActiveDuration = h1.length;

		activeBlock = blocks[start];
	}

	public void next(){

		if(++start > 0){
			prevButton.SetActive(true);
		}

		GameObject obj = activeBlock;
		Animator anim = obj.GetComponent<Animator>();
		if(anim.GetBool("moveHidden")){
			anim.SetBool("moveHidden",false);
		}

		if(anim.GetBool("moveRightFromHidden")){
			anim.SetBool("moveRightFromHidden",false);
		}

		anim.SetBool("moveLeft",true);
		Invoke("moveLeft",mvLeftDuration-0.2f);
	}

	private void moveLeft(){

		if(start == 1){
			nextButton.SetActive(false);
		}

		GameObject.Find("dotFirst").GetComponent<Image>().sprite = (Sprite)Resources.Load("images/dot_close",typeof(Sprite));
		GameObject.Find("dotSecond").GetComponent<Image>().sprite = (Sprite)Resources.Load("images/dot",typeof(Sprite));

		GameObject nextBlock = blocks[start];
		Animator anim = nextBlock.GetComponent<Animator>();		
		if(anim.GetBool("moveRightFromActive")){
			anim.SetBool("moveRightFromActive",false);
		}
		nextBlock.SetActive(true);
		anim.SetBool("moveHidden",true);
		activeBlock = nextBlock;
	}

	public void prev(){
		Animator anim = activeBlock.GetComponent<Animator>();
		anim.SetBool("moveRightFromActive",true);
		if(anim.GetBool("moveHidden")){
			anim.SetBool("moveHidden",false);
		}

		if(anim.GetBool("moveRightFromHidden")){
			anim.SetBool("moveRightFromHidden",false);
		}
		Invoke("moveRight",moveRightFromActiveDuration);
		start--;

	}

	private void moveRight(){

		if(start < 2){
			nextButton.SetActive(true);
		}
		if(start == 0){
			prevButton.SetActive(false);
		}

		GameObject.Find("dotFirst").GetComponent<Image>().sprite = (Sprite)Resources.Load("images/dot",typeof(Sprite));
		GameObject.Find("dotSecond").GetComponent<Image>().sprite = (Sprite)Resources.Load("images/dot_close",typeof(Sprite));

		GameObject hiddenBlock = blocks[start];
		Animator anim2 = hiddenBlock.GetComponent<Animator>();
		anim2.SetBool("moveRightFromHidden",true);
		if(anim2.GetBool("moveLeft")){
			anim2.SetBool("moveLeft",false);
		}
		activeBlock = hiddenBlock;
	}

	public void backMainMenu(){
		SceneManager.LoadScene("Main_menu");
	}
}
