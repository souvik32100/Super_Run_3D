using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTruck : MonoBehaviour
{
    [Header("Monster Truck")]
    public GameObject wfl;
    public GameObject wfr;
    public GameObject wbl;
    public GameObject wbr;
    // Start is called before the first frame update
    void Start()
    {
        wfl.GetComponent<Animator>().enabled = false;
        wfr.GetComponent<Animator>().enabled = false;
        wbl.GetComponent<Animator>().enabled = false;
        wbr.GetComponent<Animator>().enabled = false;
    }

}
