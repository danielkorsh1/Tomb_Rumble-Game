using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrapButton : MonoBehaviour
{
    [SerializeField] private Animator TrapAnim = null;
    [SerializeField] private bool dropTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (dropTrigger)
            {
                Debug.Log("haha");
                TrapAnim.Play("TrapDrop", 0, 0.3f);
                dropTrigger = false;
            }


        }

    }
}

