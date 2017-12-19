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
        get { return gc.inventory[slot][0]; }
    }

    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        gc.RaiseInvEvent += Set;

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
                    gc.currentBattle.SetCurrentAttack(item);
                    gc.player.SetAttack(item.Func, "Throw");

                    if (!gc.cheatInfiniteItems)
                    {
                        gc.TakeItem(slot);
                    }
                }
                else if (gc.currentItem.name == item.name)
                {
                    gc.currentBattle.NullCurrentAttackPreview();
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
                    gc.currentBattle.NullCurrentAttackPreview();
                }
            }
        }
    }

    public void Set()
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
