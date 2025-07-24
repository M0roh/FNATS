using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using VoidspireStudio.FNATS.Animatronics.Routes;
using VoidspireStudio.FNATS.Core;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.Animatronics
{
    public enum AnimatronicState
    {
        Off,
        Route,
        Waiting,
        Forwarding,
        Sabotage,
        Attack
    }

    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public abstract class BaseAnimatronicAI : MonoBehaviour
    {
        [Header("Path")]
        [SerializeField] private List<AnimatronicRoute> _availableRoutes;
        [SerializeField] private float _timeBetweenSteps = 0f;
        private AnimatronicRoute _currentRoute;
        private RouteStep _currentStep;
        private GoToStep _lastGoToStep;
        private float _waitTimer = 0f;

        [Header("Обзор")]
        [SerializeField] private Transform _head;
        [SerializeField] private float _viewDistance = 10f;
        [SerializeField] private float _fieldOfView = 90f;

        private AnimatronicState _currentState;
        private Vector3 _lastSeenPosition;

        private NavMeshAgent _agent;
        private Animator _animator;

        private const string IDLE = "IDLE";
        private const string WALKING = "WALK";
        private const string ATTACK = "ATTACK";
        private const string RUN = "RUNNING";

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();

            _agent.updateRotation = false;
        }

        private void Update()
        {
            if (_currentState == AnimatronicState.Off)
                return;

            if (TryFindPlayer())
                _currentState = AnimatronicState.Forwarding;

            switch (_currentState)
            {
                case AnimatronicState.Off:
                    return;
                
                case AnimatronicState.Route:
                    Route();
                    break;
                
                case AnimatronicState.Sabotage:
                    break;
                
                case AnimatronicState.Waiting:
                    Wait();
                    break;

                case AnimatronicState.Forwarding:
                    PlayerForward();
                    break;
            }
        }

        public void Wait()
        {
            _animator.SetTrigger(IDLE);
            _waitTimer -= Time.deltaTime;

            if (_waitTimer <= 0f)
            {
                _waitTimer = 0f;
                _currentState = AnimatronicState.Route;

                if (!_currentStep.HasNextSteps)
                {
                    var routes = _availableRoutes;
                    if (routes.Count > 1)
                        routes = routes.Where(route => route.RouteID != _currentRoute.RouteID).ToList();

                    var newRoute = routes[Random.Range(0, _availableRoutes.Count)];
                    _currentRoute = newRoute;

                    if (_currentStep is SabotageStep sabotageStep)
                        _currentStep = newRoute.GetNearestStep(_lastGoToStep.Target.position);
                    else
                        _currentStep = newRoute.GetStartStep;
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

        public void Route()
        {
            if (_agent.HasReachedDestination())
            {
                _waitTimer += _timeBetweenSteps;
                _currentState = AnimatronicState.Waiting;
            }
        }

        public void ProcessStep()
        {
            switch (_currentStep)
            {
                case GoToStep goToStep:
                    _animator.SetTrigger(WALKING);
                    _agent.SetDestination(goToStep.Target.position);
                    break;

                case WaitStep waitStep:
                    _animator.SetTrigger(IDLE);
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

        public void PlayerForward()
        {
            _animator.SetTrigger(RUN);

            _agent.SetAreaCost(3, 1);
            _agent.SetAreaCost(0, 1);

            if (TryFindPlayer())
            {
                _lastSeenPosition = Player.Instance.transform.position;
                _agent.SetDestination(_lastSeenPosition);
            }
            else
            {
                if (Vector3.Distance(transform.position, _lastSeenPosition) < 0.5f)
                {
                    _agent.SetDestination(_lastGoToStep.Target.position);
                    _currentState = AnimatronicState.Route;
                    _agent.SetAreaCost(3, 5);
                    _agent.SetAreaCost(0, 1);
                }
            }

            if ((Player.Instance.transform.position - transform.position).magnitude < 0.5f)
                _currentState = AnimatronicState.Attack;
        }

        public bool TryFindPlayer(bool _obstacleCheck = true)
        {
            Vector3 directionToPlayer = Player.Instance.transform.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer < _viewDistance)
            {
                float angle = Vector3.Angle(_head.forward, directionToPlayer);
                if (angle < _fieldOfView / 2f)
                {
                    float relativeViewDistance = _viewDistance;

                    if (Player.Instance.IsCrouch)
                        relativeViewDistance /= 2f;

                    if (Player.Instance.IsRunning)
                        relativeViewDistance *= 1.5f;

                    if (!_obstacleCheck || !Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit _, relativeViewDistance))
                        return true;
                }
            }

            return false;
        }
    }
}
