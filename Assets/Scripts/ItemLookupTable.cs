    using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace I302.Manu
{
    [CreateAssetMenu(fileName = "ItemLookupTable", menuName = "Inventories and Items/LookupTable")]
    public class ItemLookupTable : SerializedScriptableObject
    {
        [field: SerializeField]
        public Dictionary<string, Item> ItemCollection { get; private set; } = new Dictionary<string, Item>();

        public Item GetItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName)) return null;
            return ItemCollection[itemName];
        }
        
#if UNITY_EDITOR
        [FoldoutGroup("Automation")][Button]
        public void PopulateItemCollection()
        {
            ItemCollection.Clear();
            List<Item> itemList = AssetManagementUtil.GetAllScriptableObjectInstances<Item>();

            foreach (Item item in itemList)
            {
                ItemCollection.Add(item.Name, item);
            }
        }
#endif
    }
}

#if UNITY_EDITOR
public static class AssetManagementUtil
{
    public static List<T> GetAllScriptableObjectInstances<T>() where T : ScriptableObject
    {
        return AssetDatabase.FindAssets($"t: {typeof(T).Name}").ToList()
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>)
            .ToList();
    }
}
#endif