using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private Vector3 _force;
        [SerializeField, Range(0.1f, 3f)] private float _time;
        
        
        
        
        private void Start()
        {
            StartCoroutine(F());

        }

        private IEnumerator F()
        {
            while (true)
            {
                yield return new WaitForSeconds(_time);
                Inpuls();
            }
        }
        
        private void Inpuls()
        {
            GetComponent<Rigidbody>().AddForce(_force, ForceMode.VelocityChange);
        }
    }
}