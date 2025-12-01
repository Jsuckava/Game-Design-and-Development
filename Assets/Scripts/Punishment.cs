using UnityEngine;
using System;
using UnityEngine.Events;

[System.Serializable]
public class Punishment
{
    public string title;
    public string description;
    public Sprite cardArt;
    public bool isMinigame;
    public Action<PlayerStats> onPunishmentSelected;
    public string itemType; 
    public int quantity;   
    public float value;    
    public Punishment(string title, string description, Sprite cardArt, bool isMinigame, Action<PlayerStats> action, string itemType = "", int quantity = 0, float value = 0f)
    {
        this.title = title;
        this.description = description;
        this.cardArt = cardArt;
        this.isMinigame = isMinigame;
        this.onPunishmentSelected = action;
        this.itemType = itemType;
        this.quantity = quantity;
        this.value = value;
    }
}