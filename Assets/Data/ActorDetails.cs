using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Flags]
public enum ActorTeam {
    NoTeam = 0,
    Team0 = 1 << 0,
    Team1 = 1 << 1,
    Team2 = 1 << 2,
    Team3 = 1 << 3,
}
// Reminder to myself
// to compare flags (bit wise operation)
// (currentEnum & incomingEnum) != 0

[CreateAssetMenu(fileName = "New Actor Details", menuName = "GNOME/Actor Details")]
public class ActorDetails : SerializedScriptableObject {
    [SerializeField, BoxGroup("Settings"), PreviewField]
    public Sprite ActorBodySprite;

    [SerializeField, BoxGroup("Settings")] 
    public Color ActorBodyColor = Color.white;

    [SerializeField, BoxGroup("Settings")] 
    public string ActorName;

    [SerializeField, BoxGroup("Settings")] 
    public ActorTeam FriendlyWithTeam = ActorTeam.Team0;

    [SerializeField, BoxGroup("Settings")] 
    public ActorBrain BrainBehavior;

    [SerializeField, BoxGroup("Settings")] 
    public List<StatValue> BaseStats;
}