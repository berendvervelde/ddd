using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueGameOnClick : MonoBehaviour {

	[HideInInspector] public Button cb;

	void Start () {
		this.cb  = this.GetComponent<Button>();
		CanvasGroup cg = this.GetComponent<CanvasGroup>();
		cg.alpha = 0;
		StartCoroutine(WaitForIt());
	}
	
	private void continueGame(){
		// start game
		GameManager.instance.restoreGame();
		SceneManager.LoadScene(1);
	}

	IEnumerator WaitForIt() {
		// this works, but instead this script should declare itself to the GameManager and after the gameprogress has been loaded
		// start this coroutine
        yield return new WaitForSeconds(0.2f);
        if (GameManager.instance.continueAvailable) {
			CanvasGroup cg = this.GetComponent<CanvasGroup>();
			while (cg.alpha < 1f) {
				cg.alpha = cg.alpha + (Time.deltaTime);
				yield return null;
			}
			this.cb.onClick.AddListener(delegate {continueGame(); });
		} else {
			this.gameObject.SetActive(false);
		}
    }
}
