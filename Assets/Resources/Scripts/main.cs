using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class main : MonoBehaviour
{
    [SerializeField] RawImage drawTab;
    [SerializeField] RawImage voteTab;
    [SerializeField] TextMeshProUGUI drawText;
    [SerializeField] TextMeshProUGUI voteText;
    [SerializeField] GameObject drawStuff;
    [SerializeField] GameObject voteStuff;
    public bool drawOrVote = false; //draw = false, vote = true

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void changeMode(bool b)
    {
        if (b)
        {
            drawTab.color = Color.black;
            drawText.color = Color.black;
            voteTab.color = new Color(.8f, .8f, .8f);
            voteText.color = new Color(.8f, .8f, .8f);
            voteStuff.SetActive(true);
            drawStuff.SetActive(false);
        }
        else
        {
            voteTab.color = Color.black;
            voteText.color = Color.black;
            drawTab.color = new Color(.8f, .8f, .8f);
            drawText.color = new Color(.8f, .8f, .8f);
            voteStuff.SetActive(false);
            drawStuff.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
