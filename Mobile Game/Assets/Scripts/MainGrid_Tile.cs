using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainGrid_Tile : MonoBehaviour
{
    [HideInInspector] public bool hasTile = false;
    [SerializeField] private float setTileDuration;
    [SerializeField] private float setScaleDuration;
    [SerializeField] private float scaleMult;
    [SerializeField] private Transform colorTransform;
    [SerializeField] private Color highlightColor;
    private Color regularColor;
    public GameObject ownTile;

    // Start is called before the first frame update
    void Start()
    {
        regularColor = colorTransform.GetComponent<RegularPolygon>().Color;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator SetTile(GameObject tile)
    {
        float t = 0;
        Vector2 startPos = tile.transform.position;

        while (t < setTileDuration)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / setTileDuration);
            percent = Mathf.SmoothStep(0, 1, percent);

            tile.transform.position = Vector3.Lerp(startPos, transform.position, percent);

            yield return null;
        }

        tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -2);
    }

    public IEnumerator SetTileAnimation()
    {
        GameObject tile = ownTile;

        float t = 0;
        Vector2 startScale = tile.transform.localScale;

        while (t < setScaleDuration)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / setScaleDuration);
            percent = Mathf.SmoothStep(0, 1, percent);

            tile.transform.localScale = Vector3.Lerp(startScale, startScale * scaleMult, percent);

            yield return null;
        }

        t = 0;

        while (t < setScaleDuration)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / setScaleDuration);
            percent = Mathf.SmoothStep(0, 1, percent);

            tile.transform.localScale = Vector3.Lerp(startScale * scaleMult, startScale, percent);

            yield return null;
        }

        tile.transform.localScale = startScale;
    }

    public void HighlightColor(bool flag)
    {
        if (flag)
        {
            colorTransform.GetComponent<RegularPolygon>().Color = highlightColor;
        }
        else
        {
            colorTransform.GetComponent<RegularPolygon>().Color = regularColor;
        }
    }
}