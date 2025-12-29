using System.Collections;
using TMPro;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class MessageGameUI : MonoBehaviour
    {
        
        [SerializeField] private TMP_Text _text;

        private void Awake()
        {
            _text.gameObject.SetActive(false);
        }
        
        public void Send(string msg, float duration = 1.5f)
        {
            _text.text = msg;
            StartCoroutine(ShowPanel(duration));
        }

        private IEnumerator ShowPanel(float duration = 1.5f)
        {
            _text.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            _text.gameObject.SetActive(false);
        }
    }
}