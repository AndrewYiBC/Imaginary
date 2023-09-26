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
    private LineRenderer lr2;

    // Polar
    private bool isInPolar1 = false;
    private bool isInPolar2 = false;

    [SerializeField] private GameObject lrPolar1GameObject;
    [SerializeField] private GameObject lrPolar2GameObject;
    private Vector3[] points1 = new Vector3[2];
    private Vector3[] points2;

    private float directionInput = 0;
    [SerializeField] private float maxRadius;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr1 = lrPolar1GameObject.GetComponent<LineRenderer>();
        lr2 = lrPolar2GameObject.GetComponent<LineRenderer>();
        lr1.positionCount = 0;
        lr2.positionCount = 0;
    }

    void Update()
    {
        if (isInPolar1)
        {
            points1[1] = transform.position;
            DrawLinePolar1();
            MaxRadiusCheck();
        }
        else if (isInPolar2)
        {
            DrawLinePolar2();
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
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                lr1.positionCount = 0;
                lr2.positionCount = 0;
            }
        }
    }

    public void PolarDirection(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isInPolar1)
            {
                isInPolar1 = false;
                isInPolar2 = true;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                directionInput = context.ReadValue<float>();
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

    private void MaxRadiusCheck()
    {
        float x0 = points1[0].x;
        float x1 = points1[1].x;
        if (x1 - x0 > maxRadius)
        {
            transform.position = new Vector3(x0 + maxRadius, transform.position.y, transform.position.z);
        }
        else if (x1 - x0 < -maxRadius)
        {
            transform.position = new Vector3(x0 - maxRadius, transform.position.y, transform.position.z);
        }
    }

    private void DrawLinePolar1()
    {
        lr1.SetPosition(0, points1[0]);
        lr1.SetPosition(1, points1[1]);
    }

    private void DrawLinePolar2()
    {
        for (int i = 0; i < lr2.positionCount; i++)
        {
            lr2.SetPosition(i, points2[i]);
        }
    }
}
