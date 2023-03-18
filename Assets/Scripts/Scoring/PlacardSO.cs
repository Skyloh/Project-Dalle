using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Placard", menuName = "ScriptableObjects/Placards", order = 1)]
public class PlacardSO : ScriptableObject
{
    [TextArea]
    public string description;

    public string[] GetWords()
    {
        List<string> list = new List<string>(description.Split(' '));

        list.RemoveAll((string s) => s.Length < 2);

        return list.ToArray();
    }
}
