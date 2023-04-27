using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// abstract this with the tutorial thing
public class UISpriteAnimatorScript : MonoBehaviour
{
    [SerializeField] float max_offset_delay = 0.5f;
    [SerializeField] List<Sprite> sprites;
    int index;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        
        enabled = false;
    }

    public void ForceSprite(Sprite sprite = null)
    {
        if (!sprite)
        {
            image.sprite = sprites[0];
        }

        else
        {
            image.sprite = sprite;
        }
    }

    private void OnEnable()
    {
        index = 0;

        StartCoroutine(IEAnimate());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator IEAnimate()
    {
        yield return new WaitForSecondsRealtime(Random.Range(0f, max_offset_delay));

        while (true)
        {
            yield return new WaitForSecondsRealtime(0.5f);

            image.sprite = sprites[index];

            index = index == sprites.Count - 1 ? 0 : index + 1;
        }
    }
}
