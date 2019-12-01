using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PopTheCircle.Game;
using GracesGames.SimpleFileBrowser.Scripts;
using UnityEngine.UI;

public class SimulatePrepareScene : MonoBehaviour
{
    public GameObject fileBrowserPrefab;
    public GameObject loadingPanel;
    public Text musicFilePathText;
    public Text noteDataFilePathText;
    public Button startButton;

    private string musicFilePath = "";
    private string noteDataFilePath = "";



    private void Awake()
    {
        UpdateUI();
    }

    public void OpenMusicBrowser()
    {
        GameObject fileBrowserObject = Instantiate(fileBrowserPrefab, transform);
        fileBrowserObject.name = "FileBrowser";

        FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        fileBrowserScript.SetupFileBrowser(
            (aspectRatio > 1.0f) ? ViewMode.Landscape : ViewMode.Portrait,
            (PlayerPrefs.HasKey("SimulatorLastDirectory")) ? PlayerPrefs.GetString("SimulatorLastDirectory", "") : ""
            );

        fileBrowserScript.OpenFilePanel(new string[] { "mp3", "ogg", "wav" });
        fileBrowserScript.OnFileSelect += LoadMusic;
    }

    public void OpenNoteDataBrowser()
    {
        GameObject fileBrowserObject = Instantiate(fileBrowserPrefab, transform);
        fileBrowserObject.name = "FileBrowser";

        FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        fileBrowserScript.SetupFileBrowser(
            (aspectRatio > 1.0f) ? ViewMode.Landscape : ViewMode.Portrait,
            (PlayerPrefs.HasKey("SimulatorLastDirectory")) ? PlayerPrefs.GetString("SimulatorLastDirectory", "") : ""
            );

        fileBrowserScript.OpenFilePanel(new string[] { "ntd" });
        fileBrowserScript.OnFileSelect += LoadNoteData;
    }

    private void LoadMusic(string _path)
    {
        StartCoroutine(MusicLoadingCoroutine(_path));
        musicFilePath = _path;

        string lastDirectory = _path;
        int i = _path.LastIndexOf('\\');
        if (i < 0)
            i = _path.LastIndexOf('/');
        if (i > 0)
            lastDirectory = _path.Substring(0, i);

        PlayerPrefs.SetString("SimulatorLastDirectory", lastDirectory);
        PlayerPrefs.Save();
    }

    private IEnumerator MusicLoadingCoroutine(string _path)
    {
        yield return null;
        loadingPanel.SetActive(true);
        if (!string.IsNullOrEmpty(_path))
        {
            Coroutine loadCoroutine = StartCoroutine(SaveLoad.LoadMusicFile(_path, OnMusicLoaded));
            yield return loadCoroutine;
        }
        loadingPanel.SetActive(false);
        yield return null;
    }
    
    private void OnMusicLoaded(AudioClip _clip)
    {
        GlobalData.Instance.musicClip = _clip;
        UpdateUI();
    }

    private void LoadNoteData(string _path)
    {
        GlobalData.Instance.noteDataJson = SaveLoad.LoadNoteDataJSON(_path);
        noteDataFilePath = _path;

        string lastDirectory = _path;
        int i = _path.LastIndexOf('\\');
        if (i < 0)
            i = _path.LastIndexOf('/');
        if (i > 0)
            lastDirectory = _path.Substring(0, i);

        PlayerPrefs.SetString("SimulatorLastDirectory", lastDirectory);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void StartSimulate()
    {
        SceneManager.LoadScene("Game");
    }

    private void UpdateUI()
    {
        musicFilePathText.text = musicFilePath;
        noteDataFilePathText.text = noteDataFilePath;

        if (string.IsNullOrEmpty(musicFilePath) || string.IsNullOrEmpty(noteDataFilePath))
            startButton.interactable = false;
        else
            startButton.interactable = true;
    }
}