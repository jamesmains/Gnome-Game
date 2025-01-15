using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Actor Details", menuName = "GNOME/Actor Details")]
public class ActorDetails : ScriptableObject {
    [SerializeField, BoxGroup("Settings"), PreviewField]
    public Sprite ActorBodySprite;
    
    [SerializeField, BoxGroup("Settings")]
    public Color ActorBodyColor = Color.white;
    
    [SerializeField, BoxGroup("Settings")]
    public string ActorName;
}
