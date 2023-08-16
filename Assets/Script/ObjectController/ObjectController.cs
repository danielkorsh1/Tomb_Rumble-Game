using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine.Events;

public class ObjectController : MonoBehaviour
{
    // 
    public DatabaseReference DBreference;
    private string id;
    public static int DBTotalScore;
    private List<string> DBObjects = new();
    private List<string> DBRareObjects = new();

    [SerializeField] private InspectController inspectController;
    
    [Header("Regular Objects")]
    [SerializeField] private string itemName;
    [SerializeField] public bool collectiable;

    [Header("Score")]
    [SerializeField] public Text scoreText;
    [SerializeField] private int score;
    

    [Header("Rare Objects")]
    [SerializeField] public bool isUnique;
    [SerializeField] private GameObject RubyAmulet;
    [SerializeField] private GameObject RubyPyramid;
    [SerializeField] private GameObject RubyScarrab;
    [SerializeField] private GameObject Mirror;


    void Start()
    {
        //brings the user UID that has logged in
        id = FireBaseManager.instance.GetUserID();
        //bring reference to the real time database
        DBreference = FireBaseManager.instance.databaseReference();
        //gets the score that is in the realtime database of the specific user
        DBTotalScore = FireBaseManager.instance.GetTotalScore();
        //gets the list<string> of the objects that the user collected that is in the realtime database of the specific user
        DBObjects = FireBaseManager.instance.GetCollectable();
        //gets the list<string> of the Rare objects that the user collected that is in the realtime database of the specific user
        DBRareObjects = FireBaseManager.instance.GetRareCollectable();


        if (gameObject.GetComponent<ObjectController>().isUnique)
        {
            if (DBRareObjects.Contains(gameObject.name))
            {
                gameObject.SetActive(false);
                ShowItemBox(gameObject.name);
            }
        }

        if (gameObject.GetComponent<ObjectController>().collectiable)
        {
            if (DBObjects.Contains(gameObject.name))
            {
                gameObject.SetActive(false);
            }
        }

       

    }

    public void ShowObjectName()
    {
        inspectController.ShowName(itemName);
    }

    public void HideObjectName()
    {
        inspectController.HideName();
    }

    public bool UniqueItem()
    {
        return isUnique;
    }

    public int GetScore()
    {
        return score;
    }

    public void AddScore(int score)
    {
        DBTotalScore += score;
        scoreText.text = DBTotalScore.ToString();
        StartCoroutine(UpdateScore(DBTotalScore));

    }

    public void AddObjects(string ObjectId)
    {
        DBObjects.Add(ObjectId);
        StartCoroutine(UpdateObjects(DBObjects));
        
    }

    public void AddRareObjects(string RareObjectId)
    {
        DBRareObjects.Add(RareObjectId);
        StartCoroutine(UpdateRareObjects(DBRareObjects));

    }

    public void ShowItemBox(string name)
    {
        if (name.Equals("RubyAmulet"))
            RubyAmulet.SetActive(true);
        else if (name.Equals("RubyPyramid"))
            RubyPyramid.SetActive(true);
        else if (name.Equals("RubyScarrab"))
            RubyScarrab.SetActive(true);
        else if (name.Equals("Mirror"))
            Mirror.SetActive(true);
    }

    private IEnumerator UpdateScore(int DataScore)
    {
        //Set the currently logged in user Score

        Task DBTask = DBreference.Child("users").Child(id).Child("TotalScore").SetValueAsync(DataScore);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Score is now updated
           
        }
    }

    private IEnumerator UpdateObjects(List<string> ObjectsData)
    {
        //Set the currently logged in user Objects
        Task DBTask = DBreference.Child("users").Child(id).Child("Collecables").SetValueAsync(ObjectsData);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Score is now updated

        }
    }

    private IEnumerator UpdateRareObjects(List<string> RareObjectsData)
    {
        //Set the currently logged in user Objects
        Task DBTask = DBreference.Child("users").Child(id).Child("Rare Collection").SetValueAsync(RareObjectsData);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Score is now updated

        }
    }
}
