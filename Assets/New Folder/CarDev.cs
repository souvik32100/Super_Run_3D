using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CarDev : MonoBehaviour
{
    public Transform Target;
    public GameObject newWheel1;
    public GameObject newWheel1x;
    public GameObject newWheel2;
    public GameObject newWheel2x;
    public GameObject oldWheel1;
    public GameObject oldWheel2;


    public GameObject oldWheeler;
    public GameObject newWheeler1;
    public GameObject newWheeler1x;
    public GameObject newWheeler2;
    public GameObject newWheeler2x;

    public GameObject newcelling;
    public GameObject newcellingx;
    public GameObject oldcelling;


    public GameObject Spring1;
    public GameObject Spring2;
    public GameObject Door;
    public GameObject Doorx;
    public GameObject Exsosot;
    public GameObject Exsosotx;

    public GameObject newGlow;
    public GameObject newGlowx;
    public GameObject oldGlow;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    int x = 0;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (x == 0)
            {
                oldcelling.transform.DOMove(Target.position, 1f);
                newcelling.transform.DOMove(transform.position, 1f).OnComplete(() =>
                {
                    newcelling.SetActive(false);
                    oldcelling.SetActive(false);
                    newcellingx.SetActive(true);
                    x = 1;
                });
            }
            if (x == 1)
            {
                oldGlow.transform.DOMove(Target.position, 1f);
                newGlow.transform.DOMove(transform.position, 1f).OnComplete(() =>
                {
                    newGlow.SetActive(false);
                    oldGlow.SetActive(false);
                    newGlowx.SetActive(true);
                    x = 2;
                });
            }
            if (x==2)
            {
                oldWheel1.transform.DOMove(Target.position,1f);
                newWheel1.transform.DOMove(transform.position, 1f).OnComplete(() =>
                {
                    oldWheel1.SetActive(false);
                    newWheel1.SetActive(false);
                    newWheel1x.SetActive(true);
                    x = 3;
                });
            }
            if (x == 3)
            {
                oldWheel2.transform.DOMove(Target.position, 1f);
                newWheel2.transform.DOMove(transform.position, 1f).OnComplete(() =>
                {
                    oldWheel2.SetActive(false);
                    newWheel2.SetActive(false);
                    newWheel2x.SetActive(true);
                    x = 4;
                });
            }
            if (x == 4)
            {
                oldWheeler.transform.DOMove(Target.position, 1f);
                newWheeler1.transform.DOMove(transform.position, 1f).OnComplete(() =>
                {
                    oldWheeler.SetActive(false);
                    newWheeler1.SetActive(false);
                    newWheeler1x.SetActive(true);
                    x = 5;
                });
                newWheeler2.transform.DOMove(transform.position, 1f).OnComplete(() =>
                {
                    
                    newWheeler2.SetActive(false);
                    newWheeler2x.SetActive(true);
                    
                });
            }
            
            if(x==5)
            {
                Spring1.transform.DOMove(Target.position, 1f);
                Door.transform.DOMove(transform.position, 1f).OnComplete(() =>
                {
                    Door.SetActive(false);
                    Spring1.SetActive(false);
                    Doorx.SetActive(true);
                    x= 6;
                });
            }
            if(x==6)
            {
                Spring2.transform.DOMove(Target.position, 1f);
                Exsosot.transform.DOMove(transform.position, 1f).OnComplete(() =>
                {
                    Exsosot.SetActive(false);
                    Spring2.SetActive(false);
                    Exsosotx.SetActive(true);
                    x= 7;
                });
            }
            

        }
    }
}
