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
    Walk,
    Gesture,
    Action
}


public class NPCAnimationBehavior : MonoBehaviour
{
    [SerializeField] Animator animatorSource;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] Aim headAim;
    [SerializeField] Aim chestAim;


    [SerializeField] [Range(1f, 100f)] float ACCURACY = 5f;
    [SerializeField] float BLEND_LERP = 0.0125f;

    Mesh skinnedMesh;

    Dictionary<BlendEmotions, int> emotions_to_blendindex;

    string[] states = new string[] { "closed", "happy", "sad", "angry", "wink" };

    bool resetting_mood = false;

    private void Awake()
    {
        // despite the name, the sharedMesh does not share blendshape values
        // among instances of the same object in the scene.
        skinnedMesh = meshRenderer.sharedMesh;

        emotions_to_blendindex = new Dictionary<BlendEmotions, int>();

        for (int i = 0; i < states.Length; i++)
        {
            // this is all commented out post-maya import because the names got fucked up

            // int index = skinnedMesh.GetBlendShapeIndex(states[i]);

            // emotions_to_blendindex.Add((BlendEmotions)i, index);
            // index_to_weight.Add(index, 0);

            emotions_to_blendindex.Add((BlendEmotions)i, i);
        }
    }

    public void SetRuntimeAnimator(RuntimeAnimatorController rac)
    {
        animatorSource.runtimeAnimatorController = rac;
    }

    public void PlayAnimation(Animations anim)
    {
        // BUGGED, doesnt do anything => animatorSource.CrossFade(request, 0.25f);

        //animatorSource.SetInteger(stateHash, (int)anim);

        switch (anim)
        {
            case Animations.Idle:
                animatorSource.CrossFade("Idle", 0.2f);
                //animatorSource.Play("Idle");
                break;

            case Animations.Gesture:
                animatorSource.CrossFade("Gesture", 0.2f);
                break;

            case Animations.Walk:
                animatorSource.CrossFade("Walk", 0.2f);
                break;

            default:
                break;
        }
    }

    // given a string argument, performs the desired behavior
    public void Dispatch(string arg)
    {
        switch (arg)
        {
            case "":
                this.ClearWeights();
                break;

            case "resetgaze":
                LookAt(Vector3.zero, AimTargetOps.Reset);
                break;

            case "gesture":
                animatorSource.CrossFade("Gesture", 0.1f);
                break;

            case "idleflair":
                animatorSource.CrossFade("Idle Anim", 0.1f);
                break;

            default: // must be an emotion => emote Happy 10
                if (!arg.Contains("emote"))
                {
                    Debug.LogError(string.Format("Invalid argument {0}", arg));

                    return;
                }

                string[] result = arg.Split(' ');

                int index = 0;

                while (!states[index].Equals(result[1].ToLower()))
                {
                    index += 1;
                }

                SetBlendWeight((BlendEmotions)index, int.Parse(result[2]) * 10f);

                break;
        }
    }


    public void SetBlendWeight(BlendEmotions blend, float weight)
    {
        if (resetting_mood)
        {
            return; // if we are resetting mood, don't care about setting it to something else.
        }

        StopAllCoroutines();

        StartCoroutine(IELerpBlendWeight(blend, weight));
    }

    public void ClearWeights()
    {
        StopAllCoroutines();

        StartCoroutine(IELerpAllToZero());
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

    IEnumerator IELerpAllToZero()
    {
        resetting_mood = true;

        float avg_weight = 100f;

        while (avg_weight > ACCURACY)
        {
            avg_weight = 0f;

            for (int i = 0; i < skinnedMesh.blendShapeCount; i++)
            {
                float current_weight = meshRenderer.GetBlendShapeWeight(i);

                current_weight = Mathf.Lerp(current_weight, 0f, BLEND_LERP);

                meshRenderer.SetBlendShapeWeight(i, current_weight);

                avg_weight += current_weight;
            }

            avg_weight /= skinnedMesh.blendShapeCount;

            yield return new WaitForEndOfFrame(); // i dont need Update's fidelity.
        }

        for (int i = 0; i < skinnedMesh.blendShapeCount; i++)
        {
            meshRenderer.SetBlendShapeWeight(i, 0f);
        }

        resetting_mood = false;
    }
}
