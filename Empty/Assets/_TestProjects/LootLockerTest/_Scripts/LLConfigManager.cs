using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker;
using LootLocker.Requests;
using UnityEditor;

public class LLConfigManager : MonoBehaviour
{
    private const string DEVELOP_MOD_API_KEY = "dev_cc3723a2ab584e9bb391faaf3ef1445d";
    
    [SerializeField] private string leaderboardId = "highscoreLeaderboard";
    [SerializeField] private int minTopScoreToShow = 10;
    [SerializeField] private int maxTopScoreToShow = 50;
    [SerializeField] private int minRankToShowMax = 45;
    [SerializeField] private int nearRankToShow = 30;

    
    
    private List<LeaderboardRecord> leaderboardRecords;
    private bool isBestScroreUpdated;
    private int memberRank;
    
    void Start()
    {
        var config = LootLockerConfig.Get();
        config.apiKey = DEVELOP_MOD_API_KEY;
        config.developmentMode = true;
        
        Debug.Log($"LootLocker cofig game version {config.game_version}, " +
                  $"bundle game version {PlayerSettings.bundleVersion} + " +
                  $"config.developmentMode {config.developmentMode}");
    }
    
    
    
    

    private IEnumerator SubmitScoreCoroutine()
    {
        if(!PlayerManager.Instance.IsSessionStarted()) yield break;
        
        bool done = false;
        string playerID = PlayerManager.Instance.GetPlayerID();
        int score = PlayerPrefs.GetInt("bestScore");
        
        LootLockerSDKManager.SubmitScore(playerID, score,leaderboardId, (response) =>
        {
            if (response.success) {
                //Debug.Log("Successful submit score");
                done = true;
            } else {
                Debug.Log("failed to submit score: " + response.errorData.message);
                done = true;
            }
        });
        
        yield return new WaitWhile(() => done == false);
    }

    private IEnumerator FetchLeaderboardCoroutine()
    {
        if(!PlayerManager.Instance.IsSessionStarted()) yield break;
        leaderboardRecords = new List<LeaderboardRecord>();
        
        yield return GetMemberRankCoroutine();
        int countToShow;
        
        if (memberRank <= minRankToShowMax)
        {
            memberRank = 1;
            countToShow = maxTopScoreToShow;
        }
        else
        {
            yield return GetScoreListCourutine(minTopScoreToShow, 1);
            LeaderboardRecord record = new LeaderboardRecord
            {
                name = "...",
                place = 0,
                score = 0
            };
            leaderboardRecords.Add(record);

            memberRank -= nearRankToShow / 2;
            countToShow = nearRankToShow;
        }
        yield return GetScoreListCourutine(countToShow, memberRank);
        
    }

    
    private IEnumerator GetScoreListCourutine(int count, int startRank)
    {
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardId,count,startRank-1, (response) =>
        {
            if (response.success)
            {
                LootLockerLeaderboardMember[] members = response.items;
                foreach (var member in members)
                {
                    LeaderboardRecord record = new LeaderboardRecord();
                    if (member.player.name != "")
                    {
                        record.name = member.player.name;
                    }
                    else
                    {
                        record.name = member.player.public_uid;
                    }

                    record.place = member.rank;
                    record.score = member.score;

                    leaderboardRecords.Add(record);
                }
                
                //Debug.Log($"Leaderboard is Fetched count: {count} startRank: {startRank}");
                done = true;
            }
            else
            {
                Debug.Log("Failed to fetch top Leaderboard" + response.errorData.message);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
    
    private IEnumerator GetMemberRankCoroutine()
    {
        bool done = false;
        
        LootLockerSDKManager.GetMemberRank(leaderboardId,PlayerManager.Instance.GetPlayerID(), response =>
        {
            if (response.success)
            {
                memberRank = response.rank;
                //Debug.Log("Got member rank - " + memberRank);
                done = true;
            }
            else
            {
                Debug.Log("Failed to get member rank");
                done = true;
            }

        });
        yield return new WaitWhile(() => done == false);
    }

    
    

    public void UpdateScore()
    {
        if (isBestScroreUpdated)
        {
            isBestScroreUpdated = false;
            SubmitScore();
        }
    }
    
    public Coroutine SubmitScore()
    {
        return StartCoroutine(SubmitScoreCoroutine());
    }

    public Coroutine FetchLeaderboard()
    {
        return StartCoroutine(FetchLeaderboardCoroutine());
    }

    public List<LeaderboardRecord> GetLeaderboardList()
    {
        return leaderboardRecords;
    }

    

    private void SetIsBestScoreUpdated(int score)
    {
        isBestScroreUpdated = true;
    }
    
    
    public class LeaderboardRecord
    {
        public string name;
        public int place;
        public int score;

    }
    
}
