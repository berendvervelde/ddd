using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCharacterImage : MonoBehaviour {

	public Sprite [] characters;
	// Use this for initialization
	void Start () {
		int sc = GameManager.instance.dataController.selectedCharacter;
		Button button = GetComponent<Button>();
		button.GetComponent<Image>().sprite = characters[sc];
	}
}
