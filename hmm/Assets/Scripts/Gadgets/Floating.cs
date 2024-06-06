using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Floating : MonoBehaviour
{
    [Header("Y")]
    [SerializeField] private float yOffset;
    [SerializeField] private float yFloatSpeed;
    [Space]

    [Header("X")]
    [SerializeField] private float xOffset;
    [SerializeField] private float xFloatSpeed;
    [Space]

    [SerializeField] private bool pixalize;
    [SerializeField] private int snapValue;
    private float yTime = 0;
    private float xTime = 0;
    private Vector2 startPos;

    private void Start()
    {
        float snappedX = Mathf.Round(transform.position.x * snapValue) / snapValue;
        float snappedY = Mathf.Round(transform.position.y * snapValue) / snapValue;

        transform.position = new Vector2(snappedX, snappedY);
        startPos = transform.position;
    }

    private void Update()
    {
        #region Y

        yTime += Time.deltaTime * yFloatSpeed;

        float k = Mathf.Lerp(-yOffset, yOffset, Mathf.Sin(yTime) / 2 + 0.5f);

        float yValue = startPos.y + k;

        if (pixalize)
        {
            yValue = Mathf.Round(yValue * snapValue) / snapValue;
        }

        transform.position = new Vector2(transform.position.x, yValue);

        #endregion

        #region X

        xTime += Time.deltaTime * xFloatSpeed;

        float t = Mathf.Lerp(-xOffset, xOffset, Mathf.Sin(xTime) / 2 + 0.5f);

        float xValue = startPos.x + t;

        if (pixalize)
        {
            xValue = Mathf.Round(xValue * snapValue) / snapValue;
        }

        transform.position = new Vector2(xValue, transform.position.y);

        #endregion
    }
}
