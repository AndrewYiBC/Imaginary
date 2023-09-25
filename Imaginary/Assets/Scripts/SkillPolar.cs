using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillPolar : MonoBehaviour
{
    // Variables
    // Components
    private Rigidbody2D rb;
    private LineRenderer lr1;

    // Polar
    private bool isInPolar1 = false;
    private bool isInPolar2 = false;

    [SerializeField] private GameObject lrPolar1GameObject;
    private Vector3[] points1 = new Vector3[2];



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr1 = lrPolar1GameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (isInPolar1)
        {
            points1[1] = transform.position;
            drawLinePolar1();
        }
    }



    // Input System
    public void Polar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!IsInPolar())
            {
                // Not in Polar mode: enter Polar mode
                isInPolar1 = true;
                rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                lr1.positionCount = 2;
                points1[0] = transform.position;
            }
            else if (isInPolar1)
            {
                // In Polar stage 1: exit Polar mode
                isInPolar1 = false;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                lr1.positionCount = 0;
            }
            else
            {
                // In Polar stage 2: exit Polar mode
                isInPolar2 = false;
            }
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
        return isInPolar1 | isInPolar2;
    }

    private void drawLinePolar1()
    {
        lr1.SetPosition(0, points1[0]);
        lr1.SetPosition(1, points1[1]);
    }
}
