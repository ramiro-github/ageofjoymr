using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class RotateCabinet : MonoBehaviour
{
    public TextMeshProUGUI textPro;

    void FixedUpdate()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
            {
                GameObject parentObject = hit.collider.gameObject.transform.parent.gameObject;
                textPro.text = "Rotacionar: " + parentObject.name;
                parentObject.transform.Rotate(new Vector3(5, 0, 0) * Time.fixedDeltaTime);
            }
            else
            {
                textPro.text = "";
            }
        }
    }
}