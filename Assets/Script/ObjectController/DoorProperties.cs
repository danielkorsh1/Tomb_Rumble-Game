using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorProperties : MonoBehaviour
{
    [SerializeField] private string access;

    public string getAceess()
    {
        return access;
    }

    public int getLength()
    {
        return access.Length;
    }
}
