﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour {
    [HideInInspector] public int baseValue = 1; // the amount of coins to the player after defeating the monster, or damage a bom causes
    public int minValue = 1;                    // minValue is randomized by valueRandomizer, the result is affected by timeValueMultiplier and playertime
    public int maxValue = 10;                   // health of monster, value of coin
    public int minLevel = 1;                    // minlevel the monster will appear.
    public int maxLevel = -1;                   // maxlevel the monster will appear, -1 means all
    public int timerStartValue = 10;            // bombtimer. also zombiespawner and unicorn shooter position
    [HideInInspector] public int timerValue = 10;                // bushfire, tree and necromancer spreadrate
    public int valueRandomizer = 1;
    public float timeValueMultiplier = 0.1f;
    public bool showValue = true;
    public bool giveKey = false;                    // if true this item gives a key of the exit to the player
    public bool indestructible = false;             // item cannot be detroyed by bombs
    public GameObject coin;
    public GameObject tempObject;
    public GameObject foundFeedbackObject;
    public GameObject spawnObject;
    public GameObject specialFx;
    public Canvas valuesCanvas;
    public Canvas nameCanvas;
    [HideInInspector] public GameObject[] monsters;     // needed for the chest
    [HideInInspector] public GameObject[] items;        // needed for the chest
    [HideInInspector] public int value;
    [HideInInspector] public bool hideFromList = false;
    [HideInInspector] public bool isAlreadyDead = false;
    private Animator animator;
    public AudioClip invocationSound;

    /*types of things on the map
		0: restore health potion (red)
        1: unknown potion, health
		2: poison potion
		3: 
		4: 
        5: unknown potion, poison
		...
		20: sword
		21: staff
		22: arrow
		23: knife
		24: axe
		...
		30: chest
		31: coin
        32: gem
        33: key
		...
		40: bomb
		...
		100: standard enemy
        101: sheep                  - gives health if not hit with weapon
        102: bushfire               - burns items
        103: zombie                 - hits from the right
        104: Evil Larry             - spawns zombies and weapons
        105: Count de Nostradame    - steals health from player
        106: Prancer                - unicorn shoots rainbows
        107: sebastian              - 50% chance of poison if hit with bare hands
        108: wasp                   - 50% chance of stinging you if you walk past
        109: egg                    - will hatch into snakewoman
        110: snakewoman             - comes out of an egg
        111: small tree             - grows into medium tree
        112: medium tree            - grows into large tree
        113: large tree
        114: Bunny                  - breeder
        115: DEATH                  - steals health from everyone
		...
	*/
    private Text values;
    public bool show;
    public GameObject spawnParticles;
    public int type;

    void Start() {
        this.timerValue = this.timerStartValue;
        this.values = this.valuesCanvas.GetComponent<Text>();
        // unicorn random name
        if (this.type == 106 && this.nameCanvas != null){
            String [] unicornNames = {"Prancer", "Stardust", "Sparkles", "Serenity", "Midnight", "Sapphire", "Aeolus", "Celeste", "Snowflake"};
            Text name = this.nameCanvas.GetComponent<Text>();
            name.text = unicornNames[Random.Range(0, unicornNames.Length)];
        }

        if (this.showValue) {
            this.values.text = this.value.ToString();
        }

        if (!this.hideFromList) {
            GameManager.instance.AddItemToList(this);
        } else {
            initValue(0);
        }

        this.animator = this.transform.GetChild(0).GetComponent<Animator>();
    }
    void OnDisable() {
        if (this.tempObject != null){
            Destroy(this.tempObject);
        }
        if (this.type > 99) {
            GameManager.instance.activeMonsters--;
            //Debug.Log("active monsters: " + GameManager.instance.activeMonsters);
        }
    }
    public void finishFullInit(GameObject[] m, GameObject[] i) {
        // these lists are needed for chests
        this.monsters = m;
        this.items = i;
        initValue(GameManager.instance.gameProgress.steps);
        this.show = false;
    }
    private Text getChildTextComponent() {
        // find the right child by finding its tag
        for (int i = 0; i < this.transform.childCount; i++) {
            if (this.transform.GetChild(i).gameObject.tag == "CardCanvas") {
                return this.transform.GetChild(i).gameObject.GetComponent<Text>();
            }
        }
        return null;
    }
    public void setShow(bool particles, bool silent) {
        if (particles) {
            Instantiate(spawnParticles, this.transform.position, this.transform.rotation);
        }

        for (int i = 0; i < this.transform.childCount; i++) {
            this.transform.GetChild(i).gameObject.SetActive(true);
        }
        this.show = true;

        if (this.invocationSound != null && !silent) {
            if (this.type < 100) {
                SoundManager.instance.playFx2(this.invocationSound);
            } else {
                SoundManager.instance.playFx1(this.invocationSound);
            }
        }
    }
    // enemy spawns coin, chest spawns random, some enemies spawn other enemies
    public void ItemSpawnCoin() {
        // spawn coin
        GameObject coin = Instantiate(this.coin, this.transform.position, Quaternion.identity);
        Item i = coin.GetComponent<Item>();
        i.setShow(true, false);
        i.value = this.baseValue;
        // die
        //GameManager.instance.RemoveItemFromList(this);
        Destroy(this.gameObject);
    }
    // for feedback purposes show some animation
    public void ItemFoundFeedback(bool killThis) {
        StartCoroutine(AnimateFoundItem(Instantiate(this.foundFeedbackObject, this.transform.position, Quaternion.identity), killThis));
    }
    public void ChestSpawnRandom() {
        GameObject item;
        Item i;
        int choice = Random.Range(0, this.baseValue);
        if (choice < 6) {
            item = Instantiate(this.coin, this.transform.position, Quaternion.identity);
        } else if (choice < 15) {
            item = Instantiate(this.items[Random.Range(0, this.items.Length)], this.transform.position, Quaternion.identity);
        } else {
            item = Instantiate(this.monsters[Random.Range(0, this.items.Length)], this.transform.position, Quaternion.identity);
        }
        i = item.GetComponent<Item>();
        i.setShow(true, false);
        i.value = this.baseValue;
        GameManager.instance.AddItemToList(i);
        Destroy(this.gameObject);
    }
    public void initValue(int playerSteps) {
        this.value = Random.Range(this.minValue, this.minValue + valueRandomizer + (int)(timeValueMultiplier * playerSteps));
        if (this.value > this.maxValue) {
            this.value = this.maxValue;
        }
        this.baseValue = this.value;
        if (this.values != null && this.showValue) {
            this.values.text = this.value.ToString();
        }
    }
    public void updateValue(int value) {
        this.value = value;
        if (this.showValue) {
            // bomb or death
            if (this.type == 40 || this.type == 115){
                this.values.text = this.timerValue + "/" + this.value;
            } else {
                this.values.text = this.value.ToString();
            }
        }
    }
    public void updateTimer(int timer) {
        this.timerValue = timer;
        if (this.values !=null && this.showValue) {
            this.values.text = this.timerValue + "/" + this.value;
        }
    }
    public void countDownBomb() {
        if (this.timerValue > 0){
            updateTimer(--this.timerValue);
        } else if (this.timerValue == 0) {
            this.timerValue = -1;
            Animator a = findComponentInChildren<Animator>(this.gameObject);
            a.SetTrigger("AnimateItem");
            SoundManager.instance.playFxClip(SoundManager.location_bomb);
            StartCoroutine(AnimateBombExplode());
            GameManager.instance.player.gameCamera.GetComponent<CameraShake>().ShakeCamera(5, 1f);
        }
    }
    public void countDownSpread() {
        this.timerValue--;
        if (this.timerValue == 0) {
            this.timerValue = this.timerStartValue;
            bushFire();
        }
    }
    public void zombieHit(){
        // check if player to the left
        int x = (int)(this.gameObject.transform.position.x / BoardManager.wMultiplier);
        int y = (int)(this.gameObject.transform.position.y / BoardManager.hMultiplier);

        Player p = GameManager.instance.player;
        int px = (int)(GameManager.instance.player.gameObject.transform.position.x / BoardManager.wMultiplier);
        int py = (int)(GameManager.instance.player.gameObject.transform.position.y / BoardManager.hMultiplier);
        if (px == (x - 1) && py == y){
            p.hitFromBehind(1);
        }
    }
    public void vampireSuck(){
        // check if player around vampire
        int x = (int)(this.gameObject.transform.position.x / BoardManager.wMultiplier);
        int y = (int)(this.gameObject.transform.position.y / BoardManager.hMultiplier);

        Player p = GameManager.instance.player;
        int px = (int)(GameManager.instance.player.gameObject.transform.position.x / BoardManager.wMultiplier);
        int py = (int)(GameManager.instance.player.gameObject.transform.position.y / BoardManager.hMultiplier);
        if (px == (x - 1) && py == y || 
            px == (x + 1) && py == y ||
            px == x && py == (y - 1) ||
            px == x && py == (y + 1)){
            //transfer 1 health to vampire
            updateValue(this.value + 1);
            ItemFoundFeedback(false);
            p.hitFromBehind(1);
        }
    }
    public void waspSting(){
        // check if player is around wasp but sting only half of the time
        if(Random.value < 0.5f){
            int x = (int)(this.gameObject.transform.position.x / BoardManager.wMultiplier);
            int y = (int)(this.gameObject.transform.position.y / BoardManager.hMultiplier);

            Player p = GameManager.instance.player;
            int px = (int)(GameManager.instance.player.gameObject.transform.position.x / BoardManager.wMultiplier);
            int py = (int)(GameManager.instance.player.gameObject.transform.position.y / BoardManager.hMultiplier);
            if (px == (x - 1) && py == y || 
                px == (x + 1) && py == y ||
                px == x && py == (y - 1) ||
                px == x && py == (y + 1)){
                this.animator.SetTrigger("Attack");
                p.hitFromBehind(1);
            }
        }
    }
    public void waitEgg(){
        this.timerValue--;
        if(this.timerValue == 0){
            this.timerValue = -1;
            StartCoroutine(AnimateEggHatching());
        }
    }
    private void hatchEgg(){
        GameObject item = Instantiate(this.spawnObject, this.transform.position, Quaternion.identity);
        Item i = item.GetComponent<Item>();
        i.setShow(true, false);
        i.value = this.baseValue + 10;
        GameManager.instance.AddItemToList(i);
        Destroy(this.gameObject);
    }

    public void growTree(bool breed){
        this.timerValue--;
        if(breed){
            if(this.timerValue == 0){
                this.timerValue = this.timerStartValue;
                plantTree();
            }
        } else {
            if(this.timerValue == 0) {
                this.timerValue = -1;
                GameObject item = Instantiate(this.spawnObject, this.transform.position, Quaternion.identity);
                Item i = item.GetComponent<Item>();
                i.setShow(true, false);
                i.value = i.baseValue = this.baseValue + 3;
                GameManager.instance.AddItemToList(i);
                Destroy(this.gameObject);
            }
        }
    }
    public void shootRainbows(){

        int x = (int)(this.gameObject.transform.position.x / BoardManager.wMultiplier);
        int y = (int)(this.gameObject.transform.position.y / BoardManager.hMultiplier);

        Player p = GameManager.instance.player;
        int px = (int)(GameManager.instance.player.gameObject.transform.position.x / BoardManager.wMultiplier);
        int py = (int)(GameManager.instance.player.gameObject.transform.position.y / BoardManager.hMultiplier);

        Vector3 newPosition = this.transform.position;

        this.timerValue --;
        if(this.timerValue < 0){
            this.timerValue = 3;
        }
        bool drawRainbow = false;
        
        switch (timerValue){
            case 0:
            // top
            if (y > 0){
                if(GameManager.instance.itemMap[x,y-1] == null){
                    newPosition.y = newPosition.y - BoardManager.hMultiplier;
                    drawRainbow = true;
                    if(px == x && py == y - 1){
                        p.hitFromBehind(1);
                    }
                }
            }
            break;
            case 1:
            // right
            if (x < GameManager.instance.boardScript.columns - 1){
                if(GameManager.instance.itemMap[x + 1,y] == null){
                    newPosition.x = newPosition.x + BoardManager.wMultiplier;
                    drawRainbow = true;
                    if(px == x + 1 && py == y){
                        p.hitFromBehind(1);
                    }
                }
            }
            break;
            case 2:
            // bottom
            if (y < GameManager.instance.boardScript.rows - 1){
                if(GameManager.instance.itemMap[x,y + 1] == null){
                    newPosition.y = newPosition.y + BoardManager.hMultiplier;
                    drawRainbow = true;
                    if(px == x && py == y + 1){
                        p.hitFromBehind(1);
                    }
                }
            }
            break;
            case 3:
            // left
            if (x > 0){
                if(GameManager.instance.itemMap[x - 1,y] == null){
                    newPosition.x = newPosition.x - BoardManager.wMultiplier;
                    drawRainbow = true;
                    if(px == x - 1 && py == y){
                        p.hitFromBehind(1);
                    }
                }
            }
            break;
        }
        if(drawRainbow){
            this.tempObject = Instantiate(this.foundFeedbackObject, this.transform.position, Quaternion.identity);
            StartCoroutine(SmoothMovement(this.tempObject, newPosition));
            
            //StartCoroutine(AnimateBrieflyAppearing(this.tempObject));
        }
    }
    public void countDownZombieSpawn(){
        this.timerValue--;
        if (this.timerValue == 0) {
            this.timerValue = this.timerStartValue;
            GameManager.instance.boardScript.spawnZombieSpawn();
            SoundManager.instance.playFxClip(SoundManager.location_zombieSpawn);
        }
    }

    public void feastUponTheLiving(){
        this.updateTimer(--this.timerValue);
        if (this.timerValue == 0) {

            Instantiate(this.specialFx, this.transform.position, Quaternion.identity);
            // steal health from monsters and objects
            Item[] targets = new Item[12];
            int pointer = 0;

            int x = (int)(this.gameObject.transform.position.x / BoardManager.wMultiplier);
            int y = (int)(this.gameObject.transform.position.y / BoardManager.hMultiplier);

            int px = (int)(GameManager.instance.player.gameObject.transform.position.x / BoardManager.wMultiplier);
            int py = (int)(GameManager.instance.player.gameObject.transform.position.y / BoardManager.hMultiplier);

            pointer = addToAvailableMonsters(targets, x - 1, y, pointer);
            pointer = addToAvailableMonsters(targets, x - 1, y -1, pointer);
            pointer = addToAvailableMonsters(targets, x - 1, y + 1, pointer);

            pointer = addToAvailableMonsters(targets, x, y - 1, pointer);
            pointer = addToAvailableMonsters(targets, x, y + 1, pointer);

            pointer = addToAvailableMonsters(targets, x + 1, y -1, pointer);
            pointer = addToAvailableMonsters(targets, x + 1, y, pointer);
            pointer = addToAvailableMonsters(targets, x + 1, y + 1, pointer);

            pointer = addToAvailableMonsters(targets, x - 2, y, pointer);
            pointer = addToAvailableMonsters(targets, x + 2, y, pointer);
            pointer = addToAvailableMonsters(targets, x, y - 2, pointer);
            pointer = addToAvailableMonsters(targets, x, y + 2, pointer);

            for (int i=0; i<targets.Length; i++){
                if(targets[i] != null  && !targets[i].isAlreadyDead){
                    targets[i].updateValue(targets[i].value - 1);
                    if(targets[i].value < 1){
                        targets[i].isAlreadyDead = true;
                        targets[i].ItemSpawnCoin();
                    } else {
                        StartCoroutine(AnimateFoundItem(Instantiate(this.foundFeedbackObject, targets[i].gameObject.transform.position, Quaternion.identity), false));
                    }
                    this.value ++;
                }
            }
            // steal health from player
            if(px - x > -2 && px - x < 2 && py - y > -2 && py - y < 2){
                GameManager.instance.player.hitFromBehind(2);
                this.value ++;
                GameManager.instance.player.CheckIfGameOver();
            }
            if(this.value > this.maxValue){
                this.value = this.maxValue;
            }
            updateTimer(this.timerStartValue);
        }
    }

    private void bushFire() {
        Item[] targets = new Item[4];
        int pointer = 0;
        int x = (int)(this.gameObject.transform.position.x / BoardManager.wMultiplier);
        int y = (int)(this.gameObject.transform.position.y / BoardManager.hMultiplier);
        pointer = addToAvailableItems(targets, x - 1, y, pointer);
        pointer = addToAvailableItems(targets, x + 1, y, pointer);
        pointer = addToAvailableItems(targets, x, y - 1, pointer);
        pointer = addToAvailableItems(targets, x, y + 1, pointer);

        if (pointer > 0) {
            int choice = Random.Range(0, pointer);
            GameObject newFire = Instantiate(this.gameObject, targets[choice].gameObject.transform.position, this.transform.rotation);
            Item it = newFire.GetComponent<Item>();
            it.timerValue = this.timerValue;
            //replace item with bushfire
            Destroy(targets[choice].gameObject);
        }
    }

    public void breed(){
        this.timerValue--;
        if (this.timerValue == 0) {
            this.timerValue = 4;
            int x = (int)(this.gameObject.transform.position.x / BoardManager.wMultiplier);
            int y = (int)(this.gameObject.transform.position.y / BoardManager.hMultiplier);

            int px = (int)(GameManager.instance.player.gameObject.transform.position.x / BoardManager.wMultiplier);
            int py = (int)(GameManager.instance.player.gameObject.transform.position.y / BoardManager.hMultiplier);

            bool isPositionAvailable = false;
            Vector3 emptyPos = new Vector3(x * BoardManager.wMultiplier, y * BoardManager.hMultiplier, 0f);

            if (x < GameManager.instance.boardScript.columns - 1 && GameManager.instance.itemMap[x + 1,y] == null && !(px == x+1 && py == y)){
                isPositionAvailable = true;
                emptyPos.x = (x+1) * BoardManager.wMultiplier;
            } else if (y < GameManager.instance.boardScript.rows - 1 && GameManager.instance.itemMap[x,y + 1] == null  && !(px == x && py == y+1)){
                isPositionAvailable = true;
                emptyPos.y = (y+1) * BoardManager.hMultiplier;
            } else if (y > 0 && GameManager.instance.itemMap[x,y-1] == null  && !(px == x && py == y-1)){
                isPositionAvailable = true;
                emptyPos.y = (y-1) * BoardManager.hMultiplier;
            } else if (x > 0 && GameManager.instance.itemMap[x - 1,y] == null  && !(px == x-1 && py == y)){
                isPositionAvailable = true;
                emptyPos.x = (x-1) * BoardManager.wMultiplier;
            }

            if (isPositionAvailable) {
                GameObject newMonster = Instantiate(this.gameObject, emptyPos, this.transform.rotation);
                Item it = newMonster.GetComponent<Item>();
                it.baseValue = it.value = 2;
                it.timerValue = 4;
                it.setShow(true, false);
            }
        }
    }
    private void plantTree() {
        
        int x = (int)(this.gameObject.transform.position.x / BoardManager.wMultiplier);
        int y = (int)(this.gameObject.transform.position.y / BoardManager.hMultiplier);

        int px = (int)(GameManager.instance.player.gameObject.transform.position.x / BoardManager.wMultiplier);
        int py = (int)(GameManager.instance.player.gameObject.transform.position.y / BoardManager.hMultiplier);

        bool isPositionAvailable = false;
        Vector3 treePos = new Vector3(x * BoardManager.wMultiplier, y * BoardManager.hMultiplier, 0f);

        if (x < GameManager.instance.boardScript.columns - 1 && GameManager.instance.itemMap[x + 1,y] == null && !(px == x+1 && py == y)){
            isPositionAvailable = true;
            treePos.x = (x+1) * BoardManager.wMultiplier;
        } else if (y < GameManager.instance.boardScript.rows - 1 && GameManager.instance.itemMap[x,y + 1] == null  && !(px == x && py == y+1)){
            isPositionAvailable = true;
            treePos.y = (y+1) * BoardManager.hMultiplier;
        } else if (y > 0 && GameManager.instance.itemMap[x,y-1] == null  && !(px == x && py == y-1)){
            isPositionAvailable = true;
            treePos.y = (y-1) * BoardManager.hMultiplier;
        } else if (x > 0 && GameManager.instance.itemMap[x - 1,y] == null  && !(px == x-1 && py == y)){
            isPositionAvailable = true;
            treePos.x = (x-1) * BoardManager.wMultiplier;
        }

        if (isPositionAvailable) {
            GameObject newTree = Instantiate(this.spawnObject, treePos, this.transform.rotation);
            Item it = newTree.GetComponent<Item>();
            it.baseValue = it.value = 4;
            it.timerValue = this.timerValue;
            it.setShow(true, false);
        }
    }

    private int addToAvailableItems(Item[] targets, int x, int y, int pointer) {
        if (x > -1 && x < GameManager.instance.boardScript.columns && y > -1 && y < GameManager.instance.boardScript.rows) {
            Item i = GameManager.instance.itemMap[x, y];
            if (i != null && i.show && i.type < 100) {
                targets[pointer++] = i;
            }
        }
        return pointer;
    }

        private int addToAvailableMonsters(Item[] targets, int x, int y, int pointer) {
        if (x > -1 && x < GameManager.instance.boardScript.columns && y > -1 && y < GameManager.instance.boardScript.rows) {
            Item i = GameManager.instance.itemMap[x, y];
            if (i != null && i.show && i.type > 99) {
                targets[pointer++] = i;
            }
        }
        return pointer;
    }

    public void bombExplode() {
        int x = (int)(this.gameObject.transform.position.x / BoardManager.wMultiplier);
        int y = (int)(this.gameObject.transform.position.y / BoardManager.hMultiplier);

        // items
        explode(x - 1, y - 1);
        explode(x - 1, y);
        explode(x - 1, y + 1);
        explode(x + 1, y - 1);
        explode(x + 1, y);
        explode(x + 1, y + 1);
        explode(x, y - 1);
        explode(x, y);
        explode(x, y + 1);

        //player
        Player p = GameManager.instance.player;
        int px = (int)(GameManager.instance.player.gameObject.transform.position.x / BoardManager.wMultiplier);
        int py = (int)(GameManager.instance.player.gameObject.transform.position.y / BoardManager.hMultiplier);
        if (Math.Abs(px - x) < 2 && Math.Abs(py - y) < 2) {

            p.setHealth(p.health - (int)(this.value / 2));
        }
        Destroy(this.gameObject);
    }
    private void explode(int x, int y) {
        if (x > -1 && x < GameManager.instance.boardScript.columns && y > -1 && y < GameManager.instance.boardScript.rows) {
            Item i = GameManager.instance.itemMap[x, y];
            if (i != null && !i.indestructible) {
                // coin does not spawn coin
                if (i.type != 31) {
                    if (!i.show) {
                        i.setShow(true, false);
                    } else {
                        if (this.value >= i.value) {
                            i.isAlreadyDead = true;
                            i.ItemSpawnCoin();
                        } else {
                            i.updateValue(i.value - this.value);
                        }
                    }
                } else {
                    int newValue = i.value - this.value;
                    if (newValue < 1) {
                        newValue = 1;
                    }
                    i.updateValue(newValue);
                }
            }
        }
    }
    private T findComponentInChildren<T>(GameObject go) {
        // find the right child by finding its tag
        for (int i = 0; i < go.transform.childCount; i++) {
            T comp = go.transform.GetChild(i).gameObject.GetComponent<T>();
            if (comp != null) {
                return comp;
            }
        }
        return default(T);
    }
    public void openChest() {
        Animator a = findComponentInChildren<Animator>(this.gameObject);
        a.SetTrigger("AnimateItem");
        StartCoroutine(AnimateOpenChest());
    }
    protected IEnumerator AnimateOpenChest() {
        yield return new WaitForSeconds(0.3f);
        ChestSpawnRandom();
    }
    protected IEnumerator AnimateBombExplode() {
        yield return new WaitForSeconds(0.3f);
        bombExplode();
    }
    protected IEnumerator AnimateEggHatching() {
        Animator a = findComponentInChildren<Animator>(this.gameObject);
        a.SetTrigger("Hatching");
        SoundManager.instance.playFxClip(SoundManager.location_cracking);
        yield return new WaitForSeconds(1.4f);
        hatchEgg();
    }
    protected IEnumerator AnimateFoundItem(GameObject go, bool killThis) {
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

        while (3 > go.transform.localScale.x) {
            go.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * 7;
            go.transform.position += new Vector3(0, 0.11f, 0);
            sr.color -= new Color(0, 0, 0, 0.05f);
            yield return null;
        }
        Destroy(go);
        if (killThis) {
            Destroy(this.gameObject);
        }
    }
    protected IEnumerator AnimateBrieflyAppearing(GameObject go) {
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

        sr.color = new Color(1f,1f,1f,0f);

        while (sr.color.a < 0.9f) {
            sr.color += new Color(0, 0, 0, 0.1f);
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        while (sr.color.a > 0f) {
            sr.color -= new Color(0, 0, 0, 0.1f);
            yield return null;
        }
        Destroy(go);
    }
    
    protected IEnumerator SmoothMovement(GameObject go, Vector3 end) {
        float inverseMoveTime = 12f;
        Rigidbody2D rb2D = go.GetComponent<Rigidbody2D>();
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        float sqrRemainingDistance = (go.transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPostion);
            sqrRemainingDistance = (go.transform.position - end).sqrMagnitude;
            sr.color -= new Color(0, 0, 0, 0.01f);
            yield return null;
        }
        Destroy(go);
    }
}