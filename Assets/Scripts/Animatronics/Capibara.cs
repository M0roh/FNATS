using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using VoidspireStudio.FNATS.Player;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.Animatronics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Capibara : MonoBehaviour
    {
        public enum State
        {
            Walk,
            Run,
            Wait
        }

        [Header("Settings")]
        [SerializeField] private float _viewDistance = 4f;
        [SerializeField] private float _walkRadius = 30f;
        [SerializeField] private float _runDistance = 7f;
        [SerializeField] private float _waitBetweenTargets = 3f;
        [SerializeField] private float _rotationSpeed = 3f;

        private Vector3 _target = Vector3.zero;
        private State _currentState = State.Wait;
        private bool _isWaiting = false;

        private Coroutine _waitCoroutine;

        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            _agent.updateRotation = false;
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
            PlayerCheck();

            if (_isWaiting) return;

            switch (_currentState)
            {
                case State.Walk:
                    if (_agent.HasReachedDestination())
                        _currentState = State.Wait;
                    break;

                case State.Wait:
                    _waitCoroutine = StartCoroutine(WaitTimer(_waitBetweenTargets));
                    break;

                case State.Run:
                    if (_agent.HasReachedDestination())
                        _waitCoroutine = StartCoroutine(WaitTimer(_waitBetweenTargets * 0.5f));
                    break;
            }
        }

        public IEnumerator SetRoamingPosition()
        {
            yield return StartCoroutine(Util.GetRandomPointOnNavMesh(transform.position, _walkRadius, (result) => _target = result, 5));

            _agent.SetDestination(_target);
        }

        public IEnumerator WaitTimer(float waitTime)
        {
            _isWaiting = true;

            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(SetRoamingPosition());

            _currentState = State.Walk;

            _isWaiting = false;
        }

        public void PlayerCheck()
        {
            Vector3 playerPosition = Player.Player.Instance.transform.position;
            if (Vector3.Distance(transform.position, playerPosition) <= _viewDistance)
            {
                if (_isWaiting)
                {
                    StopCoroutine(_waitCoroutine);
                    _isWaiting = false;
                }

                _currentState = State.Run;

                Vector3 _runDirection = (transform.position - playerPosition).normalized;
                _target = transform.position + _runDirection * _runDistance;

                _agent.SetDestination(_target);
            }
        }
    }
}