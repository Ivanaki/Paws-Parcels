using R3;

namespace MotorHome.Scripts
{
    public class ObjectPlaceInteractable : InteractableObject
    {
        public Observable<Unit> OnInteract => _onInteract;
        private Subject<Unit> _onInteract = new();
        
        
        protected override void Awake()
        {
            base.Awake();
            
        }
        
        public override void Interact()
        {
            _onInteract.OnNext(Unit.Default);
        }
    }
}