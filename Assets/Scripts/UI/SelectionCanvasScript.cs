﻿using UnityEngine;
using UnityEngine.UI;

public class SelectionCanvasScript : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] Text placard_text; // the text that the placard UI displays
    [SerializeField] Image displayed; // the reference to the UI Image displaying the painting

    UISpriteAnimatorScript background;
    [SerializeField] Sprite dupeBG;

    // OnEnable and Children don't mix well.
    // Serialized for performance.
    [SerializeField] UISpriteAnimatorScript[] uisas;
    [SerializeField] PlayerDataSO data; // the player's data we store so we can access their list of paintings
    int currently_displaying_index; // the index of the currently displayed painting

    PaintingScript target;

    private void Awake()
    {
        background = canvas.GetComponent<UISpriteAnimatorScript>();
    }

    private void Start()
    {
        this.enabled = false;
    }

    private void Update()
    {
        // in case we pause while on this menu, exit.
        if (Time.timeScale == 0f && Input.GetAxisRaw("Cancel") != 0)
        {
            OnBack();
        }
    }

    void ToggleCanvas(bool state)
    {
        canvas.SetActive(state);

        foreach (UISpriteAnimatorScript script in uisas)
        {
            script.enabled = state;
        }

        background.enabled = true;
    }

    public void StartCanvas(PaintingScript caller)
    {
        this.enabled = true;

        data.IS_PLAYER_ENABLED = false;

        target = caller;

        placard_text.text = caller.GetCurrentPlacardText();

        currently_displaying_index = 0;

        ToggleCanvas(true);

        UpdateUI();

        UIController.ToggleUIFocus(true);
    }

    public void NextPainting()
    {
        currently_displaying_index = IncrementIndex(1);

        UpdateUI();
    }

    public void PreviousPainting()
    {
        currently_displaying_index = IncrementIndex(-1);

        UpdateUI();
    }

    public void OnBack()
    {
        ToggleCanvas(false);

        data.IS_PLAYER_ENABLED = true;

        this.enabled = false;

        UIController.ToggleUIFocus(false);
    }

    public void OnSet()
    {
        target.SetPainting(data.GetPainting(currently_displaying_index));
    }

    int IncrementIndex(int value)
    {
        if (value > 0)
        {
            return (currently_displaying_index + 1) % data.LengthOfPaintings();
        }

        int c = currently_displaying_index - 1;

        if (c < 0)
        {
            return data.LengthOfPaintings() - 1;
        }

        else
        {
            return c;
        }
    }

    void UpdateUI()
    {
        PaintingSO p = data.GetPainting(currently_displaying_index);

        if (!p.CheckIfEmpty())
        {
            background.enabled = false;

            background.ForceSprite(dupeBG);
        } 

        else
        {
            background.enabled = true;

            background.ForceSprite();
        }

        displayed.sprite = p.painting;
    }
}
