using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour
{
    public static SceneLoadingManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void LoadScene(int ID)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(ID);
    }

    public void LoadActiveScene()
    {
        var activeScene = SceneManager.GetActiveScene();
        LoadScene(activeScene.buildIndex);
    }

    public void LoadNextScene()
    {
        var activeScene = SceneManager.GetActiveScene();
        LoadScene(activeScene.buildIndex + 1);
    }
}
