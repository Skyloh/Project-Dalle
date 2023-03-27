using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlendEmotions
{
    Closed,
    Happy,
    Sad,
    Angry,
    Wink
}

public enum AimTargetOps
{
    Head,
    Chest,
    Both,
    Reset
}

public enum Animations 
{ 
    Idle,
    Walk
}


public class NPCAnimationBehavior : MonoBehaviour
{
    [SerializeField] Animator animatorSource;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] Aim headAim;
    [SerializeField] Aim chestAim;
    // [SerializeField] BlendWrapper wrapper;


    [SerializeField] [Range(1f, 100f)] float ACCURACY = 5f;
    [SerializeField] float BLEND_LERP = 0.0125f;


    RuntimeAnimatorController animator;
    int stateHash;
    Mesh skinnedMesh;

    Dictionary<BlendEmotions, int> emotions_to_blendindex;
    // Dictionary<int, float> index_to_weight;
    IEnumerator blendProcess;

    private void Awake()
    {
        animator = animatorSource.runtimeAnimatorController;

        stateHash = Animator.StringToHash("State");

        // despite the name, the sharedMesh does not share blendshape values
        // among instances of the same object in the scene.
        skinnedMesh = meshRenderer.sharedMesh;

        string[] states = new string[] { "Closed", "Happy", "Sad", "Angry", "WinkLeft" };

        emotions_to_blendindex = new Dictionary<BlendEmotions, int>();
        // index_to_weight = new Dictionary<int, float>();

        for (int i = 0; i < states.Length; i++)
        {
            // int index = skinnedMesh.GetBlendShapeIndex(states[i]);

            // emotions_to_blendindex.Add((BlendEmotions)i, index);
            // index_to_weight.Add(index, 0);

            emotions_to_blendindex.Add((BlendEmotions)i, i);
        }

        // wrapper.Init(index_to_weight, skinnedMesh.blendShapeCount);
    }

    public void PlayAnimation(Animations anim)
    {
        // BUGGED, doesnt do anything => animatorSource.CrossFade(request, 0.25f);

        animatorSource.SetInteger(stateHash, (int)anim);
    }

    public void SetBlendWeight(BlendEmotions blend, float weight)
    {
        if (blendProcess != null)
        {
            StopCoroutine(blendProcess);
        }

        blendProcess = IELerpBlendWeight(blend, weight);

        StartCoroutine(blendProcess);
    }

    public void ClearWeights()
    {
        StopAllCoroutines();

        for (int i = 0; i < skinnedMesh.blendShapeCount; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, 0f);
            // wrapper.Push(i, 0f);
        }
    }

    public void LookAt(Vector3 position, AimTargetOps type)
    {
        switch (type)
        {
            case AimTargetOps.Head:
                if (position == Vector3.zero)
                {
                    headAim.Reset();

                    break;
                }

                headAim.LerpToDestination(position);
                break;

            case AimTargetOps.Chest:
                if (position == Vector3.zero)
                {
                    chestAim.Reset();

                    break;
                }

                chestAim.LerpToDestination(position);
                break;

            case AimTargetOps.Both:
                headAim.LerpToDestination(position);
                chestAim.LerpToDestination(position);
                break;

            default: // Reset
                headAim.Reset();
                chestAim.Reset();
                break;
        }
    }

    IEnumerator IELerpBlendWeight(BlendEmotions blend, float destination)
    {
        int index = emotions_to_blendindex[blend];

        float current_weight = meshRenderer.GetBlendShapeWeight(index); // wrapper.GetWeight(index);

        while (Mathf.Abs(current_weight - destination) > ACCURACY)
        {
            current_weight = Mathf.Lerp(current_weight, destination, BLEND_LERP);

            meshRenderer.SetBlendShapeWeight(index, current_weight);

            // wrapper.Push(index, current_weight);

            yield return new WaitForEndOfFrame(); // i dont need Update's fidelity.
        }

        meshRenderer.SetBlendShapeWeight(index, destination);

        // wrapper.Push(index, destination);
    }
}
