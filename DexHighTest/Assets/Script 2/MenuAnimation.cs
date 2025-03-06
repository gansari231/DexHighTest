using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuAnimation : MonoBehaviour
{

    public RectTransform[] objectsToMove;  // Buttons to rotate
    public Transform center;           // Center point
    public float radius = 5f;          // Circle radius
    public float moveSpeed = 2f;           // moving speed
    public float sizeSpeed = 2f;           // size speed
    public float[] startAngles;        // Starting angles
    public float[] stopAngles;         // Stop angles
    public float[] finalSize;          // Final size for each button

    private float[] angles;
    private bool[] isMoving;
    public bool isExpanded = false; // Track animation state
    private float[] originalSize; // Store original sizes

    void Start()
    {
        int count = objectsToMove.Length;
        angles = new float[count];
        isMoving = new bool[count];
        originalSize = new float[count];

        for (int i = 0; i < count; i++)
        {
            if (objectsToMove[i] == null) continue;
            angles[i] = startAngles[i] * Mathf.Deg2Rad;
            isMoving[i] = false;
            originalSize[i] = objectsToMove[i].sizeDelta.x; // Store original size

            UpdateTransform(i);
        }
    }

    void UpdateTransform(int i)
    {
        float x = center.position.x + Mathf.Cos(angles[i]) * radius;
        float y = center.position.y + Mathf.Sin(angles[i]) * radius;
        objectsToMove[i].position = new Vector3(x, y, objectsToMove[i].position.z);

        // Scale based on animation state using RectTransform sizeDelta
        /*float targetSize = isExpanded ? finalSize[i] : originalSize[i];
        float newSize = Mathf.MoveTowards(objectsToMove[i].sizeDelta.x, targetSize, sizeSpeed * Time.deltaTime);

        objectsToMove[i].sizeDelta = new Vector2(newSize, newSize);*/
    }

    public void ToggleAnimation()
    {
        isExpanded = !isExpanded;
        StartCoroutine(MoveAll());
    }

    IEnumerator MoveAll()
    {
        while (true)
        {
            bool anyMoving = false;

            for (int i = 0; i < objectsToMove.Length; i++)
            {
                float targetAngle = isExpanded ? stopAngles[i] : startAngles[i];
                float deltaAngle = Mathf.DeltaAngle(Mathf.Rad2Deg * angles[i], targetAngle);

                if (Mathf.Abs(deltaAngle) < moveSpeed * Time.deltaTime) // More precise stopping condition
                {
                    isMoving[i] = false;
                    angles[i] = targetAngle * Mathf.Deg2Rad;
                }
                else
                {
                    float moveAmount = Mathf.Sign(deltaAngle) * moveSpeed * Time.deltaTime;
                    angles[i] += moveAmount * Mathf.Deg2Rad;
                    isMoving[i] = true;
                }

                UpdateTransform(i);
                anyMoving |= isMoving[i];
            }

            if (!anyMoving) break;
            yield return null;
        }
    }

    public void RotateButtons(bool clockwise)
    {
        StartCoroutine(RotateWithMovement(clockwise));
    }

    IEnumerator RotateWithMovement1(bool clockwise)
    {
        int count = objectsToMove.Length;
        float[] initialAngles = new float[count];
        float[] targetAngles = new float[count];
        //float[] targetSizes = new float[count];

        for (int i = 0; i < count; i++)
        {
            initialAngles[i] = angles[i];
            int nextIndex = clockwise ? (i + 1) % count : (i - 1 + count) % count;
            targetAngles[nextIndex] = initialAngles[i];
            //targetSizes[nextIndex] = objectsToMove[i].sizeDelta.x;
        }

        bool moving = true;
        while (moving)
        {
            moving = false;
            for (int i = 0; i < count; i++)
            {
                float deltaAngle = Mathf.DeltaAngle(Mathf.Rad2Deg * angles[i], Mathf.Rad2Deg * targetAngles[i]);
                if (Mathf.Abs(deltaAngle) > moveSpeed * Time.deltaTime)
                {
                    float moveAmount = Mathf.Sign(deltaAngle) * moveSpeed * Time.deltaTime;
                    angles[i] += moveAmount * Mathf.Deg2Rad;
                    moving = true;
                }
                else
                {
                    angles[i] = targetAngles[i];
                }

                /*float newSize = Mathf.MoveTowards(objectsToMove[i].sizeDelta.x, targetSizes[i], sizeSpeed * Time.deltaTime);
                objectsToMove[i].sizeDelta = new Vector2(newSize, newSize);*/
                UpdateTransform(i);
            }
            yield return null;
        }
    }

    IEnumerator RotateWithMovement(bool clockwise)
    {
        int count = objectsToMove.Length;
        float[] initialAngles = new float[count];
        float[] targetAngles = new float[count];

        // Store initial angles
        for (int i = 0; i < count; i++)
        {
            initialAngles[i] = angles[i];
        }

        // Assign target angles correctly
        for (int i = 0; i < count; i++)
        {
            int nextIndex = clockwise ? (i + 1) % count : (i - 1 + count) % count;
            targetAngles[i] = initialAngles[nextIndex];  // Corrected target assignment
        }

        bool moving = true;
        while (moving)
        {
            moving = false;
            for (int i = 0; i < count; i++)
            {
                float deltaAngle = Mathf.DeltaAngle(Mathf.Rad2Deg * angles[i], Mathf.Rad2Deg * targetAngles[i]);

                if (Mathf.Abs(deltaAngle) > moveSpeed * Time.deltaTime)
                {
                    float moveAmount = Mathf.Sign(deltaAngle) * moveSpeed * Time.deltaTime;
                    angles[i] += moveAmount * Mathf.Deg2Rad;
                    moving = true;
                }
                else
                {
                    angles[i] = targetAngles[i];  // Snap to final angle
                }

                UpdateTransform(i); // Apply transformation
            }
            yield return null;
        }
    }

}
