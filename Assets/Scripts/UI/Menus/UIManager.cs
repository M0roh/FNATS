using System.Collections;
using UnityEngine;

namespace VoidspireStudio.FNATS.UI.Menus
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Меню")]
        [SerializeField] private GameObject _settings;
        [SerializeField] private GameObject _credits;

        [Header("Настройки")]
        [SerializeField] private float _timeBetweenMenuSwapping = 0.1f;

        private GameObject _previousMenu;
        private GameObject _openedSubMenu;

        public bool IsOpenedSubMenu => _openedSubMenu != null;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void OpenSettings(GameObject fromMenu)
        {
            StartCoroutine(OpenSubMenu(fromMenu, _settings));
            _previousMenu = fromMenu;
        }

        public void OpenCredits(GameObject fromMenu)
        {
            StartCoroutine(OpenSubMenu(fromMenu, _credits));
            _previousMenu = fromMenu;
        }

        public void BackToMenu(GameObject fromMenu)
        {
            if (_previousMenu == null) return;

            StartCoroutine(CloseSubMenu(fromMenu));
        }

        public IEnumerator CloseSubMenu(GameObject fromMenu)
        {
            var animator = fromMenu.GetComponent<Animator>();
            animator.ResetTrigger("SHOW");
            animator.SetTrigger("HIDE");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Hide"));

            StartCoroutine(DestroyOnEnd(fromMenu, animator));

            yield return new WaitForSecondsRealtime(_timeBetweenMenuSwapping);

            var previousAnimator = _previousMenu.GetComponent<Animator>();
            previousAnimator.ResetTrigger("HIDE");
            previousAnimator.SetTrigger("SHOW");
        }

        public IEnumerator DestroyOnEnd(GameObject objectToDestroy, Animator animator)
        {
            yield return new WaitForSecondsRealtime(animator.GetCurrentAnimatorStateInfo(0).length);

            Destroy(objectToDestroy);
        }

        public IEnumerator OpenSubMenu(GameObject menuFrom, GameObject subMenu)
        {
            if (_openedSubMenu != null) yield break;
            _openedSubMenu = new GameObject();

            var animator = menuFrom.GetComponent<Animator>();
            animator.ResetTrigger("SHOW");
            animator.SetTrigger("HIDE");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Hide"));

            yield return new WaitForSecondsRealtime(_timeBetweenMenuSwapping);

            Destroy(_openedSubMenu);
            _openedSubMenu = Instantiate(subMenu, menuFrom.transform.parent);
        }
    }
}
