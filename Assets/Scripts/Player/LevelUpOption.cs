using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpOption : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image icon;

    private ItemData item;
    private LevelUpUI ui;

    public void Setup(ItemData data, LevelUpUI parent)
    {
        item = data;
        ui = parent;

        nameText.text = data.itemName;
        icon.sprite = data.icon;
    }

    public void OnClick()
    {
        ui.SelectItem(item);
    }
}