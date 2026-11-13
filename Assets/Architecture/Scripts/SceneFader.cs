using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;
    public CanvasGroup FaderGroup;
    public float SceneTransitionTime = 1f;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        
        DontDestroyOnLoad(gameObject);
    }

    public async void FadeToScene(string scene)
    {
        await Fade(0f, 1f, SceneTransitionTime / 2f);

        await SceneManager.LoadSceneAsync(scene);

        await Awaitable.WaitForSecondsAsync(1f);

        await Fade(1f, 0f, SceneTransitionTime / 2f);
    }

    private async Awaitable Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        FaderGroup.alpha = startAlpha;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            FaderGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            
            await Awaitable.NextFrameAsync();
        }

        FaderGroup.alpha = endAlpha;
        GameManager.Instance.ChangeState(GameState.Playing);
    }
}
