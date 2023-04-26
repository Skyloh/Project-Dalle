using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueScript : MonoBehaviour
{
    [SerializeField] PlayerDataSO data;
    [SerializeField] SFXHandler sfx; // use the player sound source

    [SerializeField] TextMeshProUGUI ui, name_ui;
    [SerializeField] GameObject displayMouse;
    [SerializeField] GameObject canvas;

    [SerializeField] Color32 color;
    [SerializeField] AudioClip crawl_audio, progress_audio;

    [SerializeField] float LOCKOUT_TIMER = 1f;

    [SerializeField] char[] DELAY_CHARS = new char[] { '.', '!', '?'};
    List<char> delay_chars;

    string[] all_text;
    string[] flair;
    IConvoEvent postConvoEvent;
    NPCAnimationBehavior speakingNPCAnimationBehavior;

    int rend_index; // what index are we at in the set of texts
    int crawl; // what character position are we at in the rendering text

    int current; // who is currently talking (by instanceID)

    bool rendering; // are we in the process of rendering any text

    float lockout; // how long the player is locked out of progressing text after clicking.

    private void Awake()
    {
        this.enabled = false;
        delay_chars = new List<char>(DELAY_CHARS);
    }

    public void Init(string[] text, string[] flair, int ID, NPCAnimationBehavior npc, string npc_name, IConvoEvent pce)
    {
        if (ID == current)
        {
            return;
        }

        current = ID;

        StopAllCoroutines();

        name_ui.text = npc_name;

        all_text = text;
        this.flair = flair;

        postConvoEvent = pce;

        rend_index = 0;
        speakingNPCAnimationBehavior = npc;

        npc.ClearWeights();

        canvas.SetActive(true);

        StartText();
    }

    private void FixedUpdate()
    {
        // if the player wants to progress and we can progress, progress.
        if (lockout > LOCKOUT_TIMER && Input.GetAxis("Fire1") > 0)
        {
            ProgressText();
        }

        // if we are locked out of skipping text, increment the time
        if (!(lockout > LOCKOUT_TIMER))
        {
            lockout += Time.fixedDeltaTime;
        }

        // if somehow we're still rendering but the player becomes enabled, stop.
        // if (rendering && data.IS_PLAYER_ENABLED)
        // {
        //    StopDialogue();
        //}

    }

    void StartText()
    {
        displayMouse.SetActive(false);

        // if we've reached the end of all dialogue
        if (rend_index == all_text.Length)
        {
            StopDialogue();

            postConvoEvent?.Activate(
                speakingNPCAnimationBehavior
                .gameObject
                .GetComponent<DialogueTrigger>());

            return;
        }

        // why do i have to wrap everything in an array :[
        string[] flairs = flair[rend_index].Split(new string[] { ", " }, StringSplitOptions.None);

        foreach (string s in flairs)
        {
            speakingNPCAnimationBehavior.Dispatch(s.ToLower());
        }

        ui.text = all_text[rend_index];
        ui.ForceMeshUpdate();

        rendering = true;
        StartCoroutine(IEGradualReveal());
    }

    IEnumerator IEGradualReveal()
    {
        int total_characters = ui.textInfo.characterCount;

        ui.maxVisibleCharacters = 0;

        crawl = 0;

        yield return new WaitForSeconds(0.15f);

        while (crawl < total_characters)
        {
            sfx.PlaySound(crawl_audio, true);

            ui.maxVisibleCharacters = crawl + 1;// crawl % (total_characters + 1);

            char character = ui.textInfo.characterInfo[crawl].character;

            if (delay_chars.Contains(character))
            {
                yield return new WaitForSeconds(0.25f + data.TEXT_CRAWL_SPEED);
            }

            yield return new WaitForSeconds(data.TEXT_CRAWL_SPEED);

            crawl += 1;
        }

        displayMouse.SetActive(true);

        rendering = false;
    }

    public void ProgressText()
    {
        lockout = 0f;

        if (rendering)
        {
            StopAllCoroutines();

            ui.maxVisibleCharacters = ui.textInfo.characterCount;

            displayMouse.SetActive(true);

            rendering = false;

            return;
        }

        rend_index++;

        // sfx.PlaySound(progress_audio);

        StartText();
    }

    public void StopDialogue()
    {
        rendering = false;

        current = -1;
        rend_index = 0;

        lockout = 0f;

        data.IS_PLAYER_ENABLED = true;

        canvas.SetActive(false);

        StopAllCoroutines();

        if (speakingNPCAnimationBehavior)
        {
            speakingNPCAnimationBehavior.ClearWeights();
        }

        this.enabled = false;
    }

    /* DEPRECATED. I found a better solution: https://www.youtube.com/watch?v=IqpgJlhtmoo
    IEnumerator IETextCrawl()
    {
        crawl = 0;
        ui.ForceMeshUpdate();

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

                sfx.PlaySound(crawl_audio, true);

                char character = info.characterInfo[crawl].character;

                if (character.Equals('.'))
                {
                    yield return new WaitForSeconds(0.25f + data.TEXT_CRAWL_SPEED);
                }

                yield return new WaitForSeconds(data.TEXT_CRAWL_SPEED);
            }

            else
            {
                // Debug.Log("skipping");
            }

            crawl++;
        }
        

        completed = true; // true
    }
    */
}
