using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

public class BoardManager : MonoBehaviour {
    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class Count {
        public int minimum;             //Minimum value for our Count class.
        public int maximum;             //Maximum value for our Count class.

        //Assignment constructor.
        public Count(int min, int max) {
            minimum = min;
            maximum = max;
        }
    }

	private const int
		// the location refers to position in the items- and monster array.
		location_chest = 0,
		location_health = 1,
		location_bomb = 2,
		location_coin = 3,
        location_poison = 4,
		location_unknown_potion_poison = 5,
		location_unknown_potion_health = 6,

		location_lowMonsters = 4,
		location_midMonsters = 6,
		location_highMonsters = 4,
        location_bushMonster = 6,
        location_zombie = 7,
        location_larry = 8,
        location_vampire = 9,
        location_unicorn = 10,
        location_wasp = 5,
        location_spider = 4;

    [HideInInspector] public int columns = 5;
    [HideInInspector] public int rows = 6;
    [HideInInspector] public int activemonsters;
    [HideInInspector] public int levelType;
    [HideInInspector] public bool isLocked;

    public static int wMultiplier = 2;
    public static int hMultiplier = 3;

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] itemTiles;                                  // potions, chests, treasure, poison, bombs
    public GameObject[] weapons;
    public GameObject[] monsterTiles;                               // also items but treated differently
    public GameObject gemTile;                                      // There can only be one, so this item cannot be added to the itemtiles
    public GameObject keyTile;                                      // There can only be one, so this item cannot be added to the itemtiles
    public GameObject exit;
    [HideInInspector] public GameObject instantiatedExit;
    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
    private List<Vector3> gridPositions = new List<Vector3>();   //A list of possible locations to place tiles.
    private GameObject extraGem;

    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList() {
        gridPositions.Clear();
        for (int x = 0; x < this.columns; x++) {
            for (int y = 0; y < this.rows; y++) {
                gridPositions.Add(new Vector3(x * wMultiplier, y * hMultiplier, 0f));
            }
        }
    }

    public void refreshGridpositions(){
        Item [,] itemMap = GameManager.instance.itemMap;
        this.gridPositions.Clear();

        // exclude exit, exclude player position
        int exitx = itemMap.GetLength(0) -1;
        int exity = itemMap.GetLength(1) -1;
        int px = (int)(GameManager.instance.player.transform.position.x / BoardManager.wMultiplier);
        int py = (int)(GameManager.instance.player.transform.position.y / BoardManager.hMultiplier);

        for(int i=0; i < itemMap.GetLength(0); i++){
            for(int j=0; j < itemMap.GetLength(1); j++){
                if(!(i == exitx && j == exity) && !(i == px && j == py)){
                    if(itemMap[i,j] == null){
                        gridPositions.Add(new Vector3(i * wMultiplier, j * hMultiplier, 0f));
                    }
                }
            }
        }
    }

    public void spawnZombieSpawn() {
        refreshGridpositions();
        if(this.gridPositions.Count > 1){
            // add weapon
            GameObject w = Instantiate(setCorrectWeapon(), RandomPosition(), Quaternion.identity);
            w.GetComponent<Item>().finishFullInit(null, null);
            // add zombie
            GameObject z = Instantiate(this.monsterTiles[BoardManager.location_zombie], RandomPosition(), Quaternion.identity);
            z.GetComponent<Item>().finishFullInit(null, null);
        }
    }

    //Sets up the outer walls and floor (background) of the game board.
    void boardSetup() {
        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < this.columns + 1; x++) {
            for (int y = -1; y < this.rows + 1; y++) {
                GameObject toInstantiate;
                if (x == -1 || x == this.columns || y == -1 || y == rows) {
                    /*
						top left,
						top-right,
						left,
						right,
						top1 - top4
					 */
                    if (x == -1 && y == -1) {
                        toInstantiate = wallTiles[Random.Range(4, 8)];
                    } else if (x == -1 && y == rows) {
                        toInstantiate = wallTiles[0];
                    } else if (x == this.columns && y == -1) {
                        toInstantiate = wallTiles[Random.Range(4, 8)];
                    } else if (x == this.columns && y == rows) {
                        toInstantiate = wallTiles[1];
                    } else if (x == -1) {
                        toInstantiate = wallTiles[2];
                    } else if (x == this.columns) {
                        toInstantiate = wallTiles[3];
                    } else {
                        toInstantiate = wallTiles[Random.Range(4, 8)];
                    }
                } else {
                    int tileNr = Random.Range(0, floorTiles.Length * 2);
                    if (tileNr >= floorTiles.Length) {
                        tileNr = 0;
                    }
                    toInstantiate = floorTiles[tileNr];
                }

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                GameObject instance = Instantiate(toInstantiate, new Vector3(x * wMultiplier, y * hMultiplier, 0f), Quaternion.identity) as GameObject;
                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent(boardHolder);
            }
        }
    }
    private GameObject setCorrectWeapon() {
        GameObject weapon = null;
        int weaponNumber = 0;
        switch (GameManager.instance.dataController.selectedCharacter) {
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
            Item it = weapons[i].GetComponent<Item>();
            if (it.type == weaponNumber) {
                weapon = weapons[i];
                break;
            }
        }
        return weapon;
    }

    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition() {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        //Remove the entry at randomIndex from the list so that it can't be re-used.
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    private void AddTiles(GameObject tileChoice, int amount) {
        for (int i = 0; i < amount; i++) {
            Instantiate(tileChoice, RandomPosition(), Quaternion.identity);
        }
    }

    private GameObject[] restrictLevel(GameObject[] tileArray, int level){
        List<GameObject> restricted = new List<GameObject>();

        for (int i=0; i< tileArray.Length; i++){
            Item it = tileArray[i].GetComponent<Item>();
            if((it.minLevel == -1 || it.minLevel <= level) && (it.maxLevel == -1 || it.maxLevel >= level)){
                restricted.Add(tileArray[i]);
            }
        }
        return restricted.ToArray();
    }

    private void AddRandomTiles(GameObject[] tileArray, int min, int max, int amount) {

        for (int i = 0; i < amount; i++) {
            GameObject tileChoice = tileArray[Random.Range(min, max)];
            Instantiate(tileChoice, RandomPosition(), Quaternion.identity);
        }
    }

    public int SetupScene(int level, int time) {
        this.activemonsters = 0;
        this.levelType = setLevelType(level);
        setBoardSize(this.levelType);

        boardSetup();
        InitialiseList();

        int totalPlaces = this.columns * this.rows;

        //Instantiate the exit tile in the upper right hand corner of our game board
        int exitIndex = gridPositions.Count - 1;
        Vector3 exitPosition = gridPositions[exitIndex];
        gridPositions.RemoveAt(exitIndex);
        this.instantiatedExit = Instantiate(exit, exitPosition, Quaternion.identity);
        totalPlaces--;

        // remove player position and ajacent positions from gridlist so the player has some space
        gridPositions.RemoveAt(this.rows);
        gridPositions.RemoveAt(1);
        gridPositions.RemoveAt(0);
        totalPlaces -= 3;
		// add one gem to the board, more can be earned
        Instantiate(this.gemTile, RandomPosition(), Quaternion.identity);
        totalPlaces--;

        int itemCount = addItemsToBoard(totalPlaces, this.levelType, level);

        return itemCount + 1;
    }

    private void setBoardSize(int type){
        // random level size variations, default
        const int maxSize = 11;
        this.columns = Random.Range(2, maxSize);
        this.rows = Random.Range((maxSize - this.columns), maxSize);

        // some specific values
        switch (levelType) {
            case GameManager.level_necromancer:
            this.columns = Random.Range(4, maxSize);
            this.rows = Random.Range(4, maxSize);
            break;
            case GameManager.level_wasteland:
            this.columns = Random.Range(10, 15);
            this.rows = Random.Range(10, 15);
            break;
        }
    }

    private int addItemsToBoard(int availablePlaces, int levelType, int level) {
        this.isLocked = false;
        int itemCount = 0;
        // standard level. If 5 x 6 then availablePlaces == 25 and:
		int monsterCount = (int)(availablePlaces * 5 / 10);		// 12
        int consumableCount = (int)(availablePlaces * 2 / 10);	//  5
        int weaponCount = (int)(availablePlaces * 2 / 10);      //  5

        // special levels can override the amounts above
        switch (levelType) {
            case GameManager.level_bomb:
				monsterCount = (int)(availablePlaces * 4 / 10);
                consumableCount = (int)(availablePlaces * 1 / 10);
                weaponCount = (int)(availablePlaces * 1 / 10);
				int bombsCount = (int)(availablePlaces * 3 / 10);
                itemCount += bombsCount; 
                AddTiles(this.itemTiles[BoardManager.location_bomb], bombsCount);
                break;
			case GameManager.level_treasure:
				monsterCount = (int)(availablePlaces * 3 / 10);
                consumableCount = (int)(availablePlaces * 1 / 10);
                weaponCount = (int)(availablePlaces * 1 / 10);
				int treasureCount = (int)(availablePlaces * 4 / 10);
                itemCount +=treasureCount;
                AddTiles(this.itemTiles[BoardManager.location_coin], treasureCount);
                break;
			case GameManager.level_low_monsters:
				monsterCount = 0;
				int lowMonstersCount = (int)(availablePlaces * 5 / 10);
                this.activemonsters += lowMonstersCount;
                itemCount += lowMonstersCount;
                AddRandomTiles(this.monsterTiles, 0, BoardManager.location_lowMonsters, lowMonstersCount);
                consumableCount = (int)(availablePlaces * 2 / 10);
                weaponCount = (int)(availablePlaces * 2 / 10);
                break;
			case GameManager.level_wasteland:
				monsterCount = (int)(availablePlaces * 1 / 15);
                consumableCount = (int)(availablePlaces * 1 / 15);
                weaponCount = (int)(availablePlaces * 1 / 15);
                break;
            case GameManager.level_potion:
                monsterCount = 1;
                consumableCount = (int)(availablePlaces * 1 / 10);
                weaponCount = 1;
                int potionCount = (int)(availablePlaces * 7 / 10);
                itemCount += potionCount;
                AddRandomTiles(this.itemTiles, BoardManager.location_unknown_potion_poison, BoardManager.location_unknown_potion_health + 1, potionCount);
                break;
            case GameManager.level_bushfire:
                monsterCount = (int)(availablePlaces * 1 / 10);
                int bushMonsterCount = (int)(availablePlaces * 2 / 10);
                itemCount += bushMonsterCount;
                AddTiles(this.monsterTiles[BoardManager.location_bushMonster], bushMonsterCount);
                consumableCount = (int)(availablePlaces * 3 / 10);
				bombsCount = (int)(availablePlaces * 1 / 10);
                weaponCount = (int)(availablePlaces * 2 / 10);
                break;
            case GameManager.level_necromancer:
                this.isLocked = true;
                monsterCount = 0;
                int zombieCount = (int)(availablePlaces * 1 / 10);
                this.activemonsters += zombieCount;
                itemCount += zombieCount;
                AddTiles(this.monsterTiles[BoardManager.location_zombie], zombieCount);
                weaponCount = 0;
                consumableCount = (int)(availablePlaces * 1 / 10);
                AddTiles(this.monsterTiles[BoardManager.location_larry], 1);
                itemCount += 1;
                break;
            case GameManager.level_locked:
                this.isLocked = true;
                AddTiles(keyTile, 1);
                itemCount += 1;
                monsterCount = (int)(availablePlaces * 3 / 10);
                break;
             case GameManager.level_vampire:
                this.isLocked = true;
                monsterCount = (int)(availablePlaces * 1 / 10);
                AddTiles(this.monsterTiles[BoardManager.location_vampire], 1);
                itemCount += 2;
                break;
            case GameManager.level_test:
                monsterCount = 0;
                int spiderCount = (int)(availablePlaces * 1 / 10);
                AddTiles(this.monsterTiles[BoardManager.location_spider], spiderCount);
                itemCount += spiderCount;
                int waspCount = (int)(availablePlaces * 1 / 10);
                AddTiles(this.monsterTiles[BoardManager.location_wasp], waspCount);
                itemCount += waspCount;
                break;
        }

        // add weapon
        GameObject weapon = setCorrectWeapon();
        for (int i = 0; i < weaponCount; i++) {
            Instantiate(weapon, RandomPosition(), Quaternion.identity);
        }

        // only add monsters with the right levelrestictions
        if(monsterCount > 0){
            GameObject [] restricted = restrictLevel(this.monsterTiles, level);
            AddRandomTiles(restricted, 0, restricted.Length, monsterCount);
        }
        // add items
        AddRandomTiles(this.itemTiles, 0, this.itemTiles.Length, consumableCount);

        // activemonsters is used to determine if all monsters have been killed and a gem is won.
		this.activemonsters += monsterCount;

        itemCount += monsterCount + 
			consumableCount + 
			weaponCount;

        return itemCount;
    }

    private int setLevelType(int level) {
        if (level == 1){
             return GameManager.level_test;
        }
        if (level < 2) {
            return GameManager.level_low_monsters;
        }
        if (level == 2) {
            return GameManager.level_bomb;
        }
        if (level == 3) {
            return GameManager.level_treasure;
        }
		if (level == 4) {
            return GameManager.level_wasteland;
        }
        if (level == 5) {
            return GameManager.level_potion;
        }
        if (level == 6) {
            return GameManager.level_bushfire;
        }
        if(level == 10){
            return GameManager.level_necromancer;
        }
        
        return GameManager.level_standard;
    }

    public void addExtraGem(){
        this.extraGem = Instantiate(this.gemTile, RandomPosition(), Quaternion.identity);
    }
    public void removeExtraGem(){
        gridPositions.Add(this.extraGem.transform.position);
        this.extraGem = null;
    }
    public void setTileToEmpty(Vector3 position){
        gridPositions.Add(this.extraGem.transform.position);
    }
}
