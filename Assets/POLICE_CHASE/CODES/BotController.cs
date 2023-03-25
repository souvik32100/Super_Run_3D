using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum BotType
{
    enemy,
    civilian,
    attacker

}
public class BotController : MonoBehaviour
{
    public BotType botType;
    public GameObject botLights;
    public float currentSpeed;
    public float maxSpeed;
    public float speedIncreaseModifier;
    Rigidbody rb;
    public Animator anim;
    public Image scanBar;
    public bool hit;
    public float hitBoostTime;
    public float randomBoostMultiplier;
    public bool dead;
    public int lane;
    public GameObject enemyLight;
    [Header("Criminal Lane Change")]
    public float laneChangeDelay;
    public bool turnRight;
    public bool turnLeft;
    public bool turning;
    public GameObject minePrefab;
    public GameObject spikePrefab;
    public GameObject boostTrail;

    // Start is called before the first frame update
    void Start()
    { 
        //if (boostTrail != null)
        //    boostTrail.SetActive(false);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        hitBoostTime = 0;
        laneChangeDelay = 0;
        if (botType == BotType.enemy)
            scanBar.fillAmount = 0;

        if(botLights != null)
        {
            if (GameManagerPC.instance.dayNightToggleInt == 0) botLights.SetActive(false);
            else if (GameManagerPC.instance.dayNightToggleInt == 1) botLights.SetActive(true);

            if (lane == 1)//front light off
            {
                botLights.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        if(enemyLight != null)
        {
            if(GameManagerPC.instance.timeOfDay == TimeofDay.night)
                enemyLight.SetActive(true);
            else
                enemyLight.SetActive(false);
        }

      /*  if(botType == BotType.attacker)
        {
            Vector3 attackerRotation = new Vector3(this.gameObject.transform.rotation.x, 0, this.transform.rotation.z);
            this.gameObject.transform.rotation = Quaternion.Euler(attackerRotation);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSpeed < 0)
        {
            //rb.isKinematic = true;
            return;
        }
        if (GameManagerPC.instance.startType == StartType.standing)
            if (!GameManagerPC.instance.startGame) return;
        if (GameManagerPC.instance.victory)
        {
            
            if(currentSpeed <= 0)
            {
                //rb.isKinematic = true;
                return;
            }
            else
            {
                if(GameManagerPC.instance.vehicleType==VehicleType.car || GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                    currentSpeed -= Time.deltaTime * 30;
                if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
                    currentSpeed -= Time.deltaTime * 32;
            }
        }

        if (currentSpeed < maxSpeed)
        {
            currentSpeed += speedIncreaseModifier * Time.deltaTime;
        }

        if (!dead)
        {
           
        }
        //bot lane direction control
        if (lane == 0 && botType == BotType.civilian && !hit)//wrong way
        {
            transform.position -= Vector3.forward * (currentSpeed/2) * Time.deltaTime;
        }
        if (lane == 1 && botType == BotType.civilian && !hit)//right way
        {
            transform.position += Vector3.forward * currentSpeed * Time.deltaTime;
        }

        if(botType == BotType.enemy)
        {
            transform.position += Vector3.forward * currentSpeed * Time.deltaTime;
        }

        if (botType == BotType.attacker)
        {
            transform.position += Vector3.forward * currentSpeed * Time.deltaTime;
        }

        if (hit && botType == BotType.enemy)
        {
            hitBoostTime += Time.deltaTime;
            if(hitBoostTime > 2f)
            {

                currentSpeed -= randomBoostMultiplier;
                hitBoostTime = 0;
                hit = false;
            }
            
        }

        if(botType == BotType.enemy && !hit && !turnLeft && !turnRight && !GameManagerPC.instance.victory)
        {
            //transform.DOLocalRotate(new Vector3(0, 180, 0), .2f);
        }
        if (botType == BotType.civilian && !hit && !GameManagerPC.instance.victory)
        {
            if(lane == 0)
                transform.DOLocalRotate(new Vector3(0, 0, 0), .2f);
            if(lane == 1)
                transform.DOLocalRotate(new Vector3(0, 180, 0), .2f);
        }

        //BOT LANE SWITCH AI
        if (botType == BotType.enemy && !GameManagerPC.instance.victory && !hit && ( GameManagerPC.instance.startType == StartType.standing || ( GameManagerPC.instance.startType == StartType.chasing && GameManagerPC.instance.startChase)))
        {
            laneChangeDelay += Time.deltaTime;
            if (laneChangeDelay >= 4f && !turnRight)//right
            {
                //DROP SPIKE or BOMB
                //GameManagerPC.instance.thiefHead[GameManagerPC.instance.thiefIndex].SetActive(true);
                int i = Random.Range(0, 1);
                if (i == 0)
                {
                    int j = Random.Range(0, 2);
                    if (j == 0)
                    {
                        //GameObject mine = Instantiate(minePrefab, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - 1), Quaternion.identity);
                        //Destroy(mine, 10f);
                    }
                    else if (j == 1)
                    {
                       //GameObject spike = Instantiate(spikePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), spikePrefab.transform.rotation);
                       //Destroy(spike, 10f);
                    }
                }
            
                transform.DOLocalMoveX(transform.position.x + 7.5f, 0.3f).OnComplete(() =>
                {
                    //GameManagerPC.instance.thiefHead[GameManagerPC.instance.thiefIndex].SetActive(false);
                });
                //transform.DOLocalRotate(new Vector3(0, 190, 0), .3f).SetLoops(2, LoopType.Yoyo);
                turnRight = true;
                turnLeft = false;
                laneChangeDelay = 0;
            }
            if (laneChangeDelay >= 4f && turnRight && !turnLeft)//left
            {
                int i = Random.Range(0, 1);
                if (i == 0)
                {
                    int j = Random.Range(0, 2);
                    if (j == 0)
                    {
                        //GameObject mine = Instantiate(minePrefab, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - 1), Quaternion.identity);
                        //Destroy(mine, 10f);
                    }
                    else if (j == 1)
                    {
                        //GameObject spike = Instantiate(spikePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                        //Destroy(spike, 10f);
                    }
                }
            
                transform.DOLocalMoveX(transform.position.x - 7.5f, 0.3f).OnComplete(() =>
                {

                });
                //transform.DOLocalRotate(new Vector3(0, 170, 0), .3f).SetLoops(2, LoopType.Yoyo);
                turnRight = false;
                turnLeft = true;
                laneChangeDelay = 0;
            }
        }

    }
    private void OnCollisionEnter(Collision other)
    {
        if(botType==BotType.enemy && other.gameObject.GetComponent<BotController>() != null && other.gameObject.GetComponent<BotController>().botType == BotType.civilian && !other.gameObject.GetComponent<BotController>().dead)
        {
            other.transform.DOLocalMoveY(other.transform.position.y + 10, 0.5f).SetLoops(2, LoopType.Yoyo);
            GameObject hitFX = Instantiate(GameManagerPC.instance.carBlastFX, other.gameObject.transform.position, Quaternion.identity);
            Destroy(hitFX, 2f);
            
            //GameManagerPC.instance.OnHitComicFX(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 6, other.gameObject.transform.position.z),other.gameObject);
            
            SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomHitCarSFX());
            other.gameObject.GetComponent<BotController>().hit = true;
            other.gameObject.GetComponent<BotController>().dead = true;
            Destroy(other.gameObject);

            //GameObject angryEmo = Instantiate(GameManagerPC.instance.angryEmo, new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 6, other.gameObject.transform.position.z - 6), GameManagerPC.instance.angryEmo.transform.rotation);
            //angryEmo.transform.parent = gameObject.transform;
            //if (GameManagerPC.instance.cameraView == CameraView.sideView)
            //    angryEmo.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            //angryEmo.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 1f);
            //Destroy(angryEmo, 1f);
        }
        if (botType == BotType.civilian && other.gameObject.CompareTag("Ground") && hit)
        {
            dead = true;
            GameObject hitFX = Instantiate(GameManagerPC.instance.carHitImpactFX, transform.position, Quaternion.identity);
            Destroy(hitFX, 2f);
            Destroy(gameObject, 3f);
        }
        
        if(GameManagerPC.instance.hasAttackereArrived && botType == BotType.attacker && other.gameObject.GetComponent<BotController>() != null && other.gameObject.GetComponent<BotController>().botType == BotType.civilian && !other.gameObject.GetComponent<BotController>().dead)
        {
            other.transform.DOLocalMoveY(other.transform.position.y + 10, 0.5f).SetLoops(2, LoopType.Yoyo);
            GameObject hitFX = Instantiate(GameManagerPC.instance.carBlastFX, other.gameObject.transform.position, Quaternion.identity);
            Destroy(hitFX, 2f);
            
            //GameManagerPC.instance.OnHitComicFX(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 6, other.gameObject.transform.position.z), other.gameObject);
            
            SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomHitCarSFX());
            other.gameObject.GetComponent<BotController>().hit = true;
            other.gameObject.GetComponent<BotController>().dead = true;
            Destroy(other.gameObject);
        }
        //if (other.gameObject.CompareTag("crosser"))
        //{
        //    other.transform.DOLocalMoveY(other.transform.position.y + 5, 0.5f).SetLoops(2, LoopType.Yoyo);
        //    GameObject hitFX = Instantiate(GameManagerPC.instance.hitCivilianFX, other.gameObject.transform.position, Quaternion.identity);
        //    Destroy(hitFX, 2f);
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        if (botType == BotType.enemy && other.gameObject.CompareTag("scanArea") && !GameManagerPC.instance.victory && !GameManagerPC.instance.defeat)
        {
            GameManagerPC.instance.victory = true;
            StartCoroutine(GameManagerPC.instance.EndCutsceneDelayRoutine());
            SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().scanSFX);
            //other.gameObject.GetComponentInParent<Helicopter>().heliSpotLight.GetComponent<MeshCollider>().enabled=true;
            GameManagerPC.instance.player.currentSpeed = GameManagerPC.instance.enemy.GetComponent<BotController>().currentSpeed;
            GameManagerPC.instance.player.maxSpeed = GameManagerPC.instance.player.currentSpeed;
            GameManagerPC.instance.scanning = true;
            GameManagerPC.instance.player.gameObject.GetComponent<Helicopter>().heliSpotLight.GetComponent<MeshRenderer>().material = GameManagerPC.instance.player.gameObject.GetComponent<Helicopter>().scanningLightMat;
        }
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (botType == BotType.enemy && other.gameObject.CompareTag("scanArea") && !GameManagerPC.instance.victory)
    //    {
    //        GameManagerPC.instance.player.gameObject.GetComponent<Helicopter>().heliSpotLight.GetComponent<MeshRenderer>().material = GameManagerPC.instance.player.gameObject.GetComponent<Helicopter>().scanningLightMat;
    //        GameManagerPC.instance.scanDuration += Time.deltaTime;
    //        scanBar.fillAmount = (float)GameManagerPC.instance.scanDuration / (float)GameManagerPC.instance.scanDurationMax;
    //        if (GameManagerPC.instance.scanDuration >= GameManagerPC.instance.scanDurationMax && !GameManagerPC.instance.victory)
    //        {
    //            ///Misson Complete
    //            GameManagerPC.instance.victory = true;
    //            //GameManagerPC.instance.VibrationControl(120);
    //            StartCoroutine(GameManagerPC.instance.EndCutsceneDelayRoutine());

    //            int randIndex = Random.Range(0, 2);
    //            if (randIndex == 0) gameObject.transform.DOLocalRotate(new Vector3(0, -45, 0), 2f);
    //            if (randIndex == 1) gameObject.transform.DOLocalRotate(new Vector3(0, 45, 0), 2f);
    //        }
    //    }
    //}
    private void OnTriggerExit(Collider other)
    {
       
        if (botType == BotType.enemy && other.gameObject.CompareTag("scanArea"))
        {
            GameManagerPC.instance.scanning = false;
            GameManagerPC.instance.scanDuration -= Time.deltaTime;
            scanBar.fillAmount = (float)GameManagerPC.instance.scanDuration / (float)GameManagerPC.instance.scanDurationMax;
            GameManagerPC.instance.player.gameObject.GetComponent<Helicopter>().heliSpotLight.GetComponent<MeshRenderer>().material = GameManagerPC.instance.player.gameObject.GetComponent<Helicopter>().normalLightMat;
        }
    }
}
