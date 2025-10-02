using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Sounds;

namespace VoidspireStudio.FNATS.UI
{
    public class LoadScreenScene : MonoBehaviour
    {
        public static string SceneToLoad { get; set; } = "Main Menu";

        [SerializeField] private Image _progressBar;

        private void Start()
        {
            AudioManager.Instance.StopLoopMusic();
            StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(SceneToLoad);
            op.allowSceneActivation = false;

            while (!op.isDone)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);

                _progressBar.fillAmount = progress;

                if (progress >= 0.9f)
                    op.allowSceneActivation = true;

                yield return null;
            }
        }
    }
}