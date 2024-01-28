using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class InstanceObjectInScene : MonoBehaviour
{
    public AnchorManager anchorManager;
    public GameObject prefabCelingFan;

    private bool isInstanceCelingFan = false;

    public List<OVRSceneAnchor> sceneAnchors;

    public void StartInstance()
    {
        InstanceCelingFan();
    }

    private void InstanceCelingFan()
    {
        for (int i = 0; i < sceneAnchors.Count; i++)
        {
            OVRSceneAnchor anchor = sceneAnchors[i];

            OVRSemanticClassification classification = anchor.GetComponent<OVRSemanticClassification>();

            if (classification.Contains(OVRSceneManager.Classification.WallFace) && !isInstanceCelingFan)
            {

                Vector3 positionCelingFan = new Vector3(0, anchor.transform.position.y, 0);

                GameObject celingFan = Instantiate(prefabCelingFan, positionCelingFan, anchor.transform.rotation);
                celingFan.gameObject.AddComponent<OVRSpatialAnchor>();
                isInstanceCelingFan = true;
            }
        }
    }
}
