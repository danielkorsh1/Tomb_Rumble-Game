using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectController : MonoBehaviour
{
    [SerializeField] private GameObject objectNameBG;
    [SerializeField] private Text objectNameUi;

    private void Start()
    {
        objectNameBG.SetActive(false);
    }

    public void ShowName(string objectName)
    {
        objectNameBG.SetActive(true);
        objectNameUi.text = objectName;
    }

    public void HideName()
    {
        objectNameBG.SetActive(false);
        objectNameUi.text = "";
    }
}
