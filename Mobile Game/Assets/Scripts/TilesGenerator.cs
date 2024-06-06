using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TilesGenerator : MonoBehaviour
{
    //static instance
    public static TilesGenerator instance;

    public GameData Data;

    //tile related references
    [Header("Tiles")]
    [SerializeField] private GameObject[] tiles = new GameObject[3];
    [SerializeField] private float tileScaleDuration = 1f;
    [SerializeField] private float tileScaleMult = 1f;
    private Vector2 tileScale;
    [SerializeField] private Transform groupParent;
    [SerializeField] private Transform mainGrid;
    [SerializeField] private float timeBetweenSpawningTiles;
    [SerializeField] private float groupDragDuration;
    [Space]

    //respawning the tile
    [Header("Respawn System")]
    public Vector2[] shapes = new Vector2[3];
    private bool[] shapeBools = new bool[3];
    [Space]

    [Header("Dragging")]
    [HideInInspector] public GameObject[] groups = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        tileScale = Data.mainGridScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            RespawnShape();
            RespawnShape();
            RespawnShape();
        }

        if (shapeBools[0] && groups[0] == null)
        {
            shapeBools[0] = false;
        }
        if (shapeBools[1] && groups[1] == null)
        {
            shapeBools[1] = false;
        }
        if (shapeBools[2] && groups[2] == null)
        {
            shapeBools[2] = false;
        }
    }

    #region Respawning Tiles

    private void RespawnShape()
    {
        if (!shapeBools[0])
        {
            GameObject g = CreateShape(shapes[0]);

            shapeBools[0] = true;
            groups[0] = g;
        }
        else
        {
            if (!shapeBools[1])
            {
                GameObject g = CreateShape(shapes[1]);

                shapeBools[1] = true;
                groups[1] = g;
            }
            else
            {
                if (!shapeBools[2])
                {
                    GameObject g = CreateShape(shapes[2]);

                    shapeBools[2] = true;
                    groups[2] = g;
                }
            }
        }
    }

    #endregion

    #region Creating Stuff

    private GameObject CreateShape(Vector2 pos)
    {
        List<Tile> tileList = new List<Tile>();

        GameObject group = new GameObject();
        group.AddComponent<BoxCollider2D>();
        group.layer = LayerMask.NameToLayer("Group");

        int num = (Random.Range(0, 4) < 2) ? 3 : ((Random.Range(0, 2) == 0) ? 2 : 4);

        CreateShapeRecursiveStart(tileList, pos, num, group, pos);

        StartCoroutine(CreateShapeRecursiveFinish(tileList, group.transform));

        return group;
    }

    private void CreateShapeRecursiveStart(List<Tile> tileList, Vector2 currentPos, int num, GameObject group, Vector2 finalPos)
    {
        //if the making of the shape is done
        if (num <= 0)
        {
            //making the group
            group.name = "group";
            group.transform.parent = groupParent;

            float sumX = 0, sumY = 0;

            for (int i = 0; i < tileList.Count; i++)
            {
                sumX += tileList[i].GetObject().transform.position.x;
                sumY += tileList[i].GetObject().transform.position.y;
            }

            sumX /= tileList.Count;
            sumY /= tileList.Count;

            group.transform.position = new Vector2(sumX, sumY);

            for (int i = 0; i < tileList.Count; i++)
            {
                tileList[i].GetObject().transform.parent = group.transform;
            }

            group.transform.position = finalPos;
            group.transform.rotation = Quaternion.Euler(0, 0, 90);

            return;
        }

        //get a random valid position
        Vector2 desiredPosition = GetAvailablePositions(tileList, currentPos)[Random.Range(0, GetAvailablePositions(tileList, currentPos).Count)];

        //create the tile
        Tile nextTile = CreateTile((ElementType)(Random.Range(1, 4)));
        nextTile.GetObject().SetActive(false);

        nextTile.SetPosition(desiredPosition);

        tileList.Add(nextTile);

        CreateShapeRecursiveStart(tileList, desiredPosition, num - 1, group, finalPos);
    }

    private IEnumerator CreateShapeRecursiveFinish(List<Tile> tileList, Transform group)
    {
        for (int i = 0; i < tileList.Count; i++)
        {
            Tile t = CreateTile(tileList[i].GetType());

            t.GetObject().transform.position = tileList[i].GetObject().transform.position;
            t.GetObject().transform.rotation = tileList[i].GetObject().transform.rotation;

            t.GetObject().transform.parent = group;

            yield return new WaitForSecondsRealtime(timeBetweenSpawningTiles);
        }

        yield return new WaitForSecondsRealtime(0.1f);

        for (int i = 0; i < tileList.Count; i++)
        {
            Destroy(tileList[i].GetObject());
        }
    }

    private Tile CreateTile(ElementType type)
    {
        GameObject newTile = Instantiate(tiles[(int)type - 1]);
        Tile t = new Tile(type, newTile);

        StartCoroutine(CreateTileCoroutine(t, type));

        return t;
    }

    private IEnumerator CreateTileCoroutine(Tile t, ElementType type)
    {
        StartCoroutine(t.SetScale(tileScale * tileScaleMult, tileScaleDuration));

        yield return new WaitForSecondsRealtime(tileScaleDuration);

        StartCoroutine(t.SetScale(tileScale, tileScaleDuration / tileScaleMult));
    }

    #endregion

    #region Minor Stuff

    private List<Vector2> GetAvailablePositions(List<Tile> tileList, Vector3 pos)
    {
        List<Vector2> validPositions = new List<Vector2>();

        //right
        if (!IsThePositionTaken(tileList, new Vector3(pos.x + Data.mainSpaceBetweenGridsX, pos.y, pos.z)))
        {
            validPositions.Add(new Vector3(pos.x + Data.mainSpaceBetweenGridsX, pos.y, pos.z));
        }

        //left
        if (!IsThePositionTaken(tileList, new Vector3(pos.x - Data.mainSpaceBetweenGridsX, pos.y, pos.z)))
        {
            validPositions.Add(new Vector3(pos.x - Data.mainSpaceBetweenGridsX, pos.y, pos.z));
        }

        //up-right
        if (!IsThePositionTaken(tileList, new Vector3(pos.x + 0.5f * Data.mainSpaceBetweenGridsX, pos.y + Data.mainSpaceBetweenGridsY, pos.z)))
        {
            validPositions.Add(new Vector3(pos.x + 0.5f * Data.mainSpaceBetweenGridsX, pos.y + Data.mainSpaceBetweenGridsY, pos.z));
        }

        //up-left
        if (!IsThePositionTaken(tileList, new Vector3(pos.x - 0.5f * Data.mainSpaceBetweenGridsX, pos.y + Data.mainSpaceBetweenGridsY, pos.z)))
        {
            validPositions.Add(new Vector3(pos.x - 0.5f * Data.mainSpaceBetweenGridsX, pos.y + Data.mainSpaceBetweenGridsY, pos.z));
        }

        //down-right
        if (!IsThePositionTaken(tileList, new Vector3(pos.x + 0.5f * Data.mainSpaceBetweenGridsX, pos.y - Data.mainSpaceBetweenGridsY, pos.z)))
        {
            validPositions.Add(new Vector3(pos.x + 0.5f * Data.mainSpaceBetweenGridsX, pos.y - Data.mainSpaceBetweenGridsY, pos.z));
        }

        //down-left
        if (!IsThePositionTaken(tileList, new Vector3(pos.x - 0.5f * Data.mainSpaceBetweenGridsX, pos.y - Data.mainSpaceBetweenGridsY, pos.z)))
        {
            validPositions.Add(new Vector3(pos.x - 0.5f * Data.mainSpaceBetweenGridsX, pos.y - Data.mainSpaceBetweenGridsY, pos.z));
        }

        return validPositions;
    }

    private bool IsThePositionTaken(List<Tile> tileList, Vector3 pos)
    {
        for (int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i].GetObject().transform.position == pos)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}

public class Tile
{
    private ElementType type;
    private GameObject obj;

    public Tile(ElementType type, GameObject obj)
    {
        this.obj = obj;
        this.obj.transform.position = Vector2.zero;
        this.obj.transform.localScale = Vector2.zero;

        this.type = type;
    }

    #region Set & Get

    public GameObject GetObject()
    {
        return obj;
    }

    public new ElementType GetType()
    {
        return type;
    }

    #endregion

    #region IEnumerators

    public void SetPosition(Vector2 position)
    {
        if (obj)
        {
            obj.transform.position = position;
        }
    }

    public IEnumerator SetScale(Vector2 scale, float duration)
    {
        if (obj)
        {
            float t = 0;
            Vector2 startScale = obj.transform.localScale;

            while (t < duration)
            {
                t += Time.deltaTime;
                float percent = Mathf.Clamp01(t / duration);
                percent = Mathf.SmoothStep(0, 1, percent);

                obj.transform.localScale = Vector2.Lerp(startScale, scale, percent);

                yield return null;
            }

            obj.transform.localScale = scale;
        }
    }

    #endregion
}