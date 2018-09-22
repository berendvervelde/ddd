using UnityEngine;
using UnityEngine.UI;

public class updateScoreText : MonoBehaviour {

	public Text goldText;
	public Text highscore;
	private Text introGoldText;
	private Text introHighScoreText;
	private bool textSet = false;
	
	void Start () {
		this.introGoldText = this.goldText.GetComponent<Text>();
        this.introHighScoreText = this.highscore.GetComponent<Text>();
	}
	void Update(){
		// because permanentData will be null in the Start() the score has to be set in the Update().
		// Thanks to the boolean textSet, the text will only be updated once. 
		if(!textSet){
			if (GameManager.instance.permanentData != null){
				if(this.introHighScoreText != null ){
					this.introHighScoreText.text = "High score: " + GameManager.instance.permanentData.highscore;
				}
				if (this.introGoldText != null){
					this.introGoldText.text = "Current gold: " + GameManager.instance.permanentData.gold;
				}
				textSet = true;
			}
		}
	}
}
