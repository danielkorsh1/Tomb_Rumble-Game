using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscScript : MonoBehaviour
{
    public GameObject BackButton;
 
    public void BackOff()
    {
        BackButton.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
