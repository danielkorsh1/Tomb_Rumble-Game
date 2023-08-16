using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Firebase.Database;

public class InspectRayCast : MonoBehaviour
{
    //the length of the line that will check for objects
    [SerializeField] private float rayLength;
    //the layer name
    [SerializeField] private LayerMask LayerMaskInteract;
    //objectController for the display of objects
    private ObjectController InteractableObject;
    //objectController for the Note display
    private ObjectController noteObj;
    //image param for the crosshair
    [SerializeField] private Image crosshair;
    //param of the class ButtoController which have the methods of displaying and removie the object text
    private ButtonDoorController ButtonRaycast;
    //param of the class NoteController
    private NoteController _noteController;
    //param that the raycast will exlude from going past(walls)
    [SerializeField] private string excludeLayer = null;
    //button that will be pressed for intractiong with objects
    private KeyCode interact = KeyCode.E;
    //key to press Door buttons
    private KeyCode DoorButton = KeyCode.Mouse0;
    //the string that is the correct word for opening a secret door
    private string access;
    //bool for activating the color of the crosshair
    private bool isCrosshairActive;
    //making sure that it wont loop the display of object names
    private bool doOnce;
    //a int param that counts how many tries the player has done at the hiddendoor
    private int buttonCount = 0;
    //getting the aDoor properties for the specific door
    private DoorProperties SecretDoor;
    //the dictonery param
    [SerializeField] private Image poneglyph;
    private bool poneglyphActive = false;
    [SerializeField] private AudioSource PoneglyphSound;
    private int totalScore;

    //fire base handler
    public DatabaseReference DBreference;
    public static int DBTotalScore;
    [SerializeField] public Text scoreText;


    public void Start()
    {
        //gets the score that is in the realtime database of the specific user
        DBTotalScore = FireBaseManager.instance.GetTotalScore();
        
        scoreText.text = DBTotalScore.ToString();
    }




    private void Update()
    {
        activePoniglyph();
        

        RaycastHit hit;
        Vector3 front = transform.TransformDirection(Vector3.forward);
        int mask = 1 << LayerMask.NameToLayer(excludeLayer) | LayerMaskInteract.value;

        //this if statment will make a a stright line with the params above that it is infront of the object we have placed it on(the player camera) and will see if there is something infront of it
        //5 cm ahead and look for a layermask
        if (Physics.Raycast(transform.position, front, out hit, rayLength, mask))
        {
           

            if (hit.collider.CompareTag("DoorButton"))
            {

                if (!doOnce)
                {
                    ButtonRaycast = hit.collider.gameObject.GetComponent<ButtonDoorController>();
                    SecretDoor = hit.collider.gameObject.GetComponentInParent<DoorProperties>();
                    CrosshairChange(true);

                }
                if (Input.GetKeyDown(DoorButton))
                {
                    ButtonRaycast.PlayButtonAnimation(SecretDoor);
                    access += ButtonRaycast.letter;
                    buttonCount++;
                    Debug.Log(access);

                }

                if (buttonCount == SecretDoor.getLength())
                {
                    if (access.Equals(SecretDoor.getAceess()) )
                    {
                        ButtonRaycast.PlayAnimation();
                        buttonCount = 0;
                        access = "";
                    }

                    else
                    {
                        ButtonRaycast.ResetButtons(SecretDoor);
                        buttonCount = 0;
                        access = "";

                    }
                }

                isCrosshairActive = true;
                doOnce = true;

            }


            // if the collider hits an object with a "riddleNote" tag it will let you open up the text and image
            else if (hit.collider.CompareTag("RiddelNote"))
            {
                if (!doOnce)
                {
                    CrosshairChange(true);
                    _noteController = hit.collider.GetComponent<NoteController>();
                }
                    if (_noteController != null)
                    {
                        
                        if (Input.GetKeyDown(interact))
                        {
                        _noteController.ShowNote();
                        }
                    }


                    else
                    {
                        ClearNote();
                    }
                

                isCrosshairActive = true;
                doOnce = true;

            }

            doOnce = false;
            //this if statment will see if the player is looking directly at an object that has the "InteractObject"
            //which then will enter the if


            if (hit.collider.gameObject.layer == 6 && hit.collider.gameObject.GetComponent<ObjectController>() != null)
            {

                if (!doOnce)
                {
                    InteractableObject = hit.collider.gameObject.GetComponent<ObjectController>();
                    InteractableObject.ShowObjectName();
                    CrosshairChange(true);

                }

                
                if (InteractableObject.collectiable) 
                { 
                    if (!InteractableObject.UniqueItem())
                    {
                        if (Input.GetKeyDown(interact))
                        {
                            InteractableObject.AddScore(InteractableObject.GetScore());
                            InteractableObject.AddObjects(InteractableObject.name);

                            InteractableObject.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (Input.GetKeyDown(interact))
                        {
                            InteractableObject.AddScore(InteractableObject.GetScore());
                            InteractableObject.AddRareObjects(InteractableObject.gameObject.name);
                            InteractableObject.ShowItemBox(InteractableObject.gameObject.name);
                            InteractableObject.gameObject.SetActive(false);
                        }
                    }
                }

                isCrosshairActive = true;
                doOnce = true;

            }



        }

        else
        {
            if (InteractableObject != null)
                InteractableObject.HideObjectName();
            if (isCrosshairActive)
            {
                CrosshairChange(false);
                if (doOnce)
                {
                    doOnce = false;
                }
            }

        }

        
    }


    
    

    //this method will turn on and off our crosshair taking account if theres something that can be intractable
    //on = the crosshair will turn to the color red
    //off = will make the crosshair its defult color that is white
    void CrosshairChange(bool on)
    {
        if (on && !doOnce)
        { 
        crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.white;
            isCrosshairActive = false;
        }
    }
    //this method will clear our note object from our screen
    void ClearNote()
    {
        if (_noteController != null)
        {
            CrosshairChange(false);
            _noteController = null;
        }
    }

    public void activePoniglyph()
    {
        if (Input.GetKeyDown(KeyCode.F) && !poneglyphActive)
        {
            poneglyph.gameObject.SetActive(true);
            poneglyphActive = true;
            PoneglyphSound.Play();
        }
        else if (poneglyphActive)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                poneglyph.gameObject.SetActive(false);
                poneglyphActive = false;
                PoneglyphSound.Play();
            }
        }
    }





}
