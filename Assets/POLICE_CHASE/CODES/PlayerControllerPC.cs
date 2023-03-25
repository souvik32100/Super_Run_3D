using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using DG.Tweening;

public class PlayerControllerPC : MonoBehaviour
{
    [Header("Player Control")]
    //PLAYER
    public Animator anim;
    public float currentSpeed;
    public float maxSpeed;
    public float speedIncreaseModifier;
    private Vector3 moveVector;
    public Rigidbody rb;

    private float speed = 10.0f;
    public bool leanLeft;
    public bool leanRight;
    public bool dead;
    public bool spiked;
    private float playerCurrentPosX;
    private Camera cam;
    public GameObject cop;
    public GameObject cop2;
    [Header("Lane Control")]
    public int currentLane;

    [Header("Jump")]
    public bool grounded;
    public bool jumping;
    [Header("Slide")]
    public ParticleSystem slideFX;
    public bool sliding;
    [Header("Slope")]
    public bool slopeUp;
    public float slopeUpSpeed;
    private float initSlopeUpSpeed;
    public Transform rayOrigin;
    public Transform rayOrigin2;
    public float rayLength;
    [Header("PowerUps")]
    public GameObject magnet;
    public GameObject helmet;
    public float magnetDuration=5f;
    public float helmetDuration = 10f;
    public bool helmetOn;

    [Header("Swipe Detect")]
    //SWIPE DETECTION
    public float swipeThreshold = 50f;
    public float timeThreshold = 0.3f;

    public UnityEvent OnSwipeLeft;
    public UnityEvent OnSwipeRight;
    public UnityEvent OnSwipeUp;
    public UnityEvent OnSwipeDown;

    private Vector2 fingerDown;
    private DateTime fingerDownTime;
    private Vector2 fingerUp;
    private DateTime fingerUpTime;

    public GameObject redLight;
    public GameObject blueLight;
    public GameObject copLights;
    public GameObject playerHeadLight;
    public bool stopPlayer;
    public bool endSliding;
    int randIndex;

    public bool braking;
    public float brakeForce;

    [Header("Car Interior")]
    public GameObject steeringWheel;
    public GameObject gear;
    public GameObject brake;

    [Header("Rush Hour Control")]
    public int laneID;
    public float lastPos;
    public float startPos;
    public float travelTime;
    public int playerHealth;
    public int maxplayerHealth;
    public bool laneChanging;
    public bool laneChangeBoost;
    public bool canChangeLane;
    public GameObject driftTrail;
    [Header("Free Swipe Control")]
    private Vector3 touchPosition;
    private Vector3 direction;

    public float REFRESH_TIME = 0.1f;
    private Vector3 touchStartPos;
    float refreshDelta = 0.0f;
    [SerializeField]
    [Range(0, 20)]
    float smoothingTime = 10.0f; //decrease value to move faster

    public GameObject backlight;

    public GameObject endSun;

    public Animator enemyAnim;

    public GameObject headLight;

    public GameObject CarSpark;
    public GameObject CarSpark2;
    IEnumerator sparking()
    {
        if(spark)
        {
            yield return new WaitForSeconds(2f);
            CarSpark.SetActive(true);
            yield return new WaitForSeconds(1f);
            CarSpark.SetActive(false);
            StartCoroutine(sparking());
        }
       
    }
    // Start is called before the first frame update
    void Start()
    {
        
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        initSlopeUpSpeed = slopeUpSpeed;
        //transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
        currentLane = 0;
        if (GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
            maxplayerHealth = 6;
        //playerHealth = maxplayerHealth;
        cam = Camera.main;
        //SlopeDetect();
        randIndex = UnityEngine.Random.Range(0, 2);
        if (copLights != null) copLights.SetActive(false);
        if (playerHeadLight != null) 
            playerHeadLight.SetActive(false);

        GameManagerPC.instance.leftArrowParticle.SetActive(false);
    }
    int c = 0;
    // Update is called once per frame
    void Update()
    {
        if(GameManagerPC.instance.startChase==true&&c==0)
        {
            //StartCoroutine(sparking());
            c = 1;
        }
        if (GameManagerPC.instance.startType == StartType.standing)
        {
            if (!GameManagerPC.instance.startChase) return;
        }

        if (dead) return;
        if (GameManagerPC.instance.victory)
        {
            if (currentSpeed <= 0 && !stopPlayer)
            {
                endSun.SetActive(true);
                SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomEnemyDownSFX());
                GameObject hitFX = Instantiate(GameManagerPC.instance.enemyDownFX, GameManagerPC.instance.enemy.transform.position, Quaternion.identity);
                GameManagerPC.instance.VibrationControl(120);
                //Destroy(hitFX, 2f);
                GameManagerPC.instance.enemy.GetComponent<Rigidbody>().isKinematic = true;
                
                UIManagerPC.instance.gamePanel.SetActive(false);
                if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
                {
                    var gmPC = GameManagerPC.instance.car.GetComponent<PlayerControllerPC>();
                    //gmPC.cop.SetActive(true);
                    //gmPC.cop.GetComponent<Animator>().SetTrigger("aim");
                    //gmPC.cop2.SetActive(true);
                    //gmPC.cop2.GetComponent<Animator>().SetTrigger("aim");
                    //gmPC.cop2.transform.DOLookAt(GameManagerPC.instance.thiefPrefab[GameManagerPC.instance.thiefIndex].transform.position, 0.5f);
                    //gmPC.cop.transform.DOLookAt(GameManagerPC.instance.thiefPrefab[GameManagerPC.instance.thiefIndex].transform.position, 0.5f);
                }
                else
                {
                    cop.SetActive(true);
                    cop.GetComponent<Animator>().SetTrigger("aim");
                    cop2.SetActive(true);
                    cop2.GetComponent<Animator>().SetTrigger("aim");
                    cop2.transform.DOLookAt(GameManagerPC.instance.thiefPrefab[GameManagerPC.instance.thiefIndex].transform.position, 0.5f);
                    cop.transform.DOLookAt(GameManagerPC.instance.thiefPrefab[GameManagerPC.instance.thiefIndex].transform.position, 0.5f);

                    if (GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                    {
                        MonsterTruck mt = gameObject.GetComponentInChildren<MonsterTruck>();
                        mt.wfl.GetComponent<Animator>().enabled = false;
                        mt.wfr.GetComponent<Animator>().enabled = false;
                        mt.wbl.GetComponent<Animator>().enabled = false;
                        mt.wbr.GetComponent<Animator>().enabled = false;
                    }
                
                }


                if (GameManagerPC.instance.thiefIndex == 0)
                {
                    GameManagerPC.instance.thiefPrefab[GameManagerPC.instance.thiefIndex].SetActive(true);
                }
                else if (GameManagerPC.instance.thiefIndex == 1)
                {
                    GameManagerPC.instance.thiefPrefab[GameManagerPC.instance.thiefIndex].SetActive(true);
                }


                StartCoroutine( GameManagerPC.instance.EndSlowMoRoutine());

                stopPlayer = true;
                return;
            }
            else if(!stopPlayer)
            {
                if (GameManagerPC.instance.vehicleType == VehicleType.car || GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                {
                    if (randIndex == 0) transform.DOLocalRotate(new Vector3(0, 45, 0), 1.3f);
                    if (randIndex == 1) transform.DOLocalRotate(new Vector3(0, -45, 0), 1.3f);

                    currentSpeed -= Time.deltaTime * 40;
                    //rb.isKinematic = true;
                    if (!endSliding)
                    {
                        GameManagerPC.instance.EndFPPtoTPPSwitch();
                        //SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().brakeSFX);
                        endSliding = true;
                    }
                }
                if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
                {
                    currentSpeed -= Time.deltaTime * 40;
                }
                if (GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                {
                    //wfl.GetComponent<Animator>().enabled = false;
                    //wfr.GetComponent<Animator>().enabled = false;
                    //wbl.GetComponent<Animator>().enabled = false;
                    //wbr.GetComponent<Animator>().enabled = false;
                }        
            }
        }
        if (GameManagerPC.instance.defeat)
        {
            if (currentSpeed <= 0 && !stopPlayer)
            {
                SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomEnemyDownSFX());
                GameObject hitFX = Instantiate(GameManagerPC.instance.enemyDownFX, GameManagerPC.instance.policeExterior.transform.position, Quaternion.identity);
                hitFX.transform.parent = GameManagerPC.instance.policeExterior.transform;
                hitFX.transform.localPosition = new Vector3(0, 1, 8);
                
                stopPlayer = true;
                return;
            }
            else if (!stopPlayer)
            {
                if (GameManagerPC.instance.vehicleType == VehicleType.car || GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                {
                    UIManagerPC.instance.gamePanel.SetActive(false);
                    //if (randIndex == 0) transform.DOLocalRotate(new Vector3(0, 45, 0), 2f);
                    //if (randIndex == 1) transform.DOLocalRotate(new Vector3(0, -45, 0), 2f);
                    //transform.DOLocalRotate(new Vector3(180, 0, 0), 2f);
                    GameManagerPC.instance.policeExterior.transform.GetChild(0).GetComponent<Animator>().enabled = true;
                    headLight.SetActive(false);
                    //Debug.Log(GameManagerPC.instance.carInstance.GetComponent<Animator>());
                    currentSpeed = 0;//Time.deltaTime * 27;
                    if (!endSliding)
                    {
                        GameManagerPC.instance.EndFPPtoTPPSwitch();
                        SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().brakeSFX);
                        endSliding = true;
                    }
                }
                if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
                {
                    currentSpeed -= Time.deltaTime * 34;
                }
            }
        }

        //SlopeDetect();
        //SPEED UP OVER TIME
        if (currentSpeed < maxSpeed && !GameManagerPC.instance.victory && !GameManagerPC.instance.defeat)
        {
            currentSpeed += speedIncreaseModifier * Time.deltaTime;
        }

        
        if(GameManagerPC.instance.vehicleType == VehicleType.car || GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
        {
                //MOVE FORWARD
                if (!stopPlayer)
                {
                    transform.position += Vector3.forward * currentSpeed * Time.deltaTime;
                }

        }
        if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
        {
                //MOVE FORWARD
                if (!stopPlayer)
                {
                    transform.position += Vector3.forward * currentSpeed * Time.deltaTime;
                }
        }

        if (slopeUp)
        {
            //grounded = false;
            //transform.Translate(new Vector3(0, 1, 1) * slopeUpSpeed * Time.deltaTime);
            transform.position += new Vector3(0, 1, 1) * slopeUpSpeed * Time.deltaTime;
            //transform.Rotate(new Vector3(-1, 0, 0));
        }

        if (!GameManagerPC.instance.victory && !GameManagerPC.instance.defeat && !spiked)
        {
            ////////////PLAYER CONTROL 
            //SwipeControl();
            
            //HoldToMoveControl();
            //TapToChangeLane();
            if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
                TouchDrag();
            if (GameManagerPC.instance.vehicleType == VehicleType.car || GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                TapToChangeLanePoliceQuest(); //HoldToMoveControl();  //FreeSwipeControl();

            if (laneChanging && laneID == 0)
            {
                GameManagerPC.instance.policeExterior.transform.GetChild(0).localRotation = Quaternion.Slerp(GameManagerPC.instance.policeExterior.transform.GetChild(0).localRotation, Quaternion.Euler(0, -15, 3.5f), Time.deltaTime * 5f);
                Invoke("CarRotator",0.25f);
            }

            if (laneChanging && laneID == 1)
            {
                GameManagerPC.instance.policeExterior.transform.GetChild(0).localRotation = Quaternion.Slerp(GameManagerPC.instance.policeExterior.transform.GetChild(0).localRotation, Quaternion.Euler(0, 15, 3.5f), Time.deltaTime * 5f);
                Invoke("CarRotator", 0.25f);
            }
        }

        //Side boundary check   
        if (transform.position.x > 8f)
            transform.position = new Vector3(8f, transform.position.y, transform.position.z);
        if (transform.position.x < -2f)
            transform.position = new Vector3(-2f, transform.position.y, transform.position.z);
        //if (transform.position.x >= -(speed / 100 + .05f) && transform.position.x <= (speed / 100 + .05f))
        //    transform.position = new Vector3(0, transform.position.y, transform.position.z);

        moveVector = Vector3.zero;

        if (GameManagerPC.instance.vehicleType == VehicleType.car)
        {
            if (!laneChanging && !GameManagerPC.instance.victory && !spiked)
            {
                //transform.DOLocalRotate(new Vector3(transform.rotation.x, 0, 0), .25f);
            }
        }
        if (GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
        {
            if (!laneChanging && !GameManagerPC.instance.victory && !spiked)
            {
                transform.GetChild(0).DOLocalRotate(new Vector3(transform.rotation.x, 0, 0), .25f);
                MonsterTruck mt= gameObject.GetComponentInChildren<MonsterTruck>();
                mt.wfl.transform.DOLocalRotate(new Vector3(0, 0, 0), .2f);
                mt.wfr.transform.DOLocalRotate(new Vector3(0, 0, 0), .2f);
            }
        }
        if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
        {
            if (!laneChanging && !GameManagerPC.instance.victory)
            {
                transform.DOLocalRotate(new Vector3(-5, 180, 0), .3f);
            }
        }

        GameManagerPC.instance.playerCurrentPos = this.transform.localPosition;
    }

    public void CarRotator()
    {
        GameManagerPC.instance.policeExterior.transform.GetChild(0).localRotation = Quaternion.Slerp(GameManagerPC.instance.policeExterior.transform.GetChild(0).localRotation, Quaternion.Euler(0, 0, 0f), Time.deltaTime * 15f);
    }
    public GameObject CoinCollectFX;
    #region VARIOUS CONTROLS
    private void CheckSwipe()
    {
        float duration = (float)this.fingerUpTime.Subtract(this.fingerDownTime).TotalSeconds;
        if (duration > this.timeThreshold) return;

        float deltaX = this.fingerDown.x - this.fingerUp.x;
        if (Mathf.Abs(deltaX) > this.swipeThreshold)
        {
            if (deltaX > 0)
            {
                this.OnSwipeRight.Invoke();
                leanRight = true;
                leanLeft = false;
                transform.DOLocalMoveX(transform.position.x + 4f,0.3f).OnComplete(()=>
                {
                    leanRight = false;
                });
               
                if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
                {
                    transform.DOLocalRotate(new Vector3(-5, 180, 10), .3f).SetLoops(2, LoopType.Yoyo);
                }
                if (GameManagerPC.instance.vehicleType == VehicleType.car)
                {
                    transform.DOLocalRotate(new Vector3(0, 10, 0), .3f).SetLoops(2, LoopType.Yoyo);
                    steeringWheel.transform.DOLocalRotate(new Vector3(0, 45, 0), 0.5f).SetLoops(2, LoopType.Yoyo);
                }

                if (currentLane == 0) currentLane = 1;
                else if (currentLane == -1) currentLane = 0;
            }
            else if (deltaX < 0)
            {
                this.OnSwipeLeft.Invoke();
                leanLeft = true;
                leanRight = false;
                transform.DOLocalMoveX(transform.position.x - 4f, 0.3f).OnComplete(() =>
                {
                    leanLeft = false;
                });
                if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
                {
                    transform.DOLocalRotate(new Vector3(-5, 180, -10), .3f).SetLoops(2, LoopType.Yoyo);
                }
                if (GameManagerPC.instance.vehicleType == VehicleType.car)
                {
                    transform.DOLocalRotate(new Vector3(0, -10, 0), .3f).SetLoops(2, LoopType.Yoyo);
                    steeringWheel.transform.DOLocalRotate(new Vector3(0, -45, 0), 0.5f).SetLoops(2, LoopType.Yoyo);
                }
                if (currentLane == 0) currentLane = -1;
                else if (currentLane == 1) currentLane = 0;
            }
        }

        //float deltaY = fingerDown.y - fingerUp.y;
        //if (Mathf.Abs(deltaY) > this.swipeThreshold)
        //{
        //    if (deltaY > 0 && grounded && !sliding)
        //    {
        //        this.OnSwipeUp.Invoke();
        //        rb.useGravity = false;
        //        jumping = true;
        //        //anim.SetTrigger("Jump");
        //        transform.DOLocalMoveY(transform.position.y + 2.25f, 0.5f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        //        {
        //            rb.useGravity = true;
        //            jumping = false;
        //        });
        //    }
        //    else if (deltaY < 0 && !sliding)
        //    {
        //        sliding = true;
        //        this.OnSwipeDown.Invoke();
        //        //anim.SetTrigger("Slide");
        //        slideFX.Play();
        //        StartCoroutine(slideColliderControl());
        //    }
        //}

        this.fingerUp = this.fingerDown;
    }
    public void SwipeControl()
    {
        //swipe
        if (Input.GetMouseButtonDown(0))
        {
            this.fingerDown = Input.mousePosition;
            this.fingerUp = Input.mousePosition;
            this.fingerDownTime = DateTime.Now;

            //if (canStretch)
            //{
            //    //size up
            //    spine.transform.position = new Vector3(spine.transform.position.x, Mathf.Lerp(currentHeight, maxHeight, 1.5f), spine.transform.position.z);
            //}
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.fingerDown = Input.mousePosition;
            this.fingerUpTime = DateTime.Now;
            this.CheckSwipe();

        }
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                this.fingerDown = touch.position;
                this.fingerUp = touch.position;
                this.fingerDownTime = DateTime.Now;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                this.fingerDown = touch.position;
                this.fingerUpTime = DateTime.Now;
                this.CheckSwipe();
            }
        }
    }
    public void FreeSwipeControl()
    {
        if (Input.GetMouseButton(0))
        {
            touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = (touchPosition - transform.position).normalized;
            //rb.velocity = new Vector2(direction.x, direction.y) * 50;
            
            //transform.position = new Vector3(Mathf.Lerp(transform.position.x, Input.mousePosition.x ,0.1f * Time.deltaTime), transform.position.y, transform.position.z);
            transform.position = new Vector3(transform.position.x + Input.mousePosition.x * 0.1f * Time.deltaTime, transform.position.y, transform.position.z);
        }

    }
void TouchDrag()
{
    if (Input.GetMouseButtonDown(0))
    {
        touchStartPos = Input.mousePosition;
        refreshDelta = 0.0f;
    }
    if (Input.GetMouseButton(0))
    {

        refreshDelta += Time.deltaTime;
        Vector3 diff = (Input.mousePosition - touchStartPos);
        Vector3 finalPos = transform.position + diff;

        if (refreshDelta >= REFRESH_TIME)
        {

            refreshDelta = 0.0f;
            touchStartPos = Input.mousePosition;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(finalPos.x, transform.position.y, transform.position.z), Time.deltaTime / smoothingTime);

    }
}
public void HoldToMoveControl()
    {
        if (Input.GetMouseButton(0))
        {
            if(transform.position.x >= lastPos)
            {
                //leanLeft = true;
                //leanRight = false;
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, lastPos, 5f * Time.deltaTime), transform.position.y, transform.position.z);
                //transform.DOLocalRotate(new Vector3(0, -10, 0), .3f).SetLoops(2, LoopType.Yoyo);
              
                if (transform.position.x >= 1.25f)
                {
                    laneChanging = true;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y - 10, transform.rotation.z), 50f * Time.deltaTime);
                }
                else
                {
                    laneID = 1;
                    laneChanging = false;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z), 50f * Time.deltaTime);
                }
                
            }

        }
        //if (Input.GetMouseButtonUp(0))
        else
        {
            if (transform.position.x < startPos)
            {
                //leanLeft = false;
                //leanRight = true;
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, startPos, 5f * Time.deltaTime), transform.position.y, transform.position.z);
                //transform.DOLocalRotate(new Vector3(0, 10, 0), .3f).SetLoops(2, LoopType.Yoyo);
                if (transform.position.x <= 4.75f)
                {
                    laneChanging = true;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y + 10, transform.rotation.z), 50f * Time.deltaTime);
                }
                else
                {
                    laneID = 0;
                    laneChanging = false;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z), 50f * Time.deltaTime);
                }
            }
        }

        //lane wise max speed change 
        //if (laneChanging && !laneChangeBoost)
        //{
        //    speedIncreaseModifier += 20;
        //    maxSpeed += 10;
        //    laneChangeBoost = true;

        //}
        //if (!laneChanging && laneChangeBoost)
        //{
        //    speedIncreaseModifier -= 20;
        //    maxSpeed -= 10;
        //    laneChangeBoost = false;
        //}
    }
    public void TapToChangeLane()
    {
        
        if (Input.GetMouseButtonDown(0) && !laneChanging)
        {
            if (laneID == 0)
            {
                laneChanging = true;
                transform.DOLocalMoveX(transform.position.x - 6.5f, 0.25f).OnComplete(() =>
                {
                    laneID = 1;
                    laneChanging = false;
                });
                if (GameManagerPC.instance.vehicleType == VehicleType.car)
                {
                    transform.DOLocalRotate(new Vector3(0, -5, 0), .2f).SetLoops(2, LoopType.Yoyo);
                    steeringWheel.transform.DOLocalRotate(new Vector3(0, 45, 0), 0.5f).SetLoops(2, LoopType.Yoyo);
                }
            }
            else if (laneID == 1)
            {
                laneChanging = true;
                transform.DOLocalMoveX(transform.position.x + 6.5f, 0.25f).OnComplete(() =>
                {
                    laneID = 0;
                    laneChanging = false;
                });
                if (GameManagerPC.instance.vehicleType == VehicleType.car)
                {
                    transform.DOLocalRotate(new Vector3(0, 5, 0), .2f).SetLoops(2, LoopType.Yoyo);
                    steeringWheel.transform.DOLocalRotate(new Vector3(0, 45, 0), 0.5f).SetLoops(2, LoopType.Yoyo);
                }
            }
        }
    }
    public void TapToChangeLanePoliceQuest()
    {
        
        if (Input.GetMouseButtonDown(0) && !laneChanging)
        {
            GameManagerPC.instance.tapCount++;

            if (GameManagerPC.instance.tapCount < 2)
            {
                //GameManagerPC.instance.leftArrowParticle.SetActive(true);
                //GameManagerPC.instance.leftArrowParticle.GetComponent<ParticleSystem>().Play();
            }

            if (GameManagerPC.instance.tapCount > 1)
            {
                if (GameManagerPC.instance.tapCount >= GameManagerPC.instance.tapRequired && Screen.height / Screen.width >= 1)
                {
                    //Debug.LogError(" //show download button");
                    //UIManagerPC.instance.btnDownloadInGameP.SetActive(true);
                    //UIManagerPC.instance.btnDownloadInGameP.transform.GetComponent<RectTransform>().DOScale(Vector3.one * .9f, .2f).SetLoops(2, LoopType.Yoyo);
                }
                else if (GameManagerPC.instance.tapCount >= GameManagerPC.instance.tapRequired && Screen.height / Screen.width < 1)
                {
                    if (GameManagerPC.instance.cameraView == CameraView.backView)
                    {
                        UIManagerPC.instance.btnDownloadInGameL.SetActive(true);
                        UIManagerPC.instance.btnDownloadInGameSideViewL.SetActive(false);
                        //UIManagerPC.instance.btnDownloadInGameL.transform.GetComponent<RectTransform>().DOScale(Vector3.one * .9f, .2f).SetLoops(2, LoopType.Yoyo);
                    }
                    else
                    {
                        UIManagerPC.instance.btnDownloadInGameL.SetActive(false);
                        UIManagerPC.instance.btnDownloadInGameSideViewL.SetActive(true);
                        UIManagerPC.instance.btnDownloadInGameSideViewL.transform.GetComponent<RectTransform>().DOScale(Vector3.one * .9f, .2f).SetLoops(2, LoopType.Yoyo);
                    }

                }

                UIManagerPC.instance.imgTaptochange.SetActive(false);
                GameManagerPC.instance.leftArrowParticle.SetActive(false);

                if (laneID == 0)
                {
                    SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomLaneTurnSFX());
                    laneChanging = true;
                    
                    if (GameManagerPC.instance.vehicleType == VehicleType.car || GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                        driftTrail.SetActive(true); //CarSpark2.SetActive(true);
                    transform.DOLocalMoveX(transform.position.x - 6.5f, 0.3f).OnComplete(() =>
                    {
                        //transform.DOShakeScale(0.2f,0.1f);

                        laneID = 1;
                        laneChanging = false;
                        if (GameManagerPC.instance.vehicleType == VehicleType.car || GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                            driftTrail.SetActive(false); //CarSpark2.SetActive(false);
                        
                        
                    });
                    if (GameManagerPC.instance.vehicleType == VehicleType.car)
                    {

                    }
                    if (GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                    {
                        MonsterTruck mt = gameObject.GetComponentInChildren<MonsterTruck>();
                        mt.wfl.GetComponent<Animator>().enabled = false;
                        mt.wfr.GetComponent<Animator>().enabled = false;
                        transform.GetChild(0).DOLocalRotate(new Vector3(0, -7, 0), .2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                        {
                            mt.wfl.GetComponent<Animator>().enabled = true;
                            mt.wfr.GetComponent<Animator>().enabled = true;
                        });
                        mt.wfl.transform.DOLocalRotate(new Vector3(0, -18, 0), .2f).SetLoops(2, LoopType.Yoyo);
                        mt.wfr.transform.DOLocalRotate(new Vector3(0, -18, 0), .2f).SetLoops(2, LoopType.Yoyo);
                    }

                }
                else if (laneID == 1)
                {
                    SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomLaneTurnSFX());
                    laneChanging = true;
                    
                    if (GameManagerPC.instance.vehicleType == VehicleType.car || GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                        driftTrail.SetActive(true); //CarSpark2.SetActive(true);
                    transform.DOLocalMoveX(transform.position.x + 6.5f, 0.3f).OnComplete(() =>
                    {
                        laneID = 0;
                        laneChanging = false;
                        if (GameManagerPC.instance.vehicleType == VehicleType.car || GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                            driftTrail.SetActive(false); //CarSpark2.SetActive(false);
                        
                    });
                    if (GameManagerPC.instance.vehicleType == VehicleType.car)
                    {
                        //GameManagerPC.instance.policeExterior.transform.DOLocalRotate(new Vector3(0, 5, -3.5f), .2f).SetLoops(2, LoopType.Yoyo);
                        //GameManagerPC.instance.policeExterior.GetComponent<Animator>().ResetTrigger("rotateL");
                        //GameManagerPC.instance.policeExterior.GetComponent<Animator>().ResetTrigger("rotateR");
                        //GameManagerPC.instance.policeExterior.GetComponent<Animator>().SetTrigger("rotateR");
                    }
                    if (GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
                    {
                        MonsterTruck mt = gameObject.GetComponentInChildren<MonsterTruck>();
                        mt.wfl.GetComponent<Animator>().enabled = false;
                        mt.wfr.GetComponent<Animator>().enabled = false;
                        transform.GetChild(0).DOLocalRotate(new Vector3(0, 7, 0), .2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                        {
                            mt.wfl.GetComponent<Animator>().enabled = true;
                            mt.wfr.GetComponent<Animator>().enabled = true;
                        });
                        mt.wfl.transform.DOLocalRotate(new Vector3(0, 18, 0), .2f).SetLoops(2, LoopType.Yoyo);
                        mt.wfr.transform.DOLocalRotate(new Vector3(0, 18, 0), .2f).SetLoops(2, LoopType.Yoyo);
                    }
                }
            }


        }
    }
#endregion
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Coin"))
        {
            //Instantiate(CoinCollectFX, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            
        }
        if (other.gameObject.CompareTag("enemyLcol"))
        {
            GameObject spark = Instantiate(GameManagerPC.instance.hitSparkFX, new Vector3(other.gameObject.transform.position.x - 2, other.gameObject.transform.position.y, other.gameObject.transform.position.z), GameManagerPC.instance.hitSparkFX.transform.rotation);
            //spark.transform.parent = other.gameObject.transform;
            Destroy(spark, 1.5f);
        }
        if (other.gameObject.CompareTag("enemyRcol"))
        {
            GameObject spark = Instantiate(GameManagerPC.instance.hitSparkFX, new Vector3(other.gameObject.transform.position.x - 3, other.gameObject.transform.position.y, other.gameObject.transform.position.z), GameManagerPC.instance.hitSparkFX.transform.rotation);
            //spark.transform.parent = other.gameObject.transform;
            Destroy(spark, 1.5f);
        }
        if (other.gameObject.CompareTag("enemyBcol"))
        {
            GameObject spark = Instantiate(GameManagerPC.instance.hitSparkFX, new Vector3(other.gameObject.transform.position.x - 3.5f, other.gameObject.transform.position.y, other.gameObject.transform.position.z - 2), GameManagerPC.instance.hitSparkFX.transform.rotation);
            //spark.transform.parent = other.gameObject.transform;
            Destroy(spark, 1.5f);
        }

        //if (other.gameObject.CompareTag("spike"))
        //{
        //    PlayerHealthDecrease(2);
        //    //GameObject hitFX = Instantiate(GameManagerPC.instance.carBlastFX, other.gameObject.transform.position, Quaternion.identity);
        //    //Destroy(hitFX, 2f);
        //    //Destroy(other.gameObject);
        //    SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().tireBlastSFX);
        //    GameManagerPC.instance.VibrationControl(100);
        //    CameraFollow.Instance.camShake = true;
        //    Camera.main.DOShakeRotation(0.5f, 1, 10, 50).OnComplete(() =>
        //    {
        //        CameraFollow.Instance.camShake = false;

        //    });

        //    spiked = true;
        //    transform.DOLocalRotate(new Vector3(0, -5, 0f), .2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        //    {
        //        transform.DOLocalRotate(new Vector3(0, 5, 0f), .2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        //        {
        //            spiked = false;
        //        });     
        //    });

        //    //other.gameObject.transform.parent.gameObject.SetActive(false);
        //}
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        //CAR CHASE
        if (!GameManagerPC.instance.victory && !GameManagerPC.instance.defeat && GameManagerPC.instance.hasAttackereArrived)
        {
            if (other.gameObject.GetComponent<BotController>() != null && other.gameObject.GetComponent<BotController>().botType == BotType.attacker)
            {
                SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomHitCarSFX());

                if (GameManagerPC.instance.cameraView == CameraView.backView)
                {
                    
                    CameraFollow.Instance.camShake = true;
                    Camera.main.DOShakeRotation(0.5f, 1, 10, 50).OnComplete(() =>
                    {
                        CameraFollow.Instance.camShake = false;
                    });
                }
                if (randIndex == 0) other.gameObject.transform.DOLocalMoveX(-1f, 2f).OnComplete(
                                                    ()=> other.gameObject.GetComponent<BotController>().currentSpeed +=5);
                if (randIndex == 1) other.gameObject.transform.DOLocalMoveX(8f, 2f).OnComplete(
                                                    () => other.gameObject.GetComponent<BotController>().currentSpeed += 5);
                // other.transform.DOLocalMoveY(other.transform.position.y + 12, .5f).SetLoops(2, LoopType.Yoyo);
                other.gameObject.GetComponent<BotController>().hit = true;
                GameObject hitFX = Instantiate(GameManagerPC.instance.carBlastFX, other.gameObject.transform.position, Quaternion.identity);
                Destroy(hitFX, 2f);
                
                //GameManagerPC.instance.OnHitComicFX(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 10, other.gameObject.transform.position.z + 5), other.gameObject);
               
                // Destroy(other.gameObject,.5f);
                PlayerHealthDecrease(2);
            }
        }

        if (other.gameObject.GetComponent<BotController>() != null && other.gameObject.GetComponent<BotController>().botType == BotType.enemy)
        {
            if(other.gameObject.GetComponent<BotController>().botType == BotType.enemy)
                GameManagerPC.instance.enemyHealth--;
            
            
            if (GameManagerPC.instance.enemyHealth <= 0 && !GameManagerPC.instance.victory && !GameManagerPC.instance.defeat)
            {
                
                //GameObject hitFX = Instantiate(GameManager.instance.hitSparkFX, new Vector3(other.gameObject.transform.position.x + 0.5f, other.gameObject.transform.position.y, other.gameObject.transform.position.z), GameManager.instance.carHitImpactFX.transform.rotation,other.gameObject.transform);
                //hitFX.GetComponent<ParticleSystem>().loop=true;
                //GameObject hitFX2 = Instantiate(GameManager.instance.hitSparkFX, new Vector3(other.gameObject.transform.position.x - 0.5f, other.gameObject.transform.position.y, other.gameObject.transform.position.z), GameManager.instance.carHitImpactFX.transform.rotation, other.gameObject.transform);
                //hitFX2.GetComponent<ParticleSystem>().loop = true;

                //GameManagerPC.instance.OnHitComicFX(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 5, other.gameObject.transform.position.z), other.gameObject);

                //GameManagerPC.instance.VibrationControl(120);

                //GameObject deadEmo = Instantiate(GameManagerPC.instance.deadEmo, new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 6, other.gameObject.transform.position.z), GameManagerPC.instance.angryEmo.transform.rotation);
                //deadEmo.transform.parent = other.gameObject.transform;
                //if (GameManagerPC.instance.cameraView == CameraView.sideView)
                //    deadEmo.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                //deadEmo.transform.DOPunchScale(new Vector3(0.07f, 0.07f, 0.07f), 1f);
                //Destroy(deadEmo, 3f);

                //if (randIndex == 0) other.gameObject.transform.DOLocalRotate(new Vector3(0, -45, 0), 2f);
                //if (randIndex == 1) other.gameObject.transform.DOLocalRotate(new Vector3(0, 45, 0), 2f);
                //other.gameObject.transform.DOLocalMoveY(other.transform.position.y + 3, 0.5f).SetLoops(2, LoopType.Yoyo);
                other.gameObject.transform.DOLocalMoveY(other.transform.position.y + 3, 0.5f);
                enemyAnim.enabled = true;
                GameManagerPC.instance.victory = true;
                GetComponent<BoxCollider>().enabled = false;
                other.gameObject.GetComponent<BoxCollider>().enabled = false;
                SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomHitCarSFX());
                //Mission Complete
                StartCoroutine(GameManagerPC.instance.EndCutsceneDelayRoutine());

                //Debug.Log("FB_EVENT_SUCCESS");
                //FPG.FacebookManager.GetInstance().LogEvent("CarChase_Success");
                //StartCoroutine(VictorySlowMoRoutine());
            }
            else if(!GameManagerPC.instance.victory)
            {
                int randInd = UnityEngine.Random.Range(0, 2);
                enemyAnim.enabled = true;
                //if (randInd == 0) other.gameObject.transform.DOLocalRotate(new Vector3(0, 160, 0), .25f).SetLoops(2, LoopType.Yoyo);
                //else if (randInd == 1) other.gameObject.transform.DOLocalRotate(new Vector3(0, 200, 0), .25f).SetLoops(2, LoopType.Yoyo);
            }



            //int randIndPlayer = UnityEngine.Random.Range(0, 2);
            //if(randIndPlayer==0) gameObject.transform.DOLocalRotate(new Vector3(5, 160, 0), .125f).SetLoops(2, LoopType.Yoyo);
            //else if (randIndPlayer == 1) gameObject.transform.DOLocalRotate(new Vector3(5, 200, 0), .125f).SetLoops(2, LoopType.Yoyo);
            if (GameManagerPC.instance.enemyHealth >= 0)
            {
                SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomEnemyDownSFX());
                //CameraFollow.Instance.camShake = true;
                //Camera.main.DOShakeRotation(0.5f, 3, 10, 50).OnComplete(() =>
                //{
                //    CameraFollow.Instance.camShake = false;

                //});
                other.gameObject.GetComponent<BotController>().hit = true;
                other.gameObject.GetComponent<BotController>().randomBoostMultiplier = UnityEngine.Random.Range(10, 21);
                other.gameObject.GetComponent<BotController>().currentSpeed += other.gameObject.GetComponent<BotController>().randomBoostMultiplier;
                GameObject hitFX = Instantiate(GameManagerPC.instance.carHitImpactFX, other.gameObject.transform.position, GameManagerPC.instance.carHitImpactFX.transform.rotation);
                Destroy(hitFX, 2f);
            }

        }
       
      
        if (GameManagerPC.instance.vehicleType == VehicleType.car && other.gameObject.GetComponent<BotController>() != null && other.gameObject.GetComponent<BotController>().botType == BotType.civilian && !other.gameObject.GetComponent<BotController>().hit && !other.gameObject.GetComponent<BotController>().dead)
        {
            
            SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomHitCarSFX());
            GameManagerPC.instance.VibrationControl(100);
            if (GameManagerPC.instance.cameraView == CameraView.backView)
            {

                CameraFollow.Instance.camShake = true;
                Camera.main.DOShakeRotation(0.75f, 2, 20, 50).OnComplete(() =>
               {
                   CameraFollow.Instance.camShake = false;

               });
            }
            other.transform.DOLocalMoveY(other.transform.position.y + 12, 0.5f).SetLoops(2, LoopType.Yoyo);
            other.gameObject.GetComponent<BotController>().hit = true;
            GameObject hitFX = Instantiate(GameManagerPC.instance.carBlastFX, other.gameObject.transform.position, Quaternion.identity);
            Destroy(hitFX, 2f);
           
            //GameManagerPC.instance.OnHitComicFX(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 10, other.gameObject.transform.position.z + 5), other.gameObject);
            
            Destroy(other.gameObject);
           
            //GameObject angEmo = Instantiate(GameManagerPC.instance.angryEmo, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 5, gameObject.transform.position.z), GameManagerPC.instance.angryEmo.transform.rotation);
            //angEmo.transform.parent = gameObject.transform;
            //if(GameManagerPC.instance.cameraView==CameraView.sideView)
            //    angEmo.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            //angEmo.transform.DOPunchScale(new Vector3(0.125f,0.125f,0.125f),1f);
            //Destroy(angEmo, 1.2f);

            PlayerHealthDecrease(1);
        }
        if (GameManagerPC.instance.vehicleType == VehicleType.car && other.gameObject.GetComponent<BotController>() != null && other.gameObject.GetComponent<BotController>().botType == BotType.civilian && other.gameObject.GetComponent<BotController>().dead && other.gameObject.layer != 28)
        {
            
            SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomHitCarSFX());
            GameManagerPC.instance.VibrationControl(100);

            if (GameManagerPC.instance.cameraView == CameraView.backView)
            {
                CameraFollow.Instance.camShake = true;
                Camera.main.DOShakeRotation(0.5f, 1, 10, 50).OnComplete(() =>
                {
                    CameraFollow.Instance.camShake = false;

                });
            }
            
            int rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0)
            {
                other.transform.DOLocalMoveX(other.transform.position.x + 2, 0.35f);
                other.transform.DOLocalRotate(new Vector3(0, other.transform.rotation.y + 135, 0), 0.5f);
            }
            else if (rand == 1)
            {
                other.transform.DOLocalMoveX(other.transform.position.x - 2, 0.35f);
                other.transform.DOLocalRotate(new Vector3(0, other.transform.rotation.y - 135, 0), 0.5f);
            }
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            other.gameObject.layer = 28;//botDead layer
            GameObject hitFX = Instantiate(GameManagerPC.instance.carBlastFX, other.gameObject.transform.position, Quaternion.identity);
            Destroy(hitFX, 2f);
            
            //GameManagerPC.instance.OnHitComicFX(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 5, other.gameObject.transform.position.z), other.gameObject);
            
            Destroy(other.gameObject, 3f);
            PlayerHealthDecrease(1);

        }
        //if (other.gameObject.CompareTag("crosser"))
        //{
        //    CameraFollow.Instance.camShake = true;
        //    Camera.main.DOShakeRotation(0.5f, 1, 10, 50).OnComplete(() =>
        //    {
        //        CameraFollow.Instance.camShake = false;

        //    });
        //    other.transform.DOLocalMoveY(other.transform.position.y + 5, 0.5f).SetLoops(2, LoopType.Yoyo);
        //    GameObject hitFX = Instantiate(GameManagerPC.instance.hitCivilianFX, other.gameObject.transform.position, Quaternion.identity);
        //    Destroy(hitFX, 2f);
        //}
        //MONSTER TRUCK
        if (GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck && other.gameObject.GetComponent<BotController>() != null && other.gameObject.GetComponent<BotController>().botType == BotType.civilian)
        {
            SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomCrushCarSFX());
            GameManagerPC.instance.VibrationControl(100);
            if (GameManagerPC.instance.cameraView == CameraView.backView)
            {
                CameraFollow.Instance.camShake = true;
                Camera.main.DOShakeRotation(0.5f, 1, 10, 50).OnComplete(() =>
                {
                    CameraFollow.Instance.camShake = false;

                });
            }

            PlayerHealthDecrease(1);
            
            //transform.GetChild(0).DOLocalRotate(new Vector3(-10, transform.rotation.y, transform.rotation.z), 0.2f).SetLoops(2,LoopType.Yoyo).OnComplete(()=>
            //{
            //    //transform.GetChild(0).DOLocalRotate(new Vector3(10, transform.rotation.y, transform.rotation.z), 0.2f).SetLoops(2, LoopType.Yoyo);
            //});
            transform.GetChild(0).DOLocalRotate(new Vector3(10, transform.rotation.y, transform.rotation.z), 0.2f).SetLoops(2, LoopType.Yoyo);
            transform.GetChild(0).transform.DOLocalMoveY(transform.GetChild(0).position.y + 0.85f, 0.3f).SetLoops(2, LoopType.Yoyo);
            transform.GetChild(0).GetChild(0).transform.DOShakePosition(0.85f,0.35f);
            other.transform.DOLocalMoveY(0, 0.2f);
            other.transform.DOScale(new Vector3(other.transform.localScale.x + 0.4f, other.transform.localScale.y - 0.4f, other.transform.localScale.z + 0.4f),0.2f);
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            other.gameObject.GetComponent<BotController>().hit = true;
            //GameObject hitFX = Instantiate(GameManagerPC.instance.carBlastFX, other.gameObject.transform.position, Quaternion.identity);
            //Destroy(hitFX, 2f);
            GameObject hitFX = Instantiate(GameManagerPC.instance.carHitImpactFX, other.gameObject.transform.position, Quaternion.identity);
            Destroy(hitFX, 2f);
            //GameManagerPC.instance.OnHitComicFX(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 5, other.gameObject.transform.position.z), other.gameObject);
            Destroy(other.gameObject, 3f);
            //PlayerHealthDecrease();

        }
        //if (other.gameObject.CompareTag("bomb"))
        //{
        //    PlayerHealthDecrease(1);
        //    GameObject hitFX = Instantiate(GameManagerPC.instance.carBlastFX, other.gameObject.transform.position, Quaternion.identity);
        //    Destroy(hitFX, 2f);
        //    Destroy(other.gameObject.transform.parent.gameObject);
        //    SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomHitCarSFX());
        //    GameManagerPC.instance.VibrationControl(100);
        //    CameraFollow.Instance.camShake = true;
        //    Camera.main.DOShakeRotation(0.5f, 1, 10, 50).OnComplete(() =>
        //    {
        //        CameraFollow.Instance.camShake = false;

        //    });
        //}
    }
    bool spark = true;
    IEnumerator waitforBlast()
    {
        spark = false;
        Destroy(CarSpark);
        yield return new WaitForSeconds(1f);
        transform.Find("PoliceCarExterior").GetChild(0).Find("carCrashFX").gameObject.SetActive(true);
    }
    
    public void PlayerHealthDecrease(int i)
    {
        playerHealth-=i;
        UIManagerPC.instance.playerHealthBar.fillAmount = (float)playerHealth / (float)maxplayerHealth;

        ///LOSING CONDITION
        if (playerHealth <= 0 && !GameManagerPC.instance.defeat && !GameManagerPC.instance.victory)
        {

            StartCoroutine(waitforBlast());
            //GameManagerPC.instance.VibrationControl(120);
            GameManagerPC.instance.defeat = true;
            UIManagerPC.instance.gameoverPanelLose.SetActive(true);

            //if (randIndex == 0) transform.DOLocalRotate(new Vector3(0, 45, 0), 1.3f);
            //if (randIndex == 1) transform.DOLocalRotate(new Vector3(0, -45, 0), 1.3f);

            //GameObject deadEmo = Instantiate(GameManagerPC.instance.deadEmo, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 5, gameObject.transform.position.z), GameManagerPC.instance.angryEmo.transform.rotation);
            //deadEmo.transform.parent = gameObject.transform;
            //if (GameManagerPC.instance.cameraView == CameraView.sideView)
            //    deadEmo.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            //deadEmo.transform.DOPunchScale(new Vector3(0.07f, 0.07f, 0.07f), 1f);
            //Destroy(deadEmo, 3f);

            //UIManagerPC.instance.uiPrefabs.LoadUIPrefab(UIManagerPC.instance.uiPrefabs.PanelMissionFailed);
            //Debug.Log("FB_EVENT_FAIL");
            //FPG.FacebookManager.GetInstance().LogEvent("CarChase_Fail");
        }
    }
    void Death(string animState)
    {
        
        //GameObject hitFX = Instantiate(GameManager.instance.hitFX, transform.position, Quaternion.identity);
        //Destroy(hitFX, 2f);
        GameManagerPC.instance.VibrationControl(120);
        cam.transform.DOShakePosition(0.3f, 0.7f);
        transform.DOLocalMoveY(transform.position.y + 1f, 0.45f).SetLoops(2, LoopType.Yoyo);
        //anim.SetTrigger(animState);//Death//LowWallFall
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
        rb.useGravity = false;
        //Mission Failed.....
        dead = true;
        //UIManagerPC.instance.txtgameOver.rectTransform.DOShakeScale(0.45f, 0.2f);
        UIManagerPC.instance.gamePanel.SetActive(false);
    }

    public void BrakeButtonPress()
    {
        braking = true;
        brake.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().GetRandomBrakeSFX());
    }
    public void BrakeButtonRelease()
    {
        braking = false;
        brake.transform.DOLocalRotate(new Vector3(15, 0, 0), 0.5f);
        gear.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).SetLoops(2,LoopType.Yoyo);
    }
    
}
