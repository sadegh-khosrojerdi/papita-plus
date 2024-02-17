using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UPersian.Components;

public class SubMenuManager : MonoBehaviour
{
    public static SubMenuManager instance;
    public Animator menuUI;
    public RtlText menuName;
    public List<GameObject> prouduct;
    public List<RtlText> ProuductName;
    public Color loadToColor;
    int witchPackage;
    public GameObject nullGame,boxUpdate,LockBox, boxNetIcon,boxNeedsUpdate,introPage;
    public GameObject[] dogsIntro;
    public RtlText nullText, PlayerPosition, PlayerCoins;
    [HideInInspector]
    public int DownloadFinished;
    public AudioClip m_audio;
    public AudioSource audioSource;
    public Color[] backgrounds;
    public Sprite[] background_image,headersImage, uiSprite;
    public Image m_back,header,ui_buttons_home,ui_buttons_buy;
 
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        /*
        if (Screen.width > 2300)
        {
            menuUI.Play("submenu menu 2400");
        }
        else
        {
            menuUI.Play("submenu menu 1920");
        }
        */
    }
    void Start()
    {
        introToPage();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = m_audio;
        if(GameObject.Find("Audio Controller"))
        if (PlayerPrefs.GetInt("with_Ad") == 1 & AdiveryConfig.instance.BannerIsShowing)
            AdiveryConfig.instance.OnBannerAdDestroy();

        PlayerCoins.text = UnityEngine.PlayerPrefs.GetInt("coins").ToString();
        if (PlayerPrefs.GetInt("with_Ad") == 0)
        {
            PlayerPosition.text = "0";
        }
        else
        {
            PlayerPosition.text = PlayerPrefs.GetString("player_Position");
        }
        
        if (PlayerPrefs.GetInt("comeForBuy") == 1) // az setting omade bekhare
        {
            PlayerPrefs.SetInt("comeForBuy", 0);
            LockBox.SetActive(true);
            audioSource.Play();
        }

        
            if ( AudioController.instance.canPlay)
            {
                
                AudioController.instance.playbackground();
            }

        // اپدیت جدید منتشر شده اما فورس نیست
        if (float.Parse(Application.version) < PlayerPrefs.GetFloat("minorVersion"))
        {
            boxNeedsUpdate.SetActive(true);
            setBtn();
            boxNeedsUpdate.transform.GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(body));
        }

        StartCoroutine(GetAccPay(PlayerPrefs.GetString("base_url") + "/account/tariffs"));

        makeBackground();

        loadToColor =  backgrounds[PlayerPrefs.GetInt("witchCat")];
    }
    async void introToPage()
    {
        if (PlayerPrefs.GetInt("witchCat") == 5 | PlayerPrefs.GetInt("witchCat") == 25)
        {
            introPage.GetComponent<Image>().color = backgrounds[PlayerPrefs.GetInt("witchCat")];

            introPage.SetActive(true);
            for (int i = 0; i < dogsIntro.Length; i++)
            {
                    dogsIntro[i].SetActive(false);
            }
            dogsIntro[2].SetActive(true);
            await System.Threading.Tasks.Task.Delay(3500);
            introPage.SetActive(false);
        }
        else
        {
            introPage.GetComponent<Image>().color = backgrounds[PlayerPrefs.GetInt("witchCat")];

            introPage.SetActive(true);
            int w = Random.Range(0, dogsIntro.Length + 1);
            for (int i = 0; i < dogsIntro.Length; i++)
            {
                if (i == w)
                    dogsIntro[i].SetActive(true);
                else
                    dogsIntro[i].SetActive(false);
            }
            await System.Threading.Tasks.Task.Delay(1000);
            introPage.SetActive(false);
        }
      
      
    }
    void makeBackground()
    {
        int witch = PlayerPrefs.GetInt("witchCat");

        m_back.sprite = background_image[witch];
        header.sprite = headersImage[witch];
        ui_buttons_home.sprite = uiSprite[witch];
        ui_buttons_buy.sprite = uiSprite[witch];
    }
    public void getProducts()
    {
        for (int s = 0; s < PlayerPrefs.GetInt("categoriesCount"); s++)
        {
            if (PlayerPrefs.GetInt("witchCat") == s)
            {
                menuName.text = PlayerPrefs.GetString("categories" + s);
            }
        }

        for (int i = 0; i < prouduct.Count; i++)
        {
            if (i < PlayerPrefs.GetInt("productCount"))
            {
                prouduct[i].SetActive(true);

            }
            else
            {
                prouduct[i].SetActive(false);
            }
        }

        for (int i = 0; i < ProuductName.Count; i++)
        {
            if (i < PlayerPrefs.GetInt("productCount"))
            {

                ProuductName[i].text = PlayerPrefs.GetString("categories" + i + "product" + i);
                //Debug.Log(ProuductName[i].text + "    " + ProuductName[i].text.Length);

                if (ProuductName[i].text.Length > 13)
                {
                    ProuductName[i].fontSize = 30;
                }
            }

        }

    



    }
    void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {

            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                Initiate.Fade("MainmenuGame", loadToColor, 3f);
            }
        }
    }
    int CountMovie = 0;
    int CountGame = 0;

    IEnumerator GetAccPay(string urlCheck)
    {
        Debug.Log("token : " + UnityEngine.PlayerPrefs.GetString("user_token"));
        using (UnityWebRequest webRequest = UnityWebRequest.Get(urlCheck))
        {

            webRequest.SetRequestHeader("Authorization", "Bearer " + UnityEngine.PlayerPrefs.GetString("user_token"));
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.chunkedTransfer = false;
            webRequest.certificateHandler = new BypassCertificate();
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = urlCheck.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                
            }
            else
            {


                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                if (webRequest.downloadHandler.text.Contains("Unauthenticated"))
                {
                    Debug.Log("user paride !!!");
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("اطلاعات کاربر پیدا نشد لطفا دوباره وارد شوید");
#endif
                }
                else
                {
                    if (string.IsNullOrEmpty(webRequest.downloadHandler.text))
                    {
                        // اتمام اشتراک
                        PlayerPrefs.SetInt("MyDays", 0);
                        PlayerPrefs.SetInt("with_Ad", 0);

                    }
                    else
                    {
                        CheckPayData(webRequest.downloadHandler.text);


                    }
                }


            }
        }

    }
    public void CheckPayData(string _url)
    {

        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<RootCheckPayment>(_url);


            if (user.countday == null)
            {
                
                // BuyButton[0].gameObject.SetActive(false);
                PlayerPrefs.SetInt("MyDays", 0);
                PlayerPrefs.SetInt("with_Ad", 0);
            }
            else
            {
                PlayerPrefs.SetInt("MyDays", int.Parse(user.countday));
                Debug.LogError("day of user : " + PlayerPrefs.GetInt("MyDays"));
                Debug.Log("start time : " + user.start_day);
                Debug.Log("start time : " + user.end_day);
                PlayerPrefs.SetString("start_Day", user.start_day);
                PlayerPrefs.SetString("end_Day", user.end_day);
                if (PlayerPrefs.GetInt("MyDays") > 0)
                {


                    PlayerPrefs.SetInt("loginSucces", 2);
                    PlayerPrefs.SetInt("with_Ad", 1);
                }
                else
                {
                  
                    PlayerPrefs.SetInt("loginSucces", 1);
                    PlayerPrefs.SetInt("with_Ad", 0);
                }
            }
        






    }
    string body;
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

    public void changeScene(string Scene)
    {
        if (GameObject.Find("Audio Controller")) 
            AudioController.instance.playSound(1);
        Initiate.Fade(Scene, loadToColor, 3f);
    }
}
[System.Serializable]


public class RootGetLinks
{
    public int id { get; set; }
    public string file_link { get; set; }
    public string file_name { get; set; }
    public string screenshot_link { get; set; }
    public string screenshot_name { get; set; }
    public string package_id { get; set; }
    public object created_at { get; set; }
    public object updated_at { get; set; }
}
