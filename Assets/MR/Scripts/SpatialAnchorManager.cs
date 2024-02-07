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

    public GameObject cabPersistenceManager;

    public List<OVRSpatialAnchor.UnboundAnchor> unboundAnchorsList = new List<OVRSpatialAnchor.UnboundAnchor>();


    void Start()
    {
        StartCoroutine(LoadAllAnchor());
    }

    private IEnumerator LoadAllAnchor()
    {

        Debug.Log("[DEBUG] Inicio Carregar anchoras");

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
                    Debug.Log("[DEBUG] Anchora localizada: " + anchor.Uuid);
                }
                else
                {
                    anchor.Localize((a, success) =>
                    {

                        if (success)
                        {
                            unboundAnchorsList.Add(anchor);
                            Debug.Log("[DEBUG] Anchora localizada depois: " + anchor.Uuid);
                        }

                    });
                }
            }
        });

        yield return new WaitForSeconds(0.05f);

        StartCoroutine(cabPersistenceManager.GetComponent<CabPersistenceManager>().instanceCab(this));
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