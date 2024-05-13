using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FowardCreation : MonoBehaviour
{
    private GameObject startTile;
    private GameObject endTile;
    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject pathPrefab;
    public GameObject edgePrefab;

    private GameObject[] mapTile;
    private int[] pathPieceIndex;
    private int numTile = 0;


    [Header("Parameters")]
    public int mapSize = 2;
    public int pathLength = 2;
    public float distanceBetweenTiles = 5f;

    private int minPathLength;
    
    // Start is called before the first frame update
    void Start()
    {
        mapTile = new GameObject[mapSize * mapSize];
        startTile = (GameObject) Instantiate(tilePrefab);
        mapTile[numTile] = startTile;
        startTile.name = numTile.ToString();
        numTile++;

        // This should be changed 
        minPathLength = mapSize;
              
        MapGeneration(startTile);

        CreateBorder();
        CreatePath();

        PrintArray(PathArray());
        
        
    }

    void MapGeneration (GameObject sTile)
    {
        if (sTile == null) 
        {
            return;
        }

        GameObject prevTile = sTile; 

        GameObject pivotTile = sTile;
        for (int outer = 0; outer < mapSize; outer++)
        {
            // Only update the pivot tile after the first iteration
            if (outer > 0)
            {
                //Setting the previous tile to the pivot tile and then creating a new tile to the right of the previous row
                prevTile = pivotTile;

                pivotTile = (GameObject) Instantiate(tilePrefab, prevTile.transform.position, prevTile.transform.rotation);
                pivotTile.transform.Translate(distanceBetweenTiles, 0f, 0f, Space.World);

                mapTile[numTile] = pivotTile;

                pivotTile.name = numTile.ToString();
                numTile++; 
                // Setting the previous tile to the pivot tile to start the new row of tiles
                prevTile = pivotTile;
            }
        
            for (int i = 0; i < mapSize - 1; i++)
            {      
                GameObject tile = (GameObject) Instantiate(tilePrefab, prevTile.transform.position, prevTile.transform.rotation);
    
                tile.transform.Translate(0f,0f,distanceBetweenTiles, Space.World);
                prevTile = tile;
                tile.name = numTile.ToString();
                mapTile[numTile] = tile;
                numTile++;
            }
        }
    }

    
    

    Boolean IsEdge(int index)
    {
        if (index < mapSize) {return true;}
        if ((index % mapSize) == 0) {return true;}
        if (((index + 1) % mapSize) == 0) {return true;}
        if (index > (mapSize * (mapSize - 1))) {return true;}

        return false;
        
    }

    void CreateBorder() 
    {
        ConvertToEdge(EdgeArray(mapTile));
    }

    void CreatePath()
    {
        ConvertToPath(PathArray());
    }

    // Creates an integer array of the indexes of edge pieces on the map 
    int[] EdgeArray(GameObject[] mapTiles)
    {
        int[] edgeIndex = new int[(mapSize * 4) - 4];
        int edgeCount = 0;

        for (int i = 0; i < mapTiles.Length; i++) 
        {
            if (IsEdge(i))
            {   
                edgeIndex[edgeCount] = i;
                edgeCount++;
            }
        }
        return edgeIndex;

    }

    int[] PathArray()
    {
        int[] pathIndex = new int[pathLength];
        int[] mapTileIndex = new int[mapSize * mapSize];

        for (int i = 0; i < mapTileIndex.Length; i++)
        {
            mapTileIndex[i] = i;
        }

        int startIndex = mapSize;
        int currentPathIndex = startIndex;
        // Hard coded for testing purposes 

        for (int i = 0; i < pathLength; i++)
        {
            // The first two path tiles should be created in a straight line from the bottom corner 
            if (i < 2)
            {
                pathIndex[i] = currentPathIndex;
                if (i == 0)
                {
                    currentPathIndex++;
                }
                
            }
            else
            {
                currentPathIndex = NextPathIndex(ConnectedTileIndex(currentPathIndex));
                pathIndex[i] = currentPathIndex;
            }
        }

        return pathIndex;

        int NextPathIndex(int[] potentialIndex)
        {

            if (potentialIndex == null)
            {
                return -1;
            }


            int index = -1;
            int newIndex = -1;

            for (int i = 0; i < potentialIndex.Length; i++) {
                if (IsEdge(potentialIndex[i]))
                {
                    potentialIndex[i] = -1;
                }  

                for (int j = 0; j < pathIndex.Length; j++)
                {
                    if ( potentialIndex[i] == pathIndex[j]) {
                        potentialIndex[i] = -1;
                    }
                }
            }

            int count = 0;

            while ((newIndex == -1) || (count < 4))
            {
                index = UnityEngine.Random.Range(0, 3);
                if (potentialIndex[index] != -1)
                {
                    newIndex = index;
                }
                count++;
            }
            
            return potentialIndex[newIndex];
        }

    }

    // Uses an array of indexs to replace the ground pieces with edge pieces 
    void ConvertToEdge(int[] edgePieces)
    {
        for (int i = 0; i < edgePieces.Length; i++)
        {
            GameObject temp = mapTile[edgePieces[i]];
            mapTile[edgePieces[i]] = Instantiate(edgePrefab, temp.transform.position, temp.transform.rotation);
            Destroy(temp);
        }
    }

    void ConvertToPath(int[] pathPieces)
    {
        for (int i = 0; i < pathPieces.Length; i++)
        {
            if (pathPieces[i] != -1)
            {
                GameObject temp = mapTile[pathPieces[i]];
                mapTile[pathPieces[i]] = Instantiate(pathPrefab, temp.transform.position, temp.transform.rotation);
                Destroy(temp);
            }  
        }
    }


    int[] ConnectedTileIndex(int index)
    {

        if (index == -1) {
            return null;
        }


        int[] tileIndex = new int[4];

        // Forward tile
        tileIndex[0] = index + 1;
        // Right tile
        tileIndex[1] = index + mapSize;
        // Backwards tile
        tileIndex[2] = index - 1;
        // Left tile
        tileIndex[3] = index - mapSize;


        return tileIndex;
    }


    

    void PrintArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log("Index: " + i + "\nValue: " + array[i]);
        }
    }

    


}

 