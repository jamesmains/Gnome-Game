
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat", menuName = "GNOME/Stat")]
public class Stat: ScriptableObject {
        public string StatDisplayName;
        public Sprite StatIcon;
        public StatType StatType;
}