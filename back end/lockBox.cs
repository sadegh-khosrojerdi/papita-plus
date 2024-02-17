using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UPersian.Components;
using System;
using io.adtrace.sdk;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class lockBox : MonoBehaviour
{
    public static lockBox instance;
    public Button[] BuyButton;
    public GameObject boxDis,boxMessageDis;
    public RtlText alarm,messageDisc;
    public Text userCode;
    public Text discont;
    public Text AlarmDiscount;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void OnEnable()
    {
        alarm.text = "براي حذف تبليغات و استفاده از اين محتوا نياز به خريد اشتراک داريد";
        alarm.color = Color.black;
        StartCoroutine(GetTaarifs(PlayerPrefs.GetString("base_url") + "/tariffs"));

        if (PlayerPrefs.HasKey("phoneNumber"))
        {
            BuyButton[5].gameObject.SetActive(false);
        }
        else
        {
            BuyButton[5].gameObject.SetActive(true);
            BuyButton[5].onClick.AddListener(() => GoForLogin());
        }
        discont.text = null;
        textToCopy = UnityEngine.PlayerPrefs.GetString("m_sarial");
        AlarmDiscount.text = "کد تخفيف خود را وارد نماييد";

        if (PlayerPrefs.GetInt("showDicoBox") == 1 & PlayerPrefs.GetInt("MyDays") < 180)
        {
            boxMessageDis.SetActive(true);
            messageDisc.text = PlayerPrefs.GetString("Dico_Message");
        }
        else
        {
            boxMessageDis.SetActive(false);
        }

    }
    public void OpenContact(int a)
    {

        if (a == 0)
        {
            Application.OpenURL(PlayerPrefs.GetString("link_Insta"));
        }
        else if (a == 1)
        {
            Application.OpenURL(PlayerPrefs.GetString("link_telegram"));
        }
        else if (a == 2)
        {
            Application.OpenURL(PlayerPrefs.GetString("link_whatsApp"));
        }

        Firebase.Analytics.FirebaseAnalytics.LogEvent("Contact_us", "Contact_us_app", 1);

    }

    string textToCopy;

    public void CopyToClipboard()
    {



        userCode.text = "user Code :" + textToCopy;
        // دسترسی به کلاس ClipboardManager در سیستم عامل اندروید
        AndroidJavaObject unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject clipboardManager = unityActivity.Call<AndroidJavaObject>("getSystemService", "clipboard");

        // ایجاد یک شیء ClipData جدید با متن مورد نظر
        AndroidJavaClass clipDataClass = new AndroidJavaClass("android.content.ClipData");
        AndroidJavaObject clipData = clipDataClass.CallStatic<AndroidJavaObject>("newPlainText", "text", textToCopy);

        // کپی کردن متن به کلیپ بورد
        clipboardManager.Call("setPrimaryClip", clipData);

        Debug.Log("Text copied to clipboard: " + textToCopy);
#if UNITY_IOS
        userCode.text = "user Code :" + textToCopy;
       TouchScreenKeyboard keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
            keyboard.text = textToCopy;
            keyboard.active = true;
            keyboard.text = textToCopy;
            keyboard.active = false;
#endif

        Debug.Log("Clipboard copy is not supported on this platform.");




    }

    void OnApplicationFocus()
    {
        Debug.Log(counterFoucos);
        if (GoForPay & counterFoucos == 1)
        {
            StartCoroutine(GetAccPay(PlayerPrefs.GetString("base_url") + "/account/tariffs"));
            Debug.Log("checking pay in foucos");
            counterFoucos = 0;
        }
        else
        {
            counterFoucos++;
        }

        if (PlayerPrefs.HasKey("user_token") & !GoForPay)
        {

            //خریدشو چک میکنه

            StartCoroutine(GetAccPay(PlayerPrefs.GetString("base_url") + "/account/tariffs"));

        }



    }
    public void AfterBuy()
    {
        StartCoroutine(GetAccPay(PlayerPrefs.GetString("base_url") + "/account/tariffs"));
    }

    IEnumerator GetTaarifs(string uri)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {

            webRequest.SetRequestHeader("Authorization", "Bearer " + UnityEngine.PlayerPrefs.GetString("user_token"));
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.chunkedTransfer = false;
            webRequest.certificateHandler = new BypassCertificate();
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                if (webRequest.downloadHandler.text.Contains("Unauthenticated"))
                {
                    Debug.Log("user paride !!!!");
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("اطلاعات شما موجود نیست ویا با دستگاه دیگری وارد شده اید");
#endif
                    PlayerPrefs.DeleteAll();
                    Scene scene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene("intro");
                }
                else
                {
                    CheckDataTaarefe(webRequest.downloadHandler.text);
                }

            }
        }


    }



    public void CheckDataTaarefe(string _url)
    {

        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<List<RootTaarefe>>(_url);

        //if(user[0].day>0)



        if (user[0].id == 1) //اولین بار که رجیستر میکنه ابجکت اول با قیمت صفر میاد و میگه رایگان داره
        {
            // PlayerPrefs.SetInt("firstTimeComingToApp", 1);
            PlayerPrefs.SetString("freeAccountDay", user[0].day);
            BuyButton[0].gameObject.SetActive(true);
            BuyButton[0].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = "آزمایشی " + user[0].day + " روزه ";


            if (PlayerPrefs.GetInt("store") == 1)
            {
                for (int i = 0; i < 3; i++)
                {


                    BuyButton[i + 1].gameObject.SetActive(true);
                    PlayerPrefs.SetString("buttonName" + i, user[i + 1].name);
                    PlayerPrefs.SetString("buttonPrice" + i, user[i + 1].price);
                    PlayerPrefs.SetString("StorePrice" + user[i].id, user[i].price);
                    String ss = user[i].id.ToString();

                    BuyButton[i + 1].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = user[i + 1].name + "\n" + user[i + 1].price + " تومان ";

                }
                if (buyButtonsConfing)
                {
                    buyButtonsConfing = false;
                    if (BuyButton[0].gameObject.activeSelf)
                        BuyButton[0].onClick.AddListener(() => BuyButtons((user[0].id).ToString()));
                    if (BuyButton[1].gameObject.activeSelf)
                        BuyButton[1].onClick.AddListener(() => BuyButtons((user[1].id).ToString()));
                    if (BuyButton[2].gameObject.activeSelf)
                        BuyButton[2].onClick.AddListener(() => BuyButtons((user[2].id).ToString()));
                    if (BuyButton[3].gameObject.activeSelf)
                        BuyButton[3].onClick.AddListener(() => BuyButtons((user[3].id).ToString()));
                }
            }
            else if (PlayerPrefs.GetInt("store") == 2)
            {
                if (BuyButton[0].gameObject.activeSelf)
                    BuyButton[0].onClick.AddListener(() => BuyButtons((user[0].id).ToString()));

                for (int i = 0; i < 3; i++)
                {


                    BuyButton[i + 1].gameObject.SetActive(true);
                    PlayerPrefs.SetString("buttonName" + i, user[i + 1].name);
                    PlayerPrefs.SetString("buttonPrice" + i, user[i + 1].price);
                    PlayerPrefs.SetString("StorePrice" + user[i].id, user[i].price);
                    String ss = user[i].id.ToString();

                    BuyButton[i + 1].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = user[i + 1].name + "\n" + user[i + 1].price + " تومان ";

                }
                if (BuyButton[1].gameObject.activeSelf)
                    BuyButton[1].onClick.AddListener(() => InAppStore.instance.purchaseProduct(0));
                if (BuyButton[2].gameObject.activeSelf)
                    BuyButton[2].onClick.AddListener(() => InAppStore.instance.purchaseProduct(1));
                if (BuyButton[3].gameObject.activeSelf)
                    BuyButton[3].onClick.AddListener(() => InAppStore.instance.purchaseProduct(2));
            }

            else if (PlayerPrefs.GetInt("store") == 3)
            {
                if (BuyButton[0].gameObject.activeSelf)
                    BuyButton[0].onClick.AddListener(() => BuyButtons((user[0].id).ToString()));
                for (int i = 0; i < 3; i++)
                {


                    BuyButton[i + 1].gameObject.SetActive(true);
                    PlayerPrefs.SetString("buttonName" + i, user[i + 1].name);
                    PlayerPrefs.SetString("buttonPrice" + i, user[i + 1].price);
                    PlayerPrefs.SetString("StorePrice" + user[i].id, user[i].price);
                    String ss = user[i].id.ToString();

                    BuyButton[i + 1].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = user[i + 1].name + "\n" + user[i + 1].price + " تومان ";

                }
                if (BuyButton[1].gameObject.activeSelf)
                    BuyButton[1].onClick.AddListener(() => InAppStore.instance.purchaseProduct(0));
                if (BuyButton[2].gameObject.activeSelf)
                    BuyButton[2].onClick.AddListener(() => InAppStore.instance.purchaseProduct(1));
                if (BuyButton[3].gameObject.activeSelf)
                    BuyButton[3].onClick.AddListener(() => InAppStore.instance.purchaseProduct(2));
            }


        }
        else // دفعات بعد توی لیستش فقط تعرفه ها رو برمیگردونه
        {


            if (PlayerPrefs.GetInt("store") == 1)
            {
                for (int i = 0; i < 3; i++)
                {


                    BuyButton[i + 1].gameObject.SetActive(true);
                    PlayerPrefs.SetString("buttonName" + i, user[i].name);
                    PlayerPrefs.SetString("buttonPrice" + i, user[i].price);
                    String ss = user[i].id.ToString();

                    BuyButton[i + 1].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = user[i].name + " " + user[i].price + " تومان ";

                }
                if (buyButtonsConfing)
                {
                    buyButtonsConfing = false;
                    if (BuyButton[1].gameObject.activeSelf)
                        BuyButton[1].onClick.AddListener(() => BuyButtons((user[0].id).ToString()));
                    if (BuyButton[2].gameObject.activeSelf)
                        BuyButton[2].onClick.AddListener(() => BuyButtons((user[1].id).ToString()));
                    if (BuyButton[3].gameObject.activeSelf)
                        BuyButton[3].onClick.AddListener(() => BuyButtons((user[2].id).ToString()));
                }
            }
            else if (PlayerPrefs.GetInt("store") == 2)
            {
                for (int i = 0; i < 3; i++)
                {


                    BuyButton[i + 1].gameObject.SetActive(true);
                    PlayerPrefs.SetString("buttonName" + i, user[i].name);
                    PlayerPrefs.SetString("buttonPrice" + i, user[i].price);
                    PlayerPrefs.SetString("StorePrice" + user[i].id, user[i].price);
                    String ss = user[i].id.ToString();

                    BuyButton[i + 1].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = user[i].name + " " + user[i].price + " تومان ";

                }

                if (BuyButton[1].gameObject.activeSelf)
                    BuyButton[1].onClick.AddListener(() => InAppStore.instance.purchaseProduct(0));
                if (BuyButton[2].gameObject.activeSelf)
                    BuyButton[2].onClick.AddListener(() => InAppStore.instance.purchaseProduct(1));
                if (BuyButton[3].gameObject.activeSelf)
                    BuyButton[3].onClick.AddListener(() => InAppStore.instance.purchaseProduct(2));
            }

            else if (PlayerPrefs.GetInt("store") == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    BuyButton[i + 1].gameObject.SetActive(true);
                    PlayerPrefs.SetString("buttonName" + i, user[i].name);
                    PlayerPrefs.SetString("buttonPrice" + i, user[i].price);
                    PlayerPrefs.SetString("StorePrice" + user[i].id, user[i].price);
                    String ss = user[i].id.ToString();

                    BuyButton[i + 1].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = user[i].name + " " + user[i].price + " تومان ";

                }

                if (BuyButton[1].gameObject.activeSelf)
                    BuyButton[1].onClick.AddListener(() => InAppStore.instance.purchaseProduct(0));
                if (BuyButton[2].gameObject.activeSelf)
                    BuyButton[2].onClick.AddListener(() => InAppStore.instance.purchaseProduct(1));
                if (BuyButton[3].gameObject.activeSelf)
                    BuyButton[3].onClick.AddListener(() => InAppStore.instance.purchaseProduct(2));
            }

        }



    }

    public string witchTaarefe;
    bool buyButtonsConfing = true;
    public void BuyButtons(string w)
    {

        witchTaarefe = w;
        Debug.Log("go to buy with taarefe : " + w);
        StartCoroutine(SendbuyRequest());
        if (w == "0")
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("start_Game", "start_Game_app", 1);
        }
    }

    [HideInInspector] public bool GoForPay = false;
    int counterFoucos = 0;
    IEnumerator SendbuyRequest()
    {


        WWWForm form = new WWWForm();


        form.AddField("discount_id", "1");
        form.AddField("tariff_id", witchTaarefe);
        form.AddField("discount_code", discont.text);
        using (UnityWebRequest webReq = UnityWebRequest.Post(PlayerPrefs.GetString("base_url") + "/create-bill", form))
        {

            webReq.SetRequestHeader("Authorization", "Bearer " + UnityEngine.PlayerPrefs.GetString("user_token"));
            webReq.SetRequestHeader("Accept", "application/json");
            webReq.chunkedTransfer = false;
            webReq.certificateHandler = new BypassCertificate();
            // Request and wait for the desired page.
            yield return webReq.SendWebRequest();

            // string[] pages = uri.Split('/');
            //  int page = pages.Length - 1;

            if (webReq.isNetworkError)
            {
                //  Debug.Log(pages[page] + ": Error: " + webRequest.error);
                if (GoForPay)
                {
                    GoForPay = false;

                    alarm.text = "عدم ارتباط با سرور خرید";
                    alarm.color = Color.red;


                }
                else
                {
                    InternetAvailabilityTest.instance.CheckConnection();
                }
            }
            else
            {
                Debug.Log(webReq.downloadHandler.text);
                if (witchTaarefe != "1")
                {
                    processJsonDataForBuy(webReq.downloadHandler.text);
                }

            }
        }


    }

    void processJsonDataForBuy(string _url)
    {
        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<RootCreateBill>(_url);
        // در اسکریپت اینترو منیجر 3 ثانیه منتظر میماند

        Debug.Log(user.interData[0].link);

        if (string.IsNullOrEmpty(user.interData[0].message))
        {
            Debug.Log("go To Pay");
            GoForPay = true;
            counterFoucos = 0;
            alarm.text = "انتقال به درگاه";
            alarm.color = Color.black;
            Application.OpenURL(user.interData[0].link);
        }
        else
        {
            alarm.text = "این اشتراک برای شما فعال است";
            alarm.color = Color.green;

        }


    }






    IEnumerator GetAccPay(string urlCheck)
    {
       // Debug.Log("token : " + UnityEngine.PlayerPrefs.GetString("user_token"));
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


                Debug.Log(pages[page] + ": Error: " + webRequest.error);
                if (GoForPay)
                {
                    GoForPay = false;

                    alarm.text = "عدم اتصال به درگاه";
                    alarm.color = Color.red;


                }
                else
                {
                    InternetAvailabilityTest.instance.CheckConnection();


                }
            }
            else
            {


                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                if (webRequest.downloadHandler.text.Contains("Unauthenticated"))
                {
                    PlayerPrefs.SetInt("with_Ad", 0);
                    PlayerPrefs.SetInt("MyDays", 0);
                    Debug.Log("user paride !!!!");
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("اطلاعات شما موجود نیست ویا با دستگاه دیگری وارد شده اید");
#endif
                    PlayerPrefs.DeleteAll();

                    SceneManager.LoadScene("intro");
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
    public void GoForLogin()
    {
        PlayerPrefs.SetInt("comeForAddData", 1);
        SceneManager.LoadScene("intro");
    }
    public void CheckPayData(string _url)
    {

        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<RootCheckPayment>(_url);


        if (GoForPay)
        {
            if (user.countday != null)
            {
                if (int.Parse(user.countday) > PlayerPrefs.GetInt("MyDays"))
                {
                    GoForPay = false;
                    PlayerPrefs.SetInt("with_Ad", 1);
                    if(AdiveryConfig.instance.BannerIsShowing)
                       AdiveryConfig.instance.OnBannerAdDestroy();
                    // خرید موفق
                    alarm.text = "خرید موفق";
                    alarm.color = Color.green;
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید موفقیت آمیز بود");

#endif
                    if(witchTaarefe=="0" | witchTaarefe == "3" & PlayerPrefs.GetInt("store") == 3 | PlayerPrefs.GetInt("store") == 2)
                    {
                        AdTraceEvent adtraceEvent = new AdTraceEvent("sftl7r");
                        AdTrace.trackEvent(adtraceEvent);
                    }

                    if (witchTaarefe == "1" | witchTaarefe == "4" & PlayerPrefs.GetInt("store") == 3 | PlayerPrefs.GetInt("store") == 2)
                    {
                        AdTraceEvent adtraceEvent = new AdTraceEvent("zu5cv0");
                        AdTrace.trackEvent(adtraceEvent);
                    }
                    if (witchTaarefe == "2" | witchTaarefe == "5" & PlayerPrefs.GetInt("store") == 3 | PlayerPrefs.GetInt("store") == 2)
                    {
                        AdTraceEvent adtraceEvent = new AdTraceEvent("3fjxv5");
                        AdTrace.trackEvent(adtraceEvent);
                    }

                    if (!PlayerPrefs.HasKey("phoneNumber"))
                    {
                        // go to init phone number
                        alarm.text = "خرید موفق" + "\n" + "برای پشتیبانی بهتر لطفا اطلاعات فرزند خود را وارد نمایید";
                        alarm.color = Color.black;
                        BuyButton[0].gameObject.SetActive(false);
                        BuyButton[1].gameObject.SetActive(false);
                        BuyButton[2].gameObject.SetActive(false);
                        BuyButton[6].gameObject.SetActive(false);
                        BuyButton[3].gameObject.SetActive(false);
                        BuyButton[5].gameObject.SetActive(false);
                        BuyButton[4].gameObject.SetActive(true);
                        
                        BuyButton[4].onClick.AddListener(() => AfterPaySucces());
                    }
                    else
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
                else
                {
                    alarm.text = "خرید ناموفق";
                    alarm.color = Color.red;
                    GoForPay = true;
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید انجام نشد");
#endif

                }
            }
            else
            {
                alarm.text = "خرید ناموفق";
                alarm.color = Color.red;
                GoForPay = true;
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید انجام نشد");
#endif
            }


        }
        else
        {
            if (user.countday == null)
            {
                alarm.text = "هنوز خریدی نداشته اید";
              
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

                    alarm.text = "اشتراک دارید";
                    alarm.color = Color.green;
                    BuyButton[0].gameObject.SetActive(true);
                    alarm.color = Color.blue;
                    alarm.text =
                     PlayerPrefs.GetInt("MyDays") + " روز از اشتراک شما " + "\n" + "باقی مانده است";

                   // PlayerPrefs.SetInt("loginSucces", 2);
                    PlayerPrefs.SetInt("with_Ad", 1);
                }
                else
                {
                    BuyButton[0].gameObject.SetActive(false);
                    alarm.text =
                     user.countday + " اشنراک شما پایان یافته است " + "\n" + "نسبت به تمدید آن اقدام نمایید";
                    PlayerPrefs.SetInt("loginSucces", 1);
                    PlayerPrefs.SetInt("with_Ad", 0);
                }
            }
        }






    }
    void AfterPaySucces()
    {
        PlayerPrefs.SetInt("comeForAddData", 1);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("btn_takmil_sabtnam", "btn_takmil_sabtnam_name", 1);
        SceneManager.LoadScene("intro");
      
    }
    public void setStatusMarket(string taarefeId)
    {
        PlayerPrefs.SetString("storeAdded", taarefeId);
        StartCoroutine(SetStatus(PlayerPrefs.GetString("base_url") + "/store-payment", taarefeId));

        GoForPay = false;

        // خرید موفق
        alarm.text = "خرید موفق";
        alarm.color = Color.green;
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید موفقیت آمیز بود");

#endif
        if (!PlayerPrefs.HasKey("phoneNumber"))
        {
            // go to init phone number
            alarm.text = "خرید موفق" + "\n" + "برای پشتیبانی بهتر لطفا اطلاعات فرزند خود را وارد نمایید";
            alarm.color = Color.black;
            BuyButton[0].gameObject.SetActive(false);
            BuyButton[1].gameObject.SetActive(false);
            BuyButton[6].gameObject.SetActive(false);
            BuyButton[2].gameObject.SetActive(false);
            BuyButton[3].gameObject.SetActive(false);
            BuyButton[5].gameObject.SetActive(false);
            BuyButton[4].gameObject.SetActive(true);
            PlayerPrefs.SetInt("comeForAddData", 1);
            BuyButton[4].onClick.AddListener(() => SceneManager.LoadScene("intro"));
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator SetStatus(string urlCheck, string TID)
    {

        WWWForm form = new WWWForm();
        form.AddField("last_price", PlayerPrefs.GetString("StorePrice" + TID));
        form.AddField("tariff_id", TID);
        form.AddField("discount_id", "0");
        form.AddField("transaction_id", UnityEngine.PlayerPrefs.GetString("token"));
        form.AddField("market_id", UnityEngine.PlayerPrefs.GetString("M_store"));


        using (var www = UnityWebRequest.Post(urlCheck, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + UnityEngine.PlayerPrefs.GetString("user_token"));
            www.SetRequestHeader("Accept", "application/json");
            www.chunkedTransfer = false;
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();



            if ((www.downloadHandler.text).Contains("Error"))
            {
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید کاربر ثبت نشد");
#endif
            }
            else if (System.String.IsNullOrEmpty(www.downloadHandler.text))
            {
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید کاربر ثبت نشد");
#endif

            }
            else
            {
                CheckPayMarket(www.downloadHandler.text);
            }

        }



    }

    void CheckPayMarket(string _url)
    {

        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<RootMarket>(_url);

        if (user.message == "success")
        {
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید کاربر با موفقیت ثبت شد");
#endif
            PlayerPrefs.DeleteKey("storeAdded");

        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید ناموفق");
#endif
        }

    }




    //////////////////////////////////////////////
    ///               kod takhfif
    //////////////////////////////////////////
    ///

    public void checkDiscountCode()
    {
        StartCoroutine(discountCodeCheck(PlayerPrefs.GetString("base_url") + "/check-discount-code"));
    }

    IEnumerator discountCodeCheck(string urlCheck)
    {
        Debug.Log("discont code : " + discont.text);
        WWWForm form = new WWWForm();

        form.AddField("discount_code", discont.text);


        using (var www = UnityWebRequest.Post(urlCheck, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + UnityEngine.PlayerPrefs.GetString("user_token"));
            www.SetRequestHeader("Accept", "application/json");
            www.chunkedTransfer = false;
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();



            if ((www.downloadHandler.text).Contains("Error"))
            {
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("عدم ارتباط با سرور");
#endif
            }
            else if (System.String.IsNullOrEmpty(www.downloadHandler.text))
            {
#if UNITY_ANDROID && !UNITY_EDITOR
    introManager.instance._ShowAndroidToastMessage("عدم ارتباط با سرور");
#endif

            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                CheckDiscountCode(www.downloadHandler.text);
            }

        }



    }


    void CheckDiscountCode(string _url)
    {

        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<RootDiscountCode>(_url);

        if (user.interData[0].status == 200)
        {
            AlarmDiscount.text = "کد ثبت شد" + "\n" + user.interData[0].discount + " درصد تخفیف دارید ";
            alarm.text = "کد ثبت شد" + "\n" + user.interData[0].discount + " درصد تخفیف دارید ";

#if UNITY_ANDROID && !UNITY_EDITOR
             introManager.instance._ShowAndroidToastMessage("کد ثبت شد" + "\n" + user.interData[0].discount + " درصد تخفیف دارید ");
#endif

            for (int i = 0; i < 3; i++)
            {


                //PlayerPrefs.GetString("buttonName" + i);
                //  PlayerPrefs.GetString("buttonPrice" + i);
                // PlayerPrefs.GetString("StorePrice" + i);
                Debug.Log(PlayerPrefs.GetString("StorePrice" + i));
                float pr = float.Parse(PlayerPrefs.GetString("buttonPrice" + i)) - ((float.Parse(PlayerPrefs.GetString("buttonPrice" + i))) * (float.Parse(user.interData[0].discount))) / 100f;

                BuyButton[i + 1].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = PlayerPrefs.GetString("buttonName" + i) + "\n" + pr.ToString() + " تومان ";

            }

            if (PlayerPrefs.GetInt("store") == 2 || PlayerPrefs.GetInt("store") == 3)
            {
                if (BuyButton[1].gameObject.activeSelf)
                {
                    BuyButton[1].onClick.RemoveAllListeners();
                    BuyButton[1].onClick.AddListener(() => InAppStore.instance.purchaseProduct(3));
                }

                if (BuyButton[2].gameObject.activeSelf)
                {
                    BuyButton[2].onClick.RemoveAllListeners();
                    BuyButton[2].onClick.AddListener(() => InAppStore.instance.purchaseProduct(4));

                }
                if (BuyButton[3].gameObject.activeSelf)
                {
                    BuyButton[3].onClick.RemoveAllListeners();
                    BuyButton[3].onClick.AddListener(() => InAppStore.instance.purchaseProduct(5));
                }
            }

            boxDis.SetActive(false);

        }
        else
        {
            AlarmDiscount.text = "کد اشتباه است";
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("کد اشتباه است");
#endif
        }

    }
}
