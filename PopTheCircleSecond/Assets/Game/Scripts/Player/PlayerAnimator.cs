using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopTheCircle.Game;

public class PlayerAnimator : MonoBehaviour {

    Animator ani;

    public int kindCount;

    private void Awake()
    {
        ani = GetComponentInChildren<Animator>();

    }

    private void Start()
    {
        JudgeManager.Instance.judgeEvent.AddListener(Play);
    }

    void Play() {
        ani.SetInteger("Kind", Random.Range(1, kindCount + 1));
        ani.SetTrigger("Play");
    }


}
