using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvaAnimators : MonoBehaviour
{
    public Animator animatorE;
    public Animator animatorP;
    public string evento = "Idle";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        animatorE.Play(evento);
        animatorP.Play(evento);
    }
}
