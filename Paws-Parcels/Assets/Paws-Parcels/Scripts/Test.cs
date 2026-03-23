using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace DefaultNamespace
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private string _s;
        
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                GetComponent<DialogueRunner>().StartDialogue(_s);
            }
        }
    }
}