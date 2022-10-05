using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Color = UnityEngine.Color;
using Button = UnityEngine.UI.Button;
using Application = UnityEngine.Application;
using UnityEngine.Networking;

public class voting : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> characters = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> authors = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> votes = new List<TextMeshProUGUI>();
    [SerializeField] List<RawImage> upvotes = new List<RawImage>();
    [SerializeField] List<RawImage> downvotes = new List<RawImage>();
    [SerializeField] List<int> ids = new List<int>();
    [SerializeField] List<RawImage> images = new List<RawImage>();
    [SerializeField] TMP_InputField searchQuery;
    [SerializeField] Button previousButton;
    [SerializeField] Button nextButton;
    [SerializeField] TextMeshProUGUI pageText;

    [SerializeField] TextMeshProUGUI sortText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI searchText;

    [SerializeField] List<TextMeshProUGUI> copyTexts = new List<TextMeshProUGUI>();
    [SerializeField] achievements achievementHandler;
    [SerializeField] copy clip;
    [SerializeField] GameObject fullscreenedObject;
    [SerializeField] TextMeshProUGUI fullscreenedChar;
    [SerializeField] TextMeshProUGUI fullscreenedArtist;
    [SerializeField] RawImage fullscreenedImage;
    [SerializeField] GameObject musicPlaying;

    string sortMode = "top";
    string time = "week";
    string searchMode = "character";
    int page = 0;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getFromDatabase(true));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void clearAll()
    {
        ids.Clear();
        for (int i = 0; i < 6; i++)
        {
            characters[i].text = "";
            authors[i].text = "";
            votes[i].text = "0";
            images[i].texture = null;
            upvotes[i].color = new Color(.81f, .81f, .81f);
            downvotes[i].color = new Color(.81f, .81f, .81f);
        }
    }


    IEnumerator getFromDatabase(bool isStart)
    {
        clearAll();



        WWWForm form = new WWWForm();
        form.AddField("sortMode", sortMode);
        form.AddField("time", time);
        form.AddField("search", searchQuery.text);
        form.AddField("searchMode", searchMode);
        form.AddField("page", page);

        if(time == "all" && sortMode == "top" && searchQuery.text == "")
        {
            achievementHandler.unlockAchievement(7);
        }

        using (UnityWebRequest www = UnityWebRequest.Post("https://crowseeds.com/FONT/receive.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
                Debug.Log(www.result);
            }
            else
            {
                string[] results = www.downloadHandler.text.Split('\t');

                if(results.Length < 25 && isStart && time == "week")
                {
                    time = "month";
                    timeText.text = "Time: Month";
                    StartCoroutine(getFromDatabase(false));
                    yield break;
                }

                if (results.Length < 25 && isStart && time == "month")
                {
                    time = "all";
                    timeText.text = "Time: All";
                    StartCoroutine(getFromDatabase(false));
                    yield break;
                }

                for (int i = 0; i < results.Length - 1; i += 4)
                {
                    ids.Add(int.Parse(results[i]));
                    StartCoroutine(GetTexture(int.Parse(results[i]), i / 4));
                    characters[i / 4].text = results[i + 1];
                    authors[i / 4].text = "Drawn by: " + results[i + 2];
                    votes[i / 4].text = results[i + 3];
                    if (PlayerPrefs.GetString(results[i], "none") == "upvoted")
                    {
                        upvotes[i / 4].color = new Color(.25f, .25f, .25f);
                    }
                    else if (PlayerPrefs.GetString(results[i], "none") == "downvoted")
                    {
                        downvotes[i / 4].color = new Color(.25f, .25f, .25f);
                    }

                }
                pageText.text = "Page " + page.ToString();
                nextButton.interactable = true;
                previousButton.interactable = true;
                if (results.Length != 25)
                {
                    nextButton.interactable = false;
                }
                if (page == 0)
                {
                    previousButton.interactable = false;
                }
            }
        }
    }

    IEnumerator GetTexture(int id, int index)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://crowseeds.com/font/images/" + id.ToString() + ".png");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            images[index].texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }

        if (characters[index].text == "")
        {
            images[index].texture = null;
        }
    }

    public void previousPage()
    {
        if (page > 0)
        {
            page--;
            StartCoroutine(getFromDatabase(false));
        }
    }

    public void nextPage()
    {
        if (nextButton.interactable == true)
        {
            page++;
            StartCoroutine(getFromDatabase(false));
        }
    }

    public void changeSort()
    {
        switch (sortMode)
        {
            case "top":
                sortMode = "new";
                sortText.text = "Sort By: New";
                break;
            case "new":
                sortMode = "random";
                sortText.text = "Sort By: Random";
                break;
            case "random":
                sortMode = "bottom";
                sortText.text = "Sort By: Bottom";
                break;
            case "bottom":
                sortMode = "top";
                sortText.text = "Sort By: Top";
                break;
        }
        page = 0;
        StartCoroutine(getFromDatabase(false));
    }

    public void changeTime()
    {
        switch (time)
        {
            case "all":
                time = "hour";
                timeText.text = "Time: Hour";
                break;
            case "hour":
                time = "day";
                timeText.text = "Time: Day";
                break;
            case "day":
                time = "week";
                timeText.text = "Time: Week";
                break;
            case "week":
                time = "month";
                timeText.text = "Time: Month";
                break;
            case "month":
                time = "year";
                timeText.text = "Time: Year";
                break;
            case "year":
                time = "all";
                timeText.text = "Time: All";
                break;
        }
        page = 0;
        StartCoroutine(getFromDatabase(false));
    }

    public void changeSearch()
    {
        switch (searchMode)
        {
            case "character":
                searchMode = "author";
                searchText.text = "Search by Author:";
                break;
            case "author":
                searchMode = "character";
                searchText.text = "Search by Character:";
                break;
        }
        page = 0;
        StartCoroutine(getFromDatabase(false));
    }

    public void editSearch()
    {
        page = 0;
        Debug.Log(searchQuery.text);
        StartCoroutine(getFromDatabase(false));
    }

    public void upvote(int i)
    {
        upvotes[i].color = new Color(.81f, .81f, .81f);
        downvotes[i].color = new Color(.81f, .81f, .81f);
        achievementHandler.unlockAchievement(2);


        if (PlayerPrefs.GetString(ids[i].ToString(), "none") != "upvoted")
        {
            if (PlayerPrefs.GetString(ids[i].ToString(), "none") == "downvoted")
            {
                StartCoroutine(vote("upvote", ids[i], i));
            }
            PlayerPrefs.SetString(ids[i].ToString(), "upvoted");
            upvotes[i].color = new Color(.25f, .25f, .25f);
            StartCoroutine(vote("upvote", ids[i], i));
        }
        else
        {
            PlayerPrefs.SetString(ids[i].ToString(), "none");
            upvotes[i].color = new Color(.81f, .81f, .81f);
            StartCoroutine(vote("downvote", ids[i], i));
        }
    }

    public void downvote(int i)
    {
        upvotes[i].color = new Color(.81f, .81f, .81f);
        downvotes[i].color = new Color(.81f, .81f, .81f);
        achievementHandler.unlockAchievement(1);

        if (PlayerPrefs.GetString(ids[i].ToString(), "none") != "downvoted")
        {
            if (PlayerPrefs.GetString(ids[i].ToString(), "none") == "upvoted")
            {
                StartCoroutine(vote("downvote", ids[i], i));
            }
            PlayerPrefs.SetString(ids[i].ToString(), "downvoted");
            downvotes[i].color = new Color(.25f, .25f, .25f);
            StartCoroutine(vote("downvote", ids[i], i));
        }
        else
        {
            PlayerPrefs.SetString(ids[i].ToString(), "none");
            downvotes[i].color = new Color(.81f, .81f, .81f);
            StartCoroutine(vote("upvote", ids[i], i));
        }
    }

    IEnumerator vote(string mode, int id, int voteIndex)
    {
        WWWForm form = new WWWForm();
        form.AddField("type", mode);
        form.AddField("id", id);

        using (UnityWebRequest www = UnityWebRequest.Post("https://crowseeds.com/FONT/vote.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                votes[voteIndex].text = www.downloadHandler.text;
            }
        }
    }

    public void share(int i)
    {
        if (i < ids.Count)
        {
            if(Application.platform == RuntimePlatform.WebGLPlayer)
            {
                clip.copyToClip(characters[i].text + "\n" + authors[i].text + "\nTimes Newer Roman\n\n" + "https://crowseeds.com/font/images/" + ids[i].ToString() + ".png");
                achievementHandler.unlockAchievement(4);
            }
            else
            {
                GUIUtility.systemCopyBuffer = characters[i].text + "\n" + authors[i].text + "\nTimes Newer Roman\n\n" + "https://crowseeds.com/font/images/" + ids[i].ToString() + ".png";
            }
            
        }


        StartCoroutine(fadeCopy(i));
    }

    IEnumerator fadeCopy(int i)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/textFader")).GetComponent<colorFader3>().set(copyTexts[i], new Color(.315f, .315f, .315f, .41f), .25f);
        yield return new WaitForSeconds(.5f);
        Instantiate(Resources.Load<GameObject>("Prefabs/textFader")).GetComponent<colorFader3>().set(copyTexts[i], new Color(.315f, .315f, .315f, 0), 1f);
    }

    public void fullscreenImage(int i)
    {
        if(i < ids.Count)
        {
            fullscreenedObject.SetActive(true);
            fullscreenedImage.texture = images[i].texture;
            fullscreenedChar.text = characters[i].text;
            fullscreenedArtist.text = authors[i].text;
            musicPlaying.SetActive(false);
        }
    }

    public void leaveFullscreen()
    {
        fullscreenedObject.SetActive(false);
        musicPlaying.SetActive(true);
    }
}
