using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    public Transform target;
    public Vector3 offset;
    public float smoothFactor;
    public PlayerControllerPC playerController;

    private bool isTargetFound = false;
    private Vector3 defaultPos;
    private Vector3 step;
    public ParticleSystem cameraDashFX;
    public bool camShake;
    public bool isHeliCam;

    public static CameraFollow SharedManager()
    {
        return Instance;
    }

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }
    private void Start()
    {
        defaultPos = transform.localPosition;
        step = defaultPos;
    }


    private void LateUpdate()
    {
        //if (GamePanel.sharedInstance.isTimeOver) return;

        if (target == null)
        {
            return;
        }
        if (target && isTargetFound == false)
        {
            isTargetFound = true;
        }
        
        if (!camShake && !GameManagerPC.instance.player.stopPlayer)
        {
            if (GameManagerPC.instance.vehicleType == VehicleType.helicopter)
            {
                offset = new Vector3(0,12,-10);
                target = GameManagerPC.instance.heli.transform;
                isHeliCam = true;

            }
            if (GameManagerPC.instance.vehicleType == VehicleType.car)
            {
                //offset = new Vector3(0, 15, -12);
                target = GameManagerPC.instance.car.transform;
                if (GameManagerPC.instance.cameraView == CameraView.sideView)
                {
                    offset = new Vector3(53, 30, 10);
                    transform.rotation = Quaternion.Euler(new Vector3(25, -90, 0));
                }
                if (GameManagerPC.instance.cameraView == CameraView.backView)
                {
                    //offset = new Vector3(0, 14, -12);
                    //transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
                }
            }
            if (GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck)
            {
                offset = new Vector3(0, 16, -14);
                target = GameManagerPC.instance.monsterTruck.transform;
            }

            if (GameManagerPC.instance.cameraView == CameraView.sideView)
            {
                Vector3 desiredPosition = new Vector3(0, target.transform.position.y, target.transform.position.z) + offset;
                Vector3 smoothedPosition = Vector3.Lerp(desiredPosition, transform.position, smoothFactor * Time.deltaTime);
                transform.position = smoothedPosition;
            }
            else if (GameManagerPC.instance.cameraView == CameraView.backView)
            {
                Vector3 desiredPosition = target.transform.position + offset;
                desiredPosition.x = target.transform.position.x/*offset.x*/;
                Vector3 smoothedPosition = Vector3.Lerp(desiredPosition, transform.position, smoothFactor * Time.deltaTime);
                transform.position = smoothedPosition;
            }
           
      
            //transform.position = new Vector3(transform.position.x , transform.position.y , target.transform.position.z + offset.z);
            if(GameManagerPC.instance.startGame && !GameManagerPC.instance.victory)
            {
                
            }
            //transform.rotation = target.transform.rotation;
            if (!GameManagerPC.instance.FPPMode)
            {
                //transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
            }
        }
        else if (camShake)
        {
            Vector3 desiredPosition = target.transform.position + offset;
            desiredPosition.x = target.transform.position.x;
            Vector3 smoothedPosition = Vector3.Lerp(desiredPosition, transform.position, smoothFactor * Time.deltaTime);
            transform.position = smoothedPosition;
            //transform.position = new Vector3(transform.position.x, transform.position.y, target.transform.position.z + offset.z);
        }
        else if (GameManagerPC.instance.startGame && !GameManagerPC.instance.startChase && (GameManagerPC.instance.vehicleType == VehicleType.car || GameManagerPC.instance.vehicleType == VehicleType.helicopter || GameManagerPC.instance.vehicleType == VehicleType.toDOMonsterTruck))
        {
            //Camera.main.transform.LookAt(GameManagerPC.instance.thiefHead[GameManagerPC.instance.thiefIndex].transform.position);
        }

    }

    public void ShakeCamera()
    {
        
        transform.DOShakePosition(1f, new Vector3(transform.position.x + 100f, transform.position.y + 50f, transform.position.z + 100f));
    }

    public void BlastShake()
    {
        
        transform.DOShakePosition(1f, new Vector3(100f, 50f, 0), 10, 8f, false, true);
    }
}
