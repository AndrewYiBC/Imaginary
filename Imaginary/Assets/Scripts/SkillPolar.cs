using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillPolar : MonoBehaviour
{
    // Variables
    // Components
    private Rigidbody2D rb;
    // 
    private bool isInPolar1 = false;
    private bool isInPolar2 = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }



    // Input System
    public void Polar(InputAction.CallbackContext context)
    {
        if (!IsInPolar())
        {
            // Not in Polar mode: enter Polar mode
            isInPolar1 = true;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else if (IsInPolar1())
        {
            // In Polar stage 1: exit Polar mode
            isInPolar1 = false;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            // In Polar stage 2: exit Polar mode
            isInPolar2 = false;
        }
    }



    // Helper Functions
    public bool IsInPolar1()
    {
        return isInPolar1;
    }

    public bool IsInPolar2()
    {
        return isInPolar2;
    }

    public bool IsInPolar()
    {
        return IsInPolar1() || IsInPolar2();
    }
}
