using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTurn : MonoBehaviour
{
    [Header("Sides:")]
    [SerializeField] private GameObject box;
    [SerializeField] private GameObject up;
    [SerializeField] private GameObject down;
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject left;
    [Space(5)]

    [SerializeField] private float max = 1.15f;
    [SerializeField] private float min = 0.82f;
    private float rotation;

    // Start is called before the first frame update
    void Start()
    {
        rotation = transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        rotation = transform.rotation.eulerAngles.z;

        TurnUp();
        TurnDown();
        TurnRight();
        TurnLeft();
    }

    private void TurnUp()
    {
        if (rotation <= 90 && rotation >= 0)
        {
            float percent = (90 - Mathf.Abs(rotation)) / 90;

            up.transform.localPosition = new Vector2(0, (max - min) * percent + min);
        }
        else if (rotation <= 360 && rotation >= 270)
        {
            float percent = (Mathf.Abs(rotation) - 270) / 90;

            up.transform.localPosition = new Vector2(0, (max - min) * percent + min);
        }
        else if (up.transform.localPosition.y != 0)
        {
            up.transform.localPosition = Vector2.zero;
        }
    }

    private void TurnDown()
    {
        if (rotation >= 180 && rotation <= 270)
        {
            float percent = (270 - Mathf.Abs(rotation)) / 90;

            down.transform.localPosition = new Vector2(0, -((max - min) * percent + min));
        }
        else if (rotation <= 180 && rotation >= 90)
        {
            float percent = (Mathf.Abs(rotation) - 90) / 90;

            down.transform.localPosition = new Vector2(0, -((max - min) * percent + min));
        }
        else if (down.transform.localPosition.y != 0)
        {
            down.transform.localPosition = Vector2.zero;
        }
    }

    private void TurnRight()
    {
        if (rotation <= 90 && rotation >= 0)
        {
            float percent = Mathf.Abs(rotation) / 90;

            right.transform.localPosition = new Vector2((max - min) * percent + min, 0);
        }
        else if (rotation <= 180 && rotation >= 90)
        {
            float percent = (180 - Mathf.Abs(rotation)) / 90;

            right.transform.localPosition = new Vector2((max - min) * percent + min, 0);
        }
        else if (right.transform.localPosition.x != 0)
        {
            right.transform.localPosition = Vector2.zero;
        }
    }

    private void TurnLeft()
    {
        if (rotation <= 360 && rotation >= 270)
        {
            float percent = (360 - Mathf.Abs(rotation)) / 90;

            left.transform.localPosition = new Vector2(-((max - min) * percent + min), 0);
        }
        else if (rotation <= 270 && rotation >= 180)
        {
            float percent = (Mathf.Abs(rotation) - 180) / 90;

            left.transform.localPosition = new Vector2(-((max - min) * percent + min), 0);
        }
        else if (left.transform.localPosition.x != 0)
        {
            left.transform.localPosition = Vector2.zero;
        }
    }
}
