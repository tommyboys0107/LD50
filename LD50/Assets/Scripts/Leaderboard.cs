using System;
using System.Collections.Generic;
using CliffLeeCL;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] TMP_Text playerRecordText;
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] Button submitBtn;
    [SerializeField] Button refreshBtn;
    [SerializeField] List<TMP_Text> topRecordTextList = new List<TMP_Text>();
    [SerializeField] int leaderBoardID;
    [SerializeField] int topRecordCount = 10;

    bool isWaitingSubmitResponse = false;
    
    void Start()
    {
        StartGuestSession();
        submitBtn.onClick.AddListener(OnSubmitClicked);
        refreshBtn.onClick.AddListener(OnRefreshClicked);
        playerNameInput.onSubmit.AddListener(SubmitScore);
    }

    void OnEnable()
    {
        UpdateLeaderboard();
        playerRecordText.text = TimeSpan.FromSeconds(GameManager.Instance.roundTimer.CurrentTime).ToString("mm:ss.ff");
    }

    void OnSubmitClicked()
    {
        if (isWaitingSubmitResponse)
            return;

        SubmitScore(playerNameInput.text);
    }

    void OnRefreshClicked()
    {
        UpdateLeaderboard();
    }

    void StartGuestSession()
    {
        LootLockerSDKManager.StartGuestSession((response) => { Debug.Log("Login success!"); });
    }

    public void UpdateLeaderboard()
    {
        LootLockerSDKManager.GetScoreList(leaderBoardID, topRecordCount, (response) =>
        {
            if (response.success)
            {
                var topRecord = response.items;

                for (var i = 0; i < topRecordTextList.Count; i++)
                    if (i < topRecord.Length)
                        topRecordTextList[i].text =
                            $"{i}. {@TimeSpan.FromTicks(topRecord[i].score):mm\\:ss\\.ff} --- {topRecord[i].member_id}";
                    else
                        topRecordTextList[i].text = $"{i}. ??:??.??";
            }
            else
            {
                for (var i = 0; i < topRecordTextList.Count; i++)
                    if (i == 0)
                        topRecordTextList[i].text = "Network error! Please try ro refresh.";
                    else
                        topRecordTextList[i].text = $"{i}. ??:??.?? - No Record.";
            }
        });
    }


    void SubmitScore(string inputStr)
    {
        var score = (int) TimeSpan.FromSeconds(GameManager.Instance.roundTimer.CurrentTime).Ticks;

        Debug.Log($"Save {GameManager.Instance.roundTimer.CurrentTime} to leaderboard.");
        isWaitingSubmitResponse = true;
        LootLockerSDKManager.SubmitScore(inputStr, score, leaderBoardID, (response) =>
        {
            if (response.success)
            {
                playerNameInput.gameObject.SetActive(false);
                submitBtn.gameObject.SetActive(false);
                UpdateLeaderboard();
            }
            isWaitingSubmitResponse = false;
        });
    }
}