using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour {
    public int slot;

    private GameController gc;
    private Image icon;
    private Text text;

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

        if (gc.currentPlayerAttack == null)
        {
            if (gc.inventory[slot] != null)
            {
                if (gc.inventory[slot][0].type != ItemType.Ingredient && gc.currentBattle.currentItem == null)
                {
                    gc.currentPlayerAttack = new PlayerAttack(gc.inventory[slot][0]);
                    if (!gc.cheatInfiniteItems)
                    {
                        gc.TakeItem(slot);
                    }
                }
                else
                {
                    gc.currentBattle.NullCurrentAttackPreview();
                }
            }
        }
        else
        {
            if (gc.inventory[slot] == null ||
                gc.inventory[slot].Count < gc.inventory[slot][0].stackSize)
            {
                gc.GiveItem(slot, gc.currentPlayerAttack.currentItem);
                gc.currentBattle.NullCurrentAttackPreview();
                gc.currentBattle.AttackPreview();
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
