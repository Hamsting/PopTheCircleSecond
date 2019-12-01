using GracesGames.SimpleFileBrowser.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class BrowserManager : Singleton<BrowserManager>
    {
        public GameObject fileBrowserPrefab;

        

        public void OpenFileBrowser(FileBrowserMode _fileBrowserMode, Action<string> _onPathSelected)
        {
            GameObject fileBrowserObject = Instantiate(fileBrowserPrefab, transform);
            fileBrowserObject.name = "FileBrowser";
            
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            fileBrowserScript.SetupFileBrowser(
                (aspectRatio > 1.0f) ? ViewMode.Landscape : ViewMode.Portrait,
                (PlayerPrefs.HasKey("LastDirectory")) ? PlayerPrefs.GetString("LastDirectory", "") : ""
                );
            if (_fileBrowserMode == FileBrowserMode.Save)
            {
                fileBrowserScript.SaveFilePanel("Untitled", new string[] { "ntd" });
                fileBrowserScript.OnFileSelect += _onPathSelected;
            }
            else
            {
                fileBrowserScript.OpenFilePanel(new string[] { "ntd" });
                fileBrowserScript.OnFileSelect += _onPathSelected;
            }
        }
    }
}