using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TrapDeathCollision : MonoBehaviour
{
    public DeathRespawn deathRespawn;
    public AudioSource DeathSound;
    public GameObject GUI;
    private bool collisonOccured = false;
    public void OnCollisionEnter(Collision collision)
    {
        if (collisonOccured)
            return;
        if (collision.gameObject.tag == "Player")
        {
            
            deathRespawn.getDeath();
            DeathSound.Play();
            collisonOccured = true;
            GUI.SetActive(false);

        }
        else
        {
            GUI.SetActive(true);
        }
        
    }
    public bool GetcollisonOccured()
    {
        return collisonOccured;
    }


}
