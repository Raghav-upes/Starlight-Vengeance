using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TerrainData FlatLand;
    /*   public static GameManager Instance;

        public GameObject Tower1Canvas;
        public GameObject Tower2Canvas;
        public GameObject Tower3Canvas;

        private void Start()
        {
            Instance=this;
        }
        public void Tower2Activate()
        {
            Tower1Canvas.SetActive(false);
            Tower2Canvas.SetActive(true);  
        }


        public void Tower3Activate()
        {
            Tower2Canvas.SetActive(false);
            Tower3Canvas.SetActive(true);
        }

    */

    private void Awake()
    {
        //TerrainData originalTerrainData = FlatLand;
        //TerrainData op = new TerrainData();

        //// Copying the terrain heights
        //int heightmapWidth = originalTerrainData.heightmapResolution;
        //int heightmapHeight = originalTerrainData.heightmapResolution;
        //float[,] heights = originalTerrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        //op.heightmapResolution = originalTerrainData.heightmapResolution;
        //op.size = originalTerrainData.size;
        //op.SetHeights(0, 0, heights);

        // Assign the new terrain data to active terrain without modifying the original
        Terrain.activeTerrain.terrainData = FlatLand;
        Terrain.activeTerrain.GetComponent<TerrainCollider>().terrainData = FlatLand;
    }

}
