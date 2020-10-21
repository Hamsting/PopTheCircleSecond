using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public static class ChecksumGenerator : System.Object
    {
        public const int GeneratorVersion = 1;

        public static string lastDateTimeStr = "1970-01-01 00:00:01";



        public static bool CreateChecksumDic(string _rootDir, out Dictionary<string, string> _hashes)
        {
            _hashes = new Dictionary<string, string>();

            try
            {
                DirectoryInfo rootDirInfo = new DirectoryInfo(_rootDir);
                if (!rootDirInfo.Exists)
                {
                    Debug.LogError("ChecksumGenerator::_rootDir not exists");
                    return false;
                }

                var subDirInfos = rootDirInfo.GetDirectories("*", SearchOption.AllDirectories);
                foreach (var subDirInfo in subDirInfos)
                {
                    string relPath = GetRelativePath(subDirInfo.FullName, _rootDir);
                    _hashes.Add(relPath, "Dir");
                }

                var allFileInfos = rootDirInfo.GetFiles("*", SearchOption.AllDirectories);
                foreach (var fileInfo in allFileInfos)
                {
                    if (fileInfo.Name.Equals("hashes.json"))
                        continue;

                    string fullPath = fileInfo.FullName;
                    string relPath = GetRelativePath(fullPath, _rootDir);

                    byte[] btAscii = File.ReadAllBytes(fullPath);
                    byte[] btHash = MD5.Create().ComputeHash(btAscii);
                    string hashStr = BitConverter.ToString(btHash).Replace("-", "").ToLower();

                    _hashes.Add(relPath, hashStr.Replace(Path.DirectorySeparatorChar, '/'));
                }

                return true;
            }
            catch (Exception _e)
            {
                Debug.LogError(_e);
                return false;
            }
        }

        private static string GetRelativePath(string _targetPath, string _rootDir)
        {
            Uri pathUri = new Uri(_targetPath);
            if (!_rootDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                _rootDir += Path.DirectorySeparatorChar;

            Uri folderUri = new Uri(_rootDir);
            // return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString());
        }

        public static JSONObject ChecksumDicToJSON(Dictionary<string, string> _dic)
        {
            JSONObject rootJson = new JSONObject();

            rootJson.AddField("version", GeneratorVersion);
            rootJson.AddField("updatedTime", GetNowTimeString());

            JSONObject hashesJson = new JSONObject();
            foreach (var p in _dic)
            {
                JSONObject hashJson = new JSONObject();
                hashJson.AddField("key", p.Key);
                hashJson.AddField("value", p.Value);
                hashesJson.Add(hashJson);
            }

            rootJson.AddField("hashes", hashesJson);
            return rootJson;
        }

        public static Dictionary<string, string> JSONToChecksumDic(JSONObject _json)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            JSONObject hashesJson = _json["hashes"];
            if (hashesJson != null)
            {
                foreach (var hashJson in hashesJson.list)
                {
                    dic.Add(hashJson["key"]?.str ?? "", hashJson["value"]?.str ?? "");
                }
            }
            else
            {
                Debug.LogError("ChecksumGenerator::JSONToChecksumDic failed");
            }

            return dic;
        }

        public static string GetNowTimeString()
        {
            lastDateTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return lastDateTimeStr;
        }
    }
}


