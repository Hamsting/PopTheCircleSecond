using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PopTheCircle.NoteEditor
{
    public class BlankScene : MonoBehaviour
    {
        private void Awake()
        {
            GameObject managers = GameObject.Find("Managers");
            if (managers != null)
                Destroy(managers);

            SceneManager.LoadScene("Maker");
        }
    }
}