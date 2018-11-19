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
	private Button itemsButton;
	private Button monsterButton;

	private string [] descriptionsItems = {
		"Collect it, spend it. Or swim in it.",
		"Fighting monsters can be taxing so grab these to improve your health. Read prescription label for possible side effects.",
		"Can contain anything from gold to health to monsters.",
		"Bombs will go off after a contdown period. They can also be picked up and thrown. Cool!",
		"Why would you want to drink one of those!?!. It'll hurt you over time. A health potion will counteract it's effect.",
		"Could be health or poison. Only one way to find out I guess...",
		"Oh, how pretty they are. You can also buy all kind of temporary upgrades with these.",
		"You need one of those if the exit is locked."
		};
	private string [] descriptionsMonsters = {
		"Be careful. This is a genuine attack chicken.",
		"Sheeps are nice and fluffy. Do not attack with a weapon.",
		"Toad is just minding it's own business.",
		"You know what rabbits are known for, right?",
		"Spiders can be poisonous if tackled bare handed.",
		"Wasps might sting you for no other reason than you passing close.",
		"Zombies have a mean left! Don't come near.",
		"Fire spreads to everything flammable.",
		"It looks like it's hatching.",
		"Small trees grow into bigger trees",
		"Bigger trees grow into really big trees",
		"It's a really big tree",
		"So, what does come out of the egg?",
		"Unicorns spreads rainbows of love. And love can really hurt.",
		"Vampires suck! Especially if you get close.",
		"The necromancer hates the living but loves the dead. So he creates some more.",
		"Death sucks the life out of everything."
		};

	// Use this for initialization
	void Start () {
		this.index = 0;

		Button prevChar = bPrev.GetComponent<Button>();
		prevChar.onClick.AddListener(delegate {selectItem(-1); });
		Button nextChar = bNext.GetComponent<Button>();
		nextChar.onClick.AddListener(delegate {selectItem(1); });
		this.description = descriptionText.GetComponent<Text>();

		this.itemsButton = bItems.GetComponent<Button>();
		this.itemsButton.onClick.AddListener(delegate {toggleSelection(); });
		this.monsterButton = bMonsters.GetComponent<Button>();
		this.monsterButton.onClick.AddListener(delegate {toggleSelection(); });
		this.itemsSelected = true;
		this.itemsButton.interactable = false;

		this.instantiatedItems = new GameObject [items.Length];
		this.instantiatedMonsters = new GameObject [monsters.Length];
		instanciateItem(items);
		instanciateMonsters(monsters);
		updateSelectedItem();
	}
	private void instanciateItem(GameObject [] characters) {
		Vector3 down = new Vector3(0, 0.4f, 0);
		for (int i=0; i<characters.Length; i++){
			instantiatedItems[i] = Instantiate(characters[i], this.gameObject.transform.localPosition - down, Quaternion.identity);
			Item it = instantiatedItems[i].GetComponent<Item>();
			it.hideFromList = true;
			it.setShow(false, true);
			instantiatedItems[i].SetActive(false);
		}
	}
	private void instanciateMonsters(GameObject [] characters) {
		Vector3 down = new Vector3(0, 0.4f, 0);
		for (int i=0; i<characters.Length; i++){
			this.instantiatedMonsters[i] = Instantiate(characters[i], this.gameObject.transform.localPosition - down, Quaternion.identity);
			Item it = this.instantiatedMonsters[i].GetComponent<Item>();
			it.hideFromList = true;
			it.setShow(false, true);
			this.instantiatedMonsters[i].SetActive(false);
		}
	}

	private void toggleSelection(){
		if(itemsSelected){
			this.itemsButton.interactable = true;
			this.monsterButton.interactable = false;
		} else {
			this.itemsButton.interactable = false;
			this.monsterButton.interactable = true;
		}
		this.itemsSelected = !this.itemsSelected;
		this.index = 0;
		selectItem(0);
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
