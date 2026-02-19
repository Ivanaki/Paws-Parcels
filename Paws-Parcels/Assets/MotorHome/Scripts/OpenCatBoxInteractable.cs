using UnityEngine;
using UnityEngine.Playables;
using Zenject;

namespace MotorHome.Scripts
{
    public class OpenCatBoxInteractable : InteractableObject
    {
        [SerializeField] private PlayableDirector _cutScene;
        
        [Inject] private IStopMove _stopMove;

        private bool can = true;
        
        public override void Interact()
        {
            if (can)
            {
                //EventManagerTest.CloseShowable.Invoke();
                _stopMove.StopMove();
                _cutScene.Play();
                can = false;
                gameObject.SetActive(false);
            }
        }
    }
}