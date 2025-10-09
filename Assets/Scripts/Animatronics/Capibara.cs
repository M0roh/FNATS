using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using VoidspireStudio.FNATS.Utils;
using Cysharp.Threading.Tasks;
using System.Threading;

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

        private CancellationTokenSource _waitCancelToken;

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
                    WaitTimerWrapper(_waitBetweenTargets).Forget();
                    break;

                case State.Run:
                    if (_agent.HasReachedDestination())
                        WaitTimerWrapper(_waitBetweenTargets / 2).Forget();
                    break;
            }
        }

        public async UniTask SetRoamingPosition(CancellationToken token = default)
        {
            _target = (await Util.GetRandomPointOnNavMesh(transform.position, _walkRadius, 5, cancellationToken: token).SuppressCancellationThrow()).Result;

            _agent.SetDestination(_target);
        }

        public async UniTask WaitTimer(float waitTime)
        {
            _isWaiting = true;

            await UniTask.Delay(Mathf.RoundToInt(waitTime * 1000), cancellationToken: _waitCancelToken.Token).SuppressCancellationThrow();

            await SetRoamingPosition(_waitCancelToken.Token);

            _currentState = State.Walk;

            _isWaiting = false;
        }

        public async UniTask WaitTimerWrapper(float waitTime)
        {
            _waitCancelToken = CancellationTokenSource.CreateLinkedTokenSource(new(), this.GetCancellationTokenOnDestroy());
            try
            {
                await WaitTimer(waitTime).SuppressCancellationThrow();
            }
            finally
            {
                _waitCancelToken.Dispose();
                _waitCancelToken = null;
            }
        }

        public void PlayerCheck()
        {
            Vector3 playerPosition = Player.Player.Instance.transform.position;
            if (Vector3.Distance(transform.position, playerPosition) <= _viewDistance)
            {
                if (_isWaiting)
                {
                    if (_waitCancelToken != null)
                    {
                        _waitCancelToken.Cancel();
                        _waitCancelToken.Dispose();
                        _waitCancelToken = null;
                    }
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