using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lexicon : MonoBehaviour {

	public Button bPrev, bNext, bItems, bMonsters;
	public GameObject [] items;
	public GameObject [] monsters;
	public Text descriptionText;
	private GameObject [] instantiatedItems;
	private GameObject [] instantiatedMonsters;
	private GameObject selectedItem;
	private int index = 0;
	private Text description;
	private bool itemsSelected = true;

	private string [] descriptionsItems = {
		"Collect it, spend it. Or swim in it.",
		"Fighting monsters can be taxing so grab these to improve your health. Read prescription label for possible side effects.",
		"Can contain anything from gold to health to monsters.",
		"Bombs will go off after a contdown period. They can also be picked up and thrown. Cool!",
		"Why would you want to drink one of those!?!. It'll hurt you over time. A health potion will counteract it's effect.",
		"Could be health or poison. Only one way to find out I guess..."
		};
	private string [] descriptionsMonsters = {
		"Collect it, spend it. Or swim in it.",
		"Fighting monsters can be taxing so grab these to improve your health. Read prescription label for possible side effects.",
		"Can contain anything from gold to health to monsters.",
		"Bombs will go off after a contdown period. They can also be picked up and thrown. Cool!",
		"Why would you want to drink one of those!?!. It'll hurt you over time. A health potion will counteract it's effect.",
		"Could be health or poison. Only one way to find out I guess..."
		};

	// Use this for initialization
	void Start () {
		this.index = 0;

		Button prevChar = bPrev.GetComponent<Button>();
		prevChar.onClick.AddListener(delegate {selectItem(-1); });
		Button nextChar = bNext.GetComponent<Button>();
		nextChar.onClick.AddListener(delegate {selectItem(1); });
		this.description = descriptionText.GetComponent<Text>();

		this.instantiatedItems = new GameObject [items.Length];
		this.instantiatedMonsters = new GameObject [monsters.Length];
		instanciateItem(items);
		updateSelectedItem();
	}
	private void instanciateItem(GameObject [] characters) {
		for (int i=0; i<characters.Length; i++){
			instantiatedItems[i] = Instantiate(characters[i], this.gameObject.transform.localPosition, Quaternion.identity);
			Item it = instantiatedItems[i].GetComponent<Item>();
			it.hideFromList = true;
			it.setShow(true);
			instantiatedItems[i].SetActive(false);
		}
	}

	private void selectItem(int direction) {
		this.index += direction;
		int length = this.itemsSelected ? items.Length : monsters.Length;

		if(this.index < 0){
			this.index = length - 1;
		} else if (this.index > length -1 ){
			this.index = 0;
		}
		updateSelectedItem();
	}

	private void updateSelectedItem(){
		this.description.text = this.itemsSelected ? descriptionsItems[this.index] : descriptionsMonsters[this.index];
		if (selectedItem != null){
			selectedItem.SetActive(false);
		}
		selectedItem = this.itemsSelected ? instantiatedItems[index] : instantiatedMonsters[index];
		selectedItem.SetActive(true);
	}
}
