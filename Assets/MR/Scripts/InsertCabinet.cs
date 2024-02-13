using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System;

public class InsertCabinet : MonoBehaviour
{

    public GameObject lastInstance;
    private GameObject Player;

    public GameObject prefabGabinet;

    private float floorPosition = 0;

    private bool getFloor = false;

    public GameObject cubeTest;

    private float accumulatedTime = 0f;

    private int countCabinet = 0;

    private Dictionary<string, string> insertCabInformation = new Dictionary<string, string>();

    void Start()
    {
        Player = GameObject.Find("Complete XR Origin Set Up MR");

#if UNITY_EDITOR
        getFloor = true;
#endif
    }

    IEnumerator getFloorPosition()
    {

        while (getFloor == false)
        {

            OVRSceneAnchor[] sceneAnchors = FindObjectsOfType<OVRSceneAnchor>();

            for (int i = 0; i < sceneAnchors.Length; i++)
            {

                OVRSceneAnchor anchor = sceneAnchors[i];

                OVRSemanticClassification classification = anchor.GetComponent<OVRSemanticClassification>();

                if (classification.Contains(OVRSceneManager.Classification.Floor))
                {

                    floorPosition = anchor.transform.position.y;
                    getFloor = true;
                    break;
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void instanceCabinet(Dictionary<string, string> cabInformation, Vector3 position, Quaternion rotation, bool isAnchorSaved)
    {

        while (getFloor == false)
        {
            StartCoroutine(getFloorPosition());
        }

        Vector3 positionUpdateFloor = new Vector3(position.x, floorPosition, position.z);

        GameObject _default = Instantiate(prefabGabinet, positionUpdateFloor, rotation);
        _default.GetComponent<CabinetControllerMR>().insertCabinet = this.gameObject;
        _default.GetComponent<CabinetControllerMR>().isAnchorSaved = isAnchorSaved;
        _default.GetComponent<CabinetControllerMR>().AgentPlayerPositions.Add(Player);
        _default.GetComponent<CabinetControllerMR>().game.CabinetDBName = cabInformation["folderName"];
        _default.GetComponent<CabinetControllerMR>().game.Rom = cabInformation["rom"];
        _default.GetComponent<CabinetControllerMR>().game.Position = 0;

        insertCabInformation = cabInformation;
    }

    void Update()
    {
        if (lastInstance != null)
        {

            Vector3 newPosition = new Vector3(transform.position.x, floorPosition + 0.05f, transform.position.z);
            lastInstance.transform.position = newPosition;

            if (OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.Return))
            {
                lastInstance.transform.position = new Vector3(lastInstance.transform.position.x, floorPosition, lastInstance.transform.position.z);
                OVRSpatialAnchor workingAnchor = lastInstance.AddComponent<OVRSpatialAnchor>();

                StartCoroutine(anchorCreated(workingAnchor));

                lastInstance = null;
            }

            float xAxisValue = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;

            if (xAxisValue > 0)
            {
                lastInstance.transform.Rotate(new Vector3(0, 35 * Time.deltaTime, 0));
            }
            else if (xAxisValue < 0)
            {
                lastInstance.transform.Rotate(new Vector3(0, -35 * Time.deltaTime, 0));
            }
        }
    }

    IEnumerator anchorCreated(OVRSpatialAnchor osAnchor)
    {
        while (!osAnchor.Created && !osAnchor.Localized)
        {
            yield return new WaitForEndOfFrame();
        }

        osAnchor.Save((anchor, success) =>
        {

            if (success)
            {
                string filePath = Path.Combine(ConfigManager.BaseDir, "cabinetsdb", insertCabInformation["folderName"], "SpatialAnchor.json");

                var UuidObject = new { Uuid = osAnchor.Uuid.ToString() };

                string json = JsonConvert.SerializeObject(UuidObject, Formatting.Indented);

                File.WriteAllText(filePath, json);
            }

        });
    }
}
