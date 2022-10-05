using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;
using Application = UnityEngine.Application;
using UnityEngine.Networking;
using System;
using System.Runtime.InteropServices;

public class copy : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void CopyToClipboard(string text);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void copyToClip(string t)
    {
        CopyToClipboard(t);
        GUIUtility.systemCopyBuffer = t;
        Debug.Log("copying!!!!");
    }
}
