using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class colorFader3 : MonoBehaviour
{
    TextMeshProUGUI obj;
    Color destColor;
    Color sourceColor;

    float time = 0;
    float duration;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime / duration;


        if (obj == null)
        {
            Destroy(gameObject);
        }
        else if (time > 1)
        {
            obj.color = destColor;
            Destroy(gameObject);
        }
        else
        {
            obj.color = Color.Lerp(sourceColor, destColor, time);
        }
    }

    public void set(TextMeshProUGUI o, Color d, float dur)
    {
        obj = o;
        destColor = d;
        sourceColor = obj.color;
        duration = dur;
    }
}
