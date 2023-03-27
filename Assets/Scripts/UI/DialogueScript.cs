using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueScript : MonoBehaviour
{
    [SerializeField] PlayerDataSO data;
    [SerializeField] TextMeshProUGUI ui;
    [SerializeField] GameObject canvas;

    [SerializeField] Color32 color;
    [SerializeField] AudioClip crawl_audio, progress_audio;

    string[] all_text;

    string rendering_text;
    int rend_index;
    int crawl;

    int current;

    bool completed; // is the text finished rendering
    bool rendering; // are we in the process of rendering

    float lockout;

    private void Awake()
    {
        this.enabled = false;
    }

    public void Init(string[] text, int ID)
    {
        if (ID == current)
        {
            return;
        }

        current = ID;

        StopAllCoroutines();

        all_text = text;
        rend_index = 0;

        rendering = true;
        completed = false;

        canvas.SetActive(true);

        StartText();
    }

    private void FixedUpdate()
    {
        // if we have started rendering text, the player wants to progress, and we can progress, progress.
        if (lockout > 0.75f && rendering && Input.GetAxis("Fire1") > 0)
        {
            lockout = 0f;

            ProgressText();
        }

        // if we are locked out of skipping text, increment the time
        if (!(lockout > 0.75f))
        {
            lockout += Time.fixedDeltaTime;
        }

    }

    void StartText()
    {
        if (rend_index == all_text.Length)
        {
            rendering = false;

            data.CAN_CAMERA_RAYCAST = true;

            canvas.SetActive(false);

            current = -1;

            StopAllCoroutines();

            this.enabled = false;

            return;
        }

        rendering_text = all_text[rend_index];

        ui.text = all_text[rend_index];
        ui.color = Color.clear;

        ui.ForceMeshUpdate(true);

        crawl = 0;

        StartCoroutine(IETextCrawl());
    }

    IEnumerator IETextCrawl()
    {
        TMP_TextInfo info = ui.textInfo;

        TMP_CharacterInfo[] char_info = info.characterInfo;

        Color32[] vertColors;

        yield return new WaitForSeconds(0.15f);

        while (crawl < rendering_text.Length)
        {
            if (info.characterInfo[crawl].isVisible)
            {
                int material_index = char_info[crawl].materialReferenceIndex;

                vertColors = info.meshInfo[material_index].colors32;

                int vert_index = char_info[crawl].vertexIndex;

                vertColors[vert_index + 0] = color;
                vertColors[vert_index + 1] = color;
                vertColors[vert_index + 2] = color;
                vertColors[vert_index + 3] = color;

                ui.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                SFXHandler.PlaySound(crawl_audio, true);

                char character = info.characterInfo[crawl].character;

                if (character.Equals('.'))
                {
                    yield return new WaitForSeconds(0.25f + data.TEXT_CRAWL_SPEED);
                }

                yield return new WaitForSeconds(data.TEXT_CRAWL_SPEED);
            }

            else
            {
                Debug.Log("skipping");
            }

            crawl++;
        }
        

        completed = true; // true
    }

    public void ProgressText()
    {
        if (!rendering)
        {
            return;
        }

        if (!completed)
        {
            StopAllCoroutines();

            ui.color = color;

            completed = true;

            return;
        }

        rend_index++;

        SFXHandler.PlaySound(progress_audio);

        StartText();
    }
}
