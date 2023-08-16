using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Climb : MonoBehaviour
{

    private float wallLookAngle;
    public float maxWallLookAngle;
    public bool ladderFront = false;

    private float time;
    private float downTime = 0.5f;


    public float climbspeed = 0.05f;
    private bool onLadder = false; 

    public Rigidbody rb;
    public PlayerController pc;
    private Ladder Ladder;

    
    Vector3 rotation;
    
    Vector3 targetDirection;
    RaycastHit raycastHit;

    private void LateUpdate()
    {
        time += Time.deltaTime;
        rotation = transform.rotation.eulerAngles;
        targetDirection = Quaternion.Euler(0.0f, rotation.y, 0.0f) * Vector3.forward;
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, targetDirection, Color.green);
        LadderCheck();
        stateMachine();

    }


    private void stateMachine()
    {

        pc._Animator.enabled = true;
        if (ladderFront && CoolDown() && (wallLookAngle < maxWallLookAngle))
        {
            
            StartCliming();
            float verticalInput = Input.GetAxisRaw("Vertical");
            rb.useGravity = true;
            if (verticalInput > 0)
            {
                // Climbing up
                ClimingUp();
                pc._grounded = true;
                
                pc._Animator.SetBool(pc._climbHash, ladderFront);
                pc._Animator.SetBool(pc._fallingHash, !pc._grounded);

                if (rb.transform.position.y >= Ladder.getLadderLength() && CoolDown())
                {
                    float climbUpDistance = 0.5f;
                    Vector3 climbUpPosition = new Vector3(rb.transform.position.x, Ladder.getLadderLength() + climbUpDistance, Ladder.getLadderZ() - 0.1f);
                    rb.MovePosition(climbUpPosition + (targetDirection * 0.1f));
                    time = 0;
                    ladderFront = false;
                    
                }
            }
            else if (ladderFront && verticalInput == 0)
            {
                pc._Animator.SetBool(pc._climbHash, ladderFront);
                pc._Animator.enabled = false;
                rb.velocity = Vector3.zero;
                rb.useGravity = false;
                pc._grounded = true;

            }
            else if (verticalInput < 0)
            {
                // Climbing down
                pc._Animator.SetBool(pc._climbHash, ladderFront);
                pc._Animator.SetBool(pc._fallingHash, !pc._grounded);
                ClimingDown();
                pc._grounded = true;
              
            }
           
        }
        else
        {
            if(ladderFront && Ladder.getLadderY() >= rb.transform.position.y + 0.5f)
            {
                Vector3 offLadder = new Vector3(rb.transform.position.x, rb.transform.position.y, rb.transform.position.z);
                rb.MovePosition(offLadder + (-targetDirection * 1.5f));
            }
            
            pc._grounded = true;
            StopCliming();
            rb.useGravity = true;
            ladderFront = false;
            pc._Animator.SetBool(pc._climbHash, ladderFront);

        }
    }



    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TP_LadderCollider"))
        {
           

            if (Input.GetKey(KeyCode.E))
            {
                ladderFront = true;
                float yrotation = rotation.y;

                if (yrotation > 180f)
                {
                    transform.eulerAngles = new Vector3(0, rotation.y - 180, 0);
                }

                else
                {
                    transform.eulerAngles = new Vector3(0, rotation.y + 180, 0); ;
                }
                Vector3 climbUpPosition = new Vector3(rb.transform.position.x, Ladder.getLadderLength(), Ladder.getLadderZ() - 0.1f);
                rb.MovePosition(climbUpPosition + targetDirection);
            }
        }
    }


    private void LadderCheck()
    {
        
        float ladderGrabDistance = .7f;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, targetDirection, out  raycastHit, ladderGrabDistance))
           
        {
            if (raycastHit.transform.TryGetComponent(out Ladder ladder))
            {

                Ladder = ladder;
                ladderFront = true;
                wallLookAngle = Vector3.Angle(targetDirection, -raycastHit.normal);
                

            }
        }
        else
        {
            ladderFront = false;
            wallLookAngle = 90f;
           
        }
    }

    private void StartCliming()
    {
        onLadder = true;
    }

    private void ClimingUp()
    {
        rb.velocity = new Vector3(0, climbspeed,0);
    }
    private void ClimingDown()
    {
        rb.velocity = new Vector3(0, -climbspeed, 0);
        if (rb.transform.position.y <= Ladder.transform.position.y && CoolDown())
        {
            rb.velocity = Vector3.zero;
            ladderFront = false;
            time = 0;
        }
    }
    private void StopCliming()
    {
        onLadder = false;
    }
    public bool getOnLadder()
    {
        return onLadder;
    }
    private bool CoolDown()
    {
        if(time >= downTime)
        {
            return true;
        }
        return false;
    }
}