using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseIconScript : MonoBehaviour
{
    // will have more future implementation when not 16:9

    RectTransform rect;
    Image image;

    [SerializeField] Sprite[] sprites;
    int index;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        index = 0;

        image.sprite = sprites[index];

        StartCoroutine(IEAnimate());
    }

    IEnumerator IEAnimate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            image.sprite = sprites[index];

            index = index == sprites.Length - 1 ? 0 : index + 1;
        }
    }
}
