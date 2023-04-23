using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBoardScript : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    int current;

    [SerializeField] List<Sprite> sprites;
    [SerializeField, Range(0.05f, 1f)] float tick_rate = 0.5f;

    private void Start()
    {
        current = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(IEAnimate());
    }

    IEnumerator IEAnimate()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.5f));

        while (true)
        {
            yield return new WaitForSeconds(tick_rate);

            spriteRenderer.sprite = sprites[current];

            current = current == sprites.Count - 1 ? 0 : current + 1;
        }
    }
}
