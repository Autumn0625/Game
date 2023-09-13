using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.U2D.IK;
using TMPro;
using UnityEditor;
using UnityEngine.Timeline;
using System.Security.Cryptography;
using UnityEngine.SocialPlatforms.Impl;

public class RandomSpawnManager : MonoBehaviour
{
    [SerializeField] GameObject[] objectPrefab; // 要生成的Prefab
    private int numberOfObjectsToSpawn; // 要生成的物體數量
    Vector3 minPosition = new Vector3(-10, -10, 0); // 最小座標
    Vector3 maxPosition = new Vector3(10, 10, 0); // 最大座標

    public int initialGrass = 5;
    public int initialLamb = 5;
    public int initialSheep = 10;
    public int initialWolf = 1;
    public int initialDog = 1;

    private List<GameObject> grassList = new List<GameObject>();
    private List<GameObject> huntList = new List<GameObject>();
    private List<GameObject> runList = new List<GameObject>();
    private List<Vector3> existingPoints = new List<Vector3>();
    private List<int> LambcoinList = new List<int>();
    private List<int> SheepcoinList = new List<int>();
    public List<int> ShowList = new List<int>();


    private List<Vector3> allPositions = new List<Vector3>();//所有物件的座標
    private List<Vector3> grassPositions = new List<Vector3>();
    private List<Vector3> lambPositions = new List<Vector3>();
    private List<Vector3> sheepPositions = new List<Vector3>();
    private List<Vector3> huntPositions = new List<Vector3>();//由羊跟小羊組成的獵物
    private List<Vector3> wolfPositions = new List<Vector3>();
    private List<Vector3> runPositions = new List<Vector3>();//由狼組成的被驅趕者
    private List<Vector3> dogPositions = new List<Vector3>();

    private Vector3 PositionWolf = new Vector3();
    private Vector3 PositionDog = new Vector3();

    private List<Vector3> updatedSheepPositions = new List<Vector3>();//下一回合的羊座標
    private List<Vector3> updatedLambPositions = new List<Vector3>();//下一回合的小羊座標
    private List<Vector3> updatedWolfPositions = new List<Vector3>();//下一回合的狼座標
    private List<Vector3> updatedDogPositions = new List<Vector3>();//下一回合的狗座標

    private List<GameObject> spawnedLambObjects = new List<GameObject>();
    private List<GameObject> spawnedSheepObjects = new List<GameObject>();
    private List<GameObject> spawnedWolfObjects = new List<GameObject>();
    private List<GameObject> spawnedDogObjects = new List<GameObject>();

    private bool done = false;
    private bool signal = false;
    private int eat = 0;
    private int waitRounds = 0;
    [SerializeField] int currentRound = 1;
    [SerializeField] int grassEaten = 0;
    [SerializeField] int lambEaten = 0;
    [SerializeField] int sheepEaten = 0;
    [SerializeField] int wolfAteLamb = 0;
    [SerializeField] int wolfAteSheep = 0;
    [SerializeField] int dogAttackedWolf = 0;
    [SerializeField] int totalCoins = 0;

    [SerializeField] TextMeshProUGUI CurrentRound;
    [SerializeField] TextMeshProUGUI GrassEaten;
    [SerializeField] TextMeshProUGUI LambEaten;
    [SerializeField] TextMeshProUGUI SheepEaten;
    [SerializeField] TextMeshProUGUI WolfAteLamb;
    [SerializeField] TextMeshProUGUI WolfAteSheep;
    [SerializeField] TextMeshProUGUI DogAttackedWolf;
    [SerializeField] TextMeshProUGUI TotalCoins;
    void Start()
    {
        //第一回合物件生成
        CreateObject(0, initialGrass);
        CreateObject(1, initialLamb);
        CreateObject(2, initialSheep);
        CreateObject(3, initialWolf);
        CreateObject(4, initialDog);
    }
    void Update()
    {
        CurrentRound.text = "CurrentRound:" + currentRound.ToString();
        GrassEaten.text = "GrassEaten:" + grassEaten.ToString();
        LambEaten.text = "LambEaten:" + lambEaten.ToString();
        SheepEaten.text = "SheepEaten:" + sheepEaten.ToString();
        WolfAteLamb.text = "WolfAteLamb:" + wolfAteLamb.ToString();
        WolfAteSheep.text = "WolfAteSheep:" + wolfAteSheep.ToString();
        DogAttackedWolf.text = "DogAttackedWolf:" + dogAttackedWolf.ToString();
        TotalCoins.text = "TotalCoins:" + totalCoins.ToString();
    }
    private bool IsPositionInList(Vector3 positionToCheck, List<Vector3> positionList)
    {
        foreach (Vector3 position in positionList)
        {
            if (position == positionToCheck)
            {
                return true; // 位置已存在於列表中
            }
        }
        return false; // 位置不存在於列表中
    }//確認座標是否有重複
    public void CreateLamb()
    {
        if (currentRound % 2 == 0)
        {
            foreach (Vector3 spawnPoint in updatedLambPositions)
            {
                GameObject obj = Instantiate(objectPrefab[1], spawnPoint, Quaternion.identity, transform);
                spawnedLambObjects.Add(obj);
            }

            lambPositions.Clear();
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            foreach (Vector3 spawnPoint in lambPositions)
            {
                GameObject obj = Instantiate(objectPrefab[1], spawnPoint, Quaternion.identity, transform);
                spawnedLambObjects.Add(obj);
            }

            updatedLambPositions.Clear();
        }

    }//生成小羊
    public void CreateSheep()
    {
        if (currentRound % 2 == 0)
        {
            foreach (Vector3 spawnPoint in updatedSheepPositions)
            {
                GameObject obj = Instantiate(objectPrefab[2], spawnPoint, Quaternion.identity, transform);
                spawnedSheepObjects.Add(obj);
            }
            sheepPositions.Clear();
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            foreach (Vector3 spawnPoint in sheepPositions)
            {
                GameObject obj = Instantiate(objectPrefab[2], spawnPoint, Quaternion.identity, transform);
                spawnedSheepObjects.Add(obj);
            }

            updatedSheepPositions.Clear();
        }

    }//生成羊
    public void CreateWolf()
    {
        if (currentRound % 2 == 0)
        {
            foreach (Vector3 spawnPoint in updatedWolfPositions)
            {
                GameObject obj = Instantiate(objectPrefab[3], spawnPoint, Quaternion.identity, transform);
                spawnedWolfObjects.Add(obj);
            }

            wolfPositions.Clear();
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            foreach (Vector3 spawnPoint in wolfPositions)
            {
                GameObject obj = Instantiate(objectPrefab[3], spawnPoint, Quaternion.identity, transform);
                spawnedWolfObjects.Add(obj);
            }

            updatedWolfPositions.Clear();
        }
    }//生成狼
    public void CreateDog()
    {
        if (currentRound % 2 == 0)
        {
            foreach (Vector3 spawnPoint in updatedDogPositions)
            {
                GameObject obj = Instantiate(objectPrefab[4], spawnPoint, Quaternion.identity, transform);
                spawnedDogObjects.Add(obj);
            }

            dogPositions.Clear();
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            foreach (Vector3 spawnPoint in dogPositions)
            {
                GameObject obj = Instantiate(objectPrefab[4], spawnPoint, Quaternion.identity, transform);
                spawnedDogObjects.Add(obj);
            }

            updatedDogPositions.Clear();
        }
    }//生成狗
    void DestroyLambNextTurn()
    {
        foreach (GameObject obj in spawnedLambObjects)
        {
            Destroy(obj); // 銷毀遊戲對象
        }
        spawnedLambObjects.Clear(); // 清空列表
    }//摧毀上一回合的小羊
    void DestroySheepNextTurn()
    {
        foreach (GameObject obj in spawnedSheepObjects)
        {
            Destroy(obj); // 銷毀遊戲對象
        }
        spawnedSheepObjects.Clear(); // 清空列表

    }//摧毀上一回合的羊
    void DestroyWolfNextTurn()
    {
        foreach (GameObject obj in spawnedWolfObjects)
        {
            Destroy(obj); // 銷毀遊戲對象
        }
        spawnedWolfObjects.Clear(); // 清空列表
    }//摧毀上一回合的狼
    void DestroyDogNextTurn()
    {
        foreach (GameObject obj in spawnedDogObjects)
        {
            Destroy(obj); // 銷毀遊戲對象
        }
        spawnedDogObjects.Clear(); // 清空列表
    }//摧毀上一回合的狗
    public void CreateObject(int x, int y)
    {
        int objectCounter = 0;
        List<Vector3> randomSpawnPoints = GenerateRandomSpawnPoints(y);

        foreach (Vector3 spawnPoint in randomSpawnPoints)
        {
            int r = x;

            GameObject obj = Instantiate(objectPrefab[r], spawnPoint, Quaternion.identity, transform);
            string objectName = objectPrefab[r] + objectCounter.ToString();
            obj.name = objectName;
            objectCounter++;

            allPositions.Add(spawnPoint);
            if (r == 0)
            {
                grassPositions.Add(spawnPoint);
                grassList.Add(obj);
            }
            else if (r == 1)
            {
                lambPositions.Add(spawnPoint);
                huntPositions.Add(spawnPoint);
                spawnedLambObjects.Add(obj);
                huntList.Add(obj);
                LambcoinList.Add(0);
            }
            else if (r == 2)
            {
                sheepPositions.Add(spawnPoint);
                huntPositions.Add(spawnPoint);
                spawnedSheepObjects.Add(obj);
                huntList.Add(obj);
                SheepcoinList.Add(0);
            }
            else if (r == 3)
            {
                wolfPositions.Add(spawnPoint);
                runPositions.Add(spawnPoint);
                spawnedWolfObjects.Add(obj);
                runList.Add(obj);
            }
            else if (r == 4)
            {
                dogPositions.Add(spawnPoint);
                spawnedDogObjects.Add(obj);
            }
        }
    }//生成物件
    public List<Vector3> GenerateRandomSpawnPoints(int a)//隨機生成座標
    {
        numberOfObjectsToSpawn = a;
        List<Vector3> spawnPoints = new List<Vector3>();
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            Vector3 randomSpawnPoint;
            int maxAttempts = 10000;
            int attempts = 0;

            do
            {
                randomSpawnPoint = new Vector3(
                    Random.Range((int)minPosition.x, (int)maxPosition.x + 1),
                    Random.Range((int)minPosition.y, (int)maxPosition.y + 1),
                    0
                );

                attempts++;

                if (attempts >= maxAttempts)
                {
                    Debug.LogError("ERROR: Cannot generate a unique coordinate!");
                    break;
                }
            } while (allPositions.Contains(randomSpawnPoint));

            allPositions.Add(randomSpawnPoint);
            spawnPoints.Add(randomSpawnPoint);
            Debug.Log("Generated Spawn: " + randomSpawnPoint);
        }
        return spawnPoints;
    }
    public void Grass()
    {
        for (int i = 0; i < grassPositions.Count; i++)
        {
            Vector3 position = grassPositions[i];
            //Debug.Log("Grass Position " + i + ": " + position);
        }
    }
    public void Lamb()
    {
        for (int i = 0; i < lambPositions.Count; i++)
        {
            Vector3 position = lambPositions[i];
            //Debug.Log("Lamb Position " + i + ": " + position);
        }
        if (currentRound % 2 == 0)//currentRound ==2
        {
            if (grassPositions.Count == 1)
            {
                foreach (Vector3 lambPosition in lambPositions)
                {
                    float nearestDistance = float.MaxValue;
                    float secondNearestDistance = float.MaxValue; // 初始化第二近距離為最大值
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // 初始化第二近位置為零向量

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(lambPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // 將目前最近的設為第二近
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // 更新最近的位置和距離
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // 更新第二近的位置和距離
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }
                    // 現在 nearestGrassPosition 就是每一隻羊最近的草的座標
                    //Debug.Log("Nearest grass to lamb at " + lambPosition + " is at " + nearestPosition);
                    float Fx = lambPosition.x - nearestPosition.x;
                    float Fy = lambPosition.y - nearestPosition.y;
                    if (Fx == Fy)
                    {
                        grassPositions.RemoveAt(0);
                        done = true;
                    }

                }
                if (done)
                {
                    foreach (Vector3 lambPosition in lambPositions)
                    {
                        updatedLambPositions.Add(lambPosition);
                    }
                }
            }
            if (!done)
            {
                int roundCount = 0;
                foreach (Vector3 lambPosition in lambPositions)
                {
                    Debug.Log("grassPositions.Count =" + grassPositions.Count);
                    float nearestDistance = float.MaxValue;
                    float secondNearestDistance = float.MaxValue; // 初始化第二近距離為最大值
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // 初始化第二近位置為零向量

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(lambPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // 將目前最近的設為第二近
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // 更新最近的位置和距離
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // 更新第二近的位置和距離
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }
                    // 現在 nearestGrassPosition 就是每一隻羊最近的草的座標
                    //Debug.Log("Nearest grass to lamb at " + lambPosition + " is at " + nearestPosition);

                    float Fx = lambPosition.x - nearestPosition.x;
                    float Fy = lambPosition.y - nearestPosition.y;
                    if (Fx > 1.0f)
                    {
                        Fx = 1.0f;
                    }
                    else if (Fx < -1.0f)
                    {
                        Fx = -1.0f;
                    }
                    if (Fy > 1.0f)
                    {
                        Fy = 1.0f;
                    }
                    else if (Fy < -1.0f)
                    {
                        Fy = -1.0f;
                    }
                    if (grassPositions.Count == 0)
                    {
                        updatedLambPositions.Add(lambPosition);
                    }
                    else if ((Mathf.Abs(Fx) >= 1.0f))
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedLambPosition = new Vector3(lambPosition.x - Fx, lambPosition.y, lambPosition.z);
                        Vector3 newupdatedLambPosition = new Vector3(lambPosition.x, lambPosition.y - Fy, lambPosition.z);
                        if (!IsPositionInList(updatedLambPosition, huntPositions) && !IsPositionInList(updatedLambPosition, dogPositions))
                        {
                            updatedLambPositions.Add(updatedLambPosition); // 將更新後的位置添加到 List 中
                            allPositions.Add(updatedLambPosition);
                            huntPositions.Add(updatedLambPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == lambPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == lambPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else if (!IsPositionInList(newupdatedLambPosition, huntPositions) && !IsPositionInList(updatedLambPosition, dogPositions))
                        {
                            updatedLambPositions.Add(newupdatedLambPosition); // 將更新後的位置添加到 List 中
                            allPositions.Add(newupdatedLambPosition);
                            huntPositions.Add(newupdatedLambPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == lambPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == lambPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            updatedLambPositions.Add(lambPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if ((Mathf.Abs(Fy) >= 1.0f))
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedLambPosition = new Vector3(lambPosition.x, lambPosition.y - Fy, lambPosition.z);
                        if (!IsPositionInList(updatedLambPosition, huntPositions) && !IsPositionInList(updatedLambPosition, dogPositions))
                        {
                            updatedLambPositions.Add(updatedLambPosition); // 將更新後的位置添加到 List 中
                            allPositions.Add(updatedLambPosition);
                            huntPositions.Add(updatedLambPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == lambPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == lambPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            updatedLambPositions.Add(lambPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Fx == 0 && Fy == 0)
                    {
                        for (int i = grassPositions.Count - 1; i >= 0; i--)
                        {
                            if (grassPositions[i] == nearestPosition)
                            {
                                grassPositions.RemoveAt(i);
                            }
                        }
                        LambcoinList[roundCount]++;
                        Debug.Log("Eat!");
                        grassEaten++;
                        if (LambcoinList[roundCount] == 3)
                        {
                            totalCoins = totalCoins + 10;
                            lambEaten++;
                            LambcoinList[roundCount] = 0;
                            LambcoinList.RemoveAt(roundCount);
                            SheepcoinList.Add(0);
                            updatedSheepPositions.Add(lambPositions[roundCount]);
                            Debug.Log("Lamb Grow!");
                            roundCount--;
                        }
                        else
                        {
                            Fx = lambPosition.x - secondNearestPosition.x;
                            Fy = lambPosition.y - secondNearestPosition.y;
                            if (Fx > 1.0f)
                            {
                                Fx = 1.0f;
                            }
                            else if (Fx < -1.0f)
                            {
                                Fx = -1.0f;
                            }
                            if (Fy > 1.0f)
                            {
                                Fy = 1.0f;
                            }
                            else if (Fy < -1.0f)
                            {
                                Fy = -1.0f;
                            }
                            if ((Mathf.Abs(Fx) >= 1.0f))
                            {
                                // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                                Vector3 updatedLambPosition = new Vector3(lambPosition.x - Fx, lambPosition.y, lambPosition.z);
                                Vector3 newupdatedLambPosition = new Vector3(lambPosition.x, lambPosition.y - Fy, lambPosition.z);
                                if (!IsPositionInList(updatedLambPosition, huntPositions) && !IsPositionInList(updatedLambPosition, updatedDogPositions))
                                {
                                    updatedLambPositions.Add(updatedLambPosition); // 將更新後的位置添加到 List 中
                                    allPositions.Add(updatedLambPosition);
                                    huntPositions.Add(updatedLambPosition);
                                    for (int i = huntPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (huntPositions[i] == lambPosition)
                                        {
                                            huntPositions.RemoveAt(i);
                                        }
                                    }
                                    for (int i = allPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (allPositions[i] == lambPosition)
                                        {
                                            allPositions.RemoveAt(i);
                                        }
                                    }
                                }
                                else if (!IsPositionInList(newupdatedLambPosition, huntPositions) && !IsPositionInList(updatedLambPosition, updatedDogPositions))
                                {
                                    updatedLambPositions.Add(newupdatedLambPosition); // 將更新後的位置添加到 List 中
                                    allPositions.Add(newupdatedLambPosition);
                                    huntPositions.Add(newupdatedLambPosition);
                                    for (int i = huntPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (huntPositions[i] == lambPosition)
                                        {
                                            huntPositions.RemoveAt(i);
                                        }
                                    }
                                    for (int i = allPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (allPositions[i] == lambPosition)
                                        {
                                            allPositions.RemoveAt(i);
                                        }
                                    }
                                }
                                else
                                {
                                    updatedLambPositions.Add(lambPosition); // 將更新後的位置添加到 List 中
                                }
                            }
                            else if ((Mathf.Abs(Fy) >= 1.0f))
                            {
                                // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                                Vector3 updatedLambPosition = new Vector3(lambPosition.x, lambPosition.y - Fy, lambPosition.z);
                                if (!IsPositionInList(updatedLambPosition, huntPositions) && !IsPositionInList(updatedLambPosition, updatedDogPositions))
                                {
                                    updatedLambPositions.Add(updatedLambPosition); // 將更新後的位置添加到 List 中
                                    allPositions.Add(updatedLambPosition);
                                    huntPositions.Add(updatedLambPosition);
                                    for (int i = huntPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (huntPositions[i] == lambPosition)
                                        {
                                            huntPositions.RemoveAt(i);
                                        }
                                    }
                                    for (int i = allPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (allPositions[i] == lambPosition)
                                        {
                                            allPositions.RemoveAt(i);
                                        }
                                    }
                                }
                                else
                                {
                                    updatedLambPositions.Add(lambPosition); // 將更新後的位置添加到 List 中
                                }
                            }
                        }

                    }
                    else
                    {
                        Debug.Log("stop2");
                        // 如果不需要移動，則保持原位置
                        updatedLambPositions.Add(lambPosition); ;
                    }
                roundCount++;
                }
            }
            done = false;
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            if (grassPositions.Count == 1)
            {
                foreach (Vector3 updatedLambPosition in updatedLambPositions)
                {
                    Debug.Log("grassPositions.Count =" + grassPositions.Count);
                    float nearestDistance = float.MaxValue;
                    float secondNearestDistance = float.MaxValue; // 初始化第二近距離為最大值
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // 初始化第二近位置為零向量
                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(updatedLambPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // 將目前最近的設為第二近
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // 更新最近的位置和距離
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // 更新第二近的位置和距離
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }
                    // 現在 nearestGrassPosition 就是每一隻羊最近的草的座標
                    //Debug.Log("Nearest grass to lamb at " + lambPosition + " is at " + nearestPosition);
                    float Fx = updatedLambPosition.x - nearestPosition.x;
                    float Fy = updatedLambPosition.y - nearestPosition.y;
                    if (Fx == Fy)
                    {
                        grassPositions.RemoveAt(0);
                        done = true;
                    }
                }
                if (done)
                {
                    foreach (Vector3 updatedLambPosition in updatedLambPositions)
                    {
                        lambPositions.Add(updatedLambPosition);
                    }
                }
            }
            if (!done)
            {
                int roundCount = 0;
                foreach (Vector3 updatedLambPosition in updatedLambPositions)
                {
                    Debug.Log("grassPositions.Count =" + grassPositions.Count);
                    float nearestDistance = float.MaxValue;
                    float secondNearestDistance = float.MaxValue; // 初始化第二近距離為最大值
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // 初始化第二近位置為零向量

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(updatedLambPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // 將目前最近的設為第二近
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // 更新最近的位置和距離
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // 更新第二近的位置和距離
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }

                    // 現在 nearestGrassPosition 就是每一隻羊最近的草的座標
                    //Debug.Log("Nearest grass to lamb at " + lambPosition + " is at " + nearestPosition);

                    float Fx = updatedLambPosition.x - nearestPosition.x;
                    float Fy = updatedLambPosition.y - nearestPosition.y;
                    if (Fx > 1.0f)
                    {
                        Fx = 1.0f;
                    }
                    else if (Fx < -1.0f)
                    {
                        Fx = -1.0f;
                    }
                    if (Fy > 1.0f)
                    {
                        Fy = 1.0f;
                    }
                    else if (Fy < -1.0f)
                    {
                        Fy = -1.0f;
                    }
                    if (grassPositions.Count == 0)
                    {
                        lambPositions.Add(updatedLambPosition);
                    }
                    else if ((Mathf.Abs(Fx) >= 1.0f))
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 lambPosition = new Vector3(updatedLambPosition.x - Fx, updatedLambPosition.y, updatedLambPosition.z);
                        Vector3 newlambPosition = new Vector3(updatedLambPosition.x, updatedLambPosition.y - Fy, updatedLambPosition.z);
                        if (!IsPositionInList(lambPosition, huntPositions) && !IsPositionInList(lambPosition, updatedDogPositions))
                        {
                            lambPositions.Add(lambPosition); // 將更新後的位置添加到 List 中
                            allPositions.Add(lambPosition);
                            huntPositions.Add(lambPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == updatedLambPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedLambPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else if (!IsPositionInList(newlambPosition, huntPositions) && !IsPositionInList(newlambPosition, updatedDogPositions))
                        {
                            lambPositions.Add(newlambPosition); // 將更新後的位置添加到 List 中
                            allPositions.Add(newlambPosition);
                            huntPositions.Add(newlambPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == updatedLambPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedLambPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            lambPositions.Add(updatedLambPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if ((Mathf.Abs(Fy) >= 1.0f))
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 lambPosition = new Vector3(updatedLambPosition.x, updatedLambPosition.y - Fy, updatedLambPosition.z);
                        if (!IsPositionInList(lambPosition, huntPositions) && !IsPositionInList(lambPosition, updatedDogPositions))
                        {
                            lambPositions.Add(lambPosition); // 將更新後的位置添加到 List 中
                            allPositions.Add(lambPosition);
                            huntPositions.Add(lambPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == updatedLambPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedLambPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            lambPositions.Add(updatedLambPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Fx == 0 && Fy == 0)
                    {
                        for (int i = grassPositions.Count - 1; i >= 0; i--)
                        {
                            if (grassPositions[i] == nearestPosition)
                            {
                                grassPositions.RemoveAt(i);
                            }
                        }
                        Debug.Log("Eat!");
                        LambcoinList[roundCount]++;
                        grassEaten++;
                        if (LambcoinList[roundCount] == 3)
                        {
                            totalCoins = totalCoins + 10;
                            lambEaten++;
                            LambcoinList[roundCount] = 0;
                            LambcoinList.RemoveAt(roundCount);
                            SheepcoinList.Add(0);
                            sheepPositions.Add(updatedLambPositions[roundCount]);
                            Debug.Log("Lamb Grow!");
                            roundCount--;
                        }
                        else
                        {
                            Fx = updatedLambPosition.x - secondNearestPosition.x;
                            Fy = updatedLambPosition.y - secondNearestPosition.y;
                            if (Fx > 1.0f)
                            {
                                Fx = 1.0f;
                            }
                            else if (Fx < -1.0f)
                            {
                                Fx = -1.0f;
                            }
                            if (Fy > 1.0f)
                            {
                                Fy = 1.0f;
                            }
                            else if (Fy < -1.0f)
                            {
                                Fy = -1.0f;
                            }
                            if ((Mathf.Abs(Fx) >= 1.0f))
                            {
                                // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                                Vector3 lambPosition = new Vector3(updatedLambPosition.x - Fx, updatedLambPosition.y, updatedLambPosition.z);
                                Vector3 newlambPosition = new Vector3(updatedLambPosition.x, updatedLambPosition.y - Fy, updatedLambPosition.z);
                                if (!IsPositionInList(lambPosition, huntPositions) && !IsPositionInList(lambPosition, dogPositions))
                                {
                                    lambPositions.Add(lambPosition); // 將更新後的位置添加到 List 中
                                    allPositions.Add(lambPosition);
                                    huntPositions.Add(lambPosition);
                                    for (int i = huntPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (huntPositions[i] == updatedLambPosition)
                                        {
                                            huntPositions.RemoveAt(i);
                                        }
                                    }
                                    for (int i = allPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (allPositions[i] == updatedLambPosition)
                                        {
                                            allPositions.RemoveAt(i);
                                        }
                                    }
                                }
                                else if (!IsPositionInList(newlambPosition, huntPositions) && !IsPositionInList(newlambPosition, dogPositions))
                                {
                                    lambPositions.Add(newlambPosition); // 將更新後的位置添加到 List 中
                                    allPositions.Add(newlambPosition);
                                    huntPositions.Add(newlambPosition);
                                    for (int i = huntPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (huntPositions[i] == updatedLambPosition)
                                        {
                                            huntPositions.RemoveAt(i);
                                        }
                                    }
                                    for (int i = allPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (allPositions[i] == updatedLambPosition)
                                        {
                                            allPositions.RemoveAt(i);
                                        }
                                    }
                                }
                                else
                                {
                                    lambPositions.Add(updatedLambPosition); // 將更新後的位置添加到 List 中
                                }
                            }
                            else if ((Mathf.Abs(Fy) >= 1.0f))
                            {
                                // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                                Vector3 lambPosition = new Vector3(updatedLambPosition.x, updatedLambPosition.y - Fy, updatedLambPosition.z);
                                if (!IsPositionInList(lambPosition, huntPositions) && !IsPositionInList(lambPosition, dogPositions))
                                {
                                    lambPositions.Add(lambPosition); // 將更新後的位置添加到 List 中
                                    allPositions.Add(lambPosition);
                                    huntPositions.Add(lambPosition);
                                    for (int i = huntPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (huntPositions[i] == updatedLambPosition)
                                        {
                                            huntPositions.RemoveAt(i);
                                        }
                                    }
                                    for (int i = allPositions.Count - 1; i >= 0; i--)
                                    {
                                        if (allPositions[i] == updatedLambPosition)
                                        {
                                            allPositions.RemoveAt(i);
                                        }
                                    }
                                }
                                else
                                {
                                    lambPositions.Add(updatedLambPosition); // 將更新後的位置添加到 List 中
                                }
                            }
                        }
                    }
                    else
                    {
                        // 如果不需要移動，則保持原位置
                        Debug.Log("stop2");
                        lambPositions.Add(updatedLambPosition);
                    }
                    roundCount++;
                }
            }
            done = false;
        }
        if (currentRound % 2 == 0)
        {
            Debug.Log("Round " + currentRound + " updatedLambPositions.Count = " + updatedLambPositions.Count);
            //CreateLamb();
            grassList.Clear();
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            Debug.Log("Round " + currentRound + " LambPositions.Count = " + lambPositions.Count);
            //CreateLamb();
            grassList.Clear();
        }

    }
    public void Sheep()
    {
        for (int i = 0; i < sheepPositions.Count; i++)
        {
            Vector3 position = sheepPositions[i];
            //Debug.Log("Sheep Position " + i + ": " + position);
        }
        if (currentRound % 2 == 0)//currentRound ==2
        {
            if (sheepPositions.Count == 1)
            {
                foreach (Vector3 sheepPosition in sheepPositions)
                {
                    float nearestDistance = float.MaxValue;
                    float secondNearestDistance = float.MaxValue; // 初始化第二近距離為最大值
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // 初始化第二近位置為零向量

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(sheepPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // 將目前最近的設為第二近
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // 更新最近的位置和距離
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // 更新第二近的位置和距離
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }
                    // 現在 nearestGrassPosition 就是每一隻羊最近的草的座標
                    //Debug.Log("Nearest grass to lamb at " + lambPosition + " is at " + nearestPosition);
                    float Fx = sheepPosition.x - nearestPosition.x;
                    float Fy = sheepPosition.y - nearestPosition.y;
                    if (Fx == Fy)
                    {
                        grassPositions.RemoveAt(0);
                        done = true;
                    }

                }
                if (done)
                {
                    foreach (Vector3 sheepPosition in sheepPositions)
                    {
                        updatedSheepPositions.Add(sheepPosition);
                    }
                }
            }
            if (!done)
            {
                int roundCount = 0;
                foreach (Vector3 sheepPosition in sheepPositions)
                {
                    Debug.Log("currentRound" + currentRound + "grassPositions.Count =" + grassPositions.Count);
                    float nearestDistance = float.MaxValue;
                    float secondNearestDistance = float.MaxValue; // 初始化第二近距離為最大值
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // 初始化第二近位置為零向量

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(sheepPosition, grassPosition);
                        if (distance < nearestDistance)
                        {
                            // 將目前最近的設為第二近
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // 更新最近的位置和距離
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // 更新第二近的位置和距離
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }

                    // 現在 nearestGrassPosition 就是每一隻羊最近的草的座標
                    //Debug.Log("Nearest grass to sheep at " + sheepPosition + " is at " + nearestPosition);

                    
                    float Fx = sheepPosition.x - nearestPosition.x;
                    float fx = Fx;
                    float Fy = sheepPosition.y - nearestPosition.y;
                    float fy = Fy;
                    if (Fx > 2.0f)
                    {
                        Fx = 2.0f;
                    }
                    else if (Fx < -2.0f)
                    {
                        Fx = -2.0f;
                    }
                    if (Fy > 2.0f)
                    {
                        Fy = 2.0f;
                    }
                    else if (Fy < -2.0f)
                    {
                        Fy = -2.0f;
                    }
                    if (Fx > 0)
                    {
                        fx--;
                    }
                    else
                    {
                        fx++;
                    }
                    if (Fy > 0)
                    {
                        fy--;
                    }
                    else
                    {
                        fy++;
                    }
                    if (grassPositions.Count == 0)
                    {
                        updatedSheepPositions.Add(sheepPosition);
                    }
                    else if (Mathf.Abs(Fx) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y, sheepPosition.z);
                        Vector3 newupdatedSheepPosition = new Vector3(sheepPosition.x - fx, sheepPosition.y - fy, sheepPosition.z);
                        if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                        {
                            updatedSheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(updatedSheepPosition);
                            allPositions.Add(updatedSheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == sheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == sheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else if (!IsPositionInList(newupdatedSheepPosition, huntPositions) && !IsPositionInList(newupdatedSheepPosition, dogPositions))
                        {
                            updatedSheepPositions.Add(newupdatedSheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(newupdatedSheepPosition);
                            allPositions.Add(newupdatedSheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == sheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == sheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            updatedSheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                        Vector3 updatedSheepPosition = new Vector3(sheepPosition.x, sheepPosition.y - Fy, sheepPosition.z);
                        Vector3 newupdatedSheepPosition = new Vector3(sheepPosition.x - fx, sheepPosition.y - fy, sheepPosition.z);
                        if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                        {
                            updatedSheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(updatedSheepPosition);
                            allPositions.Add(updatedSheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == sheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == sheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else if (!IsPositionInList(newupdatedSheepPosition, huntPositions) && !IsPositionInList(newupdatedSheepPosition, dogPositions))
                        {
                            updatedSheepPositions.Add(newupdatedSheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(newupdatedSheepPosition);
                            allPositions.Add(newupdatedSheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == sheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == sheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            updatedSheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y - Fy, sheepPosition.z);
                        if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                        {
                            updatedSheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(updatedSheepPosition);
                            allPositions.Add(updatedSheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == sheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == sheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            updatedSheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                        Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y - Fy, sheepPosition.z);
                        if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                        {
                            updatedSheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(updatedSheepPosition);
                            allPositions.Add(updatedSheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == sheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == sheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            updatedSheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Fx == 0f && Fy == 0f)
                    {
                        for (int i = grassPositions.Count - 1; i >= 0; i--)
                        {
                            if (grassPositions[i] == nearestPosition)
                            {
                                grassPositions.RemoveAt(i);
                            }
                        }
                        Debug.Log("Eat!");
                        SheepcoinList[roundCount]++;
                        grassEaten++;
                        Fx = sheepPosition.x - secondNearestPosition.x;
                        fx = Fx;
                        Fy = sheepPosition.y - secondNearestPosition.y;
                        fy = Fy;
                        if (Fx > 2.0f)
                        {
                            Fx = 2.0f;
                        }
                        else if (Fx < -2.0f)
                        {
                            Fx = -2.0f;
                        }
                        if (Fy > 2.0f)
                        {
                            Fy = 2.0f;
                        }
                        else if (Fy < -2.0f)
                        {
                            Fy = -2.0f;
                        }
                        if (Fx > 0)
                        {
                            fx--;
                        }
                        else
                        {
                            fx++;
                        }
                        if (Fy > 0)
                        {
                            fy--;
                        }
                        else
                        {
                            fy++;
                        }
                        if (Mathf.Abs(Fx) >= 2.0f)
                        {
                            // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                            Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y, sheepPosition.z);
                            Vector3 newupdatedSheepPosition = new Vector3(sheepPosition.x - fx, sheepPosition.y - fy, sheepPosition.z);
                            if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                            {
                                updatedSheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(updatedSheepPosition);
                                allPositions.Add(updatedSheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == sheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == sheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else if (!IsPositionInList(newupdatedSheepPosition, huntPositions) && !IsPositionInList(newupdatedSheepPosition, dogPositions))
                            {
                                updatedSheepPositions.Add(newupdatedSheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(newupdatedSheepPosition);
                                allPositions.Add(newupdatedSheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == sheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == sheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else
                            {
                                updatedSheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                            }
                        }
                        else if (Mathf.Abs(Fy) >= 2.0f)
                        {
                            // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                            Vector3 updatedSheepPosition = new Vector3(sheepPosition.x, sheepPosition.y - Fy, sheepPosition.z);
                            Vector3 newupdatedSheepPosition = new Vector3(sheepPosition.x - fx, sheepPosition.y - fy, sheepPosition.z);
                            if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                            {
                                updatedSheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(updatedSheepPosition);
                                allPositions.Add(updatedSheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == sheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == sheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else if (!IsPositionInList(newupdatedSheepPosition, huntPositions) && !IsPositionInList(newupdatedSheepPosition, dogPositions))
                            {
                                updatedSheepPositions.Add(newupdatedSheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(newupdatedSheepPosition);
                                allPositions.Add(newupdatedSheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == sheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == sheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else
                            {
                                updatedSheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                            }
                        }
                        else if (Mathf.Abs(Fx) >= 1.0f)
                        {
                            // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                            Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y - Fy, sheepPosition.z);
                            if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                            {
                                updatedSheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(updatedSheepPosition);
                                allPositions.Add(updatedSheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == sheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == sheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else
                            {
                                updatedSheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                            }
                        }
                        else if (Mathf.Abs(Fy) >= 1.0f)
                        {
                            // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                            Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y - Fy, sheepPosition.z);
                            if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                            {
                                updatedSheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(updatedSheepPosition);
                                allPositions.Add(updatedSheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == sheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == sheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else
                            {
                                updatedSheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("stop2" + sheepPosition);
                        // 如果不需要移動，則保持原位置
                        updatedSheepPositions.Add(sheepPosition);
                    }
                    if (SheepcoinList[roundCount] == 3)
                    {
                        totalCoins = totalCoins + 5;
                        SheepcoinList[roundCount] = 0;
                        sheepEaten++;
                    }
                    roundCount++;
                }
            }
            done = false;
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            if (grassPositions.Count == 1)
            {
                foreach (Vector3 updatedSheepPosition in updatedSheepPositions)
                {
                    Debug.Log("grassPositions.Count =" + grassPositions.Count);
                    float nearestDistance = float.MaxValue;
                    float secondNearestDistance = float.MaxValue; // 初始化第二近距離為最大值
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // 初始化第二近位置為零向量
                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(updatedSheepPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // 將目前最近的設為第二近
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // 更新最近的位置和距離
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // 更新第二近的位置和距離
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }

                    float Fx = updatedSheepPosition.x - nearestPosition.x;
                    float Fy = updatedSheepPosition.y - nearestPosition.y;
                    if (Fx == Fy)
                    {
                        grassPositions.RemoveAt(0);
                        done = true;
                    }
                }
                if (done)
                {
                    foreach (Vector3 updatedSheepPosition in updatedSheepPositions)
                    {
                        sheepPositions.Add(updatedSheepPosition);
                    }
                }
            }
            if (!done)
            {
                int roundCount = 0;
                foreach (Vector3 updatedSheepPosition in updatedSheepPositions)
                {
                    Debug.Log("currentRound" + currentRound + " grassPositions.Count =" + grassPositions.Count);
                    float nearestDistance = float.MaxValue;
                    float secondNearestDistance = float.MaxValue; // 初始化第二近距離為最大值
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // 初始化第二近位置為零向量

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(updatedSheepPosition, grassPosition);
                        if (distance < nearestDistance)
                        {
                            // 將目前最近的設為第二近
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // 更新最近的位置和距離
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // 更新第二近的位置和距離
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }

                    // 現在 nearestGrassPosition 就是每一隻羊最近的草的座標
                    //Debug.Log("Nearest grass to sheep at " + updatedSheepPosition + " is at " + nearestPosition);
                    
                    float Fx = updatedSheepPosition.x - nearestPosition.x;
                    float fx = Fx;
                    float Fy = updatedSheepPosition.y - nearestPosition.y;
                    float fy = Fy;
                    if (Fx > 2.0f)
                    {
                        Fx = 2.0f;
                    }
                    else if (Fx < -2.0f)
                    {
                        Fx = -2.0f;
                    }
                    if (Fy > 2.0f)
                    {
                        Fy = 2.0f;
                    }
                    else if (Fy < -2.0f)
                    {
                        Fy = -2.0f;
                    }
                    if (Fx > 0)
                    {
                        fx--;
                    }
                    else
                    {
                        fx++;
                    }
                    if (Fy > 0)
                    {
                        fy--;
                    }
                    else
                    {
                        fy++;
                    }
                    if (grassPositions.Count == 0)
                    {
                        sheepPositions.Add(updatedSheepPosition);
                    }
                    else if (Mathf.Abs(Fx) >= 2.0f)
                    {
                        Fy = 2.0f;
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y, updatedSheepPosition.z);
                        Vector3 newsheepPosition = new Vector3(updatedSheepPosition.x - fx, updatedSheepPosition.y - fy, updatedSheepPosition.z);
                        if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                        {
                            sheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(sheepPosition);
                            allPositions.Add(sheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == updatedSheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedSheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else if (!IsPositionInList(newsheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                        {
                            sheepPositions.Add(newsheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(newsheepPosition);
                            allPositions.Add(newsheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == sheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == sheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            sheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                        Vector3 sheepPosition = new Vector3(updatedSheepPosition.x, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                        Vector3 newsheepPosition = new Vector3(updatedSheepPosition.x - fx, updatedSheepPosition.y - fy, updatedSheepPosition.z);
                        if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                        {
                            sheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(sheepPosition);
                            allPositions.Add(sheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == updatedSheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedSheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else if (!IsPositionInList(newsheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                        {
                            sheepPositions.Add(newsheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(newsheepPosition);
                            allPositions.Add(newsheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == sheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == sheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            sheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                        if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                        {
                            sheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(sheepPosition);
                            allPositions.Add(sheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == updatedSheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedSheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            sheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                        Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                        if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                        {
                            sheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                            huntPositions.Add(sheepPosition);
                            allPositions.Add(sheepPosition);
                            for (int i = huntPositions.Count - 1; i >= 0; i--)
                            {
                                if (huntPositions[i] == updatedSheepPosition)
                                {
                                    huntPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedSheepPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }

                        }
                        else
                        {
                            sheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Fx == 0f && Fy == 0f)
                    {
                        for (int i = grassPositions.Count - 1; i >= 0; i--)
                        {
                            if (grassPositions[i] == nearestPosition)
                            {
                                grassPositions.RemoveAt(i);
                            }
                        }
                        Debug.Log("Eat!");
                        SheepcoinList[roundCount]++;
                        grassEaten++;
                        Fx = updatedSheepPosition.x - secondNearestPosition.x;
                        fx = Fx;
                        Fy = updatedSheepPosition.y - secondNearestPosition.y;
                        fy = Fy;
                        if (Fx > 2.0f)
                        {
                            Fx = 2.0f;
                        }
                        else if (Fx < -2.0f)
                        {
                            Fx = -2.0f;
                        }
                        if (Fy > 2.0f)
                        {
                            Fy = 2.0f;
                        }
                        else if (Fy < -2.0f)
                        {
                            Fy = -2.0f;
                        }
                        if (Fx > 0)
                        {
                            fx--;
                        }
                        else
                        {
                            fx++;
                        }
                        if (Fy > 0)
                        {
                            fy--;
                        }
                        else
                        {
                            fy++;
                        }
                        if (Mathf.Abs(Fx) >= 2.0f)
                        {
                            Fy = 2.0f;
                            // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                            Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y, updatedSheepPosition.z);
                            Vector3 newsheepPosition = new Vector3(updatedSheepPosition.x - fx, updatedSheepPosition.y - fy, updatedSheepPosition.z);
                            if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                            {
                                sheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(sheepPosition);
                                allPositions.Add(sheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == updatedSheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == updatedSheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else if (!IsPositionInList(newsheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                            {
                                sheepPositions.Add(newsheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(newsheepPosition);
                                allPositions.Add(newsheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == sheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == sheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else
                            {
                                sheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                            }
                        }
                        else if (Mathf.Abs(Fy) >= 2.0f)
                        {
                            // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                            Vector3 sheepPosition = new Vector3(updatedSheepPosition.x, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                            Vector3 newsheepPosition = new Vector3(updatedSheepPosition.x - fx, updatedSheepPosition.y - fy, updatedSheepPosition.z);
                            if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                            {
                                sheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(sheepPosition);
                                allPositions.Add(sheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == updatedSheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == updatedSheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else if (!IsPositionInList(newsheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                            {
                                sheepPositions.Add(newsheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(newsheepPosition);
                                allPositions.Add(newsheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == sheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == sheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else
                            {
                                sheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                            }
                        }
                        else if (Mathf.Abs(Fx) >= 1.0f)
                        {
                            // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                            Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                            if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                            {
                                sheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(sheepPosition);
                                allPositions.Add(sheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == updatedSheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == updatedSheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }
                            }
                            else
                            {
                                sheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                            }
                        }
                        else if (Mathf.Abs(Fy) >= 1.0f)
                        {
                            // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                            Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                            if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                            {
                                sheepPositions.Add(sheepPosition); // 將更新後的位置添加到 List 中
                                huntPositions.Add(sheepPosition);
                                allPositions.Add(sheepPosition);
                                for (int i = huntPositions.Count - 1; i >= 0; i--)
                                {
                                    if (huntPositions[i] == updatedSheepPosition)
                                    {
                                        huntPositions.RemoveAt(i);
                                    }
                                }
                                for (int i = allPositions.Count - 1; i >= 0; i--)
                                {
                                    if (allPositions[i] == updatedSheepPosition)
                                    {
                                        allPositions.RemoveAt(i);
                                    }
                                }

                            }
                            else
                            {
                                sheepPositions.Add(updatedSheepPosition); // 將更新後的位置添加到 List 中
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("stop2" + updatedSheepPosition);
                        // 如果不需要移動，則保持原位置
                        sheepPositions.Add(updatedSheepPosition);
                    }
                    if (SheepcoinList[roundCount] == 3)
                    {
                        totalCoins = totalCoins + 5;
                        SheepcoinList[roundCount] = 0;
                        sheepEaten++;
                    }
                    roundCount++;
                }
            }
            done = false;
        }
        if (currentRound % 2 == 0)
        {
            Debug.Log("currentRound" + currentRound + "updatedSheepPositions.Count = " + updatedSheepPositions.Count);
            //CreateSheep();
            grassList.Clear();
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            Debug.Log("currentRound" + currentRound + "SheepPositions.Count = " + sheepPositions.Count);
            //CreateSheep();
            grassList.Clear();
        }


    }
    public void Wolf()
    {
        for (int i = 0; i < wolfPositions.Count; i++)
        {
            Vector3 position = wolfPositions[i];
            //Debug.Log("Wolf Position " + i + ": " + position);
        }
        for (int i = 0; i < huntPositions.Count; i++)
        {
            Vector3 position = huntPositions[i];
            //Debug.Log("Hunt Position " + i + ": " + position);
        }
        if (currentRound % 2 == 0)//currentRound ==2
        {
            foreach (Vector3 wolfPosition in wolfPositions)
            {
                float nearestDistance = float.MaxValue;
                float secondNearestDistance = float.MaxValue; // 初始化第二近距離為最大值
                Vector3 nearestPosition = Vector3.zero;
                Vector3 secondNearestPosition = Vector3.zero; // 初始化第二近位置為零向量

                foreach (Vector3 huntPosition in huntPositions)
                {
                    float distance = Vector3.Distance(wolfPosition, huntPosition);
                    if (distance < nearestDistance)
                    {
                        // 將目前最近的設為第二近
                        secondNearestDistance = nearestDistance;
                        secondNearestPosition = nearestPosition;

                        // 更新最近的位置和距離
                        nearestDistance = distance;
                        nearestPosition = huntPosition;
                    }
                    else if (distance < secondNearestDistance)
                    {
                        // 更新第二近的位置和距離
                        secondNearestDistance = distance;
                        secondNearestPosition = huntPosition;
                    }
                }

                // 現在 nearestHuntPosition 就是每一隻狼最近的獵物的座標
                Debug.Log("Nearest hunt to wolf at " + wolfPosition + " is at " + nearestPosition);

                float Fx = wolfPosition.x - nearestPosition.x;
                float fx = Fx;
                float fX = fx;
                float Fy = wolfPosition.y - nearestPosition.y;
                float fy = Fy;
                float fY = fy;
                if (Fx > 3.0f)
                {
                    Fx = 3.0f;
                }
                else if (Fx < -3.0f)
                {
                    Fx = -3.0f;
                }
                if (Fy > 3.0f)
                {
                    Fy = 3.0f;
                }
                else if (Fy < -3.0f)
                {
                    Fy = -3.0f;
                }
                if (Fx > 0)
                {
                    fx--;
                }
                else
                {
                    fx++;
                }
                if (Fy > 0)
                {
                    fy--;
                }
                else
                {
                    fy++;
                }
                if (fx > 0)
                {
                    fX--;
                }
                else
                {
                    fx++;
                }
                if (fy > 0)
                {
                    fY--;
                }
                else
                {
                    fY++;
                }
                if (huntPositions.Count == 0)
                {
                    updatedWolfPositions.Add(wolfPosition);
                }
                else if (Mathf.Abs(Fx) >= 3.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y, wolfPosition.z);
                    Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - fY, wolfPosition.z);
                    Vector3 nextupdatedWolfPosition = new Vector3(wolfPosition.x - fX, wolfPosition.y - fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(updatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(updatedWolfPosition);
                    }
                    else if (!IsPositionInList(newupdatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(newupdatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(newupdatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(newupdatedWolfPosition);
                    }
                    else if (!IsPositionInList(nextupdatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(nextupdatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(nextupdatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(nextupdatedWolfPosition);
                    }
                    else
                    {
                        updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Mathf.Abs(Fy) >= 3.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x, wolfPosition.y - Fy, wolfPosition.z);
                    Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - fY, wolfPosition.z);
                    Vector3 nextupdatedWolfPosition = new Vector3(wolfPosition.x - fX, wolfPosition.y - fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(updatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(updatedWolfPosition);
                    }
                    else if (!IsPositionInList(newupdatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(newupdatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(newupdatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(newupdatedWolfPosition);
                    }
                    else if (!IsPositionInList(nextupdatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(nextupdatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(nextupdatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(nextupdatedWolfPosition);
                    }
                    else
                    {
                        updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Mathf.Abs(Fx) >= 2.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - fy, wolfPosition.z);
                    Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - Fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(updatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(updatedWolfPosition);
                    }
                    else if (!IsPositionInList(newupdatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(newupdatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(newupdatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(newupdatedWolfPosition);
                    }
                    else
                    {
                        updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Mathf.Abs(Fy) >= 2.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - Fy, wolfPosition.z);
                    Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(updatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(updatedWolfPosition);
                    }
                    else if (!IsPositionInList(newupdatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(newupdatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(newupdatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(newupdatedWolfPosition);
                    }
                    else
                    {
                        updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Mathf.Abs(Fx) >= 1.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - Fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(updatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(updatedWolfPosition);
                    }
                    else
                    {
                        updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Mathf.Abs(Fy) >= 1.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - Fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(updatedWolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == wolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == wolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(updatedWolfPosition);
                    }
                    else
                    {
                        updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Fx == 0f && Fy == 0f)
                {
                    for (int i = huntPositions.Count - 1; i >= 0; i--)
                    {
                        if (huntPositions[i] == nearestPosition)
                        {
                            huntPositions.RemoveAt(i);
                        }
                    }
                    Debug.Log("Wolf Hunt!");
                    Fx = wolfPosition.x - secondNearestPosition.x;
                    fx = Fx;
                    fX = fx;
                    Fy = wolfPosition.y - secondNearestPosition.y;
                    fy = Fy;
                    fY = fy;
                    if (Fx > 3.0f)
                    {
                        Fx = 3.0f;
                    }
                    else if (Fx < -3.0f)
                    {
                        Fx = -3.0f;
                    }
                    if (Fy > 3.0f)
                    {
                        Fy = 3.0f;
                    }
                    else if (Fy < -3.0f)
                    {
                        Fy = -3.0f;
                    }
                    if (Fx > 0)
                    {
                        fx--;
                    }
                    else
                    {
                        fx++;
                    }
                    if (Fy > 0)
                    {
                        fy--;
                    }
                    else
                    {
                        fy++;
                    }
                    if (fx > 0)
                    {
                        fX--;
                    }
                    else
                    {
                        fx++;
                    }
                    if (fy > 0)
                    {
                        fY--;
                    }
                    else
                    {
                        fY++;
                    }
                    if (Mathf.Abs(Fx) >= 3.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y, wolfPosition.z);
                        Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - fY, wolfPosition.z);
                        Vector3 nextupdatedWolfPosition = new Vector3(wolfPosition.x - fX, wolfPosition.y - fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(updatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedWolfPosition);
                        }
                        else if (!IsPositionInList(newupdatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(newupdatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(newupdatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newupdatedWolfPosition);
                        }
                        else if (!IsPositionInList(nextupdatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(nextupdatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(nextupdatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(nextupdatedWolfPosition);
                        }
                        else
                        {
                            updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 3.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x, wolfPosition.y - Fy, wolfPosition.z);
                        Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - fY, wolfPosition.z);
                        Vector3 nextupdatedWolfPosition = new Vector3(wolfPosition.x - fX, wolfPosition.y - fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(updatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedWolfPosition);
                        }
                        else if (!IsPositionInList(newupdatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(newupdatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(newupdatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newupdatedWolfPosition);
                        }
                        else if (!IsPositionInList(nextupdatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(nextupdatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(nextupdatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(nextupdatedWolfPosition);
                        }
                        else
                        {
                            updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - fy, wolfPosition.z);
                        Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - Fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(updatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedWolfPosition);
                        }
                        else if (!IsPositionInList(newupdatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(newupdatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(newupdatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newupdatedWolfPosition);
                        }
                        else
                        {
                            updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - Fy, wolfPosition.z);
                        Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(updatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedWolfPosition);
                        }
                        else if (!IsPositionInList(newupdatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(newupdatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(newupdatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newupdatedWolfPosition);
                        }
                        else
                        {
                            updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - Fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(updatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedWolfPosition);
                        }
                        else
                        {
                            updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - Fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(updatedWolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == wolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == wolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedWolfPosition);
                        }
                        else
                        {
                            updatedWolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                }
                else
                {
                    Debug.Log("stop2" + wolfPosition);
                    // 如果不需要移動，則保持原位置
                    updatedWolfPositions.Add(wolfPosition);
                }
                PositionWolf = wolfPosition;
                Debug.Log("wPositionWolf " + PositionWolf);
            }
            foreach (Vector3 lambPosition in lambPositions)
            {
                if (lambPosition.x == PositionWolf.x && lambPosition.y == PositionWolf.y)
                {
                    Debug.Log("Lamb Yummy!");
                    wolfAteLamb++;
                    eat++;
                }
            }
            for (int i = lambPositions.Count - 1; i >= 0; i--)
            {
                if (lambPositions[i] == PositionWolf)
                {
                    lambPositions.RemoveAt(i);
                }
            }
            for (int i = huntPositions.Count - 1; i >= 0; i--)
            {
                if (huntPositions[i] == PositionWolf)
                {
                    huntPositions.RemoveAt(i);
                }
            }
            foreach (Vector3 sheepPosition in sheepPositions)
            {
                if (sheepPosition.x == PositionWolf.x && sheepPosition.y == PositionWolf.y)
                {
                    Debug.Log("Sheep Yummy!");
                    wolfAteSheep++;
                    eat++;
                }
            }
            for (int i = sheepPositions.Count - 1; i >= 0; i--)
            {
                if (sheepPositions[i] == PositionWolf)
                {
                    sheepPositions.RemoveAt(i);
                }
            }
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            foreach (Vector3 updatedWolfPosition in updatedWolfPositions)
            {
                float nearestDistance = float.MaxValue;
                float secondNearestDistance = float.MaxValue; // 初始化第二近距離為最大值
                Vector3 nearestPosition = Vector3.zero;
                Vector3 secondNearestPosition = Vector3.zero; // 初始化第二近位置為零向量

                foreach (Vector3 huntPosition in huntPositions)
                {
                    float distance = Vector3.Distance(updatedWolfPosition, huntPosition);
                    if (distance < nearestDistance)
                    {
                        // 將目前最近的設為第二近
                        secondNearestDistance = nearestDistance;
                        secondNearestPosition = nearestPosition;

                        // 更新最近的位置和距離
                        nearestDistance = distance;
                        nearestPosition = huntPosition;
                    }
                    else if (distance < secondNearestDistance)
                    {
                        // 更新第二近的位置和距離
                        secondNearestDistance = distance;
                        secondNearestPosition = huntPosition;
                    }
                }

                // 現在 nearestHuntPosition 就是每一隻狼最近的獵物的座標
                Debug.Log("Nearest hunt to wolf at " + updatedWolfPosition + " is at " + nearestPosition);
                
                float Fx = updatedWolfPosition.x - nearestPosition.x;
                float fx = Fx;
                float fX = fx;
                float Fy = updatedWolfPosition.y - nearestPosition.y;
                float fy = Fy;
                float fY = fy;
                if (Fx > 3.0f)
                {
                    Fx = 3.0f;
                }
                else if (Fx < -3.0f)
                {
                    Fx = -3.0f;
                }
                if (Fy > 3.0f)
                {
                    Fy = 3.0f;
                }
                else if (Fy < -3.0f)
                {
                    Fy = -3.0f;
                }
                if (Fx > 0)
                {
                    fx--;
                }
                else
                {
                    fx++;
                }
                if (Fy > 0)
                {
                    fy--;
                }
                else
                {
                    fy++;
                }
                if (fx > 0)
                {
                    fX--;
                }
                else
                {
                    fx++;
                }
                if (fy > 0)
                {
                    fY--;
                }
                else
                {
                    fY++;
                }
                if (huntPositions.Count == 0)
                {
                    wolfPositions.Add(updatedWolfPosition);
                }
                else if (Mathf.Abs(Fx) >= 3.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y, updatedWolfPosition.z);
                    Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - fY, updatedWolfPosition.z);
                    Vector3 nextwolfPosition = new Vector3(updatedWolfPosition.x - fX, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(wolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(wolfPosition);
                    }
                    else if (!IsPositionInList(newwolfPosition, grassPositions))
                    {
                        wolfPositions.Add(newwolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(newwolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(newwolfPosition);
                    }
                    else if (!IsPositionInList(nextwolfPosition, grassPositions))
                    {
                        wolfPositions.Add(nextwolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(nextwolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(nextwolfPosition);
                    }
                    else
                    {
                        wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Mathf.Abs(Fy) >= 3.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                    Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - fY, updatedWolfPosition.z);
                    Vector3 nextwolfPosition = new Vector3(updatedWolfPosition.x - fX, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(wolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(wolfPosition);
                    }
                    else if (!IsPositionInList(newwolfPosition, grassPositions))
                    {
                        wolfPositions.Add(newwolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(newwolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(newwolfPosition);
                    }
                    else if (!IsPositionInList(nextwolfPosition, grassPositions))
                    {
                        wolfPositions.Add(nextwolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(nextwolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(nextwolfPosition);
                    }
                    else
                    {
                        wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Mathf.Abs(Fx) >= 2.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                    Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(wolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(wolfPosition);
                    }
                    else if (!IsPositionInList(newwolfPosition, grassPositions))
                    {
                        wolfPositions.Add(newwolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(newwolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(newwolfPosition);
                    }
                    else
                    {
                        wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Mathf.Abs(Fy) >= 2.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                    Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(wolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(wolfPosition);
                    }
                    else if (!IsPositionInList(newwolfPosition, grassPositions))
                    {
                        wolfPositions.Add(newwolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(newwolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(newwolfPosition);
                    }
                    else
                    {
                        wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Mathf.Abs(Fx) >= 1.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(wolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(wolfPosition);
                    }
                    else
                    {
                        wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Mathf.Abs(Fy) >= 1.0f)
                {
                    // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                        runPositions.Add(wolfPosition);
                        for (int i = runPositions.Count - 1; i >= 0; i--)
                        {
                            if (runPositions[i] == updatedWolfPosition)
                            {
                                runPositions.RemoveAt(i);
                            }
                        }
                        for (int i = allPositions.Count - 1; i >= 0; i--)
                        {
                            if (allPositions[i] == updatedWolfPosition)
                            {
                                allPositions.RemoveAt(i);
                            }
                        }
                        allPositions.Add(wolfPosition);

                    }
                    else
                    {
                        wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                    }
                }
                else if (Fx == 0f && Fy == 0f)
                {
                    for (int i = huntPositions.Count - 1; i >= 0; i--)
                    {
                        if (huntPositions[i] == nearestPosition)
                        {
                            huntPositions.RemoveAt(i);
                        }
                    }
                    Debug.Log("Eat!");
                    Fx = updatedWolfPosition.x - secondNearestPosition.x;
                    fx = Fx;
                    fX = fx;
                    Fy = updatedWolfPosition.y - secondNearestPosition.y;
                    fy = Fy;
                    fY = fy;
                    if (Fx > 3.0f)
                    {
                        Fx = 3.0f;
                    }
                    else if (Fx < -3.0f)
                    {
                        Fx = -3.0f;
                    }
                    if (Fy > 3.0f)
                    {
                        Fy = 3.0f;
                    }
                    else if (Fy < -3.0f)
                    {
                        Fy = -3.0f;
                    }
                    if (Fx > 0)
                    {
                        fx--;
                    }
                    else
                    {
                        fx++;
                    }
                    if (Fy > 0)
                    {
                        fy--;
                    }
                    else
                    {
                        fy++;
                    }
                    if (fx > 0)
                    {
                        fX--;
                    }
                    else
                    {
                        fx++;
                    }
                    if (fy > 0)
                    {
                        fY--;
                    }
                    else
                    {
                        fY++;
                    }
                    if (Mathf.Abs(Fx) >= 3.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y, updatedWolfPosition.z);
                        Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - fY, updatedWolfPosition.z);
                        Vector3 nextwolfPosition = new Vector3(updatedWolfPosition.x - fX, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(wolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(wolfPosition);
                        }
                        else if (!IsPositionInList(newwolfPosition, grassPositions))
                        {
                            wolfPositions.Add(newwolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(newwolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newwolfPosition);
                        }
                        else if (!IsPositionInList(nextwolfPosition, grassPositions))
                        {
                            wolfPositions.Add(nextwolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(nextwolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(nextwolfPosition);
                        }
                        else
                        {
                            wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 3.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                        Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - fY, updatedWolfPosition.z);
                        Vector3 nextwolfPosition = new Vector3(updatedWolfPosition.x - fX, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(wolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(wolfPosition);
                        }
                        else if (!IsPositionInList(newwolfPosition, grassPositions))
                        {
                            wolfPositions.Add(newwolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(newwolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newwolfPosition);
                        }
                        else if (!IsPositionInList(nextwolfPosition, grassPositions))
                        {
                            wolfPositions.Add(nextwolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(nextwolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(nextwolfPosition);
                        }
                        else
                        {
                            wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                        Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(wolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(wolfPosition);
                        }
                        else if (!IsPositionInList(newwolfPosition, grassPositions))
                        {
                            wolfPositions.Add(newwolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(newwolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newwolfPosition);
                        }
                        else
                        {
                            wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                        Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(wolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(wolfPosition);
                        }
                        else if (!IsPositionInList(newwolfPosition, grassPositions))
                        {
                            wolfPositions.Add(newwolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(newwolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newwolfPosition);
                        }
                        else
                        {
                            wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(wolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(wolfPosition);
                        }
                        else
                        {
                            wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // 將更新後的位置添加到 List 中
                            runPositions.Add(wolfPosition);
                            for (int i = runPositions.Count - 1; i >= 0; i--)
                            {
                                if (runPositions[i] == updatedWolfPosition)
                                {
                                    runPositions.RemoveAt(i);
                                }
                            }
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedWolfPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(wolfPosition);

                        }
                        else
                        {
                            wolfPositions.Add(updatedWolfPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                }
                else
                {
                    Debug.Log("stop2" + updatedWolfPosition);
                    // 如果不需要移動，則保持原位置
                    wolfPositions.Add(updatedWolfPosition);
                }
                PositionWolf = updatedWolfPosition;
                Debug.Log("wPositionWolf " + PositionWolf);
            }
            foreach (Vector3 updatedLambPosition in updatedLambPositions)
            {
                if (updatedLambPosition.x == PositionWolf.x && updatedLambPosition.y == PositionWolf.y)
                {
                    Debug.Log("Lamb Yummy!");
                    wolfAteLamb++;
                    eat++;
                }
            }
            for (int i = updatedLambPositions.Count - 1; i >= 0; i--)
            {
                if (updatedLambPositions[i] == PositionWolf)
                {
                    updatedLambPositions.RemoveAt(i);
                }
            }
            for (int i = huntPositions.Count - 1; i >= 0; i--)
            {
                if (huntPositions[i] == PositionWolf)
                {
                    huntPositions.RemoveAt(i);
                }
            }
            foreach (Vector3 updatedSheepPosition in updatedSheepPositions)
            {
                if (updatedSheepPosition.x == PositionWolf.x && updatedSheepPosition.y == PositionWolf.y)
                {
                    Debug.Log("Sheep Yummy!");
                    wolfAteSheep++;
                    eat++;
                }
            }
            for (int i = updatedSheepPositions.Count - 1; i >= 0; i--)
            {
                if (updatedSheepPositions[i] == PositionWolf)
                {
                    updatedSheepPositions.RemoveAt(i);
                }
            }
        }
        if (currentRound % 2 == 0)
        {
            Debug.Log("updatedWolfPositions.Count = " + updatedWolfPositions.Count);
            //CreateWolf();
            huntList.Clear();
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            Debug.Log("WolfPositions.Count = " + wolfPositions.Count);
            //CreateWolf();
            huntList.Clear();
        }
    }
    public void Dog()
    {
        Debug.Log("WolfCount = " + runPositions.Count);
        for (int i = 0; i < dogPositions.Count; i++)
        {
            Vector3 position = dogPositions[i];
            //Debug.Log("Dog Position " + i + ": " + position);
        }
        for (int i = 0; i < wolfPositions.Count; i++)
        {
            Vector3 position = wolfPositions[i];
            Debug.Log("Wolf Position " + i + ": " + position);
        }
        for (int i = 0; i < runPositions.Count; i++)
        {
            Vector3 position = runPositions[i];
            //Debug.Log("Wolf Position " + i + ": " + position);
        }
        if (currentRound % 2 == 0)//currentRound ==2
        {
            foreach (Vector3 dogPosition in dogPositions)
            {
                // 現在 nearestWolfPosition 就是每一隻狼最近的獵物的座標
                Debug.Log("Nearest wolf to dog at " + dogPosition + " is at " + PositionWolf);
                if (runPositions.Count == 0)
                {
                    updatedDogPositions.Add(dogPosition);
                }
                else
                {
                    float Fx = dogPosition.x - runPositions[0].x;
                    float fx = Fx;
                    float fX = fx;
                    float FX = fX;
                    float Fy = dogPosition.y - runPositions[0].y;
                    float fy = Fy;
                    float fY = fy;
                    float FY = fY;
                    if (Fx > 4.0f)
                    {
                        Fx = 4.0f;
                    }
                    else if (Fx < -4.0f)
                    {
                        Fx = -4.0f;
                    }
                    if (Fy > 4.0f)
                    {
                        Fy = 4.0f;
                    }
                    else if (Fy < -4.0f)
                    {
                        Fy = -4.0f;
                    }
                    if (Fx > 0)
                    {
                        fx--;
                    }
                    else
                    {
                        fx++;
                    }
                    if (Fy > 0)
                    {
                        fy--;
                    }
                    else
                    {
                        fy++;
                    }
                    if (fx > 0)
                    {
                        fY--;
                    }
                    else
                    {
                        fx++;
                    }
                    if (fy > 0)
                    {
                        fY--;
                    }
                    else
                    {
                        fY++;
                    }
                    if (FX > 0)
                    {
                        FX--;
                    }
                    else
                    {
                        FX++;
                    }
                    if (FY > 0)
                    {
                        FY--;
                    }
                    else
                    {
                        FY++;
                    }
                    if (Mathf.Abs(Fx) >= 4.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - FY, dogPosition.z);
                        Vector3 nextupdatedDogPosition = new Vector3(dogPosition.x - FX, dogPosition.y - fy, dogPosition.z);
                        Vector3 finalupdatedDogPosition = new Vector3(dogPosition.x - fX, dogPosition.y - fY, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedDogPosition);
                        }
                        else if (!IsPositionInList(newupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(newupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newupdatedDogPosition);
                        }
                        else if (!IsPositionInList(nextupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(nextupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(nextupdatedDogPosition);
                        }
                        else if (!IsPositionInList(finalupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(finalupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(finalupdatedDogPosition);
                        }
                        else
                        {
                            updatedDogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 4.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x, dogPosition.y - Fy, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - FY, dogPosition.z);
                        Vector3 nextupdatedDogPosition = new Vector3(dogPosition.x - FX, dogPosition.y - fy, dogPosition.z);
                        Vector3 finalupdatedDogPosition = new Vector3(dogPosition.x - fX, dogPosition.y - fY, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedDogPosition);
                        }
                        else if (!IsPositionInList(newupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(newupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newupdatedDogPosition);
                        }
                        else if (!IsPositionInList(nextupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(nextupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(nextupdatedDogPosition);
                        }
                        else if (!IsPositionInList(finalupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(finalupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(finalupdatedDogPosition);
                        }
                        else
                        {
                            updatedDogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 3.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - fY, dogPosition.z);
                        Vector3 nextupdatedDogPosition = new Vector3(dogPosition.x - fX, dogPosition.y - fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedDogPosition);
                        }
                        else if (!IsPositionInList(newupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(newupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newupdatedDogPosition);
                        }
                        else if (!IsPositionInList(nextupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(nextupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(nextupdatedDogPosition);
                        }
                        else
                        {
                            updatedDogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 3.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x, dogPosition.y - Fy, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - fY, dogPosition.z);
                        Vector3 nextupdatedDogPosition = new Vector3(dogPosition.x - fX, dogPosition.y - fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedDogPosition);
                        }
                        else if (!IsPositionInList(newupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(newupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newupdatedDogPosition);
                        }
                        else if (!IsPositionInList(nextupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(nextupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(nextupdatedDogPosition);
                        }
                        else
                        {
                            updatedDogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y - fy, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - Fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedDogPosition);
                        }
                        else if (!IsPositionInList(newupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(newupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newupdatedDogPosition);
                        }
                        else
                        {
                            updatedDogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - Fy, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y - fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedDogPosition);
                        }
                        else if (!IsPositionInList(newupdatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(newupdatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newupdatedDogPosition);
                        }
                        else
                        {
                            updatedDogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y - Fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedDogPosition);
                        }
                        else
                        {
                            updatedDogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y - Fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == dogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(updatedDogPosition);
                        }
                        else
                        {
                            updatedDogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Fx == 0f && Fy == 0f)
                    {
                        eat = 3;
                        Debug.Log("Wolf Bye!");
                        dogAttackedWolf++;
                        updatedDogPositions.Add(dogPosition);
                        //runPositions.RemoveAt(0);
                        //updatedWolfPositions.RemoveAt(0);
                    }
                    else
                    {
                        Debug.Log("stop2" + dogPosition);
                        // 如果不需要移動，則保持原位置
                        updatedDogPositions.Add(dogPosition);
                    }

                }
                PositionDog = dogPosition;
                Debug.Log(PositionDog);
            }
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            foreach (Vector3 updatedDogPosition in updatedDogPositions)
            {
                // 現在 nearestHuntPosition 就是每一隻狗最近的狼的座標
                Debug.Log("Nearest wolf to dog at " + updatedDogPosition + " is at " + PositionWolf);
                if (runPositions.Count == 0)
                {
                    dogPositions.Add(updatedDogPosition);
                }
                else
                {
                    float Fx = updatedDogPosition.x - runPositions[0].x;
                    float fx = Fx;
                    float fX = fx;
                    float FX = fX;
                    float Fy = updatedDogPosition.y - runPositions[0].y;
                    float fy = Fy;
                    float fY = fy;
                    float FY = fY;
                    if (Fx > 4.0f)
                    {
                        Fx = 4.0f;
                    }
                    else if (Fx < -4.0f)
                    {
                        Fx = -4.0f;
                    }
                    if (Fy > 4.0f)
                    {
                        Fy = 4.0f;
                    }
                    else if (Fy < -4.0f)
                    {
                        Fy = -4.0f;
                    }
                    if (Fx > 0)
                    {
                        fx--;
                    }
                    else
                    {
                        fx++;
                    }
                    if (Fy > 0)
                    {
                        fy--;
                    }
                    else
                    {
                        fy++;
                    }
                    if (fx > 0)
                    {
                        fY--;
                    }
                    else
                    {
                        fx++;
                    }
                    if (fy > 0)
                    {
                        fY--;
                    }
                    else
                    {
                        fY++;
                    }
                    if (FX > 0)
                    {
                        FX--;
                    }
                    else
                    {
                        FX++;
                    }
                    if (FY > 0)
                    {
                        FY--;
                    }
                    else
                    {
                        FY++;
                    }
                    if (Mathf.Abs(Fx) >= 4.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - FY, updatedDogPosition.z);
                        Vector3 nextdogPosition = new Vector3(updatedDogPosition.x - FX, updatedDogPosition.y - fy, updatedDogPosition.z);
                        Vector3 finaldogPosition = new Vector3(updatedDogPosition.x - fX, updatedDogPosition.y - fY, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(dogPosition);
                        }
                        else if (!IsPositionInList(newdogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(newdogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newdogPosition);
                        }
                        else if (!IsPositionInList(nextdogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(nextdogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(nextdogPosition);
                        }
                        else if (!IsPositionInList(finaldogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(finaldogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(finaldogPosition);
                        }
                        else
                        {
                            dogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 4.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - FY, updatedDogPosition.z);
                        Vector3 nextdogPosition = new Vector3(updatedDogPosition.x - FX, updatedDogPosition.y - fy, updatedDogPosition.z);
                        Vector3 finaldogPosition = new Vector3(updatedDogPosition.x - fX, updatedDogPosition.y - fY, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(dogPosition);
                        }
                        else if (!IsPositionInList(newdogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(newdogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newdogPosition);
                        }
                        else if (!IsPositionInList(nextdogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(nextdogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(nextdogPosition);
                        }
                        else if (!IsPositionInList(finaldogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(finaldogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(finaldogPosition);
                        }
                        else
                        {
                            dogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 3.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - fY, updatedDogPosition.z);
                        Vector3 nextdogPosition = new Vector3(updatedDogPosition.x - fX, updatedDogPosition.y - fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(dogPosition);
                        }
                        else if (!IsPositionInList(newdogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(newdogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newdogPosition);
                        }
                        else if (!IsPositionInList(nextdogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(newdogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newdogPosition);
                        }
                        else
                        {
                            dogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 3.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - fY, updatedDogPosition.z);
                        Vector3 nextdogPosition = new Vector3(updatedDogPosition.x - fX, updatedDogPosition.y - fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(dogPosition);
                        }
                        else if (!IsPositionInList(newdogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(newdogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newdogPosition);
                        }
                        else if (!IsPositionInList(nextdogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(newdogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newdogPosition);
                        }
                        else
                        {
                            dogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y - fy, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(dogPosition);
                        }
                        else if (!IsPositionInList(newdogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(newdogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newdogPosition);
                        }
                        else
                        {
                            dogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y - fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(dogPosition);
                        }
                        else if (!IsPositionInList(newdogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(newdogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(newdogPosition);
                        }
                        else
                        {
                            dogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 x 座標
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(dogPosition);
                        }
                        else
                        {
                            dogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // 創建新的 Vector3 表示羊移動後的位置，只更新 y 座標
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // 將更新後的位置添加到 List 中
                            for (int i = allPositions.Count - 1; i >= 0; i--)
                            {
                                if (allPositions[i] == updatedDogPosition)
                                {
                                    allPositions.RemoveAt(i);
                                }
                            }
                            allPositions.Add(dogPosition);
                        }
                        else
                        {
                            dogPositions.Add(updatedDogPosition); // 將更新後的位置添加到 List 中
                        }
                    }
                    else if (Fx == 0f && Fy == 0f)
                    {
                        eat = 3;
                        Debug.Log("Wolf Bye!");
                        dogAttackedWolf++;
                        dogPositions.Add(updatedDogPosition);
                        //runPositions.RemoveAt(0);
                        //wolfPositions.RemoveAt(0);
                    }
                    else
                    {
                        Debug.Log("stop2" + updatedDogPosition);
                        // 如果不需要移動，則保持原位置
                        dogPositions.Add(updatedDogPosition);
                    }
                }
                PositionDog = updatedDogPosition;
                Debug.Log(PositionDog);
            }
        }
        if (currentRound % 2 == 0)
        {
            Debug.Log("updatedDogPositions.Count = " + updatedDogPositions.Count);
            //CreateDog();
            runList.Clear();
        }
        else if (currentRound % 2 == 1 && currentRound > 1)
        {
            Debug.Log("DogPositions.Count = " + dogPositions.Count);
            //CreateDog();
            runList.Clear();
        }
    }
    public void RunGameRounds(int c)//每一回合的執行邏輯內容
    {
        currentRound = c;
        DestroyDogNextTurn();
        DestroySheepNextTurn();
        DestroyLambNextTurn();
        Grass();
        Lamb();
        Sheep();
        if (eat == 3)//若狼吃了三隻羊或小羊
        {
            DestroyWolfNextTurn();
            eat = 0;
            waitRounds = Random.Range(7, 12);
            wolfPositions.Clear();
            updatedWolfPositions.Clear();
            runPositions.Clear();
            runList.Clear();
        }
        if (currentRound == 6)//第六回合才讓狼出現
        {
            DestroyWolfNextTurn();
            Wolf();
            CreateWolf();
        }
        else if (waitRounds == 1)
        {
            CreateObject(3, 1);
            Debug.Log("Wolf Ready to Out!");
            waitRounds--;
            signal = true;
        }
        else if (signal)
        {
            DestroyWolfNextTurn();
            Wolf();
            CreateWolf();
            signal = false;
        }
        else if (waitRounds == 0)
        {
            DestroyWolfNextTurn();
            Wolf();
            CreateWolf();
        }
        else
        {
            waitRounds--;
        }
        Dog();
        CreateDog();
        CreateSheep();
        CreateLamb();
        if (huntPositions.Count < 10)//當場內羊及小羊總數< 10隻時每回合隨機生5個草
        {
            if (grassPositions.Count < 15)
            {
                CreateObject(0, 5);
            }
            else
            {
                Debug.Log("Grass More than 15!");
            }

        }
        else if (huntPositions.Count >= 10)//當場內羊及小羊總數>=10隻時每回合隨機產生3個草
        {
            if (grassPositions.Count < 15)
            {
                CreateObject(0, 3);
            }
            else
            {
                Debug.Log("Grass More than 15!");
            }
        }
        if (huntPositions.Count <= 10 && currentRound % 5 == 0)//當場內羊及小羊總數<=10隻時每5回合會在地圖上隨機多2隻小羊
        {

            if (huntPositions.Count < 20)
            {
                CreateObject(1, 2);
            }
            else
            {
                Debug.Log("Hunt More than 20!");
            }
        }
        else if (huntPositions.Count > 10 && currentRound % 5 == 0)//當場內羊及小羊總數> 10隻時每5回合會在地圖上隨機多1隻小羊
        {
            if (huntPositions.Count < 20)
            {
                CreateObject(1, 1);
            }
            else
            {
                Debug.Log("Hunt More than 20!");
            }
        }
    }
    public void DisplayResults()
    {
        Debug.Log("草被吃的次數: " + grassEaten);
        Debug.Log("小羊吃滿草的次數: " + lambEaten);
        Debug.Log("羊吃滿草的次數: " + sheepEaten);
        Debug.Log("狼吃掉小羊的次數: " + wolfAteLamb);
        Debug.Log("狼吃掉羊的次數: " + wolfAteSheep);
        Debug.Log("犬攻擊狼的次數: " + dogAttackedWolf);
        Debug.Log("總共獲得金幣: " + totalCoins);
    }
}
