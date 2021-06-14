using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public static class SaveLoad
    {
        public static IEnumerator LoadMusicFile(string _path, Action<AudioClip> _callback)
        {
            string path = _path;

            if (string.IsNullOrEmpty(path) || path.Equals("-"))
            {
                _callback(null);
            }
            if (!Path.IsPathRooted(path))
            {
                string noteDataDir = MakerManager.Instance.noteDataFilePath;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                noteDataDir = noteDataDir.Substring(0, noteDataDir.LastIndexOf("\\")) + "\\";
#else
                noteDataDir = noteDataDir.Substring(0, noteDataDir.LastIndexOf("/")) + "/";
#endif
                path = noteDataDir + _path;
            }

            float timer = 0.0f;
            bool isFailed = false;

            WWW www = new WWW("file://" + path);
            while (!www.isDone)
            {
                if (timer > 5.0f)
                {
                    isFailed = true;
                    break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            if (isFailed)
            {
                Debug.LogError("SaveLoad::LoadMusicFile timeout");
                www.Dispose();
                _callback(null);
            }
            else if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("SaveLoad::LoadMusicFile has error : " + www.error);
                www.Dispose();
                _callback(null);
            }
            else
            {
                AudioClip clip = null;
#if !UNITY_ANDROID && !UNITY_IOS
                clip = NAudioPlayer.FromMp3Data(www.bytes);
#else
                clip = www.GetAudioClip();
#endif
                _callback(clip);
            }
            yield return null;
        }

        public static void SaveNoteDataJSON(string _path, JSONObject _noteDataJson)
        {
            if (!string.IsNullOrEmpty(_path) && _noteDataJson != null)
                File.WriteAllText(_path, _noteDataJson.ToString(true), Encoding.Unicode);
            else
                Debug.Log("SaveLoad::Invalid path or empty file given : " + _path);
        }

        public static JSONObject LoadNoteDataJSON(string _path)
        {
            if (!string.IsNullOrEmpty(_path))
            {
                FileInfo fileInfo = new FileInfo(_path);
                if (!fileInfo.Exists)
                {
                    Debug.Log("SaveLoad::Invalid path or empty file given : " + _path);
                    return null;
                }

                string jsonStr = File.ReadAllText(_path, Encoding.Unicode);
                return new JSONObject(jsonStr);
            }
            else
            {
                Debug.Log("SaveLoad::Invalid path or empty file given : " + _path);
                return null;
            }
        }
    }
}