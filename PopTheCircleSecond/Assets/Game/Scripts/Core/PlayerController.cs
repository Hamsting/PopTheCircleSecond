using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    /*
        Animation 구조
        
        State
            >> Ground (total = 2)
                >> Idle
                >> Run
            >> Sky (total = 2)
                >> Jump
                >> Fly
        
        Attack (total = 2)
            >> Single
            >> Multiple

        NoteCategory (total = 3)
            >> Normal Hit
            >> Long Hit
            >> Infinity Hit            
     */

    private Animator ani;

    private void Awake()
    {
        ani = GetComponent<Animator>();

    }


    public void Animate() {


    }


}
