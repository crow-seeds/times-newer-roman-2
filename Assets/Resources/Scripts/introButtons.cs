using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class introButtons : MonoBehaviour
{
    public RectTransform m;
    bool moving = false;
    EasingFunction.Function function;
    public TextMeshProUGUI muteText;
    public TextMeshProUGUI fullscreenText;
    public TextMeshProUGUI darkModeText;
    public AudioSource music;
    public RawImage blackScreen;

    float newX;
    float newY;
    float oldX;
    float oldY;

    float timer = 0;

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject mainGame;
    [SerializeField] main game;
    [SerializeField] RectTransform musicPlaying;
    [SerializeField] GameObject darkMode;

    [SerializeField] achievements achievementHandler;

    // Start is called before the first frame update
    void Start()
    {
        EasingFunction.Ease movement = EasingFunction.Ease.EaseInOutBounce;
        function = EasingFunction.GetEasingFunction(movement);

        if(PlayerPrefs.GetInt("mute", 0) == 1)
        {
            music.mute = true;
            muteText.text = "[Music: Off]";
        }

        if (PlayerPrefs.GetInt("darkMode", 0) == 1)
        {
            darkMode.SetActive(true);
            darkModeText.text = "[Dark Mode: On]";
            blackScreen.color = new Color(1, 1, 1, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            timer += Time.deltaTime;
            m.localPosition = new Vector2(function(oldX, newX, timer * 4), function(oldY, newY, timer * 4));

            if (timer >= .25)
            {
                moving = false;
                timer = 0;
                m.localPosition = new Vector2(newX, newY);
            }
        }

        if(!mainMenu.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            goBackToMenu();
        }

        if (Screen.fullScreen)
        {
            fullscreenText.text = "[Fullscreen: On]";
        }
        else
        {
            fullscreenText.text = "[Fullscreen: Off]";
        }
    }

    public void goToInfo()
    {
        moving = true;
        oldX = m.localPosition.x;
        oldY = m.localPosition.y;
        newX = 0;
        newY = 900;
    }

    public void goToSettings()
    {
        moving = true;
        oldX = m.localPosition.x;
        oldY = m.localPosition.y;
        newX = 0;
        newY = -900;
    }

    public void goToMain()
    {
        moving = true;
        oldX = m.localPosition.x;
        oldY = m.localPosition.y;
        newX = 0;
        newY = 0;
        Debug.Log("moving");
    }

    public void setPlayerPrefs()
    {
        if(Screen.fullScreen)
        {
            fullscreenText.text = "[Fullscreen: On]";
        }

        if(PlayerPrefs.GetInt("mute", 0) == 1)
        {
            music.mute = true;
            muteText.text = "[Music: Off]";
        }
    }

    public void toggleMute()
    {
        if(PlayerPrefs.GetInt("mute", 0) == 0)
        {
            music.mute = true;
            muteText.text = "[Music: Off]";
            PlayerPrefs.SetInt("mute", 1);
        }
        else
        {
            music.mute = false;
            muteText.text = "[Music: On]";
            PlayerPrefs.SetInt("mute", 0);
        }
    }

    public void toggleFullscreen()
    {
        if (!Application.isMobilePlatform)
        {
            activateFullscreen();
        }
        
        if(Screen.fullScreen)
        {
            fullscreenText.text = "[Fullscreen: On]";
        }
        else
        {
            fullscreenText.text = "[Fullscreen: Off]";
        }
    }

    public void toggleDarkmode()
    {
        if (PlayerPrefs.GetInt("darkMode", 0) == 0)
        {
            achievementHandler.unlockAchievement(6);
            darkMode.SetActive(true);
            darkModeText.text = "[Dark Mode: On]";
            PlayerPrefs.SetInt("darkMode", 1);
            blackScreen.color = new Color(1, 1, 1, 0);
        }
        else
        {
            darkMode.SetActive(false);
            darkModeText.text = "[Dark Mode: Off]";
            PlayerPrefs.SetInt("darkMode", 0);
            blackScreen.color = new Color(0, 0, 0, 0);
        }
    }

    public void goBackToMenu()
    {
        StartCoroutine(fadeTransition2());
    }

    public void activateFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void goToDraw()
    {
        StartCoroutine(fadeTransition());
        game.drawOrVote = false;
        game.changeMode(false);
    }

    IEnumerator fadeTransition()
    {
        if(PlayerPrefs.GetInt("darkMode", 0) == 0)
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/imageFader")).GetComponent<colorFader2>().set(blackScreen, new Color(0, 0, 0, 1), .5f);
        }
        else
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/imageFader")).GetComponent<colorFader2>().set(blackScreen, new Color(1, 1, 1, 1), .5f);
        }
        
        yield return new WaitForSeconds(.5f);
        if (PlayerPrefs.GetInt("darkMode", 0) == 0)
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/imageFader")).GetComponent<colorFader2>().set(blackScreen, new Color(0, 0, 0, 0), .5f);
        }
        else
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/imageFader")).GetComponent<colorFader2>().set(blackScreen, new Color(1, 1, 1, 0), .5f);
        }
        musicPlaying.parent = mainGame.transform;
        musicPlaying.SetSiblingIndex(0);
        mainGame.SetActive(true);
        mainMenu.SetActive(false);
    }

    IEnumerator fadeTransition2()
    {
        if (PlayerPrefs.GetInt("darkMode", 0) == 0)
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/imageFader")).GetComponent<colorFader2>().set(blackScreen, new Color(0, 0, 0, 1), .5f);
        }
        else
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/imageFader")).GetComponent<colorFader2>().set(blackScreen, new Color(1, 1, 1, 1), .5f);
        }

        yield return new WaitForSeconds(.5f);
        if (PlayerPrefs.GetInt("darkMode", 0) == 0)
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/imageFader")).GetComponent<colorFader2>().set(blackScreen, new Color(0, 0, 0, 0), .5f);
        }
        else
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/imageFader")).GetComponent<colorFader2>().set(blackScreen, new Color(1, 1, 1, 0), .5f);
        }
        musicPlaying.parent = mainMenu.transform;
        musicPlaying.SetSiblingIndex(0);
        mainGame.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void goToVote()
    {
        StartCoroutine(fadeTransition());
        game.drawOrVote = true;
        game.changeMode(true);
    }
}
