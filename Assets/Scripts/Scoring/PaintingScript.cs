using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaintingScript : MonoBehaviour, IRaycastable
{
    [SerializeField] PaintingSO paintingSO;
    [SerializeField] PlacardSO placardSO;
    [SerializeField] SpriteRenderer painting;
    [SerializeField] TextMeshPro placardText;

    void Start()
    {
        placardText.text = placardSO.description;
        painting.sprite = paintingSO.painting;
    }

    public bool OnHit(Transform source)
    {
        // source is Camera, and Camera has this component.
        // I didn't want to make it static, so i just did this instead.
        SelectionCanvasScript s = source.GetComponent<SelectionCanvasScript>();
        
        s.StartCanvas(this);

        return false; // stop camera raycasting
    }

    public void SetPainting(PaintingSO p)
    {
        if (paintingSO)
        {
            paintingSO.used = false;
        }

        paintingSO = p;

        p.used = true;

        painting.sprite = p.painting;
    }

    public string GetCurrentPlacardText()
    {
        return placardSO.description;
    }

    public void SetPlacard(PlacardSO p)
    {
        placardSO = p;

        placardText.text = placardSO.description;
    }

    public int CalculateScore()
    {
        return paintingSO.Score(placardSO.GetWords());
    }

    public int GetMaxScorePossible()
    {
        return paintingSO.MaxScore();
    }
}
