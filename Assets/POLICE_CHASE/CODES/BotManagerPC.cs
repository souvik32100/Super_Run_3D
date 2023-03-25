using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BotManagerPC : MonoBehaviour
{
    public static BotManagerPC instance;
    public GameObject[] botPrefabs;
    private Transform playerTransform;
    public float minSpawnDistance;
    public float maxSpawnDistance;
    public float[] lanePositions;
    public float spawnDelay;
    public float spawnDelayMax;
    public int botsToSpawn;

    bool firstSpawn = true;

    GameObject spawnedBot;
    GameObject spawnedBot2;

    int laneCount = 0;

    private void Awake()
    {
        if (!instance) instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnDelay = 0;
    }

    public static BotManagerPC sharedManager()
    {
        return instance;
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

        if (!GameManagerPC.instance.startGame) return;
        if (GameManagerPC.instance.victory) return;

        maxSpawnDistance = playerTransform.position.z + 300;
        minSpawnDistance = playerTransform.position.z + 250;

        spawnDelay += Time.deltaTime;
        if(spawnDelay >= spawnDelayMax)
        {
            //if(firstSpawn)
            //{
            //    spawnedBot = botPrefabs[0];
            //    firstSpawn = false;
            //}
            //else
            //{
                int randBot = Random.Range(1, botPrefabs.Length);
                int randBot2 = Random.Range(1, botPrefabs.Length);
                spawnedBot = botPrefabs[randBot];
                spawnedBot2 = botPrefabs[randBot2];
            //}
            int randLane = Random.Range(0, lanePositions.Length);
            GameObject botCar = Instantiate(spawnedBot, new Vector3(lanePositions[0], 0.5f, Random.Range(minSpawnDistance, maxSpawnDistance)), spawnedBot.transform.rotation);
            GameObject botCar2 = Instantiate(spawnedBot2, new Vector3(lanePositions[1], 0.5f, Random.Range(minSpawnDistance, maxSpawnDistance)), spawnedBot2.transform.rotation);

            botCar.GetComponent<BotController>().lane = 0;
            botCar2.GetComponent<BotController>().lane = 1;

            //if (randLane == 0)//wrong way car
            //{
            //    botCar.GetComponent<BotController>().lane = randLane;
            //}
            //else if (randLane == 1)//right way car
            //{
            //    botCar.GetComponent<BotController>().lane = randLane;
            //}
            Destroy(botCar, 15f);
            Destroy(botCar2, 15f);
            spawnDelay = 0;
        }

    }

    public IEnumerator GenerateKiller()
    {

        yield return new WaitForSeconds(7.0f);
        
        GameManagerPC.instance.hasAttackereArrived = true;
        Vector3 player = GameManagerPC.instance.playerCurrentPos;
        GameObject go = Instantiate(botPrefabs[2], new Vector3(player.x > 5 ? 2.25f : 4.75f,player.y,player.z -15f), botPrefabs[2].transform.rotation);
        go.SetActive(true);

        yield return new WaitForSeconds(1.3f);
        go.GetComponent<BotController>().currentSpeed = 70f;
        StartCoroutine(DefeatCops(go));
    }

    public IEnumerator DefeatCops(GameObject attacker)
    {
        yield return new WaitForSeconds(2f);
        attacker.transform.DOLocalMoveX(attacker.transform.localPosition.x > 5 ? -1f : 8f, .2f).OnComplete(
                                                        ()=> attacker.transform.DOLocalMoveX(attacker.transform.localPosition.x > 5 ? -1f : 8f, .2f));
    }
}
