using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Firebase.Database;



public class FadeInOut : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;
    private string id;
    public DatabaseReference DBreference;


     void Start()
    {
        id = FireBaseManager.instance.GetUserID();
        DBreference = FireBaseManager.instance.databaseReference();
    }
    //funcstion to call in the next script
    public void getKey()
    {
        if (Input.GetKey(KeyCode.E))
        {
            fadeToNextLevel();
            StartCoroutine(UpdatesceneNum(levelToLoad));
        }
    }
    //fade to the next level in the Queue
    public void fadeToNextLevel ()
    {
        FadeToLevel(SceneManager.GetActiveScene().buildIndex+1);
    }
    //triggers the animation and sets the  value to level to load
    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }
    //loads the scene 
    public void onFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);

    }
    public int GetlevelToLoad()
    {
        return levelToLoad;
    }
    private IEnumerator UpdatesceneNum(int num)
    {
        //Set the currently logged in user xp

        Task DBTask = DBreference.Child("users").Child(id).Child("sceneNum").SetValueAsync(num);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Xp is now updated
        }
    }
}
