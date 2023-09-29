using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SkillPolar : MonoBehaviour
{
    // Constants
    private const float PI = Mathf.PI;

    // Variables
    // GameObjects
    private GameObject playerObject;
    [SerializeField] private GameObject textGroup;
    [SerializeField] private GameObject textBaseRadius_Obj;
    [SerializeField] private GameObject textExpSign_Obj;
    [SerializeField] private GameObject textExpAngle_Obj;

    // Components and such
    private Transform pltf; // playerObject's transform
    private Rigidbody2D rb;
    private LineRenderer lr1;
    private LineRenderer lr2;

    private TextMeshPro textBaseRadius;
    private TextMeshPro textExpAngle;

    // Polar Skill
    private bool isInPolar1 = false;
    private bool isInPolar2 = false;

    [SerializeField] private GameObject lrPolar1GameObject;
    [SerializeField] private GameObject lrPolar2GameObject;
    private Vector3[] points1 = new Vector3[2];
    private List<Vector3> points2 = new List<Vector3>();

    private float directionInput = 0;
    [SerializeField] private float maxRadius;
    [SerializeField] private float minRadius;

    // Polar Coordinate
    [SerializeField] private float periodPerRadius;
    private float timeIncrementPerSecond = 60f;
    private float polarRadiusSigned = 0;
    private float polarRadius = 0;
    private float polarAngle = 0;   // In radians



    void Start()
    {
        pltf = transform.parent;
        playerObject = pltf.gameObject;
        rb = playerObject.GetComponent<Rigidbody2D>();

        lr1 = lrPolar1GameObject.GetComponent<LineRenderer>();
        lr2 = lrPolar2GameObject.GetComponent<LineRenderer>();
        lr1.positionCount = 0;
        lr2.positionCount = 0;

        textBaseRadius = textBaseRadius_Obj.GetComponent<TextMeshPro>();
        textExpAngle = textExpAngle_Obj.GetComponent<TextMeshPro>();
    }

    void Update()
    {
        if (isInPolar1)
        {
            points1[1] = pltf.position;
            DrawLinePolar1();
            CalcPolarCoord1();
            MaxRadiusCheck();
            UpdatePolarText1();
        }
        else if (isInPolar2)
        {
            DrawLinePolar2();
            UpdatePolarText2();
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
                textGroup.SetActive(true);
                textExpSign_Obj.SetActive(false);
                rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                lr1.positionCount = 2;
                points1[0] = pltf.position;
            }
            else
            {
                // In Polar mode: exit Polar mode and reset all Polar mode related stuff
                ResetAll();
            }
        }
    }

    public void PolarDirection(InputAction.CallbackContext context)
    {
        if (context.performed && polarRadius > minRadius)
        {
            if (isInPolar1)
            {
                isInPolar1 = false;
                isInPolar2 = true;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                directionInput = context.ReadValue<float>();
                
                bool up = directionInput >= 0;
                StartCoroutine(PolarRotate(up));
            }
        }
    }



    // Coroutines
    IEnumerator PolarRotate(bool up)
    {
        // Increment polar angle by angleIncrement every timeIncrement
        // so that the player rotates by 2 * Pi radians per period
        // note that period / timeIncrement = number of angle increments per period

        float timeIncrement = 1f / timeIncrementPerSecond;
        float period = periodPerRadius * polarRadius;
        float angleIncrement = 2f * PI * (timeIncrement / period);  // In radians

        if (polarRadiusSigned >= 0)
        {
            polarAngle = 0f;
            if (!up)
            {
                // clockwise
                angleIncrement = -angleIncrement;
            }
        }
        else
        {
            polarAngle = PI;
            if (up)
            {
                // clockwise
                angleIncrement = -angleIncrement;
            }
        }

        int counter = 0;
        int maxCounter = (int)(period * timeIncrementPerSecond + 1);
        Vector2 origin = points1[0];
        points2.Add(points1[1]);

        while(true)
        {
            yield return new WaitForSeconds(timeIncrement);
            if (!isInPolar2)
            {
                yield break;
            }
            polarAngle += angleIncrement;
            Vector2 posOnUnitCirle = new Vector3(Mathf.Cos(polarAngle), Mathf.Sin(polarAngle), pltf.position.z);
            Vector2 pos = origin + polarRadius * posOnUnitCirle;
            pltf.position = pos;
            if (counter <= maxCounter)
            {
                points2.Add(pos);
                counter++;
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
        return isInPolar1 || isInPolar2;
    }

    private void CalcPolarCoord1()
    {
        float x0 = points1[0].x;
        float x1 = points1[1].x;
        polarRadiusSigned = x1 - x0;
        polarRadius = Mathf.Abs(polarRadiusSigned);
        if (polarRadiusSigned >= 0f)
        {
            polarAngle = 0f;
        } else
        {
            polarAngle = PI;
        }
    }

    private void MaxRadiusCheck()
    {
        if (polarRadius > maxRadius)
        {
            if (polarRadiusSigned > maxRadius)
            {
                pltf.position = new Vector3(points1[0].x + maxRadius, pltf.position.y, pltf.position.z);
            }
            else if (polarRadiusSigned < -maxRadius)
            {
                pltf.position = new Vector3(points1[0].x - maxRadius, pltf.position.y, pltf.position.z);
            }
        }
    }

    private void DrawLinePolar1()
    {
        lr1.SetPosition(0, points1[0]);
        lr1.SetPosition(1, points1[1]);
    }

    private void DrawLinePolar2()
    {
        lr2.positionCount = points2.Count;
        for (int i = 0; i < lr2.positionCount; i++)
        {
            lr2.SetPosition(i, points2[i]);
        }
    }

    private void UpdatePolarText1()
    {
        textBaseRadius.text = polarRadius.ToString("F1");
        if (polarRadiusSigned >= 0f)
        {
            textExpAngle.text = "0";
        }
        else
        {
            textExpAngle.text = "π";
        }
    }

    private void UpdatePolarText2()
    {
        textBaseRadius.text = polarRadius.ToString("F1");
        textExpAngle.text = Mathf.Abs(polarAngle / PI).ToString("F1") + "π";
        textExpSign_Obj.SetActive(polarAngle < 0);
    }

    private void ResetAll()
    {
        textGroup.SetActive(false);
        isInPolar1 = false;
        isInPolar2 = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        lr1.positionCount = 0;
        lr2.positionCount = 0;
        points2.Clear();
        polarRadiusSigned = 0;
        polarRadius = 0;
        polarAngle = 0;
    }
}
