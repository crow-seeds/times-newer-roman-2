using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class sampleCharacter : MonoBehaviour
{
    public RectTransform body;
    public letterList lList;
    public TextMeshProUGUI author;
    public TextMeshProUGUI character;
    public float speed;
    public RawImage img;
    public int num;

    // Start is called before the first frame update
    void Start()
    {
        getNewChar();
    }

    void getNewChar()
    {
        int amount = lList.cList.Length;
        int rand = Random.Range(0, amount);
        num = rand;
        characterInfo randC = lList.cList[rand];
        img.texture = Resources.Load<Texture2D>("Sprites/Characters/" + (rand + 1).ToString());
        author.text = "Drawn by: " + randC.author;
        character.text = "\"" + randC.character + "\"";
    }

    // Update is called once per frame
    void Update()
    {
        body.anchoredPosition = new Vector3(body.anchoredPosition.x - Time.deltaTime * speed, body.anchoredPosition.y, 0);

        if (body.anchoredPosition.x <= -1200)
        {
            body.anchoredPosition = new Vector3(1200, body.anchoredPosition.y, 0);
            getNewChar();
        }
    }
}
