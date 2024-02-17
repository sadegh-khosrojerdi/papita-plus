﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.Networking;
using UPersian.Components;

public class ScrollViewItem : MonoBehaviour
{
    // Start is called before the first frame update

    public Image childImage;
    public int flag;
    public bool isMovie;
    public int witchScroolView;
    public Font englishFont;
    public Font persianFont;

    string CardsName;
    bool CardsLock;
    void OnEnable()
    {

        Invoke("CheckName", 0.5f);
        Invoke("setButton", 0.7f);

        if (PlayerPrefs.GetInt("witchLang") == 1)
        {
            this.gameObject.transform.GetChild(3).gameObject.GetComponent<RtlText>().font = persianFont;
            this.gameObject.transform.GetChild(3).gameObject.GetComponent<RtlText>().fontSize = 50;
        }
        else
        {
            this.gameObject.transform.GetChild(3).gameObject.GetComponent<RtlText>().font = englishFont;
            this.gameObject.transform.GetChild(3).gameObject.GetComponent<RtlText>().fontSize = 50;
        }

    }

    void CheckName()
    {
        if (isMovie)
        {
            if (PlayerPrefs.GetInt("witchCat") == 5 | PlayerPrefs.GetInt("witchCat") == 25)
            {
                string inputString = PlayerPrefs.GetString("MovieName" + PlayerPrefs.GetInt("witchCat") + witchScroolView + flag);

                int separatorIndex = inputString.IndexOf('*');
                if (separatorIndex >= 0)
                {
                    string part1 = inputString.Substring(0, separatorIndex);
                    string part2 = inputString.Substring(separatorIndex + 1);

                    CardsName = part1;
                    if (part2.ToUpper().Contains("L"))
                    {
                        CardsLock = true;
                    }
                    else
                    {
                        CardsLock = false;
                    }
                }
                else
                {
                    Debug.LogError("رشته ورودی نامعتبر است، علامت '+' یافت نشد.");
                }
             

            }
            else
            {
                string inputString = PlayerPrefs.GetString("MovieName" + PlayerPrefs.GetInt("witchCat") + flag);


                int separatorIndex = inputString.IndexOf('*');
                if (separatorIndex >= 0)
                {
                    string part1 = inputString.Substring(0, separatorIndex);
                    string part2 = inputString.Substring(separatorIndex + 1);

                    CardsName = part1;
                    if (part2.ToUpper().Contains("L"))
                    {
                        CardsLock = true;
                    }
                    else
                    {
                        CardsLock = false;
                    }
                }
                else
                {
                    Debug.LogError("رشته ورودی نامعتبر است، علامت '+' یافت نشد.");
                }


            }
        }
        else
        {
            

                string inputString = PlayerPrefs.GetString("GameName" + PlayerPrefs.GetInt("witchCat") + flag);

            int separatorIndex = inputString.IndexOf('*');
            if (separatorIndex >= 0)
            {
                string part1 = inputString.Substring(0, separatorIndex);
                string part2 = inputString.Substring(separatorIndex + 1);
                Debug.Log(part2);

                CardsName = part1;
                if (part2.ToUpper().Contains("L"))
                {
                    CardsLock = true;
                }
                else
                {
                    CardsLock = false;
                }
            }
            else
            {
                Debug.LogError("رشته ورودی نامعتبر است، علامت '+' یافت نشد.");
            }


        }

        if (PlayerPrefs.GetInt("freeFirstTime") == 1)
            CardsLock = false;
    }
    void setButton()
    {
        if (!isMovie)    // برای داستان سیو کردن اسکرول ویو بازی همیشه 100 است
            witchScroolView = 100;
        if (flag == 0)
        {
            InternetAvailabilityTest.instance.CheckConnection();
        }
        if (isMovie)
        {
           
            if (PlayerPrefs.GetInt("witchCat") == 5 | PlayerPrefs.GetInt("witchCat") == 25)
            {

                this.gameObject.transform.GetChild(3).gameObject.GetComponent<RtlText>().text = CardsName;
                if (PlayerPrefs.GetInt("with_Ad") == 0 )
                {
                    if (CardsLock)
                    {
                        this.gameObject.transform.GetChild(4).gameObject.SetActive(true);  //locked
                        GetComponent<Button>().onClick.AddListener(() => OpenBox(0));
                    }
                    else
                    {
                        this.gameObject.transform.GetChild(4).gameObject.SetActive(false);  //open
                        GetComponent<Button>().onClick.AddListener(() => OpenLink(flag, PlayerPrefs.GetInt("prouductIDMovie" + PlayerPrefs.GetInt("witchCat") + witchScroolView + flag)));
                    }
                    
                }
                else
                {
                    this.gameObject.transform.GetChild(4).gameObject.SetActive(false);  //open
                    GetComponent<Button>().onClick.AddListener(() => OpenLink(flag, PlayerPrefs.GetInt("prouductIDMovie" + PlayerPrefs.GetInt("witchCat") + witchScroolView + flag)));

                }
              

                // نشان دادن تایم ویدیو
                if (PlayerPrefs.HasKey("movieLenght" + PlayerPrefs.GetInt("witchCat")+witchScroolView + flag))
                {
                    Debug.Log("we are in cat 5");
                  
                    this.gameObject.transform.GetChild(2).gameObject.SetActive(true);
                    this.gameObject.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Text>().text =
                         PlayerPrefs.GetString("timerCounter" + PlayerPrefs.GetInt("witchCat")+witchScroolView + flag) + " / " +
                        PlayerPrefs.GetString("movieLenght" + PlayerPrefs.GetInt("witchCat")+witchScroolView + flag);
                }
                else
                {
                    this.gameObject.transform.GetChild(0).gameObject.SetActive(false);

                }

               
            }
            else
            {
                this.gameObject.transform.GetChild(3).gameObject.GetComponent<RtlText>().text = CardsName;
                Debug.Log("make movie");
                if (PlayerPrefs.GetInt("with_Ad") == 0)
                {
                    if (CardsLock) 
                    {
                        this.gameObject.transform.GetChild(4).gameObject.SetActive(true);  //locked
                        GetComponent<Button>().onClick.AddListener(() => OpenBox(0));
                    }
                    else // اگه اکانتش تموم شده اما از سمت سرور بازه
                    {

                        this.gameObject.transform.GetChild(4).gameObject.SetActive(false);  //open

                        GetComponent<Button>().onClick.AddListener(() => OpenLink(flag, PlayerPrefs.GetInt("prouductIDMovie" + PlayerPrefs.GetInt("witchCat") + flag)));
                    }
                    
                }
                else // اکانت داره دیگه کارت لاک رو بررسی نکن
                {

                    this.gameObject.transform.GetChild(4).gameObject.SetActive(false);  //open

                    GetComponent<Button>().onClick.AddListener(() => OpenLink(flag, PlayerPrefs.GetInt("prouductIDMovie" + PlayerPrefs.GetInt("witchCat") + flag)));

                }


                // نشان دادن تایم ویدیو
                if (PlayerPrefs.HasKey("movieLenght" + PlayerPrefs.GetInt("witchCat") + flag))
                {
                  
                    this.gameObject.transform.GetChild(2).gameObject.SetActive(true);
                    this.gameObject.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Text>().text =
                         PlayerPrefs.GetString("timerCounter" + PlayerPrefs.GetInt("witchCat") + flag) + " / " +
                        PlayerPrefs.GetString("movieLenght" + PlayerPrefs.GetInt("witchCat") + flag);
                }
                else
                {
                    this.gameObject.transform.GetChild(0).gameObject.SetActive(false);

                }
            }
         

        }
        else   /// game
        {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            this.gameObject.transform.GetChild(2).gameObject.SetActive(false);

            this.gameObject.transform.GetChild(3).gameObject.GetComponent<RtlText>().text = CardsName;


            if (PlayerPrefs.GetInt("with_Ad") == 0  )
            {
                if (!CardsLock)
                {
                    this.gameObject.transform.GetChild(4).gameObject.SetActive(false);  //open
                    if (float.Parse(PlayerPrefs.GetString("Game_Version" + PlayerPrefs.GetInt("witchCat") + flag)) <= float.Parse(Application.version))
                    {
                        if (PlayerPrefs.GetString("Game_Scene_Name" + PlayerPrefs.GetInt("witchCat") + flag) == "comming soon")
                        {

                            GetComponent<Button>().onClick.AddListener(() => playBtnSound());

                        }
                        else
                        {
                            GetComponent<Button>().onClick.AddListener(() => OpenGame());

                        }
                    }
                    else
                    {
                        GetComponent<Button>().onClick.AddListener(() => OpenBox(1));
                    }
                }
                else
                {
                    if (PlayerPrefs.GetString("Game_Scene_Name" + PlayerPrefs.GetInt("witchCat") + flag) == "comming soon")
                    {
                        GetComponent<Button>().onClick.AddListener(() => playBtnSound());
                    }
                    else
                    {
                        this.gameObject.transform.GetChild(4).gameObject.SetActive(true);  //locked
                        GetComponent<Button>().onClick.AddListener(() => OpenBox(0));
                    }
                }
              
               
            }
            else
            {
              

                    this.gameObject.transform.GetChild(4).gameObject.SetActive(false);  //open
                    if (float.Parse(PlayerPrefs.GetString("Game_Version" + PlayerPrefs.GetInt("witchCat") + flag)) <= float.Parse(Application.version))
                    {
                        if (PlayerPrefs.GetString("Game_Scene_Name" + PlayerPrefs.GetInt("witchCat") + flag) == "comming soon")
                        {

                            GetComponent<Button>().onClick.AddListener(() => playBtnSound());

                        }
                        else
                        {
                            GetComponent<Button>().onClick.AddListener(() => OpenGame());

                        }
                    }
                    else
                    {
                        GetComponent<Button>().onClick.AddListener(() => OpenBox(1));
                    }
                
               

              
            }
         
          
        }
    }
   
   void playBtnSound()
    {
        AudioController.instance.playSound(3);
    }

    void OpenGame()
    {
        if (UnityEngine.PlayerPrefs.GetInt("with_Ad") == 0)
            AdiveryConfig.instance.showRewardedAd();

        string saveing = PlayerPrefs.GetInt("prouductIDMovie" + PlayerPrefs.GetInt("witchCat")).ToString() + witchScroolView.ToString();
        float saveedValue = this.gameObject.transform.parent.transform.parent.transform.parent.gameObject.GetComponent<ScrollRectEx>().horizontalScrollbar.value;
        PlayerPrefs.SetFloat(saveing, saveedValue);

        playGameScript.pgs.playGamePressed(flag, PlayerPrefs.GetInt("prouductID" + PlayerPrefs.GetInt("witchCat") + flag), true);
    }
   
    void OpenLink(int f , int id)
    {
        if (UnityEngine.PlayerPrefs.GetInt("with_Ad") == 0)
            AdiveryConfig.instance.showRewardedAd();

        string saveing = PlayerPrefs.GetInt("prouductIDMovie" + PlayerPrefs.GetInt("witchCat")).ToString() + witchScroolView.ToString();
        float saveedValue = this.gameObject.transform.parent.transform.parent.transform.parent.gameObject.GetComponent<ScrollRectEx>().horizontalScrollbar.value;
        PlayerPrefs.SetFloat(saveing, saveedValue);
      
         StartCoroutine(SendProudutIdToServer(PlayerPrefs.GetString("base_url") +"/categories/product-user", id));
        
            if (PlayerPrefs.GetInt("witchCat") == 5 | PlayerPrefs.GetInt("witchCat") == 25)
            {
                AudioController.instance.playSound(1);
                AudioController.instance.Stopbackground();
                PlayerPrefs.SetInt("VideoFlag", f);
                PlayerPrefs.SetInt("witchScroolView", witchScroolView);
                SubMenuManager.instance.changeScene("video_1");
            }
            else
            {
                AudioController.instance.playSound(1);
                AudioController.instance.Stopbackground();
                PlayerPrefs.SetInt("VideoFlag", f);
                SubMenuManager.instance.changeScene("video_1");
            }


       

    }
    string body;
    void OpenBox(int a)
    {
        if (a == 0) // open box lock
        {
            
            SubMenuManager.instance.LockBox.SetActive(true);
            SubMenuManager.instance.audioSource.Play();
        }
        else
        {
            setBtn();
            SubMenuManager.instance.boxUpdate.SetActive(true);
            SubMenuManager.instance.boxUpdate.transform.GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(body));
        }


    }
    void setBtn()
    {
        if (UnityEngine.PlayerPrefs.GetInt("store") == 1)
        {

            body = "https://play.google.com/store/apps/details?id=" + Application.identifier;


        }
        else if (UnityEngine.PlayerPrefs.GetInt("store") == 2)
        {
            body = "https://cafebazaar.ir/app/" + Application.identifier;

        }

        else if (UnityEngine.PlayerPrefs.GetInt("store") == 3)
        {
            body = "https://myket.ir/app/" + Application.identifier;

        }
        else if (UnityEngine.PlayerPrefs.GetInt("store") == 5)
        {
            body = "https://toopmarket.com/app/" + Application.identifier;

        }
        else
        {
            body = "https://play.google.com/store/apps/details?id=" + Application.identifier;
        }

    }
    public void downImage(int a)
    {
        if (isMovie)
        {
            this.gameObject.transform.GetChild(1).gameObject.SetActive(true); //btn play video
            if (PlayerPrefs.GetInt("witchCat") == 5 | PlayerPrefs.GetInt("witchCat") == 25)
            {
                if (PlayerPrefs.HasKey("movieLenght" + PlayerPrefs.GetInt("witchCat")+witchScroolView + flag) & File.Exists(Application.persistentDataPath + "VideoScreenShot" + PlayerPrefs.GetInt("witchCat")+witchScroolView + flag + ".png"))
                {
                    LoadImageFromDisk("VideoScreenShot" + PlayerPrefs.GetInt("witchCat") +witchScroolView+ flag + ".png");
                    Debug.Log("screen shot taken by user");
                    this.gameObject.transform.GetChild(0).gameObject.SetActive(true); //screen video
                    OnLoadImage(PlayerPrefs.GetString("ScreenShotMovie" + PlayerPrefs.GetInt("witchCat") + witchScroolView + a), PlayerPrefs.GetString("ScreenShotMovieName" + PlayerPrefs.GetInt("witchCat") + witchScroolView + a));

                }
                else
                {
                    if (!File.Exists(Application.persistentDataPath + PlayerPrefs.GetString("ScreenShotMovieName" + PlayerPrefs.GetInt("witchCat")+witchScroolView + a)) )
                    {
                        Debug.LogError("screen shot Movie Not Exist!");
                        this.gameObject.transform.GetChild(0).gameObject.SetActive(false); //screen video
                        OnLoadImage(PlayerPrefs.GetString("ScreenShotMovie" + PlayerPrefs.GetInt("witchCat") +witchScroolView+ a), PlayerPrefs.GetString("ScreenShotMovieName" + PlayerPrefs.GetInt("witchCat") +witchScroolView+ a));
                    }
                    else
                    {
                        LoadImageFromDisk(PlayerPrefs.GetString("ScreenShotMovieName" + PlayerPrefs.GetInt("witchCat") +witchScroolView+ a));
                        this.gameObject.transform.GetChild(0).gameObject.SetActive(true); //screen video
                        OnLoadImage(PlayerPrefs.GetString("ScreenShotMovie" + PlayerPrefs.GetInt("witchCat") + witchScroolView + a), PlayerPrefs.GetString("ScreenShotMovieName" + PlayerPrefs.GetInt("witchCat") + witchScroolView + a));

                    }
                }
            }
            else
            {
              
                    if (PlayerPrefs.HasKey("movieLenght" + PlayerPrefs.GetInt("witchCat") + flag) & File.Exists(Application.persistentDataPath + "VideoScreenShot" + PlayerPrefs.GetInt("witchCat") + flag + ".png"))
                    {
                        LoadImageFromDisk("VideoScreenShot" + PlayerPrefs.GetInt("witchCat") + flag + ".png");
                        this.gameObject.transform.GetChild(0).gameObject.SetActive(true); //screen video
                        Debug.Log("screen shot taken by user");
                        OnLoadImage(PlayerPrefs.GetString("ScreenShotMovie" + PlayerPrefs.GetInt("witchCat") +  a), PlayerPrefs.GetString("ScreenShotMovieName" + PlayerPrefs.GetInt("witchCat") +  a));

                    }
                    else
                    {
                        if (!File.Exists(Application.persistentDataPath + PlayerPrefs.GetString("ScreenShotMovieName" + PlayerPrefs.GetInt("witchCat") + a)))
                        {
                            Debug.Log("screen shot Movie Not Exist!");
                            this.gameObject.transform.GetChild(0).gameObject.SetActive(false); //screen video
                            OnLoadImage(PlayerPrefs.GetString("ScreenShotMovie" + PlayerPrefs.GetInt("witchCat") + a), PlayerPrefs.GetString("ScreenShotMovieName" + PlayerPrefs.GetInt("witchCat") + a));
                        }
                        else
                        {
                            LoadImageFromDisk(PlayerPrefs.GetString("ScreenShotMovieName" + PlayerPrefs.GetInt("witchCat") + a));
                            this.gameObject.transform.GetChild(0).gameObject.SetActive(true); //screen video
                            OnLoadImage(PlayerPrefs.GetString("ScreenShotMovie" + PlayerPrefs.GetInt("witchCat") +  a), PlayerPrefs.GetString("ScreenShotMovieName" + PlayerPrefs.GetInt("witchCat") +  a));

                        }
                    }
                
              
            }
       
            
          
        }
        else
        {
            if (!File.Exists(Application.persistentDataPath + PlayerPrefs.GetString("ScreenShotGameName" + PlayerPrefs.GetInt("witchCat") + a)))
            {
                Debug.LogError("screen shot Game Not Exist!");
                OnLoadImage(PlayerPrefs.GetString("ScreenShotGame" + PlayerPrefs.GetInt("witchCat") + a), PlayerPrefs.GetString("ScreenShotGameName" + PlayerPrefs.GetInt("witchCat") + a));

            }
            else
            {
                LoadImageFromDisk(PlayerPrefs.GetString("ScreenShotGameName" + PlayerPrefs.GetInt("witchCat") + a));

            }

        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////
    ////////////////////////////////////////



    [SerializeField]
    public string imageURL;
    public string fileName;

    public void OnLoadImage(string url, string Fname)
    {
        imageURL = url;

        fileName = Fname;
       // Debug.Log("my url Down:" + imageURL + "\n" + "png name : " + fileName);
        StartCoroutine(LoadTextureFromWeb());

    }

    IEnumerator LoadTextureFromWeb()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error: " + www.error);
            SubMenuManager.instance.boxNetIcon.SetActive(true);
        }
        else
        {
            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
            childImage.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
            childImage.SetNativeSize();
            OnSaveImageButtonClick();


        }
    }

    public void OnLoadImageFromDiskButtonClick()
    {
        if (!File.Exists(Application.persistentDataPath + fileName))
        {
            Debug.LogError("File Not Exist!");
            return;
        }

        // LoadImageFromDisk();
    }
    // load texture bytes from disk and compose sprite from bytes
    public void LoadImageFromDisk(string Fnames)
    {
        fileName = Fnames;
        byte[] textureBytes = File.ReadAllBytes(Application.persistentDataPath + fileName);
       // Debug.Log("load image from this Addres : " + Application.persistentDataPath + fileName);
        Texture2D loadedTexture = new Texture2D(0, 0);
        loadedTexture.LoadImage(textureBytes);
        if (isMovie)
        {
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().SetNativeSize();
        }
        else
        {
            childImage.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
            childImage.SetNativeSize();
        }
     
      
    }

    public void OnSaveImageButtonClick()
    {
        if (childImage.sprite == null)
        {
            Debug.LogError("No Image to Save!");
            return;
        }

        WriteImageOnDisk();
    }

    private void WriteImageOnDisk()
    {
        byte[] textureBytes = childImage.sprite.texture.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + fileName, textureBytes);
        Debug.Log("File Written On Disk!");
     
    }
   
    /// <summary>
    /// click event - make image blank 
    /// </summary>
    public void OnBlankImageButtonClick()
    {
        childImage.sprite = null;
    }


    IEnumerator SendProudutIdToServer(string uri,int pID)
    {
        //string prouductId = "[" + pID+ "]";
        WWWForm form = new WWWForm();

        form.AddField("products_id", pID);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, form))
        {

            webRequest.SetRequestHeader("Authorization", "Bearer " + UnityEngine.PlayerPrefs.GetString("user_token"));
            webRequest.SetRequestHeader("Accept", "application/json");
            //point:
            // BypassCertificate class in the main app controller script
            webRequest.chunkedTransfer = false;
            webRequest.certificateHandler = new BypassCertificate();
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

           
            Debug.Log(webRequest.downloadHandler.text);
            if ((webRequest.downloadHandler.text).Contains("Error"))
            {
              
            }
            else if (System.String.IsNullOrEmpty(webRequest.downloadHandler.text))
            {
               

            }
            else
            {
                Debug.Log("prouduct id : " + pID + " sent To Server");
            }
        }

    }

}
