using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotIndicator : MonoBehaviour {

    public int slotIndex = 0;

    private Light slotLight;
    private const float maxLight = 2.0f;

    bool endBrightening = false;

    private void Awake()
    {
        slotLight = gameObject.GetComponent<Light>();
    }

    void OnMouseOver()
    {
        endBrightening = false;
        if (slotLight.intensity < maxLight)
        {
            slotLight.intensity += Time.deltaTime * 4;
        }
    }

    private void OnMouseExit()
    {
        endBrightening = true;
    }

    private void Update()
    {
        if(endBrightening)
        {
            if (slotLight.intensity > 0)
            {
                slotLight.intensity -= Time.deltaTime * 4;
            }
            else
            {
                endBrightening = false;
                slotLight.intensity = 0;
            }
        }
    }
}
