using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using VoidspireStudio.FNATS.Animatronics.Routes;
using VoidspireStudio.FNATS.Core;
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

    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public abstract class AnimatronicAI : MonoBehaviour
    {
        [Header("Движение")]
        [SerializeReference] private List<AnimatronicRoute> _availableRoutes;
        [SerializeField] private float _timeBetweenSteps = 0f;
        [SerializeField] private float _attackDistance = 1.5f;
        [SerializeField] private AnimatronicState _currentState = AnimatronicState.Waiting;
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

        private NavMeshAgent _agent;
        private Animator _animator;

        protected const string IDLE = "IDLE";
        protected const string WALKING = "WALK";
        protected const string ATTACK = "ATTACK";
        protected const string RUN = "RUNNING";

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();

            _agent.updateRotation = false;
        }

        private void Start()
        {
            _fallbackReturnPosition = transform.position;
        }

        private void Update()
        {
            Vector3 velocity = _agent.velocity;
            velocity.y = 0;

            if (velocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }

        private void FixedUpdate()
        {
            if (_currentState == AnimatronicState.Off)
                return;

            if ((_currentState == AnimatronicState.Route ||
                _currentState == AnimatronicState.Waiting) &&
                TryFindPlayer())
                _currentState = AnimatronicState.Forwarding;

            switch (_currentState)
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

        protected abstract void OfficeAtack();

        private void Attack()
        {
            Player.Instance.Freeze();
            Player.Instance.ForceLookAt(_head.transform.position);
            _agent.isStopped = true;

            //_animator.SetTrigger(ATTACK);

            Debug.Log("Game over");
        }

        protected abstract void PerformSabotage(SabotageStep step);

        private void Wait()
        {
            //_animator.SetTrigger(IDLE);
            _waitTimer -= Time.fixedDeltaTime;

            if (_waitTimer <= 0f)
            {
                _waitTimer = 0f;
                _currentState = AnimatronicState.Route;

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
                _waitTimer += _timeBetweenSteps;
                _currentState = AnimatronicState.Waiting;
            }
        }

        private void ProcessStep()
        {
            switch (_currentStep)
            {
                case GoToStep goToStep:
                    //_animator.SetTrigger(WALKING);
                    _agent.SetDestination(goToStep.Target.position);
                    break;

                case WaitStep waitStep:
                    //_animator.SetTrigger(IDLE);
                    _waitTimer += waitStep.WaitTime;
                    _currentState = AnimatronicState.Waiting;
                    break;

                case SabotageStep:
                    _currentState = AnimatronicState.Sabotage;
                    break;

                case AttackStep:
                    break;
            }
        }

        private void PlayerForward()
        {
            //_animator.SetTrigger(RUN);

            if (TryFindPlayer())
            {
                _agent.SetAreaCost(0, _agent.GetAreaCost(3));
                _lastSeenPosition = Player.Instance.transform.position;
                _agent.SetDestination(_lastSeenPosition);

                if ((transform.position - _lastSeenPosition).magnitude <= _attackDistance)
                {
                    _currentState = AnimatronicState.Attack;
                    _agent.SetAreaCost(3, NavMesh.GetAreaCost(3));
                    return;
                }
            }
            else if (_agent.HasReachedDestination())
            {
                if (_lastGoToStep != null)
                    _agent.SetDestination(_lastGoToStep.Target.position);
                else
                    _agent.SetDestination(_fallbackReturnPosition);

                _waitTimer += _timeBetweenSteps;
                _currentState = AnimatronicState.Waiting;
                _agent.SetAreaCost(0, NavMesh.GetAreaCost(0));
            }
        }

        public bool TryFindPlayer()
        {
            Vector3 directionToPlayer = Player.Instance.transform.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            float relativeViewDistance = _viewDistance;

            if (Player.Instance.IsCrouch)
                relativeViewDistance /= 2f;

            if (Player.Instance.IsRunning)
                relativeViewDistance *= 1.5f;

            if (distanceToPlayer < relativeViewDistance)
            {
                float angle = Vector3.Angle(_head.forward, directionToPlayer);
                if (angle < _fieldOfView / 2f || distanceToPlayer < _viewDistanceAround)
                {
                    if (Physics.Raycast(_head.position, directionToPlayer.normalized, out RaycastHit hit, relativeViewDistance) && hit.collider.CompareTag("Player"))
                        return true;
                }
            }

            return false;
        }
    }
}
