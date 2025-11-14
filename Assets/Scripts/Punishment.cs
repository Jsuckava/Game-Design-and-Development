using System;
using UnityEngine;

public class Punishment
{
    public string title;
    public string description;
    public Sprite cardArt; 
    public Action<PlayerStats> onPunishmentSelected; 

    public Punishment(string title, string description, Sprite cardArt, Action<PlayerStats> onPunishmentSelected)
    {
        this.title = title;
        this.description = description;
        this.cardArt = cardArt; 
        this.onPunishmentSelected = onPunishmentSelected;
    }
}