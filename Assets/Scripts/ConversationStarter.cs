using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DialogueEditor; // unity installed package for dialogues

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;//NPCConversation is the script attached to game object Robot Converdsation

    // conversation variable of type NPCConversation to hold reference to specific conversation
    private void OnMouseOver() // called every frame while mouse is over element
    {
        // Check for left-click AND if a conversation is NOT already active
        if (Input.GetMouseButtonDown(0) && !ConversationManager.Instance.IsConversationActive)
        {
            ConversationManager.Instance.StartConversation(myConversation); // accessing a global manager of dialogue system to start conversation
        } //calls the dialogue system to start the assigned conversation
    }
}// TO BE ADDED IMP
//Consider debouncing (ignore clicks while dialogue is already running) — check ConversationManager.Instance.IsConversationActive if available.
