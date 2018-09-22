using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartGameOnClick : MonoBehaviour {

	public void LoadSceneByIndex(int index){
		// remove the continue option
		GameManager.instance.continueAvailable = false;
		GameManager.instance.deleteGameState();
		
		SceneManager.LoadScene(index);
	}


}
