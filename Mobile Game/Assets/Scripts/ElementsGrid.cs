using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using Unity.VisualScripting;
using UnityEngine;

public class ElementsGrid : MonoBehaviour
{
    [Header("Grid")]
    public GameData Data;
    private GameObject[] grids;
    [SerializeField] private GameObject normalGrid;
    int gridNum;
    [Space]

    [Header("Elements")]
    [SerializeField] private GameObject[] prefubs;
    private Element[] elements;
    [Space]

    [Header("Stuff")]
    [SerializeField] private Camera cam;
    private Vector2 screenPos;
    private Vector3 worldPos;
    private Element currentlyDragging;
    private List<int> availableMoves = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        gridNum = 0;

        grids = new GameObject[7];
        elements = new Element[7];

        PlaceGrids();
        SpawnSingleElement(ElementType.Fire, 1, 0);
        SpawnSingleElement(ElementType.Earth, 1, 2);
        SpawnSingleElement(ElementType.Water, 1, 1);
        SpawnSingleElement(ElementType.Water, 1, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (!cam)
        {
            cam = Camera.current;
            return;
        }

        //get his mouse position
        Vector3 mousePos = Input.mousePosition;
        screenPos = new Vector2(mousePos.x, mousePos.y);

        worldPos = cam.ScreenToWorldPoint(screenPos);

        //cast the raycast
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100, LayerMask.GetMask("Grid"));

        int hitGridNum;

        //if we aim at a grid
        if (hit)
        {
            hitGridNum = LookupGridIndex(hit.transform.gameObject);

            //if we press down the mouse
            if (Input.GetMouseButtonDown(0) && elements[hitGridNum])
            {
                //take the element as a reference
                currentlyDragging = elements[hitGridNum];
                currentlyDragging.transform.position = new Vector3(currentlyDragging.transform.position.x, currentlyDragging.transform.position.y, -1);
                currentlyDragging.SetScale(Data.originalElementsScale * Data.scaleMult);

                //get the available moves
                availableMoves = currentlyDragging.GetAvailableMoves();
            }

            //if we release it
            if (Input.GetMouseButtonUp(0) && currentlyDragging)
            {
                int previousGrid = currentlyDragging.currentGrid;
                currentlyDragging.SetScale(Data.originalElementsScale);

                //if the move is not valid
                if (!MoveTo(currentlyDragging, hitGridNum))
                {
                    currentlyDragging.SetPosition(GetGridCenter(previousGrid));
                    currentlyDragging = null;
                }
                else
                {
                    currentlyDragging = null;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0) && currentlyDragging)
            {
                currentlyDragging.SetPosition(GetGridCenter(currentlyDragging.currentGrid));
                currentlyDragging.SetScale(Data.originalElementsScale);
                currentlyDragging = null;
            }
        }

        //if we are dragging something
        if (Input.GetMouseButton(0) && currentlyDragging)
        {
            currentlyDragging.transform.position = Vector3.Lerp(currentlyDragging.transform.position, new Vector3(worldPos.x, worldPos.y, currentlyDragging.transform.position.z), Time.deltaTime * 10);
        }
    }

    #region Making The Grid

    private void PlaceGrids()
    {
        PlaceGridLine(2, Data.elementalSpaceBetweenGridsY);
        PlaceGridLine(3, 0);
        PlaceGridLine(2, -Data.elementalSpaceBetweenGridsY);
    }

    private void PlaceGridLine(int num, float yPos)
    {
        //if the number is even
        if (num % 2 == 0)
        {
            for (int i = 0; i < num / 2; i++)
            {
                GameObject grid1 = Instantiate(normalGrid);
                GameObject grid2 = Instantiate(normalGrid);

                grid1.transform.parent = transform;
                grid1.transform.localScale = Data.elementalGridScale;
                grid1.transform.localPosition = new Vector2((i + 0.5f) * Data.elementalSpaceBetweenGridsX, yPos);
                grid1.name = gridNum.ToString();
                grid1.layer = LayerMask.NameToLayer("Grid");
                grid1.transform.position = new Vector3(grid1.transform.position.x, grid1.transform.position.y, 1);
                grids[gridNum] = grid1;
                gridNum++;

                grid2.transform.parent = transform;
                grid2.transform.localScale = Data.elementalGridScale;
                grid2.transform.localPosition = new Vector2(-1 * (i + 0.5f) * Data.elementalSpaceBetweenGridsX, yPos);
                grid2.name = gridNum.ToString();
                grid2.layer = LayerMask.NameToLayer("Grid");
                grid2.transform.position = new Vector3(grid2.transform.position.x, grid2.transform.position.y, 1);
                grids[gridNum] = grid2;
                gridNum++;
            }
        }
        //if it is odd
        else
        {
            GameObject grid = Instantiate(normalGrid);
            grid.transform.localPosition = new Vector2(0, yPos);
            grid.transform.parent = transform;
            grid.transform.localPosition = new Vector3(grid.transform.position.x, grid.transform.position.y, 1);
            grid.transform.localScale = Data.elementalGridScale;

            grids[gridNum] = grid;
            grid.name = gridNum.ToString();
            grid.layer = LayerMask.NameToLayer("Grid");
            gridNum++;

            for (int i = 0; i < num / 2; i++)
            {
                GameObject grid1 = Instantiate(normalGrid);
                GameObject grid2 = Instantiate(normalGrid);

                grid1.transform.parent = transform;
                grid1.transform.localScale = Data.elementalGridScale;
                grid1.transform.localPosition = new Vector2((i + 1) * Data.elementalSpaceBetweenGridsX, yPos);
                grid1.name = gridNum.ToString();
                grid1.layer = LayerMask.NameToLayer("Grid");
                grid1.transform.position = new Vector3(grid1.transform.position.x, grid1.transform.position.y, 1);
                grids[gridNum] = grid1;
                gridNum++;

                grid2.transform.parent = transform;
                grid2.transform.localScale = Data.elementalGridScale;
                grid2.transform.localPosition = new Vector2(-1 * (i + 1) * Data.elementalSpaceBetweenGridsX, yPos);
                grid2.name = gridNum.ToString();
                grid2.layer = LayerMask.NameToLayer("Grid");
                grid2.transform.position = new Vector3(grid2.transform.position.x, grid2.transform.position.y, 1);
                grids[gridNum] = grid2;
                gridNum++;
            }
        }
    }

    #endregion

    #region Basic Mechanics

    private Element SpawnSingleElement(ElementType type, int level, int gridNum)
    {
        GameObject newElement = Instantiate(prefubs[(int)type - 1], transform);
        Element el = newElement.GetComponent<Element>();

        elements[gridNum] = el;

        el.type = type;
        el.level = level;
        el.currentGrid = gridNum;

        el.transform.localScale = Data.originalElementsScale;

        el.SetPosition(GetGridCenter(gridNum), true);

        return el;
    }

    private Vector2 GetGridCenter(int gridNum)
    {
        GameObject flag = new GameObject();

        flag.transform.parent = grids[gridNum].transform;
        flag.transform.localPosition = Vector2.zero;

        Vector2 center = flag.transform.position;

        Destroy(flag, 0.1f);

        return center;
    }

    private bool MoveTo(Element el, int gridNum, bool force = false)
    {
        //if the move isn't valid
        if (!ContainsValidMove(ref availableMoves, gridNum))
        {
            return false;
        }

        Element flag = elements[gridNum];

        //if there is an element, replace it (or merge)
        if (flag)
        {
            if (flag.type == el.type && flag.level == el.level && flag.level < 3)
            {
                Destroy(flag.transform.gameObject);

                elements[el.currentGrid] = null;
                elements[gridNum] = el;
                el.currentGrid = gridNum;

                //move the element to the desired location
                el.SetPosition(GetGridCenter(gridNum), force);
                el.LevelUp();

                return true;
            }

            flag.SetPosition(GetGridCenter(el.currentGrid), force);
        }

        //move the element to the desired location
        el.SetPosition(GetGridCenter(gridNum), force);

        elements[gridNum] = el;
        elements[el.currentGrid] = flag;

        int previousGrid = el.currentGrid;
        el.currentGrid = gridNum;

        if (flag)
        {
            flag.currentGrid = previousGrid;
        }

        return true;
    }

    private bool ContainsValidMove(ref List<int> moves, int grid)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i] == grid)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Minor Stuff

    private int LookupGridIndex(GameObject hitInfo)
    {
        if (hitInfo)
        {
            for (int i = 0; i < grids.Length; i++)
            {
                if (grids[i] == hitInfo)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    #endregion
}
