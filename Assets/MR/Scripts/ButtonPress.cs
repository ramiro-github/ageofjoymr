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
                Debug.Log("[DEBUG] Pressionou o botão");

                button.onClick.Invoke();
            }
        }

    }
}
