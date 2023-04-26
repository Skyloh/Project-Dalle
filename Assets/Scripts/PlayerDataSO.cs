using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerDataSO : ScriptableObject
{
    public float MAX_DISTANCE = 2.1f;
    public float VOLUME = 1f;
    public float TEXT_CRAWL_SPEED = 0.1f;
    public float SENSITIVITY = 0.2f;

    public bool IS_PLAYER_ENABLED = true; // hacky solution

    // list of selectables the player has
    [SerializeField] PaintingSO[] PAINTINGS;

    public void ScrambleAndInitPaintings()
    {
        for (int i = 0; i < PAINTINGS.Length; i++)
        {
            int index = Random.Range(0, PAINTINGS.Length);

            var cache = PAINTINGS[index];

            PAINTINGS[i].used = PAINTINGS[index].used = false;

            PAINTINGS[index] = PAINTINGS[i];
            PAINTINGS[i] = cache;
        }
    }

    public int LengthOfPaintings()
    {
        return PAINTINGS.Length;
    }

    public void SetPaintings(PaintingSO[] paintings)
    {
        this.PAINTINGS = paintings;
    }

    public PaintingSO GetPainting(int index)
    {
        if (index < 0 || index > PAINTINGS.Length)
        {
            Debug.LogError(string.Format("Index {0} is not valid for length {1}!", index, PAINTINGS.Length));

            return null;
        }

        return PAINTINGS[index];
    }
}
