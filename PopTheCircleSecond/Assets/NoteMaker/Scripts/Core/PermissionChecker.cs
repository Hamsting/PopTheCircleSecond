using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace PopTheCircle.NoteEditor
{
    public class PermissionChecker : Singleton<PermissionChecker>
    {
        public bool AllChecked
        {
            get
            {
                return allChecked;
            }
        }
        public bool IsExWriteAvailable
        {
            get
            {
                return isExStorageAvailable;
            }
        }

        private bool allChecked = false;
        private bool isExStorageAvailable = false;
        private bool isCoroutineRunning = false;



        protected override void Awake()
        {
            base.Awake();

            if (!allChecked && !isCoroutineRunning)
                CheckAllPermission();
        }
        
        public void CheckAllPermission()
        {
            StartCoroutine(PermissionAllCheckCoroutine());
        }

        private IEnumerator PermissionAllCheckCoroutine()
        {
            isCoroutineRunning = true;
            yield return new WaitForEndOfFrame();
            
            yield return ExWriteCheckCoroutine();

            allChecked = true;
            isCoroutineRunning = false;
        }

        public void CheckExWritePermission()
        {
            StartCoroutine(ExWriteCheckCoroutine());
        }

        private IEnumerator ExWriteCheckCoroutine()
        {
            isExStorageAvailable = 
                Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) &&
                Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
            if (!isExStorageAvailable)
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                Permission.RequestUserPermission(Permission.ExternalStorageRead);

                yield return new WaitForSeconds(0.2f);
                yield return new WaitUntil(() => (Application.isFocused == true));

                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) ||
                    !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
                {
                    OpenAppSettingsWindow();
                }
                isExStorageAvailable = 
                    Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) &&
                    Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
            }
        }

        private void OpenAppSettingsWindow()
        {
            try
            {
#if UNITY_ANDROID
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        string packageName = currentActivityObject.Call<string>("getPackageName");

                        using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                        {
                            using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                            {
                                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                                {
                                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                                    currentActivityObject.Call("startActivity", intentObject);
                                }
                            }
                        }
                    }
                }
#else
                Debug.Log("PermissionChecker::OpenAppSettingsWindow works only in android");
#endif
            }
            catch (System.Exception _e)
            {
                Debug.LogException(_e);
            }
        }
    }
}
