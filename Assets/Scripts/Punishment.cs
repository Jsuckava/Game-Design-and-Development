using System;
using UnityEngine;

public class Punishment
{
    public string title;
    public string description;
    public Sprite cardArt;
    public bool isMinigame;
    public Action<PlayerStats> onPunishmentSelected; 

    public Punishment(string title, string description, Sprite cardArt, bool newIsMinigame, Action<PlayerStats> onPunishmentSelected)
    {
        this.title = title;
        this.description = description;
        this.cardArt = cardArt; 
        isMinigame = newIsMinigame;
        this.onPunishmentSelected = onPunishmentSelected;
    }
}