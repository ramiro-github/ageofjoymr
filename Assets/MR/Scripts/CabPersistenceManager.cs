using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class CabPersistenceManager : MonoBehaviour
{

    private GameObject insertCabinetGameObject;
    public OVRPassthroughLayer passthroughLayer;

    void Start()
    {
        insertCabinetGameObject = GameObject.Find("InsertCabinet");
    }

    public IEnumerator instanceCab(SpatialAnchorManager spatialAnchorManager)
    {

        passthroughLayer.textureOpacity =  0.1f;

        Debug.Log("[DEBUG] Começando instancia cab");

        string cabinetsdbPath = Path.Combine(ConfigManager.BaseDir, "cabinetsdb");
        DirectoryInfo directoryInfo = new DirectoryInfo(cabinetsdbPath);

        if (directoryInfo.Exists)
        {
            DirectoryInfo[] folders = directoryInfo.GetDirectories();

            foreach (DirectoryInfo folder in folders)
            {
                string uuidJson = spatialAnchorManager.getUuidCab(folder.Name);
                Debug.Log("[DEBUG] Uuid do json: " + uuidJson);

                if (uuidJson != "")
                {
                    var anchor = spatialAnchorManager.unboundAnchorsList.FirstOrDefault(a => a.Uuid.ToString() == uuidJson);

                    if (!anchor.Equals(default(OVRSpatialAnchor.UnboundAnchor)))
                    {
                        Debug.Log("[DEBUG] Instanciando o cabinet");

                        Vector3 position = anchor.Pose.position;
                        Quaternion rotation = anchor.Pose.rotation;

                        while (insertCabinetGameObject == null)
                        {
                            Debug.Log("[DEBUG] Aguardando instancia insertCabinet");
                            insertCabinetGameObject = GameObject.Find("InsertCabinet");
                            yield return new WaitForSeconds(1f);
                        }

                        insertCabinetGameObject.GetComponent<InsertCabinet>().instanceCabinet(folder.Name, position, rotation, true);
                        yield return new WaitForSeconds(2f);

                        GameObject currentCabinet = GameObject.Find(folder.Name);

                        while (currentCabinet == null)
                        {
                            yield return new WaitForSeconds(1f);
                            currentCabinet = GameObject.Find(folder.Name);
                        }

                    }
                    else
                    {
                        Debug.Log("[DEBUG] Sem ancora para o cabinet");
                    }
                }
                else
                {
                    Debug.Log("[DEBUG] Uuid do json vazio ");
                }
            }
        }
        else
        {
            Debug.Log("[DEBUG] Não existe a pasta cabinetsdb");
        }

        passthroughLayer.textureOpacity =  1f;

    }
}
