using R3;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(Image),  typeof(Button))]
    public class GameQuestsPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text _text;
        
        [SerializeField] private Color _enterColor = new Color(1, 1, 1, 0.03529412f);
        [SerializeField] private Color _exitColor = new Color(0, 0, 0, 0);
        [SerializeField] private Color _selectedColor = new Color(0, 0, 0, 0);

        private Image _image;
        private Button _button;
        
        private bool _isSelected = false;
        
        private readonly Subject<Unit> _selected = new();
        public Observable<Unit> Selected => _selected;
        
        public QuestEnum Quest { private set; get; }
        public void Initialize(string text, QuestEnum quest)
        {
            _text.text = text;
            Quest = quest;
        }
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            _image.color = _exitColor;
            
            _button.onClick.AddListener(() => _selected.OnNext(Unit.Default));
        }

        public void OnPointerEnter(PointerEventData eventData = null)
        {
            if (_image && !_isSelected) _image.color = _enterColor;
        }

        public void OnPointerExit(PointerEventData eventData = null)
        {
            if (_image&& !_isSelected) _image.color = _exitColor;
        }

        private void OnDisable()
        {
            OnPointerExit();
        }

        public void Select()
        {
            _isSelected = true;
            if (_image) _image.color = _selectedColor;
        }

        public void Deselect()
        {
            _isSelected = false;
            if (_image) _image.color = _exitColor;
        }
    }
}