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
    bool completed;
    bool rendering;

    int current;

    bool can_progress = true;
    float lockout;

    private void Update()
    {
        if (Input.GetAxis("Fire1") > 0 && can_progress)
        {
            can_progress = false;
            lockout = 0f;

            ProgressText();
        }

        if (!can_progress)
        {
            lockout += Time.deltaTime;
        }

        if (lockout > 0.75f)
        {
            can_progress = true;
        }
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

        canvas.SetActive(true);

        StartText();
    }

    void StartText()
    {
        if (rend_index == all_text.Length)
        {
            rendering = false;

            canvas.SetActive(false);

            current = -1;

            return;
        }

        rendering_text = all_text[rend_index];

        ui.text = all_text[rend_index];
        ui.color = Color.clear;

        ui.ForceMeshUpdate(true);

        crawl = 0;

        StartCoroutine(IETextCrawl());

        completed = false;
    }

    IEnumerator IETextCrawl()
    {
        TMP_TextInfo info = ui.textInfo;

        TMP_CharacterInfo[] char_info = info.characterInfo;

        Color32[] vertColors;

        yield return new WaitForSeconds(0.15f);

        while(crawl < rendering_text.Length)
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
