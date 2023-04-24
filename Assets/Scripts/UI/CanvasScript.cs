using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    public PlayerDataSO data;

    public void UpdateCameraSensitivity(float value)
    {
        data.SENSITIVITY = value;
    }

    public void UpdateTextCrawl(float value)
    {
        data.TEXT_CRAWL_SPEED = value;
    }

    public void UpdateVolume(float value)
    {
        data.VOLUME = value;
    }
}
