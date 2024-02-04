using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

public class SpatialAnchorManager : MonoBehaviour
{

    public class MyJsonObject
    {
        public string Uuid { get; set; }
    }

    public void LoadAnchorByUuid(string uuid, string name, GameObject InsertCabinet)
    {
        Guid guidUuid = Guid.Parse(uuid);

        OVRSpatialAnchor.LoadOptions options = new OVRSpatialAnchor.LoadOptions
        {
            Timeout = 0,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = new List<Guid> { guidUuid }
        };

        Debug.Log("[DEBUG] Procurando a anchora local ");

        OVRSpatialAnchor.LoadUnboundAnchors(options, anchorSavedUUIDList =>
        {
            foreach (var anchor in anchorSavedUUIDList)
            {
                if (anchor.Localized)
                {
                    Debug.Log("[DEBUG] Anchora Localizada");
                    instanceCab(name, InsertCabinet, anchor);
                    break;
                }
                else
                {
                    anchor.Localize((a, success) =>
                    {

                        if (success)
                        {
                            instanceCab(name, InsertCabinet, a);
                            Debug.Log("[DEBUG] Anchora Localizada em anchor.Localize");
                        }
                        else
                        {
                            Debug.Log("[DEBUG] Anchora nao Localizada em anchor.Localize");
                        }

                    });

                    Debug.Log("[DEBUG] Anchora Nao Localizada");
                }
            }
        });
    }

    void instanceCab(string name, GameObject InsertCabinet, OVRSpatialAnchor.UnboundAnchor anchor)
    {
        Debug.Log("[DEBUG] Instanciando Gabinete salvo posição");
        InsertCabinet.GetComponent<InsertCabinet>().instanceCabinet(name, anchor.Pose.position, anchor.Pose.rotation, true);
    }

    public void managerInstanceSpatialAnchor(GameObject InsertCabinet, string nameFolder)
    {
        string filePath = Path.Combine(ConfigManager.BaseDir, "cabinetsdb", nameFolder, "SpatialAnchor.json");

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            MyJsonObject myObject = JsonConvert.DeserializeObject<MyJsonObject>(jsonContent);

            Debug.Log("[DEBUG] SpatialAnchor.json Existe! ");

            if (myObject.Uuid != "")
            {
                Debug.Log("[DEBUG] SpatialAnchor.json tem posição salva ");
                LoadAnchorByUuid(myObject.Uuid, nameFolder, InsertCabinet);
            }
            else
            {
                Debug.Log("[DEBUG] SpatialAnchor.json nao tem posição salva ");
            }
        }
        else
        {
            Debug.Log("[DEBUG] SpatialAnchor.json Nao Existe! ");
        }
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
}