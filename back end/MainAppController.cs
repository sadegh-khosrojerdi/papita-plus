using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using System.Text;

using System.IO;
using Newtonsoft.Json;
using UPersian.Components;

public class MainAppController : MonoBehaviour
{
    // Start is called before the first frame update

    public static MainAppController instance;


    public enum Store
    {
        googleplay,
        Bazar,
        Mayket,
        AppStore,
        toop

    }
   
    public string BaseUrl;
    public Color mycolor;


    
    string orderId, serverdevise, whereFrom;
    string IPAddress;
    
    public GameObject  boxUpdate;
    [Space(20)]
    public Store mystore = new Store();
  
    private float mytimeofVideo;
    string m_serial;
    [Header("--- change buttons by change store ---")]
    public Button[] BuyButton;
    public GameObject discountCode;
    public Text version;
    //sadegh
    public bool clear_data;

    void Awake()
    {
        if (instance == null)
        {
          
            instance = this;
        }
     
       
    }
    void Start()
    {
        if (clear_data)
        {
            PlayerPrefs.DeleteAll();
        }
        introManager.instance.buttons[3].gameObject.SetActive(false);
        introManager.instance.buttons[0].gameObject.SetActive(true);
        PlayerPrefs.SetString("base_url", BaseUrl);
        version.text = "version : " + Application.version;
        
        if (PlayerPrefs.GetInt("needsUpdate")==1)
        {
            Debug.Log("your version needs to update !!!!!!!!!!!!!");
            
            boxUpdate.SetActive(true);
            boxUpdate.transform.GetChild(1).gameObject.GetComponent<RtlText>().text = PlayerPrefs.GetString("Force_Update_Message");
            //برای فورس اپدیت ورژنش رو چک میکنه
            StartCoroutine(GetRequestAtFirst3(PlayerPrefs.GetString("base_url") + "/settings"));
        }
        else
        {
            boxUpdate.SetActive(false);
            Debug.Log("your version is enable");

          
        }

       

        PlayerPrefs.DeleteKey("witchLang");
        PlayerPrefs.DeleteKey("scrollValueMain");
        startConfig();
        checkFirstButton();

    }
    public void checkFirstButton()
    {
      

        if (PlayerPrefs.GetInt("MyDays") > 0 & PlayerPrefs.GetInt("loginSucces") != 2)
        {
            introManager.instance.buttons[2].gameObject.SetActive(false);
            introManager.instance.buttons[3].gameObject.SetActive(true);
            introManager.instance.buttons[0].gameObject.SetActive(false);
        }
        else
        {
            if (PlayerPrefs.GetInt("loginSucces") == 2)
            {
                introManager.instance.buttons[2].gameObject.SetActive(false);
                introManager.instance.buttons[3].gameObject.SetActive(false);
                introManager.instance.buttons[0].gameObject.SetActive(true);
            }
            else
            {
                introManager.instance.buttons[2].gameObject.SetActive(true);
                introManager.instance.buttons[3].gameObject.SetActive(false);
                introManager.instance.buttons[0].gameObject.SetActive(true);
            }
        }
    }
    void startConfig()
    {
        if (PlayerPrefs.HasKey("storeAdded"))
        {
            StartCoroutine(SetStatus(PlayerPrefs.GetString("base_url") + "/store-payment", PlayerPrefs.GetString("storeAdded")));
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("در حال ثبت مجدد استور");
#endif
        }

        if (PlayerPrefs.HasKey("user_token"))
        {
            //خریدشو چک میکنه
            AfterSuccesLogin();
            Debug.Log("user token : " + PlayerPrefs.GetString("user_token"));
        }
        // InvokeRepeating("checkNet", 3, 5);

        // اینجا فقط بار اول اجرای بازی هستش
        if (UnityEngine.PlayerPrefs.GetInt("store") == 0 | !PlayerPrefs.HasKey("store"))
        {

           

#if UNITY_ANDROID
            string serial = SystemInfo.deviceUniqueIdentifier;


            if (string.IsNullOrEmpty(serial)) //اگه کاربر اجازه دسترسی به شناسش نده موقع نصب
            {
                this.orderId = Guid.NewGuid().ToString();
                UnityEngine.PlayerPrefs.SetString("orderId", orderId);
                UnityEngine.PlayerPrefs.Save();


                m_serial = orderId;
            }
            else
            {

                m_serial = "SN" + serial;
                UnityEngine.PlayerPrefs.SetString("m_sarial", m_serial);
            }



#elif UNITY_IOS

        string serial = Device.advertisingIdentifier;
             serverdevise = SystemInfo.deviceName + " ver :"+ Application.version;
          if (string.IsNullOrEmpty(serial))
         {
            serial = orderId;
            imeiii.text = "OR" + serial;
            m_serial = imeiii.text;
          }
          else{
             m_serial = "SN" + serial;
                UnityEngine.PlayerPrefs.SetString("m_sarial", m_serial);
          }

#else
           string serial = SystemInfo.deviceUniqueIdentifier;
        if (string.IsNullOrEmpty(serial))
        {
            serial = orderId;
            imeiii.text = "OR" + serial;
            m_serial = imeiii.text;
        }
        else
        {
             m_serial = "SN" + serial;
                UnityEngine.PlayerPrefs.SetString("m_sarial", m_serial);
        }


#endif
          
        }


        // اینجا هربار که بازی اجرا میشه
        if (mystore == Store.googleplay)
        {
            UnityEngine.PlayerPrefs.SetInt("store", 1);
            UnityEngine.PlayerPrefs.SetString("M_store", "1");
        }
        else if (mystore == Store.Bazar)
        {
            UnityEngine.PlayerPrefs.SetInt("store", 2);
            UnityEngine.PlayerPrefs.SetString("M_store", "2");
        }

        else if (mystore == Store.Mayket)
        {
            UnityEngine.PlayerPrefs.SetInt("store", 3);
            UnityEngine.PlayerPrefs.SetString("M_store", "3");
        }
        else if (mystore == Store.AppStore)
        {
            UnityEngine.PlayerPrefs.SetInt("store", 4);
            UnityEngine.PlayerPrefs.SetString("M_store", "4");
        }
        else if (mystore == Store.toop)
        {
            UnityEngine.PlayerPrefs.SetInt("store", 5);
            UnityEngine.PlayerPrefs.SetString("M_store", "5");
        }
         GetLocalIPAddress();

        m_serial = UnityEngine.PlayerPrefs.GetString("m_sarial");
        // m_sendPhoneNumberPath = AppDomain + PhoneUrlPath;
        serverdevise = SystemInfo.deviceName + " ver :" + Application.version;





        whereFrom = UnityEngine.PlayerPrefs.GetString("M_store");


        //ثبت دیتای یوزر
        if(!PlayerPrefs.HasKey("user_token"))
        SetData();
    }
    void checkNet()
    {

        InternetAvailabilityTest.instance.CheckConnection();
    }
    /*
    IEnumerator checkInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }
 */

    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {

                IPAddress = ip.ToString();
                UnityEngine.PlayerPrefs.SetString("ipaddress", IPAddress);
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {

            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                Application.Quit();
            }
        }
    }

    public void QuitGame()
    {

        Application.Quit();


    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////// set Data of user  ////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void SetData()
    {
        StartCoroutine(InitUserData());
    }

    IEnumerator InitUserData()
    {
        //  Level 1
        string market_id = UnityEngine.PlayerPrefs.GetString("M_store");
        string clientID = UnityEngine.PlayerPrefs.GetString("m_sarial");
        string _devise = SystemInfo.deviceName;
        string version = Application.version;



        WWWForm form = new WWWForm();
        form.AddField("client_id", clientID);
        form.AddField("device", _devise);
        form.AddField("currect_app_version", version);
        form.AddField("market_id", market_id);
        form.AddField("ip", PlayerPrefs.GetString("ipaddress"));

        using (var www = UnityWebRequest.Post(PlayerPrefs.GetString("base_url") + "/account/init", form))
        {
            www.SetRequestHeader("Accept", "application/json");
            www.chunkedTransfer = false;
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();


            Debug.Log("first data uploaded");
            Debug.Log(www.downloadHandler.text);
            CheckInitData(www.downloadHandler.text);
        }


    }

     void CheckInitData(string _url)
    {

        var user = JsonConvert.DeserializeObject<RootInit>(_url);

        Debug.Log("token init : " +  user.interData[0].token);
        
        UnityEngine.PlayerPrefs.SetString("user_token", user.interData[0].token);

        if (!string.IsNullOrEmpty(user.applicationsData.mobile))
        {
           
            UnityEngine.PlayerPrefs.SetString("phoneNumber", user.applicationsData.mobile);
            PlayerPrefs.SetInt("loginSucces", 2);
            checkFirstButton();
        }
        

        if (!string.IsNullOrEmpty(user.applicationsData.name))
            UnityEngine.PlayerPrefs.SetString("ChildrenName", user.applicationsData.name);

        if (!string.IsNullOrEmpty(user.applicationsData.gender))
        {
            if(user.applicationsData.gender== "male") PlayerPrefs.SetInt("gender", 1);
            else PlayerPrefs.SetInt("gender", 2);

        }
        if (!string.IsNullOrEmpty(user.applicationsData.age))
        {
            PlayerPrefs.SetInt("age", int.Parse(user.applicationsData.age));
        }

        if (!string.IsNullOrEmpty(user.applicationsData.name))
            UnityEngine.PlayerPrefs.SetString("ChildrenName", user.applicationsData.name);

        
          

        PlayerPrefs.SetString("user_InviteCode", user.applicationsData.invite_code);

        StartCoroutine(GetRequestAtFirst3(PlayerPrefs.GetString("base_url") + "/settings"));


        //خریدشو چک میکنه
        StartCoroutine(GetTaarifs(PlayerPrefs.GetString("base_url") + "/tariffs"));
        StartCoroutine(GetAccPay(PlayerPrefs.GetString("base_url") + "/account/tariffs"));

        if (user.applicationsData.mobile != null)
        {
            
            Debug.Log("login successful at init link");
            PlayerPrefs.SetInt("loginSucces", 1);
           
           
        }
        else
        {
            
            PlayerPrefs.DeleteKey("phoneNumber");
            PlayerPrefs.SetInt("gender", 1);
            introManager.instance.buttons[2].gameObject.SetActive(true);
            Debug.Log("first Comeing user");
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////   register   /////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    string gender;
    // public List<myQuestions> allQuestions = new List<myQuestions>();

    public void register()
    {

        StartCoroutine(rgisterUserData());
    }
    IEnumerator rgisterUserData()
    {
        //  Level 2


        string UserMobile = UnityEngine.PlayerPrefs.GetString("phoneNumber");




        string clientID = UnityEngine.PlayerPrefs.GetString("m_sarial");


        string name = PlayerPrefs.GetString("ChildrenName");

        if (PlayerPrefs.GetInt("gender") == 1)
        {
            gender = "male";
        }
        else
        {
            gender = "female";
        }
        string newpassword = UnityEngine.PlayerPrefs.GetString("password");

        string age = PlayerPrefs.GetInt("age").ToString();

        string favourite = "[" + PlayerPrefs.GetString("favourite") + "100]";

        WWWForm form = new WWWForm();

        form.AddField("name", name);
        form.AddField("mobile", UnityEngine.PlayerPrefs.GetString("phoneNumber"));
        form.AddField("client_id", UnityEngine.PlayerPrefs.GetString("m_sarial"));
        //form.AddField("questions", qusetion);
        form.AddField("age", age);
        form.AddField("favorites", favourite);
        form.AddField("gender", gender);
        form.AddField("password", newpassword);

        FirstRegister = true;
        using (var www = UnityWebRequest.Post(PlayerPrefs.GetString("base_url") + "/account/register", form))
        {
            www.SetRequestHeader("Accept", "application/json");
            www.chunkedTransfer = false;
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();



            Debug.Log("Form upload complete!");
            Debug.Log(www.downloadHandler.text);
            if ((www.downloadHandler.text).Contains("Error"))
            {
                GameObject.Find("Text status kod").GetComponent<Text>().text = "ﺪﺸﻧ ﻝﺎﺳﺭﺍ ﺪﮐ - ﺭﻭﺮﺳ ﺎﺑ ﻁﺎﺒﺗﺭﺍ ﻡﺪﻋ";
            }
            else if (System.String.IsNullOrEmpty(www.downloadHandler.text))
            {
                GameObject.Find("Text status kod").GetComponent<Text>().text = " ﺭﻭﺮﺳ ﺎﺑ ﻁﺎﺒﺗﺭﺍ ﻡﺪﻋ";

            }
            else
            {
                CheckMobileAtRegister(www.downloadHandler.text);
            }
        }





    }
    [HideInInspector] public bool waitForSmsRegister = false;
    public void CheckMobileAtRegister(string _url)
    {

        var user = JsonConvert.DeserializeObject<RootRegister>(_url);





        Debug.Log(user.message);
        string Message = user.message;
        int status = user.status;

        if (Message == "user created")
        {
            PlayerPrefs.SetInt("loginSucces", 2);
            // یوزر از قبل موجود نیست و ثبت شماره موفق بوده پس پیامک رو بفرسته
            // Debug.Log("sms sent");

            //   waitForSmsRegister = true;
            //  introManager.instance.buttons[41].gameObject.SetActive(false);
            // Timer.instanseTimer.timeText = GameObject.Find("timerKod").GetComponent<Text>();
            // Timer.instanseTimer.timeRemaining = 120;
            // Timer.instanseTimer.timerIsRunning = true;
            PlayerPrefs.SetString("user_InviteCode", user.user.invite_code);
          ///  GameObject.Find("Text status kod").GetComponent<Text>().text = "ﻩﺭﺎﻤﺷ ﻪﺑ ﻩﺪﺷ ﻝﺎﺳﺭﺍ ﺪﯿﯾﺎﺗ ﺪﮐ" + "\n"
          //      + PlayerPrefs.GetString("phoneNumber") + "\n" + "ﺪﯿﯾﺎﻤﻧ ﺩﺭﺍﻭ ﺍﺭ";
           // GameObject.Find("Text status kod").GetComponent<Text>().color = Color.black;
#if UNITY_ANDROID && !UNITY_EDITOR
  
              //  introManager.instance._ShowAndroidToastMessage("کد ورود برای شما پیامک شد");
               introManager.instance._ShowAndroidToastMessage("حساب کاربری با موفقیت ساخته شد");
#endif
            StartCoroutine(sendAnswers());
            // Firebase.Analytics.FirebaseAnalytics.LogEvent("sms_sent", "sms_sent_app", 1);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("register_succesfull", "register_succesfull_app", 1);
        }
        else
        {
            introManager.instance.buttons[41].gameObject.SetActive(true);
            if (GameObject.Find("Text status kod"))
            {
                GameObject.Find("Text status kod").GetComponent<Text>().text = "ﺖﺳﺍ ﻩﺪﺷ ﺖﺒﺛ ﻼﺒﻗ ﺱﺎﻤﺗ ﻩﺭﺎﻤﺷ ﻦﯾﺍ" + "\n" +
                "ﺪﯿﻨﮐ ﺩﺭﺍﻭ ﯼﺮﮕﯾﺩ ﺱﺎﻤﺗ ﻩﺭﺎﻤﺷ ﻞﺒﻗ ﯼﻮﻨﻣ ﺯﺍ" + "\n" + "ﺪﯾﻮﺷ ﺩﺭﺍﻭ ﺩﻮﺧ ﺰﻣﺭ ﺎﺑ ﺩﻭﺭﻭ ﯼﻮﻨﻣ ﺯﺍ ﺎﯾﻭ";
                GameObject.Find("Text status kod").GetComponent<Text>().color = Color.red;
            }
            Invoke("comebackToLogin", 4);
#if UNITY_ANDROID && !UNITY_EDITOR
  
                introManager.instance._ShowAndroidToastMessage("این شماره تماس قبلا ثبت شده است");
#endif
        }




    }
    //اگه شماره تماس تکراری زده بود
    void comebackToLogin()
    {

        introManager.instance.DoChanges(16, 20, 100);
    }
    IEnumerator sendAnswers()
    {
        //sent question to server level 3

        for (int i = 0; i < 8; i++)
        {

            WWWForm formQ = new WWWForm();

            formQ.AddField("client_id", UnityEngine.PlayerPrefs.GetString("m_sarial"));
            formQ.AddField("q_id", (i + 1).ToString());
            formQ.AddField("q_answer", PlayerPrefs.GetInt("question" + (i + 1)).ToString());



            using (var wwwQ = UnityWebRequest.Post(PlayerPrefs.GetString("base_url") + "/account/add-question", formQ))
            {
                wwwQ.chunkedTransfer = false;
                wwwQ.certificateHandler = new BypassCertificate();
                yield return wwwQ.SendWebRequest();

                if (wwwQ.isNetworkError || wwwQ.isHttpError)
                {
                    Debug.Log(wwwQ.error);
                }
                else
                {


                    Debug.Log(wwwQ.downloadHandler.text);
                }
            }



        }
    }


    // توی این سشن کاربر تا اینجا اطلاعات شخصی و شماره تماسشو فرستاده منتظر تایید با کد و مشخص کردن رمزه

    public void verifyUserFirstTime()
    {

        StartCoroutine(verifyUserForFirstTime());
    }
    IEnumerator verifyUserForFirstTime()
    {

        //  Level 4
        string UserMobile = UnityEngine.PlayerPrefs.GetString("phoneNumber");
        string kodTaeed = UnityEngine.PlayerPrefs.GetString("KodTaeed");
        string newpassword = UnityEngine.PlayerPrefs.GetString("password");


        WWWForm form = new WWWForm();
        form.AddField("code", kodTaeed);
        form.AddField("mobile", UserMobile);
        form.AddField("password", newpassword);


        using (var www = UnityWebRequest.Post(PlayerPrefs.GetString("base_url") + "/account/verify-code", form))
        {
            www.SetRequestHeader("Accept", "application/json");
            www.chunkedTransfer = false;
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();


            Debug.Log("Form upload complete!");
            Debug.Log(www.downloadHandler.text);
            CheckForChangePasstPass(www.downloadHandler.text);
        }


    }
    bool FirstRegister = false;
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////  Login With password ///////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void AfterSuccesLogin()
    {
        //خریدشو چک میکنه

        StartCoroutine(GetTaarifs(PlayerPrefs.GetString("base_url") + "/tariffs"));
        StartCoroutine(GetAccPay(PlayerPrefs.GetString("base_url") + "/account/tariffs"));
        //برای فورس اپدیت ورژنش رو چک میکنه
        StartCoroutine(GetRequestAtFirst3(PlayerPrefs.GetString("base_url") + "/settings"));
    }
    public void Login()
    {

        StartCoroutine(LoginUser());
    }
    IEnumerator LoginUser()
    {
        // level 5
        string UserMobile = UnityEngine.PlayerPrefs.GetString("phoneNumber");
        string UserPassword = UnityEngine.PlayerPrefs.GetString("password");

        if (string.IsNullOrEmpty(UserMobile))
        {
            Alarm.GetComponent<Text>().text = "ﺖﺳﺍ ﻩﺪﺸﻧ ﺩﺭﺍﻭ ﺱﺎﻤﺗ ﻩﺭﺎﻤﺷ";
        }
        else if (string.IsNullOrEmpty(UserPassword))
        {
            Alarm.GetComponent<Text>().text = "ﺪﯿﯾﺎﻤﻧ ﺩﺭﺍﻭ ﺍﺭ ﺭﻮﺒﻋ ﺰﻣﺭ";
        }
        else
        {
            WWWForm form = new WWWForm();


            form.AddField("mobile", UserMobile);
            form.AddField("password", UserPassword);


            FirstRegister = true;
            using (var www = UnityWebRequest.Post(PlayerPrefs.GetString("base_url") + "/account/login", form))
            {

                www.chunkedTransfer = false;
                www.certificateHandler = new BypassCertificate();

                yield return www.SendWebRequest();

                Debug.Log("Form upload complete!");
                Debug.Log(www.downloadHandler.text);
                GameObject Alarm = GameObject.Find("Text Login");
                if (System.String.IsNullOrEmpty(www.downloadHandler.text) || (www.downloadHandler.text).Contains("Error:"))
                {
                    if (Alarm != null)
                    {
                        Alarm.GetComponent<Text>().text = " ...ﺭﻭﺮﺳ ﺎﺑ ﻁﺎﺒﺗﺭﺍ ﻡﺪﻋ";
                        Alarm.GetComponent<Text>().color = Color.red;
                        if (GameObject.Find("meti"))
                        {
                            GameObject.Find("meti").GetComponent<Text>().text = "not connect server" + www.error;
                            GameObject.Find("meti").GetComponent<Text>().color = Color.red;
                        }

                    }
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Login_btn_faild_serverNotResponse", "Login_btn_faild_serverNotResponse_app", 1);
                  
                }
                else if ((www.downloadHandler.text).Contains("<!doctype html>"))
                {

                    Alarm.GetComponent<Text>().text = "ﺖﺴﯿﻧ ﺖﺳﺭﺩ ﺱﺎﻤﺗ ﻩﺭﺎﻤﺷ ﺖﻣﺮﻓ";
                }
                else
                {


                    if (Alarm != null)
                    {
                        Alarm.GetComponent<Text>().text = "... ﯽﺳﺭﺮﺑ ﻝﺎﺣ ﺭﺩ";
                        Alarm.GetComponent<Text>().color = Color.green;
                        if (GameObject.Find("meti"))
                        {
                            GameObject.Find("meti").GetComponent<Text>().text = www.downloadHandler.text;
                            GameObject.Find("meti").GetComponent<Text>().color = Color.green;
                        }

                    }

                    CheckPasswordLogin(www.downloadHandler.text);
                }
            }
        }
    


    }
    public class LoginUserData
    {
        public string mobile;
        public string password;

    }

    public void CheckPasswordLogin(string _url)
    {


        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<RootLogin>(_url);
        // در اسکریپت اینترو منیجر 3 ثانیه منتظر میماند

        Debug.Log(user.message);
        string Message = user.message;
        string Token = user.token;
        int status = user.status;

        if (status == 200)
        {
            // login succes
            // اینجا موفق وارد شده توکن رو از سرور میگیره
            Debug.Log("login successful");
            PlayerPrefs.SetInt("loginSucces", 2);
            UnityEngine.PlayerPrefs.SetString("user_token", Token);
            //خریدشو چک میکنه
            StartCoroutine(GetTaarifs(PlayerPrefs.GetString("base_url") + "/tariffs"));
            StartCoroutine(GetAccPay(PlayerPrefs.GetString("base_url") + "/account/tariffs"));

            Debug.Log("his name : " + user.user.name + " his age : " + user.user.age + " gender : " + user.user.gender);

            PlayerPrefs.SetInt("age", int.Parse(user.user.age));
            if (user.user.gender == "male")
            {
                PlayerPrefs.SetInt("gender", 1);
            }
            else
            {
                PlayerPrefs.SetInt("gender", 2);
            }

            PlayerPrefs.SetString("ChildrenName", user.user.name);

            try
            {
                string m_city = user.user.score.city;
                Debug.Log("sadegh  ++++++++++++++" + m_city);
                PlayerPrefs.SetString("city", user.user.score.city);
            }
            catch
            {
                Debug.Log("dont have city");
            }

        


#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("با موفقیت وارد شدید");
#endif
        }
        else if (status == 404)
        {
            // اینجا بهش میگه که نام کاربری یا رمز عبور اشتباه است
            PlayerPrefs.SetInt("loginSucces", 1);
            Debug.Log("wrong password");
            GameObject Alarm = GameObject.Find("Text Login");
            if (Alarm != null)
            {
                Alarm.GetComponent<Text>().text = "ﺖﺳﺍ ﻩﺎﺒﺘﺷﺍ ﺭﻮﺒﻋ ﺰﻣﺭ ﺎﯾ ﯼﺮﺑﺭﺎﮐ ﻡﺎﻧ";
                Alarm.GetComponent<Text>().color = Color.red;
            }
            PlayerPrefs.DeleteKey("user_token");
            Firebase.Analytics.FirebaseAnalytics.LogEvent("Login_btn_faild_inCorrectPass", "Login_btn_faild_inCorrectPass_app", 1);

        }
        else
        {
            // کاربری با این یوزر پیدا نشد

            PlayerPrefs.SetInt("loginSucces", 1);

            GameObject Alarm = GameObject.Find("Text Login");
            if (Alarm != null)
            {
                Alarm.GetComponent<Text>().text = "!! ﺪﺸﻧ ﺍﺪﯿﭘ ﺮﺑﺭﺎﮐ";
                Alarm.GetComponent<Text>().color = Color.red;
            }
            PlayerPrefs.DeleteKey("user_token");
            Firebase.Analytics.FirebaseAnalytics.LogEvent("Login_btn_faild_userNotFound", "Login_btn_faild_userNotFound_app", 1);

        }


    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////  Reset password ///////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SendResetPassword()
    {
        // level 6
        PlayerPrefs.SetString("phoneNumber", introManager.instance.levels[21].transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text);
        GameObject.Find("text mobile Num").GetComponent<Text>().text = UnityEngine.PlayerPrefs.GetString("phoneNumber");
        introManager.instance.buttons[49].gameObject.SetActive(false);
        Timer.instanseTimer.timeText = GameObject.Find("timerKod").GetComponent<Text>();
        Timer.instanseTimer.timeRemaining = 120;
        Timer.instanseTimer.timerIsRunning = true;
        StartCoroutine(SendResetPass());
    }



    IEnumerator SendResetPass()
    {

        string UserMobile = UnityEngine.PlayerPrefs.GetString("phoneNumber");



        WWWForm form = new WWWForm();


        form.AddField("mobile", UserMobile);



        FirstRegister = true;
        using (var www = UnityWebRequest.Post(PlayerPrefs.GetString("base_url") + "/account/send-verify-code", form))
        {
            www.SetRequestHeader("Accept", "application/json");
            www.chunkedTransfer = false;
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();

            Debug.Log("number sent for reset Pass");
            Debug.Log(www.downloadHandler.text);


            if ((www.downloadHandler.text).Contains("Error:"))
            {
                introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().text = " ﺭﻭﺮﺳ ﺎﺑ ﻁﺎﺒﺗﺭﺍ ﻡﺪﻋ";
                introManager.instance.levels[22].transform.GetChild(4).gameObject.GetComponent<Text>().text = " ";
                introManager.instance.levels[22].transform.GetChild(1).gameObject.SetActive(true);
            }
            else if (System.String.IsNullOrEmpty(www.downloadHandler.text))
            {
                introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().text = "ﺖﺴﯿﻧ ﺩﻮﺟﻮﻣ ﺱﺎﻤﺗ ﻩﺭﺎﻤﺷ" + "\n" + "ﺪﯿﻨﮐ ﻡﺎﻧ ﺖﺒﺛ ﺍﺪﺘﺑﺍ ﺎﻔﻄﻟ";
                introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().color = Color.red;
                introManager.instance.levels[22].transform.GetChild(4).gameObject.GetComponent<Text>().text = " ";
                introManager.instance.levels[22].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {

                introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().text = "... ﯽﺳﺭﺮﺑ ﻝﺎﺣ ﺭﺩ";
                introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().color = Color.green;

                CheckMobileThatSendForResetPass(www.downloadHandler.text);
            }
        }


    }

    public void CheckMobileThatSendForResetPass(string _url)
    {

        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<RootResendVerifyCode>(_url);
        // در اسکریپت اینترو منیجر 3 ثانیه منتظر میماند

        Debug.Log(user.message);
        string Message = user.message;

        int status = user.status;

        if (status == 201)
        {
            // 

            Debug.Log("sms rest send");
            introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().text = "ﺪﺷ ﻝﺎﺳﺭﺍ ﺪﯿﯾﺎﺗ ﺪﮐ";
            introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().color = Color.green;
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("کد تایید ارسال شد");
#endif


        }
        else if (status == 200)
        {
            // 
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("پس از دو دقیقه مجددا تلاش نمایید");
#endif
            Debug.Log("wait 2 minuts to send again");
            introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().text = "ﺪﯿﯾﺎﻤﻧ ﺵﻼﺗ ﻪﻘﯿﻗﺩ 2 ﺯﺍ ﺲﭘ ﺎﻔﻄﻟ";
            introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().color = Color.red;
            introManager.instance.levels[22].transform.GetChild(4).gameObject.GetComponent<Text>().text = " ";
            introManager.instance.levels[22].transform.GetChild(1).gameObject.SetActive(true);

        }
        else
        {
            // کاربری با این یوزر پیدا نشد
            Debug.Log("user dont exist");
            introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().text = "ﺖﺴﯿﻧ ﺩﻮﺟﻮﻣ ﺱﺎﻤﺗ ﻩﺭﺎﻤﺷ" + "\n" + "ﺪﯿﻨﮐ ﻡﺎﻧ ﺖﺒﺛ ﺍﺪﺘﺑﺍ ﺎﻔﻄﻟ";
            introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().color = Color.red;
            introManager.instance.levels[22].transform.GetChild(4).gameObject.GetComponent<Text>().text = " ";
            introManager.instance.levels[22].transform.GetChild(1).gameObject.SetActive(true);
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("کاربری با این شماره تماس پیدا نشد");
#endif


        }


    }

    public void ResetPassword()
    {

        StartCoroutine(ResetPass());
    }

    IEnumerator ResetPass()
    {
        // level 7
        string UserMobile = UnityEngine.PlayerPrefs.GetString("phoneNumber");
        string kodTaeed = UnityEngine.PlayerPrefs.GetString("KodTaeed");
        string newpassword = UnityEngine.PlayerPrefs.GetString("password");

        WWWForm form = new WWWForm();


        form.AddField("mobile", UserMobile);
        form.AddField("password", newpassword);
        form.AddField("code", kodTaeed);

        FirstRegister = true;
        using (var www = UnityWebRequest.Post(PlayerPrefs.GetString("base_url") + "/account/reset-password", form))
        {
            www.SetRequestHeader("Accept", "application/json");
            www.chunkedTransfer = false;
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();

            Debug.Log("Form upload complete!");
            Debug.Log(www.downloadHandler.text);

            if ((www.downloadHandler.text).Contains("Error:"))
            {
                introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().text = " ﺭﻭﺮﺳ ﺎﺑ ﻁﺎﺒﺗﺭﺍ ﻡﺪﻋ";
                introManager.instance.levels[22].transform.GetChild(4).gameObject.GetComponent<Text>().text = " ";
                introManager.instance.levels[22].transform.GetChild(1).gameObject.SetActive(true);
            }
            else if (System.String.IsNullOrEmpty(www.downloadHandler.text))
            {

                introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().text = "ﺖﺴﯿﻧ ﺩﻮﺟﻮﻣ ﺱﺎﻤﺗ ﻩﺭﺎﻤﺷ" + "\n" + "ﺪﯿﻨﮐ ﻡﺎﻧ ﺖﺒﺛ ﺍﺪﺘﺑﺍ ﺎﻔﻄﻟ";
                introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().color = Color.red;
                introManager.instance.levels[22].transform.GetChild(4).gameObject.GetComponent<Text>().text = " ";
                introManager.instance.levels[22].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {


                introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().text = "... ﯽﺳﺭﺮﺑ ﻝﺎﺣ ﺭﺩ";
                introManager.instance.levels[22].transform.GetChild(3).gameObject.GetComponent<Text>().color = Color.green;
                CheckPasswordchange(www.downloadHandler.text);
            }
        }



    }

    public void CheckPasswordchange(string _url)
    {


        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<RootCheckPasswordChange>(_url);





        Debug.Log(user.message);
        string Message = user.message;
        int status = user.status;
        string token = user.token;



        if (status == 200)
        {
            GameObject.Find("Text forget password").GetComponent<Text>().text = "... ﺪﯿﻨﮐﺮﺒﺻ ﺎﻔﻄﻟ ﺰﯿﻣﺁ ﺖﯿﻘﻓﻮﻣ";

            PlayerPrefs.SetInt("passwordChanged", 1);
            // PlayerPrefs.SetString("user_token", token);
            introManager.instance.levels[22].transform.GetChild(4).GetComponent<Text>().text = " ";
            //تغییر رمز موفق دوباره به صفحه ورود میفرستیمش



#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("تغییر رمز موفقیت آمیز بود");
#endif

        }

        else
        {
            // کد اشتباه است یا زمان ان تمام شده است
            Debug.Log("kod expire or kod wrong !!!");
            GameObject.Find("Text forget password").GetComponent<Text>().text = "  " + "\n" + "ﺖﺳﺍ ﻩﺎﺒﺘﺷﺍ ﺪﯿﯾﺎﺗ ﺪﮐ";
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("کد اشتباه است");
#endif
            PlayerPrefs.SetInt("loginSucces", 1);
            PlayerPrefs.SetInt("passwordChanged", 0);
            PlayerPrefs.DeleteKey("user_token");
        }

    }

    public void CheckForChangePasstPass(string _url)
    {


        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<Root>(_url);





        Debug.Log(user.message);
        string Message = user.message; // to string ekhtiyari
        string Token = user.token;




        if (Message == "success")
        {
            GameObject.Find("Text status kod").GetComponent<Text>().text = "... ﺪﯿﻨﮐﺮﺒﺻ ﺎﻔﻄﻟ ﺰﯿﻣﺁ ﺖﯿﻘﻓﻮﻣ";
            UnityEngine.PlayerPrefs.SetString("user_token", Token);

            //خریدشو چک میکنه
            StartCoroutine(GetTaarifs(PlayerPrefs.GetString("base_url") + "/tariffs"));
            StartCoroutine(GetAccPay(PlayerPrefs.GetString("base_url") + "/account/tariffs"));

            PlayerPrefs.SetInt("loginSucces", 2);
            Debug.Log("token :" + Token);
            //ثبت نام اولیه با موفقیت انجام شد
            FirstRegister = false;
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("ثبت نام با موفقیت انجام شد");
#endif
            Debug.Log("register succesfull :)");

            Firebase.Analytics.FirebaseAnalytics.LogEvent("register_succesfull", "register_succesfull_app", 1);

            if (FirstRegister)
            {


            }
            else
            {
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("رمز عبور باموفقیت تغییر کرد");
#endif
                Debug.Log("password Changeed !!!!");
                // پسوورد با موفقیت تغییر پیدا کرد
            }
        }



        else
        {
            // کد اشتباه است یا زمان ان تمام شده است
            Debug.Log("kod expire or kod wrong !!!");
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("کد اشتباه است یا زمان آن تمام شده");
#endif
            PlayerPrefs.SetInt("loginSucces", 1);
            PlayerPrefs.DeleteKey("user_token");
            GameObject.Find("Text status kod").GetComponent<Text>().text = " ﺖﺳﺍ ﻩﺎﺒﺘﺷﺍ ﺪﮐ";
        }


        /////////////////////////////////////////////// taarefe ha /////////////////////////////////////////////////



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
                    PlayerPrefs.SetInt("with_Ad", 0);
                    PlayerPrefs.SetInt("MyDays", 0);
                    Debug.Log("user paride !!!!");
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("اطلاعات شما موجود نیست ویا با دستگاه دیگری وارد شده اید");
#endif
                    PlayerPrefs.DeleteAll();
                    Scene scene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(scene.name);
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


            if (mystore == Store.googleplay)
            {
                for (int i = 0; i < user.Count; i++)
                {

                    if (i > 0)
                    {
                        PlayerPrefs.SetString("buttonName" + i, user[i].name);
                        PlayerPrefs.SetString("buttonPrice" + i, user[i].price);
                        String ss = user[i].id.ToString();
                        BuyButton[i].gameObject.SetActive(true);
                        BuyButton[i].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = user[i].name + " " + user[i].price + " تومان ";

                    }

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
            else if (mystore == Store.Bazar)
            {
                if (BuyButton[0].gameObject.activeSelf)
                    BuyButton[0].onClick.AddListener(() => BuyButtons((user[0].id).ToString()));

                for (int i = 0; i < 3; i++)
                {

                    
                    BuyButton[i + 1].gameObject.SetActive(true);
                    PlayerPrefs.SetString("buttonName" + i, user[i+1].name);
                    PlayerPrefs.SetString("buttonPrice" + i, user[i+1].price);
                    PlayerPrefs.SetString("StorePrice" + user[i].id, user[i].price);
                    String ss = user[i].id.ToString();

                    BuyButton[i + 1].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = user[i+1].name + " " + user[i+1].price + " تومان ";

                }
                if (BuyButton[1].gameObject.activeSelf)
                    BuyButton[1].onClick.AddListener(() => InAppStore.instance.purchaseProduct(0));
                if (BuyButton[2].gameObject.activeSelf)
                    BuyButton[2].onClick.AddListener(() => InAppStore.instance.purchaseProduct(1));
                if (BuyButton[3].gameObject.activeSelf)
                    BuyButton[3].onClick.AddListener(() => InAppStore.instance.purchaseProduct(2));
            }

            else if (mystore == Store.Mayket)
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

                    BuyButton[i + 1].gameObject.transform.GetChild(0).GetComponent<RtlText>().text = user[i + 1].name + " " + user[i + 1].price + " تومان ";

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
            if (PlayerPrefs.GetInt("MyDays") > 0)
            {

                introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "اشتراک دارید";
                introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().color = Color.green;
                BuyButton[0].gameObject.SetActive(true);
                introManager.instance.levels[19].transform.GetChild(1).gameObject.GetComponent<RtlText>().text =
                 PlayerPrefs.GetInt("MyDays") + " روز از اشتراک شما " + "\n" + "باقی مانده است";
                BuyButton[0].gameObject.transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "شروع بازی";
               // PlayerPrefs.SetInt("loginSucces", 2);
                PlayerPrefs.SetInt("with_Ad", 1);
            }
            else
            {
                PlayerPrefs.SetInt("with_Ad", 0);
            }
            if (PlayerPrefs.GetInt("MyDays") > 0)
            {
                BuyButton[0].gameObject.SetActive(true);
            }

            if (mystore == Store.googleplay)
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
            else if (mystore == Store.Bazar)
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

            else if (mystore == Store.Mayket)
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
    public GameObject Alarm;
    [HideInInspector] public bool GoForPay = false;
    int counterFoucos = 0;
    IEnumerator SendbuyRequest()
    {


        WWWForm form = new WWWForm();


        form.AddField("discount_id", "1");
        form.AddField("tariff_id", witchTaarefe);
        form.AddField("discount_code", discountCode.transform.GetChild(2).gameObject.GetComponent<Text>().text);
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

                    introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "عدم ارتباط با سرور خرید";
                    introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().color = Color.red;


                }
                else
                {
                    InternetAvailabilityTest.instance.CheckConnection();
                }
            }
            else
            {
                Debug.Log(webReq.downloadHandler.text);
                if(witchTaarefe != "1")
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
            introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "انتقال به درگاه";
            introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().color = Color.black;
            Application.OpenURL(user.interData[0].link);
        }
        else
        {
            introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "این اشتراک برای شما فعال است";
            introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().color = Color.green;

        }


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
            StartCoroutine(GetTaarifs(PlayerPrefs.GetString("base_url") + "/tariffs"));
            StartCoroutine(GetAccPay(PlayerPrefs.GetString("base_url") + "/account/tariffs"));

        }


        checkNet();

    }
    public void Reload()
    {
        introManager.instance.levels[19].transform.GetChild(2).gameObject.SetActive(false);
        if (PlayerPrefs.HasKey("user_token"))
        {

            //خریدشو چک میکنه
            StartCoroutine(GetTaarifs(PlayerPrefs.GetString("base_url") + "/tariffs"));
            StartCoroutine(GetAccPay(PlayerPrefs.GetString("base_url") + "/account/tariffs"));

        }
        else
        {
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }

    }
    IEnumerator GetAccPay(string urlCheck)
    {

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
                introManager.instance.levels[19].transform.GetChild(2).gameObject.SetActive(true);
                discountCode.SetActive(false);
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
                if (GoForPay)
                {
                    GoForPay = false;

                    introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "عدم اتصال به درگاه";
                    introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().color = Color.red;


                }
                else
                {
                    InternetAvailabilityTest.instance.CheckConnection();


                }
            }
            else
            {
                introManager.instance.levels[19].transform.GetChild(2).gameObject.SetActive(false);
                discountCode.SetActive(true);
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                if (webRequest.downloadHandler.text.Contains("Unauthenticated"))
                {
                    PlayerPrefs.SetInt("with_Ad", 0);
                    PlayerPrefs.SetInt("MyDays", 0);
                    PlayerPrefs.DeleteAll();
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

                        for (int i = 0; i < introManager.instance.levels.Length; i++)
                        {
                            introManager.instance.levels[i].SetActive(false);
                        }
                        introManager.instance.levels[19].SetActive(true);
                        BuyButton[0].gameObject.SetActive(false);
                        introManager.instance.levels[19].transform.GetChild(1).gameObject.GetComponent<RtlText>().text = " اشنراک شما پایان یافته است " + "\n" + "نسبت به تمدید آن اقدام نمایید";

                        StartCoroutine(GetTaarifs(PlayerPrefs.GetString("base_url") + "/tariffs"));

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


        if (GoForPay)
        {
            if (user.countday != null)
            {
                if (int.Parse(user.countday) > PlayerPrefs.GetInt("MyDays"))
                {
                    GoForPay = false;
                    PlayerPrefs.SetInt("with_Ad", 1);
                    // خرید موفق
                    introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "خرید موفق";
                    introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().color = Color.green;
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید موفقیت آمیز بود");

#endif
                }
                else
                {
                    introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "خرید ناموفق";
                    introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().color = Color.red;
                    GoForPay = true;
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید انجام نشد");
#endif

                }
            }
            else
            {
                introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "خرید ناموفق";
                introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().color = Color.red;
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
                introManager.instance.levels[19].transform.GetChild(1).gameObject.GetComponent<RtlText>().text = "هنوز خریدی نداشته اید";
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

                    introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "اشتراک دارید";
                    introManager.instance.levels[19].transform.GetChild(0).gameObject.GetComponent<RtlText>().color = Color.green;
                    BuyButton[0].gameObject.SetActive(true);
                    introManager.instance.levels[19].transform.GetChild(1).gameObject.GetComponent<RtlText>().text =
                     PlayerPrefs.GetInt("MyDays") + " روز از اشتراک شما " + "\n" + "باقی مانده است";
                    BuyButton[0].gameObject.transform.GetChild(0).gameObject.GetComponent<RtlText>().text = "شروع بازی";
                   // PlayerPrefs.SetInt("loginSucces", 2);
                    PlayerPrefs.SetInt("with_Ad", 1);
                }
                else
                {
                    BuyButton[0].gameObject.SetActive(false);
                    introManager.instance.levels[19].transform.GetChild(1).gameObject.GetComponent<RtlText>().text =
                     user.countday + " اشنراک شما پایان یافته است " + "\n" + "نسبت به تمدید آن اقدام نمایید";
                    
                    PlayerPrefs.SetInt("with_Ad", 0);
                }
            }
        }

      
     



    }
    /// <summary>
    /// ///////////////////////////////////////////        Market Status                  ////////////////////////////////////
    /// </summary>
    public void PayFaildMarket()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید ناموفق بود");
#endif
    }
    public void setStatusMarket(string taarefeId)
    {
        PlayerPrefs.SetString("storeAdded", taarefeId);
        StartCoroutine(SetStatus(PlayerPrefs.GetString("base_url") + "/store-payment", taarefeId));
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

        if(user.message== "success")
        {
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید کاربر با موفقیت ثبت شد");
#endif
            PlayerPrefs.DeleteKey("storeAdded");
            AfterSuccesLogin();
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("خرید ناموفق");
#endif
        }

    }
    /// <summary>
    /// ////////////////////////////////////////      Discount Code                  ///////////////////////////////////////
    /// </summary>
    
    public void checkDiscountCode()
    {
        StartCoroutine(discountCodeCheck(PlayerPrefs.GetString("base_url") + "/check-discount-code"));
    }

    IEnumerator discountCodeCheck(string urlCheck)
    {
        Debug.Log("discont code : " + discountCode.transform.GetChild(2).gameObject.GetComponent<Text>().text);
        WWWForm form = new WWWForm();
     
        form.AddField("discount_code", discountCode.transform.GetChild(2).gameObject.GetComponent<Text>().text);
    

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

#if UNITY_ANDROID && !UNITY_EDITOR
             introManager.instance._ShowAndroidToastMessage("کد ثبت شد" + "\n" + user.interData[0].discount + " درصد تخفیف دارید ");
#endif
            if(mystore == Store.Bazar || mystore == Store.Mayket)
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
     
               
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
   introManager.instance._ShowAndroidToastMessage("کد اشتباه است");
#endif
        }

    }


    IEnumerator GetRequestAtFirst3(string uri)
    {
       
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri+"?version="+ Application.version.ToString()))
        {

            webRequest.SetRequestHeader("Authorization", "Bearer " + UnityEngine.PlayerPrefs.GetString("user_token"));
            webRequest.SetRequestHeader("Accept", "application/json");
            //point:
            // BypassCertificate class in the main app controller script
            webRequest.chunkedTransfer = false;
            webRequest.certificateHandler = new BypassCertificate();
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
                InternetAvailabilityTest.instance.CheckConnection();

            }
            else
            {
              //  Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                if ((webRequest.downloadHandler.text).Contains("Error"))
                {
                    // serverBox.SetActive(true);
                }
                else
                {
                    Debug.Log("setting : " + webRequest.downloadHandler.text);
                    CheckDataAtFirst3(webRequest.downloadHandler.text);
                }

            }
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
  
    public void CheckDataAtFirst3(string _url)
    {

        // برای خواندن در اندروید قایل لینک در روت و اضافه کردن دات نت جیسون الزامی است
        var user = JsonConvert.DeserializeObject<RootSettings>(_url);




        PlayerPrefs.SetString("link_whatsApp", user.whatsapp);
        PlayerPrefs.SetString("link_telegram", user.telegram);
        PlayerPrefs.SetString("link_Insta", user.instagram);
        PlayerPrefs.SetString("Force_Update_Message", user.message);
        
        if (user.status != "enable")
        {
          
            Debug.Log("your version needs to update !!!!!!!!!!!!!");
            PlayerPrefs.SetInt("needsUpdate", 1);
            boxUpdate.SetActive(true);
            boxUpdate.transform.GetChild(1).gameObject.GetComponent<RtlText>().text = user.message;
            setBtn();
            boxUpdate.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(body));
            Firebase.Analytics.FirebaseAnalytics.LogEvent("Needs_Force_Update", "Needs_Force_Update_app", 1);
        }
        else
        {
            PlayerPrefs.SetInt("needsUpdate", 0);
            boxUpdate.SetActive(false);
            // get last minor version from server
           // PlayerPrefs.SetFloat("minorVersion", float.Parse(user.message));
            Debug.Log("your version is enable");

            // اینجا زمان کاربر رو چک میکنیم تو هرروز فقط یبار ببینه بازی جایزه رو
            if (PlayerPrefs.GetInt("MyDays") <= 0)
                introManager.instance.CheckTime();
        }

        if (user.hasdico == "enable")
        {
            PlayerPrefs.SetInt("showDicoBox", 1);
            PlayerPrefs.SetString("Dico_Message", user.dicotext);
        }
        else
        {
            PlayerPrefs.SetInt("showDicoBox", 0);
        }
        
        if (user.firsttimefree == "enable")
        {
            if(PlayerPrefs.GetInt("freeFirstTime") == 1)
            {
                PlayerPrefs.SetInt("freeFirstTime", 2);
            }
            else
            {
                PlayerPrefs.SetInt("freeFirstTime", 1);
            }
           
        }
        else
        {
            PlayerPrefs.DeleteKey("freeFirstTime");
        }
       
        PlayerPrefs.SetInt("news_version", user.newsversion);
        PlayerPrefs.SetString("news_Message", user.newsmessage);

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
    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////
    /// </summary>
    public class UserResetPass
    {
        public string mobile;
        public string code;
        public string newPassword;

    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
[Serializable]
public class InterDatumDis
{
    public int status { get; set; }
    public string discount { get; set; }
}
[Serializable]
public class RootDiscountCode
{
    public List<InterDatumDis> interData { get; set; }
}
[Serializable]
public class RootMarket
{
    public string message { get; set; }
}
[Serializable]
public class UserDataInFirst
{
    public string client_id;
    public string device;
    public string currect_app_version;
    public string market_id;
    public string ip;
}
[System.Serializable]
public class UserDataRegister
{
    public string name;
    public string client_id;
    public string mobile;
    public string age;
    public string favourites;
    public string gender;
   
 
    // public List<myQuestions> questions;

}

[System.Serializable]

public class myQuestions
{

    public string q_id;
    public string q_answer;

}
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        //Simply return true no matter what
        return true;
    }
}

///////////////////////////////////////////// taarefe ha //////////////////////////////////////////////////
[System.Serializable]
public class RootTaarefe
{
    public int id { get; set; }
    public string name { get; set; }
    public string day { get; set; }
    public string price { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
[System.Serializable]

public class InterDatum
{
    public int status { get; set; }
    public string link { get; set; }
    public string message { get; set; }
}
[System.Serializable]
public class RootCreateBill
{
    public List<InterDatum> interData { get; set; }
}


public class RootCheckPayment
{
    [JsonProperty("count-day")]
    public string countday { get; set; }
    public string start_day { get; set; }
    public string end_day { get; set; }
}
