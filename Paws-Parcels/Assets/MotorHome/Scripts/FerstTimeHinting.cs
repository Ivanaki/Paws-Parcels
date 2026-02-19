using System;
using UnityEngine;
using UnityEngine.Events;

public class FerstTimeHinting : MonoBehaviour
{

    public bool fertsTime = true;
    [SerializeField] private UnityEvent FertsTimeEvent;
    private void OnTriggerEnter(Collider other)
    {
        if (fertsTime && other.gameObject.name == "MyCar")
        {
            FertsTimeEvent.Invoke();
            fertsTime = false;
            print("MYCAR");
        }
    }
}
