using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class movementController : MonoBehaviour
{
    // Start is called before the first frame update


    //public float speedFactor;
    public float speed;
    public float turningSpeed;

    
    void Start()
    {
 
    }

    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        TurnRight();
        TurnLeft();
        Move();
    }

    void Move()
    {
        this.gameObject.transform.Translate(new Vector3(speed,0,0), Space.Self);
    }
    void TurnRight() 
    {
        if (CrossPlatformInputManager.GetButton("Fire2"))
        {
            this.transform.Rotate(new Vector3(0, turningSpeed, 0));
        }
    }
    void TurnLeft()
    {
        if (CrossPlatformInputManager.GetButton("Fire1"))
        {
            this.transform.Rotate(new Vector3(0, -turningSpeed, 0));
        }
        
    }
}
