using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Linq;


[RequireComponent(typeof(GridLayoutGroup))]
public class CabinetMenu : MonoBehaviour
{

    public GameObject ButtonSelectCabinetPrefab;
    public int columns = 4;
    public int rows = 10;

    private int currentIndex = 0;
    private int itemsPerPage = 5;

    private GridLayoutGroup gridLayoutGroup;

    private List<FileInfo> yamlFilesList = new List<FileInfo>();

    private string lastNameCabinetSelected = "";

    private GameObject insertCabinetGameObject;

    public TextMeshProUGUI gameTitle;

    public Transform painelSelectTransform;

    public VideoPlayer videoPlayer;
    private List<string> imageExtensions = new List<string>() { "jpg", "jpeg", "png", "gif", "bmp", "tiff", "ico" };

    public GameObject Components;

    private bool isMenuActive = true;

    private bool isKeyPressed = false;

    private int limitCabinetList = 9;

    private SpatialAnchorManager spatialAnchorManager;

    private GameObject lastGameObjectSelect;

    public Dictionary<string, string> lastCabInformationSelected = new Dictionary<string, string>();

    private GameObject buttonInsert;
    private GameObject buttonDelete;

    private List<string> cabinetInserted = new List<string>();

    private void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        spatialAnchorManager = GameObject.Find("SpatialAnchorManager").GetComponent<SpatialAnchorManager>();
    }

    void Start()
    {
        insertCabinetGameObject = GameObject.Find("InsertCabinet");

        StartCoroutine(loadFiles());
    }

    public IEnumerator loadFiles()
    {
        while (
            buttonDelete == null ||
            buttonInsert == null ||
            spatialAnchorManager == null ||
            insertCabinetGameObject == null
            )
        {

            insertCabinetGameObject = GameObject.Find("InsertCabinet");
            spatialAnchorManager = GameObject.Find("SpatialAnchorManager").GetComponent<SpatialAnchorManager>();
            buttonInsert = GameObject.Find("ButtonInsert");
            buttonDelete = GameObject.Find("ButtonDelete");

            yield return new WaitForSeconds(0.05f);
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(@"" + ConfigManager.BaseDir + "/cabinetsdb/");
        DirectoryInfo[] folders = directoryInfo.GetDirectories();

        foreach (DirectoryInfo folder in folders)
        {
            FileInfo[] yamlFiles = folder.GetFiles("description.yaml");
            if (yamlFiles.Length > 0)
            {
                yamlFilesList.Add(yamlFiles[0]);
            }
        }
        yield return new WaitForSeconds(0.05f);

        StartCoroutine(CreateInventory());
    }

    IEnumerator CreateInventory()
    {
         IDeserializer deserializator = new DeserializerBuilder()
         .WithNamingConvention(CamelCaseNamingConvention.Instance)
         .Build();

         for (int i = 0; i < yamlFilesList.Count; i++)
         {
             if (i > limitCabinetList)
                 break;

             using (var reader = new StreamReader(yamlFilesList[i].FullName))
             {

                 GameObject button = null;

                 try
                 {

                     var data = deserializator.Deserialize<Dictionary<string, object>>(reader);
                     Dictionary<object, object> videoData = (Dictionary<object, object>)data["video"];

                     button = Instantiate(ButtonSelectCabinetPrefab, painelSelectTransform);
                     Button btn = button.GetComponent<Button>();

                     string folderName = Path.GetFileName(yamlFilesList[i].DirectoryName);

                     string game = "Name Not Found";

                     if (data.ContainsKey("game") && !string.IsNullOrEmpty(data["game"].ToString()))
                     {
                         game = data["game"].ToString();
                     }
                     else if (data.ContainsKey("name") && !string.IsNullOrEmpty(data["name"].ToString()))
                     {
                         game = data["name"].ToString();
                     }
                     else if (data.ContainsKey("rom") && !string.IsNullOrEmpty(data["rom"].ToString()))
                     {
                         game = data["rom"].ToString();
                     }

                     string video = "";

                     if (videoData.ContainsKey("file") && !string.IsNullOrEmpty(videoData["file"].ToString()))
                     {
                         video = videoData["file"].ToString();
                     }

                     string rom = "";

                     if (data.ContainsKey("rom") && !string.IsNullOrEmpty(data["rom"].ToString()))
                     {
                         rom = Path.GetFileNameWithoutExtension(data["rom"].ToString());
                     }

                     TextMeshProUGUI buttonText = btn.GetComponentInChildren<TextMeshProUGUI>();
                     buttonText.text = game;

                     Dictionary<string, string> cabInformation = new Dictionary<string, string>
                     {
                         {"folderName", folderName},
                         {"game", game},
                         {"video", video},
                         {"rom", rom}
                     };

                     btn.onClick.AddListener(() => OnButtonSelectCabinet(cabInformation));

                     if (i == 0)
                     {
                         btn.onClick.Invoke();
                     }

                 }
                 catch (Exception ex)
                 {
                     Destroy(button);
                 }
             }
         }

        yield return new WaitForSeconds(0.05f);
    }

    private void OnButtonSelectCabinet(Dictionary<string, string> cabInformation)
    {

        buttonInsert.GetComponent<Button>().onClick.RemoveAllListeners();
        buttonDelete.GetComponent<Button>().onClick.RemoveAllListeners();

        lastCabInformationSelected = cabInformation;
        gameTitle.text = cabInformation["game"];

        videoPlayer.Stop();
        videoPlayer.GetComponent<VideoPlayer>().url = null;

        if (cabInformation["video"] != "")
        {
            string videoURL = ConfigManager.BaseDir + "/cabinetsdb/" + cabInformation["folderName"] + "/" + cabInformation["video"];
            videoPlayer.GetComponent<VideoPlayer>().url = videoURL;
            videoPlayer.Play();
        }

        buttonInsert.GetComponent<Button>().onClick.AddListener(() => OnButtonInsertCabinet(cabInformation));
        buttonDelete.GetComponent<Button>().onClick.AddListener(() => OnButtonDeleteCabinet(cabInformation));
    }

    public void OnButtonInsertCabinet(Dictionary<string, string> cabInformation)
    {

        if (cabInformation.Count != 0 && !cabinetInserted.Contains(cabInformation["rom"]))
        {
            buttonInsert.GetComponent<Button>().onClick.RemoveAllListeners();

            cabinetInserted.Add(cabInformation["rom"]);

            Vector3 position = new Vector3(0, 0, 0);

            videoPlayer.Stop();
            videoPlayer.GetComponent<VideoPlayer>().url = null;
            insertCabinetGameObject.GetComponent<InsertCabinet>().instanceCabinet(cabInformation, position, Quaternion.identity, false);
        }
    }

    public void OnButtonDeleteCabinet(Dictionary<string, string> cabInformation)
    {

        if (cabInformation.Count != 0)
        {

            buttonInsert.GetComponent<Button>().onClick.RemoveAllListeners();
            buttonDelete.GetComponent<Button>().onClick.RemoveAllListeners();

            cabinetInserted.Remove(cabInformation["rom"]);
            GameObject oldEmptyGameObject = GameObject.Find(cabInformation["rom"]);

            if (oldEmptyGameObject)
            {
                Destroy(oldEmptyGameObject);
            }

            spatialAnchorManager.deleteOldUuid(cabInformation["folderName"]);
        }
    }

    void Update()
    {
        if (!isKeyPressed && (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.RawButton.Start)))
        {
            isKeyPressed = true;
            isMenuActive = !isMenuActive;
            Components.SetActive(isMenuActive);
        }

        if (isKeyPressed && (Input.GetKeyUp(KeyCode.Space) || OVRInput.GetUp(OVRInput.RawButton.Start)))
        {
            isKeyPressed = false;
        }
    }
}