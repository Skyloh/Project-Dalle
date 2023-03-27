using UnityEngine;
using UnityEngine.UI;

public class SelectionCanvasScript : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] Text placard_text; // the text that the placard UI displays
    PaintingSO painting;
    [SerializeField] Image displayed; // the reference to the UI Image displaying the painting
    [SerializeField] PlayerDataSO data; // the player's data we store so we can access their list of paintings
    int currently_displaying_index; // the index of the currently displayed painting

    PaintingScript target;

     private void Update()
    {
        // in case we pause while on this menu, exit.
        if (Input.GetAxis("Cancel") != 0)
        {
            OnBack();
        }
    }

    void ToggleCanvas(bool state)
    {
        canvas.SetActive(state);
    }

    public void StartCanvas(PaintingScript caller)
    {
        data.IS_PLAYER_ENABLED = false;

        target = caller;

        currently_displaying_index = 0;

        UpdateUI();

        ToggleCanvas(true);

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

        painting = p;

        displayed.sprite = p.painting;
    }
}
