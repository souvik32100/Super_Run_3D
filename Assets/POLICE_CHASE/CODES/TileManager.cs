using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefab;
    public GameObject[] tilePrefabNight;
    public Transform playerTransform;
    private float spawnZ = 72.5f;
    private float tileLength = 97f;
    private int amountTileOnScreen = 7;
    public float safeZone = 150f;
    private int lastPrefabIndex = 0;

    public List<GameObject> activeTiles;

    public bool tileDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        activeTiles = new List<GameObject>();
        
        if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
        {
            playerTransform = GameManagerPC.instance.heli.transform;
        }
        if (GameManagerPC.instance.vehicleType == VehicleType.car)
        {
            playerTransform = GameManagerPC.instance.car.transform;
        }
        if (GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
        {
            playerTransform = GameManagerPC.instance.monsterTruck.transform;
        }

        for (int i = 0; i < amountTileOnScreen; i++)
        {
            //if (StartMenuPC.instance.IsHelimode())
            //{
            //    GameManagerPC.instance.timeOfDay = TimeofDay.night;
            //}
            SpawnTile();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerPC.instance.vehicleType == VehicleType.car)
        {
            playerTransform = GameManagerPC.instance.car.transform;
        }
        else if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
        {
            playerTransform = GameManagerPC.instance.heli.transform;
        }
        else if (GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
        {
            playerTransform = GameManagerPC.instance.monsterTruck.transform;
        }


        if ( Mathf.Abs(playerTransform.position.z - safeZone) > Mathf.Abs( spawnZ - amountTileOnScreen * tileLength))
        {
            SpawnTile();
            if (!GameManagerPC.instance.FPPMode && GameManagerPC.instance.cameraView != CameraView.sideView)
                DeleteTile();
            if (GameManagerPC.instance.cameraView == CameraView.sideView && !tileDestroyed)
            {
                StartCoroutine(DeleteTileRoutine());
            }
        }
    }
    private void SpawnTile(int prefabIndex = -1)
    {
        if(GameManagerPC.instance.timeOfDay == TimeofDay.day)
        {
            GameObject go = Instantiate(tilePrefab[RandomPrefabIndex()]) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = Vector3.forward * spawnZ;
            spawnZ += tileLength;
            activeTiles.Add(go);
        }
        else if (GameManagerPC.instance.timeOfDay == TimeofDay.night)
        {
            GameObject go = Instantiate(tilePrefabNight[RandomPrefabIndexNight()]) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = Vector3.forward * spawnZ;
            spawnZ += tileLength;
            activeTiles.Add(go);
        }
    }
    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
    public IEnumerator DeleteTileRoutine()
    {
        tileDestroyed = false;
        yield return new WaitForSeconds(1.5f);

        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
        tileDestroyed = true;

        yield return new WaitForSeconds(1f);

        tileDestroyed = false;
    }
    private int RandomPrefabIndex()
    {
        if (tilePrefab.Length <= 1) return 0;
        int randomIndex = lastPrefabIndex;
        while(randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0, tilePrefab.Length);
        }
        lastPrefabIndex = randomIndex;
        return randomIndex;
    }
    private int RandomPrefabIndexNight()
    {
        if (tilePrefabNight.Length <= 1) return 0;
        int randomIndex = lastPrefabIndex;
        while (randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0, tilePrefabNight.Length);
        }
        lastPrefabIndex = randomIndex;
        return randomIndex;
    }
}
