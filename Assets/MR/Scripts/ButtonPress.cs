using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ButtonPress : MonoBehaviour
{
    IEnumerator delayButtonOnList(Button button)
    {
        button.interactable = false;
        yield return new WaitForSeconds(1.0f);
        button.interactable = true;
    }

    void FixedUpdate()
    {
        Vector3 rayOrigin = transform.position;

        Vector3 rayDirection = transform.forward;

        float maxDistance = 0.005f;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance))
        {

            if (hit.collider.gameObject.tag == "Button")
            {
                Button button = hit.collider.gameObject.GetComponentInChildren<Button>();

                if (button != null && button.interactable)
                {
                    OVRInput.SetControllerVibration(.3f, 0.3f, OVRInput.Controller.RTouch);

                    button.onClick.Invoke();
                    Debug.Log("Raycast colidiu com: " + hit.collider.gameObject.name);

                    if (
                        hit.collider.gameObject.name == "PanelButtonNextCabList" ||
                        hit.collider.gameObject.name == "PanelButtonBackCabList"
                        )
                    {
                        StartCoroutine(delayButtonOnList(button));
                    }
                }
            }

            Debug.Log("Raycast colidiu com: " + hit.collider.gameObject.name);

        }
        else
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
    }
}
