using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RocketPart : MonoBehaviour
{
    public CraneController controller;
    public Rigidbody rb;
    public bool locked;
    private Vector3[] positions;

    
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (controller.gameOver) return;
        
        RocketPart otherRocketPart = other.transform.GetComponent<RocketPart>();

        if (other.transform.CompareTag("Ground"))
        {
            controller.GameOver();
            return;
        }
        
        if (otherRocketPart)
        {
            if (controller && !locked)
            {
                if (GetComponent<SpringJoint>()) return;
                SpringJoint joint = gameObject.AddComponent<SpringJoint>();

                joint.connectedBody = other.rigidbody;
                joint.spring = 200f;
                
                controller.PlayPlop();
                controller.rocketParts.Add(this);
                controller.GenerateNewPart();
                locked = true;
            }
        }
    }
}
