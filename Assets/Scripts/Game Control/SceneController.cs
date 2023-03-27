using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] Image fillImage;
    [SerializeField] Image backgroundImage;

    public static SceneController instance;

    void Start()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            gameObject.SetActive(false);
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

        AsyncOperation load = SceneManager.LoadSceneAsync(build_name, LoadSceneMode.Single);

        load.allowSceneActivation = false;

        fillImage.fillAmount = 0f;
        backgroundImage.color = Color.black;

        while (fillImage.fillAmount <= 0.95f)
        {
            fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, load.progress, 0.1f);
            backgroundImage.color = Color.Lerp(backgroundImage.color, Color.white, load.progress * 0.1f);

            yield return new WaitForEndOfFrame();
        }

        fillImage.fillAmount = 1f;
        backgroundImage.color = Color.white;

        yield return new WaitForSeconds(0.25f);

        load.allowSceneActivation = true;

        yield return new WaitUntil(() => load.isDone);

        // fillImage = null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(build_name));

        gameObject.SetActive(false);
    }
}
