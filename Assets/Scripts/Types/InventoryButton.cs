using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour {
    public int slot;

    private GameController gc;
    private Image icon;
    private Text text;

    private Item item
    {
        get {
            if (gc.inventory[slot] != null)
            {
                return gc.inventory[slot][0];
            }
            else
            {
                return null;
            }
        }
    }

    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        gc.RaiseInvEvent += SetItem;

        icon = transform.Find("Icon").GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();

        text.text = "";
    }

    public void UseItem()
    {
        if (gc.state == GameState.inBattle)
        {
            if (gc.inventory[slot] != null)
            {
                if (gc.currentItem == null)
                {
                    gc.battle.SetCurrentAttackPreview(item);
                    gc.player.SetAttack(item.Func, "");

                    if (!gc.cheatInfiniteItems)
                    {
                        gc.TakeItem(slot);
                    }
                }
                else if (gc.currentItem.name == item.name)
                {
                    gc.battle.NullCurrentAttackPreview();
                    if (!gc.cheatInfiniteItems)
                    {
                        gc.GiveItem(slot, item);
                    }
                }
            }
            else
            {
                if (gc.currentItem != null)
                {
                    gc.GiveItem(slot, gc.currentItem);
                    gc.battle.NullCurrentAttackPreview();
                }
            }
        }
    }

    public void SetItem()
    {
        if (gc.inventory[slot] != null)
        {
            icon.sprite = gc.inventory[slot][0].sprite;
            text.text = gc.inventory[slot].Count.ToString();
        }
        else
        {
            icon.sprite = null;
            text.text = "";
        }
    }
}
