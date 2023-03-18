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

    public void OnHit(Transform source)
    {
        // source is Camera, and Camera has this component.
        // I didn't want to make it static, so i just did this instead.
        SelectionCanvasScript s = source.GetComponent<SelectionCanvasScript>();
        
        // TODO: this could be activated twice if clicked fast enough?
        s.StartCanvas(this);
    }

    public void SetPainting(PaintingSO p)
    {
        paintingSO = p;

        painting.sprite = p.painting;
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
