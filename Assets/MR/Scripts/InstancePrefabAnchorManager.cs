using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class InstancePrefabAnchorManager : MonoBehaviour
{
    private GameObject insertCabinetGameObject;
    public OVRPassthroughLayer passthroughLayer;
    public GameObject prefabFrame;
    public List<Texture2D> availableImages;
    private HashSet<int> usedIndices;
    private bool isLoadedPictures = false;

    void Start()
    {
        insertCabinetGameObject = GameObject.Find("InsertCabinet");

        loadPictures();
    }


    private void loadPictures()
    {
        Object[] allFiles = Resources.LoadAll("Decoration/Pictures");

        availableImages = new List<Texture2D>();
        usedIndices = new HashSet<int>();

        foreach (var file in allFiles)
        {

            if (file is Texture2D)
            {
                Texture2D texture = file as Texture2D;
                if (texture != null)
                {
                    availableImages.Add(texture);
                }
            }
        }

        isLoadedPictures = true;
    }

    public void instanceFrame(OVRSceneAnchor anchor)
    {
        Vector3 position = anchor.transform.position;
        Quaternion localRotation = anchor.transform.localRotation;

        Vector3 newPosition = new Vector3(position.x, position.y, position.z);

        GameObject instance = Instantiate(prefabFrame, newPosition, localRotation);

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, availableImages.Count);
        } while (usedIndices.Contains(randomIndex) || isLoadedPictures == false);

        usedIndices.Add(randomIndex);

        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.mainTexture = availableImages[randomIndex];

        Transform pictureTransform = instance.transform.Find("picture");
        Renderer pictureRenderer = pictureTransform.GetComponent<Renderer>();

        pictureRenderer.material = newMaterial;
        instance.AddComponent<OVRSpatialAnchor>();
    }

    public IEnumerator instanceCab(SpatialAnchorManager spatialAnchorManager)
    {

        passthroughLayer.textureOpacity = 0.1f;

        string cabinetsdbPath = Path.Combine(ConfigManager.BaseDir, "cabinetsdb");
        DirectoryInfo directoryInfo = new DirectoryInfo(cabinetsdbPath);

        if (directoryInfo.Exists)
        {
            DirectoryInfo[] folders = directoryInfo.GetDirectories();

            foreach (DirectoryInfo folder in folders)
            {
                string uuidJson = spatialAnchorManager.getUuidCab(folder.Name);

                if (uuidJson != "")
                {
                    var anchor = spatialAnchorManager.unboundAnchorsList.FirstOrDefault(a => a.Uuid.ToString() == uuidJson);

                    if (!anchor.Equals(default(OVRSpatialAnchor.UnboundAnchor)))
                    {

                        Vector3 position = anchor.Pose.position;
                        Quaternion rotation = anchor.Pose.rotation;

                        while (insertCabinetGameObject == null)
                        {
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
                    }
                }
                else
                {
                }
            }
        }
        else
        {
        }

        passthroughLayer.textureOpacity = 1f;

    }
}
