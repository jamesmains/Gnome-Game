using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CharacterGroupManager {
    private static List<CharacterGroup> AllGroups = new();
    public const int GroupLimit = 10;
    
    public static CharacterGroup CreateNewGroup(Character leader) {
        var newGroup = new CharacterGroup(leader);
        AllGroups.Add(newGroup);
        return newGroup;
    }
}

[Serializable]
public class CharacterGroup {
    public Character Leader; 
    public EntityTeams Team () => GetLeader().SelfEntity.Team;
    public List<Character> Members = new();

    public CharacterGroup(Character leader) {
        Leader = leader;
    }
    
    private Character GetLeader() {
        if(Leader == null) AssignLeader();
        return Leader;
    }

    private void AssignLeader() {
        Leader = Members.OrderByDescending(member => member.SelfEntity.Health).FirstOrDefault();
    }

    public void SeekAllies() {
        for (int i = Members.Count; i < CharacterGroupManager.GroupLimit; i++) {
            Leader.SelfEntity.FindNearestAlly();
        }
    }
}
