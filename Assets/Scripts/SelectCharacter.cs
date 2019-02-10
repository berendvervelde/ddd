using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectCharacter : MonoBehaviour {

	public Button bPrev, bNext, bBack;
	public Text descriptionText, propertiesText;
	public GameObject [] characters;
	private GameObject [] instantiatedCharacters;
	private GameObject selectedCharacter;
	private int index = 0;
	private Text description, properties;

	private string [] descriptions = {
		"The knight slays dragons, rescues damsels and find grails (preferably holy). He has a special attack when he is surrounded.",
		"The ranger likes trees more than people. Thus he uses arrows made from the former to kill the latter. If the rangers aim is true, he can shoot two monsters in a row.",
		"The rogue is a shady character who prefers to attack monsters from the back. That's the kind of character she is.",
		"The dwarf is a jolly character. After a number of kills she drinks on her enemies (lack of) health.",
		"The wizard usually comes in grey or white. Due to lifelong study, he can shoot two monsters in a row. None shall pass this wizard!"
		};

	// Use this for initialization
	void Start () {
		this.index = GameManager.instance.dataController.selectedCharacter;

		Button prevChar = bPrev.GetComponent<Button>();
		prevChar.onClick.AddListener(delegate {selectCharacter(-1); });
		Button nextChar = bNext.GetComponent<Button>();
		nextChar.onClick.AddListener(delegate {selectCharacter(1); });
		this.description = this.descriptionText.GetComponent<Text>();
		this.properties = this.propertiesText.GetComponent<Text>();

		this.instantiatedCharacters = new GameObject [characters.Length];
		instanciateCharacters(characters);
		updateSelectedCharacter();

		Button backButton = bBack.GetComponent<Button>();
		backButton.onClick.AddListener(delegate {loadSceneOnClick(); });
	}

	private void loadSceneOnClick(){
		// the selectCharaterscene should normally return to the introscene unless it is used from the IAP scene.
		int sceneIndex = GameManager.instance.characterChoiceReturnSceneIndex;
		GameManager.instance.characterChoiceReturnSceneIndex = 0;
		SceneManager.LoadScene(sceneIndex);
	}
	private void instanciateCharacters(GameObject [] characters) {
		for (int i=0; i<characters.Length; i++){
			instantiatedCharacters[i] = Instantiate(characters[i], this.gameObject.transform.localPosition, Quaternion.identity);
			instantiatedCharacters[i].SetActive(false);
		}
	}

	private void selectCharacter(int direction) {
		this.index += direction;
		if(this.index < 0){
			this.index = characters.Length -1;
		} else if (this.index > characters.Length -1 ){
			this.index = 0;
		}
		GameManager.instance.dataController.setSelectedCharacter(this.index);
		updateSelectedCharacter();

		// remove the continue option
		GameManager.instance.continueAvailable = false;
		GameManager.instance.deleteGameState();
	}

	private void updateSelectedCharacter(){
		this.description.text = descriptions[this.index];
		if (selectedCharacter != null){
			selectedCharacter.SetActive(false);
		}
		selectedCharacter = instantiatedCharacters[index];
		selectedCharacter.SetActive(true);

		int health = 0;
		int strength = 0;
		switch (GameManager.instance.dataController.selectedCharacter){
			case GameManager.character_knight:
				health = GameManager.instance.permanentData.knightBaseHealth;
				strength = GameManager.instance.permanentData.knightStrength;
			break;
			case GameManager.character_ranger:
				health = GameManager.instance.permanentData.rangerBaseHealth;
				strength = GameManager.instance.permanentData.rangerStrength;
			break;
			case GameManager.character_dwarf:
				health = GameManager.instance.permanentData.dwarfBaseHealth;
				strength = GameManager.instance.permanentData.dwarfStrength;
			break;
			case GameManager.character_rogue:
				health = GameManager.instance.permanentData.rogueBaseHealth;
				strength = GameManager.instance.permanentData.rogueStrength;
			break;
			case GameManager.character_wizard:
				health = GameManager.instance.permanentData.wizardBaseHealth;
				strength = GameManager.instance.permanentData.wizardStrength;
			break;
		}
		this.properties.text = "Current health: " + health + ", current strength: " + strength;
	}
}
