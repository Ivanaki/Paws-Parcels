using System;
using R3;
using UnityEngine;

public class MarksObject : MonoBehaviour
{
    public BehaviorSubject<bool> IsPlayerInCollider = new(false);
    public float ColliderRadius => _sphereCollider.radius;
        
    private SphereCollider _sphereCollider;
        
    public GameObject ShowableObject;

    public int needCondition = 0;
    public bool conditionPassed = false;
    public bool inTrigger = false;
    public bool storyMarkNow = false;
    public bool inZone = false;

    public Transform carPosition;
    public float showDistance = 100f;
    private void Awake()
    {
        if(!storyMarkNow)
            ShowableObject?.SetActive(false);
    }

    private void Start()
    {
        if (storyMarkNow)
        {
            ShowableObject.SetActive(true);
        }
    }

    public void StoryMark(bool on)
    {
        if (on)
        {
            storyMarkNow = on;
            if (inZone)
            {
                
            
                ShowableObject.SetActive(on);
                this.enabled = on;
            }
        }
        else
        {
            storyMarkNow = on;
            
            ShowableObject.SetActive(on);
            this.enabled = on;
        }

        print($"{on}, {ShowableObject.activeSelf}");
        // if (on)
        // {
        //     ShowableObject.SetActive(true);
        // }
            
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, carPosition.position);
        print(distance);
        if (distance < showDistance)
        {
            
            inZone = true;
            if (storyMarkNow)
            {
                StoryMark(true);
            }

        }
        else
        {
            inZone = false;
        }
    }
}
