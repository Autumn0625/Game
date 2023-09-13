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
        // 初始化場景，包括放置羊、小羊、狼、犬和草
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
            return; // 如果正在等待下一回合按鈕，不執行下一回合的邏輯
        else
        {
            currentRound++;
            Debug.Log(currentRound+"GM");
            if (currentRound > totalRounds)
            {
                // 遊戲結束，顯示統計結果等
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
            return; // 如果正在等待下一回合按鈕，不執行下一回合的邏輯
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
            // 當按下下一回合按鈕時，設置等待標誌為false，以執行下一回合
            ToFinalRound = false;
        }
    }
    public void OnNextRoundButtonClick()
    {
        if (currentRound <= totalRounds)
        {
            // 當按下下一回合按鈕時，設置等待標誌為false，以執行下一回合
            waitForNextRound = false;
        }
    }
    
}
