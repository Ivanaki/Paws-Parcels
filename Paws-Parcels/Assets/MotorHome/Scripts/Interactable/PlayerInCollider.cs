using System;
using R3;
using UnityEngine;
using UnityEngine.Events;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class PlayerInCollider : MonoBehaviour
    {
        //public GameObject Player { private set; get; } = null;
        
        public BehaviorSubject<bool> IsPlayerInCollider = new(false);
        public float ColliderRadius => _sphereCollider.radius;
        
        private SphereCollider _sphereCollider;
        
        public GameObject ShowableObject;

        public int needCondition = 0;
        public bool conditionPassed = false;
        public bool inTrigger = false;
        public bool storyMarkNow = false;
        private void Awake()
        {
            
            GetComponent<Collider>().isTrigger = true;
            //EventManagerTest.CloseShowable.AddListener(CloseShowableNo);
            //EventManagerTest.OpenShowableMark.AddListener(OpenMark);
            //EventManagerTest.ConditionPassed.AddListener(CondPassed);
            //EventManagerTest.StoryMark.AddListener(StoryMark);
            //EventManagerTest.ConditionPassed.Invoke();
            if(!storyMarkNow)
                ShowableObject?.SetActive(false);
            print("offNow");
            if (TryGetComponent(out SphereCollider sphereCollider))
                if (sphereCollider.center != Vector3.zero)
                    Debug.LogError("Collider center mismatch");
            
            if (TryGetComponent(out BoxCollider boxCollider))
                if (boxCollider.center != Vector3.zero)
                    Debug.LogError("Collider center mismatch");
            
            TryGetComponent(out _sphereCollider);
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
            storyMarkNow = on;
            
            ShowableObject.SetActive(on);
            this.enabled = on;
            print($"{on}, {ShowableObject.activeSelf}");
            // if (on)
            // {
            //     ShowableObject.SetActive(true);
            // }
            
        }

        
        private void OpenMark()
        {
            throw new NotImplementedException();
        }

        private void CondPassed(int type)
        {
            print($"Passed {type}, {needCondition}, {inTrigger}");
            if (type == 0 && needCondition != 3)
            {
                conditionPassed = false;
            }
            else if (type == needCondition)
            {
                conditionPassed = true;
                if (inTrigger)
                {
                    ShowableObject.SetActive(true);
                }
            }
            else if (needCondition == 3)
            {
                if (type > 0)
                {
                    conditionPassed = false;
                    ShowableObject.SetActive(false);
                }
                else
                {
                    conditionPassed = true;
                    if (inTrigger)
                    {
                        ShowableObject.SetActive(true);
                    }
                }
            }
            
            if (needCondition == 31)
            {
                if (type > 1)
                {
                    conditionPassed = false;
                    ShowableObject.SetActive(false);
                }
                else
                {
                    conditionPassed = true;
                    if (inTrigger)
                    {
                        ShowableObject.SetActive(true);
                    }
                }
                    
            }
                

        }
        

        private void CloseShowableNo()
        {
            if(ShowableObject != null && !storyMarkNow)
                ShowableObject.SetActive(false);
            print("noShow");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                inTrigger = true;
                IsPlayerInCollider.OnNext(true);
                if (ShowableObject != null)
                {
                    if (needCondition > 0) {
                        if (conditionPassed)
                        {
                            ShowableObject.SetActive(true);
                        }
                        
                    }
                    else
                    {
                        ShowableObject.SetActive(true);
                    }
                }
                
                //Player = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                inTrigger = false;
                IsPlayerInCollider.OnNext(false);
                if (ShowableObject != null)
                {
                    if (!storyMarkNow)
                    {
                        ShowableObject.SetActive(false);
                    }
                    
                }
                    
            }
        }
    }
}