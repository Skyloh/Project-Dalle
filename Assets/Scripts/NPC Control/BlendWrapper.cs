using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendWrapper : MonoBehaviour
{
    // god damn it i hate software so much
    // WHY REMOVE SUPPORT
    // ARGH
    //
    // anyways
    // i need this class because blender 3.4 exports only to binary FBX now.
    // Unity reads in animations, slapping a flat 0 weight on all shapeweights, always.
    // this means that you cannot change weights in code as they will be immediately set to 0.
    // Attempted Bugfixes:
    // use older version of blender with ASCII FBX export support
    // > lmao, no. cant open the 3.4 project in 2.79 AT ALL
    // toggle "Relative" in shapekeys before exporting to binary
    // > zilch. tested this 3 times.
    // use maya to import the binary FBX, then export as ASCII FBX
    // > :/
    // use lateupdate
    // > fine, twist my damn arm.

    // TODO, check to see if adding keyframes for blendshapes in animations does anything


    [SerializeField] SkinnedMeshRenderer meshRenderer;
    // optimize this TODO
    // i dont need a reference to meshrender here and in animationbehavior too

    Dictionary<int, float> vals;
    int n;

    public void Init(Dictionary<int, float> vals, int n)
    {
        this.vals = vals;
        this.n = n;
    }

    public void Push(int index, float new_val)
    {
        vals[index] = new_val;
    }

    public float GetWeight(int index)
    {
        return vals[index];
    }

    void LateUpdate()
    {
        for (int i = 1; i < n; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, vals[i]);
        }
    }
}
