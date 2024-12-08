using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public Vector2Int gridSize;
    public Vector2Int roomSizeRange;
    public GameObject tilePrefab;
    public GameObject tileWallsPrefab;

    public GameObject enemyPrefab;

    public GameObject[,] tilesInstanced;

    //public GameObject player;

    public int pathsToGenerate;
    public int roomsToGenerate;

    public int enemiesToGenerate;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Generate()
    {
        tilesInstanced = new GameObject[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                tilesInstanced[x, z] = Instantiate(tilePrefab, new Vector3(x * tilePrefab.transform.localScale.x, 0, z * tilePrefab.transform.localScale.z), Quaternion.identity);
                tilesInstanced[x, z].SetActive(false);
            }
        }

        StartCoroutine(GeneratePath(0, 0));
    }
    
    private IEnumerator GeneratePath(int startX, int startZ)
    {
        int x = startX;
        int z = startZ;
        List<Vector2Int> allowedDirections;
        Vector2Int lastDirection = Vector2Int.right;
        
        for (int i = 0; i < 30; i++)
        {
            tilesInstanced[x, z].SetActive(true);
            //player.transform.position = new Vector3(x * tilePrefab.transform.localScale.x, 0, z * tilePrefab.transform.localScale.z);
            yield return new WaitForSeconds(0.05f);
            allowedDirections = new List<Vector2Int>();
            if(x>0)
                if(!tilesInstanced[x-1, z].activeSelf)
                    allowedDirections.Add(Vector2Int.left);
            if(x<gridSize.x-1)
                if(!tilesInstanced[x+1, z].activeSelf)
                    allowedDirections.Add(Vector2Int.right);
            if(z>0)
                if(!tilesInstanced[x, z-1].activeSelf)
                    allowedDirections.Add(Vector2Int.down);
            if(z<gridSize.y-1)
                if(!tilesInstanced[x, z+1].activeSelf)
                    allowedDirections.Add(Vector2Int.up);
            if(x + lastDirection.x > 0 && x + lastDirection.x < gridSize.x - 1 && z + lastDirection.y > 0 && z + lastDirection.y < gridSize.y - 1)
                if(!tilesInstanced[x + lastDirection.x, z + lastDirection.y].activeSelf){
                    for(int j = 0; j < 10; j++){
                        allowedDirections.Add(lastDirection);
                    }
                }

            if(allowedDirections.Count>0)
            {
                Vector2Int chosenDirection = allowedDirections[Random.Range(0, allowedDirections.Count)];
                x += chosenDirection.x;
                z += chosenDirection.y;
                lastDirection = chosenDirection;
            }
        }

        pathsToGenerate--;
        if(pathsToGenerate>0){
            Vector2Int newStartPosition = GetNewStartPosition();
            StartCoroutine(GeneratePath(newStartPosition.x, newStartPosition.y));
        }
        else
            StartCoroutine(GenerateRooms());
    }

    private IEnumerator GenerateRooms()
    {
        for(int i = 0; i < roomsToGenerate; i++){
            Vector2Int newStartPosition = GetNewStartPosition();
            int roomSize = Random.Range(roomSizeRange.x, roomSizeRange.y);
            for(int x = newStartPosition.x - roomSize; x <= newStartPosition.x + roomSize; x++)
            {
                for(int z = newStartPosition.y - roomSize; z <= newStartPosition.y + roomSize; z++)
                {
                    if(x >= 0 && x < gridSize.x && z >= 0 && z < gridSize.y)
                    tilesInstanced[x, z].SetActive(true);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(GenerateRoomWalls());
    }

    private IEnumerator GenerateRoomWalls(){
        for(int x = 0; x < gridSize.x; x++){
            for(int z = 0; z < gridSize.y; z++){
                if(tilesInstanced[x, z].activeSelf){
                    GameObject walls = Instantiate(tileWallsPrefab, new Vector3(x * tilePrefab.transform.localScale.x, 0, z * tilePrefab.transform.localScale.z), Quaternion.identity);
                    TileWall tileWall = walls.GetComponent<TileWall>();
                    bool rWall = false;
                    if(x >= gridSize.x - 1)
                        rWall = true;
                    else
                        if(!tilesInstanced[x+1, z].activeSelf)
                            rWall = true;

                    bool lWall = false;
                    if(x <= 0)  
                        lWall = true;
                    else
                        if(!tilesInstanced[x-1, z].activeSelf)
                            lWall = true;


                    bool fWall = false;
                    if(z >= gridSize.y - 1)
                        fWall = true;
                    else
                        if(!tilesInstanced[x, z+1].activeSelf)
                            fWall = true;

                    bool bWall = false; 
                    if(z <= 0)
                        bWall = true;
                    else
                        if(!tilesInstanced[x, z-1].activeSelf)
                            bWall = true;

                    tileWall.SetWalls(rWall, lWall, fWall, bWall);
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        StartCoroutine(GenerateEnemies());
    }

    private IEnumerator GenerateEnemies(){
        for(int i = 0; i < enemiesToGenerate; i++)
        {
            Vector2Int enemyPosition = GetRandomEnemyPosition();
            GameObject enemy = Instantiate(enemyPrefab, new Vector3(enemyPosition.x * tilePrefab.transform.localScale.x, 1, enemyPosition.y * tilePrefab.transform.localScale.z), Quaternion.identity);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private Vector2Int GetNewStartPosition()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                bool otherPathPossible = false;
                if(tilesInstanced[x, z].activeSelf){
                    if(x>0)
                        if(!tilesInstanced[x-1, z].activeSelf)
                            otherPathPossible = true;
                    if(x<gridSize.x-1)
                        if(!tilesInstanced[x+1, z].activeSelf)
                            otherPathPossible = true;
                    if(z>0)
                        if(!tilesInstanced[x, z-1].activeSelf)
                            otherPathPossible = true;
                    if(z<gridSize.y-1)
                        if(!tilesInstanced[x, z+1].activeSelf)
                            otherPathPossible = true;

                    if(otherPathPossible)
                        positions.Add(new Vector2Int(x, z));
                }
            }
        }
        return positions[Random.Range(0, positions.Count)];
    }

    private Vector2Int GetRandomEnemyPosition()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                if(tilesInstanced[x, z].activeSelf){
                        positions.Add(new Vector2Int(x, z));
                }
            }
        }
        return positions[Random.Range(0, positions.Count)];
    }
}
