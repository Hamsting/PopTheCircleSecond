using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 사용자의 세팅 값을 저장한다.
    /// </summary>
    public static class UserSettings
    {
        // In-Game Settings
        public static float UserGameSpeed = 5.0f;
        public static float UserNoteScale = 2.5f;

        public static int UserMusicSyncDelayMs = -42;

        public static ClearGaugeType ClearGaugeType = ClearGaugeType.Normal;
        public static NoteAppearType NoteAppearType = NoteAppearType.Normal;



        // Global Settings
        public static string NoteDataRootPath = "";






        public static void LoadUserSettings()
        {
            UserGameSpeed = PlayerPrefs.GetFloat("UserGameSpeed", 5.0f);
            UserNoteScale = PlayerPrefs.GetFloat("UserNoteScale", 2.5f);
            UserMusicSyncDelayMs = PlayerPrefs.GetInt("UserMusicSyncDelayMs", -42);

            NoteDataRootPath = PlayerPrefs.GetString("NoteDataRootPath", "");

            ClearGaugeType = (ClearGaugeType)PlayerPrefs.GetInt("ClearGaugeType", (int)ClearGaugeType.Normal);
            NoteAppearType = (NoteAppearType)PlayerPrefs.GetInt("NoteAppearType", (int)NoteAppearType.Normal);
        }

        public static void SaveUserSettings()
        {
            PlayerPrefs.SetFloat("UserGameSpeed", UserGameSpeed);
            PlayerPrefs.SetFloat("UserNoteScale", UserNoteScale);
            PlayerPrefs.SetInt("UserMusicSyncDelayMs", UserMusicSyncDelayMs);

            PlayerPrefs.SetString("NoteDataRootPath", NoteDataRootPath);

            PlayerPrefs.SetInt("ClearGaugeType", (int)ClearGaugeType);
            PlayerPrefs.SetInt("NoteAppearType", (int)NoteAppearType);
            PlayerPrefs.Save();
        }
    }
}