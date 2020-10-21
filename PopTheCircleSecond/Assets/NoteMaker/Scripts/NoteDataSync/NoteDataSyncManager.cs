using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class NoteDataSyncManager : Singleton<NoteDataSyncManager>
    {
        public static readonly string FTPUserName = "pi";
        public static readonly string FTPPassword = "coineve4267*";
        public static readonly string FTPServerRoot = "ftp://119.207.165.159:42671/SyncedNoteData/";

        public string syncRootPath = "";
        public bool IsSyncFinished
        {
            get { return isTaskFinished; }
        }
        [InspectorReadOnly] public bool isUploadPopupNeededAndOpened = false;
        [InspectorReadOnly] public bool isDownloadPopupNeededAndOpened = false;
        [InspectorReadOnly] public bool isPopupConfirmed = false;
        [InspectorReadOnly] public int loadedServerHashVersion = 0;
        [InspectorReadOnly] public string loadedServerHashDateStr = "";
        [TextArea] public string localHashesJsonStr = "";

        private Thread thread;
        private bool isTaskFinished = true;
        private bool isServerHashesDownloadCompleted = false;
        private string serverhashesResultStr = "";
        private List<string> uploadFileList = new List<string>();
        private List<string> downloadFileList = new List<string>();
        private List<string> removeFileList = new List<string>();
        private List<string> createDirList = new List<string>();
        private List<string> removeDirList = new List<string>();
        private bool isPopupOpened = false;
        private bool isLoadingPanelNeeded = false;
        private bool isFileDownloadCompleted = false;
        private bool isFileUploadCompleted = false;



        private void Start()
        {
            if (PlayerPrefs.HasKey("syncRootPath"))
                syncRootPath = PlayerPrefs.GetString("syncRootPath");
            else
                syncRootPath = "";
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (!isTaskFinished && thread != null)
            {
                thread.Abort();
            }
        }

        private void Update()
        {
            if (isLoadingPanelNeeded != MakerUIManager.Instance.loadingPanel.activeSelf)
            {
                MakerUIManager.Instance.loadingPanel.SetActive(isLoadingPanelNeeded);
            }

            if (isUploadPopupNeededAndOpened && !isPopupOpened)
            {
                MakerUIManager.Instance.popup.OpenSyncUploadNotePopup();
                isPopupOpened = true;
            }
            if (isDownloadPopupNeededAndOpened && !isPopupOpened)
            {
                MakerUIManager.Instance.popup.OpenSyncDownloadNotePopup();
                isPopupOpened = true;
            }
        }

        public void SetSyncRootPath(string _path)
        {
            syncRootPath = new FileInfo(_path).Directory.FullName;
            if (!syncRootPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                syncRootPath += Path.DirectorySeparatorChar;

            PlayerPrefs.SetString("syncRootPath", syncRootPath);
            PlayerPrefs.Save();
        }

        public string MakeUploadChangeListString()
        {
            string res = "";

            res += "\n## Upload Local Files : \n";
            foreach (var s in uploadFileList)
            {
                res += "  - " + s + "\n";
            }

            res += "\n## Remove Server Files : \n";
            foreach (var s in removeFileList)
            {
                res += "  - " + s + "\n";
            }

            res += "\n## Create Server Directories : \n";
            foreach (var s in createDirList)
            {
                res += "  - " + s + "\n";
            }

            res += "\n## Remove Server Directories : \n";
            foreach (var s in removeDirList)
            {
                res += "  - " + s + "\n";
            }

            return res;
        }

        public string MakeDownloadChangeListString()
        {
            string res = "";

            res += "\n## Download Server Files : \n";
            foreach (var s in downloadFileList)
            {
                res += "  - " + s + "\n";
            }

            res += "\n## Remove Local Files : \n";
            foreach (var s in removeFileList)
            {
                res += "  - " + s + "\n";
            }

            res += "\n## Create Local Directories : \n";
            foreach (var s in createDirList)
            {
                res += "  - " + s + "\n";
            }

            res += "\n## Remove Local Directories : \n";
            foreach (var s in removeDirList)
            {
                res += "  - " + s + "\n";
            }

            return res;
        }





        public bool StartSyncUpload()
        {
            if (!isTaskFinished)
            {
                Debug.Log("NoteDataSyncManager::Working task already exists, Try later");
                return false;
            }

            thread = new Thread(new ThreadStart(SyncUploadTask));
            thread.IsBackground = true;
            thread.Start();
            return true;
        }

        private void SyncUploadTask()
        {
            try
            {
                isTaskFinished = false;
                isLoadingPanelNeeded = true;

                Dictionary<string, string> localHashes;
                if (!ChecksumGenerator.CreateChecksumDic(syncRootPath, out localHashes))
                {
                    isTaskFinished = true;
                    return;
                }
                localHashesJsonStr = ChecksumGenerator.ChecksumDicToJSON(localHashes).ToString(true);
                File.WriteAllText(Path.Combine(syncRootPath, "hashes.json"), localHashesJsonStr, Encoding.UTF8);

                DownloadServerHashes();
                while (!isServerHashesDownloadCompleted) { Thread.Sleep(16); }

                var serverHashesJson = new JSONObject(serverhashesResultStr);
                loadedServerHashDateStr = serverHashesJson["updatedTime"]?.str ?? "Not Defined";
                loadedServerHashVersion = (int)(serverHashesJson["version"]?.i ?? -1);

                var serverHashes = ChecksumGenerator.JSONToChecksumDic(serverHashesJson);

                uploadFileList.Clear();
                removeFileList.Clear();
                createDirList.Clear();
                removeDirList.Clear();
                foreach (var p in serverHashes)
                {
                    string pKey = p.Key.Replace(Path.DirectorySeparatorChar, '/');

                    if (string.IsNullOrEmpty(pKey))
                        continue;

                    if (localHashes.ContainsKey(pKey))
                    {
                        if (!p.Value.Equals("Dir") && !p.Value.Equals(localHashes[pKey]))
                            uploadFileList.Add(pKey);

                        localHashes.Remove(pKey);
                    }
                    else
                    {
                        if (p.Value.Equals("Dir"))
                            removeDirList.Add(pKey);
                        else
                            removeFileList.Add(pKey);
                    }
                }
                foreach (var p in localHashes)
                {
                    if (p.Value.Equals("Dir"))
                        createDirList.Add(p.Key);
                    else
                        uploadFileList.Add(p.Key);
                }

                isLoadingPanelNeeded = false;
                isPopupOpened = false;
                isUploadPopupNeededAndOpened = true;
                while (isUploadPopupNeededAndOpened) { Thread.Sleep(16); }
                if (!isPopupConfirmed)
                {
                    isTaskFinished = true;
                    return;
                }

                isLoadingPanelNeeded = true;
                foreach (var cd in createDirList)
                {
                    CreateFTPDir(cd);
                }
                foreach (var uf in uploadFileList)
                {
                    UploadFileToFTP(uf);
                }
                foreach (var rf in removeFileList)
                {
                    RemoveFTPFile(rf);
                }
                if (removeDirList.Count > 0)
                {
                    // CraeteFTPTrashcanDir(ChecksumGenerator.lastDateTimeStr);
                    foreach (var rd in removeDirList)
                    {
                        RemoveFTPDir(rd);
                    }
                }
                UploadFileToFTP("hashes.json");

                isLoadingPanelNeeded = false;
                isTaskFinished = true;
            }
            catch (Exception _e)
            {
                Debug.LogError(_e);
                isTaskFinished = true;
            }
        }

        public bool StartSyncDownload()
        {
            if (!isTaskFinished)
            {
                Debug.Log("NoteDataSyncManager::Working task already exists, Try later");
                return false;
            }

            thread = new Thread(new ThreadStart(SyncDownloadTask));
            thread.IsBackground = true;
            thread.Start();
            return true;
        }

        private void SyncDownloadTask()
        {
            try
            {
                isTaskFinished = false;
                isLoadingPanelNeeded = true;

                Dictionary<string, string> localHashes;
                if (!ChecksumGenerator.CreateChecksumDic(syncRootPath, out localHashes))
                {
                    isTaskFinished = true;
                    return;
                }
                localHashesJsonStr = ChecksumGenerator.ChecksumDicToJSON(localHashes).ToString(true);
                File.WriteAllText(Path.Combine(syncRootPath, "hashes.json"), localHashesJsonStr, Encoding.UTF8);

                DownloadServerHashes();
                while (!isServerHashesDownloadCompleted) { Thread.Sleep(16); }

                var serverHashesJson = new JSONObject(serverhashesResultStr);
                loadedServerHashDateStr = serverHashesJson["updatedTime"]?.str ?? "Not Defined";
                loadedServerHashVersion = (int)(serverHashesJson["version"]?.i ?? -1);

                var serverHashes = ChecksumGenerator.JSONToChecksumDic(serverHashesJson);

                downloadFileList.Clear();
                removeFileList.Clear();
                createDirList.Clear();
                removeDirList.Clear();
                foreach (var p in serverHashes)
                {
                    string pKey = p.Key.Replace(Path.DirectorySeparatorChar, '/');

                    if (string.IsNullOrEmpty(pKey))
                        continue;

                    if (localHashes.ContainsKey(pKey))
                    {
                        if (!p.Value.Equals("Dir") && !p.Value.Equals(localHashes[pKey]))
                            downloadFileList.Add(pKey);

                        localHashes.Remove(pKey);
                    }
                    else
                    {
                        if (p.Value.Equals("Dir"))
                            createDirList.Add(pKey);
                        else
                            downloadFileList.Add(pKey);
                    }
                }
                foreach (var p in localHashes)
                {
                    if (p.Value.Equals("Dir"))
                        removeDirList.Add(p.Key);
                    else
                        removeFileList.Add(p.Key);
                }

                isLoadingPanelNeeded = false;
                isPopupOpened = false;
                isDownloadPopupNeededAndOpened = true;
                while (isDownloadPopupNeededAndOpened) { Thread.Sleep(16); }
                if (!isPopupConfirmed)
                {
                    isTaskFinished = true;
                    return;
                }

                isLoadingPanelNeeded = true;
                foreach (var cd in createDirList)
                {
                    Directory.CreateDirectory(Path.Combine(syncRootPath, cd));
                }
                foreach (var df in downloadFileList)
                {
                    DownloadFileFromFTP(df);
                    while (!isFileDownloadCompleted) { Thread.Sleep(16); }
                }
                foreach (var rf in removeFileList)
                {
                    File.Delete(Path.Combine(syncRootPath, rf));
                }
                foreach (var rd in removeDirList)
                {
                    Directory.Delete(Path.Combine(syncRootPath, rd));
                }

                isLoadingPanelNeeded = false;
                isTaskFinished = true;
            }
            catch (Exception _e)
            {
                Debug.LogError(_e);
                isTaskFinished = true;
            }
        }




        
        public void UploadFileToFTP(string _relPath)
        {
            string targetFilePath = FTPServerRoot + _relPath;

            WebClient client = new WebClient();
            Uri uri = new Uri(targetFilePath);

            client.UploadFileCompleted += new UploadFileCompletedEventHandler(OnFileUploadCompleted);
            client.Credentials = new System.Net.NetworkCredential(FTPUserName, FTPPassword);
            client.UploadFileAsync(uri, WebRequestMethods.Ftp.UploadFile, Path.Combine(syncRootPath, _relPath));
        }
        
        private void OnFileUploadCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            isFileUploadCompleted = true;
        }

        private void RemoveFTPFile(string _relPath)
        {
            try
            {
                var ftpRequest = (FtpWebRequest)WebRequest.Create(Path.Combine(FTPServerRoot, _relPath));
                ftpRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassword);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;

                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception _ex) { Debug.LogError(_ex.ToString()); }
        }

        private void RemoveFTPDir(string _relPath)
        {
            /*
            try
            {
                var ftpRequest = (FtpWebRequest)WebRequest.Create(Path.Combine(FTPServerRoot, _relPath));
                ftpRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassword);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.Rename; // WebRequestMethods.Ftp.DeleteFile; // WebRequestMethods.Ftp.RemoveDirectory;


                string renameTo = Path.Combine(FTPServerRoot, "../NoteDataTrashcan", _relPath);
                if (renameTo.EndsWith("/"))
                    renameTo = renameTo.Remove(renameTo.Length - 1, 1);
                ftpRequest.RenameTo = renameTo;

                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception _ex) { Debug.LogError(_ex.ToString()); }
            */
        }

        private void CreateFTPDir(string _relPath)
        {
            try
            {
                var ftpRequest = (FtpWebRequest)WebRequest.Create(Path.Combine(FTPServerRoot, _relPath));
                ftpRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassword);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;

                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception _ex) { Debug.LogError(_ex.ToString()); }
        }

        /*
        private void CraeteFTPTrashcanDir(string _timeStr)
        {
            try
            {
                var ftpRequest = (FtpWebRequest)WebRequest.Create(Path.Combine(FTPServerRoot, "../NoteDataTrashcan", _timeStr));
                ftpRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassword);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;

                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception _ex) { Debug.LogError(_ex.ToString()); }

            try
            {
                var ftpRequest = (FtpWebRequest)WebRequest.Create(Path.Combine(FTPServerRoot, "../NoteDataTrashcan", _timeStr));
                ftpRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassword);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.Rename; // WebRequestMethods.Ftp.DeleteFile; // WebRequestMethods.Ftp.RemoveDirectory;
                
                string renameTo = Path.Combine(FTPServerRoot, "../NoteDataTrashcan", _timeStr + "_Hashipal");
                if (renameTo.EndsWith("/"))
                    renameTo = renameTo.Remove(renameTo.Length - 1, 1);
                ftpRequest.RenameTo = renameTo;

                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception _ex) { Debug.LogError(_ex.ToString()); }
        }
        */





        public void DownloadFileFromFTP(string _relPath)
        {
            string targetFilePath = FTPServerRoot + _relPath;

            WebClient client = new WebClient();
            Uri uri = new Uri(targetFilePath);

            client.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDataDownloadCompleted);
            client.Credentials = new System.Net.NetworkCredential(FTPUserName, FTPPassword);
            client.DownloadFileAsync(uri, Path.Combine(syncRootPath, _relPath));
        }
        
        private void OnDataDownloadCompleted(object _sender, AsyncCompletedEventArgs _e)
        {
            isFileDownloadCompleted = true;
        }
        




        private void DownloadServerHashes()
        {
            serverhashesResultStr = "";
            isServerHashesDownloadCompleted = false;

            string targetFilePath = FTPServerRoot + "hashes.json";

            WebClient client = new WebClient();
            Uri uri = new Uri(targetFilePath);

            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloadServerHashesCompleted);
            client.Credentials = new System.Net.NetworkCredential(FTPUserName, FTPPassword);
            client.DownloadStringAsync(uri);
        }

        private void OnDownloadServerHashesCompleted(object _sender, DownloadStringCompletedEventArgs _e)
        {
            if (string.IsNullOrEmpty(_e.Result))
            {
                Debug.LogError("NoteDataSyncManager::Server hashes download failed");
                return;
            }

            serverhashesResultStr = _e.Result;
            isServerHashesDownloadCompleted = true;
        }
    }
}