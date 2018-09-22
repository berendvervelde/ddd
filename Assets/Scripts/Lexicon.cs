using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lexicon : MonoBehaviour {

	public Button bPrev, bNext;
	public GameObject [] items;
	private GameObject [] instantiatedItems;
	private GameObject selectedItem;
	private int index = 0;
	private Text description;

	private string [] descriptions = {
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
		this.description = findTextComponent(this.gameObject);

		this.instantiatedItems = new GameObject [items.Length];
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

	private Text findTextComponent(GameObject go){
		// find the right child by finding its tag
		for (int i = 0; i < go.transform.childCount; i++) {
			if(go.transform.GetChild (i).gameObject.tag == "HeldCanvas"){
				return go.transform.GetChild (i).gameObject.GetComponent<Text>();
			}
		}
        return null;
	}

	private void selectItem(int direction) {
		this.index += direction;
		if(this.index < 0){
			this.index = items.Length -1;
		} else if (this.index > items.Length -1 ){
			this.index = 0;
		}
		updateSelectedItem();
	}

	private void updateSelectedItem(){
		this.description.text = descriptions[this.index];
		if (selectedItem != null){
			selectedItem.SetActive(false);
		}
		selectedItem = instantiatedItems[index];
		selectedItem.SetActive(true);
	}
}
