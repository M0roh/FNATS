using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Core;
using VoidspireStudio.FNATS.UI.Menus;

public class Credits : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 20f;

    private RectTransform contentRect;

    private void OnEnable()
    {
        GameInput.Instance.InputActions.UI.Cancel.performed += Back;

        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect не назначен!");
            enabled = false;
            return;
        }
        contentRect = scrollRect.content;
        contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0);
    }

    private void OnDisable()
    {
        GameInput.Instance.InputActions.UI.Cancel.performed -= Back;
    }

    public void Back(InputAction.CallbackContext _)
    {
        UIManager.Instance.BackToMenu(gameObject);
    }

    private void Update()
    {
        if (contentRect == null) return;

        float newY = contentRect.anchoredPosition.y + scrollSpeed * Time.deltaTime;

        float minY = 0f; 
        if (newY < minY)
            newY = minY;

        contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, newY);
    }
}
