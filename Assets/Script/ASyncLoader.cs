using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ASyncLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainmenu;
    [SerializeField] private Slider LoadingSlider;

    public void LoadLevelbtn(int levelToLoad)
    {
        mainmenu.SetActive(false);
        loadingScreen.SetActive(true);
        StartCoroutine(LoadLevelAsync(levelToLoad));
    }


    IEnumerator LoadLevelAsync(int leveToLoad)
    {
        AsyncOperation loadOpertion = SceneManager.LoadSceneAsync(leveToLoad);
        while (!loadOpertion.isDone)
        {
            float proggress = Mathf.Clamp01(loadOpertion.progress / 0.9f);
            LoadingSlider.value = proggress;
            yield return null;
        }
    }
}
