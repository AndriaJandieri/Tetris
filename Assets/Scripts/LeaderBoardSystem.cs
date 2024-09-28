using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class LeaderBoardSystem : MonoBehaviour
{

    //public ScoreEntryScript[] scoreEntries;
    public GameObject[] scoreEntries;

    [HideInInspector]
    public DataEntry[] localDataEntries = new DataEntry[10];
    [HideInInspector]
    public DataEntry[] onlineDataEntries = new DataEntry[10];
    [HideInInspector]
    public DataEntry[] processedDataEntries = new DataEntry[10];

    public bool isOnline;

    //string httpRequestUrl;
    string savingPath;

    [System.Serializable]
    public struct DataEntry
    {
        [SerializeField] public string name;
        [SerializeField] public int score;
    }

    [System.Serializable]
    public class DataWrapper
    {
        public DataEntry[] wrapperLocalEntries;
    }

    #region Online

    private const string gDriveURL = "(((HERE GOES HIGHSCORE GDRIVE'S LINK)))";

    public string gDriveJSON;

    IEnumerator SynchHighScores()
    {
        UnityWebRequest www = UnityWebRequest.Get(gDriveURL);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            isOnline = false;
            Debug.Log(www.error);
            //what should happened if connection have problem
        }
        else
        {
            isOnline = true;
            gDriveJSON = www.downloadHandler.text;

            LoadOnlineDataFromJSON();

            ProcessArrays(localDataEntries, onlineDataEntries);

            DisplayLocalScores();
            StartCoroutine("UpdateOnlineData");
        }
    }
    void ProcessArrays(DataEntry[] localData, DataEntry[] onlineData)
    {
        DataEntry[] mergedArray = new DataEntry[localData.Length + onlineData.Length];

        for (int i = 0; i < mergedArray.Length; i++)
        {
            if (i < localData.Length)
            {
                mergedArray[i].name = localData[i].name;
                mergedArray[i].score = localData[i].score;
            }
            if (i >= localData.Length)
            {
                mergedArray[i].name = onlineData[i - localData.Length].name;
                mergedArray[i].score = onlineData[i - localData.Length].score;
            }
        }

        var sortedArray = mergedArray.OrderByDescending(p => p.score).GroupBy(e => e.name).Select(group => group.First()).ToArray();
        mergedArray = sortedArray;

        for (int i = 0; i < processedDataEntries.Length; i++)
        {
            processedDataEntries[i].name = mergedArray[i].name;
            processedDataEntries[i].score = mergedArray[i].score;
        }

        localDataEntries = processedDataEntries;
        SaveLocalDataToJSON();
    }
    IEnumerator UpdateOnlineData()
    {
        UnityWebRequest www = UnityWebRequest.Get(HttpRequestUrl());

        // Send the request and wait for a response
        yield return www.SendWebRequest();

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            /* */

            // Print the response text
            Debug.Log("Received: " + www.downloadHandler.text);
        }
    }

    string HttpRequestUrl()
    {
        return "https://script.google.com/macros/ (((HERE GOES GOOGLE SCRIPT MACRO LINK))))) " +
            "wrapperLocalEntries" +
            "%22%3A%20%5B%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"name%22%3A%20%22{localDataEntries[0].name}" +
            "%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"score%22%3A%20{localDataEntries[0].score}" +
            "%0A%20%20%20%20%20%20%20%20%7D%2C%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"name%22%3A%20%22{localDataEntries[1].name}" +
            "%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"score%22%3A%20{localDataEntries[1].score}" +
            "%0A%20%20%20%20%20%20%20%20%7D%2C%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"name%22%3A%20%22{localDataEntries[2].name}" +
            "%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"score%22%3A%20{localDataEntries[2].score}" +
            "%0A%20%20%20%20%20%20%20%20%7D%2C%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"name%22%3A%20%22{localDataEntries[3].name}" +
            "%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"score%22%3A%20{localDataEntries[3].score}" +
            "%0A%20%20%20%20%20%20%20%20%7D%2C%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"name%22%3A%20%22{localDataEntries[4].name}" +
            "%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"score%22%3A%20{localDataEntries[5].score}" +
            "%0A%20%20%20%20%20%20%20%20%7D%2C%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"name%22%3A%20%22{localDataEntries[5].name}" +
            "%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"score%22%3A%20{localDataEntries[5].score}" +
            "%0A%20%20%20%20%20%20%20%20%7D%2C%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"name%22%3A%20%22{localDataEntries[6].name}" +
            "%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"score%22%3A%20{localDataEntries[6].score}" +
            "%0A%20%20%20%20%20%20%20%20%7D%2C%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"name%22%3A%20%22{localDataEntries[7].name}" +
            "%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"score%22%3A%20{localDataEntries[7].score}" +
            "%0A%20%20%20%20%20%20%20%20%7D%2C%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"name%22%3A%20%22{localDataEntries[8].name}" +
            "%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"score%22%3A%20{localDataEntries[8].score}" +
            "%0A%20%20%20%20%20%20%20%20%7D%2C%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"name%22%3A%20%22{localDataEntries[9].name}" +
            "%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%22" +
            $"score%22%3A%20{localDataEntries[9].score}" +
            "%0A%20%20%20%20%20%20%20%20%7D%0A%20%20%20%20%5D%0A%7D";
    }


    #endregion


    private void Awake()
    {
        savingPath = Application.persistentDataPath + "/HighScores.json";
    }
    void Start()
    {
        LoadLocalDataFromJSON();
        StartCoroutine("SynchHighScores");
        DisplayLocalScores();
    }
    void Update()
    {
    }

    public void SaveLocalDataToJSON()
    {
        DataWrapper wrapper = new DataWrapper();
        wrapper.wrapperLocalEntries = localDataEntries;

        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(savingPath, json);
        Debug.Log("Player data saved to: " + savingPath);
    }

    public void LoadOnlineDataFromJSON()
    {
        if (gDriveJSON != null)
        {
            DataWrapper wrapper = new DataWrapper();
            JsonUtility.FromJsonOverwrite(gDriveJSON, wrapper);
            DataEntry[] items = wrapper.wrapperLocalEntries;

            //Pass element values from gDrive on "onlineDataEntries"
            for (int i = 0; i < onlineDataEntries.Length; i++)
            {
                onlineDataEntries[i].name = items[i].name;
                onlineDataEntries[i].score = items[i].score;
            }
        }
        else
        {
            Debug.Log("gDJSON is not provided");
        }
    }
    public void LoadLocalDataFromJSON()
    {
        if (File.Exists(savingPath))
        {
            Debug.Log("FILE EXISTS");
            string loadedJSON = File.ReadAllText(savingPath);

            DataWrapper wrapper = new DataWrapper();

            JsonUtility.FromJsonOverwrite(loadedJSON, wrapper);

            DataEntry[] items = wrapper.wrapperLocalEntries;

            for (int i = 0; i < localDataEntries.Length; i++)
            {
                localDataEntries[i].name = items[i].name;
                localDataEntries[i].score = items[i].score;
            }
        }
        else
        {
            InitScore();
            SaveLocalDataToJSON();
            DisplayLocalScores();
            Debug.Log("Default scores are loaded");
        }
    }

    public void DisplayLocalScores()
    {
        for (int i = 0; i < scoreEntries.Length; i++)
        {
            scoreEntries[i].GetComponent<ScoreEntryScript>().playerName.text = localDataEntries[i].name;
            scoreEntries[i].GetComponent<ScoreEntryScript>().score.text = localDataEntries[i].score.ToString();
        }
    }

    public void InitScore()
    {
        localDataEntries[0] = new DataEntry { name = "John", score = 10 };
        localDataEntries[1] = new DataEntry { name = "Merry", score = 50 };
        localDataEntries[2] = new DataEntry { name = "George", score = 20 };
        localDataEntries[3] = new DataEntry { name = "Suzy", score = 70 };
        localDataEntries[4] = new DataEntry { name = "Jenny", score = 30 };
        localDataEntries[5] = new DataEntry { name = "Chris", score = 40 };
        localDataEntries[6] = new DataEntry { name = "Conor", score = 80 };
        localDataEntries[7] = new DataEntry { name = "Valentina", score = 100 };
        localDataEntries[8] = new DataEntry { name = "Luke", score = 85 };
        localDataEntries[9] = new DataEntry { name = "Jimmy", score = 65 };

        var sortedArray = localDataEntries.OrderByDescending(p => p.score).ToArray();
        localDataEntries = sortedArray;
    }


    public void AddEntry(string name, int score)
    {
        if (isOnline)
        {
            StartCoroutine("SynchHighScores");
        }

        if (score >= localDataEntries[9].score)
        {
            localDataEntries[9] = new DataEntry { name = name, score = score };

            var sortedArray = localDataEntries.OrderByDescending(p => p.score).ToArray();
            localDataEntries = sortedArray;

            for (int i = 0; i < localDataEntries.Length; i++)
            {
                if (name == localDataEntries[i].name)
                {
                    var current = scoreEntries[i].GetComponent<ScoreEntryScript>().playerName;
                    current.color = Color.green;
                    current.fontStyle = FontStyles.Bold;
                }
            }
        }
    }

    public void ResetHighScoresTextStyle()
    {
        foreach (var item in scoreEntries)
        {
            item.GetComponent<ScoreEntryScript>().playerName.color = Color.white;
            item.GetComponent<ScoreEntryScript>().playerName.fontStyle = FontStyles.Normal;
        }
    }
}
