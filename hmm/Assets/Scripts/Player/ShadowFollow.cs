using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFollow : MonoBehaviour
{
    [SerializeField] private float yPos;
    [SerializeField] private float maxHeight;
    [SerializeField] private float startOpacity;
    [SerializeField] private GameObject player;
    [SerializeField] private float playerYPos;
    [SerializeField] private float changeOpacitySpeed;

    private float opacity;
    private float t = 0;

    private float xScale;
    private float yScale;
    private float diff;

    private void Awake()
    {
        SetOpacity(opacity / 255f);

        xScale = transform.localScale.x;
        yScale = transform.localScale.y;

        diff = playerYPos - yPos;
    }

    // Update is called once per frame
    void Update()
    {
        //x position
        float xPos = player.transform.position.x;

        transform.position = new Vector2(xPos, yPos);

        //y position
        float h = Mathf.Clamp(player.transform.position.y - diff, yPos, maxHeight);

        float percent = 1 - (h - yPos) / (maxHeight - yPos);

        transform.localScale = new Vector2(xScale * percent, yScale * percent);

        //opacity
        t += Time.deltaTime * changeOpacitySpeed;

        SetOpacity(Mathf.Sin(t) / 30f + (startOpacity / 255) - 0.1f);
    }

    private void SetOpacity(float value)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, value);
    }
}
