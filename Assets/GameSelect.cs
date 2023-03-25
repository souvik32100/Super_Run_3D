using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Mercilogo_Day()
    {
        SceneManager.LoadScene(2);
    }
    public void MonsterTruck_Day()
    {
        SceneManager.LoadScene(1);
    }

    public void MonsterTruck_Night()
    {
        SceneManager.LoadScene(3);
    }
}
