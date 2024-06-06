using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGrid : MonoBehaviour
{
    [Header("Grid")]
    public GameData Data;
    [HideInInspector] public GameObject[] grids;
    [SerializeField] private GameObject normalGrid;
    int gridNum;
    

    // Start is called before the first frame update
    void Start()
    {
        gridNum = 0;
        grids = new GameObject[61];

        PlaceGrids();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Making The Grid

    private void PlaceGrids()
    {
        PlaceGridLine(5, 4 * Data.mainSpaceBetweenGridsY);
        PlaceGridLine(6, 3 * Data.mainSpaceBetweenGridsY);
        PlaceGridLine(7, 2 * Data.mainSpaceBetweenGridsY);
        PlaceGridLine(8, Data.mainSpaceBetweenGridsY);
        PlaceGridLine(9, 0);
        PlaceGridLine(8, -Data.mainSpaceBetweenGridsY);
        PlaceGridLine(7, -2 * Data.mainSpaceBetweenGridsY);
        PlaceGridLine(6, -3 * Data.mainSpaceBetweenGridsY);
        PlaceGridLine(5, -4 * Data.mainSpaceBetweenGridsY);
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
                grid1.transform.localScale = Data.mainGridScale;
                grid1.transform.localPosition = new Vector2((i + 0.5f) * Data.mainSpaceBetweenGridsX, yPos);
                grid1.name = gridNum.ToString();
                grid1.layer = LayerMask.NameToLayer("MainGrid");
                grid1.transform.position = new Vector3(grid1.transform.position.x, grid1.transform.position.y, 1);
                grids[gridNum] = grid1;
                gridNum++;

                grid2.transform.parent = transform;
                grid2.transform.localScale = Data.mainGridScale;
                grid2.transform.localPosition = new Vector2(-1 * (i + 0.5f) * Data.mainSpaceBetweenGridsX, yPos);
                grid2.name = gridNum.ToString();
                grid2.layer = LayerMask.NameToLayer("MainGrid");
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
            grid.transform.localScale = Data.mainGridScale;

            grids[gridNum] = grid;
            grid.name = gridNum.ToString();
            grid.layer = LayerMask.NameToLayer("MainGrid");
            gridNum++;

            for (int i = 0; i < num / 2; i++)
            {
                GameObject grid1 = Instantiate(normalGrid);
                GameObject grid2 = Instantiate(normalGrid);

                grid1.transform.parent = transform;
                grid1.transform.localScale = Data.mainGridScale;
                grid1.transform.localPosition = new Vector2((i + 1) * Data.mainSpaceBetweenGridsX, yPos);
                grid1.name = gridNum.ToString();
                grid1.layer = LayerMask.NameToLayer("MainGrid");
                grid1.transform.position = new Vector3(grid1.transform.position.x, grid1.transform.position.y, 1);
                grids[gridNum] = grid1;
                gridNum++;

                grid2.transform.parent = transform;
                grid2.transform.localScale = Data.mainGridScale;
                grid2.transform.localPosition = new Vector2(-1 * (i + 1) * Data.mainSpaceBetweenGridsX, yPos);
                grid2.name = gridNum.ToString();
                grid2.layer = LayerMask.NameToLayer("MainGrid");
                grid2.transform.position = new Vector3(grid2.transform.position.x, grid2.transform.position.y, 1);
                grids[gridNum] = grid2;
                gridNum++;
            }
        }
    }

    #endregion
}
