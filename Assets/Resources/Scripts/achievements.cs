using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class achievements : MonoBehaviour
{
    List<int> newgroundsMedalIDs = new List<int> { 70065, 70066, 70067, 70068, 70069, 70070, 70071, 70125 };
    //artist, hater, supporter, egoist, patron, signed, dark moder
    [SerializeField] GameObject newgrounds;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void unlockAchievement(int i)
    {
        if (newgrounds.activeSelf && Application.platform == RuntimePlatform.WebGLPlayer)
        {
            newgrounds.GetComponent<NGHelper>().unlockMedal(newgroundsMedalIDs[i]);
        }
    }
}
