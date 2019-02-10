using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;

public class IAP : MonoBehaviour {

	private const int 
		type_health = 0,
		type_strength = 1;

	public Text goldAmount;
	private Text goldAmountText;
	public Button purchaseButton;
	private Button bPurchase;
	public Button cancelButton;
	public Button characterChoiceButton;
	public Text currentPropertiesText;
	private Text tCurrentProperties;
	private Button bCharacterChoice;
	public Toggle extraHealth;
	private Toggle tExtraHealth;
	public Text extraHealthCost;
	private Text tExtraHealthCost;
	public Toggle extraStrength;
	private Toggle tExtraStrength;
	public Text extraStrengthCost;
	private Text tExtraStrengthCost;
	private int newGold;
	private int healthCost;
	private int strengthCost;
	void Start () {
		this.bPurchase = this.purchaseButton.GetComponent<Button>();
		this.bPurchase.onClick.AddListener(delegate {purchase();});
		this.bPurchase.interactable = false;
		Button cancel = this.cancelButton.GetComponent<Button>();
		cancel.onClick.AddListener(delegate {LoadSceneByIndex(0, -1); });

		this.bCharacterChoice = this.characterChoiceButton.GetComponent<Button>();
		this.bCharacterChoice.onClick.AddListener(delegate {LoadSceneByIndex(2, 6); });

		this.tExtraHealth = this.extraHealth.GetComponent<Toggle>();
		this.tExtraHealth.onValueChanged.AddListener(delegate { updateUpgrades(IAP.type_health);});
		this.tExtraHealthCost = this.extraHealthCost.GetComponent<Text>();
		this.tExtraStrength = this.extraStrength.GetComponent<Toggle>();
		this.tExtraStrength.onValueChanged.AddListener(delegate { updateUpgrades(IAP.type_strength);});
		this.tExtraStrengthCost = this.extraStrengthCost.GetComponent<Text>();
		this.tCurrentProperties = this.currentPropertiesText.GetComponent<Text>();

		this.goldAmountText = this.goldAmount.GetComponent<Text>();
		this.newGold = GameManager.instance.permanentData.gold;
		SetCurrentProperties();
		setGoldAmountText(true);
		setUpgradeCost();
		setupUpgrades();
	}
	private void SetCurrentProperties(){
		int strength = 0;
		int health = 0;
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
		this.tCurrentProperties.text = "Current health: " + health + "\nCurrent strength: " + strength;

	}
	private void setupUpgrades(){
		if (this.healthCost > this.newGold && !this.tExtraHealth.isOn){
			this.tExtraHealth.interactable = false;
		} else {
			this.tExtraHealth.interactable = true;
		}
		if (this.strengthCost > this.newGold && !this.tExtraStrength.isOn){
			this.tExtraStrength.interactable = false;
		} else {
			this.tExtraStrength.interactable = true;
		}
	}
	private void setUpgradeCost(){

		this.healthCost = 1000;
		this.strengthCost = 1000;

		const int healthAdder = 5000,
		strengthAdder = 5000;

		const int defaultKnighHealth = 11,
			defaultDwarfHealth = 10,
			defaultrangerHealth = 10,
			defaultWizardHealth = 9,
			defaultRogueHealth = 9,
			defaultKnightStrength = 2,
			defaultRangerStrength = 1,
			defaultWizardStrength = 2,
			defaultRogueStrength = 1,
			defaultDwarfStrength = 1;

		switch (GameManager.instance.dataController.selectedCharacter){
			case GameManager.character_knight:
				this.healthCost += (GameManager.instance.permanentData.knightBaseHealth - defaultKnighHealth) * healthAdder;
				this.strengthCost += (GameManager.instance.permanentData.knightStrength - defaultKnightStrength) * strengthAdder;
			break;
			case GameManager.character_ranger:
				this.healthCost += (GameManager.instance.permanentData.rangerBaseHealth - defaultrangerHealth) * healthAdder;
				this.strengthCost += (GameManager.instance.permanentData.rangerStrength - defaultRangerStrength) * strengthAdder;
			break;
			case GameManager.character_dwarf:
				this.healthCost += (GameManager.instance.permanentData.dwarfBaseHealth - defaultDwarfHealth) * healthAdder;
				this.strengthCost += (GameManager.instance.permanentData.dwarfStrength - defaultDwarfStrength) * strengthAdder;
			break;
			case GameManager.character_rogue:
				this.healthCost += (GameManager.instance.permanentData.rogueBaseHealth - defaultRogueHealth) * healthAdder;
				this.strengthCost += (GameManager.instance.permanentData.rogueStrength - defaultRogueStrength) * strengthAdder;
			break;
			case GameManager.character_wizard:
				this.healthCost += (GameManager.instance.permanentData.wizardBaseHealth - defaultWizardHealth) * healthAdder;
				this.strengthCost += (GameManager.instance.permanentData.wizardStrength - defaultWizardStrength) * strengthAdder;
			break;
		}
		this.tExtraHealthCost.text = this.healthCost.ToString("N0", CultureInfo.CurrentCulture) + " gold";
		this.tExtraStrengthCost.text = this.strengthCost.ToString("N0", CultureInfo.CurrentCulture) + " gold";
	}

	private void updateUpgrades(int type){
		switch (type){
			case IAP.type_health:
			if(this.tExtraHealth.isOn){
                this.newGold -= this.healthCost;

            } else {
                this.newGold += this.healthCost;
            }
			break;
			case IAP.type_strength:
			if(this.tExtraStrength.isOn){
                this.newGold -= this.healthCost;

            } else {
                this.newGold += this.healthCost;
            }
			break;
		}

		if (this.tExtraHealth.isOn || this.tExtraStrength.isOn){
			this.bPurchase.interactable = true;
		} else {
			this.bPurchase.interactable = false;
		}
		setGoldAmountText(false);
		setupUpgrades();
	}

	private void purchase(){
		switch (GameManager.instance.dataController.selectedCharacter){
			case GameManager.character_knight:
			if(this.tExtraHealth.isOn){
				GameManager.instance.permanentData.knightBaseHealth ++;
			}
			if(this.tExtraStrength.isOn){
				GameManager.instance.permanentData.knightStrength ++;
			}
			break;
			case GameManager.character_ranger:
			if(this.tExtraHealth.isOn){
				GameManager.instance.permanentData.rangerBaseHealth ++;
			}
			if(this.tExtraStrength.isOn){
				GameManager.instance.permanentData.rangerStrength ++;
			}
			break;
			case GameManager.character_dwarf:
			if(this.tExtraHealth.isOn){
				GameManager.instance.permanentData.dwarfBaseHealth ++;
			}
			if(this.tExtraStrength.isOn){
				GameManager.instance.permanentData.dwarfStrength ++;
			}
			break;
			case GameManager.character_rogue:
			if(this.tExtraHealth.isOn){
				GameManager.instance.permanentData.rogueBaseHealth ++;
			}
			if(this.tExtraStrength.isOn){
				GameManager.instance.permanentData.rogueStrength ++;
			}
			break;
			case GameManager.character_wizard:
			if(this.tExtraHealth.isOn){
				GameManager.instance.permanentData.wizardBaseHealth ++;
			}
			if(this.tExtraStrength.isOn){
				GameManager.instance.permanentData.wizardStrength ++;
			}
			break;
		}
		GameManager.instance.permanentData.gold = newGold;
		GameManager.instance.dataController.savePermanentData(GameManager.instance.permanentData);
		LoadSceneByIndex(0, -1);
	}
	private void setGoldAmountText(bool initial){
		if(initial){
			this.goldAmountText.text = "You have " + this.newGold.ToString("N0", CultureInfo.CurrentCulture) + " gold.\nSpend it wisely.";
		} else {
			this.goldAmountText.text = "You have " + this.newGold.ToString("N0", CultureInfo.CurrentCulture) + " gold.";	
		}
		
	}
	public void LoadSceneByIndex(int index, int returnIndex){
		if (returnIndex > 0){
			GameManager.instance.characterChoiceReturnSceneIndex = returnIndex;
		}
		SceneManager.LoadScene(index);
	}
}
