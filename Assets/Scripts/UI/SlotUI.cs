using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class SlotUI : MonoBehaviour
{
    public int slotID;
    public Image itemIcon;
    public TextMeshProUGUI quantityText;


    [SerializeField] private Sprite baseItemIcon;

    [SerializeField] private GameObject highlight;

    public void SetItem(Inventory.Slot slot)
    {
        if(slot != null)
        {
            if (slot.icon == null) 
            {
                slot.icon = baseItemIcon; 
            }
            else
            {
                itemIcon.sprite = slot.icon;
            }
            itemIcon.color = new (1,1,1,1);
            quantityText.text = slot.count.ToString();
        }
    }

    public void SetEmpty()
    {
        itemIcon.sprite = baseItemIcon;
        itemIcon.color = new(1, 1, 1, 1);
        quantityText.text ="";
    }

    public void SetHightlight(bool isOn)
    {
        highlight.SetActive(isOn); 
    }
}
