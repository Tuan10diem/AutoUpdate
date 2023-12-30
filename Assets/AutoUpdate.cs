using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class AutoUpdate : MonoBehaviour
{

    string url = "https://vlph.cdnjx.mobi/G4VN/asset";
    
    List<string> listFiles = new List<string>();

    private IEnumerator VersionCheck()
    {
        string localFolderPath = Path.Combine(Application.persistentDataPath, "Data");
        string localFolderVer=PlayerPrefs.GetString("LocalFolderVer","");
        string curVersion = PlayerPrefs.GetString("CurrentVersion", "");
        
        string verPath = url + "/VersionCheckG4VN.json";
        UnityWebRequest request = UnityWebRequest.Get(verPath);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Connect Success");
            string lastModified = request.GetResponseHeader("Last-Modified");
            string lastModifiedFolder="-";
            if (Directory.Exists(localFolderPath))
            {
                lastModifiedFolder = System.IO.File.GetLastWriteTime(localFolderPath).ToString();
            }
            if (curVersion != lastModified || lastModifiedFolder!=localFolderVer)
            {
                PlayerPrefs.SetString("CurrentVersion", lastModified);
                
                StartCoroutine(UpdateFile());
                PlayerPrefs.SetString("LocalFolderVer", lastModifiedFolder);
            }
            else
            {
                Debug.Log("Dont need to update");
            }
        }

    }

    private IEnumerator UpdateFile()
    {
        Debug.Log("Updating...");
        string urlAsset = "https://vlph.cdnjx.mobi/G4VN/asset/data";
        for (int i = 0; i < listFiles.Count; i++)
        {
           
            string path = Path.Combine(urlAsset, listFiles[i]);
            UnityWebRequest request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string lastModifiedFileLocal = "";
                string localFilePath = Path.Combine(Application.persistentDataPath, "Data", listFiles[i]);
                if (System.IO.File.Exists(localFilePath))
                {
                    lastModifiedFileLocal = System.IO.File.GetLastWriteTime(localFilePath).ToString();
                }
                else
                {
                    PlayerPrefs.SetString(listFiles[i] + "File", "-");
                }

                string lastModifiedOnHost = request.GetResponseHeader("Last-Modified");
                if (PlayerPrefs.GetString(listFiles[i], "") != lastModifiedOnHost || lastModifiedFileLocal != PlayerPrefs.GetString(listFiles[i] + "File", ""))
                {
                    if (lastModifiedFileLocal != PlayerPrefs.GetString(listFiles[i] + "File", "")) Debug.Log(lastModifiedFileLocal + "|" + PlayerPrefs.GetString(listFiles[i] + "File", ""));
                    PlayerPrefs.SetString(listFiles[i], lastModifiedOnHost);
                    Debug.Log(listFiles[i] + " Need To Update");
                    Updating(request, listFiles[i]);

                }

            }
            else
            {
                Debug.Log("Connect Failed");
            }
            
        }
       
        yield return null;
    }

    private void Updating(UnityWebRequest request, string fileName)
    {
        string localFilePath = Path.Combine(Application.persistentDataPath, "Data", fileName);
        System.IO.File.WriteAllBytes(localFilePath, request.downloadHandler.data);
        PlayerPrefs.SetString(fileName + "File", System.IO.File.GetLastWriteTime(localFilePath).ToString());
    }

    private void CreateFolder(string folderPath) 
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Folder created at: " + folderPath);
        }
        else
        {
            Debug.Log("Folder already exists at: " + folderPath);
        }
    }

    private void InitListFile()
    {
        listFiles.Add("amulet.json");
        listFiles.Add("armor.json");
        listFiles.Add("belt.json");
        listFiles.Add("boodSettings.json");
        listFiles.Add("boot.json");
        listFiles.Add("BuySellSettings.json");
        listFiles.Add("cuff.json");
        listFiles.Add("GoodSettings.json");
        listFiles.Add("helm.json");
        listFiles.Add("horse.json");
        listFiles.Add("leveladd.json");
        listFiles.Add("levelexp.json");
        listFiles.Add("magicattrib.json");
        listFiles.Add("magicdesc.json");
        listFiles.Add("map.json");
        listFiles.Add("meleeweapon.json");
        listFiles.Add("missle.json");
        listFiles.Add("normalsettings.json");
        listFiles.Add("ObjDropData.json");
        listFiles.Add("pendant.json");
        listFiles.Add("potion.json");
        listFiles.Add("questkey.json");
        listFiles.Add("rangeweapon.json");
        listFiles.Add("ring.json");
        listFiles.Add("skill.json");
        listFiles.Add("specialaction.json");
        listFiles.Add("SpecialEquip.json");
        listFiles.Add("speciallayer.json");
        listFiles.Add("statemagic.json");
        listFiles.Add("townportal.json");
    }

    // Start is called before the first frame update
    void Awake()
    {
        InitListFile();
        StartCoroutine(VersionCheck());
        CreateFolder(Path.Combine(Application.persistentDataPath, "Data"));
    }
}
