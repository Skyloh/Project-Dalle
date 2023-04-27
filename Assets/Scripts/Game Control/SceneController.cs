using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    GameObject loadingProgress;
    [SerializeField] Image fillImage;
    [SerializeField] Image backgroundImage;
    [SerializeField] Image fadeImage;

    [SerializeField, Range(1, 25)] int FADE_ACCURACY = 2;
    [SerializeField, Range(0.0001f, 2f)] float FADE_LERP = 0.0125f;

    [SerializeField] PlayerDataSO data;

    public static SceneController instance;

    AsyncOperation load;
    bool loading;

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

            loadingProgress = backgroundImage.gameObject;

            loadingProgress.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public void ChangeScene(string scene)
    {
        if (loading)
        {
            return;
        }

        loading = true;

        gameObject.SetActive(true);

        fillImage.fillAmount = 0f;

        StartCoroutine(IEChangeSceneProcess(scene));
    }

    private IEnumerator IEChangeSceneProcess(string build_name)
    {
        data.IS_PLAYER_ENABLED = false;

        yield return StartCoroutine(IEFadeToColor(Color.black, 1f));

        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);

        loadingProgress.SetActive(true);

        yield return StartCoroutine(IEFadeToColor(Color.clear, 4f));

        load = SceneManager.LoadSceneAsync(build_name, LoadSceneMode.Single);

        load.allowSceneActivation = false;

        yield return StartCoroutine(IELerpFillImage());

        yield return new WaitForSeconds(0.25f);

        load.allowSceneActivation = true;

        yield return new WaitUntil(() => load.isDone);

        yield return StartCoroutine(IEFadeToColor(Color.black, 3f));

        // fillImage = null;

        loadingProgress.SetActive(false);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(build_name));

        data.IS_PLAYER_ENABLED = true;

        yield return StartCoroutine(IEFadeToColor(Color.clear, 0.75f));

        gameObject.SetActive(false);

        loading = false;
    }

    private IEnumerator IELerpFillImage()
    {
        fillImage.fillAmount = 0f;
        backgroundImage.color = Color.black;

        float value = 0f;

        while (fillImage.fillAmount <= 0.98f || !IsSimilarColor(backgroundImage.color, Color.white))
        {
            if (load.progress < 0.9f)
            {
                value = load.progress;
            } 

            else
            {
                value += Time.smoothDeltaTime;
            }

            fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, value, Time.smoothDeltaTime);
            backgroundImage.color = Color.Lerp(backgroundImage.color, Color.white, Time.smoothDeltaTime * 2f);

            yield return new WaitForEndOfFrame();
        }

        fillImage.fillAmount = 1f;
        backgroundImage.color = Color.white;
    }

    private IEnumerator IEFadeToColor(Color destination, float multiplier)
    {
        Color progress = fadeImage.color;

        while(!IsSimilarColor(progress, destination))
        {
            progress = Color.Lerp(progress, destination, FADE_LERP * multiplier * Time.smoothDeltaTime);

            fadeImage.color = progress;

            yield return new WaitForEndOfFrame();
        }

    }

    bool IsSimilarColor(Color col1, Color col2)
    {
        return (Mathf.Abs(col1.r - col2.r)
            + Mathf.Abs(col1.g - col2.g)
            + Mathf.Abs(col1.b - col2.b)
            + Mathf.Abs(col1.a - col2.a)) * 100 < FADE_ACCURACY;
    }
}
