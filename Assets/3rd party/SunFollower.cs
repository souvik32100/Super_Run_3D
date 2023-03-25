using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFollower : MonoBehaviour
{

    public Transform player;
    public float offsetZ;

    void Start()
    {
        
    }


    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, player.position.z + offsetZ);
    }
}
