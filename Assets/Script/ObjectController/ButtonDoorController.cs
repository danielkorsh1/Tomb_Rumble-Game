using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonDoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnim;
    [SerializeField] public char letter;
    [SerializeField] private Animator ButtonAnim;
    [SerializeField] private string ButtonPressAnimation;
    [SerializeField] private string ButtonUnpressAnimation;
    [SerializeField] private AudioSource OpenDoor;
    [SerializeField] private AudioSource ButtonClick;

    private bool doorOpen = false;
    public static Animator[] ButtonsReset = new Animator[10];
    public static string[] ButtonsResetName = new string[10];

    private static int lettercount = 0;


    public void PlayAnimation()
    {
        if (!doorOpen)
        {
            Debug.Log("huaha");
            OpenDoor.Play();
            doorAnim.Play("DoorDown", 0, 0.0f);
            doorOpen = true;
        }
    }

    public void PlayButtonAnimation(DoorProperties secretDoor)
    {

            Debug.Log(letter);
            ButtonClick.Play();
            ButtonAnim.Play(ButtonPressAnimation, 0, 0.0f);
            ButtonsReset[lettercount] = ButtonAnim;
            ButtonsResetName[lettercount] = ButtonUnpressAnimation;
            
        if (lettercount < secretDoor.getLength())
            {
                lettercount++;
            }

    }

    public void ResetButtons(DoorProperties secretDoor)
    {
        Debug.Log(ButtonsReset[0]);
        for (int i = 0; i < secretDoor.getLength(); i++)
        {
            ButtonsReset[i].Play(ButtonsResetName[i], 0, 0.0f);
        }

        lettercount = 0;
        ButtonsReset = new Animator[10];
        ButtonsResetName = new string[10];
    }

}
