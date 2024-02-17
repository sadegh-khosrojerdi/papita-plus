using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





[System.Serializable]
public class Result
{
    public string token { get; set; }
    public string message { get; set; }
}
[System.Serializable]
public class Root
{
    public User user { get; set; }
    public string token { get; set; }
    public string message { get; set; }
}
[System.Serializable]
public class User
{
    public int id { get; set; }
    public string name { get; set; }
    public string client_id { get; set; }
    public string device { get; set; }
    public string currect_app_version { get; set; }
    public string age { get; set; }
    public string favorites { get; set; }
    public string gender { get; set; }
    public string mobile { get; set; }
    public int account_verified { get; set; }
    public object gmail { get; set; }
    public string ip { get; set; }
    public object api_token { get; set; }
    public object email_verified_at { get; set; }
    public string market_id { get; set; }
    public int status_id { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
[System.Serializable]
public class RootRegister
{
    public string message { get; set; }
    public int status { get; set; }
    public UserRegister user { get; set; }
}

[System.Serializable]
public class UserRegister
{
    public int id { get; set; }
    public string name { get; set; }
    public string client_id { get; set; }
    public string invite_code { get; set; }
    public string device { get; set; }
    public string currect_app_version { get; set; }
    public string age { get; set; }
    public string favorites { get; set; }
    public string gender { get; set; }
    public string mobile { get; set; }
    public string account_verified { get; set; }
    public object gmail { get; set; }
    public string ip { get; set; }
    public object api_token { get; set; }
    public object email_verified_at { get; set; }
    public string market_id { get; set; }
    public string status_id { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}

[System.Serializable]
public class RootLogin
{
    public UserLogin user { get; set; }
    public string token { get; set; }
    public string message { get; set; }
    public int status { get; set; }
}
[System.Serializable]
public class UserLogin
{
    public int id { get; set; }
    public string name { get; set; }
    public string client_id { get; set; }
    public string device { get; set; }
    public string currect_app_version { get; set; }
    public string age { get; set; }
    public string favorites { get; set; }
    public string gender { get; set; }
    public string mobile { get; set; }
    public string account_verified { get; set; }
    public object gmail { get; set; }
    public object ip { get; set; }
    public object api_token { get; set; }
    public object email_verified_at { get; set; }
    public string market_id { get; set; }
    public string status_id { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public ScoreLogin score { get; set; }
}
[System.Serializable]
public class ScoreLogin
{
    public int id { get; set; }
    public string user_id { get; set; }
    public string name { get; set; }
    public string age { get; set; }
    public string city { get; set; }
    public string month { get; set; }
    public string score { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public int num { get; set; }
}

[System.Serializable]
public class RootResendVerifyCode
{
    public string message { get; set; }
    public int status { get; set; }
}
[System.Serializable]
public class RootCheckPasswordChange
{
    public string token { get; set; }
    public string message { get; set; }
    public int status { get; set; }
}
[System.Serializable]
public class LogOutClass
{
    public string status;
    public List<LogOutList> interdata;

}



[System.Serializable]

public class LogOutList
{
    public string status;
    
}

/// <summary>
/// //////////////////////////////////////////////////// init user Data //////////////////////////////////////
/// </summary>

[System.Serializable]
public class ApplicationsData
{
    public int id { get; set; }
    public string name { get; set; }
    public string invite_code { get; set; }
    public int number_of_invitations { get; set; }
    public string client_id { get; set; }
    public string device { get; set; }
    public string currect_app_version { get; set; }
    public string age { get; set; }
    public string favorites { get; set; }
    public string gender { get; set; }
    public string mobile { get; set; }
    public int account_verified { get; set; }
    public object email { get; set; }
    public string ip { get; set; }
    public object api_token { get; set; }
    public object email_verified_at { get; set; }
    public string market_id { get; set; }
    public int status_id { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
[System.Serializable]
public class InterDataInit
{
    public string token { get; set; }
    public string name { get; set; }
    public int status { get; set; }
}
[System.Serializable]
public class RootInit
{
    public List<InterDataInit> interData { get; set; }
    public ApplicationsData applicationsData { get; set; }
}








