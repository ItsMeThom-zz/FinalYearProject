using Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableItemWindow : MonoBehaviour {

    private bool IsEnabled = false;
    public Canvas InfoCanvas;

    public GameObject ItemNameField;
    public GameObject ItemInfoField;

    private TextMeshProUGUI _itemName;
    private TextMeshProUGUI _itemInfo;


    private void Start()
    {
        CrosshairItemDetector.ItemHit        += ShowInfoBox;
        CrosshairItemDetector.NothingHit     += HideInfoBox;
        _itemName = ItemNameField.GetComponent<TextMeshProUGUI>();
        _itemInfo = ItemInfoField.GetComponent<TextMeshProUGUI>();
    }


    private void ShowInfoBox(GameObject obj)
    {
        if (!InfoCanvas.enabled)
        {
            //print("Showing Weapon Info Box");
            InfoCanvas.enabled = true;
            IsEnabled = true;
        }
        InteractableItem item = obj.GetComponent<InteractableItem>();
        if(item == null) { print("NULL ITEM"); return; }
        _itemName.text = item.Name;
        _itemInfo.text = item.Info;
    }

    public void HideInfoBox()
    {
        
        if (IsEnabled)
        {
            InfoCanvas.enabled = false;
            //print("Hiding Canvas this time");
            InfoCanvas.enabled = false;
            IsEnabled = false;
        }
        else
        {
            //print("Called but not hiding");
        }
    }
}
