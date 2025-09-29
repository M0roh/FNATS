using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using VoidspireStudio.FNATS.Animatronics.Routes;
using VoidspireStudio.FNATS.Cameras;
using VoidspireStudio.FNATS.Core;
using VoidspireStudio.FNATS.Nights;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.Animatronics
{
    public enum AnimatronicState
    {
        Off,
        Route,
        Sabotage,
        Waiting,
        Forwarding,
        Attack,
        OfficeAttack
    }

    public enum OfficeAttackState
    {
        NotAttack,
        Wait,
        Blocked,
        Attack
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public abstract class AnimatronicAI : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        [SerializeField]
        private string _id = "";

        [Header("Движение")]
        [SerializeField] private float _walkSpeed = 2f;
        [SerializeField] private float _runSpeed = 4f;
        [SerializeField] private float _forwardTimeMax = 3f;
        private float _forwardTimer = 0f;

        [Header("Атака")]
        [SerializeField] private float _attackWait = 8f;
        [SerializeField] private float _waitBlocked = 3f;
        private float _attackTimer = 0f;
        private OfficeAttackState _attackState = OfficeAttackState.NotAttack;
        protected bool ignorePlayer = false;
        protected bool isAttackOffice = false;

        [Header("Маршрут")]
        [SerializeReference] private List<AnimatronicRoute> _availableRoutes;
        [SerializeField] private float _timeBetweenSteps = 0f;
        [SerializeField] private float _attackDistance = 1.5f;
        private AnimatronicState _currentState = AnimatronicState.Waiting;
        private AnimatronicState _stateBeforeCurrent = AnimatronicState.Off;
        private AnimatronicRoute _currentRoute;
        private RouteStep _currentStep;
        private GoToStep _lastGoToStep;
        private Vector3 _fallbackReturnPosition;
        protected float _waitTimer = 0f;

        [Header("Обзор")]
        [SerializeField] private Transform _head;
        [SerializeField] private float _viewDistance = 25f;
        [SerializeField] private float _viewDistanceAround = 5f;
        [SerializeField] private float _fieldOfView = 90f;
        [SerializeField] private float _rotationSpeed = 5f;

        private Vector3 _lastSeenPosition;

        protected NavMeshAgent _agent;
        protected Animator _animator;

        public bool IsEnabled => _currentState != AnimatronicState.Off;

        public string Id => _id;

        public AnimatronicState СurrentState
        {
            get => _currentState;
            private set
            {
                _stateBeforeCurrent = _currentState;
                _currentState = value;
            }
        }

        protected const string IDLE = "Idle";
        protected const string WALKING = "Walk";
        protected const string ATTACK = "Attack";
        protected const string CROUCH_ATTACK = "CrouchAttack";
        protected const string RUN = "Run";

        private void Awake()
        {
            _walkSpeed *= NightManager.Instance.CurrentConfig.AnimatronicSpeedMultiplier;
            _runSpeed *= NightManager.Instance.CurrentConfig.AnimatronicSpeedMultiplier;
            
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();

            _agent.updateRotation = false;
            _agent.speed = _walkSpeed;
        }

        private void Start()
        {
            _fallbackReturnPosition = transform.position;
        }

        private void Update()
        {
            Vector3 velocity = _agent.velocity;
            velocity.y = 0;

            if (velocity.sqrMagnitude > 0.01f && СurrentState != AnimatronicState.Attack)
            {
                Quaternion targetRotation = Quaternion.LookRotation(velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }

        private void FixedUpdate()
        {
            if (!IsEnabled)
                return;

            if ((СurrentState == AnimatronicState.Route ||
                СurrentState == AnimatronicState.Waiting) &&
                TryFindPlayer())
                СurrentState = AnimatronicState.Forwarding;

            switch (СurrentState)
            {
                case AnimatronicState.Off:
                    return;
                
                case AnimatronicState.Route:
                    Route();
                    break;
                
                case AnimatronicState.Sabotage:
                    PerformSabotage((SabotageStep)_currentStep);
                    break;
                
                case AnimatronicState.Waiting:
                    Wait();
                    break;

                case AnimatronicState.Forwarding:
                    PlayerForward();
                    break;

                case AnimatronicState.Attack:
                    Attack();
                    break;

                case AnimatronicState.OfficeAttack:
                    OfficeAtack();
                    break;
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
                RegenerateID();
        }

        [Button]
        private void RegenerateID() => _id = System.Guid.NewGuid().ToString("N");

        [Button]
        private void CopyIdToClipboard()
        {
            GUIUtility.systemCopyBuffer = _id.ToString();
        }

        protected virtual void OfficeAtack()
        {
            ignorePlayer = true;

            switch (_attackState)
            {
                case OfficeAttackState.Wait:
                    if (BlockCheck())
                    {
                        _attackTimer = _waitBlocked;
                        _attackState = OfficeAttackState.Blocked;
                        return;
                    }
                    _animator.SetTrigger(IDLE);
                    _attackTimer -= Time.fixedDeltaTime;

                    if (_attackTimer <= 0f)
                    {
                        _attackTimer = _attackWait;
                        _attackState = OfficeAttackState.Attack;
                    }
                    break;

                case OfficeAttackState.Attack:
                    if (isAttackOffice) return;

                    isAttackOffice = true;
                    ignorePlayer = false;
                    
                    OfficeManager.Instance.OfficeDoor.Break();
                    OfficeManager.Instance.OfficeLight.TurnOff();

                    StartCoroutine(OfficeAttack());
                    break;

                case OfficeAttackState.Blocked:
                    if (!BlockCheck())
                    {
                        _attackTimer = 0f;
                        _attackState = OfficeAttackState.Wait;
                        return;
                    }
                    _animator.SetTrigger(IDLE);
                    _attackTimer -= Time.fixedDeltaTime;

                    if (_attackTimer <= 0f)
                    {
                        _attackTimer = _attackWait;
                        _attackState = OfficeAttackState.NotAttack;
                        _waitTimer = 1f;
                        _currentState = AnimatronicState.Waiting;
                    }
                    break;
            }
        }

        private IEnumerator OfficeAttack()
        {
            _agent.SetDestination(OfficeManager.Instance.OfficeCenter.position);

            yield return null;

            yield return new WaitUntil(() => _agent.HasReachedDestination() || _currentState == AnimatronicState.Forwarding || _currentState == AnimatronicState.OfficeAttack);

            if (_currentState == AnimatronicState.Forwarding || _currentState == AnimatronicState.Attack)
            {
                _attackState = OfficeAttackState.NotAttack;
                isAttackOffice = false;
                yield break;
            }

            if (!OfficeManager.Instance.IsPlayerInOffice)
            {
                isAttackOffice = false;
                _attackState = OfficeAttackState.NotAttack;
                _waitTimer = 1f;
                _currentState = AnimatronicState.Waiting;
                yield break;
            }
            else if (OfficeManager.Instance.IsPlayerUnderTable)
            {
                _agent.SetDestination(OfficeManager.Instance.PlayerUnderTableSearch.position);
                
                yield return new WaitUntil(() => _agent.HasReachedDestination());

                Attack();
                yield break;
            }
            else
            {
                _agent.SetDestination(Player.Player.Instance.transform.position);

                yield return null;

                yield return new WaitUntil(() => _agent.HasReachedDestination());
                Attack();
            }
        }

        protected abstract bool BlockCheck();

        private void Attack()
        {
            if (SecurityCamerasManager.Instance.IsPlayerOnCameras)
                SecurityCamerasManager.Instance.CloseCameras(new());

            _currentState = AnimatronicState.Off;

            transform.rotation = Quaternion.LookRotation(Player.Player.Instance.transform.position);

            Player.Player.Instance.Freeze();
            Player.Player.Instance.ForceLookAt(_head.transform.position);
            _agent.isStopped = true;
            _agent.ResetPath();

            _animator.ResetTrigger(RUN);
            _animator.ResetTrigger(IDLE);

            if (Player.Player.Instance.IsCrouch)
                _animator.CrossFade(CROUCH_ATTACK, 1f);
            else
                _animator.CrossFade(ATTACK, 0.2f);

            // TODO: Рестарт ночи + таймер
            Debug.Log("Game over");
        }

        protected abstract void PerformSabotage(SabotageStep step);

        private void Wait()
        {
            _animator.SetTrigger(IDLE);
            _waitTimer -= Time.fixedDeltaTime;

            if (_waitTimer <= 0f)
            {
                _waitTimer = 0f;

                if (_stateBeforeCurrent == AnimatronicState.Forwarding)
                {
                    if (_lastGoToStep != null)
                        if (_lastGoToStep.Target != null)
                            _agent.SetDestination(_lastGoToStep.Target.position);
                        else
                            Debug.LogError("LastGoToStep Target not undefined");
                    else
                        _agent.SetDestination(_fallbackReturnPosition);

                    _currentState = AnimatronicState.Route;
                    _animator.SetTrigger(WALKING);
                    return;
                }
                
                if (_currentStep == null || !_currentStep.HasNextSteps)
                {
                    var routes = _availableRoutes;
                    if (routes.Count > 1)
                        routes = routes.Where(route => route.RouteID != _currentRoute.RouteID).ToList(); 

                    var newRoute = routes[Random.Range(0, _availableRoutes.Count)];

                    if (_currentStep is not AttackStep && _lastGoToStep != null && _currentRoute != newRoute)
                        _currentStep = newRoute.GetNearestStep(_lastGoToStep.Target.position);
                    else
                        _currentStep = newRoute.GetStartStep;

                    _currentRoute = newRoute;
                }
                else
                {
                    string randomStepID = _currentStep.NextStepsIds[Random.Range(0, _currentStep.NextStepsIds.Count)];
                    _currentStep = _currentRoute.GetStepById(randomStepID);
                }

                if (_currentStep is GoToStep goToStep)
                    _lastGoToStep = goToStep;

                ProcessStep();
            }
        }

        private void Route()
        {
            if (_agent.HasReachedDestination())
            {
                _animator.SetTrigger(IDLE);
                _waitTimer += _timeBetweenSteps;
                СurrentState = AnimatronicState.Waiting;
            }
            else
                _animator.SetTrigger(WALKING);
        }

        private void ProcessStep()
        {
            switch (_currentStep)
            {
                case GoToStep goToStep:
                    _animator.SetTrigger(WALKING);
                    _agent.speed = _walkSpeed;

                    if (goToStep.Target != null)
                        _agent.SetDestination(goToStep.Target.position);
                    else
                        Debug.LogError("GoToStep Target not undefined");

                    СurrentState = AnimatronicState.Route;
                    break;

                case WaitStep waitStep:
                    _animator.SetTrigger(IDLE);
                    _waitTimer += waitStep.WaitTime;

                    СurrentState = AnimatronicState.Waiting;
                    break;

                case RotateStep rotateStep:
                    StartCoroutine(Rotate(rotateStep.Target));
                    break;

                case SabotageStep:
                    СurrentState = AnimatronicState.Sabotage;
                    break;

                case AttackStep:
                    _attackTimer = 0f;
                    _attackState = OfficeAttackState.Wait;
                    СurrentState = AnimatronicState.OfficeAttack;
                    break;
            }
        }

        public IEnumerator Rotate(Quaternion targetAngle)
        {
            while (Quaternion.Angle(transform.localRotation, targetAngle) > 0.01f)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetAngle, _rotationSpeed * Time.deltaTime);

                yield return null;
            }
            transform.localRotation = targetAngle;
        }

        private void PlayerForward()
        {
            _animator.SetTrigger(RUN);
            _agent.speed = _runSpeed;

            if (TryFindPlayer())
            {
                _agent.SetAreaCost(0, _agent.GetAreaCost(3));
                _lastSeenPosition = Player.Player.Instance.transform.position;
                _agent.SetDestination(_lastSeenPosition);
                _forwardTimer = _forwardTimeMax;
            }
            else if (_forwardTimer > 0)
            {
                _forwardTimer -= Time.fixedDeltaTime;
                _lastSeenPosition = Player.Player.Instance.transform.position;
                _agent.SetDestination(_lastSeenPosition);
            }
            else if (_agent.HasReachedDestination())
                ResetState();

            if (_agent.pathStatus == NavMeshPathStatus.PathPartial || _agent.pathStatus == NavMeshPathStatus.PathInvalid)
                ResetState();

            if ((transform.position - Player.Player.Instance.transform.position).magnitude <= _attackDistance)
            {
                Vector3 directionToPlayer = Player.Player.Instance.HeadPosition - _head.transform.position;
                if (Physics.Raycast(_head.position, directionToPlayer.normalized, out RaycastHit hit) && hit.collider.CompareTag("Player"))
                {
                    СurrentState = AnimatronicState.Attack;
                    _agent.speed = _walkSpeed;
                    _agent.SetAreaCost(0, NavMesh.GetAreaCost(0));

                    return;
                }
            }
        }

        public void ResetState()
        {
            _waitTimer += _timeBetweenSteps;
            СurrentState = AnimatronicState.Waiting;
            _agent.speed = _walkSpeed;
            _agent.SetAreaCost(0, NavMesh.GetAreaCost(0));
        }

        public bool TryFindPlayer()
        {
            if (ignorePlayer) return false;

            Vector3 directionToPlayer = Player.Player.Instance.HeadPosition - _head.transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            float relativeViewDistance = _viewDistance;

            if (Player.Player.Instance.IsCrouch)
                relativeViewDistance /= 2f;

            if (Player.Player.Instance.IsRunning)
                relativeViewDistance *= 1.5f;

            if (distanceToPlayer <= relativeViewDistance)
            {
                float angle = Vector3.Angle(_head.forward, directionToPlayer);
                if (angle < _fieldOfView / 2f || distanceToPlayer < _viewDistanceAround)
                {
                    if (Physics.Raycast(_head.position, directionToPlayer.normalized, out RaycastHit hit, relativeViewDistance) && hit.collider.CompareTag("Player"))
                    {
                        NavMeshPath path = new();
                        if (_agent.CalculatePath(Player.Player.Instance.transform.position, path) && path.status == NavMeshPathStatus.PathComplete)
                            return true;
                    }
                }
            }

            return false;
        }

        public void Disable()
        {
            _currentState = AnimatronicState.Off;
            _currentState = AnimatronicState.Off;

            _animator.ResetTrigger(WALKING);
            _animator.ResetTrigger(RUN);
            _animator.ResetTrigger(IDLE);

            _agent.ResetPath();
        }

        public void Enable() => ResetState();
    }
}
