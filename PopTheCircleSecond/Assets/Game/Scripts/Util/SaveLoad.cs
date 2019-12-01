using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PopTheCircle.Game
{
    public static class SaveLoad
    {
        public static IEnumerator LoadMusicFile(string _path, Action<AudioClip> _callback)
        {
            float timer = 0.0f;
            bool isFailed = false;

            WWW www = new WWW("file://" + _path);
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
        
        public static JSONObject LoadNoteDataJSON(string _path)
        {
            if (!string.IsNullOrEmpty(_path))
            {
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