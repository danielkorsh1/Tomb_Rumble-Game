using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LvllChanger : MonoBehaviour
{
    public FadeInOut fadeInOut;
    public GameObject GUI;

    public void OnCollisionStay(Collision collision)
    {
        Debug.Log("collision");
        if(collision.gameObject.tag == "Player")
        {
            fadeInOut.getKey();
            GUI.SetActive(false);
        }
        else
        {
            GUI.SetActive(true);
        }
    }

}
