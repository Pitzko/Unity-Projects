using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ElementType
{
    None = 0,
    Fire = 1,
    Water = 2,
    Earth = 3
}

public class Element : MonoBehaviour
{
    //general data
    public int level = 1;
    public int currentGrid;
    public GameData Data;
    public ElementType type;

    [Header("Visuals")]
    //prefubs
    public Sprite[] elements = new Sprite[3];
    [Space]

    [SerializeField] private Color FirstTopColor;
    [SerializeField] private Color FirstBottomColor;
    [Space]
    [SerializeField] private Color SecondTopColor;
    [SerializeField] private Color SecondBottomColor;

    [SerializeField] private float ChangeColorDuration;

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector2.one;

    private void Start()
    {
        transform.GetComponent<SpriteRenderer>().sprite = elements[level - 1];
        transform.GetComponent<SpriteMask>().sprite = elements[level - 1];

        transform.GetComponent<SpriteRenderer>().material.SetColor("_TopColor", FirstTopColor);
        transform.GetComponent<SpriteRenderer>().material.SetColor("_BottomColor", FirstBottomColor);

        StartCoroutine(ChangeColors());
    }

    #region technical stuff

    public List<int> GetAvailableMoves()
    {
        Dictionary<int, int[]> numberRelations = new Dictionary<int, int[]>
        {
            { 0, new int[] { 3, 1, 2 } },
            { 1, new int[] { 0, 4, 2 } },
            { 4, new int[] { 1, 6, 2 } },
            { 6, new int[] { 4, 5, 2 } },
            { 5, new int[] { 6, 3, 2 } },
            { 3, new int[] { 5, 0, 2 } },
            { 2, new int[] { 0, 1, 3, 4, 5, 6 } }
        };

        List<int> availableMoves = new List<int>();

        for (int i = 0; i < numberRelations[currentGrid].Length; i++)
        {
            availableMoves.Add(numberRelations[currentGrid][i]);
        }

        return availableMoves;
    }

    public virtual void SetPosition(Vector2 Position, bool force = false)
    {
        StartCoroutine(SetElementPosition(Position, force));
    }

    public virtual void SetScale(Vector2 scale, bool force = false)
    {
        StartCoroutine(SetElementScale(scale, force));
    }

    public void LevelUp()
    {
        if (level < 3)
        {
            level++;
        }

        transform.GetComponent<SpriteRenderer>().sprite = elements[level - 1];
        transform.GetComponent<SpriteMask>().sprite = elements[level - 1];
    }

    #endregion

    #region visuals

    private IEnumerator ChangeColors()
    {
        float k = 0;

        while (k < 1)
        {
            float t = 0;

            while (t < ChangeColorDuration)
            {
                t += Time.deltaTime;

                float percent = Mathf.Clamp01(t / ChangeColorDuration);
                percent = Mathf.SmoothStep(0, 1, percent);

                Color currentTopColor = Color.Lerp(FirstTopColor, SecondTopColor, percent);
                Color currentBottomColor = Color.Lerp(FirstBottomColor, SecondBottomColor, percent);

                transform.GetComponent<SpriteRenderer>().material.SetColor("_TopColor", currentTopColor);
                transform.GetComponent<SpriteRenderer>().material.SetColor("_BottomColor", currentBottomColor);

                yield return null;
            }

            while (t < 2 * ChangeColorDuration)
            {
                t += Time.deltaTime;

                float percent = Mathf.Clamp01((t - ChangeColorDuration) / ChangeColorDuration);
                percent = Mathf.SmoothStep(0, 1, percent);

                Color currentTopColor = Color.Lerp(SecondTopColor, FirstTopColor, percent);
                Color currentBottomColor = Color.Lerp(SecondBottomColor, FirstBottomColor, percent);

                transform.GetComponent<SpriteRenderer>().material.SetColor("_TopColor", currentTopColor);
                transform.GetComponent<SpriteRenderer>().material.SetColor("_BottomColor", currentBottomColor);

                yield return null;
            }

            yield return null;
        }
    }

    #endregion

    #region IEnumerators

    private IEnumerator SetElementPosition(Vector2 Position, bool force)
    {
        float t = 0;
        desiredPosition = Position;
        Vector3 startPosition = transform.position;

        if (!force)
        {
            while (t < Data.dragDuration)
            {
                t += Time.deltaTime;
                float percent = Mathf.Clamp01(t / Data.dragDuration);
                percent = Mathf.SmoothStep(0, 1, percent);

                transform.position = Vector3.Lerp(startPosition, desiredPosition, percent);

                yield return null;
            }
        }

        transform.position = desiredPosition;
    }

    private IEnumerator SetElementScale(Vector2 scale, bool force)
    {
        float t = 0;
        desiredScale = scale;
        Vector2 startScale = transform.localScale;

        if (!force)
        {
            while (t < Data.scaleDuration)
            {
                t += Time.deltaTime;
                float percent = Mathf.Clamp01(t / Data.scaleDuration);
                percent = Mathf.SmoothStep(0, 1, percent);

                transform.localScale = Vector2.Lerp(startScale, desiredScale, percent);

                yield return null;
            }
        }

        transform.localScale = desiredScale;
    }

    #endregion
}
