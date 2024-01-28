using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;


public class AnchorManager : MonoBehaviour
{

    public InstanceObjectInScene instanceObjectInScene;

    private void Start()
    {
       OnSceneModelLoadedEvent();
    }

    private void OnSceneModelLoadedEvent()
    {
        StartCoroutine(LoadAllAnchors());
    }

    private IEnumerator LoadAllAnchors()
    {

        instanceObjectInScene.sceneAnchors = new List<OVRSceneAnchor>(FindObjectsOfType<OVRSceneAnchor>());

        yield return new WaitForSeconds(1.0f);

        instanceObjectInScene.StartInstance();
    }
}
