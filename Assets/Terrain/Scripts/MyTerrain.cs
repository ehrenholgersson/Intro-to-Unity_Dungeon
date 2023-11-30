using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Threading;
//using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class MyTerrain : MonoBehaviour
{
    //[SerializeField] Material _material;
    [SerializeField] float _sandHeight, _sandVarience, _snowHeight, _snowVarience, _dirtAngle, _dirtVarience,_rockAngle, _rockVarience, _heightMapNoiseAmount;
    [SerializeField] TerrainData _terrainData;
    [SerializeField] Texture2D _heightSource;
    float[,] _heightMap;
    float[,] _varienceMap;
    [SerializeField] Terrain _terrain;
    //[SerializeField] float _mapScale;
    [SerializeField] int _seed = 0;
    [SerializeField] bool _useTexture = false, _randomSeed = true;
    [SerializeField] int _width;
    [SerializeField] int _depth;
    [SerializeField] int _height;
    [SerializeField] int _numTrees;
    [SerializeField] GameObject _treeControl;

    // Start is called before the first frame update
    void Start()
    {
        float timer = Time.time;
        if (_useTexture)
            _heightMap = HeightFromImage(_heightSource);
        else if (_randomSeed)
            _heightMap = Island(Upscale(HeightFromNoise(UnityEngine.Random.Range(10000, 100000), 64, 64), 4));
        else
            _heightMap = Island(Upscale(HeightFromNoise(_seed, 64, 64), 4));

        HeightMapNoise();
        _varienceMap = HeightFromNoise(UnityEngine.Random.Range(10000, 100000), 500, 500);

        //_terrainData = new TerrainData();
        Debug.Log("HeightMap size is " + _heightMap.GetLength(0));
        _terrainData.heightmapResolution = _heightMap.GetLength(0);//new Vector3(_heightmap.GetLength(0), _heightmap.GetLength(1), 100);
        _terrainData.size = new Vector3(_width, _height, _depth);
        _terrainData.SetHeights(0, 0, _heightMap);
        _terrainData.alphamapResolution = _heightMap.GetLength(0);
        // _terrainData.terrainLayers
        //SetupLayers();
        SetTerrainColor();
        AddGrass();
        AddTrees(_numTrees, 5);
        //AddTrees(100,5);
        //Terrain t = gameObject.AddComponent<Terrain>();
        //t.terrainData = _terrainData;
        //t.materialTemplate = _material;
        _terrain.Flush();
        //_terrain.gameObject.GetComponent<DestroyTree>().Setup();
        Debug.Log("Completed in " + (Time.time - timer) + " seconds");
    }

    float[,] HeightFromImage(Texture2D tex)
    {
        float[,] hmap = new float[tex.width, tex.height];
        for (int x = 0; x < hmap.GetLength(0); x++)
            for (int y = 0; y < hmap.GetLength(0); y++)
            {
                Color pixel = tex.GetPixel(x, y);
                hmap[x, y] = (pixel.r + pixel.g + pixel.b) / 3;
            }
        return hmap;
    }

    void HeightMapNoise()
    {
        float [,] noise = HeightFromNoise(UnityEngine.Random.Range(10000, 100000), _heightMap.GetLength(0), _heightMap.GetLength(1));
        for (int x = 0; x < _heightMap.GetLength(0); x++)
            for (int y = 0; y < _heightMap.GetLength(1); y++)
                _heightMap[x, y] += ((noise[x, y] * _heightMapNoiseAmount) - (_heightMapNoiseAmount / 2)) / _height;
    }

    float[,] Island(float[,] map)
    {
        for (int x = 0;x < map.GetLength(0); x++)
            for(int y = 0;y< map.GetLength(1); y++)
            {
                map[x, y] *= 1 - Mathf.Clamp(((new Vector2(map.GetLength(0) / 2, map.GetLength(1) / 2) - new Vector2(x, y)).magnitude) / (map.GetLength(0) / 2), 0, 1);
            }
        return map;
    }

    float[,] HeightFromNoise(int seed,int width,int height)
    {
        float[,] hmap = new float[width, height];
        for(int x = 0;x<width;x++)
            for (int y = 0; y < height; y++)
            {
                hmap[x, y] = Mathf.PerlinNoise((seed/10000f + x)/7, (seed/10000f + y)/7);
                //Debug.Log("In "+ ((x + seed) / 1000f )+ ","+((y + seed) / 1000f) +"out: "+hmap[x,y]);
            }
        return hmap;
    }

    void SetupLayers()
    {
        Color[] terrainColors = new Color[] { Color.yellow, Color.green, new Color(1, 0.2f, 0.2f, 1), Color.white, Color.gray };
        if (_terrainData != null )
        {
            for (int i = 0; i < terrainColors.Length; i++) 
            {
                Texture2D tex2D = new Texture2D(1,1);
                tex2D.SetPixels(new Color[] { terrainColors[i] }, 0);
                if (_terrainData.terrainLayers.Length < i)
                    _terrainData.terrainLayers[i].diffuseTexture = tex2D;
            }
        }

    }

    void SetTerrainColor()
    {
        Vector2 scalefactor = new Vector2 ((_terrainData.alphamapWidth/ _terrainData.heightmapResolution) , (_terrainData.alphamapHeight/ _terrainData.heightmapResolution));
        //float scaleX = _terrainData.heightmapResolution / _terrainData.alphamapWidth;
        float[,,] splatData = new float[_terrainData.alphamapWidth, _terrainData.alphamapHeight, _terrainData.alphamapLayers];
        Debug.Log("found " + _terrainData.alphamapLayers + " terrain layers");
        for (int x=0;x < _terrainData.alphamapWidth;x++)
            for(int y = 0; y < _terrainData.alphamapHeight;y++)
            {
                float height = _terrainData.GetHeight(y,x);
                for (int i = 1; i < _terrainData.alphamapLayers; i++)
                {
                    // decide terrain color
                    int layer = 0;
                    float normX = x * 1.0f / (_terrainData.alphamapWidth);
                    float normY = y * 1.0f / (_terrainData.alphamapWidth);
                    int varX = x * _varienceMap.GetLength(1) / (_terrainData.alphamapWidth);
                    int varY = y * _varienceMap.GetLength(0) / (_terrainData.alphamapHeight);
                    var angle = _terrainData.GetSteepness(normY, normX);

                    if (height < _sandHeight +  ((_sandVarience - _sandVarience / 2)) * _varienceMap[varX,varY])
                        layer = 0;
                    else if (angle > _rockAngle + ((_rockVarience - _sandVarience / 2)) * _varienceMap[varX, varY])
                        layer = 3;
                    else if (height > _snowHeight + ((_snowVarience - _sandVarience / 2)) * _varienceMap[varX, varY])
                        layer = 4;
                    else if (angle > _dirtAngle + ((_dirtVarience - _sandVarience / 2)) * _varienceMap[varX, varY])
                        layer = 1;
                    else
                        layer = 2;


                    switch (layer)
                    {
                        case 0:
                            splatData[x, y, 0] = 1;
                            //blend around edges
                            if (x>1&&y>1)
                            for (int xx = x - 1; xx < x + 2&& xx < splatData.GetLength(0); xx++)
                                for (int yy = y - 1; yy < y + 2&& yy < splatData.GetLength(1); yy++)
                                    if (splatData[xx, yy, 0] != 1)
                                        splatData[xx, yy, 0] = 0.5f;
                            break;

                        case 1:
                            splatData[x, y, 1] = 1;
                            if (x > 1 && y > 1)
                                for (int xx = x - 1; xx < x + 2; xx++)
                                for (int yy = y - 1; yy < y + 2; yy++)
                                    if (xx < _terrainData.alphamapWidth && yy < _terrainData.alphamapHeight && splatData[xx, yy, 1] != 1)
                                        splatData[xx, yy, 1] = 0.5f;
                            break;

                        case 2:
                            splatData[x, y, 2] = 1;
                            //blend around edges
                            if (x > 1 && y > 1)
                                for (int xx = x - 1; xx < x + 2; xx++)
                                for (int yy = y - 1; yy < y + 2; yy++)
                                    if (xx < _terrainData.alphamapWidth && yy < _terrainData.alphamapHeight && splatData[xx, yy, 2] != 1)
                                        splatData[xx, yy, 2] = 0.5f;
                            break;

                        case 3:
                            splatData[x, y, 3] = 1;
                            //blend around edges
                            if (x > 1 && y > 1)
                                for (int xx = x - 1; xx < x + 2; xx++)
                                for (int yy = y - 1; yy < y + 2; yy++)
                                    if (xx < _terrainData.alphamapWidth && yy < _terrainData.alphamapHeight && splatData[xx, yy, 3] != 1)
                                        splatData[xx, yy, 3] = 0.5f;
                            break;

                        case 4:
                            splatData[x, y, 4] = 1;
                            //blend around edges
                            if (x > 1 && y > 1)
                                for (int xx = x - 1; xx < x + 2; xx++)
                                for (int yy = y - 1; yy < y + 2; yy++)
                                    if (xx < _terrainData.alphamapWidth && yy < _terrainData.alphamapHeight && splatData[xx, yy, 4] != 1)
                                        splatData[xx, yy, 4] = 0.5f;
                            break;

                        case 5:
                            splatData[x, y, 5] = 1;
                            //blend around edges
                            if (x > 1 && y > 1)
                                for (int xx = x - 1; xx < x + 2; xx++)
                                for (int yy = y - 1; yy < y + 2; yy++)
                                    if (xx < _terrainData.alphamapWidth && yy < _terrainData.alphamapHeight && splatData[xx, yy, 5] != 1)
                                        splatData[xx, yy, 5] = 0.5f;
                            break;

                        case 6:
                            splatData[x, y, 6] = 1;
                            //blend around edges
                            if (x > 1 && y > 1)
                                for (int xx = x - 1; xx < x + 2; xx++)
                                for (int yy = y - 1; yy < y + 2; yy++)
                                    if (xx < _terrainData.alphamapWidth && yy < _terrainData.alphamapHeight && splatData[xx, yy, 6] != 1)
                                        splatData[xx, yy, 6] = 0.5f;
                            break;

                    }
                    //splatData[x, y, 0] = 1;
                    //if (height > ((_height/2) / _terrainData.alphamapLayers) * i)
                    //{
                    //    splatData[x, y, i] = 1;
                    //    Debug.Log("colouring layer " + i + " at " + x+"," + y);
                    //}
                    ////else splatData[x, y, i] = 0; // possibly can check neighbours and perform blend here if needed
                }
            }
        _terrainData.SetAlphamaps(0, 0, splatData);

    }

    float[,] Upscale(float[,] map,int passes)
    {
        //float timer = Time.time;
        while (passes > 0)
        {
            
            float[,] newMap = new float[map.GetLength(0) * 2, map.GetLength(1) * 2];
            for (int x = 0; x < newMap.GetLength(0); x += 2)
                for (int y = 0; y < newMap.GetLength(1); y += 2)
                {
                    newMap[x, y] = map[x / 2, y / 2];
                }
            for (int x = 1; x < newMap.GetLength(0)-1; x += 2)
                for (int y = 0; y < newMap.GetLength(1); y += 2)
                {
                    //if (x > 2 && x < newMap.GetLength(0) - 3)
                    //    newMap[x, y] = Mathf.Clamp(newMap[x - 1, y] - ((newMap[x - 3, y] - newMap[x - 1, y]) + (newMap[x + 1, y] - newMap[x + 3, y])) / 4, Mathf.Min(newMap[x - 1, y], newMap[x + 1,y]),Mathf.Max(newMap[x - 1, y], newMap[x + 1, y]));
                    //else
                        newMap[x, y] = (newMap[x - 1, y] + newMap[x + 1, y]) / 2;
                    //if (y == 0)
                    //    Debug.Log(x + "," + y + " = " + newMap[x, y]);
                }
            for (int x = 0; x < newMap.GetLength(0); x ++)
                for (int y = 1; y < newMap.GetLength(1)-1; y += 2)
                {
                    //if (y > 2 && y < newMap.GetLength(1) - 3)
                    //    newMap[x, y] = newMap[x , y-1] - ((newMap[x , y-3] - newMap[x , y-1]) + (newMap[x , y+1] - newMap[x, y+3])) / 2;
                    //else
                        newMap[x, y] = (newMap[x, y - 1] + newMap[x, y + 1]) / 2;
                }
            map = newMap;
            passes--;
        } 
        Debug.Log("upscaled map to " + map.GetLength(0) + " x " + map.GetLength(1));
        return map;
    }

    void AddGrass()
    {
        float[,,] alMap = _terrainData.GetAlphamaps(0,0,_terrainData.alphamapWidth, _terrainData.alphamapHeight);
        int[,] grassMap = new int[_terrainData.alphamapWidth, _terrainData.alphamapHeight];

        for (int x = 0;x < grassMap.GetLength(0)-1;x ++)
            for (int y = 0;y < grassMap.GetLength(1)- 1;y ++)
            {
                if (alMap[x,y,2]> 0.5f)
                {
                    grassMap[x, y] = 2;
                    grassMap[x, y] = 2;
                }
                else
                {
                    grassMap[x, y] = 0;
                    grassMap[x, y] = 0;
                }

            }
        _terrainData.SetDetailLayer(0, 0, 0, grassMap);
        _terrainData.SetDetailLayer(0, 0, 1, grassMap);
    }

    void AddTrees(int count, int density)
    {
        float[,,] alMap = _terrainData.GetAlphamaps(0, 0, _terrainData.alphamapWidth, _terrainData.alphamapHeight);
        int[,] treeMap = new int[_terrainData.alphamapWidth, _terrainData.alphamapHeight];
        float timer = Time.time;

        while (count > 0)
        {

            _terrainData.treeInstances = new List<TreeInstance>(0).ToArray();
            for (int gridX = 0; gridX < _terrainData.alphamapWidth - 21; gridX += 20)
            {
                for (int gridY = 0; gridY < _terrainData.alphamapHeight - 21; gridY += 20)//&& gridY < _terrainData.alphamapHeight - 21)
                {
                    int[] pos = new int[2] { gridX + UnityEngine.Random.Range(0, 20), gridY + UnityEngine.Random.Range(0, 20) };
                    if (alMap[pos[1], pos[0], 2] == 1) // to do, check surrounding trees against density value
                    {
                        treeMap[pos[0], pos[1]] = 1;
                        count--;
                    }


                    //if (Time.time > timer + 10)
                    //{
                    //    Debug.Log("took too long adding trees");
                    //    break;
                    //}
                    if (count < 1)
                        break;

                }
                if (count < 1)
                    break;
            }
            if (Time.time > timer+10)
            {
                Debug.Log("took too long adding trees");
                break;
            }
        }
        PlaceTree(treeMap);

    }
    void PlaceTree(int[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
            for (int y = 0; y < map.GetLength(1); y++)
                if (map[x, y] > 0)
                {
                    TreeInstance treeInstance = new TreeInstance();
                    treeInstance.prototypeIndex = UnityEngine.Random.Range(0,2);
                    treeInstance.widthScale = 1f;
                    treeInstance.heightScale = 1f;
                    treeInstance.color = Color.white;
                    treeInstance.lightmapColor = Color.white;
                    Vector3 position = new Vector3((float)x/map.GetLength(0),0, (float)y / map.GetLength(1));
                    position.y = _terrainData.GetInterpolatedHeight(position.x,position.z)/_terrainData.size.y;
                    treeInstance.position = position;
                    _terrain.AddTreeInstance(treeInstance);
                    //GameObject go = Instantiate(_treeControl);
                    //TreeControl tC = go.GetComponent<TreeControl>();
                    //tC.Tree = treeInstance;
                    //tC.Terrain = _terrain.gameObject.GetComponent<DestroyTree>();
                    //tC.transform.SetParent(_terrain.transform, true);
                    //go.transform.position = Vector3.Scale(position, _terrainData.size);
                    //Debug.Log("Added Tree at" + position.x + ","+position.y+ "," + position.z);
                    
                }
        Debug.Log(_terrainData.treeInstanceCount + " Trees");
    }

    // Update is called once per frame

}
