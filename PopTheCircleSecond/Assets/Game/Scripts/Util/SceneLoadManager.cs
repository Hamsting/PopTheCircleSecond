using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 씬 로드를 담당하는 매니저
    /// </summary>
    public class SceneLoadManager : Singleton<SceneLoadManager>
    {
        /// <summary>
        /// 로딩씬에서 로드할 씬
        /// </summary>
        [InspectorReadOnly]
        private string loadingSceneName;



        /// <summary>
        /// 씬을 로드 한다.
        /// </summary>
        public void Load(string sceneName, bool runLoadProgressScene, int loadMode = 0)
        {
            if (runLoadProgressScene)
            {
                loadingSceneName = sceneName;
                StartCoroutine(LoadScene("LoadingScene", false, (LoadSceneMode)loadMode));
            }
            else
                StartCoroutine(LoadScene(sceneName, runLoadProgressScene, (LoadSceneMode)loadMode));
        }

        /// <summary>
        /// 씬을 로드하는 코루틴
        /// </summary>
        private IEnumerator LoadScene(string sceneName, bool runLoadProgressScene, LoadSceneMode loadMode)
        {
            if (runLoadProgressScene)
            {
                AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, loadMode);
                async.allowSceneActivation = false;

                while (!async.isDone)
                {
                    if (async.progress >= 0.9f)
                    {
                        loadingSceneName = null;
                        async.allowSceneActivation = true;
                    }
                    yield return null;
                }
            }
            else
            {
                SceneManager.LoadScene(sceneName, loadMode);
            }
        }

    }
}