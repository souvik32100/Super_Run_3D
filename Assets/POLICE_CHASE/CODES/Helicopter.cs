using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    public GameObject topFan;
    public GameObject backFan;
    public GameObject heliSpotLight;
    public float spinSpeed;
    public Material normalLightMat;
    public Material scanningLightMat;
    public bool startFan;

    // Start is called before the first frame update
    void Start()
    {
        //UIManagerPC.instance.startGameAction += StartFan;
        //heliSpotLight.SetActive(false);
    }

    private void StartFan()
    {
        topFan = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        startFan = true;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(startFan)
    //         topFan.transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    //}
    
}
