using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DeathRespawn : MonoBehaviour
{
    public Animator animator;
    public int levelToLoad;
    
    //funcstion to call in the next script
    public void getDeath()
    {
        
        FadeToLevel2(SceneManager.GetActiveScene().buildIndex);
    }
    //triggers the animation and sets the  value to level to load
    public void FadeToLevel2(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("Dead");
    }
    //loads the scene 
    public void onFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
