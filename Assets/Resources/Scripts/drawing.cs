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


[RequireComponent(typeof(RawImage))]
public class drawing : MonoBehaviour
{

    RectTransform rt;
    RawImage ri;
    Vector3 bottomLeft = Vector3.zero;
    Vector3 topRight = Vector3.zero;
    Texture2D canvas;

    int width = 680;
    int height = 680;
    int brushSize = 10;

    [SerializeField] Camera cam;

    bool hasLastPosition = false;
    Vector2 lastPosition;

    List<string> potentialCharacters = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", ",", ".", "<", ">", "/", "?", ":", ";", "\'", "\"", "[", "{", "]", "}", "\\", "|", "-", "+", "_", "=", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "~", "`" };
    [SerializeField] TMP_InputField character;
    string characterDrawn;
    Color penColor = Color.black;
    [SerializeField] RawImage blackCircle;
    [SerializeField] RawImage whiteCircle;

    [SerializeField] GameObject circleIndicator;
    [SerializeField] GameObject squareIndicator;

    [SerializeField] List<GameObject> sizeIndicators = new List<GameObject>();
    List<int> sizeNums = new List<int> { 2, 6, 10, 20, 40, 80 };

    [SerializeField] TextMeshProUGUI xSnapText;
    [SerializeField] TextMeshProUGUI ySnapText;
    [SerializeField] TMP_InputField authorText;

    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI bigCharacter;
    public bool xSnap = false;
    public bool ySnap = false;

    bool isSquare = false;
    int lastSubmittedID = -1;

    List<Color32[]> timeslices = new List<Color32[]>();
    public int timesliceIndex = 0;

    [SerializeField] achievements achievementHandler;
    [SerializeField] copy clip;
    [SerializeField] string websiteRan;
    bool isSending = false;
    Color32[] oldTexture;

    // Start is called before the first frame update
    void Start()
    {
        // Getting the RectTransform, since this is a RawImage, which exists on the canvas and should have a rect transform
        rt = GetComponent<RectTransform>();
        // RawImage that we are going to be updating for our paint application.
        ri = GetComponent<RawImage>();
        if (ri != null)
        {
            CreateTexture2D();
        }
        getCoords();
        characterDrawn = potentialCharacters[Random.Range(0, potentialCharacters.Count)];
        character.text = characterDrawn;
        bigCharacter.text = characterDrawn;
    }

    public void changeCharacter()
    {
        characterDrawn = character.text;
        bigCharacter.text = characterDrawn;
    }

    public void setSize(int i)
    {
        brushSize = sizeNums[i];
        Debug.Log("pee");
        foreach (GameObject g in sizeIndicators)
        {
            g.SetActive(false);
        }
        sizeIndicators[i].SetActive(true);
    }

    public void getNewCharacter()
    {
        characterDrawn = potentialCharacters[Random.Range(0, potentialCharacters.Count)];
        character.text = characterDrawn;
    }

    public void penBlack()
    {
        penColor = Color.black;
        blackCircle.texture = Resources.Load<Texture2D>("Sprites/UI/blackActive");
        whiteCircle.texture = Resources.Load<Texture2D>("Sprites/UI/whiteInactive");
    }

    public void penWhite()
    {
        penColor = Color.white;
        blackCircle.texture = Resources.Load<Texture2D>("Sprites/UI/blackInactive");
        whiteCircle.texture = Resources.Load<Texture2D>("Sprites/UI/whiteActive");
    }

    public void penCircle()
    {
        isSquare = false;
        circleIndicator.SetActive(true);
        squareIndicator.SetActive(false);
    }

    public void penSquare()
    {
        isSquare = true;
        circleIndicator.SetActive(false);
        squareIndicator.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        // Make sure our stuff is valid
        if (rt != null)
        {
            if (ri != null)
            {
                HandleInput();
            }
        }
    }

    void HandleInput()
    {
        // Since we can only paint on the canvas if the mouse button is press
        // May be best to revise this so the tool has a call back for example a 
        // fill tool selected would call its own "Handle" method,

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
        {
            Vector2Int mousePos = Vector2Int.zero;
            // We have input, lets convert the mouse position to be relative to the canvas
            ConvertMousePosition(ref mousePos);
            if (xSnap)
            {
                mousePos.x = Mathf.RoundToInt(mousePos.x / 85f) * 85;
                if(mousePos.x == 680)
                {
                    mousePos.x = 679;
                }
            }

            if (ySnap)
            {
                mousePos.y = Mathf.RoundToInt(mousePos.y / 85f) * 85;
                if (mousePos.y == 680)
                {
                    mousePos.y = 679;
                }
            }


            // Checking that our mouse is in bounds, which is stored in our height and width variable and as long as it has a "positive value"
            if (MouseIsInBounds(mousePos))
            {
                // This method could be removed to be the tool method I mention above
                // you would pass in the mousePosition, and color similar to this.
                // This way each tool would be its "own" component that would be activated
                // through some form of UI.
                Color inverse = new Color(1 - penColor.r, 1 - penColor.g, 1 - penColor.b);


                if(Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
                {
                    PaintTexture(mousePos, inverse);
                }
                else {
                    PaintTexture(mousePos, penColor); // Also the color you want would be here to...
                }

                if (hasLastPosition)
                {
                    if(Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
                    {
                        PaintBetweenTwoPoints(mousePos, Vector2Int.RoundToInt(lastPosition), inverse);
                    }
                    else
                    {
                        PaintBetweenTwoPoints(mousePos, Vector2Int.RoundToInt(lastPosition), penColor);
                    }
                    
                }


                if (!hasLastPosition)
                {
                    hasLastPosition = true;
                }
                lastPosition = mousePos;
            }
            else
            {
                if (hasLastPosition)
                {
                    saveSlice();
                }

                hasLastPosition = false;
            }
        }
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {

            hasLastPosition = false;
            Vector2Int mousePos = Vector2Int.zero;
            // We have input, lets convert the mouse position to be relative to the canvas
            ConvertMousePosition(ref mousePos);

            // Checking that our mouse is in bounds, which is stored in our height and width variable and as long as it has a "positive value"
            if (MouseIsInBounds(mousePos))
            {
                saveSlice();
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    redo();
                }
                else
                {
                    undo();
                }
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                redo();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                clear();
            }


        }
    }


    public void setXSnap()
    {
        xSnap = !xSnap;
        if (!xSnap)
        {
            xSnapText.color = new Color(.315f, .315f, .315f);
        }
        else
        {
            xSnapText.color = new Color(.8f, .8f, .8f);
        }
    }

    public void setYSnap()
    {
        ySnap = !ySnap;
        if (!ySnap)
        {
            ySnapText.color = new Color(.315f, .315f, .315f);
        }
        else
        {
            ySnapText.color = new Color(.8f, .8f, .8f);
        }
    }

    void saveSlice()
    {
        if (timeslices.Count < 30 && timesliceIndex == timeslices.Count - 1)
        {
            timeslices.Add(canvas.GetPixels32());
            timesliceIndex++;
        }
        else if (timesliceIndex == 29)
        {
            for (int i = 0; i < 29; i++)
            {
                timeslices[i] = timeslices[i + 1];
            }
            timeslices[29] = canvas.GetPixels32();
        }
        else
        {
            Debug.Log("peen");
            timeslices[timesliceIndex + 1] = canvas.GetPixels32();
            for (int i = timesliceIndex + 2; i < timeslices.Count; i++)
            {
                timeslices.RemoveAt(timesliceIndex + 2);
            }
            timesliceIndex++;
        }
    }

    public void undo()
    {
        if (timesliceIndex > 0)
        {
            canvas.SetPixels32(timeslices[timesliceIndex - 1]);
            canvas.Apply(true);
            timesliceIndex = timesliceIndex - 1;
            Debug.Log(timesliceIndex);
        }
    }

    public void redo()
    {
        if (timesliceIndex < timeslices.Count - 1)
        {
            canvas.SetPixels32(timeslices[timesliceIndex + 1]);
            canvas.Apply(true);
            timesliceIndex = timesliceIndex + 1;
            Debug.Log(timesliceIndex);
        }
    }

    public void clear()
    {
        Color32 resetColor = new Color32(255, 255, 255, 255);
        Color32[] resetColorArray = canvas.GetPixels32();

        for (int i = 0; i < resetColorArray.Length; i++)
        {
            resetColorArray[i] = resetColor;
        }

        canvas.SetPixels32(resetColorArray);
        canvas.Apply();
        saveSlice();
    }

    void PaintBetweenTwoPoints(Vector2Int pos1, Vector2Int pos2, Color p)
    {
        Vector2 slope = Vector2.ClampMagnitude(pos2 - pos1, 1) * brushSize * .5f;
        Vector2 currentPos = pos1;

        while (Vector2.Distance(currentPos, pos2) > brushSize * .5f)
        {
            currentPos += slope;
            PaintTexture(Vector2Int.RoundToInt(currentPos), p);
        }
    }


    void PaintTexture(Vector2Int pos, Color color)
    {
        // In our method we don't allow transparency and we are just replacing the pixel,

        if (!isSquare)
        {
            for (int i = pos.y - brushSize; i < pos.y + brushSize; i++)
            {
                for (int j = pos.x; Mathf.Pow((j - pos.x), 2) + Mathf.Pow((i - pos.y), 2) <= Mathf.Pow(brushSize, 2); j--)
                {
                    if (i < 680 && j < 680 && i >= 0 && j >= 0)
                    {
                        canvas.SetPixel(j, i, color);
                    }

                }
                for (int j = pos.x + 1; (j - pos.x) * (j - pos.x) + (i - pos.y) * (i - pos.y) <= brushSize * brushSize; j++)
                {
                    if (i < 680 && j < 680 && i >= 0 && j >= 0)
                    {
                        canvas.SetPixel(j, i, color);
                    }
                }
            }
        }
        else
        {
            for (int i = pos.y - brushSize; i < pos.y + brushSize; i++)
            {
                for (int j = pos.x - brushSize; j < pos.x + brushSize; j++)
                {
                    if (i < 680 && j < 680 && i >= 0 && j >= 0)
                    {
                        canvas.SetPixel(j, i, color);
                    }

                }
            }
        }



        // Applying out change, we dont want to mip levels.
        // If you are doing some blending or transparency stuff that would be handled by your tool
        canvas.Apply(true);
    }

    bool MouseIsInBounds(Vector2Int mousePos)
    {
        // The position is already relative to the texture so if it is >= to 0 and less then the texture
        // width and height it is in bounds.
        if (mousePos.x >= 0 && mousePos.x < width)
        {
            if (mousePos.y >= 0 && mousePos.y < height)
            {
                return true;
            }
        }
        return false;
    }

    void ConvertMousePosition(ref Vector2Int mouseOut)
    {
        // The mouse Position, and the RawImage position are returned in the same space
        // So we can just update based off of that
        Vector3 real = cam.ScreenToWorldPoint(Input.mousePosition) - bottomLeft;
        mouseOut.x = Mathf.RoundToInt((real.x / 7.55f) * width);
        mouseOut.y = Mathf.RoundToInt((real.y / 7.55f) * width);
    }

    void getCoords()
    {
        if (rt != null)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            // Setting our corners  based on the fact GetCorners returns them in clockwise order starting from BL TL TR BR.
            bottomLeft = corners[0];
            topRight = corners[2];

            Debug.Log(topRight.x - bottomLeft.x);
            Debug.Log(topRight.y - bottomLeft.y);
        }
    }


    void CreateTexture2D()
    {
        // Creating our "Draw" texture to be the same size as our RawImage.
        canvas = new Texture2D(width, height);
        ri.texture = canvas;

        Color32 resetColor = new Color32(255, 255, 255, 255);
        Color32[] resetColorArray = canvas.GetPixels32();

        for (int i = 0; i < resetColorArray.Length; i++)
        {
            resetColorArray[i] = resetColor;
        }

        oldTexture = resetColorArray;
        canvas.SetPixels32(resetColorArray);
        canvas.Apply();
        timeslices.Add(canvas.GetPixels32());
        timesliceIndex = 0;
    }

    public void CopyToClipboard()
    {

        if (lastSubmittedID == -1)
        {
            StartCoroutine(fadeStatus("Submit First Before Sharing!"));
        }
        else
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                clip.copyToClip(characterDrawn + "\n" + "Drawn by: " + authorText.text + "\nTimes Newer Roman\n\n" + "https://crowseeds.com/font/images/" + lastSubmittedID.ToString() + ".png");
                achievementHandler.unlockAchievement(3);
            }
            else
            {
                GUIUtility.systemCopyBuffer = characterDrawn + "\n" + "Drawn by: " + authorText.text + "\nTimes Newer Roman\n\n" + "https://crowseeds.com/font/images/" + lastSubmittedID.ToString() + ".png";
            }
            StartCoroutine(fadeStatus("Copied to Clipboard!"));
            
        }

    }

    IEnumerator fadeStatus(string s)
    {
        statusText.text = s;
        Instantiate(Resources.Load<GameObject>("Prefabs/textFader")).GetComponent<colorFader3>().set(statusText, new Color(.315f, .315f, .315f, 1), .25f);
        yield return new WaitForSeconds(.5f);
        Instantiate(Resources.Load<GameObject>("Prefabs/textFader")).GetComponent<colorFader3>().set(statusText, new Color(.315f, .315f, .315f, 0), 2.5f);
    }

    public void submit()
    {
        if (!blankCheck())
        {
            StartCoroutine(fadeStatus("Can't Submit Blank Image!"));
        }else if (!similarityCheck())
        {
            StartCoroutine(fadeStatus("Can't Submit the Same Image Again!"));
        }
        else if (!timeCheck())
        {
            StartCoroutine(fadeStatus("Wait 3 Minutes Before Submitting!"));
        }else if(characterDrawn == "")
        {
            StartCoroutine(fadeStatus("Must have character!"));
        }
        else
        {
            if (!isSending)
            {
                isSending = true;
                StartCoroutine(uploadToDatabase());
                
            }
        }

    }

    bool similarityCheck()
    {
        Color32[] colorArray = canvas.GetPixels32();
        for (int i = 0; i < colorArray.Length; i++)
        {
            if (colorArray[i].r != oldTexture[i].r)
            {
                return true;
            }
        }
        return false;
    }

    bool blankCheck()
    {
        Color32[] colorArray = canvas.GetPixels32();
        for (int i = 0; i < colorArray.Length; i++)
        {
            if (colorArray[i].r == 0)
            {
                return true;
            }
        }

        return false;
    }

    bool timeCheck()
    {
        if ((int)((System.DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) - PlayerPrefs.GetInt("lastUpload", 0) < 180)
        {
            return false;
        }

        return true;
    }

    IEnumerator uploadToDatabase()
    {
        WWWForm form = new WWWForm();
        form.AddField("drawnCharacter", characterDrawn);
        form.AddField("author", authorText.text);
        form.AddBinaryData("userImage", canvas.EncodeToPNG());
        string website = "Windows Executable";

        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            website = websiteRan;
        }
        form.AddField("website", website);




        using (UnityWebRequest www = UnityWebRequest.Post("https://crowseeds.com/FONT/database.php", form))
        {
            yield return www.SendWebRequest();
            isSending = false;

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
                StartCoroutine(fadeStatus("Server Error, Try Again Later!"));
            }
            else
            {
                // Print response
                Debug.Log(www.downloadHandler.text);
                if (int.Parse(www.downloadHandler.text) >= 0)
                {
                    lastSubmittedID = int.Parse(www.downloadHandler.text);
                    Debug.Log(lastSubmittedID);
                    achievementHandler.unlockAchievement(0);
                    StartCoroutine(fadeStatus("Upload Succeeded!"));
                    PlayerPrefs.SetInt("lastUpload", (int)((System.DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds));
                    oldTexture = canvas.GetPixels32();
                }
                else
                {
                    StartCoroutine(fadeStatus("Strange Error!"));
                }
            }
        }
    }

    public void changeArtistName()
    {
        achievementHandler.unlockAchievement(5);
    }

    public void musicCredit()
    {
        Application.OpenURL("https://aghostinmyroom.newgrounds.com/");
    }


}