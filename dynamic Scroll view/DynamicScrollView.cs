using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicScrollView : MonoBehaviour
{
    public static DynamicScrollView instance;
    public Transform scrollViewContent;
    public GameObject prefab;
    GameObject newSpaceShip;
    public List<Sprite> spaceShips;
    public List<GameObject> mybtns;
    public List<GameObject> SpaceShipObj;
    public List<Transform> mybtnsPos;
    public bool isMovie;
    public int witchScroolView;
    [HideInInspector] public ScrollRectEx ssss;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }


        ssss = this.gameObject.transform.parent.transform.parent.gameObject.GetComponent<ScrollRectEx>();
    }
    void Start()
    {
        if (!isMovie)    // برای داستان سیو کردن اسکرول ویو بازی همیشه 100 است
            witchScroolView = 100;
        string savedFloat = PlayerPrefs.GetInt("prouductIDMovie" + PlayerPrefs.GetInt("witchCat")).ToString() + witchScroolView.ToString();
        Debug.Log(PlayerPrefs.GetFloat(savedFloat));
        ssss.horizontalScrollbar.value = PlayerPrefs.GetFloat(savedFloat);

        // Invoke("waitIns", 0.2f);

        Debug.Log("productCountMovie : " + PlayerPrefs.GetInt("productCountMovie"));
        Debug.Log("productCountGame : " + PlayerPrefs.GetInt("productCountGame"));


    }
    public async void setProudoucts()
    {
        Destroy(this.gameObject.transform.GetChild(0).gameObject);
        if (PlayerPrefs.GetInt("witchCat") == 5 | PlayerPrefs.GetInt("witchCat") == 25)
        {
            if (PlayerPrefs.GetInt("productCountMovie" + witchScroolView) > 0)
            {
                for (int i = 0; i < PlayerPrefs.GetInt("productCountMovie" + witchScroolView); i++)
                {
                    mybtns.Insert(i, prefab);
                    mybtnsPos.Insert(i, scrollViewContent);
                    SpaceShipObj.Insert(i, newSpaceShip);
                    await System.Threading.Tasks.Task.Delay(100);
                }
                loadDataOnBtn();
                
            }

        }
        else
        {
            if (PlayerPrefs.GetInt("productCountMovie") > 0 & PlayerPrefs.GetInt("productCountGame") > 0)
            {
                if (PlayerPrefs.GetInt("productCountMovie") > PlayerPrefs.GetInt("productCountGame"))
                {
                    for (int i = 0; i < PlayerPrefs.GetInt("productCountMovie"); i++)
                    {
                        mybtns.Insert(i, prefab);
                        mybtnsPos.Insert(i, scrollViewContent);
                        SpaceShipObj.Insert(i, newSpaceShip);
                    }
                }
                else
                {
                    for (int i = 0; i < PlayerPrefs.GetInt("productCountGame"); i++)
                    {
                        mybtns.Insert(i, prefab);
                        mybtnsPos.Insert(i, scrollViewContent);
                        SpaceShipObj.Insert(i, newSpaceShip);
                    }
                }
                Debug.Log("move and game has product");
                loadDataOnBtn();
            }
            else if (PlayerPrefs.GetInt("productCountMovie") > 0)
            {
                for (int i = 0; i < PlayerPrefs.GetInt("productCountMovie"); i++)
                {
                    mybtns.Insert(i, prefab);
                    mybtnsPos.Insert(i, scrollViewContent);
                    SpaceShipObj.Insert(i, newSpaceShip);
                }
                Debug.Log("move has product");
                loadDataOnBtn();
            }
            else if (PlayerPrefs.GetInt("productCountGame") > 0)
            {
                for (int i = 0; i < PlayerPrefs.GetInt("productCountGame"); i++)
                {
                    mybtns.Insert(i, prefab);
                    mybtnsPos.Insert(i, scrollViewContent);
                    SpaceShipObj.Insert(i, newSpaceShip);
                }
                Debug.Log(" game has product");
                loadDataOnBtn();
            }
        }


       // this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    int counterM, counterG;
    public void loadDataOnBtn()
    {

        if (isMovie)
        {
            if (PlayerPrefs.GetInt("witchCat") == 5 | PlayerPrefs.GetInt("witchCat") == 25)
            {
                for (int i = 0; i < PlayerPrefs.GetInt("productCountMovie" + witchScroolView); i++)
                {

                    spaceShips.Insert(i, SaveLoadImage.instance.giftImage.sprite);
                }

                if (PlayerPrefs.GetInt("productCountMovie" + witchScroolView) > 0)
                {
                    for (int s = 0; s < spaceShips.Count; s++)
                    {
                        SpaceShipObj[s] = Instantiate(mybtns[s], mybtnsPos[s]);
                        SpaceShipObj[s].GetComponent<ScrollViewItem>().flag = s;
                        SpaceShipObj[s].GetComponent<ScrollViewItem>().witchScroolView = witchScroolView;
                        SpaceShipObj[s].GetComponent<ScrollViewItem>().isMovie = true;
                        SpaceShipObj[s].GetComponent<ScrollViewItem>().downImage(s);

                    }
                }
            }
            else
            {
                for (int i = 0; i < PlayerPrefs.GetInt("productCountMovie"); i++)
                {

                    spaceShips.Insert(i, SaveLoadImage.instance.giftImage.sprite);
                }

                if (PlayerPrefs.GetInt("productCountMovie") > 0)
                {
                    for (int s = 0; s < spaceShips.Count; s++)
                    {
                        SpaceShipObj[s] = Instantiate(mybtns[s], mybtnsPos[s]);
                        SpaceShipObj[s].GetComponent<ScrollViewItem>().flag = s;
                        SpaceShipObj[s].GetComponent<ScrollViewItem>().isMovie = true;
                        SpaceShipObj[s].GetComponent<ScrollViewItem>().downImage(s);
                    }
                }
            }




        }
        else
        {

            for (int i = 0; i < PlayerPrefs.GetInt("productCountGame"); i++)
            {
                spaceShips.Insert(i, SaveLoadImage.instance.giftImage.sprite);
            }
            if (PlayerPrefs.GetInt("productCountGame") > 0)
            {
                for (int s = 0; s < spaceShips.Count; s++)
                {
                    SpaceShipObj[s] = Instantiate(mybtns[s], mybtnsPos[s]);
                    SpaceShipObj[s].GetComponent<ScrollViewItem>().flag = s;
                    SpaceShipObj[s].GetComponent<ScrollViewItem>().isMovie = false;
                    SpaceShipObj[s].GetComponent<ScrollViewItem>().downImage(s);

                }
            }

        }

    }

    void OpenLink(string link)
    {
        Debug.Log(link);
    }

}
