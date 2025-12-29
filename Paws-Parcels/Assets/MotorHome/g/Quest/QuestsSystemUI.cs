using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MotorHome.Scripts
{
    public class QuestsSystemUI : MonoBehaviour
    {
        //[SerializeField] private GameObject _panel;
        /*public Button _button;

        [SerializeField] private GameObject _gameObject;
        
        
        public Observable<Unit> ReturnButtonPressed => _returnButtonPressed;
        
        private readonly Subject<Unit> _returnButtonPressed = new();
        
        private IDisposable _disposable;
        
        public void UpdateQuests(List<QuestData> questDatas)
        {
            print("Updating quests");
            _gameObject.SetActive(false);
        }
    
        private void OnEnable()
        {
            _disposable = _button.OnClickAsObservable().Subscribe(_ =>
            {
                print("Button clicked");
                _returnButtonPressed.OnNext(Unit.Default);
            });
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
        
        private void Start()
        {
            //Close();
            
            
        }
        
        public void Open()
        {
            //needUpdate = true;
            //setActive = true;
            //print("Open");
        }

        public void Close()
        {
            //needUpdate = true;
            //setActive = false;
            //print("Close");
        }*/
        
        /*private bool needUpdate = false;
        private bool setActive = false;
        private void Update()
        {
            if (needUpdate)
            {
                needUpdate = false;
                StartCoroutine(G());
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                setActive = !setActive;
                StartCoroutine(G());
            }
        }

        private IEnumerator G()
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            print("G: " + setActive);
            _panel.SetActive(setActive);
        }*/
    }
}