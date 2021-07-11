using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject ref_tile;

    private int rows = 100;
    private int cols = 100;

    private float tile_size = 1;



    // Start is called before the first frame update
    void Start()
    {
        this.GenerateGrid();
    }

    private void GenerateGrid()
    {
        for(int row = -rows/2; row < rows/2;row++)
        {
            for (int col = -cols/2; col < cols/2; col++)
            {
                GameObject tile = (GameObject)Instantiate(ref_tile, transform);

                float pos_x = col * tile_size;
                float pos_y = row * (-tile_size);

                tile.transform.position = new Vector2(pos_x, pos_y);
            }
        }

        Destroy(ref_tile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
