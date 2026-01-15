using UnityEngine;

[System.Serializable]

public class ItemList
{
    public Items item;
    public string name;
    public int stacks;

    public ItemList(Items newItem, string newName, int newStacks)
    {
        item = newItem;
        name = newName;
        stacks = newStacks;
    }
}
