using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class NoteController : MonoBehaviour
{

    [SerializeField] private KeyCode closeKey;
    [SerializeField] private GameObject noteCanvas;
    [SerializeField] private Text noteTextUI;
    [SerializeField][TextArea] private string noteText;
    //an unity event that has the params of a sound
    [SerializeField] private AudioSource NoteAudio;
    private bool isOpen = false;

    public void ShowNote()
    {
        noteTextUI.text = noteText;
        noteCanvas.SetActive(true);
        isOpen = true;
        NoteAudio.Play();
        Debug.Log("exactly");
    }

    void DisableNote()
    {
        noteCanvas.SetActive(false);
        isOpen = false;
    }

    private void Update()
    {
        if (isOpen)
        {
            if (Input.GetKeyDown(closeKey))
            {
                DisableNote();
            }
        }
    }
}
