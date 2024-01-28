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

    public GameObject InsertCabinet;

    public TextMeshProUGUI textDescription;

    public Transform painelSelectTransform;

    public VideoPlayer videoPlayer;
    private List<string> imageExtensions = new List<string>() { "jpg", "jpeg", "png", "gif", "bmp", "tiff", "ico" };

    public GameObject Components;

    private bool isMenuActive = true;

    private bool isKeyPressed = false;


    private void Awake()
    {

        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        StartCoroutine(loadFiles());

    }

    void Start()
    {
        InsertCabinet = GameObject.Find("InsertCabinet");
    }

    IEnumerator loadFiles()
    {

        Debug.Log("[DEBUG] Yaml carregando");

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

        Debug.Log("[DEBUG] Yaml carregado");

        StartCoroutine(CreateInventory());
    }

    IEnumerator CreateInventory()
    {

        DestroyChildren(painelSelectTransform);

        Debug.Log("[DEBUG] Listagem carregando");

        IDeserializer deserializator = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

        for (int i = currentIndex; i < Mathf.Min(currentIndex + itemsPerPage, yamlFilesList.Count); i++)
        {
            using (var reader = new StreamReader(yamlFilesList[i].FullName))
            {
                GameObject button = null;
                try
                {
                    var data = deserializator.Deserialize<Dictionary<string, object>>(reader);
                    Dictionary<object, object> videoData = (Dictionary<object, object>)data["video"];

                    button = Instantiate(ButtonSelectCabinetPrefab, painelSelectTransform);
                    Button btn = button.GetComponent<Button>();

                    string nameFolder = data["name"].ToString();
                    string nameGame = data["game"].ToString();
                    string nameVideo = videoData["file"].ToString();

                    StartCoroutine(LoadImage(btn, nameFolder));
                    btn.onClick.AddListener(() => OnButtonSelectCabinet(nameFolder, nameVideo));

                }
                catch (Exception ex)
                {
                    Destroy(button);
                    Debug.Log("[DEBUG] Erro em carregar a listagem: " + ex.Message);
                }
            }
        }

        Debug.Log("[DEBUG] Listagem carregado");

        yield return new WaitForSeconds(0.05f);
    }

    void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    IEnumerator LoadImage(Button btn, string nameFolder)
    {

        bool isSet = false;

        foreach (string ext in imageExtensions)
        {
            string imagePath = Path.Combine(ConfigManager.BaseDir + "/cabinetsdb/", nameFolder, "marquee." + ext);

            if (File.Exists(imagePath))
            {
                byte[] fileData = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(fileData);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                btn.GetComponent<Image>().sprite = sprite;
                isSet = true;
                break;
            }
        }

        yield return null;
    }

    private void OnButtonSelectCabinet(string name, string video)
    {
        lastNameCabinetSelected = name;

        videoPlayer.Stop();
        videoPlayer.GetComponent<VideoPlayer>().url = null;

        string videoURL = ConfigManager.BaseDir + "/cabinetsdb/" + name + "/" + video;
        videoPlayer.GetComponent<VideoPlayer>().url = videoURL;
        videoPlayer.Play();
    }

    public void OnButtonInsertCabinet()
    {

        if (lastNameCabinetSelected != "")
        {
            videoPlayer.Stop();
            videoPlayer.GetComponent<VideoPlayer>().url = null;
            InsertCabinet.GetComponent<InsertCabinet>().instanceCabinet(lastNameCabinetSelected);
            lastNameCabinetSelected = "";
        }
        else
        {
            Debug.Log("[DEBUG] lastNameCabinetSelected não existe");
        }
    }

    public void OnButtonBackToVR()
    {
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene("IntroGallery");
    }


    public void OnButtonNextList()
    {
        currentIndex += itemsPerPage;
        StartCoroutine(CreateInventory());
    }

    public void OnButtonBackList()
    {
        currentIndex -= itemsPerPage;
        if (currentIndex < 0)
        {
            currentIndex = 1;
        }
        StartCoroutine(CreateInventory());
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