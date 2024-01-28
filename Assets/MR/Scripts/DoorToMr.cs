using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorToMr : MonoBehaviour
{
    private bool isLoaded = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isLoaded)
        {
            isLoaded = true;
            Resources.UnloadUnusedAssets();
            SceneManager.LoadScene("MR");
        }
    }
}