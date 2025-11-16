using UnityEngine;

[CreateAssetMenu(fileName = "PortalAudioSet",menuName = "Game/Portal Audio Set")]
public class PortalAudioSet : ScriptableObject
{
    public AudioClip[] earthPortalClips;  // Only correct portal uses these
    public AudioClip[] spacePortalClips;  // Wrong portals use these
}
