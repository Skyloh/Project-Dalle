using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Placard", menuName = "ScriptableObjects/Placards", order = 1)]
public class PlacardSO : ScriptableObject
{
    [TextArea]
    public string description;

    public string[] GetWords()
    {
        List<string> list = new List<string>(description.Split(new char[2] { ' ', ',' }));

        list.RemoveAll((string s) =>
        s.Length <= 2
        || s.Equals("and")
        || s.Equals("with"));

        string last = list[list.Count - 1];

        if (last.Contains("."))
        {
            list[list.Count - 1] = last.Remove(last.IndexOf("."));
        }

        return list.ToArray();
    }
}
