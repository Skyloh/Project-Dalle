using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] Image fillImage;

    public static SceneController instance;

    void Start()
    {
        if (instance != this)
        {
            Destroy(this);
        }

        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ChangeScene(string scene)
    {
        gameObject.SetActive(true);

        StartCoroutine(ChangeSceneProcess(scene));
    }

    private IEnumerator ChangeSceneProcess(string build_name)
    {
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);

        // yield return new WaitUntil(() => fillImage != null); // bc GetTextElement still needs to run

        AsyncOperation load = SceneManager.LoadSceneAsync(build_name, LoadSceneMode.Single);

        load.allowSceneActivation = false;

        while (!(load.progress >= 0.98f))
        {
            fillImage.fillAmount = load.progress;

            yield return new WaitForEndOfFrame();
        }

        load.allowSceneActivation = true;

        yield return new WaitUntil(() => load.isDone);

        // fillImage = null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(build_name));

        gameObject.SetActive(false);
    }
}
