using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class MessageGameUI : MonoBehaviour
    {
        
        [SerializeField] private TMP_Text _text;

        private bool _isShow = false;

        
        private void Awake()
        {
            _text.gameObject.SetActive(false);
        }
        
        public void Send(string msg, float duration = 1.5f)
        {
            if(_isShow) return;
            
            _text.text = msg;
            ShowPanel();
            Invoke(nameof(HidePanel), duration);
        }

        public void HideMessageWithText(string message)
        {
            if (message == _text.text)
            {
                HidePanel();
            }
        }
        

        public void HidePanel()
        {
            _text.gameObject.SetActive(false);
            
            _isShow = false;
        }

        public void ShowPanel()
        {
            _text.gameObject.SetActive(true);
            
            _isShow = true;
        }
    }
}