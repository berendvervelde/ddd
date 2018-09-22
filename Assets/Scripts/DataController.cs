using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
public class DataController : MonoBehaviour {
    private static string gameDataFileName = "progress";
    private static string permanentDataFileName = "permanent";
    private static string selectedCharacterString = "selectedCharacter";
    private static string musicVolumeString = "musicVolume";
    private static string fxVolumeString = "fxVolume";
    private static string cameraSizeString = "cameraSize";
    [HideInInspector] public int selectedCharacter = -1;
    [HideInInspector] public float musicVolume = -1f;
    [HideInInspector] public float fxVolume = -1f;
    [HideInInspector] public int cameraSize = -1;
    [HideInInspector] public GameProgress progress = null;
    [HideInInspector] public PermanentData data = null;
    void Awake() {
        this.selectedCharacter = PlayerPrefs.GetInt(DataController.selectedCharacterString, -1);
        this.musicVolume = PlayerPrefs.GetFloat(DataController.musicVolumeString, -1f);
        this.fxVolume = PlayerPrefs.GetFloat(DataController.fxVolumeString, -1f);
        this.cameraSize = PlayerPrefs.GetInt(DataController.cameraSizeString, -1);
    }
    public GameProgress loadGameProgress() {
        string filePath = Path.Combine(Application.persistentDataPath, DataController.gameDataFileName);

        if (File.Exists(filePath)) {
            try {
                return GameProgress.Deserialize(File.ReadAllBytes(filePath));
            } catch(EndOfStreamException e){
                // new version perhaps?
                Debug.Log(e);
                return null;
            }
        } else {
            return null;
        }
    }
    public PermanentData loadPermanentData() {
        string filePath = Path.Combine(Application.persistentDataPath, DataController.permanentDataFileName);

        if (File.Exists(filePath)) {
            try {
                return PermanentData.Deserialize(File.ReadAllBytes(filePath));
            } catch(EndOfStreamException e){
                // new version perhaps?
                Debug.Log(e);
                return null;
            }
        } else {
            return null;
        }
    }
    public void saveGameProgress(GameProgress gp){
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, DataController.gameDataFileName), gp.Serialize());
    }
    public void savePermanentData(PermanentData pd){
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, DataController.permanentDataFileName), pd.Serialize());
    }
    public void deleteGameProgress(){
        File.Delete(Path.Combine(Application.persistentDataPath, DataController.gameDataFileName));
    }

    public void setSelectedCharacter(int characterIndex){
        this.selectedCharacter = characterIndex;
        PlayerPrefs.SetInt(DataController.selectedCharacterString, characterIndex);
    }
    public void setCameraSize(int camera){
        this.cameraSize = camera;
        PlayerPrefs.SetInt(DataController.cameraSizeString, camera);
    }
    public void setMusicVolume(float volume){
        this.musicVolume = volume;
        PlayerPrefs.SetFloat(DataController.musicVolumeString, volume);
    }
    public void setFxVolume(float volume){
        this.fxVolume = volume;
        PlayerPrefs.SetFloat(DataController.fxVolumeString, volume);
    }
}