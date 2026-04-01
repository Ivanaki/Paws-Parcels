using System;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using Dialogue;
using Extentions;
using NPCDialogs;
using Paws_Parcels.Scripts;
using Paws_Parcels.Scripts.Player;
using R3;
using Services;
using Unity.Cinemachine;
using UnityEngine;
using Yarn.Unity;
using Zenject;

namespace Paws_Parcels.NPCDialogs
{
    public interface INPC
    {
        void SetDialog(DialogueReference dialogue, bool isMain);
        string NpcName { get; }
    }

    public class NPC : InteractableObject, INPC
    {
        public override string DestinationId => _npcNameCode;
        public override string UiShowTypeId => _uiShowTypeId;
        public override string MapShowTypeId => _mapShowTypeId;
        public override ReadOnlyReactiveProperty<string> UIShowTextCode => _uiShowTextCodeRP;
        public override bool DontShowWithItemInHands => false;

        public override IReadOnlyList<string> ActivateLocations => _activeLocations;

        protected override float SelectedActivationDistance => _selectedActivationDistance;

        public string NpcName => _npcNameCode;

        [Header("Interactable Settings")]
        [SerializeField] private string _uiShowTypeId = "interact";
        [SerializeField] private string _mapShowTypeId = "quest";
        [SerializeField] private List<string> _activeLocations = new(){"None"};
        [SerializeField] private float _selectedActivationDistance = 20f;
        
        [Header("NPC Settings")]
        [SerializeField] private string _npcNameCode;
        [SerializeField] DialogueReference _startNpcDialog = new();
        //[SerializeField] private NpcHeadLookAt _npcHeadLookAt;
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private float _rotationDuration = 1f;
        [SerializeField] private Transform _npcTransform;
        [SerializeField] private GameObject _mark;
        
        private ReactiveProperty<string> _uiShowTextCodeRP = new("Bug");

        [Inject] private DialogueService _dialogueService;
        [Inject] private DialogueInQuestsService _dialogueInQuestsService;
        [Inject] private PlayerService _playerService;

        private float _baseYRot;
        private Tween _rotateTween;
        
        private DialogueReference _currentDialogue = null;
        //private bool _isCurrentNPCDialogRunning = false;

        private void Awake()
        {
            _mark.SetActive(false);
            
            _uiShowTextCodeRP.Value = _npcNameCode;
            
            _baseYRot = _npcTransform.eulerAngles.y;
            _cinemachineCamera.Priority.Enabled = true;
            
            _currentDialogue ??= _startNpcDialog;
        }
        
        public override void Interact()
        {
            if (_currentDialogue != null)
            {
                _dialogueService.TryStartDialogue(_currentDialogue, _npcNameCode, _npcTransform, _rotationDuration);
            }
            else
            {
                Debug.LogWarning("Dont have any dialogue on NPC: " + _npcNameCode);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _dialogueInQuestsService.RegisterNpc(this);
            
            _disposables.Add(_dialogueService.IsDialogRunning.Subscribe(isRunning =>
            {
                if (isRunning && _dialogueService.CurrentNpcDialog == _npcNameCode)
                {
                    _rotateTween?.Kill();
                    var yRot = Utils.VectorToGlobalYRotation(_playerService.PlayerTransform.position
                                                             - _npcTransform.position);
                    _rotateTween = _npcTransform.DORotate(new Vector3(0, yRot, 0), _rotationDuration);
                    _cinemachineCamera.Priority.Value = 100;
                }
                else
                {
                    _rotateTween?.Kill();
                    _rotateTween = _npcTransform.DORotate(new Vector3(0, _baseYRot, 0), _rotationDuration);
                    _cinemachineCamera.Priority.Value = -100;
                }
            }));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _dialogueInQuestsService.UnregisterNpc(this);
        }

        public void SetDialog(DialogueReference dialogue, bool isMain)
        {
            _currentDialogue = dialogue;
            _mark.SetActive(isMain);
        }
    }
}