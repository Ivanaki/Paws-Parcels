using System.Collections;
using System.Collections.Generic;
using Paws_Parcels.Scripts.CarMovement.Ramming;
using UnityEngine;
using UnityEngine.Events;

public class AnaimationEvent : MonoBehaviour, IRammable
{
    public UnityEvent playAnim;
    public Animator animator;
    public string name;

    public bool donaut = false;
    public Rigidbody donautr;
    void Start()
    {
        //animator = GetComponent<Animator>();
        playAnim.AddListener(PlayAnim);
        //playAnim.Invoke();
    }

    public void PlayAnim()
    {
        animator.Play(name);
        if (donaut)
        {
            StartCoroutine(PushWithDelay());
            
            print('h');
        }
            
    }
    
    IEnumerator PushWithDelay()
    {
        yield return new WaitForSeconds(1f); // ждём 2 секунды
        donautr.AddForce(Vector3.forward*10,  ForceMode.Impulse);
    }

    public float MinImpactSpeed => 1f;
    
    
    bool rummed = false;
    public void OnRammed(float impactSpeed, Vector3 impactDirection)
    {
        if (rummed == false)
        {
            playAnim.Invoke();
            rummed = true;
        }
    }
}
