using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class musicRandomizer : MonoBehaviour
{
    List<int> musicNums = new List<int>{0,1,2,3,4,5,6,7};
    [SerializeField] List<string> musicCredits = new List<string>();
    int songIndex = 0;
    AudioSource aud;
    [SerializeField] TextMeshProUGUI musicText;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("firstTime", 0) == 0)
        {
            PlayerPrefs.SetInt("firstTime", 1);
            Shuffle(musicNums);
            int ind = musicNums.IndexOf(1);
            int numAt0 = musicNums[0];
            musicNums[0] = 1;
            musicNums[ind] = numAt0;
        }
        else
        {
            Shuffle(musicNums);
        }

        aud = gameObject.GetComponent<AudioSource>();
        StartCoroutine(playSong());
    }

    IEnumerator playSong()
    {
        if(songIndex >= musicNums.Count)
        {
            songIndex = 0;
        }
        aud.clip = Resources.Load<AudioClip>("Music/" + (musicNums[songIndex] + 1).ToString());
        aud.Play();
        StartCoroutine(fadeMusicText(musicCredits[musicNums[songIndex]]));
        songIndex++;
        yield return new WaitForSeconds(aud.clip.length);
        StartCoroutine(playSong());
    }

    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator fadeMusicText(string s)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/textFader")).GetComponent<colorFader3>().set(musicText, new Color(.315f, .315f, .315f, 0), 3f);
        yield return new WaitForSeconds(3.1f);
        musicText.text = s;
        Instantiate(Resources.Load<GameObject>("Prefabs/textFader")).GetComponent<colorFader3>().set(musicText, new Color(.315f, .315f, .315f, 1), 3f);
    }
}
