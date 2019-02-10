using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;      //Allows us to use SceneManager
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public const int
        sound_attack_knight = 0,
        sound_attack_ranger = 1,
        sound_attack_rogue = 2,
        sound_attack_dwarf = 3,
        sound_attack_wizard = 4,
        sound_hurt_knight = 5,
        sound_hurt_ranger = 6,
        sound_hurt_rogue = 7,
        sound_hurt_dwarf = 8,
        sound_hurt_wizard = 9;

    private int gemGold = 2000;
    public GameObject[] weapons;
    private GameObject characterWeapon;
    public GameObject [] weaponSlash;
    public AudioClip [] playerSounds;

    public GameObject levelOverlay;
    public Text levelText;
    public Text dungeonName;
    public Text dungeonType;
    public Text healthAmountPanelText;
    public Text goldPanelText;
    public Text goldMultiplierPanelText;
    public Text gemAmountPanelText;
    public GameObject extraLifePanelImage;
    public GameObject inflictionIndicationImage;                //currently displays green skull if player is poisoned. Could be extended to show other sprites
    public GameObject foundKeyPanelImage;
    public GameObject hudPanel;
    public Button continueButton;
    public GameObject gameOverOverlay;
    public Text gameOverScoreText;

    // here are all gem benefit controls
    public Text gemAmountText;
    public Text maxHealthText;
    public Text scoreMultiplierText;
    public Button maxHealthMinusButton;
    public Button maxHealthPlusButton;
    public Button scoreMultiplierMinusButton;
    public Button scoreMultiplierPlusButton;
    public Toggle extraLifeToggle;
    public Toggle extraGemToggle;
    public Toggle fullHealthToggle;
    public Toggle addWeaponToggle;
    public Toggle revealToggle;


    public GameObject[] Characters;
    public GameObject bomb;                 // to instantiate a thrown bomb in the board
    public GameObject heldBomb;             // like heldweapon
    public GameObject foundGem;             // for the got gem animation
    public GameObject greenSkull;          // if you're poisoned
    public GameObject greySkull;           // if you're hit
    public GameObject foundKey;             // if you've found the exit key
    [HideInInspector] public bool hasKey;
    public LayerMask blockingLayer;         //Layer on which collision will be checked.
    public Camera gameCamera;
    private Animator animator;                  //Used to store a reference to the Player's animator component.
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public int gold;
    private int previousGemGold;
    [HideInInspector] public int health;
    [HideInInspector] public int maxHealth;     //basehealth + gem health
    [HideInInspector] public int baseHealth;    // max health of playerclass
    [HideInInspector] public int playerStrength;
    [HideInInspector] public int scoreMultiplier = 1;
    [HideInInspector] public int poisonCount = 0;
    [HideInInspector] public int steps;
    [HideInInspector] public int selectedCharacter = GameManager.character_knight;
    private GameObject character;
    private float moveTime = 0.1f;           //Time it will take object to move, in seconds.
    private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
    private float inverseMoveTime;          //Used to make movement more efficient.
    private GameObject heldItem;
    [HideInInspector] public int heldItemValue;
    private Text heldItemValueText;
    [HideInInspector] public int heldItemType;
    private int dwarfAdvantage = 6;
    private int killEnemies = 0;
    [HideInInspector] public int gemCount = 0;
    [HideInInspector] public bool extraLife = false;
    public ParticleSystem assetSpawnParticles;

    protected void Start() {
        // set the camera size
        Camera c = this.gameCamera.GetComponent<Camera>();
        c.orthographicSize = GameManager.instance.dataController.cameraSize;

        // instanciate selected character and attach it to the player
        this.selectedCharacter = GameManager.instance.dataController.selectedCharacter;
        this.character = Instantiate(this.Characters[this.selectedCharacter], this.transform.position, Quaternion.identity);
        this.character.transform.parent = this.transform;

        this.characterWeapon = getCharacterWeapon();

        this.animator = this.character.GetComponent<Animator>();
        this.spriteRenderer = this.character.GetComponent<SpriteRenderer>();

        //For smooth movement
        this.rb2D = GetComponent<Rigidbody2D>();
        this.inverseMoveTime = 1f / moveTime;

        if (GameManager.instance.gameProgress.level == 1){
            initPlayerProperties();
            updateGameProgress();
        }
        GameManager.instance.setPlayer(this);
    }

    private void updateGameProgress(){
        GameManager.instance.gameProgress.health = this.health;
        GameManager.instance.gameProgress.maxHealth = this.maxHealth;
        GameManager.instance.gameProgress.baseHealth = this.baseHealth;
        GameManager.instance.gameProgress.gems = this.gemCount;
        GameManager.instance.gameProgress.scoreMultiplier = this.scoreMultiplier;
        GameManager.instance.gameProgress.gold = this.gold;
        GameManager.instance.gameProgress.steps = this.steps;
    }

    public void updateHUD(){
        this.gemAmountPanelText.text = this.gemCount.ToString();
        this.extraLifePanelImage.SetActive(this.extraLife);
        this.healthAmountPanelText.text =  this.health.ToString();
        this.foundKeyPanelImage.SetActive(this.hasKey);
        this.goldMultiplierPanelText.text = this.scoreMultiplier + "x";
        this.goldPanelText.text = this.gold.ToString();
    }
    private void initPlayerProperties(){
        this.playerStrength = 0;
        switch (this.selectedCharacter) {
            case GameManager.character_knight:
                this.health = this.maxHealth = this.baseHealth = GameManager.instance.permanentData.knightBaseHealth;
                this.playerStrength = GameManager.instance.permanentData.knightStrength;
                break;
            case GameManager.character_wizard:
                this.health = this.maxHealth = this.baseHealth = GameManager.instance.permanentData.wizardBaseHealth;
                this.playerStrength = GameManager.instance.permanentData.wizardStrength;
                break;
            case GameManager.character_ranger:
                this.health = this.maxHealth = this.baseHealth = GameManager.instance.permanentData.rangerBaseHealth;
                this.playerStrength = GameManager.instance.permanentData.rangerStrength;
                break;
            case GameManager.character_rogue:
                this.health = this.maxHealth = this.baseHealth = GameManager.instance.permanentData.rogueBaseHealth;
                this.playerStrength = GameManager.instance.permanentData.rogueStrength;
                break;
            case GameManager.character_dwarf:
                this.health = this.maxHealth = this.baseHealth = GameManager.instance.permanentData.dwarfBaseHealth;
                this.playerStrength = GameManager.instance.permanentData.dwarfStrength;
                break;
        }
        this.scoreMultiplier = 1;
        this.gemCount = 1;
        this.steps = 0;
        this.gold = 0;
    }

    public void setHealth(int health) {
        this.health = health;
        if (this.health > this.maxHealth){
            this.health = this.maxHealth;
        }
        this.healthAmountPanelText.text = this.health.ToString();
    }

    private GameObject getCharacterWeapon() {
        GameObject weapon = null;
        int weaponNumber = 0;
        switch (this.selectedCharacter) {
            case GameManager.character_knight:
                weaponNumber = 20;
                break;
            case GameManager.character_wizard:
                weaponNumber = 21;
                break;
            case GameManager.character_ranger:
                weaponNumber = 22;
                break;
            case GameManager.character_rogue:
                weaponNumber = 23;
                break;
            case GameManager.character_dwarf:
                weaponNumber = 24;
                break;
        }
        for (int i = 0; i < weapons.Length; i++) {
            HeldWeaponType hwt = weapons[i].GetComponent<HeldWeaponType>();
            if (hwt.type == weaponNumber) {
                weapon = weapons[i];
                break;
            }
        }
        return weapon;
    }
    public void attachWeaponToSelf(int type, int value) {
        // already holding a weapon?
        if (this.heldItem != null) {
            if (type == this.heldItemType) {
                // stack value
                this.heldItemValue += value;
                this.heldItemValueText = findTextComponent(this.heldItem);
                this.heldItemValueText.text = this.heldItemValue.ToString();
                return;
            }
            Destroy(this.heldItem);
            this.heldItem = null;
        }
        Vector3 weaponPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        weaponPosition.x = weaponPosition.x - 0.7f;
        this.heldItem = Instantiate(this.characterWeapon, weaponPosition, Quaternion.identity);
        this.heldItem.transform.parent = this.transform;
        this.heldItemValue = value;
        this.heldItemValueText = findTextComponent(this.heldItem);
        this.heldItemValueText.text = this.heldItemValue.ToString();
        this.heldItemType = type;
    }
    public void disarmPlayer(){
        if (this.heldItem != null) {
            Destroy(this.heldItem);
            this.heldItem = null;
        }
    }
    private void attachBombToSelf(int type, int value) {
        // already holding a weapon?
        if (this.heldItem != null) {
            Destroy(this.heldItem);
            this.heldItem = null;
        }
        Vector3 weaponPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        weaponPosition.x = weaponPosition.x - 0.7f;
        this.heldItem = Instantiate(this.heldBomb, weaponPosition, Quaternion.identity);
        this.heldItem.transform.parent = this.transform;
        this.heldItemValue = value;
        this.heldItemValueText = findTextComponent(this.heldItem);
        this.heldItemValueText.text = this.heldItemValue.ToString();
        this.heldItemType = type;
    }

    public void countDown(){
        // TODO: implement?
    }
    private Text findTextComponent(GameObject go) {
        // find the right child by finding its tag
        for (int i = 0; i < go.transform.childCount; i++) {
            if (go.transform.GetChild(i).gameObject.tag == "HeldCanvas") {
                return go.transform.GetChild(i).gameObject.GetComponent<Text>();
            }
        }
        return null;
    }
    public void PlayerMove(int h, int v) {
        // mirror sprite if needed
        updateFacing(h);

        int px = (int)(this.transform.position.x / BoardManager.wMultiplier);
        int py = (int)(this.transform.position.y / BoardManager.hMultiplier);
        if (GameManager.instance.edges(h, v, px, py)) {
            GameManager.instance.playersTurnEnd = true;
            return;
        }
        if (blockedByItem(h, v, px, py)) {
            checkIfPoisoned();
            GameManager.instance.playersTurnEnd = true;
            return;
        }
        Move(h, v);
        checkIfPoisoned();
    }

    private void checkIfPoisoned(){
        if (this.poisonCount > 0){
            StartCoroutine(AnimateFoundItem(Instantiate(this.greenSkull, this.transform.position, Quaternion.identity), true));
            setHealth(this.health - 1);
            CheckIfGameOver();
            playCharacterHurtSound();
            this.poisonCount --;
            if(this.poisonCount < 1){
                StartCoroutine(AnimateFadeOut(this.inflictionIndicationImage));
            }
        }
    }

    private void keyFound() {
        StartCoroutine(AnimateFoundItem(Instantiate(this.foundKey, this.transform.position, Quaternion.identity), true));
        this.hasKey = true;
        this.foundKeyPanelImage.SetActive(this.hasKey);
    }
    public void hitFromBehind(int hurt){
        StartCoroutine(AnimateFoundItem(Instantiate(this.greySkull, this.transform.position, Quaternion.identity), true));
        setHealth(this.health - hurt);
        CheckIfGameOver();
        playCharacterHurtSound();
    }
    private bool blockedByItem(int horizontal, int vertical, int px, int py) {
        // check if a monster or chest is in the way
        Item i = GameManager.instance.findItem(horizontal, vertical, px, py);
        if (i != null) {
            if (this.heldItem != null && i.type > 99) {     // type 99 = monster
                if (this.heldItemType == 40) {
                    throwBomb(i);
                    return true;
                } else {
                    int hitStrength = i.value > this.heldItemValue ? this.heldItemValue: i.value;
                    bool rangerAdvantage = this.heldItemValue >= i.value;

                    // if knight, slash around if surrounded by 3 monsters
                    if (this.selectedCharacter == GameManager.character_knight){
                        Item[] monsters = GameManager.instance.findMonstersAround(px, py);
                        if (monsters != null){
                            hitAround(monsters, hitStrength);
                        } else {
                            // hit monster
                            doHit(i, false);
                        }
                    } else {
                        // hit monster
                        doHit(i, horizontal < 0);
                    }
 
                    // if ranger or wizard, hit second monster in a row. Ranger can only do so if the first monster is killed,
                    // Wizard will only hit second monster half strength
                    if (this.selectedCharacter == GameManager.character_ranger && rangerAdvantage){
                        doSecondHit(GameManager.instance.findSecondHit(horizontal, vertical, px, py), hitStrength);
                    } else if(this.selectedCharacter == GameManager.character_wizard){
                        doSecondHit(GameManager.instance.findSecondHit(horizontal, vertical, px, py), (int)(hitStrength / 2));
                    }
                    return true;
                }
            } else if (i.type == 30) {
                i.openChest();
                return true;
            }
        }
        return false;
    }
    public void getGemFromPerformance(){
        this.gemCount ++;
        this.gemAmountPanelText.text = this.gemCount.ToString();
        StartCoroutine(AnimateFoundItem(Instantiate(this.foundGem, this.transform.position, Quaternion.identity), true));
    }
    private void playCharacterAttackSound(){
        switch (this.selectedCharacter) {
            case GameManager.character_knight:
                SoundManager.instance.playFx1(playerSounds[Player.sound_attack_knight]);
                break;
            case GameManager.character_wizard:
                SoundManager.instance.playFx1(playerSounds[Player.sound_attack_wizard]);
                break;
            case GameManager.character_ranger:
                SoundManager.instance.playFx1(playerSounds[Player.sound_attack_ranger]);
                break;
            case GameManager.character_rogue:
                SoundManager.instance.playFx1(playerSounds[Player.sound_attack_rogue]);
                break;
            case GameManager.character_dwarf:
                SoundManager.instance.playFx1(playerSounds[Player.sound_attack_dwarf]);
                break;
        }
    }
    private void playCharacterHurtSound(){
        switch (this.selectedCharacter) {
            case GameManager.character_knight:
                SoundManager.instance.playFx1(playerSounds[Player.sound_hurt_knight]);
                break;
            case GameManager.character_wizard:
                SoundManager.instance.playFx1(playerSounds[Player.sound_hurt_wizard]);
                break;
            case GameManager.character_ranger:
                SoundManager.instance.playFx1(playerSounds[Player.sound_hurt_ranger]);
                break;
            case GameManager.character_rogue:
                SoundManager.instance.playFx1(playerSounds[Player.sound_hurt_rogue]);
                break;
            case GameManager.character_dwarf:
                SoundManager.instance.playFx1(playerSounds[Player.sound_hurt_dwarf]);
                break;
        }
    }
    private void doHit(Item i, bool rogueAdvantage) {
        this.animator.SetTrigger("PlayerAttack");
        playCharacterAttackSound();

        if (this.selectedCharacter == GameManager.character_rogue) {
            if (rogueAdvantage) {
                this.heldItemValue += 3;
            } else {
                this.heldItemValue -= 1;
            }
        }

        if (this.heldItemValue > i.value) {
            // remove those extra 3 hitpoints, but only if rogue and advantage
            i.isAlreadyDead = true;
            if (this.selectedCharacter == GameManager.character_rogue && i.value < 4 && rogueAdvantage) {
                this.heldItemValue -= 3;
            } else {
                this.heldItemValue -= i.value;
            }
            if(i.giveKey){
                keyFound();
            }
            StartCoroutine(SlashMonster(this.weaponSlash[this.selectedCharacter], i, true));
            if(this.selectedCharacter == GameManager.character_dwarf){
                addToDwarfAdvantage();
            }
        } else if (this.heldItemValue < i.value) {
            i.updateValue(i.value - this.heldItemValue);
            Destroy(this.heldItem);
            this.heldItem = null;
            StartCoroutine(SlashMonster(this.weaponSlash[this.selectedCharacter], i, false));
            // if death is hit, its timer is reset
            if(i.type == 115){
                i.updateTimer(i.timerStartValue);
            }
        } else {
            i.isAlreadyDead = true;
            Destroy(this.heldItem);
            this.heldItem = null;
            if(i.giveKey){
                keyFound();
            }
            StartCoroutine(SlashMonster(this.weaponSlash[this.selectedCharacter], i, true));
            if(this.selectedCharacter == GameManager.character_dwarf){
                addToDwarfAdvantage();
            }
        }

        if (this.heldItem != null) {
            this.heldItemValueText.text = this.heldItemValue.ToString();
        }
    }

    private void hitAround(Item [] monsters, int hitStrength) {
        this.animator.SetTrigger("PlayerAttack");
        
        // hit monsters around
        for(int i = 0; i < monsters.Length; i++){
            if (monsters[i] != null){
                if (hitStrength >= monsters[i].value) {
                    monsters[i].isAlreadyDead = true;
                    if(monsters[i].giveKey){
                        keyFound();
                    }
                    StartCoroutine(SlashMonster(this.weaponSlash[this.selectedCharacter], monsters[i], true));
                } else {
                    monsters[i].updateValue(monsters[i].value - hitStrength);
                    StartCoroutine(SlashMonster(this.weaponSlash[this.selectedCharacter], monsters[i], false));
                }
            }
        }

        // update heldweapon
        if (this.heldItemValue <= hitStrength) {
            Destroy(this.heldItem);
            this.heldItem = null;
        } else {
            this.heldItemValue -= hitStrength;
            this.heldItemValueText.text = this.heldItemValue.ToString();
        }
    }

    private void doSecondHit(Item i, int hitStrength) {
        if(i != null){
            if (hitStrength >= i.value) {
                i.isAlreadyDead = true;
                if(i.giveKey){
                    keyFound();
                }
                StartCoroutine(SlashMonster(this.weaponSlash[this.selectedCharacter], i, true));
                if(this.selectedCharacter == GameManager.character_dwarf){
                    addToDwarfAdvantage();
                }
            } else if (hitStrength < i.value) {
                i.updateValue(i.value - hitStrength);
                StartCoroutine(SlashMonster(this.weaponSlash[this.selectedCharacter], i, false));
            }
        }
    }
    private void addToDwarfAdvantage(){
        killEnemies ++;
        if (killEnemies > this.dwarfAdvantage){
            killEnemies = 0;
            Animator a = this.character.GetComponent<Animator>();
            a.SetTrigger("PlayerSpecial");
            StartCoroutine(DwarfAdvantage());
        }
    }

    private void throwBomb(Item i) {
        GameObject b = Instantiate(this.bomb, i.gameObject.transform.position, Quaternion.identity);
        Item bi = b.GetComponent<Item>();
        bi.hideFromList = true;
        bi.setShow(true, false);
        bi.value = this.heldItemValue;
        // calling countdown with timevalue 0 will detonate the bomb right away
        bi.timerValue = 0;
        bi.countDownBomb();
        Destroy(this.heldItem);
        this.heldItem = null;
    }

    private void Move(int x, int y) {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(x, y);
        StartCoroutine(SmoothMovement(end));
        // end players turn
        GameManager.instance.playersTurn = false;
        this.steps++;
    }

    private void updateGold(int gold){
        this.gold += (this.scoreMultiplier * gold);
        this.goldPanelText.text = this.gold.ToString();
        // you got a gem for hoarding gold
        bool extraGem = false;
        while (previousGemGold + this.gemGold < this.gold) {
            this.gemGold += (int)(this.gemGold / 11);
            previousGemGold += this.gemGold;
            this.gemCount ++;
            this.gemAmountPanelText.text = this.gemCount.ToString();
            extraGem = true; 
        }
        if(extraGem){
            StartCoroutine(AnimateFoundItem(Instantiate(this.foundGem, this.transform.position, Quaternion.identity), true));
        }
    }
    private void updateFacing(float h) {
        if (h > 0 && spriteRenderer.flipX) {
            spriteRenderer.flipX = false;
        } else if (h < 0 && !spriteRenderer.flipX) {
            spriteRenderer.flipX = true;
        }
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object
    private void OnTriggerEnter2D(Collider2D other) {
        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Exit") {
            if(!GameManager.instance.boardScript.isLocked || this.hasKey){

                GameManager.instance.gameProgress.gold = this.gold;
                GameManager.instance.gameProgress.steps = this.steps;
                if (this.health > this.baseHealth){
                    this.health = this.baseHealth;
                }
                GameManager.instance.gameProgress.health = this.health;
                GameManager.instance.gameProgress.maxHealth = this.maxHealth;
                GameManager.instance.gameProgress.baseHealth = this.baseHealth;
                GameManager.instance.gameProgress.scoreMultiplier = 1;
                GameManager.instance.gameProgress.gems = this.gemCount;

                GameManager.instance.saveGameState();
                //Disable the player object since level is over.
                this.enabled = false;

                // exit
                if (this.hasKey){
                    SoundManager.instance.playFxClip(SoundManager.location_portcullis);
                    StartCoroutine(exitLevel(2f));
                } else {
                    StartCoroutine(exitLevel(0.5f));
                }
            }

        }
        //Check if the tag of the trigger collided with is Enemy.
        else if (other.tag == "Enemy") {
            Item e = other.gameObject.GetComponent<Item>();
            e.isAlreadyDead = true;
            if(e.giveKey){
                keyFound();
            }
            if (e.type == 101){
                // fred the sheep gives 1 health if 'hit' and also cures poison
                if (this.poisonCount > 0){
                    this.poisonCount = 0;
                    StartCoroutine(AnimateFadeOut(this.inflictionIndicationImage));
                }
                setHealth(this.health + 1);
                SoundManager.instance.playFxClip(SoundManager.locaction_sheep);
                e.ItemFoundFeedback(true);

            } else if (e.type == 107){
                // spider sebastian could poison you
                if(Random.value < 0.5f){
                    if(this.poisonCount < 1){
                        this.poisonCount = 3;
                        StartCoroutine(AnimateFadeIn(this.inflictionIndicationImage));
                    }
                } 
                setHealth(this.health - e.value);
                CheckIfGameOver();
                playCharacterHurtSound();
                Destroy(other.gameObject);
            } else {
                setHealth(this.health - e.value);
                CheckIfGameOver();
                playCharacterHurtSound();
                Destroy(other.gameObject);
            }
            updateGold(e.baseValue);

            //CheckIfGameOver();
            if(this.selectedCharacter == GameManager.character_dwarf){
                addToDwarfAdvantage();
            }
            
        } else if (other.tag == "Item") {
            Item i = other.gameObject.GetComponent<Item>();
            switch (i.type) {
                case 0:
                case 1:
                    // health, cures poison
                    if(this.poisonCount > 0){
                        this.poisonCount = 0;
                        StartCoroutine(AnimateFadeOut(this.inflictionIndicationImage));
                    }
                    setHealth(this.health + i.value);
                    SoundManager.instance.playFxClip(SoundManager.location_gulp);
                    i.ItemFoundFeedback(true);
                    break;
                case 2:
                case 5:
                    // poison
                    if(this.poisonCount < 1){
                        this.poisonCount = i.value;
                        StartCoroutine(AnimateFadeIn(this.inflictionIndicationImage));
                    }
                    SoundManager.instance.playFxClip(SoundManager.location_gulp);
                    i.ItemFoundFeedback(true);
                    break;
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                    // weapon
                    attachWeaponToSelf(i.type, i.value + this.playerStrength);
                    Destroy(other.gameObject);
                    break;
                case 31:
                    //coin
                    updateGold(i.value);
                    Destroy(other.gameObject);
                    SoundManager.instance.playFxClip(SoundManager.location_grab_coin);
                    break;
                case 32:
                    // you found a gem!
                    this.gemCount ++;
                    this.gemAmountPanelText.text = this.gemCount.ToString();
                    i.ItemFoundFeedback(true);
                    SoundManager.instance.playFxClip(SoundManager.location_grab_gem);
                    break;
                case 33:
                    // voici les clés
                    Destroy(other.gameObject);
                    keyFound();
                break;
                case 40:
                    // bomb
                    attachBombToSelf(i.type, i.value);
                    Destroy(other.gameObject);
                    break;
            }
        }
    }
    public void CheckIfGameOver() {
        if (this.health <= 0) {
            if (this.extraLife) {
                this.extraLife = false;
                this.extraLifePanelImage.SetActive(this.extraLife);
                setHealth(this.baseHealth);
                this.poisonCount = 0;
                this.inflictionIndicationImage.SetActive(false);
                Instantiate(this.assetSpawnParticles, this.transform.position, Quaternion.identity);
            } else {
                Animator a = this.character.GetComponent<Animator>();
                a.SetTrigger("PlayerDeath");
                StartCoroutine(Dying());
            }
        }
    }
    protected IEnumerator SmoothMovement(Vector3 end) {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon) {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            this.rb2D.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
        GameManager.instance.playersTurnEnd = true;
    }
    protected IEnumerator AnimateFoundItem(GameObject go, bool killObject) {
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

        while(3 > go.transform.localScale.x) {
            go.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * 7;
            go.transform.position += new Vector3(0, 0.11f, 0) ;
            sr.color -= new Color (0, 0, 0, 0.05f);
            yield return null;
        }
        if(killObject){
            Destroy(go);
        }
    }
    protected IEnumerator SlashMonster(GameObject hit, Item monster, bool destroyMonster) {
        Instantiate(hit, monster.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        if (destroyMonster) {
            monster.ItemSpawnCoin();
        }
    }
    protected IEnumerator Dying() {
        GameManager.instance.deleteGameState();
        yield return new WaitForSeconds(1f);
        GameManager.instance.GameOver();
    }
    protected IEnumerator DwarfAdvantage() {
        yield return new WaitForSeconds(1f);
        setHealth(this.health + 2);
    }

    protected IEnumerator exitLevel(float pauze){
        GameManager.instance.boardScript.instantiatedExit.GetComponent<Animator>().SetTrigger("OpenExit");
        yield return new WaitForSeconds(pauze);
        SceneManager.LoadScene(1);
    }

    protected IEnumerator AnimateFadeIn(GameObject go) {
        go.SetActive(true);
        Image i = go.GetComponent<Image>();
        i.color = new Color(1f,1f,1f,0f);

        while (i.color.a < 0.9f) {
            i.color += new Color(0, 0, 0, 0.05f);
            yield return null;
        }
    }
    protected IEnumerator AnimateFadeOut(GameObject go) {
        Image i = go.GetComponent<Image>();
        i.color = new Color(1f,1f,1f,1f);

        while (i.color.a > 0f) {
            i.color -= new Color(0, 0, 0, 0.05f);
            yield return null;
        }
        go.SetActive(false);
    }
}