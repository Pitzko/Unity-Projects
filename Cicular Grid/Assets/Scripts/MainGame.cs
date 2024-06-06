using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    public GameData Data;

    //references
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject mergeFX;

    //important variables
    private List<GameObject> tileListObjects = new List<GameObject>();
    private GameObject spawnedTile;
    private int rearrangingTilesFirstIndex = 0;
    //booleans
    private bool isDragging = false;

    //vectors
    private Vector3 worldPos;

    //coroutines
    private Coroutine dragCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        SpawnGrid(Data.startingNumberOfTiles);
    }

    // Update is called once per frame
    void Update()
    {
        #region Drag & Drop

        //get the mouse position
        Vector2 mousePos = Input.mousePosition;

        worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        //cast the raycast
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100, LayerMask.GetMask("DragZone"));

        //if the mouse is over the zone
        if (hit)
        {
            if (Input.GetMouseButtonDown(0) && spawnedTile)
            {
                dragCoroutine = StartCoroutine(DragTile(spawnedTile));

                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                StopCoroutine(dragCoroutine);

                StartCoroutine(AddTile(spawnedTile, spawnedTile.GetComponent<Tile>().isMerge));

                isDragging = false;

                spawnedTile = null;
            }
        }
        else
        {
            if (isDragging)
            {
                StopCoroutine(dragCoroutine);

                StartCoroutine(MoveTo(spawnedTile, transform.position, Data.addTileMoveDuration));

                isDragging = false;
            }
        }

        #endregion

        #region Rhythm Game

        //cast the raycast
        RaycastHit2D mergeTileHit = Physics2D.Raycast(worldPos, Vector2.zero, 100, LayerMask.GetMask("MergeTile"));

        if (mergeTileHit && mergeTileHit.transform.GetComponent<Tile>().isCharged)
        {
            if (Input.GetMouseButtonUp(0))
            {
                RearrangeTiles(mergeTileHit.transform.gameObject, tileListObjects.Count, 0);

                StartCoroutine(SpawnRythmTiles(mergeTileHit.transform.GetComponent<Tile>().noteType));
            }
        }

        #endregion

        if (Input.GetKeyDown("e"))
        {
            StartCoroutine(SpawnTile());
        }

        if (Input.GetKeyDown("f"))
        {
            StartCoroutine(Delete());
        }
    }

    private IEnumerator Delete()
    {
        for (int i = 0; i < tileListObjects.Count; i++)
        {
            Destroy(tileListObjects[i]);

            yield return new WaitForSecondsRealtime(1);
        }
    }

    #region Rhythm Stuff

    private IEnumerator SpawnRythmTileAnimation(GameObject tile)
    {
        for (int i = 0; i < Data.spawnAnimationTicks; i++)
        {
            yield return new WaitForSecondsRealtime(Data.spawnAnimationTickDuration);

            tile.SetActive(false);

            yield return new WaitForSecondsRealtime(Data.spawnAnimationTickDuration);

            tile.SetActive(true);
        }
    }

    private IEnumerator DespawnRythmTileAnimation(GameObject tile)
    {
        for (int i = 0; i < Data.spawnAnimationTicks; i++)
        {
            yield return new WaitForSecondsRealtime(Data.spawnAnimationTickDuration);

            tile.SetActive(true);

            yield return new WaitForSecondsRealtime(Data.spawnAnimationTickDuration);

            tile.SetActive(false);
        }
    }

    private IEnumerator SpawnRythmTiles(NoteType type)
    {
        int times = tileListObjects.Count / 2 + Random.Range(0, 2);

        List<Vector2> availablePositions = GetAvailablePositions();

        for (int i = 0; i < availablePositions.Count; i++)
        {
            float xValue = availablePositions[i].x - transform.position.x;
            float yValue = availablePositions[i].y - transform.position.y;

            float ratio = Data.radius / Vector2.Distance(transform.position, availablePositions[i]);

            xValue *= ratio;
            yValue *= ratio;

            availablePositions[i] = new Vector2(xValue + transform.position.x, yValue + transform.position.y);
        }

        int[] arr = new int[availablePositions.Count];

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = i;
        }

        for (int i = 0; i < arr.Length; i++)
        {
            int flag = arr[i];
            int num = Random.Range(0, arr.Length);

            arr[i] = arr[num];
            arr[num] = flag;
        }

        int[] timesArr = new int[times];

        for (int i = 0; i < times; i++)
        {
            timesArr[i] = arr[i];
        }

        for (int i = 0; i < times; i++)
        {
            for (int j = 0; j < times - 1; j++)
            {
                if (timesArr[j] > timesArr[j + 1])
                {
                    int flag = timesArr[j];

                    timesArr[j] = timesArr[j + 1];
                    timesArr[j + 1] = flag;
                }
            }
        }

        int k = 0;

        for (int i = 0; i < times; i++)
        {
            SpawnSingleRythmTile(type, availablePositions[timesArr[i]], timesArr[i] + k);
            k++;
        }

        StartCoroutine(DespawnRythmTileAnimation(tileListObjects[0]));

        yield return new WaitForSecondsRealtime(Data.rearrangeDelay);

        Destroy(tileListObjects[0]);

        List<GameObject> newList = new List<GameObject>();

        for (int i = 1; i < tileListObjects.Count; i++)
        {
            newList.Add(tileListObjects[i]);
        }

        tileListObjects.Clear();

        for (int i = 0; i < newList.Count; i++)
        {
            tileListObjects.Add(newList[i]);
        }

        RearrangeTiles(tileListObjects[0], tileListObjects.Count, Data.addTileMoveDuration);
    }

    private void SpawnSingleRythmTile(NoteType type, Vector2 position, int index)
    {
        index++;

        GameObject newTile = Instantiate(Data.Tiles[(int)type]);
        newTile.transform.position = position;

        if (index == tileListObjects.Count)
        {
            tileListObjects.Add(newTile);
        }
        else
        {
            int k = 0;

            List<GameObject> newList = new List<GameObject>();

            while (k < tileListObjects.Count)
            {
                if (k == index)
                {
                    newList.Add(newTile);
                    index = -1;
                }
                else
                {
                    newList.Add(tileListObjects[k]);
                    k++;
                }
            }

            tileListObjects.Clear();

            for (int i = 0; i < newList.Count; i++)
            {
                tileListObjects.Add(newList[i]);
            }
        }

        StartCoroutine(SpawnRythmTileAnimation(newTile));
    }

    #endregion

    #region Important Mechanics

    //private IEnumerator SpinGrid()
    //{

    //}

    private IEnumerator AddTile(GameObject tile, bool isMerging = false)
    {
        Vector2 pos = GetClosestAvailablePosition();

        float distanceRatio = Data.radius / Vector2.Distance(transform.position, pos);

        float xValue = pos.x - transform.position.x;
        float yValue = pos.y - transform.position.y;

        xValue *= distanceRatio;
        yValue *= distanceRatio;

        xValue += transform.position.x;
        yValue += transform.position.y;

        StartCoroutine(MoveTo(tile, new Vector2(xValue, yValue), Data.addTileMoveDuration));

        RearrangeTiles(tile, tileListObjects.Count + 1, Data.addTileMoveDuration);

        yield return new WaitForSecondsRealtime(Data.addTileMoveDuration);

        if (tileListObjects.Count > 2)
        {
            if (tile.GetComponent<Tile>().isMerge)
            {
                if (GetLeftTile(tile).GetComponent<Tile>().noteType == GetRightTile(tile).GetComponent<Tile>().noteType &&
                    GetLeftTile(tile).GetComponent<Tile>().noteType == tile.GetComponent<Tile>().noteType && !GetRightTile(tile).GetComponent<Tile>().isMerge && !GetLeftTile(tile).GetComponent<Tile>().isMerge)
                {
                    StartCoroutine(MergeTiles());
                }
            }
            else
            {
                if (GetRightTile(tile).GetComponent<Tile>().isMerge && GetRightTile(tile).GetComponent<Tile>().noteType == tile.GetComponent<Tile>().noteType &&
                    !GetRightTile(GetRightTile(tile)).GetComponent<Tile>().isMerge && GetRightTile(GetRightTile(tile)).GetComponent<Tile>().noteType == tile.GetComponent<Tile>().noteType)
                {
                    rearrangingTilesFirstIndex = GetTileIndex(GetRightTile(tile));

                    RearrangeTiles(GetRightTile(tile), tileListObjects.Count, 0);

                    StartCoroutine(MergeTiles());
                }
                else
                {
                    if (GetLeftTile(tile).GetComponent<Tile>().isMerge && GetLeftTile(tile).GetComponent<Tile>().noteType == tile.GetComponent<Tile>().noteType &&
                        !GetLeftTile(GetLeftTile(tile)).GetComponent<Tile>().isMerge && GetLeftTile(GetLeftTile(tile)).GetComponent<Tile>().noteType == tile.GetComponent<Tile>().noteType)
                    {
                        rearrangingTilesFirstIndex = GetTileIndex(tile);

                        RearrangeTiles(GetLeftTile(tile), tileListObjects.Count, 0);

                        StartCoroutine(MergeTiles());
                    }
                }
            }
        }
    }

    private void RearrangeTiles(GameObject t, int tileCount, float moveDuration)
    {
        //a new list for the tiles
        List<GameObject> newTileList = new List<GameObject>();

        float angle = 2 * Mathf.PI / (tileCount);
        float currentAngle;

        if (Data.radius < Vector2.Distance(t.transform.position, transform.position) + 0.01f &&
            Data.radius > Vector2.Distance(t.transform.position, transform.position) - 0.01f)
        {
            currentAngle = GetAngle(t.transform.position) + angle;
        }
        else
        {
            currentAngle = GetAngle(GetClosestAvailablePosition()) + angle;
        }

        tileListObjects.Remove(t);
        newTileList.Add(t);

        for (int i = rearrangingTilesFirstIndex; i < tileListObjects.Count; i++)
        {
            if (tileListObjects[i])
            {
                //getting the values for the position
                float xValue = Data.radius * Mathf.Sin(currentAngle);
                float yValue = Data.radius * Mathf.Cos(currentAngle);

                //moving the tile
                StartCoroutine(CircularMoveTo(tileListObjects[i], new Vector2(xValue + transform.position.x, yValue + transform.position.y), moveDuration));

                newTileList.Add(tileListObjects[i]);

                currentAngle += angle;
            }
        }

        for (int i = 0; i < rearrangingTilesFirstIndex; i++)
        {
            if (tileListObjects[i])
            {
                float xValue = Data.radius * Mathf.Sin(currentAngle);
                float yValue = Data.radius * Mathf.Cos(currentAngle);

                //moving the tile
                StartCoroutine(CircularMoveTo(tileListObjects[i], new Vector2(xValue + transform.position.x, yValue + transform.position.y), moveDuration));

                newTileList.Add(tileListObjects[i]);

                currentAngle += angle;
            }
        }

        tileListObjects.Clear();

        for (int i = 0; i < newTileList.Count; i++)
        {
            tileListObjects.Add(newTileList[i]);
        }
    }

    private IEnumerator CircularMoveTo(GameObject obj, Vector2 desiredPos, float duration)
    {
        float desiredAngle = GetAngle(desiredPos);
        float currentAngle = GetAngle(obj.transform.position);

        float angleToMove = desiredAngle - currentAngle;

        //making sure the tile takes the shorter path
        if (angleToMove > 0)
        {
            if (angleToMove > Mathf.PI)
            {
                angleToMove -= 2 * Mathf.PI;
            }
        }
        else
        {
            if (angleToMove < -Mathf.PI)
            {
                angleToMove += 2 * Mathf.PI;
            }
        }

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / duration);
            percent = Mathf.SmoothStep(0, 1, percent);

            float calculatedAngle = Mathf.Lerp(currentAngle, currentAngle + angleToMove, percent);

            float xValue = Data.radius * Mathf.Sin(calculatedAngle);
            float yValue = Data.radius * Mathf.Cos(calculatedAngle);

            obj.transform.position = new Vector2(xValue + transform.position.x, yValue + transform.position.y);

            yield return null;
        }

        float finalX = Data.radius * Mathf.Sin(desiredAngle);
        float finalY = Data.radius * Mathf.Cos(desiredAngle);

        obj.transform.position = new Vector2(finalX + transform.position.x, finalY + transform.position.y);
    }

    private float GetAngle(Vector2 tile)
    {
        float radius = Vector2.Distance(tile, transform.position);

        float y = tile.y - transform.position.y;
        float x = tile.x - transform.position.x;

        if (y == 0)
        {
            if (x > 0)
            {
                return Mathf.PI / 2;
            }

            return -Mathf.PI / 2;
        }

        if (y > 0)
        {
            if (x > 0)
            {
                return Mathf.Asin(Mathf.Abs(x) / radius);
            }

            return 2 * Mathf.PI - Mathf.Asin(Mathf.Abs(x) / radius);
        }

        if (x > 0)
        {
            return Mathf.PI - Mathf.Asin(Mathf.Abs(x) / radius);
        }

        return Mathf.Asin(Mathf.Abs(x) / radius) + Mathf.PI;
    }

    private IEnumerator MergeTiles()
    {
        yield return new WaitForSecondsRealtime(Data.mergeDelay);

        int merges = 0;

        for (int i = 1; i < (tileListObjects.Count - 1) / 2 + 1; i++)
        {
            if (tileListObjects[i].GetComponent<Tile>().noteType == tileListObjects[tileListObjects.Count - i].GetComponent<Tile>().noteType &&
                !tileListObjects[i].GetComponent<Tile>().isMerge && !tileListObjects[tileListObjects.Count - i].GetComponent<Tile>().isMerge)
            {
                merges++;

                StartCoroutine(CircularMoveTo(tileListObjects[i], tileListObjects[0].transform.position, Data.timeToMerge));
                StartCoroutine(CircularMoveTo(tileListObjects[tileListObjects.Count - i], tileListObjects[0].transform.position, Data.timeToMerge));
            }
            else
            {
                break;
            }

            yield return new WaitForSecondsRealtime(Data.timeBetweenMerges);

            Destroy(tileListObjects[i]);
            Destroy(tileListObjects[tileListObjects.Count - i]);
        }

        yield return new WaitForSecondsRealtime(Data.timeBetweenMerges / 2);

        List<GameObject> newList = new List<GameObject>();

        for (int i = 0; i < tileListObjects.Count; i++)
        {
            if (tileListObjects[i])
            {
                newList.Add(tileListObjects[i]);
            }
        }

        tileListObjects.Clear();

        for (int i = 0; i < newList.Count; i++)
        {
            tileListObjects.Add(newList[i]);
        }

        rearrangingTilesFirstIndex = 0;

        if (tileListObjects.Count == 1)
        {
            RespawnMissingTiles(tileListObjects[0]);
        }
        else
        {
            RearrangeTiles(tileListObjects[0], tileListObjects.Count, Data.addTileMoveDuration);
        }

        tileListObjects[0].GetComponent<Tile>().isCharged = true;
    }

    #endregion

    #region Spawning Stuff

    private void RespawnMissingTiles(GameObject tile)
    {
        float angle = GetAngle(tile.transform.position) + Mathf.PI * 120 / 180;

        for (int i = 0; i < 2; i++)
        {
            float xValue = Data.radius * Mathf.Sin(angle);
            float yValue = Data.radius * Mathf.Cos(angle);

            GameObject newTile = Instantiate(Data.Tiles[Random.Range(0, 3)]);
            newTile.transform.position = new Vector2(xValue + transform.position.x, yValue + transform.position.y);
            newTile.transform.parent = transform;
            tileListObjects.Add(newTile);

            angle += Mathf.PI * 120 / 180;
        }
    }

    private void SpawnGrid(int numberOfTiles)
    {
        //calculating the angle between tiles
        float angle = 2 * Mathf.PI / numberOfTiles;
        float currentAngle = 0;

        for (int i = 0; i < numberOfTiles; i++)
        {
            //getting the values for the position
            float xValue = Data.radius * Mathf.Sin(currentAngle);
            float yValue = Data.radius * Mathf.Cos(currentAngle);

            //creating the new tile
            GameObject newTile = Instantiate(Data.Tiles[Random.Range(0, 3)]);
            newTile.transform.parent = transform;
            newTile.transform.position = new Vector2(xValue + transform.position.x, yValue + transform.position.y);

            tileListObjects.Add(newTile);

            currentAngle += angle;
        }
    }

    private IEnumerator SpawnTile()
    {
        GameObject tile;

        if ((int)Random.Range(0, Data.mergeTileProbability) == 0)
        {
            //make a merge tile
            tile = Instantiate(Data.MergeTiles[Random.Range(0, 3)]);
            tile.GetComponent<Tile>().isMerge = true;
        }
        else
        {
            //make a regular tile
            tile = Instantiate(Data.Tiles[Random.Range(0, 3)]);
            tile.GetComponent<Tile>().isMerge = false;
        }

        tile.GetComponent<Tile>().isCharged = false;

        Vector2 startScale = tile.transform.localScale;

        //putting it in the center
        tile.transform.parent = transform;
        tile.transform.position = transform.position;
        tile.transform.localScale = Vector2.zero;

        float t = 0;

        while (t < Data.spawnScaleDuration)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / Data.spawnScaleDuration);
            percent = Mathf.SmoothStep(0, 1, percent);

            tile.transform.localScale = Vector2.Lerp(Vector2.zero, startScale * Data.spawnScaleMult, percent);

            yield return null;
        }

        t = 0;

        while (t < Data.spawnScaleDuration)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / Data.spawnScaleDuration);
            percent = Mathf.SmoothStep(0, 1, percent);

            tile.transform.localScale = Vector2.Lerp(startScale * Data.spawnScaleMult, startScale, percent);

            yield return null;
        }

        spawnedTile = tile;
    }

    #endregion

    #region Drag & Drop

    private IEnumerator DragTile(GameObject tile)
    {
        Vector2 desiredPos;
        Vector2 tileStartPosition = tile.transform.position;

        int num = 1;

        if (tileListObjects.Count > 2)
        {
            while (num > 0)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //calculating the distance between the mouse and the start position of the tile
                float mouseDistance = Vector2.Distance(mousePos, tileStartPosition);

                //the distance between the tiles original position and the closest available position
                float closestDistance = Vector2.Distance(tileStartPosition, GetClosestAvailablePosition());

                //getting the vector after snapping
                Vector2 snappedPos = tileStartPosition + (mouseDistance / closestDistance) * (GetClosestAvailablePosition() - tileStartPosition);

                if (mouseDistance < Data.maxDragDistance)
                {
                    desiredPos = (tileStartPosition + snappedPos) / 2;
                }
                else
                {
                    desiredPos = tileStartPosition + (Data.maxDragDistance / 2 / closestDistance) * (GetClosestAvailablePosition() - tileStartPosition);
                }

                tile.transform.position = Vector2.Lerp(tile.transform.position, desiredPos, Time.deltaTime * Data.dragSpeed);

                yield return null;
            }
        }
        else
        {
            while (num > 0)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //calculating the distance between the mouse and the start position of the tile
                float mouseDistance = Vector2.Distance(mousePos, tileStartPosition);

                if (mouseDistance < Data.maxDragDistance)
                {
                    desiredPos = (tileStartPosition + mousePos) / 2;
                }
                else
                {
                    desiredPos = (Data.maxDragDistance / Vector2.Distance(tileStartPosition, mousePos) * (mousePos - tileStartPosition)) / 2 + tileStartPosition;
                }

                tile.transform.position = Vector2.Lerp(tile.transform.position, desiredPos, Time.deltaTime * Data.dragSpeed);

                yield return null;
            }
        }
    }

    private List<Vector2> GetAvailablePositions()
    {
        List<Vector2> positions = new List<Vector2>();

        for (int i = 0; i < tileListObjects.Count; i++)
        {
            positions.Add((tileListObjects[i].transform.position + GetRightTile(tileListObjects[i]).transform.position) / 2);
        }

        return positions;
    }

    #endregion

    #region Minor Stuff

    private Vector2 GetClosestAvailablePosition()
    {
        List<Vector2> positions = GetAvailablePositions();

        float min = float.MaxValue;
        int index = 0;

        for (int i = 0; i < positions.Count; i++)
        {
            if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), positions[i]) < min)
            {
                min = Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), positions[i]);
                index = i;
            }
        }

        if (index == positions.Count - 1)
        {
            rearrangingTilesFirstIndex = 0;
        }
        else
        {
            rearrangingTilesFirstIndex = index + 1;
        }

        return positions[index];
    }

    private int GetTileIndex(GameObject tile)
    {
        for (int i = 0; i < tileListObjects.Count; i++)
        {
            if (tileListObjects[i] == tile)
            {
                return i;
            }
        }

        return -1;
    }

    private GameObject GetRightTile(GameObject tile)
    {
        int index = GetTileIndex(tile);

        //if there is only one tile
        if (tileListObjects.Count == 1 || index == -1)
        {
            return null;
        }

        if (index == tileListObjects.Count - 1)
        {
            return tileListObjects[0];
        }

        return tileListObjects[index + 1];
    }

    private GameObject GetLeftTile(GameObject tile)
    {
        int index = GetTileIndex(tile);

        //if there is only one tile
        if (tileListObjects.Count == 1 || index == -1)
        {
            return null;
        }

        if (index == 0)
        {
            return tileListObjects[tileListObjects.Count - 1];
        }

        return tileListObjects[index - 1];
    }

    private IEnumerator MoveTo(GameObject obj, Vector2 desiredPos, float duration)
    {
        float t = 0;
        Vector2 startPos = obj.transform.position;

        while (t < duration)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / duration);
            percent = Mathf.SmoothStep(0, 1, percent);

            obj.transform.position = Vector2.Lerp(startPos, desiredPos, percent);

            yield return null;
        }

        obj.transform.position = desiredPos;
    }

    #endregion
}