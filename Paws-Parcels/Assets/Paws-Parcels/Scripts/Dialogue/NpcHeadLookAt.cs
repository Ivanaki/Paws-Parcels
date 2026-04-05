using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Animator))]
    public class NpcHeadLookAt : MonoBehaviour
    {
        [Header("Target")]
        private Transform player;
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);

        [Header("Distance")]
        [SerializeField] private float detectionRadius = 6f;
        [SerializeField] private float loseRadius = 7.5f;

        [Header("Vision")]
        [SerializeField, Range(0f, 180f)] private float maxViewAngle = 75f;
        [SerializeField] private Transform eyesOrigin;
        [SerializeField] private LayerMask obstacleMask;

        [Header("Timing")]
        [SerializeField] private float enterLookDelay = 0.08f;
        [SerializeField] private float exitLookDelay = 0.15f;

        [Header("Performance")]
        [Tooltip("Как часто выполнять дорогие проверки (дистанция/угол/LOS).")]
        [SerializeField] private float senseInterval = 0.08f;

        [Tooltip("Если true, NPC распределяют expensive-checks по кадрам.")]
        [SerializeField] private bool staggerUpdates = true;

        [Header("Smoothing")]
        [SerializeField] private float weightSmoothTime = 0.18f;
        [SerializeField] private float lookMoveSpeed = 4.5f;
        [SerializeField] private float maxTurnSpeed = 180f;

        [Header("Head Bone")]
        [SerializeField] private Transform headBone;
        [SerializeField, Range(0f, 90f)] private float maxHeadAngle = 70f;

        [Header("Behavior")]
        [SerializeField] private bool requireLineOfSight = true;
        [SerializeField] private bool requireNear = true;
        [SerializeField] private bool debug = false;

        private Animator _animator;

        private float _currentWeight;
        private float _targetWeight;
        private float _weightVelocity;

        private Vector3 _currentLookDirection;
        private Vector3 _smoothedLookPosition;

        private bool _isPlayerNear;
        private bool _canLookCached;
        private float _enterTimer;
        private float _exitTimer;

        private float _detectionRadiusSqr;
        private float _loseRadiusSqr;
        private float _cosViewAngle;

        private float _nextSenseTime;
        private int _frameOffset;

        private const float MinDirectionSqr = 0.0001f;
        private const float DefaultIdleDistance = 2f;

        
        
        private void Awake()
        {
            //var t = GameObject.FindGameObjectsWithTag("Player");
            player = GameObject.FindGameObjectWithTag("Player").transform;
            
            _animator = GetComponent<Animator>();

            if (eyesOrigin == null)
                eyesOrigin = transform;

            RecalculateCachedValues();

            Vector3 initialLookPos = transform.position + transform.forward * DefaultIdleDistance + Vector3.up * 1.6f;
            _smoothedLookPosition = initialLookPos;
            _currentLookDirection = (initialLookPos - eyesOrigin.position).normalized;

            _frameOffset = Random.Range(0, 8);
            _nextSenseTime = Time.time + (staggerUpdates ? _frameOffset * 0.01f : 0f);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(player == null)
                player = GameObject.FindGameObjectWithTag("Player").transform;
            
            detectionRadius = Mathf.Max(0f, detectionRadius);
            loseRadius = Mathf.Max(detectionRadius, loseRadius);
            senseInterval = Mathf.Max(0.02f, senseInterval);
            weightSmoothTime = Mathf.Max(0.01f, weightSmoothTime);
            lookMoveSpeed = Mathf.Max(0.01f, lookMoveSpeed);
            maxTurnSpeed = Mathf.Max(1f, maxTurnSpeed);

            RecalculateCachedValues();
        }
#endif

        private void RecalculateCachedValues()
        {
            _detectionRadiusSqr = detectionRadius * detectionRadius;
            _loseRadiusSqr = loseRadius * loseRadius;
            _cosViewAngle = Mathf.Cos(maxViewAngle * Mathf.Deg2Rad);
        }

        private void Update()
        {
            if (player == null)
            {
                _targetWeight = 0f;
                UpdateWeight(Time.deltaTime);
                UpdateLookPositionIdle(Time.deltaTime);
                return;
            }

            float dt = Time.deltaTime;

            if (Time.time >= _nextSenseTime)
            {
                RunSense();
                _nextSenseTime = Time.time + senseInterval;
            }

            if (_canLookCached)
            {
                _exitTimer = 0f;
                _enterTimer += dt;

                if (_enterTimer >= enterLookDelay)
                    _targetWeight = 1f;
            }
            else
            {
                _enterTimer = 0f;
                _exitTimer += dt;

                if (_exitTimer >= exitLookDelay)
                    _targetWeight = 0f;
            }

            UpdateWeight(dt);
            UpdateLookPosition(_canLookCached, dt);
        }

        private void RunSense()
        {
            Vector3 npcPos = transform.position;
            Vector3 playerPos = player.position;
            Vector3 toPlayer = playerPos - npcPos;
            float sqrDistance = toPlayer.sqrMagnitude;

            if (_isPlayerNear)
                _isPlayerNear = sqrDistance <= _loseRadiusSqr;
            else
                _isPlayerNear = sqrDistance <= _detectionRadiusSqr;

            if (requireNear && !_isPlayerNear)
            {
                _canLookCached = false;
                return;
            }

            Vector3 origin = eyesOrigin.position;
            Vector3 target = playerPos + targetOffset;

            Origin = origin;
            Target = target;
            
            Vector3 toTarget = target - origin;
            float toTargetSqr = toTarget.sqrMagnitude;

            if (toTargetSqr < MinDirectionSqr)
            {
                _canLookCached = false;
                return;
            }

            Vector3 forward = transform.forward;
            forward.y = 0f;

            Vector3 flatToTarget = toTarget;
            flatToTarget.y = 0f;

            float flatForwardSqr = forward.sqrMagnitude;
            float flatToTargetSqr = flatToTarget.sqrMagnitude;

            if (flatForwardSqr < MinDirectionSqr || flatToTargetSqr < MinDirectionSqr)
            {
                _canLookCached = false;
                return;
            }

            float invMag = 1f / Mathf.Sqrt(flatForwardSqr * flatToTargetSqr);
            float dot = Vector3.Dot(forward, flatToTarget) * invMag;

            if (dot < _cosViewAngle)
            {
                _canLookCached = false;
                return;
            }

            if (requireLineOfSight)
            {
                float distance = Mathf.Sqrt(toTargetSqr);
                Vector3 dir = toTarget / distance;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, obstacleMask, QueryTriggerInteraction.Ignore))
                {
                    Transform hitTr = hit.transform;
                    if (hitTr != player && !hitTr.IsChildOf(player))
                    {
                        _canLookCached = false;
                        return;
                    }
                }
            }

            _canLookCached = true;
        }

        private void UpdateWeight(float dt)
        {
            _currentWeight = Mathf.SmoothDamp(
                _currentWeight,
                _targetWeight,
                ref _weightVelocity,
                weightSmoothTime,
                Mathf.Infinity,
                dt
            );

            if (_currentWeight < 0.001f)
                _currentWeight = 0f;
            else if (_currentWeight > 0.999f)
                _currentWeight = 1f;
        }

        private void UpdateLookPosition(bool canLook, float dt)
        {
            Vector3 origin = eyesOrigin.position;
            Vector3 desiredWorldPos;

            if (canLook)
            {
                desiredWorldPos = player.position + targetOffset;
            }
            else
            {
                desiredWorldPos = origin + transform.forward * DefaultIdleDistance + Vector3.up * 0.1f;
            }

            Vector3 desiredDirection = desiredWorldPos - origin;
            float sqrMag = desiredDirection.sqrMagnitude;

            if (sqrMag < MinDirectionSqr)
                desiredDirection = transform.forward;
            else
                desiredDirection /= Mathf.Sqrt(sqrMag);

            _currentLookDirection = Vector3.RotateTowards(
                _currentLookDirection,
                desiredDirection,
                maxTurnSpeed * Mathf.Deg2Rad * dt,
                lookMoveSpeed * dt
            );

            float distanceToTarget = canLook
                ? Mathf.Max(DefaultIdleDistance, Vector3.Distance(origin, desiredWorldPos))
                : DefaultIdleDistance;

            _smoothedLookPosition = origin + _currentLookDirection * distanceToTarget;
        }

        private void UpdateLookPositionIdle(float dt)
        {
            Vector3 origin = eyesOrigin.position;
            Vector3 desiredDirection = transform.forward;
            float sqrMag = desiredDirection.sqrMagnitude;

            if (sqrMag > MinDirectionSqr)
                desiredDirection /= Mathf.Sqrt(sqrMag);
            else
                desiredDirection = Vector3.forward;

            _currentLookDirection = Vector3.RotateTowards(
                _currentLookDirection,
                desiredDirection,
                maxTurnSpeed * Mathf.Deg2Rad * dt,
                lookMoveSpeed * dt
            );

            _smoothedLookPosition = origin + _currentLookDirection * DefaultIdleDistance;
        }

        private void LateUpdate()
        {
            if (headBone == null || _currentWeight < 0.001f)
                return;

            Vector3 lookDir = _smoothedLookPosition - headBone.position;
            if (lookDir.sqrMagnitude < MinDirectionSqr)
                return;

            Quaternion targetRot = Quaternion.LookRotation(lookDir, transform.up);
            Quaternion desiredRot = Quaternion.Slerp(headBone.rotation, targetRot, _currentWeight);

            float angle = Quaternion.Angle(headBone.rotation, desiredRot);
            if (angle > maxHeadAngle)
                desiredRot = Quaternion.RotateTowards(headBone.rotation, desiredRot, maxHeadAngle);

            headBone.rotation = desiredRot;
        }
        
        private void OnDrawGizmosSelected()
        {
            
            
            if (!debug)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, loseRadius);

            Vector3 origin = eyesOrigin != null ? eyesOrigin.position : transform.position + Vector3.up * 1.6f;

            Quaternion leftRot = Quaternion.AngleAxis(-maxViewAngle, Vector3.up);
            Quaternion rightRot = Quaternion.AngleAxis(maxViewAngle, Vector3.up);

            Vector3 leftDir = leftRot * transform.forward;
            Vector3 rightDir = rightRot * transform.forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(origin, leftDir * detectionRadius);
            Gizmos.DrawRay(origin, rightDir * detectionRadius);

            if (player != null)
            {
                Vector3 lookPoint = player.position + targetOffset;
                Gizmos.color = _targetWeight > 0f ? Color.green : Color.red;
                Gizmos.DrawLine(origin, lookPoint);
                Gizmos.DrawSphere(lookPoint, 0.08f);
            }

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_smoothedLookPosition, 0.06f);
        }

        public Vector3 Origin;
        public Vector3 Target;
    }
}