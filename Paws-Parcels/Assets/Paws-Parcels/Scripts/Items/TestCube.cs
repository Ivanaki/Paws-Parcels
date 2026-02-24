using System;
using UnityEngine;

namespace Paws_Parcels.Scripts
{
    public class TestCube : MonoBehaviour
    {
        private void Awake()
        {
            print("Awake");
        }

        private void OnEnable()
        {
            print("OnEnable");
        }

        private void Start()
        {
            print("Start");
        }

        public void Construct()
        {
            print("Construct");
        }
    }
}