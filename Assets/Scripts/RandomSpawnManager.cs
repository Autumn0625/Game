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
    [SerializeField] GameObject[] objectPrefab; // �n�ͦ���Prefab
    private int numberOfObjectsToSpawn; // �n�ͦ�������ƶq
    Vector3 minPosition = new Vector3(-10, -10, 0); // �̤p�y��
    Vector3 maxPosition = new Vector3(10, 10, 0); // �̤j�y��

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


    private List<Vector3> allPositions = new List<Vector3>();//�Ҧ����󪺮y��
    private List<Vector3> grassPositions = new List<Vector3>();
    private List<Vector3> lambPositions = new List<Vector3>();
    private List<Vector3> sheepPositions = new List<Vector3>();
    private List<Vector3> huntPositions = new List<Vector3>();//�Ѧϸ�p�ϲզ����y��
    private List<Vector3> wolfPositions = new List<Vector3>();
    private List<Vector3> runPositions = new List<Vector3>();//�ѯT�զ����Q�X����
    private List<Vector3> dogPositions = new List<Vector3>();

    private Vector3 PositionWolf = new Vector3();
    private Vector3 PositionDog = new Vector3();

    private List<Vector3> updatedSheepPositions = new List<Vector3>();//�U�@�^�X���Ϯy��
    private List<Vector3> updatedLambPositions = new List<Vector3>();//�U�@�^�X���p�Ϯy��
    private List<Vector3> updatedWolfPositions = new List<Vector3>();//�U�@�^�X���T�y��
    private List<Vector3> updatedDogPositions = new List<Vector3>();//�U�@�^�X�����y��

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
        //�Ĥ@�^�X����ͦ�
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
                return true; // ��m�w�s�b��C��
            }
        }
        return false; // ��m���s�b��C��
    }//�T�{�y�ЬO�_������
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

    }//�ͦ��p��
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

    }//�ͦ���
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
    }//�ͦ��T
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
    }//�ͦ���
    void DestroyLambNextTurn()
    {
        foreach (GameObject obj in spawnedLambObjects)
        {
            Destroy(obj); // �P���C����H
        }
        spawnedLambObjects.Clear(); // �M�ŦC��
    }//�R���W�@�^�X���p��
    void DestroySheepNextTurn()
    {
        foreach (GameObject obj in spawnedSheepObjects)
        {
            Destroy(obj); // �P���C����H
        }
        spawnedSheepObjects.Clear(); // �M�ŦC��

    }//�R���W�@�^�X����
    void DestroyWolfNextTurn()
    {
        foreach (GameObject obj in spawnedWolfObjects)
        {
            Destroy(obj); // �P���C����H
        }
        spawnedWolfObjects.Clear(); // �M�ŦC��
    }//�R���W�@�^�X���T
    void DestroyDogNextTurn()
    {
        foreach (GameObject obj in spawnedDogObjects)
        {
            Destroy(obj); // �P���C����H
        }
        spawnedDogObjects.Clear(); // �M�ŦC��
    }//�R���W�@�^�X����
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
    }//�ͦ�����
    public List<Vector3> GenerateRandomSpawnPoints(int a)//�H���ͦ��y��
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
                    float secondNearestDistance = float.MaxValue; // ��l�ƲĤG��Z�����̤j��
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // ��l�ƲĤG���m���s�V�q

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(lambPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // �N�ثe�̪񪺳]���ĤG��
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // ��s�̪񪺦�m�M�Z��
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // ��s�ĤG�񪺦�m�M�Z��
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }
                    // �{�b nearestGrassPosition �N�O�C�@���ϳ̪񪺯󪺮y��
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
                    float secondNearestDistance = float.MaxValue; // ��l�ƲĤG��Z�����̤j��
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // ��l�ƲĤG���m���s�V�q

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(lambPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // �N�ثe�̪񪺳]���ĤG��
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // ��s�̪񪺦�m�M�Z��
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // ��s�ĤG�񪺦�m�M�Z��
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }
                    // �{�b nearestGrassPosition �N�O�C�@���ϳ̪񪺯󪺮y��
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
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedLambPosition = new Vector3(lambPosition.x - Fx, lambPosition.y, lambPosition.z);
                        Vector3 newupdatedLambPosition = new Vector3(lambPosition.x, lambPosition.y - Fy, lambPosition.z);
                        if (!IsPositionInList(updatedLambPosition, huntPositions) && !IsPositionInList(updatedLambPosition, dogPositions))
                        {
                            updatedLambPositions.Add(updatedLambPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedLambPositions.Add(newupdatedLambPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedLambPositions.Add(lambPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if ((Mathf.Abs(Fy) >= 1.0f))
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedLambPosition = new Vector3(lambPosition.x, lambPosition.y - Fy, lambPosition.z);
                        if (!IsPositionInList(updatedLambPosition, huntPositions) && !IsPositionInList(updatedLambPosition, dogPositions))
                        {
                            updatedLambPositions.Add(updatedLambPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedLambPositions.Add(lambPosition); // �N��s�᪺��m�K�[�� List ��
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
                                // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                                Vector3 updatedLambPosition = new Vector3(lambPosition.x - Fx, lambPosition.y, lambPosition.z);
                                Vector3 newupdatedLambPosition = new Vector3(lambPosition.x, lambPosition.y - Fy, lambPosition.z);
                                if (!IsPositionInList(updatedLambPosition, huntPositions) && !IsPositionInList(updatedLambPosition, updatedDogPositions))
                                {
                                    updatedLambPositions.Add(updatedLambPosition); // �N��s�᪺��m�K�[�� List ��
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
                                    updatedLambPositions.Add(newupdatedLambPosition); // �N��s�᪺��m�K�[�� List ��
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
                                    updatedLambPositions.Add(lambPosition); // �N��s�᪺��m�K�[�� List ��
                                }
                            }
                            else if ((Mathf.Abs(Fy) >= 1.0f))
                            {
                                // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                                Vector3 updatedLambPosition = new Vector3(lambPosition.x, lambPosition.y - Fy, lambPosition.z);
                                if (!IsPositionInList(updatedLambPosition, huntPositions) && !IsPositionInList(updatedLambPosition, updatedDogPositions))
                                {
                                    updatedLambPositions.Add(updatedLambPosition); // �N��s�᪺��m�K�[�� List ��
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
                                    updatedLambPositions.Add(lambPosition); // �N��s�᪺��m�K�[�� List ��
                                }
                            }
                        }

                    }
                    else
                    {
                        Debug.Log("stop2");
                        // �p�G���ݭn���ʡA�h�O�����m
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
                    float secondNearestDistance = float.MaxValue; // ��l�ƲĤG��Z�����̤j��
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // ��l�ƲĤG���m���s�V�q
                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(updatedLambPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // �N�ثe�̪񪺳]���ĤG��
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // ��s�̪񪺦�m�M�Z��
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // ��s�ĤG�񪺦�m�M�Z��
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }
                    // �{�b nearestGrassPosition �N�O�C�@���ϳ̪񪺯󪺮y��
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
                    float secondNearestDistance = float.MaxValue; // ��l�ƲĤG��Z�����̤j��
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // ��l�ƲĤG���m���s�V�q

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(updatedLambPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // �N�ثe�̪񪺳]���ĤG��
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // ��s�̪񪺦�m�M�Z��
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // ��s�ĤG�񪺦�m�M�Z��
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }

                    // �{�b nearestGrassPosition �N�O�C�@���ϳ̪񪺯󪺮y��
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
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 lambPosition = new Vector3(updatedLambPosition.x - Fx, updatedLambPosition.y, updatedLambPosition.z);
                        Vector3 newlambPosition = new Vector3(updatedLambPosition.x, updatedLambPosition.y - Fy, updatedLambPosition.z);
                        if (!IsPositionInList(lambPosition, huntPositions) && !IsPositionInList(lambPosition, updatedDogPositions))
                        {
                            lambPositions.Add(lambPosition); // �N��s�᪺��m�K�[�� List ��
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
                            lambPositions.Add(newlambPosition); // �N��s�᪺��m�K�[�� List ��
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
                            lambPositions.Add(updatedLambPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if ((Mathf.Abs(Fy) >= 1.0f))
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 lambPosition = new Vector3(updatedLambPosition.x, updatedLambPosition.y - Fy, updatedLambPosition.z);
                        if (!IsPositionInList(lambPosition, huntPositions) && !IsPositionInList(lambPosition, updatedDogPositions))
                        {
                            lambPositions.Add(lambPosition); // �N��s�᪺��m�K�[�� List ��
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
                            lambPositions.Add(updatedLambPosition); // �N��s�᪺��m�K�[�� List ��
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
                                // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                                Vector3 lambPosition = new Vector3(updatedLambPosition.x - Fx, updatedLambPosition.y, updatedLambPosition.z);
                                Vector3 newlambPosition = new Vector3(updatedLambPosition.x, updatedLambPosition.y - Fy, updatedLambPosition.z);
                                if (!IsPositionInList(lambPosition, huntPositions) && !IsPositionInList(lambPosition, dogPositions))
                                {
                                    lambPositions.Add(lambPosition); // �N��s�᪺��m�K�[�� List ��
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
                                    lambPositions.Add(newlambPosition); // �N��s�᪺��m�K�[�� List ��
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
                                    lambPositions.Add(updatedLambPosition); // �N��s�᪺��m�K�[�� List ��
                                }
                            }
                            else if ((Mathf.Abs(Fy) >= 1.0f))
                            {
                                // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                                Vector3 lambPosition = new Vector3(updatedLambPosition.x, updatedLambPosition.y - Fy, updatedLambPosition.z);
                                if (!IsPositionInList(lambPosition, huntPositions) && !IsPositionInList(lambPosition, dogPositions))
                                {
                                    lambPositions.Add(lambPosition); // �N��s�᪺��m�K�[�� List ��
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
                                    lambPositions.Add(updatedLambPosition); // �N��s�᪺��m�K�[�� List ��
                                }
                            }
                        }
                    }
                    else
                    {
                        // �p�G���ݭn���ʡA�h�O�����m
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
                    float secondNearestDistance = float.MaxValue; // ��l�ƲĤG��Z�����̤j��
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // ��l�ƲĤG���m���s�V�q

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(sheepPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // �N�ثe�̪񪺳]���ĤG��
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // ��s�̪񪺦�m�M�Z��
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // ��s�ĤG�񪺦�m�M�Z��
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }
                    // �{�b nearestGrassPosition �N�O�C�@���ϳ̪񪺯󪺮y��
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
                    float secondNearestDistance = float.MaxValue; // ��l�ƲĤG��Z�����̤j��
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // ��l�ƲĤG���m���s�V�q

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(sheepPosition, grassPosition);
                        if (distance < nearestDistance)
                        {
                            // �N�ثe�̪񪺳]���ĤG��
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // ��s�̪񪺦�m�M�Z��
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // ��s�ĤG�񪺦�m�M�Z��
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }

                    // �{�b nearestGrassPosition �N�O�C�@���ϳ̪񪺯󪺮y��
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
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y, sheepPosition.z);
                        Vector3 newupdatedSheepPosition = new Vector3(sheepPosition.x - fx, sheepPosition.y - fy, sheepPosition.z);
                        if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                        {
                            updatedSheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedSheepPositions.Add(newupdatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedSheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                        Vector3 updatedSheepPosition = new Vector3(sheepPosition.x, sheepPosition.y - Fy, sheepPosition.z);
                        Vector3 newupdatedSheepPosition = new Vector3(sheepPosition.x - fx, sheepPosition.y - fy, sheepPosition.z);
                        if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                        {
                            updatedSheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedSheepPositions.Add(newupdatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedSheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y - Fy, sheepPosition.z);
                        if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                        {
                            updatedSheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedSheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                        Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y - Fy, sheepPosition.z);
                        if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                        {
                            updatedSheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedSheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                            Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y, sheepPosition.z);
                            Vector3 newupdatedSheepPosition = new Vector3(sheepPosition.x - fx, sheepPosition.y - fy, sheepPosition.z);
                            if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                            {
                                updatedSheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                updatedSheepPositions.Add(newupdatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                updatedSheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
                            }
                        }
                        else if (Mathf.Abs(Fy) >= 2.0f)
                        {
                            // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                            Vector3 updatedSheepPosition = new Vector3(sheepPosition.x, sheepPosition.y - Fy, sheepPosition.z);
                            Vector3 newupdatedSheepPosition = new Vector3(sheepPosition.x - fx, sheepPosition.y - fy, sheepPosition.z);
                            if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                            {
                                updatedSheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                updatedSheepPositions.Add(newupdatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                updatedSheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
                            }
                        }
                        else if (Mathf.Abs(Fx) >= 1.0f)
                        {
                            // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                            Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y - Fy, sheepPosition.z);
                            if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                            {
                                updatedSheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                updatedSheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
                            }
                        }
                        else if (Mathf.Abs(Fy) >= 1.0f)
                        {
                            // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                            Vector3 updatedSheepPosition = new Vector3(sheepPosition.x - Fx, sheepPosition.y - Fy, sheepPosition.z);
                            if (!IsPositionInList(updatedSheepPosition, huntPositions) && !IsPositionInList(updatedSheepPosition, dogPositions))
                            {
                                updatedSheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                updatedSheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("stop2" + sheepPosition);
                        // �p�G���ݭn���ʡA�h�O�����m
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
                    float secondNearestDistance = float.MaxValue; // ��l�ƲĤG��Z�����̤j��
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // ��l�ƲĤG���m���s�V�q
                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(updatedSheepPosition, grassPosition);

                        if (distance < nearestDistance)
                        {
                            // �N�ثe�̪񪺳]���ĤG��
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // ��s�̪񪺦�m�M�Z��
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // ��s�ĤG�񪺦�m�M�Z��
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
                    float secondNearestDistance = float.MaxValue; // ��l�ƲĤG��Z�����̤j��
                    Vector3 nearestPosition = Vector3.zero;
                    Vector3 secondNearestPosition = Vector3.zero; // ��l�ƲĤG���m���s�V�q

                    foreach (Vector3 grassPosition in grassPositions)
                    {
                        float distance = Vector3.Distance(updatedSheepPosition, grassPosition);
                        if (distance < nearestDistance)
                        {
                            // �N�ثe�̪񪺳]���ĤG��
                            secondNearestDistance = nearestDistance;
                            secondNearestPosition = nearestPosition;

                            // ��s�̪񪺦�m�M�Z��
                            nearestDistance = distance;
                            nearestPosition = grassPosition;
                        }
                        else if (distance < secondNearestDistance)
                        {
                            // ��s�ĤG�񪺦�m�M�Z��
                            secondNearestDistance = distance;
                            secondNearestPosition = grassPosition;
                        }
                    }

                    // �{�b nearestGrassPosition �N�O�C�@���ϳ̪񪺯󪺮y��
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
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y, updatedSheepPosition.z);
                        Vector3 newsheepPosition = new Vector3(updatedSheepPosition.x - fx, updatedSheepPosition.y - fy, updatedSheepPosition.z);
                        if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                        {
                            sheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            sheepPositions.Add(newsheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            sheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                        Vector3 sheepPosition = new Vector3(updatedSheepPosition.x, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                        Vector3 newsheepPosition = new Vector3(updatedSheepPosition.x - fx, updatedSheepPosition.y - fy, updatedSheepPosition.z);
                        if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                        {
                            sheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            sheepPositions.Add(newsheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            sheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                        if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                        {
                            sheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            sheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                        Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                        if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                        {
                            sheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            sheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                            // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                            Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y, updatedSheepPosition.z);
                            Vector3 newsheepPosition = new Vector3(updatedSheepPosition.x - fx, updatedSheepPosition.y - fy, updatedSheepPosition.z);
                            if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                            {
                                sheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                sheepPositions.Add(newsheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                sheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
                            }
                        }
                        else if (Mathf.Abs(Fy) >= 2.0f)
                        {
                            // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                            Vector3 sheepPosition = new Vector3(updatedSheepPosition.x, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                            Vector3 newsheepPosition = new Vector3(updatedSheepPosition.x - fx, updatedSheepPosition.y - fy, updatedSheepPosition.z);
                            if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                            {
                                sheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                sheepPositions.Add(newsheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                sheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
                            }
                        }
                        else if (Mathf.Abs(Fx) >= 1.0f)
                        {
                            // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                            Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                            if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                            {
                                sheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                sheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
                            }
                        }
                        else if (Mathf.Abs(Fy) >= 1.0f)
                        {
                            // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                            Vector3 sheepPosition = new Vector3(updatedSheepPosition.x - Fx, updatedSheepPosition.y - Fy, updatedSheepPosition.z);
                            if (!IsPositionInList(sheepPosition, huntPositions) && !IsPositionInList(sheepPosition, updatedDogPositions))
                            {
                                sheepPositions.Add(sheepPosition); // �N��s�᪺��m�K�[�� List ��
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
                                sheepPositions.Add(updatedSheepPosition); // �N��s�᪺��m�K�[�� List ��
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("stop2" + updatedSheepPosition);
                        // �p�G���ݭn���ʡA�h�O�����m
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
                float secondNearestDistance = float.MaxValue; // ��l�ƲĤG��Z�����̤j��
                Vector3 nearestPosition = Vector3.zero;
                Vector3 secondNearestPosition = Vector3.zero; // ��l�ƲĤG���m���s�V�q

                foreach (Vector3 huntPosition in huntPositions)
                {
                    float distance = Vector3.Distance(wolfPosition, huntPosition);
                    if (distance < nearestDistance)
                    {
                        // �N�ثe�̪񪺳]���ĤG��
                        secondNearestDistance = nearestDistance;
                        secondNearestPosition = nearestPosition;

                        // ��s�̪񪺦�m�M�Z��
                        nearestDistance = distance;
                        nearestPosition = huntPosition;
                    }
                    else if (distance < secondNearestDistance)
                    {
                        // ��s�ĤG�񪺦�m�M�Z��
                        secondNearestDistance = distance;
                        secondNearestPosition = huntPosition;
                    }
                }

                // �{�b nearestHuntPosition �N�O�C�@���T�̪��y�����y��
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
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y, wolfPosition.z);
                    Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - fY, wolfPosition.z);
                    Vector3 nextupdatedWolfPosition = new Vector3(wolfPosition.x - fX, wolfPosition.y - fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(newupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(nextupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                    }
                }
                else if (Mathf.Abs(Fy) >= 3.0f)
                {
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x, wolfPosition.y - Fy, wolfPosition.z);
                    Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - fY, wolfPosition.z);
                    Vector3 nextupdatedWolfPosition = new Vector3(wolfPosition.x - fX, wolfPosition.y - fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(newupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(nextupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                    }
                }
                else if (Mathf.Abs(Fx) >= 2.0f)
                {
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - fy, wolfPosition.z);
                    Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - Fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(newupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                    }
                }
                else if (Mathf.Abs(Fy) >= 2.0f)
                {
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - Fy, wolfPosition.z);
                    Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(newupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                    }
                }
                else if (Mathf.Abs(Fx) >= 1.0f)
                {
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - Fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                    }
                }
                else if (Mathf.Abs(Fy) >= 1.0f)
                {
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                    Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - Fy, wolfPosition.z);
                    if (!IsPositionInList(updatedWolfPosition, runPositions))
                    {
                        updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y, wolfPosition.z);
                        Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - fY, wolfPosition.z);
                        Vector3 nextupdatedWolfPosition = new Vector3(wolfPosition.x - fX, wolfPosition.y - fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(newupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(nextupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 3.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x, wolfPosition.y - Fy, wolfPosition.z);
                        Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - fY, wolfPosition.z);
                        Vector3 nextupdatedWolfPosition = new Vector3(wolfPosition.x - fX, wolfPosition.y - fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(newupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(nextupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 2.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - fy, wolfPosition.z);
                        Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - Fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(newupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - fx, wolfPosition.y - Fy, wolfPosition.z);
                        Vector3 newupdatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(newupdatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - Fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                        Vector3 updatedWolfPosition = new Vector3(wolfPosition.x - Fx, wolfPosition.y - Fy, wolfPosition.z);
                        if (!IsPositionInList(updatedWolfPosition, runPositions))
                        {
                            updatedWolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedWolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                }
                else
                {
                    Debug.Log("stop2" + wolfPosition);
                    // �p�G���ݭn���ʡA�h�O�����m
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
                float secondNearestDistance = float.MaxValue; // ��l�ƲĤG��Z�����̤j��
                Vector3 nearestPosition = Vector3.zero;
                Vector3 secondNearestPosition = Vector3.zero; // ��l�ƲĤG���m���s�V�q

                foreach (Vector3 huntPosition in huntPositions)
                {
                    float distance = Vector3.Distance(updatedWolfPosition, huntPosition);
                    if (distance < nearestDistance)
                    {
                        // �N�ثe�̪񪺳]���ĤG��
                        secondNearestDistance = nearestDistance;
                        secondNearestPosition = nearestPosition;

                        // ��s�̪񪺦�m�M�Z��
                        nearestDistance = distance;
                        nearestPosition = huntPosition;
                    }
                    else if (distance < secondNearestDistance)
                    {
                        // ��s�ĤG�񪺦�m�M�Z��
                        secondNearestDistance = distance;
                        secondNearestPosition = huntPosition;
                    }
                }

                // �{�b nearestHuntPosition �N�O�C�@���T�̪��y�����y��
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
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y, updatedWolfPosition.z);
                    Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - fY, updatedWolfPosition.z);
                    Vector3 nextwolfPosition = new Vector3(updatedWolfPosition.x - fX, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(newwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(nextwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                    }
                }
                else if (Mathf.Abs(Fy) >= 3.0f)
                {
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                    Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - fY, updatedWolfPosition.z);
                    Vector3 nextwolfPosition = new Vector3(updatedWolfPosition.x - fX, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(newwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(nextwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                    }
                }
                else if (Mathf.Abs(Fx) >= 2.0f)
                {
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                    Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(newwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                    }
                }
                else if (Mathf.Abs(Fy) >= 2.0f)
                {
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                    Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(newwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                    }
                }
                else if (Mathf.Abs(Fx) >= 1.0f)
                {
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                    }
                }
                else if (Mathf.Abs(Fy) >= 1.0f)
                {
                    // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                    Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                    if (!IsPositionInList(wolfPosition, grassPositions))
                    {
                        wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y, updatedWolfPosition.z);
                        Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - fY, updatedWolfPosition.z);
                        Vector3 nextwolfPosition = new Vector3(updatedWolfPosition.x - fX, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(newwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(nextwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 3.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                        Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - fY, updatedWolfPosition.z);
                        Vector3 nextwolfPosition = new Vector3(updatedWolfPosition.x - fX, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(newwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(nextwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 2.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                        Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(newwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                        Vector3 newwolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(newwolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                        Vector3 wolfPosition = new Vector3(updatedWolfPosition.x - Fx, updatedWolfPosition.y - Fy, updatedWolfPosition.z);
                        if (!IsPositionInList(wolfPosition, grassPositions))
                        {
                            wolfPositions.Add(wolfPosition); // �N��s�᪺��m�K�[�� List ��
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
                            wolfPositions.Add(updatedWolfPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                }
                else
                {
                    Debug.Log("stop2" + updatedWolfPosition);
                    // �p�G���ݭn���ʡA�h�O�����m
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
                // �{�b nearestWolfPosition �N�O�C�@���T�̪��y�����y��
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
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - FY, dogPosition.z);
                        Vector3 nextupdatedDogPosition = new Vector3(dogPosition.x - FX, dogPosition.y - fy, dogPosition.z);
                        Vector3 finalupdatedDogPosition = new Vector3(dogPosition.x - fX, dogPosition.y - fY, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(newupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(nextupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(finalupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 4.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x, dogPosition.y - Fy, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - FY, dogPosition.z);
                        Vector3 nextupdatedDogPosition = new Vector3(dogPosition.x - FX, dogPosition.y - fy, dogPosition.z);
                        Vector3 finalupdatedDogPosition = new Vector3(dogPosition.x - fX, dogPosition.y - fY, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(newupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(nextupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(finalupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 3.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - fY, dogPosition.z);
                        Vector3 nextupdatedDogPosition = new Vector3(dogPosition.x - fX, dogPosition.y - fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(newupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(nextupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 3.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x, dogPosition.y - Fy, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - fY, dogPosition.z);
                        Vector3 nextupdatedDogPosition = new Vector3(dogPosition.x - fX, dogPosition.y - fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(newupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(nextupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 2.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y - fy, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - Fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(newupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - fx, dogPosition.y - Fy, dogPosition.z);
                        Vector3 newupdatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y - fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(newupdatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y - Fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                        Vector3 updatedDogPosition = new Vector3(dogPosition.x - Fx, dogPosition.y - Fy, dogPosition.z);
                        if (!IsPositionInList(updatedDogPosition, huntPositions) && !IsPositionInList(updatedDogPosition, grassPositions))
                        {
                            updatedDogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            updatedDogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
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
                        // �p�G���ݭn���ʡA�h�O�����m
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
                // �{�b nearestHuntPosition �N�O�C�@�����̪񪺯T���y��
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
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - FY, updatedDogPosition.z);
                        Vector3 nextdogPosition = new Vector3(updatedDogPosition.x - FX, updatedDogPosition.y - fy, updatedDogPosition.z);
                        Vector3 finaldogPosition = new Vector3(updatedDogPosition.x - fX, updatedDogPosition.y - fY, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(newdogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(nextdogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(finaldogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 4.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - FY, updatedDogPosition.z);
                        Vector3 nextdogPosition = new Vector3(updatedDogPosition.x - FX, updatedDogPosition.y - fy, updatedDogPosition.z);
                        Vector3 finaldogPosition = new Vector3(updatedDogPosition.x - fX, updatedDogPosition.y - fY, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(newdogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(nextdogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(finaldogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 3.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - fY, updatedDogPosition.z);
                        Vector3 nextdogPosition = new Vector3(updatedDogPosition.x - fX, updatedDogPosition.y - fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(newdogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(newdogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 3.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - fY, updatedDogPosition.z);
                        Vector3 nextdogPosition = new Vector3(updatedDogPosition.x - fX, updatedDogPosition.y - fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(newdogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(newdogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 2.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y - fy, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(newdogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 2.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - fx, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        Vector3 newdogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y - fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(newdogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fx) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s x �y��
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
                        }
                    }
                    else if (Mathf.Abs(Fy) >= 1.0f)
                    {
                        // �Ыطs�� Vector3 ��ܦϲ��ʫ᪺��m�A�u��s y �y��
                        Vector3 dogPosition = new Vector3(updatedDogPosition.x - Fx, updatedDogPosition.y - Fy, updatedDogPosition.z);
                        if (!IsPositionInList(dogPosition, huntPositions) && !IsPositionInList(dogPosition, grassPositions))
                        {
                            dogPositions.Add(dogPosition); // �N��s�᪺��m�K�[�� List ��
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
                            dogPositions.Add(updatedDogPosition); // �N��s�᪺��m�K�[�� List ��
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
                        // �p�G���ݭn���ʡA�h�O�����m
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
    public void RunGameRounds(int c)//�C�@�^�X�������޿褺�e
    {
        currentRound = c;
        DestroyDogNextTurn();
        DestroySheepNextTurn();
        DestroyLambNextTurn();
        Grass();
        Lamb();
        Sheep();
        if (eat == 3)//�Y�T�Y�F�T���ϩΤp��
        {
            DestroyWolfNextTurn();
            eat = 0;
            waitRounds = Random.Range(7, 12);
            wolfPositions.Clear();
            updatedWolfPositions.Clear();
            runPositions.Clear();
            runList.Clear();
        }
        if (currentRound == 6)//�Ĥ��^�X�~���T�X�{
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
        if (huntPositions.Count < 10)//������ϤΤp���`��< 10���ɨC�^�X�H����5�ӯ�
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
        else if (huntPositions.Count >= 10)//������ϤΤp���`��>=10���ɨC�^�X�H������3�ӯ�
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
        if (huntPositions.Count <= 10 && currentRound % 5 == 0)//������ϤΤp���`��<=10���ɨC5�^�X�|�b�a�ϤW�H���h2���p��
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
        else if (huntPositions.Count > 10 && currentRound % 5 == 0)//������ϤΤp���`��> 10���ɨC5�^�X�|�b�a�ϤW�H���h1���p��
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
        Debug.Log("��Q�Y������: " + grassEaten);
        Debug.Log("�p�ϦY���󪺦���: " + lambEaten);
        Debug.Log("�ϦY���󪺦���: " + sheepEaten);
        Debug.Log("�T�Y���p�Ϫ�����: " + wolfAteLamb);
        Debug.Log("�T�Y���Ϫ�����: " + wolfAteSheep);
        Debug.Log("�������T������: " + dogAttackedWolf);
        Debug.Log("�`�@��o����: " + totalCoins);
    }
}
