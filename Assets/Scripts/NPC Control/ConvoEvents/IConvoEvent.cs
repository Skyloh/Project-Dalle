public interface IConvoEvent
{
    // returns false if the event is meant to be removed after activating
    bool Preactivate(ref string[] text, ref string[] flair);

    void Activate(DialogueTrigger npcdt);
}