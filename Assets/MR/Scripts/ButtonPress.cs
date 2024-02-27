using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ButtonPress : MonoBehaviour
{

    IEnumerator Haptics(float frequency, float amplitude, float duration, bool rightHand, bool leftHand)
    {
        if (rightHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);

        yield return new WaitForSeconds(duration);

        if (rightHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }

    void Update()
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

                if (button != null)
                {
                    OVRInput.SetControllerVibration(.3f, 0.3f, OVRInput.Controller.RTouch);

                    button.onClick.Invoke();
                    Debug.Log("Raycast colidiu com: " + hit.collider.gameObject.name);
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
