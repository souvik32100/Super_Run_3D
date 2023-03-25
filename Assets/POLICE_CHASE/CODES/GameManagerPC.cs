using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Luna.Unity;

public enum VehicleType
{
    car,
    helicopter,
    toDOMonsterTruck
}
public enum CarModelType
{
    _69Charger =0,
    marcilego =1,
    monstertruck =2,
}
public enum ResultType
{
    win,
    lose
}
public enum DifficultyLevel
{
    easy,
    hard
}
public enum StartType
{
    standing,
    chasing
}
public enum CameraView
{
    backView,
    sideView
}

public enum TimeofDay
{
    day,
    night
}
public class GameManagerPC : MonoBehaviour
{
    public static GameManagerPC instance;

    [Header("LUNA VARIABLES")]
    [SerializeField]
    [LunaPlaygroundField("Game Mode")]
    public VehicleType vehicleType;
    [SerializeField]
    [LunaPlaygroundField("Car Model")]
    public CarModelType carModel;
    [SerializeField]
    [LunaPlaygroundField("Difficulty Level")]
    public DifficultyLevel difficultyLevel;
    [SerializeField]
    [LunaPlaygroundField("Win/Lose Decider")]
    public ResultType endResultType;
    [SerializeField]
    [LunaPlaygroundField("Starting Variant")]
    public StartType startType;
    [SerializeField]
    [LunaPlaygroundField("No. of Taps for Download")]
    public int tapRequired;
    [SerializeField]
    [LunaPlaygroundField("Camera View")]
    public CameraView cameraView;

    public int tapCount;

    public GameObject[] carModelPrefabs;
    public TimeofDay timeOfDay; 
    public Camera mainCam;
    public Material daySkyBox;
    public Material nightSkyBox;
    public bool startGame;
    public bool startChase;
    public int coinAmount;
    public float score;
    public float scoreIncPerSec;
    public PlayerControllerPC player;
    public GameObject enemy;

    public float enemyDistance;
    public Material[] enemyMat;
    public GameObject decoyHelmet;

    public float enemyHealth;
    public bool victory;
    public bool defeat;
    [HideInInspector] public bool hasAttackereArrived;

    [Header("Random Thief Generation")]
    public int thiefIndex;
    public int thiefMatIndex;
    public GameObject[] thiefHead;
    public GameObject[] thiefPrefab;
    public GameObject[] thiefEndCutscenePrefab;
    public Material[] thinThiefMat;
    public Material[] fatThiefMat;

    [Header("Mode Switch")]
    public bool FPPMode;
    public Transform FPPCamPosition;
    public Transform TPPCamPosition;
    public Transform heliStartCamPosition;
    public GameObject policeExterior;
    public GameObject policeInterior;
    [Header("VEHICLE HOLDERS")]
    public GameObject car;
    public GameObject heli;
    public GameObject heliChild;
    public GameObject monsterTruck;
    public GameObject monsterTruckChild;
    [Header("Helicopter Scan")]
    public bool scanning;
    public float scanDuration;
    public float scanDurationMax = 5f;
    [Header("PARTICLES")]
    public GameObject carBlastFX;
    public GameObject carHitImpactFX;
    public GameObject enemyDownFX;
    public GameObject hitSparkFX;
    public GameObject hitCivilianFX;
    public GameObject[] comicFX;
    public GameObject angryEmo;
    public GameObject deadEmo;
    public GameObject swagEmo;
    public GameObject leftArrowParticle;

    private Vector3 enemyDirection;
    public GameObject directionalLight;
    public int dayNightToggleInt;
    [Header("End Cutscenes")]
    public GameObject startLights;
    public Transform endCamPos;
    public Transform endCamPosMT;
    public GameObject gameEndPoliceCaExterior;
    public GameObject gameEndPoliceCar;
    public GameObject gameEndMonsterTruck;
    public GameObject gameEndThiefCar;

    //public StolenObject stolenObject;
    //public StartMenuPC startmenuPC;
    [Header("Optimization")]
    public bool vehicleTypeSelected;

    public GameObject testProduct;
    public GameObject soundManagerInstance;
    [HideInInspector] public Vector3 playerCurrentPos;
    public GameObject tileManagerInstance;
    public GameObject endSceneObject;

    public GameObject model1;
    public GameObject model2;


    private void Awake()
    {
        if (instance == null) instance = this;
        dayNightToggleInt = Random.Range(0, 2);
        //DAY NIGHT CONTROL

        //if (dayNightToggleInt == 0) timeOfDay = TimeofDay.day;
        //else if (dayNightToggleInt == 1) timeOfDay = TimeofDay.night;
        //int level = AppDelegatePC.SharedManager().GetCurrentLevel();

        //if (TagManager.GetEnvModeModifier() > 0 && level % TagManager.GetEnvModeModifier() == 0) timeOfDay = TimeofDay.night;
        //else timeOfDay = TimeofDay.day;

        //if (TagManager.GetHelicopterModeModifier() > 0 && level % TagManager.GetHelicopterModeModifier() == 0)
        //{
        //    timeOfDay = TimeofDay.night;
        //}
      
        //SetTimeOfDay();


    }

    public void SetTimeOfDay()
    {
        if (timeOfDay == TimeofDay.day)
        {
            //startLights.SetActive(false);
            directionalLight.GetComponent<Light>().intensity = 1.2f;
            RenderSettings.skybox = daySkyBox;

        }
        else if (timeOfDay == TimeofDay.night)
        {
            //startLights.SetActive(true);
            directionalLight.GetComponent<Light>().intensity = 0.1f;
            RenderSettings.skybox = nightSkyBox;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        
        

        policeExterior.SetActive(true);
        //policeInterior.SetActive(false);
        heli.SetActive(false);
        monsterTruck.SetActive(false);
        score = 0;
        scoreIncPerSec = 1;

        CreateCarModel(ReturnCarModelIndex(), policeExterior.transform);
        CreateCarModel(ReturnCarModelIndex(), gameEndPoliceCar.transform);


        //random road curve direction
        int randCurveIndex = Random.Range(0, 2);
        if (randCurveIndex == 0)
        {
          //  CurvedWorld.instance.Curvature = new Vector3(0, 0, -0.15f);
        }
        if (randCurveIndex == 1)
        {
          //  CurvedWorld.instance.Curvature = new Vector3(0, 0, 0.15f);
        }

        if(cameraView == CameraView.sideView)
        {
            if (Screen.height / Screen.width >= 1)
            {
                //portrait
                Camera.main.fieldOfView = 70;

                UIManagerPC.instance.policeDialogSideViewLS.SetActive(false);
                UIManagerPC.instance.policeDialogSideViewP.SetActive(true);
            }
            else
            {
                //landscape
                Camera.main.fieldOfView = 40;

                UIManagerPC.instance.policeDialogSideViewP.SetActive(false);
                UIManagerPC.instance.policeDialogSideViewLS.SetActive(true);
            }
            directionalLight.transform.rotation = Quaternion.Euler(new Vector3(50, -90, 0));
        }
        else if (cameraView == CameraView.backView)
        {
            Camera.main.fieldOfView = 70;
            directionalLight.transform.rotation = Quaternion.Euler(new Vector3(50, 0, 0));

            UIManagerPC.instance.policeDialogBackView.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!vehicleTypeSelected)
        {
            if (vehicleType == VehicleType.helicopter)
            {
                player = heli.GetComponent<PlayerControllerPC>();
                heli.GetComponent<PlayerControllerPC>().enabled = true;
                car.GetComponent<PlayerControllerPC>().enabled = false;
                monsterTruck.GetComponent<PlayerControllerPC>().enabled = false;              
            }
            else if (vehicleType == VehicleType.car)
            {
                player = car.GetComponent<PlayerControllerPC>();
                car.GetComponent<PlayerControllerPC>().enabled = true;
                heli.GetComponent<PlayerControllerPC>().enabled = false;
                monsterTruck.GetComponent<PlayerControllerPC>().enabled = false;
            }
            else if (vehicleType == VehicleType.toDOMonsterTruck)
            {
                player = monsterTruck.GetComponent<PlayerControllerPC>();
                car.GetComponent<PlayerControllerPC>().enabled = false;
                heli.GetComponent<PlayerControllerPC>().enabled = false;
                monsterTruck.GetComponent<PlayerControllerPC>().enabled = true;
            }

            DifficultyController();

            if (startType == StartType.chasing && vehicleType==VehicleType.car)
            {
                player.cop.SetActive(false);
                player.cop2.SetActive(false);
                model1.SetActive(false);
                model2.SetActive(false);
                SoundManager.SharedManager().sirenSFX.SetActive(true);
            }
            if (vehicleType == VehicleType.car)
                player.backlight.transform.parent = policeExterior.transform.GetChild(0).transform;

            vehicleTypeSelected = true;
        }

        if (startGame && !player.dead)
        {
            //UIManagerPC.instance.txtScore.text = score.ToString("F0");
            score += scoreIncPerSec * Time.deltaTime;
        }
        enemyDistance = Vector3.Distance(player.transform.position, enemy.transform.position);
        enemyDirection = (player.transform.position - enemy.transform.position).normalized;
        if (startGame && startChase && enemyDirection.z >= 0 && !defeat && !victory)
        {
            defeat = true;
            UIManagerPC.instance.gameoverPanelLose.SetActive(true);
            // Level Failed.....
            //UIManagerPC.instance.uiPrefabs.LoadUIPrefab(UIManagerPC.instance.uiPrefabs.PanelMissionFailed);
            //Debug.Log("GAME ENDED");
            //FPG.FacebookManager.GetInstance().LogEvent("CarChase_Fail");
        }

        if(startType == StartType.chasing && !startGame)
        {
            player.currentSpeed = 40;
            player.speedIncreaseModifier = 3f;
            enemy.GetComponent<BotController>().currentSpeed = 40.5f;

        }
        if (startType == StartType.standing && !startGame)
        {
            player.speedIncreaseModifier = 25;
        }


        if(Screen.height / Screen.width >= 1 && tapCount >= tapRequired)
        {
            //portrait
            UIManagerPC.instance.btnDownloadInGameP.SetActive(true);
            UIManagerPC.instance.btnDownloadInGameL.SetActive(false);
        }
        else if(tapCount >= tapRequired)
        {
            //ls
            if (GameManagerPC.instance.cameraView == CameraView.backView)
            {
                UIManagerPC.instance.btnDownloadInGameL.SetActive(true);
            }
            else
            {
                UIManagerPC.instance.btnDownloadInGameSideViewL.SetActive(true);
            }              
            UIManagerPC.instance.btnDownloadInGameP.SetActive(false);
        }

        if (Screen.height / Screen.width >= 1)
        {
            //portrait
            if (cameraView == CameraView.sideView)
            {
                Camera.main.fieldOfView = 70;
                UIManagerPC.instance.policeDialogSideViewLS.SetActive(false);
                UIManagerPC.instance.policeDialogSideViewP.SetActive(true);
                UIManagerPC.instance.btnDownloadInGameSideViewL.SetActive(false);
                UIManagerPC.instance.policeDialogBackView.SetActive(false);
            }
           

            UIManagerPC.instance.btnDownloadInGameL.SetActive(false);
          
        }
        else
        {
            //landscape
            if (cameraView == CameraView.sideView)
            {
                Camera.main.fieldOfView = 40;
                UIManagerPC.instance.policeDialogSideViewP.SetActive(false);
                UIManagerPC.instance.policeDialogSideViewLS.SetActive(true);
                UIManagerPC.instance.policeDialogBackView.SetActive(false);
            }
              

     
      
        }


    }
    public void DifficultyController()
    {
        //difficulty control
        if (difficultyLevel == DifficultyLevel.easy)
        {
            player.maxplayerHealth = 4;
            //BotManagerPC.instance.spawnDelayMax = 2.25f;
        }
        else if (difficultyLevel == DifficultyLevel.hard)
        {
            player.maxplayerHealth = 3;
            //BotManagerPC.instance.spawnDelayMax = 1.25f;
        }
        player.playerHealth = player.maxplayerHealth;

    }
    public void restartCallback()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void VibrationControl(int vib)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
 
            Vibration.Vibrate(vib);
#endif
#if !UNITY_EDITOR && UNITY_IPHONE && UNITY_IOS
 
            Vibration.VibratePop();
#endif
    }

    public IEnumerator StartChase()
    {
        if (vehicleType == VehicleType.helicopter)
        {
            
            SoundManager.SharedManager().sirenSFX.SetActive(false);
            player.anim.enabled = false;
            player.gameObject.GetComponent<Helicopter>().heliSpotLight.SetActive(true);

        }

        //yield return new WaitForSeconds(0.4f);

        if (vehicleType == VehicleType.car || vehicleType == VehicleType.toDOMonsterTruck)
        {
            mainCam.transform.DOLocalRotate(new Vector3(38, 0, 0), 0.75f);

          

            //if (timeOfDay == TimeofDay.day) player.playerHeadLight.SetActive(false);
            //else if (timeOfDay == TimeofDay.night) player.playerHeadLight.SetActive(true);
           
            SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().engineStartSFX);
            VibrationControl(120);
            //player.gear.transform.DOLocalRotate(new Vector3(-45, 0, 0), 1f);
            if(player.copLights != null)
            {
                player.copLights.SetActive(true);
                if (player.copLights.GetComponentInParent<Animator>() != null)
                {
                    player.copLights.GetComponentInParent<Animator>().enabled = true;
                }
            }
        }
        if (vehicleType == VehicleType.helicopter)
            mainCam.transform.DOLocalRotate(new Vector3(39, 0, 0), 0.75f);
    
        if (vehicleType == VehicleType.car)
        {
            //car.GetComponent<PlayerControllerPC>().cop.SetActive(false);
            //car.GetComponent<PlayerControllerPC>().cop2.SetActive(false);
        }

        //thiefHead[thiefIndex].SetActive(false);

        yield return new WaitForSeconds(0.5f);
      

        //if (vehicleType == VehicleType.MonsterTruck)
        //{
        //    vehicleType = VehicleType.car;
        //    StartCoroutine(GameManagerPC.instance.StartChase());

        //    //var mtPC = monsterTruck.GetComponent<PlayerControllerPC>();
        //    //MonsterTruck mt = monsterTruck.GetComponentInChildren<MonsterTruck>();
        //    //mtPC.cop.SetActive(false);
        //    //mtPC.cop2.SetActive(false);
        //    //mt.wfl.GetComponent<Animator>().enabled = true;
        //    //mt.wfr.GetComponent<Animator>().enabled = true;
        //    //mt.wbl.GetComponent<Animator>().enabled = true;
        //    //mt.wbr.GetComponent<Animator>().enabled = true;
        //}

        //startChase = true;
        vehicleTypeSelected = true;
        //Debug.Log("FB_EVENT_START");
       // FPG.FacebookManager.GetInstance().LogEvent("CarChase_Start");
    }
    public void EndFPPtoTPPSwitch()
    {
        policeExterior.SetActive(true);
        FPPMode = false;
        mainCam.GetComponent<CameraFollow>().enabled = true;
        mainCam.transform.parent = null;
        //Camera.main.transform.position = FPPCamPosition.position;
        //Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, 40, 0));
    }
    public IEnumerator SlowMoRoutine()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1f;
    }
    public IEnumerator EndSlowMoRoutine()
    {
        //UIManagerPC.instance.fadeTransitionImage.SetActive(true);
        yield return new WaitForSeconds(0.5f);

      

/*        if (vehicleType == VehicleType.car)
        {
            mainCam.transform.position = endCamPos.position;
            mainCam.transform.rotation = endCamPos.rotation;
            gameEndPoliceCar.SetActive(true);

            //CreateVehicle(AppDelegatePC.SharedManager().GetSelectedProductId(), gameEndPoliceCar);
        }
        else if (vehicleType == VehicleType.helicopter)
        {
            mainCam.transform.position = endCamPos.position;
            mainCam.transform.rotation = endCamPos.rotation;
            gameEndPoliceCar.SetActive(true);

            CreateVehicle(2808, gameEndPoliceCar);
        }
        else if (vehicleType == VehicleType.MonsterTruck)
        {
            mainCam.transform.position = endCamPosMT.position;
            mainCam.transform.rotation = endCamPosMT.rotation;
            gameEndPoliceCaExterior.gameObject.SetActive(false);
            gameEndMonsterTruck.SetActive(true);

            //CreateVehicle(AppDelegatePC.SharedManager().GetSelectedProductId(), gameEndMonsterTruck);
        }*/

        //Camera.main.transform.DOMoveX(Camera.main.transform.position.x + 2, 3f).SetLoops(-1,LoopType.Yoyo);
        //Time.timeScale = 0.5f;

        yield return new WaitForSeconds(0.5f);

        //mainCam.GetComponent<Animator>().enabled = true;
        //if (vehicleType == VehicleType.MonsterTruck)
        //{
        //    mainCam.GetComponent<Animator>().SetTrigger("mt");
        //}
        //if (vehicleType == VehicleType.car || vehicleType == VehicleType.helicopter)
        //{
        //    mainCam.GetComponent<Animator>().SetTrigger("car");
        //}
        //UIManagerPC.instance.fadeTransitionImage.SetActive(false);
        Time.timeScale = 1f;
    }

    public void CreateVehicle(int productId, GameObject _gameObject)
    {
        productId = 2808;
        //GameObject instance = AnimPreFabsList.SharedManager().GetAnimObject(productId, 1, false);
        instance.transform.parent = _gameObject.transform;

        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
    }
    public void CreateCarModel(int i,Transform t)
    {
        GameObject carInstance = Instantiate(carModelPrefabs[i]);

        carInstance.transform.parent =t;

        carInstance.transform.localPosition = Vector3.zero;
        carInstance.transform.localRotation = Quaternion.identity;
        carInstance.transform.localScale = Vector3.one;
    }

    public IEnumerator EndCutsceneDelayRoutine()
    {

        yield return new WaitForSeconds(3f);

        directionalLight.transform.rotation = Quaternion.Euler(new Vector3(50, 0, 0));
        var cam = Camera.main;
        tileManagerInstance.SetActive(false);
        cam.transform.position = endCamPos.position;
        cam.transform.rotation = endCamPos.rotation;
        cam.transform.parent = endSceneObject.transform;
        cam.fieldOfView = 55;
        cam.GetComponent<Animator>().enabled = true;

        UIManagerPC.instance.gameoverPanelWin.SetActive(true);

        //int level = AppDelegatePC.SharedManager().GetCurrentLevel();
        //FPG.RateGame.Instance.RateGameIfEligible(level);

        //UIManagerPC.instance.uiPrefabs.LoadUIPrefab(UIManagerPC.instance.uiPrefabs.PanelMissionComplete);
    }

    public static void sendUserActivity(int win)
    {
        //int userLevel = AppDelegatePC.SharedManager().GetCurrentLevel();

        //if (userLevel <= 20 && win == 1)
        //    FPG.Networking.getInstance().sendUserActivity("BattleWin_" + userLevel.ToString());
        //else if (userLevel <= 20 && win == 0)
        //    FPG.Networking.getInstance().sendUserActivity("Battleloose_" + userLevel.ToString());
    }
    public void OnHitComicFX(Vector3 hitPosition, GameObject go)
    {
        //if (TagManager.IsComicFXEnabled())
        //{
           
        //}
        int randComicFX = Random.Range(0, comicFX.Length);
        GameObject comic = Instantiate(comicFX[randComicFX], hitPosition, comicFX[randComicFX].transform.rotation);
        if (go.gameObject.GetComponent<BotController>().botType == BotType.enemy)
        {
            comic.transform.parent = go.transform;
        }
        Destroy(comic, 2f);
    }

    public int ReturnCarModelIndex()
    {

        return (int)carModel;
     
    }

    public IEnumerator TrailOffRoutine()
    {
        yield return new WaitForSeconds(3f);

        enemy.GetComponent<BotController>().boostTrail.SetActive(false);
    }

    void OnApplicationQuit()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            //DatabaseManager.sharedManager().databaseBinary.Close();
            //DatabaseManager.sharedManager().databaseDocument.Close();
        }
    }

    public void DownloadButtonCallBack()
    {
#if !UNITY_EDITOR
        Playable.InstallFullGame();
#endif
        Debug.Log("Clicked");
    }
}
