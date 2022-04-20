using System;
using System.Collections.Generic;
using CliffLeeCL;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using TMPro;
using UnityEngine.SceneManagement;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] TMP_Text playerRecordText;
    [SerializeField] TMP_Text playerBestRecordText;
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] Button submitBtn;
    [SerializeField] Button restartBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] Button refreshBtn;
    [SerializeField] List<TMP_Text> topRecordTextList = new List<TMP_Text>();
    [SerializeField] int leaderBoardID;
    [SerializeField] int topRecordCount = 10;

    int PlayerCurrentRecordTicks => (int)TimeSpan.FromSeconds(GameManager.Instance.roundTimer.CurrentTime).Ticks; 
    int PlayerBestRecordTicks => PlayerPrefs.GetInt("PlayerBestRecord", 0);
    bool isWaitingSubmitResponse = false;
    
    void Start()
    {
        StartGuestSession();
        submitBtn.onClick.AddListener(OnSubmitClicked);
        restartBtn.onClick.AddListener(OnRestartClicked);
        exitBtn.onClick.AddListener(OnExitClicked);
        refreshBtn.onClick.AddListener(OnRefreshClicked);
        playerNameInput.onSubmit.AddListener(SubmitScore);
    }

    void OnEnable()
    {
        UpdateLeaderboard();
        if (PlayerCurrentRecordTicks > PlayerBestRecordTicks)
            PlayerPrefs.SetInt("PlayerBestRecord", PlayerCurrentRecordTicks);
        playerRecordText.text = TimeSpan.FromSeconds(GameManager.Instance.roundTimer.CurrentTime).ToString("mm:ss.ff");
        playerBestRecordText.text = $"Best: {@TimeSpan.FromTicks(PlayerBestRecordTicks):mm\\:ss\\.ff}";
    }

    void OnSubmitClicked()
    {
        if (isWaitingSubmitResponse)
            return;

        SubmitScore(playerNameInput.text);
        AudioManager.Instance.PlaySound(AudioManager.AudioName.ButtonClicked);
    }
    
    void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        AudioManager.Instance.PlaySound(AudioManager.AudioName.ButtonClicked);
    }
    
    void OnExitClicked()
    {
        Application.Quit();
        Debug.Log("Exit game!");
        AudioManager.Instance.PlaySound(AudioManager.AudioName.ButtonClicked);
    }

    void OnRefreshClicked()
    {
        UpdateLeaderboard();
        AudioManager.Instance.PlaySound(AudioManager.AudioName.ButtonClicked);
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
                        topRecordTextList[i].text = "Network error! Please try again.";
                    else
                        topRecordTextList[i].text = $"{i}. ??:??.?? - No Record.";
            }
        });
    }


    void SubmitScore(string inputStr)
    {
        isWaitingSubmitResponse = true;
        LootLockerSDKManager.SubmitScore(inputStr, PlayerBestRecordTicks, leaderBoardID, (response) =>
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