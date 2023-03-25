using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialGlow : MonoBehaviour
{
    private float lightDelay = 0.01f;
    public GameObject[] lightObject;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Glow());
    }

    // glow material
    IEnumerator Glow()
    {
        float minIntensity = 0f;
        float maxIntensity = 60f;
        Color emissionColor = lightObject[0].GetComponent<Renderer>().material.GetColor("_EmissionColor");
        while (true)
        {
            for (float i = maxIntensity; i >= minIntensity; i--)
            {
                yield return new WaitForSeconds(lightDelay);
                for (int j = 0; j < lightObject.Length; j++)
                {
                    lightObject[j].GetComponent<Renderer>().material.SetVector("_EmissionColor", new Vector4(emissionColor.r, emissionColor.g, emissionColor.b) * i);
                }
            }
            for (float i = minIntensity; i <= maxIntensity; i++)
            {
                yield return new WaitForSeconds(lightDelay);
                for (int j = 0; j < lightObject.Length; j++)
                {
                    lightObject[j].GetComponent<Renderer>().material.SetVector("_EmissionColor", new Vector4(emissionColor.r, emissionColor.g, emissionColor.b) * i);
                }
            }
        }
    }
}




