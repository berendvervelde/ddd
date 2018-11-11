using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    public const int
        level_standard = 0,
        level_bomb = 1,
        level_monsters = 2,
        level_low_monsters = 3,
        level_treasure = 4,
        level_potion = 5,
        level_wasteland = 6,
        level_bushfire = 7,           // bushfire will 'convert' aligning times with other bushfires, changing items into monsters
        level_necromancer = 8,        // the necromancer can add random zombies to the board
        level_death = 9,              // death will hurt you if you come too close
        level_locked = 10,              // you need to find a key to proceed
        level_vampire = 11,
        level_rabits = 12,
        level_test = 13;

    private const int
        state_intro = 0,
        state_doing_setup = 1,
        state_setup_done = 2,
        state_fadeout_overlay = 3,
        state_game = 4;

    public const int
        character_knight = 0,
        character_ranger = 1,
        character_rogue = 2,
        character_dwarf = 3,
        character_wizard = 4;

    private const int 
        type_health = 0,
        type_scoreMultiplier = 1,
        type_extraGem = 2,
        type_extraLife = 3,
        type_fullHealth = 4,
        type_weapon = 5;

    private static readonly List<String> levelTypeText = new List<String>(){
        "",
        "Explosive floor!",
        "Monstrous level",
        "Kindergarten",
        "Treasury",
        "The alchemists floor",
        "Wasteland",
        "Bushfire!",
        "Necromancer's den",
        "Death becomes you",
        "Locked down",
        "Bloodsucker!",
        "Rabbid hole",
        "Testing"
    };
    
    private static readonly List<string> adjectives = new List<string>(){
        "Diabolic",
        "Desperate",
        "Deadly",
        "Devious",
        "Doomed",
        "Dreadful",
        "Destructive",
        "Demoralizating",
        "Dehumanizating",
        "Dissatisfying",
        "Dichlorobenzene",
        "Dissapointing",
        "Discriminating",
        "Disinformed",
        "Disrespectable",
        "Discontinued",
        "Disciplining",
        "Dualistic",
        "Doleful",
        "Domineering",
        "Dramatizing",
        "Disagreeing",
        "Degenerative",
        "Denunciating",
        "Distinct",
        "Deteriorating",
        "Disapproving",
        "Distasteful",
        "Distrustful",
        "Difficult",
        "Depressing",
        "Disgusting",
        "Dismissive",
        "Disorderly",
        "Desolate",
        "Dangerous",
        "Disturbing",
        "Detrimental",
        "Debatable",
        "Decadent",
        "Dramatic",
        "Diseased",
        "Distorted",
        "Defunct",
        "Despised",
        "Demanding",
        "Defective",
        "Dreary",
        "Dated",
        "Dirty",
        "Dusty",
        "Dodgy"
    };
    private static readonly List<string> nouns = new List<string>(){
        "Desperation",
        "Death",
        "Deviousness",
        "Doom",
        "Dread",
        "Destruction",
        "Demoralization",
        "Dehumanization",
        "Dissatisfacton",
        "Dichlorobenzene",
        "Dissapointment",
        "Discrimination",
        "Disinformation",
        "Disrespectability",
        "Discontinuity",
        "Discipline",
        "Dualistism",
        "Dolefulness",
        "Dominance",
        "Disagreement",
        "Degeneration",
        "Denunciation",
        "Distinction",
        "Deterioration",
        "Disapprovement",
        "Distastefulness",
        "Distrustfulness",
        "Difficulty",
        "Disgust",
        "Desolation",
        "Danger",
        "Detriment",
        "Debate",
        "Decadence",
        "Drama",
        "Disease",
        "Distortion",
        "Demand",
        "Defect",
        "Dirt",
        "Dust"
    };
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector] public BoardManager boardScript;      //Store a reference to our BoardManager which will set up the level.
    [HideInInspector] public DataController dataController;
    [HideInInspector] public bool playersTurn = true;
    [HideInInspector] public bool playersTurnEnd = true;
    [HideInInspector] public bool waiting = false;
    [HideInInspector] public int playerPreviousHealth;      // needed for the fullHealthToggle
    [HideInInspector] public Item[,] itemMap;
    private GameObject levelImage;
    [HideInInspector] public Player player;
    private bool continueClicked = false;
    private Text dungeonName;
    private GameObject gameOverImage;
    private Text gemAmountText;
    private Text maxHealthText;
    private Text scoreMultiplierText;
    private Button maxHealthMinusButton;
    private Button maxHealthPlusButton;
    private Button scoreMultiplierMinusButton;
    private Button scoreMultiplierPlusButton;
    private Toggle extraLifeToggle;
    private Toggle extraGemToggle;
    private Toggle fullHealthToggle;
    private Toggle addWeaponToggle;
    private int declaredItems = 0;                      // if declareditems equals items itemsOnBoard we know that all items have initialized
    private int itemsOnBoard = 0;
    [HideInInspector] public bool continueAvailable = false;
    [HideInInspector] public GameProgress gameProgress;
    [HideInInspector] public PermanentData permanentData;
    private GameProgress restoredGameProgress;
    [HideInInspector] public int activeMonsters;
    private int gameState = GameManager.state_intro;

    void Awake() {
        //Make GameManager singleton
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        this.boardScript = GetComponent<BoardManager>();
        this.dataController = GetComponent<DataController>();
    }
    public void setNewPermanentData(){
        this.permanentData = new PermanentData();
        this.permanentData.highscore = 0;
        this.permanentData.gold = 0;
        this.permanentData.knightBaseHealth = 11;
        this.permanentData.wizardBaseHealth = 9;
        this.permanentData.rogueBaseHealth = 9;
        this.permanentData.rangerBaseHealth = 10;
        this.dataController.savePermanentData(this.permanentData);
    }
    void Start(){
        initDefaultValuesPreferences();

        this.permanentData = this.dataController.loadPermanentData();
        if (this.permanentData == null){
            setNewPermanentData();
        }
        this.restoredGameProgress = this.dataController.loadGameProgress();
        if (this.restoredGameProgress != null) {
            continueAvailable = true;
        }
        this.gameProgress = new GameProgress();
        this.gameProgress.level = 0;

        // start the music after the volume is set
        setSoundVolume();
        SoundManager.instance.playMusic = true;
    }

    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization() {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene scene, LoadSceneMode arg1) {
        if (scene.buildIndex == 1 && instance != null) {
            instance.gameState = state_game;
            instance.gameProgress.level++;
            instance.InitGame();
        }
    }
    void Update() {
        switch (this.gameState) {
            case GameManager.state_setup_done:
                if(this.continueClicked){
                    StartCoroutine(FadeoutScreenOverlay());
                    setupPlayer();
                    this.gameState = GameManager.state_fadeout_overlay;
                    this.continueClicked = false;
                }
                break;
            case GameManager.state_game:
                if(this.player.enabled){
                    checkPlayerMove();

                    if (playersTurnEnd && !waiting) {
                        waiting = true;
                        playersTurnEnd = false;
                        updateTimedItems();
                        if (this.activeMonsters == 0){
                            this.activeMonsters = -1;
                            this.player.getGemFromPerformance();
                        }
                        findItemsNearPlayer();
                    }
                }
                break;
        }
    }
    private void setupPlayer(){
        switch (this.gameProgress.playerChoice) {
            case GameManager.character_knight:
                this.player.baseHealth = this.player.maxHealth  = this.permanentData.knightBaseHealth;
                break;
            case GameManager.character_wizard:
                this.player.baseHealth = this.player.maxHealth  = this.permanentData.wizardBaseHealth;
                break;
            case GameManager.character_ranger:
                this.player.baseHealth = this.player.maxHealth  = this.permanentData.rangerBaseHealth;
                break;
            case GameManager.character_rogue:
                this.player.baseHealth = this.player.maxHealth  = this.permanentData.rogueBaseHealth;
                break;
            case GameManager.character_dwarf:
                this.player.baseHealth = this.player.maxHealth  = this.permanentData.dwarfBaseHealth;
                break;
        }
        this.player.maxHealth = this.player.baseHealth;
        this.player.scoreMultiplier = this.gameProgress.scoreMultiplier;
        this.player.health = this.gameProgress.health;
        this.player.steps = this.gameProgress.steps;
        this.player.gold = this.gameProgress.gold;
        this.player.gemCount = this.gameProgress.gems;
        this.player.hasKey = false;
        this.player.updateHUD();
    }
    public void restoreGame(){
        this.gameProgress = this.restoredGameProgress;
    }
    private void initDefaultValuesPreferences(){
        int sc = this.dataController.selectedCharacter;
        if (sc < 0){
            this.dataController.setSelectedCharacter(0);
        }
        int cam = this.dataController.cameraSize;
        if (cam < 0){
            this.dataController.setCameraSize(7);
        }
        float mv = this.dataController.musicVolume;
        if (mv < 0){
            this.dataController.setMusicVolume(0.5f);
        }
        float fxv = this.dataController.fxVolume;
        if (fxv < 0){
            this.dataController.setFxVolume(0.8f);
        }
    }
    void InitGame() {
        this.gameState = GameManager.state_doing_setup;
        this.continueClicked = false;

        this.declaredItems = 0;
        this.itemsOnBoard = this.boardScript.SetupScene(this.gameProgress.level, this.gameProgress.steps);
        this.activeMonsters = this.boardScript.activemonsters;
        Debug.Log("active monsters: " + this.activeMonsters);
        
        this.itemMap = new Item[this.boardScript.columns, this.boardScript.rows];

        // open exit at start
        if(!this.boardScript.isLocked){
            Animator exitAnimator = GameManager.instance.boardScript.instantiatedExit.GetComponent<Animator>();
            exitAnimator.SetTrigger("OpenExit");
        }
        // the rest can only be done after the player has registered itself with GameManager, so we continue in finishLevelSetup()
    }
    public void finishLevelSetup(){
        // we pick up where we left off in the InitGame() method now that the player has arrived
       Text levelText = this.player.levelText.GetComponent<Text>();
        levelText.text = "Level: " + this.gameProgress.level;

        Text dungeonType = this.player.dungeonType.GetComponent<Text>();
        dungeonType.text = GameManager.levelTypeText[this.boardScript.levelType];

        this.dungeonName = this.player.dungeonName.GetComponent<Text>();
        this.levelImage = this.player.levelOverlay;
        Button continueButton = this.player.continueButton.GetComponent<Button>();
        continueButton.onClick.AddListener(delegate {startLevel(); });

        this.gameOverImage = this.player.gameOverOverlay;

        this.dungeonName.text = getRandomDungeonName();
        this.gameOverImage.SetActive(false);
        this.levelImage.SetActive(true);
    }

    public void saveGameState(){
        this.dataController.saveGameProgress(this.gameProgress);
    }
    public void deleteGameState(){
        this.dataController.deleteGameProgress();
    }
    private void initGemManagement(){
        this.gemAmountText = this.player.gemAmountText.GetComponent<Text>();
        this.maxHealthText = this.player.maxHealthText.GetComponent<Text>();
        this.scoreMultiplierText = this.player.scoreMultiplierText.GetComponent<Text>();
        this.maxHealthMinusButton = this.player.maxHealthMinusButton.GetComponent<Button>();
        this.maxHealthMinusButton.onClick.AddListener(delegate {updateGemBenefits(GameManager.type_health,-1); });
        this.maxHealthPlusButton = this.player.maxHealthPlusButton.GetComponent<Button>();
        this.maxHealthPlusButton.onClick.AddListener(delegate {updateGemBenefits(GameManager.type_health,1); });
        this.scoreMultiplierMinusButton = this.player.scoreMultiplierMinusButton.GetComponent<Button>();
        this.scoreMultiplierMinusButton.onClick.AddListener(delegate {updateGemBenefits(GameManager.type_scoreMultiplier,-1); });
        this.scoreMultiplierPlusButton  = this.player.scoreMultiplierPlusButton.GetComponent<Button>();
        this.scoreMultiplierPlusButton.onClick.AddListener(delegate {updateGemBenefits(GameManager.type_scoreMultiplier,1); });
        this.extraLifeToggle = this.player.extraLifeToggle.GetComponent<Toggle>();
        this.extraLifeToggle.onValueChanged.AddListener(delegate { updateGemBenefits(GameManager.type_extraLife, 0);});
        this.extraGemToggle = this.player.extraGemToggle.GetComponent<Toggle>();
        this.extraGemToggle.onValueChanged.AddListener(delegate { updateGemBenefits(GameManager.type_extraGem, 0);});
        this.fullHealthToggle = this.player.fullHealthToggle.GetComponent<Toggle>();
        this.fullHealthToggle.onValueChanged.AddListener(delegate { updateGemBenefits(GameManager.type_fullHealth, 0);});
        this.addWeaponToggle = this.player.addWeaponToggle.GetComponent<Toggle>();
        this.addWeaponToggle.onValueChanged.AddListener(delegate { updateGemBenefits(GameManager.type_weapon, 0);});

        this.maxHealthMinusButton.interactable = false;
        this.scoreMultiplierMinusButton.interactable = false;
        if(this.gameProgress.gems == 0) {
            this.maxHealthPlusButton.interactable = false;
            this.scoreMultiplierPlusButton.interactable = false;
            this.extraLifeToggle.interactable = false;
            this.extraGemToggle.interactable = false;
            this.addWeaponToggle.interactable = false;
            this.fullHealthToggle.interactable = false;
            this.gemAmountText.text = "You have no gems. Better luck next time.";
        }else if(this.gameProgress.gems == 1){
            this.extraLifeToggle.interactable = false;
            this.extraGemToggle.interactable = false;
            this.gemAmountText.text = "You have 1 gem. Spend it wisely.";
        } else if(this.gameProgress.gems > 1) {
            this.gemAmountText.text = "You have " + this.gameProgress.gems + " gems. Spend them wisely.";
        }
        this.maxHealthText.text = this.gameProgress.maxHealth.ToString();
        this.scoreMultiplierText.text = this.gameProgress.scoreMultiplier + "x";
    }

    private void setSoundVolume() {
        SoundManager.instance.efxSource1.volume = this.dataController.fxVolume;
        SoundManager.instance.efxSource2.volume = this.dataController.fxVolume;
        SoundManager.instance.musicSource.volume = this.dataController.musicVolume;
    }
    private void updateGemBenefits(int type, int value) {
        switch (type){
            case GameManager.type_health:
            if (value > 0){
                this.gameProgress.maxHealth ++;
                this.gameProgress.gems --;
            } else if(this.gameProgress.maxHealth > this.gameProgress.baseHealth){
                this.gameProgress.maxHealth --;
                this.gameProgress.gems ++;
            }
            break;
            case GameManager.type_scoreMultiplier:
            if (value > 0){
                this.gameProgress.scoreMultiplier *= 2;
                this.gameProgress.gems --;
            } else if(this.gameProgress.scoreMultiplier > 1){
                this.gameProgress.scoreMultiplier /= 2;
                this.gameProgress.gems ++;
            }
            break;
            case GameManager.type_extraGem:
            if(this.extraGemToggle.isOn){
                this.gameProgress.gems -= 2;
                this.boardScript.addExtraGem();
            } else {
                this.gameProgress.gems += 2;
                this.boardScript.removeExtraGem();
            }
            break;
            case GameManager.type_extraLife:
            if(this.extraLifeToggle.isOn){
                this.gameProgress.gems -= 2;
                this.player.extraLife = true;
            } else {
                this.gameProgress.gems += 2;
                this.player.extraLife = false;
            }
            break;
            case GameManager.type_fullHealth:
            if(this.fullHealthToggle.isOn){
                this.playerPreviousHealth = this.gameProgress.health;
                this.gameProgress.health = this.gameProgress.maxHealth;
                this.gameProgress.gems -= 1;
            } else {
                this.gameProgress.health = this.playerPreviousHealth;
                this.gameProgress.gems += 1;
            }
            break;
            case GameManager.type_weapon:
            if(this.addWeaponToggle.isOn){
                this.player.attachWeaponToSelf(20, 10);
                this.gameProgress.gems -= 1;
            } else {
                this.player.disarmPlayer();
                this.gameProgress.gems += 1;
            }
            break;
        }

        this.maxHealthText.text = this.gameProgress.maxHealth.ToString();
        this.scoreMultiplierText.text = this.gameProgress.scoreMultiplier + "x";

        if (this.gameProgress.maxHealth > this.gameProgress.baseHealth){
            this.maxHealthMinusButton.interactable = true;
        } else {
            this.maxHealthMinusButton.interactable = false;
        }
        if (this.gameProgress.scoreMultiplier > 1){
            this.scoreMultiplierMinusButton.interactable = true;
        } else {
            this.scoreMultiplierMinusButton.interactable = false;
        }

        if(this.gameProgress.gems == 0) {
            this.maxHealthPlusButton.interactable = false;
            this.scoreMultiplierPlusButton.interactable = false;
            if (this.extraGemToggle.isOn){
                this.extraGemToggle.interactable = true;
            } else {
                this.extraGemToggle.interactable = false;
            }
            if (this.extraLifeToggle.isOn){
                this.extraLifeToggle.interactable = true;
            } else {
                this.extraLifeToggle.interactable = false;
            }
            if(this.addWeaponToggle.isOn){
                this.addWeaponToggle.interactable = true;
            } else {
                this.addWeaponToggle.interactable = false;
            }
            if(this.fullHealthToggle.isOn){
                this.fullHealthToggle.interactable = true;
            } else {
                this.fullHealthToggle.interactable = false;
            }
            this.gemAmountText.text = "You have no more gems.";
        } else if(this.gameProgress.gems == 1){
            this.maxHealthPlusButton.interactable = true;
            if (this.gameProgress.scoreMultiplier < 8){
                this.scoreMultiplierPlusButton.interactable = true;
            } else {
                this.scoreMultiplierPlusButton.interactable = false;
            }
            this.fullHealthToggle.interactable = true;
            this.addWeaponToggle.interactable = true;
            if (this.extraGemToggle.isOn){
                this.extraGemToggle.interactable = true;
            } else {
                this.extraGemToggle.interactable = false;
            }
            if (this.extraLifeToggle.isOn){
                this.extraLifeToggle.interactable = true;
            } else {
                this.extraLifeToggle.interactable = false;
            }
            this.gemAmountText.text = "You have 1 gem left.";
        }else if(this.gameProgress.gems > 1){
            this.addWeaponToggle.interactable = true;
            this.fullHealthToggle.interactable = true;
            this.maxHealthPlusButton.interactable = true;
            if (this.gameProgress.scoreMultiplier < 8){
                this.scoreMultiplierPlusButton.interactable = true;
            } else {
                this.scoreMultiplierPlusButton.interactable = false;
            }
            this.extraGemToggle.interactable = true;
            this.extraLifeToggle.interactable = true;
            this.gemAmountText.text = "You have " + this.gameProgress.gems + " gems left.";
        }
    }
    private void startLevel(){
        this.continueClicked = true;
    }
    private String getRandomDungeonName() {
        int adjectiveIndex = Random.Range(0, GameManager.adjectives.Count);
        int nounIndex = Random.Range(0, GameManager.nouns.Count);
        return GameManager.adjectives[adjectiveIndex] + " Dungeons of " + GameManager.nouns[nounIndex];
    }
    public void GameOver() {
        String score = "Final score:\n" + this.player.gold;
        if (this.player.gold > this.permanentData.highscore){
            score += "\nThat's a high score!";
            this.permanentData.highscore = this.player.gold;
        }
        this.permanentData.gold += this.player.gold;
        this.dataController.savePermanentData(this.permanentData);
        
        Text scoreText = this.player.gameOverScoreText.GetComponent<Text>();
        scoreText.text = score;

        StartCoroutine(FadeinGameoverScreen());
        this.enabled = false;
        this.gameState = state_intro;
    }

    //Call this to add the passed in Item to the List of Items. Also set its health
    public void AddItemToList(Item item) {
        if (this.gameState == GameManager.state_doing_setup) {
   
            TileMapCoordinates iC = convertCoordinatesToTileMap(item.transform.position.x, item.transform.position.y);
            this.itemMap[iC.x, iC.y] = item;
            this.declaredItems++;
            if (this.declaredItems == this.itemsOnBoard) {
                // all items accounted for. finish setup
                for(int i=0; i < this.itemMap.GetLength(0); i++){
                    for(int j=0; j < this.itemMap.GetLength(1); j++){
                        Item it = this.itemMap[i,j];
                        if(it != null){
                            it.finishFullInit(this.boardScript.monsterTiles, this.boardScript.itemTiles);
                        }
                    }
                }
                this.gameState = GameManager.state_setup_done;
            }
        } else {
            TileMapCoordinates iC = convertCoordinatesToTileMap(item.transform.position.x, item.transform.position.y);
            // sometimes multiple gameobjects get created @ the same location @ the same time, like the bushfire, bunny or treemonster
            if(this.itemMap[iC.x, iC.y] !=null && (
                item.type == 102 ||
                item.type == 111 ||
                item.type == 114
                )){
                    Destroy(this.itemMap[iC.x, iC.y].gameObject);
                }
            this.itemMap[iC.x, iC.y] = item;
        }
    }

    public void RemoveItemFromList(Item i) {
        TileMapCoordinates iC = convertCoordinatesToTileMap(i.transform.position.x, i.transform.position.y);
        this.itemMap[iC.x, iC.y] = null;
    }

    public void setPlayer(Player p) {
        this.player = p;
        this.player.hudPanel.GetComponent<CanvasGroup>().alpha = 0;
        finishLevelSetup();
        initGemManagement();
    }

    private void findItemsNearPlayer() {
        int px = (int)(this.player.transform.position.x / BoardManager.wMultiplier);
        int py = (int)(this.player.transform.position.y / BoardManager.hMultiplier);
        StartCoroutine(TurnCard(px, py));
    }

    private bool setActive(int x, int y) {
       if (x > -1 && x < this.boardScript.columns && y > -1 && y < this.boardScript.rows) {
            Item i = this.itemMap[x, y];
            if (i != null && !i.show) {
                i.setShow(true);
                return true;
            }
        }
        return false;
    }
    IEnumerator TurnCard(int px, int py) {
        bool wait = false;
        wait = setActive(px - 1, py);
        if(wait){
            yield return new WaitForSeconds(0.2f);
        }
        wait = setActive(px + 1, py);
        if(wait){
            yield return new WaitForSeconds(0.2f);
        }
        wait = setActive(px, py - 1);
        if(wait){
            yield return new WaitForSeconds(0.2f);
        }
        setActive(px, py + 1);
        playersTurn = true;
        waiting = false;
    }

    public Item findSecondHit(int horizontal, int vertical, int px, int py){
        if(!furtherEdges(horizontal, vertical, px, py)){
            Item i;
            if (horizontal > 0) {
                i = this.itemMap[px + 2, py];
            } else if (horizontal < 0) {
                i = this.itemMap[px - 2, py];
            } else if (vertical > 0) {
                i = this.itemMap[px, py + 2];
            } else {
                i = this.itemMap[px, py - 2];
            }
            // item is an existing monster
            if (i!=null && i.type > 99 && i.show){
                return i;
            }
        }
        return null;
    }

    public Item findItem(int horizontal, int vertical, int px, int py){
        Item i = null;
        // check if a monster or chest is in the way
        if (horizontal > 0) {
            i = this.itemMap[px + 1, py];
        } else if (horizontal < 0) {
            i = this.itemMap[px - 1, py];
        } else if (vertical > 0) {
            i = this.itemMap[px, py + 1];
        } else {
            i = this.itemMap[px, py - 1];
        }
        return i;
    }

    public Item[] findMonstersAround(int px, int py){
        int totalMonsters = 0;
        Item[] monsters = new Item[4];
        Item i = null;
        //check to see if the knight is surrounded by monsters
        if (px < (this.boardScript.columns - 1)){
            i = this.itemMap[px + 1, py];
            if(i != null && i.type > 99){
                monsters[totalMonsters] = i;
                totalMonsters ++;
            }
        }
        if (px > 0){
            i = this.itemMap[px - 1, py];
            if(i != null && i.type > 99){
                monsters[totalMonsters] = i;
                totalMonsters ++;
            }
        }
        if (py < (this.boardScript.rows - 1)){
            i = this.itemMap[px, py + 1];
            if(i != null && i.type > 99){
                monsters[totalMonsters] = i;
                totalMonsters ++;
            }
        }
        if (py > 0){
            i = this.itemMap[px, py - 1];
            if(i != null && i.type > 99){
                monsters[totalMonsters] = i;
                totalMonsters ++;
            }
        }
        if (totalMonsters > 2){
            return monsters;
        }
        return null;
    }

    public bool edges(int horizontal, int vertical, int px, int py) {
        if (horizontal > 0 && px < (this.boardScript.columns - 1)) {
            return false;
        }
        if (horizontal < 0 && px > 0) {
            return false;
        }
        if (vertical > 0 && py < (this.boardScript.rows - 1)) {
            return false;
        }
        if (vertical < 0 && py > 0) {
            return false;
        }
        return true;
    }

    private bool furtherEdges(int horizontal, int vertical, int px, int py) {
        if (horizontal > 0 && px < (this.boardScript.columns - 2)) {
            return false;
        }
        if (horizontal < 0 && px > 1) {
            return false;
        }
        if (vertical > 0 && py < (this.boardScript.rows - 2)) {
            return false;
        }
        if (vertical < 0 && py > 1) {
            return false;
        }
        return true;
    }

    private void updateTimedItems() {
        for(int i=0; i < this.itemMap.GetLength(0); i++){
            for(int j=0; j < this.itemMap.GetLength(1); j++){
                Item it = this.itemMap[i,j];
                if(it != null && it.show) {
                    switch (it.type) {
                        case 40:                // bomb
                        it.countDownBomb();
                        break;
                        case 102:              //bushfire
                        it.countDownSpread();
                        break;
                        case 103:               // zombie
                        it.zombieHit();
                        break;
                        case 104:               // necromancer
                        it.countDownZombieSpawn();
                        break;
                        case 105:               // vampire
                        it.countDownZombieSpawn();
                        it.vampireSuck();
                        break;
                        case 106:               // unicorn shoots rainbows
                        it.shootRainbows();
                        break;
                        case 108:
                        it.waspSting();         // wasps might sting
                        break;
                        case 109:               // egg -> snakewoman
                        it.waitEgg();
                        break;
                        case 111:               //small tree
                        it.growTree(false);
                        break;
                        case 112:               //medium tree
                        it.growTree(false);
                        break;
                        case 113:               //large tree
                        it.growTree(true);
                        break;
                        case 114:               // rabbit
                        it.breed();
                        break;
                        case 115:               // death
                        it.feastUponTheLiving();
                        break;
                    }
                }
            }
        }
        if (this.player.heldItemType == 40){
            // TODO: implement?
            //this.player.countDown();
        }
    }
    private void checkPlayerMove() {
        //If it's not the player's turn, don't check movement   
        if (playersTurn && this.gameState == GameManager.state_game) {
            int horizontal = 0;
            int vertical = 0;

            if (Input.GetMouseButtonDown(0)) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //convert to gametiles
                TileMapCoordinates mouseC = convertCoordinatesToTileMap(mousePos.x, mousePos.y);
                int px = (int)(this.player.transform.position.x / BoardManager.wMultiplier);
                int py = (int)(this.player.transform.position.y / BoardManager.hMultiplier);
                if (mouseC.x > px) {
                    horizontal = BoardManager.wMultiplier;
                    vertical = 0;
                } else if (mouseC.x < px) {
                    horizontal = -BoardManager.wMultiplier;
                    vertical = 0;
                } else if (mouseC.y > py) {
                    vertical = BoardManager.hMultiplier;
                } else if (mouseC.y < py) {
                    vertical = -BoardManager.hMultiplier;
                }
            }
            //we have movement, let the player itself figure out if he can move
            if (horizontal != 0 || vertical != 0) {
                this.player.PlayerMove(horizontal, vertical);
            }
        }
    }
    private TileMapCoordinates convertCoordinatesToTileMap(float x, float y) {
        // 1f and 1.5 f is a correction for the board position in relation of the player at the start. Caused by the size of the sprites.
        return new TileMapCoordinates((int)((x + 1f) / BoardManager.wMultiplier), (int)((y + 1.5f) / BoardManager.hMultiplier));
    }

    protected IEnumerator FadeoutScreenOverlay() {
        CanvasGroup cg = this.levelImage.GetComponent<CanvasGroup>();
        this.player.hudPanel.SetActive(true);
        CanvasGroup panel = this.player.hudPanel.GetComponent<CanvasGroup>();
        panel.alpha = 0f;

        while (cg.alpha > 0f) {
            cg.alpha = cg.alpha - (Time.deltaTime);
            panel.alpha = panel.alpha + (Time.deltaTime);
            yield return null;
        }
        yield return null;
        levelImage.SetActive(false);
        this.gameState = GameManager.state_game;
    }

    protected IEnumerator FadeinGameoverScreen() {
        this.gameOverImage.SetActive(true);
        CanvasGroup cg = this.gameOverImage.GetComponent<CanvasGroup>();
        cg.alpha =  0f;
        while (cg.alpha < 1f) {
            cg.alpha = cg.alpha + (Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(3f);
        this.gameState = GameManager.state_doing_setup;
        this.gameProgress.level = 0;
        this.enabled = true;
        SceneManager.LoadScene(0);
    }
}