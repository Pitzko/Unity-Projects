using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class GroupDrag : MonoBehaviour
{
    [SerializeField] private TilesGenerator tg;
    [SerializeField] private MainGrid mg;

    [SerializeField] private float yOffset;
    [SerializeField] private Camera cam;
    [SerializeField] private float groupMoveDuration;
    [SerializeField] private float tileSetSpaces;

    private Vector2 screenPos;
    private Vector3 worldPos;
    private int currentlyDragging;
    private int currentlyHovering = 0;
    private List<GameObject> highlighted = new List<GameObject>();

    //highlight
    private Vector2 tilePos;

    // Start is called before the first frame update
    void Start()
    {
        currentlyDragging = -1;
    }

    // Update is called once per frame
    void Update()
    {
        //get his mouse position
        Vector3 mousePos = Input.mousePosition;
        screenPos = new Vector2(mousePos.x, mousePos.y);

        worldPos = cam.ScreenToWorldPoint(screenPos);

        //cast the raycast
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100, LayerMask.GetMask("Group"));

        //Debug.Log(worldPos);

        if (hit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentlyDragging = LookUpIndexGroup(hit.transform.gameObject);
            }
        }

        if (Input.GetMouseButtonUp(0) && currentlyDragging != -1)
        {
            if (IsValidMove(tg.groups[currentlyDragging]))
            {
                StartCoroutine(SetTiles(tg.groups[currentlyDragging]));
            }
            else
            {
                StartCoroutine(MoveTo(tg.groups[currentlyDragging], tg.shapes[currentlyDragging], groupMoveDuration));
            }

            for (int i = 0; i < mg.grids.Length; i++)
            {
                mg.grids[i].GetComponent<MainGrid_Tile>().HighlightColor(false);
            }

            currentlyHovering = 0;

            currentlyDragging = -1;
        }

        if (Input.GetMouseButton(0) && currentlyDragging != -1)
        {
            tg.groups[currentlyDragging].transform.position = new Vector3(tg.groups[currentlyDragging].transform.position.x, tg.groups[currentlyDragging].transform.position.y, -4);

            tg.groups[currentlyDragging].transform.position = Vector3.Lerp(tg.groups[currentlyDragging].transform.position, new Vector3(worldPos.x, worldPos.y + yOffset, 0), Time.deltaTime * 20);

            highlighted.Clear();

            //highlights
            for (int i = 0; i < tg.groups[currentlyDragging].transform.childCount; i++)
            {
                //cast the raycast
                RaycastHit2D hitMainGrid = Physics2D.Raycast(tg.groups[currentlyDragging].transform.GetChild(i).position, Vector2.zero, 100, LayerMask.GetMask("MainGrid", "HoverMainGrid"));

                //if a tile is above one of the main grids
                if (hitMainGrid)
                {
                    highlighted.Add(hitMainGrid.transform.gameObject);
                }
            }

            currentlyHovering = HighlightTiles(highlighted);
        }
        else
        {
            if (currentlyDragging != -1 && tg.groups[currentlyDragging].transform.position != new Vector3(tg.groups[currentlyDragging].transform.position.x, tg.groups[currentlyDragging].transform.position.y, -2))
            {
                tg.groups[currentlyDragging].transform.position = new Vector3(tg.groups[currentlyDragging].transform.position.x, tg.groups[currentlyDragging].transform.position.y, -2);
            }
        }
    }

    private int LookUpIndexGroup(GameObject obj)
    {
        GameObject[] g = tg.groups;

        for (int i = 0; i < g.Length; i++)
        {
            if (g[i] == obj)
            {
                return i;
            }
        }

        Debug.Log("Group Not Found");

        return -1;
    }

    private bool IsValidMove(GameObject obj)
    {
        if (currentlyHovering == obj.transform.childCount)
        {
            return true;
        }

        return false;
    }

    private IEnumerator MoveTo(GameObject obj ,Vector2 endPos, float time)
    {
        float t = 0;
        Vector3 startPos = obj.transform.position;

        while (t < time)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / time);
            percent = Mathf.SmoothStep(0, 1, percent);

            obj.transform.position = Vector3.Lerp(startPos, endPos, percent);

            yield return null;
        }

        obj.transform.position = endPos;
    }

    private int HighlightTiles(List<GameObject> list)
    {
        int num = 0;

        for (int i = 0; i < mg.grids.Length; i++)
        {
            bool flag = false;

            for (int k = 0; k < list.Count; k++)
            {
                if (mg.grids[i] == list[k])
                {
                    flag = true;
                }
            }

            if (flag && mg.grids[i].GetComponent<MainGrid_Tile>().ownTile == null)
            {
                mg.grids[i].layer = LayerMask.NameToLayer("HoverMainGrid");
                mg.grids[i].GetComponent<MainGrid_Tile>().HighlightColor(true);

                num++;
            }
            else
            {
                mg.grids[i].layer = LayerMask.NameToLayer("MainGrid");
                mg.grids[i].GetComponent<MainGrid_Tile>().HighlightColor(false);
            }
        }

        return num;
    }

    private IEnumerator SetTiles(GameObject obj)
    {
        GameObject parent = obj.transform.GetChild(0).parent.gameObject;

        List<GameObject> scaleList = new List<GameObject>();

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            StartCoroutine(highlighted[i].GetComponent<MainGrid_Tile>().SetTile(obj.transform.GetChild(i).gameObject));

            highlighted[i].GetComponent<MainGrid_Tile>().ownTile = obj.transform.GetChild(i).gameObject;

            scaleList.Add(obj.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i).parent = null;

            i--;
        }

        for (int i = 0; i < scaleList.Count; i++)
        {
            StartCoroutine(highlighted[i].GetComponent<MainGrid_Tile>().SetTileAnimation());

            yield return new WaitForSecondsRealtime(tileSetSpaces);
        }

        highlighted.Clear();

        yield return new WaitForSecondsRealtime(0.01f);

        Destroy(parent);
    }
}