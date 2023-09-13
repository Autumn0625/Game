using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.U2D.IK;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int totalRounds = 100;
    private int currentRound = 1;
    private bool waitForNextRound = true;
    private bool ToFinalRound = true;

    

    public void Start()
    {
        // ��l�Ƴ����A�]�A��m�ϡB�p�ϡB�T�B���M��
        Button nextRoundButton = GameObject.Find("NextRoundButton").GetComponent<Button>();
        nextRoundButton.onClick.AddListener(NextRound);
        //Button SkipButton = GameObject.Find("SkipButton").GetComponent<Button>();
        //SkipButton.onClick.AddListener(FinalRound);
        NextRound();
        //FinalRound();
    }
    public void Update()
    {

    }
    public void NextRound()
    {
        if (waitForNextRound)
            return; // �p�G���b���ݤU�@�^�X���s�A������U�@�^�X���޿�
        else
        {
            currentRound++;
            Debug.Log(currentRound+"GM");
            if (currentRound > totalRounds)
            {
                // �C�������A��ܲέp���G��
                RandomSpawnManager randomSpawnManager = FindObjectOfType<RandomSpawnManager>();
                if (randomSpawnManager != null)
                {
                    randomSpawnManager.DisplayResults();
                }
            }
            else
            {
                RandomSpawnManager randomSpawnManager = FindObjectOfType<RandomSpawnManager>();
                if (randomSpawnManager != null)
                {
                    randomSpawnManager.RunGameRounds(currentRound);
                }
                waitForNextRound = true;
            }
        }
    }
    public void FinalRound()
    {
        if (ToFinalRound)
            return; // �p�G���b���ݤU�@�^�X���s�A������U�@�^�X���޿�
        else
        {
            for (int i = 0;i < totalRounds - 1; i++)
            {
                waitForNextRound = false;
                waitForNextRound = true;
            }
        }
    }
    public void OnSkipButtonClick()
    {
        if (currentRound <= totalRounds)
        {
            // ����U�U�@�^�X���s�ɡA�]�m���ݼлx��false�A�H����U�@�^�X
            ToFinalRound = false;
        }
    }
    public void OnNextRoundButtonClick()
    {
        if (currentRound <= totalRounds)
        {
            // ����U�U�@�^�X���s�ɡA�]�m���ݼлx��false�A�H����U�@�^�X
            waitForNextRound = false;
        }
    }
    
}
