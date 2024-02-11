using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ButtonPress : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Button")
        {

            Button button = other.gameObject.GetComponentInChildren<Button>();

            if (button != null)
            {

                OVRInput.SetControllerVibration(.3f, 0.3f, OVRInput.Controller.RTouch);

                button.onClick.Invoke();
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Button")
        {

            Button button = other.gameObject.GetComponentInChildren<Button>();

            if (button != null)
            {
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);

                button.onClick.Invoke();
            }
        }

    }


    IEnumerator Haptics(float frequency, float amplitude, float duration, bool rightHand, bool leftHand)
    {
        if (rightHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);

        yield return new WaitForSeconds(duration);

        if (rightHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }
}
