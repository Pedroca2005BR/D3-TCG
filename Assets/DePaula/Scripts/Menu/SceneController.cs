using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] string sceneName;

    public void PlayGame()
    {
        if (TryGetComponent<TutorialBlockerComponent>(out TutorialBlockerComponent blocker) && blocker.IsBlocked())
        {
            Debug.LogWarning("Cannot start game while in tutorial mode.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
