using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    public static LevelUpUI Instance;

    public GameObject panel;
    public LevelUpOption[] optionButtons;

    private Player player;

    private void Awake()
    {
        // Debug
        Debug.LogError("LevelUpUI Awake FIRED");

        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }


public void Show(Player p)
    {
        player = p;
        Time.timeScale = 0f;
        panel.SetActive(true);

        // Cursor to select item
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        List<ItemData> options = GetRandomItems(3);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].Setup(options[i], this);
        }
    }

    public void SelectItem(ItemData data)
    {
        player.AddItem(data);
        panel.SetActive(false);
        Time.timeScale = 1f;

        // Cursor to select item
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    List<ItemData> GetRandomItems(int count)
    {
        List<ItemData> pool = new List<ItemData>(ItemDatabase.Instance.allItems);
        List<ItemData> result = new List<ItemData>();

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }
}