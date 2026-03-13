namespace Services
{
    public class InteractablesLocationsService
    {
        public string CurrentLocation { get; private set; } = "None";
        
        private readonly EditorDataService _editorDataService;

        public InteractablesLocationsService(EditorDataService editorDataService)
        {
            _editorDataService = editorDataService;
        }

        public void SwitchLocation(string location)
        {
            CurrentLocation = location;
        }
        
        public string GetCurrentLocationCode()
        {
            return "CurrentLocationCode";
        }
    }
}