using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
#if !UNITY_EDITOR
using Luna.Unity;
#endif

public class UIManagerPC : MonoBehaviour
{
    public static UIManagerPC instance;
    [Header("SO reference")]

    public Transform canvas;
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject gameoverPanelLose;
    public GameObject gameoverPanelWin;
    public GameObject panelCTA;
    public Button btnPlay;
    public Button btnRetrylose;
    public Button btnRetryWin;
    public Button[] btnRetryChoose;
    public GameObject imgTaptochange;
    public GameObject btnDownloadInGameP;
    public GameObject btnDownloadInGameL;
    public GameObject btnDownloadInGameSideViewL;
    public GameObject policeDialogBackView;
    public GameObject policeDialogSideViewLS;
    public GameObject policeDialogSideViewP;
    //public TextMeshProUGUI txtHowTo;
    //public TextMeshProUGUI txtCoinAmount;
    //public TextMeshProUGUI txtScore;
    //public TextMeshProUGUI txtgameOver;
    public Button brakeBtn;
    public Button fppSwitchBtn;
    public Button heliSwitchBtn;
    public Image playerHealthBar;
    public GameObject playerHealthParent;
    public GameObject swipeTutorialParent;
    public Image missionStatusImage;
    public Sprite missionCompleteImg;
    public Sprite missionFailedImg;
    public GameObject fadeTransitionImage;
    public UnityAction startGameAction;
    [Header("App Rating")]
    public GameObject appRatingPanelPrefab;
    private GameObject appRatingPanel;

    bool isLunaGamEndCalled = false;

    public GameObject policeDialogBackViewLs;

    float timer = 0;

    public GameObject choosePanel;

    private void Awake()
    {   
        if (instance == null) instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        btnPlay.onClick.AddListener(() => StartRun());
        btnRetrylose.onClick.AddListener(() => OpenCTAPanelCallback());
        btnRetryWin.onClick.AddListener(() => OpenCTAPanelCallback());

        btnDownloadInGameP.GetComponent<Button>().onClick.AddListener(() => InstallFullGameCallback());
        btnDownloadInGameL.GetComponent<Button>().onClick.AddListener(() => InstallFullGameCallback());
        btnDownloadInGameSideViewL.GetComponent<Button>().onClick.AddListener(() => InstallFullGameCallback());

        //for (int i = 0; i < btnRetryChoose.Length; i++)
        //{
        //    btnRetryChoose[i].onClick.AddListener(() => OpenCTAPanelCallback());

        //}

        //txtCoinAmount.text = GameManagerPC.instance.coinAmount.ToString();
        gamePanel.SetActive(false);
        //fadeTransitionImage.SetActive(false);
        //appRatingPanel = Instantiate(appRatingPanelPrefab, canvas.transform);

        if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
        {
            HelicopterMode();
        }

        if (GameManagerPC.instance.cameraView != CameraView.sideView)
        {

            if (Screen.height > Screen.width)
            {
                policeDialogBackView.transform.GetComponent<RectTransform>().DOScale(Vector3.one, 1f);
                //policeDialogBackView.transform.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-34f, -212f, 0f), 1f).OnComplete(() =>
                //{
                    policeDialogBackView.transform.GetComponent<RectTransform>().DOScale(Vector3.one * .7f, .9f).SetLoops(-1, LoopType.Yoyo);
                //});
                policeDialogBackViewLs.SetActive(false);
            }
            else if (Screen.height < Screen.width)
            {
                policeDialogBackViewLs.transform.GetComponent<RectTransform>().DOScale(Vector3.one * 0.6f, 1f);
                //policeDialogBackViewLs.transform.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-40f, -90f, 0f), 1f).OnComplete(() =>
                //{
                    policeDialogBackViewLs.transform.GetComponent<RectTransform>().DOScale(Vector3.one * .55f, .9f).SetLoops(-1, LoopType.Yoyo);
                //});
                policeDialogBackView.SetActive(false);
            }
            //policeDialogBackView.transform.GetComponent<RectTransform>().DOScale(Vector3.one * .9f, .9f).SetLoops(-1, LoopType.Yoyo);
            policeDialogSideViewLS.transform.GetComponent<RectTransform>().DOScale(Vector3.one * .9f, .9f).SetLoops(-1, LoopType.Yoyo);
            policeDialogSideViewP.transform.GetComponent<RectTransform>().DOScale(Vector3.one * .9f, .9f).SetLoops(-1, LoopType.Yoyo);
        }


    }

    void Update()
    {
        if (!GameManagerPC.instance.startGame)
        {
            timer += Time.deltaTime;
            if (timer > 5)
                StartRun();
        }

        if (Screen.width > Screen.height)
        {
            choosePanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 20, 0);
        }
        else
        {
            choosePanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 100, 0);
        }
    }

    public void StartRun(int i=0)
    {
        if (GameManagerPC.instance.startType == StartType.standing)
            GameManagerPC.instance.tapCount++;
        //GameManagerPC.instance.leftArrowParticle.SetActive(true);
        //GameManagerPC.instance.leftArrowParticle.GetComponent<ParticleSystem>().Play();
        //if(i!=1) GameManagerPC.instance.CreateVehicle(2808, GameManagerPC.instance.policeExterior);
        //GameManagerPC.instance.CreateVehicle(AppDelegatePC.SharedManager().GetSelectedProductId(), GameManagerPC.instance.policeExterior);
        startPanel.SetActive(false);
        //GameManagerPC.instance.soundManagerInstance.SetActive(true);

        if(GameManagerPC.instance.player.cop != null && GameManagerPC.instance.player.cop2 != null)
        {
            GameManagerPC.instance.player.cop.SetActive(false);
            GameManagerPC.instance.player.cop2.SetActive(false);
            GameManagerPC.instance.model1.SetActive(false);
            GameManagerPC.instance.model2.SetActive(false);
        }
     
        //txtHowTo.gameObject.SetActive(false);
        GameManagerPC.instance.startGame = true;
        GameManagerPC.instance.startChase = true;
        btnPlay.gameObject.SetActive(false);
        gamePanel.SetActive(true);

        Camera cam = Camera.main;
        if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
        {
            cam.transform.DORotateQuaternion(Quaternion.Euler(38,0,0),0.5f);
            cam.DOFieldOfView(60,0.5f);
            imgTaptochange.SetActive(false);
            StartCoroutine(SwipeTutorialRoutine());
            SoundManager.SharedManager().engineSFX.SetActive(true);
        }
        if (GameManagerPC.instance.vehicleType == VehicleType.car)
            SoundManager.SharedManager().sirenSFX.SetActive(true);

        if (GameManagerPC.instance.endResultType == ResultType.lose && GameManagerPC.instance.vehicleType != VehicleType.helicopter)
            StartCoroutine(BotManagerPC.sharedManager().GenerateKiller());

        if (GameManagerPC.instance.startType == StartType.chasing)
        {
            GameManagerPC.instance.enemy.GetComponent<BotController>().currentSpeed = 65;
            GameManagerPC.instance.player.currentSpeed = 50f;
            //GameManagerPC.instance.enemy.GetComponent<BotController>().boostTrail.SetActive(true);

            //StartCoroutine(GameManagerPC.instance.TrailOffRoutine());
        }
    }
    public void OpenCTAPanelCallback()
    {

#if !UNITY_EDITOR
                Playable.InstallFullGame();
#endif

        if (!isLunaGamEndCalled)
        {
            Luna.Unity.LifeCycle.GameEnded();
            isLunaGamEndCalled = true;
        }

        panelCTA.SetActive(true);
        gameoverPanelLose.SetActive(false);
        gameoverPanelWin.SetActive(false);
    }
    public void InstallFullGameCallback()
    {

#if !UNITY_EDITOR
                Playable.InstallFullGame();
#endif
    }

        public void BtnRetryCallback()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ModeSwitchBtn()
    {
        heliSwitchBtn.gameObject.SetActive(false);
        GameManagerPC.instance.policeExterior.SetActive(false);
        GameManagerPC.instance.policeInterior.SetActive(true);
        fppSwitchBtn.gameObject.SetActive(false);
        GameManagerPC.instance.FPPMode = true;
        Camera.main.GetComponent<CameraFollow>().enabled = false;
        Camera.main.transform.parent = GameManagerPC.instance.player.transform;
        Camera.main.transform.position = GameManagerPC.instance.FPPCamPosition.position;
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        GameManagerPC.instance.vehicleType = VehicleType.car;
    }
    public void HelicopterMode()
    {
        //StartRun(1);
       
        playerHealthParent.SetActive(false);
        GameManagerPC.instance.heli.SetActive(true);
        GameObject helicopter = GameManagerPC.instance.heliChild;
        //GameManagerPC.instance.CreateVehicle(AppDelegatePC.SharedManager().GetSelectedProductId(), helicopter);
        GameManagerPC.instance.CreateVehicle(2808, GameManagerPC.instance.policeExterior);
        Camera.main.transform.DOMove(GameManagerPC.instance.heliStartCamPosition.position, 1f);
        Camera.main.transform.rotation = GameManagerPC.instance.heliStartCamPosition.rotation;
       
        GameManagerPC.instance.vehicleType = VehicleType.helicopter;
        CameraFollow.Instance.target = GameManagerPC.instance.heli.transform;
        GameManagerPC.instance.car.GetComponent<PlayerControllerPC>().cop.SetActive(false);
        GameManagerPC.instance.car.GetComponent<PlayerControllerPC>().cop2.SetActive(false);
        GameManagerPC.instance.model1.SetActive(false);
        GameManagerPC.instance.model2.SetActive(false);
        startGameAction?.Invoke();
       
    }
    public void MonsterTruckMode()
    {
        StartRun();
        GameObject monsterTruck = GameManagerPC.instance.monsterTruckChild;
        GameManagerPC.instance.monsterTruck.SetActive(true);
        //monsterTruck.SetActive(true);

       // GameManagerPC.instance.CreateVehicle(AppDelegatePC.SharedManager().GetSelectedProductId(), monsterTruck);

        GameManagerPC.instance.car.SetActive(false);
        GameManagerPC.instance.vehicleType = VehicleType.toDOMonsterTruck;
        CameraFollow.Instance.target = GameManagerPC.instance.monsterTruck.transform;
    }
    public IEnumerator SwipeTutorialRoutine()
    {
        swipeTutorialParent.SetActive(true);
        yield return new WaitForSeconds(4f);
        swipeTutorialParent.SetActive(false);
    }
}
