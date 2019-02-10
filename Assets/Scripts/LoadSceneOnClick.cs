using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSceneOnClick : MonoBehaviour {

	public void LoadSceneByIndex(int index){
		//reset the characterChoiceReturnSceneIndex to the default value (introscene)
		GameManager.instance.characterChoiceReturnSceneIndex = 0;
		SceneManager.LoadScene(index);
	}


}
