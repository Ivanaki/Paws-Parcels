using System.Collections.Generic;
using Paws_Parcels.Scripts.Player;
using UnityEngine;
using Yarn.Unity;

namespace Paws_Parcels.NPCDialogs
{
    public class NPC : InteractableObject
    {
        public override string DestinationId => _npcNameCode;
        public override string UiShowTypeId => _uiShowTypeId;
        public override string UIShowTextCode => _npcNameCode;
        public override bool DontShowWithItemInHands => false;

        public override IReadOnlyList<string> ActivateLocations => _activeLocations;

        [Header("Interactable Settings")]
        [SerializeField] private string _uiShowTypeId = "interact";
        [SerializeField] private List<string> _activeLocations = new() { "None" };
        
        [Header("NPC Settings")]
        [SerializeField] private string _npcNameCode;
        [SerializeField] private DialogueReference _dialogue = new();
        [SerializeField] private Transform _headTransform;
        [SerializeField] private float _headRotationSpeed = 5f;
        [SerializeField] private float _maxHeadTurnAngle = 65f;
        [SerializeField] private Vector3 _headLookOffset = new(0f, 1.6f, 0f);

        private const string PlayerTag = "Player";

        private bool _isPlayerNear;
        private Transform _playerTransform;
        private Quaternion _initialHeadLocalRotation;

        private void Awake()
        {
            if (_headTransform == null)
                _headTransform = transform;

            _initialHeadLocalRotation = _headTransform.localRotation;
        }

        public override void Interact()
        {
            
        }

        public override void Highlight()
        {
            base.Highlight();
            _isPlayerNear = true;
        }

        public override void Unhighlight()
        {
            base.Unhighlight();
            _isPlayerNear = false;
        }

        private void Update()
        {
            if (_isPlayerNear == false)
            {
                ResetHeadRotation();
                return;
            }

            if (TryGetPlayerTransform() == false)
            {
                ResetHeadRotation();
                return;
            }

            RotateHeadToPlayer();
        }

        private bool TryGetPlayerTransform()
        {
            if (_playerTransform != null)
                return true;

            var playerObject = GameObject.FindGameObjectWithTag(PlayerTag);
            if (playerObject == null)
                return false;

            _playerTransform = playerObject.transform;
            return true;
        }

        private void RotateHeadToPlayer()
        {
            var headPosition = _headTransform.position;
            var lookDirection = _playerTransform.position + _headLookOffset - headPosition;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude < Mathf.Epsilon)
                return;

            var targetWorldRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
            var targetLocalRotation = Quaternion.Inverse(transform.rotation) * targetWorldRotation;
            var targetLocalEuler = targetLocalRotation.eulerAngles;
            targetLocalEuler.x = _initialHeadLocalRotation.eulerAngles.x;
            targetLocalEuler.z = _initialHeadLocalRotation.eulerAngles.z;
            targetLocalEuler.y = Mathf.Clamp(NormalizeAngle(targetLocalEuler.y), -_maxHeadTurnAngle, _maxHeadTurnAngle);

            var clampedLocalRotation = Quaternion.Euler(targetLocalEuler);
            _headTransform.localRotation = Quaternion.Slerp(
                _headTransform.localRotation,
                clampedLocalRotation,
                _headRotationSpeed * Time.deltaTime);
        }

        private void ResetHeadRotation()
        {
            _headTransform.localRotation = Quaternion.Slerp(
                _headTransform.localRotation,
                _initialHeadLocalRotation,
                _headRotationSpeed * Time.deltaTime);
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 180f)
                angle -= 360f;

            while (angle < -180f)
                angle += 360f;

            return angle;
        }
    }
}
