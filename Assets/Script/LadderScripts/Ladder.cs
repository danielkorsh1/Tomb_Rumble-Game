using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    
    private float ladderLength;
    private float ladderZ;
    private float laddery;
    [SerializeField]  public Transform topOfLadder;
    


   public float getLadderLength()
    {
        return ladderLength;
    }
    public float getLadderZ()
    {
        return ladderZ;
    }
    public float getLadderY()
    {
        return laddery;
    }
    void Start()
    {
        ladderLength = topOfLadder.position.y;
        ladderZ = topOfLadder.position.z;
        laddery = transform.position.y;
    }

    void Update()
    {
        
    }
}
