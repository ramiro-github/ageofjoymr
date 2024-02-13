using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

public class SpatialAnchorManager : MonoBehaviour
{
    public class MyJsonObject
    {
        public string Uuid { get; set; }
        public string name { get; set; }
    }

    public GameObject instancePrefabAnchorManager;

    public List<OVRSpatialAnchor.UnboundAnchor> unboundAnchorsList = new List<OVRSpatialAnchor.UnboundAnchor>();

    private OVRSceneManager ovrSceneManager;

    void Start()
    {
        if (!Application.isEditor)
        {
            StartCoroutine(loadOVRScene());
            StartCoroutine(LoadAllAnchor());
        }
    }

    private IEnumerator loadOVRScene()
    {
        while (ovrSceneManager == null)
        {
            ovrSceneManager = FindObjectOfType<OVRSceneManager>();
            if (ovrSceneManager != null)
            {
                ovrSceneManager.SceneModelLoadedSuccessfully += OnSceneModelLoadedSuccessfully;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnSceneModelLoadedSuccessfully()
    {
        StartCoroutine(loadOVRSceneManager());
    }

    private IEnumerator loadOVRSceneManager()
    {
        Debug.Log("[DEBUG] Start loadOVRSceneManager");
        yield return new WaitForEndOfFrame();

        OVRSceneAnchor[] sceneAnchors = FindObjectsOfType<OVRSceneAnchor>();

        if (sceneAnchors != null)
        {
            for (int i = 0; i < sceneAnchors.Length; i++)
            {
                OVRSceneAnchor anchor = sceneAnchors[i];

                OVRSemanticClassification classification = anchor.GetComponent<OVRSemanticClassification>();

                Debug.Log("[DEBUG] OVRSceneAnchor label: " + classification.Labels[0]);

                if (classification.Contains(OVRSceneManager.Classification.WallArt))
                {
                    Debug.Log("[DEBUG] Encontrou arte de parede: " + classification.Labels[0]);
                    instancePrefabAnchorManager.GetComponent<InstancePrefabAnchorManager>().instanceFrame(anchor);
                }
            }
        }

        Debug.Log("[DEBUG] Final loadOVRSceneManager");
    }

    private IEnumerator LoadAllAnchor()
    {

        OVRSpatialAnchor.LoadOptions options = new OVRSpatialAnchor.LoadOptions
        {
            Timeout = 0,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = GetSavedAnchorUUIDs()
        };

        OVRSpatialAnchor.LoadUnboundAnchors(options, anchorSavedUUIDList =>
        {
            foreach (var anchor in anchorSavedUUIDList)
            {
                if (anchor.Localized)
                {
                    unboundAnchorsList.Add(anchor);
                }
                else
                {
                    anchor.Localize((a, success) =>
                    {

                        if (success)
                        {
                            unboundAnchorsList.Add(anchor);
                        }

                    });
                }
            }
        });

        yield return new WaitForSeconds(0.05f);
        StartCoroutine(instancePrefabAnchorManager.GetComponent<InstancePrefabAnchorManager>().instanceCab(this));
    }

    public void deleteOldUuid(string nameFolder)
    {

        string filePath = Path.Combine(ConfigManager.BaseDir, "cabinetsdb", nameFolder, "SpatialAnchor.json");

        string jsonContent = File.ReadAllText(filePath);

        MyJsonObject myObject = JsonConvert.DeserializeObject<MyJsonObject>(jsonContent);

        myObject.Uuid = "";

        string updatedJsonContent = JsonConvert.SerializeObject(myObject, Formatting.Indented);

        File.WriteAllText(filePath, updatedJsonContent);
    }

    public string getUuidCab(string nameFolder)
    {
        string filePath = Path.Combine(ConfigManager.BaseDir, "cabinetsdb", nameFolder, "SpatialAnchor.json");

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            MyJsonObject myObject = JsonConvert.DeserializeObject<MyJsonObject>(jsonContent);

            return myObject.Uuid;
        }

        return "";
    }

    public List<Guid> GetSavedAnchorUUIDs()
    {
        List<Guid> uuids = new List<Guid>();
        string cabinetsdbPath = Path.Combine(ConfigManager.BaseDir, "cabinetsdb");
        DirectoryInfo directoryInfo = new DirectoryInfo(cabinetsdbPath);

        if (directoryInfo.Exists)
        {
            DirectoryInfo[] folders = directoryInfo.GetDirectories();

            foreach (DirectoryInfo folder in folders)
            {
                string filePath = Path.Combine(cabinetsdbPath, folder.Name, "SpatialAnchor.json");

                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    var uuidObject = JsonConvert.DeserializeObject<MyJsonObject>(jsonContent);
                    if (uuidObject.Uuid != "")
                    {
                        Guid uuid = Guid.Parse(uuidObject.Uuid);
                        uuids.Add(uuid);
                    }
                }
            }
        }

        return uuids;
    }
}