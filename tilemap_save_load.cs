using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
//using System.Xml.Linq;
//using System.Text;
using UnityEditor;

public class MapSaver : MonoBehaviour
{
    public bool saveMap = false;
    public bool loadMap = false;
    public Tilemap maze;
    public TilemapCollider2D mazeCollider;
    public Transform[] coins;
    private GameObject[] getFromScene;
    public GameObject coinPrefab;
    public Transform temp;
    public Transform endTrigger;
    public Transform player;
    
    public bool destroyCoinTrigger = false;

    public int levelIndex;

    void Start()
    {
        getFromScene = GameObject.FindGameObjectsWithTag("Coin");
        Debug.Log(getFromScene.Length);
        coins = new Transform[getFromScene.Length];
        for(int i = 0; i < getFromScene.Length; i++){
            coins[i] = getFromScene[i].transform;
        }
    }

    void Update()
    {
        if(saveMap){
            saveMap = false;
            Save();
        }
        if(loadMap){
            loadMap = false;
            DestroyCoins();
            //Load();
            StartCoroutine(Loading());
        }
        if(destroyCoinTrigger){
            destroyCoinTrigger = false;
            DestroyCoins();
        }
    }


    public void Save(){
        if(!File.Exists(Application.dataPath + "/LevelData/" + levelIndex)){
            Directory.CreateDirectory(Application.dataPath + "/LevelData/" + levelIndex);
            Directory.CreateDirectory(Application.dataPath + "/LevelData/" + levelIndex + "/maze");
            Directory.CreateDirectory(Application.dataPath + "/LevelData/" + levelIndex + "/coins");
        }

        StreamWriter writerMaze = new StreamWriter(Application.dataPath + "/LevelData/" + levelIndex + "/maze/maze.txt");
        string dataMaze = EditorJsonUtility.ToJson(maze);
        writerMaze.Write(dataMaze.ToCharArray(), 0, dataMaze.Length);
        writerMaze.Close();

        StreamWriter writerMazeCol = new StreamWriter(Application.dataPath + "/LevelData/" + levelIndex + "/maze/mazecol.txt");
        string dataMazeCol = EditorJsonUtility.ToJson(mazeCollider);
        writerMazeCol.Write(dataMazeCol.ToCharArray(), 0, dataMazeCol.Length);
        writerMazeCol.Close();

        StreamWriter writerEnd = new StreamWriter(Application.dataPath + "/LevelData/" + levelIndex + "/maze/trigger.txt");
        string dataEnd = EditorJsonUtility.ToJson(endTrigger);
        writerEnd.Write(dataEnd.ToCharArray(), 0, dataEnd.Length);
        writerEnd.Close();

        StreamWriter writerPlayer = new StreamWriter(Application.dataPath + "/LevelData/" + levelIndex + "/maze/player.txt");
        string dataPlayer = EditorJsonUtility.ToJson(player);
        writerPlayer.Write(dataPlayer.ToCharArray(), 0, dataPlayer.Length);
        writerPlayer.Close();

        DeleteCoinData();

        getFromScene = GameObject.FindGameObjectsWithTag("Coin");
        Debug.Log(getFromScene.Length);
        coins = new Transform[getFromScene.Length];
        for(int i = 0; i < getFromScene.Length; i++){
            coins[i] = getFromScene[i].transform;
        }


        for(int i = 0; i < coins.Length; i++){
            StreamWriter writerCoin = new StreamWriter(Application.dataPath + "/LevelData/" + levelIndex + "/coins/" + i +".txt");
            string dataCoin = EditorJsonUtility.ToJson(coins[i]);
            writerCoin.Write(dataCoin.ToCharArray(), 0, dataCoin.Length);
            writerCoin.Close();
        }


    }

    public void Load(){

        StreamReader readerMaze = new StreamReader(Application.dataPath + "/LevelData/" + levelIndex + "/maze/maze.txt");
        string dataMaze = readerMaze.ReadToEnd();
        EditorJsonUtility.FromJsonOverwrite(dataMaze, maze);
        readerMaze.Close();

        StreamReader readerMazeCol = new StreamReader(Application.dataPath + "/LevelData/" + levelIndex + "/maze/mazecol.txt");
        string dataMazeCol = readerMazeCol.ReadToEnd();
        EditorJsonUtility.FromJsonOverwrite(dataMazeCol, mazeCollider);
        readerMazeCol.Close();

        StreamReader readerEnd = new StreamReader(Application.dataPath + "/LevelData/" + levelIndex + "/maze/trigger.txt");
        string dataEnd = readerEnd.ReadToEnd();
        EditorJsonUtility.FromJsonOverwrite(dataEnd, endTrigger);
        readerEnd.Close();

        StreamReader readerPlayer = new StreamReader(Application.dataPath + "/LevelData/" + levelIndex + "/maze/player.txt");
        string dataPlayer = readerPlayer.ReadToEnd();
        Transform tempPlayerPosition;
        tempPlayerPosition = temp;
        EditorJsonUtility.FromJsonOverwrite(dataPlayer, tempPlayerPosition);
        player.position = tempPlayerPosition.position;
        readerPlayer.Close();

        int coinCount = Directory.GetFiles(Application.dataPath + "/LevelData/" + levelIndex + "/coins").Length / 2;
        Debug.Log(coinCount);

        
        DestroyCoins();

        for(int i = 0; i < coinCount; i++){
            StreamReader readerCoin = new StreamReader(Application.dataPath + "/LevelData/" + levelIndex + "/coins/" + i +".txt");
            string dataCoin = readerCoin.ReadToEnd();
            EditorJsonUtility.FromJsonOverwrite(dataCoin, temp);
            Instantiate(coinPrefab, temp.position, Quaternion.identity);
        }

        getFromScene = GameObject.FindGameObjectsWithTag("Coin");
        Debug.Log(getFromScene.Length);
        coins = new Transform[getFromScene.Length];
        for(int i = 0; i < getFromScene.Length; i++){
            coins[i] = getFromScene[i].transform;
        }
    }

    public void DestroyCoins(){
        GameObject[] coinsToDestroy = GameObject.FindGameObjectsWithTag("Coin");
        foreach(GameObject current in coinsToDestroy){
            Destroy(current);
        }
    }

    public void DeleteCoinData(){
        string[] coinDataToDelete = Directory.GetFiles(Application.dataPath + "/LevelData/" + levelIndex + "/coins");
        foreach(string currCoin in coinDataToDelete){
            File.Delete(currCoin);
        }
    }

    private IEnumerator Loading(){
        yield return new WaitForSeconds(.2f);
        Load();
    }

    private IEnumerator Loading(int indexToLoad){
        yield return new WaitForSeconds(.2f);
        Load(indexToLoad);
    }

    /*---------------------------------------*/
    public void LoadWithIndex(int indexToLoad){
        DestroyCoins();
        StartCoroutine(Loading(indexToLoad));
    }
    /*---------------------------------------*/

    public void Load(int indexToLoad){
        

        StreamReader readerMaze = new StreamReader(Application.dataPath + "/LevelData/" + indexToLoad + "/maze/maze.txt");
        string dataMaze = readerMaze.ReadToEnd();
        EditorJsonUtility.FromJsonOverwrite(dataMaze, maze);
        readerMaze.Close();

        StreamReader readerMazeCol = new StreamReader(Application.dataPath + "/LevelData/" + indexToLoad + "/maze/mazecol.txt");
        string dataMazeCol = readerMazeCol.ReadToEnd();
        EditorJsonUtility.FromJsonOverwrite(dataMazeCol, mazeCollider);
        readerMazeCol.Close();

        StreamReader readerEnd = new StreamReader(Application.dataPath + "/LevelData/" + indexToLoad + "/maze/trigger.txt");
        string dataEnd = readerEnd.ReadToEnd();
        EditorJsonUtility.FromJsonOverwrite(dataEnd, endTrigger);
        readerEnd.Close();

        StreamReader readerPlayer = new StreamReader(Application.dataPath + "/LevelData/" + indexToLoad + "/maze/player.txt");
        string dataPlayer = readerPlayer.ReadToEnd();
        Transform tempPlayerPosition;
        tempPlayerPosition = temp;
        EditorJsonUtility.FromJsonOverwrite(dataPlayer, tempPlayerPosition);
        player.position = tempPlayerPosition.position;
        readerPlayer.Close();

        int coinCount = Directory.GetFiles(Application.dataPath + "/LevelData/" + indexToLoad + "/coins").Length / 2;
        Debug.Log(coinCount);

        
        DestroyCoins();

        for(int i = 0; i < coinCount; i++){
            StreamReader readerCoin = new StreamReader(Application.dataPath + "/LevelData/" + indexToLoad + "/coins/" + i +".txt");
            string dataCoin = readerCoin.ReadToEnd();
            EditorJsonUtility.FromJsonOverwrite(dataCoin, temp);
            Instantiate(coinPrefab, temp.position, Quaternion.identity);
        }

        getFromScene = GameObject.FindGameObjectsWithTag("Coin");
        Debug.Log(getFromScene.Length);
        coins = new Transform[getFromScene.Length];
        for(int i = 0; i < getFromScene.Length; i++){
            coins[i] = getFromScene[i].transform;
        }
        PlayerPrefs.SetInt("maxCoin",getFromScene.Length);
    }

}
