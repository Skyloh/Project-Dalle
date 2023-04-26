using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Paintings", order = 1)]
public class PaintingSO : ScriptableObject
{
    public Sprite painting;

    public bool used;

    public string[] high_keywords, middle_keywords, low_keywords;

    public int MaxScore()
    {
        return high_keywords.Length * 5 + middle_keywords.Length * 3 + low_keywords.Length * 1;
    }

    public int Score(string[] words)
    {
        string[][] set = new string[][] { high_keywords, middle_keywords, low_keywords };

        List<string> keywords;
        List<string> searches = new List<string>(words);

        int sum = 0;
        int val = 5;

        foreach (string[] strs in set)
        {
            keywords = new List<string>(strs);

            for (int i = 0; i < searches.Count; i++)
            {
                if (keywords.Contains(searches[i]))
                {
                    sum += val;

                    searches.RemoveAt(i);

                    i--;
                }
            }

            val -= 2;
        }

        return sum;
    }

}
