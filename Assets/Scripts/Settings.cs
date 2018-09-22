using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

	private static int StartSceneIndex = 0;
	
	public Slider musicVolumeSlider;
	public Text musicText;
	private Text tMusicText;
	private Slider sVolumeSlider;
	public Slider fxVolumeSlider;
	public Text fxText;
	private Text tFxText;
	private Slider sFxSlider;
	public Slider cameraSlider;
	public Text cameraText;
	private Text tCameraText;
	private Slider sCameraSlider;
	public GameObject OKButton;
	public GameObject CancelButton;
	public Toggle resetDataToggle;

	void Start () {
		this.sVolumeSlider = musicVolumeSlider.GetComponent<Slider>();
		musicVolumeSlider.onValueChanged.AddListener(delegate {musicSliderValueChanged(); });

		this.sFxSlider = fxVolumeSlider.GetComponent<Slider>();
		fxVolumeSlider.onValueChanged.AddListener(delegate {fxSliderValueChanged(); });

		this.sCameraSlider = cameraSlider.GetComponent<Slider>();
		cameraSlider.onValueChanged.AddListener(delegate {cameraSliderValueChanged(); });

		this.tMusicText = musicText.GetComponent<Text>();
		this.tFxText = fxText.GetComponent<Text>();
		this.tCameraText = cameraText.GetComponent<Text>();

		Button OK = OKButton.GetComponent<Button>();
        Button cancel = CancelButton.GetComponent<Button>();
        OK.onClick.AddListener(submitSettings);
        cancel.onClick.AddListener(returnToStart);

		setValues();
	}

	private void musicSliderValueChanged(){
		if(this.sVolumeSlider.value == 0){
			this.tMusicText.text = "Music volume: off";
		} else {
			this.tMusicText.text = "Music volume: " + (this.sVolumeSlider.value * 11f).ToString("0.0");
		}
	}
	private void fxSliderValueChanged(){
		if(this.sFxSlider.value == 0){
			this.tFxText.text = "FX volume: off";
		} else {
			this.tFxText.text = "FX volume: " + (this.sFxSlider.value * 11f).ToString("0.0");
		}
	}
	private void cameraSliderValueChanged(){
		if(this.sCameraSlider.value < 0.25f){
			this.tCameraText.text = "View size: near " + (6 + (int)(5 * this.sCameraSlider.value));
		} else if(this.sCameraSlider.value > 0.75f){
			this.tCameraText.text = "View size: far " + (6 + (int)(5 * this.sCameraSlider.value));
		} else {
			this.tCameraText.text = "View size: " + (6 + (int)(5 * this.sCameraSlider.value));
		}
	}
	private void setValues(){
		this.sVolumeSlider.value = GameManager.instance.dataController.musicVolume;
		if(this.sVolumeSlider.value == 0){
			this.tMusicText.text = "Music volume: off";
		} else {
			this.tMusicText.text = "Music volume: " + (this.sVolumeSlider.value * 11f).ToString("0.0");
		}

		this.sFxSlider.value = GameManager.instance.dataController.fxVolume;
		if(this.sFxSlider.value == 0){
			this.tFxText.text = "FX volume: off";
		} else {
			this.tFxText.text = "FX volume: " + (this.sFxSlider.value * 11f).ToString("0.0");
		}
		
		this.sCameraSlider.value = (GameManager.instance.dataController.cameraSize - 6) / 5f;
		if(this.sCameraSlider.value < 0.25f){
			this.tCameraText.text = "View size: near " + (6 + (int)(5 * this.sCameraSlider.value));
		} else if(this.sCameraSlider.value > 0.75f){
			this.tCameraText.text = "View size: far " + (6 + (int)(5 * this.sCameraSlider.value));
		} else {
			this.tCameraText.text = "View size: " + (6 + (int)(5 * this.sCameraSlider.value));
		}
	}

	private void submitSettings() {
		float mVolume = this.sVolumeSlider.value;
		GameManager.instance.dataController.setMusicVolume(this.sVolumeSlider.value);
        SoundManager.instance.musicSource.volume = mVolume;
		float fxVolume = this.sFxSlider.value;
		GameManager.instance.dataController.setFxVolume(this.sFxSlider.value);
		SoundManager.instance.efxSource1.volume = fxVolume;
        SoundManager.instance.efxSource2.volume = fxVolume;
		int camera = 6 + (int)(5 * this.sCameraSlider.value);
		GameManager.instance.dataController.setCameraSize(camera);
		SceneManager.LoadScene(Settings.StartSceneIndex);

		if(this.resetDataToggle.GetComponent<Toggle>().isOn) {
			GameManager.instance.setNewPermanentData();
		}
	}

	private void returnToStart(){
		SceneManager.LoadScene(Settings.StartSceneIndex);
	}
}
