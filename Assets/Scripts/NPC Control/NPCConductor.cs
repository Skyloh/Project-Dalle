using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCConductor : MonoBehaviour
{
    [SerializeField] GameObject NPC;
    [SerializeField] int NPC_COUNT = 5;
    [SerializeField] Transform LOCUS;
    [SerializeField] RuntimeAnimatorController[] ANIMATORS;

    [SerializeField] string[] names;
    [SerializeField] DialoguePack[] dialogue;
    Dictionary<string, NPCController> npcs;

    private void Awake()
    {
        npcs = new Dictionary<string, NPCController>();

        GameObject[] all_paintings = GameObject.FindGameObjectsWithTag("Painting");

        ObservationTransform[] ots = new ObservationTransform[all_paintings.Length];

        int index = 0;
        foreach (GameObject painting in all_paintings)
        {
            ots[index] = new ObservationTransform(painting.transform);
            
            index += 1;
        }

        List<string> used_names = new List<string>(names);
        List<DialoguePack> used_packs = new List<DialoguePack>(dialogue);

        for (int i = 0; i < NPC_COUNT; i += 1)
        {
            GameObject npc = GameObject.Instantiate(NPC, LOCUS.position + Random.onUnitSphere * Random.Range(0f, 5f), Quaternion.identity);

            int name_index = (int)Random.Range(0f, used_names.Count);
            int dialogue_index = (int)Random.Range(0f, used_packs.Count);

            npc.name = used_names[name_index];
            DialoguePack npc_pack = used_packs[dialogue_index];

            used_names.RemoveAt(name_index);
            used_packs.RemoveAt(dialogue_index);

            NPCController npcc = npc.GetComponent<NPCController>();

            npcc.InitNPC(ots, npc_pack, ANIMATORS[(int)Random.Range(0f, ANIMATORS.Length)], false);

            npcs.Add(npc.name, npc.GetComponent<NPCController>());
        }
    }
}
