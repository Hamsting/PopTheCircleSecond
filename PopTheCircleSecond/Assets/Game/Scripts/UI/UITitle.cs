using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITitle : MonoBehaviour {

    public void OnTouchStart() {
        //TODO::SceneLoadManager로 연동
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

}
