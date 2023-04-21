using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PictureImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;

        //importer.textureType = TextureImporterType.Sprite;

        importer.maxTextureSize = 1024;
    }

    void OnPostprocessTexture(Texture2D texture)
    {
        TextureImporter importer = (TextureImporter)assetImporter;

        if (!importer.assetPath.Contains("DALL"))
        {
            return;
        }

        string[] REMOVE = new string[]
        {
            "the",
            "trending"
        };

        PaintingSO p = ScriptableObject.CreateInstance<PaintingSO>();

        string n = importer.assetPath;

        string[] ns = n.Split('-');

        n = ns[ns.Length - 1];

        n = n.Replace(',', ' ');

        List<string> split = new List<string>(n.Split(' '));
        split.RemoveAll((string s) => s.Length <= 2 || s.Equals("") || s.Equals(" ") || s.Contains(".png"));

        foreach (string s in REMOVE)
        {
            split.Remove(s);
        }

        string file_name = "";

        foreach (string s in split)
        {
            if (file_name.Length > 15)
            {
                break;
            }

            file_name += s;
        }

        file_name.Replace(' ', '-');

        int step = split.Count / 3;
        int remainder = split.Count % 3;

        string[] high = split.GetRange(0, step).ToArray();
        string[] med = split.GetRange(step, step).ToArray();
        string[] low = split.GetRange(step * 2, step + remainder).ToArray();

        AssetDatabase.CreateAsset(p, "Assets/ScriptableObjects/Paintings/" + file_name + ".asset");


        //SerializedObject so = new SerializedObject(p);


        //so.Update();

        p.high_keywords = high;
        p.middle_keywords = med;
        p.low_keywords = low;

        //so.FindProperty("ss").stringValue = "SDSDSD";

        //so.ApplyModifiedProperties();
    }

}
