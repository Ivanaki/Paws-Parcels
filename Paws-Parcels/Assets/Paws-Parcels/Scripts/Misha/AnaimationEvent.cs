using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnaimationEvent : MonoBehaviour
{
    public UnityEvent playAnim;
    public Animator animator;
    public string name;

    public bool donaut = false;
    public Rigidbody donautr;
    void Start()
    {
        animator = GetComponent<Animator>();
        playAnim.AddListener(PlayAnim);
        playAnim.Invoke();
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

}
