using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class PunishmentDeck : MonoBehaviour
{
    [HideInInspector]
    public List<Punishment> deck = new List<Punishment>();
    
    public DisasterManager disasterManager; 
    
    [Header("Card Art Assets")]
    public Sprite elNinoArt;
    public Sprite tsunamiArt;
    public Sprite tornadoArt;
    public Sprite floodArt;
    public Sprite trashArt;
    public Sprite typhoonArt;
    public Sprite volcanicEruptionArt;
    public Sprite landslideArt;
    public Sprite earthquakeArt;
    public Sprite pestArt;
    public Sprite leptospirosisArt;
    public Sprite sunogArt; 
    public Sprite covid19Art;
    public Sprite tuberculosisArt;
    public Sprite hypertensionArt;
    public Sprite dengueArt;
    
    [Header("Reward Card Art")]
    public Sprite bambooArt;
    public Sprite nipaArt;
    public Sprite hardwoodArt;
    public Sprite sawaliArt;

    private List<Punishment> disasterCards = new List<Punishment>();
    private List<Punishment> materialCards = new List<Punishment>();

    void Awake()
    {
        if (disasterManager == null)
        {
            disasterManager = FindFirstObjectByType<DisasterManager>();
        }
        
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        disasterCards.Add(new Punishment(
            "El Niño", 
            "A long period of extreme heat...", 
            elNinoArt, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "El Niño"); } 
        ));
        disasterCards.Add(new Punishment(
            "Tsunami", 
            "A giant sea wave that hits...", 
            tsunamiArt, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "Tsunami"); } 
        ));
        disasterCards.Add(new Punishment(
            "Tornado", 
            "A violent rotating windstorm...", 
            tornadoArt, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "Tornado"); } 
        ));
        disasterCards.Add(new Punishment(
            "Landslide", 
            "Sudden collapse of soil...", 
            landslideArt, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "Landslide"); } 
        ));
        disasterCards.Add(new Punishment(
            "Earthquake", 
            "Sudden shaking of the ground...", 
            earthquakeArt, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "Earthquake"); } 
        ));
        disasterCards.Add(new Punishment(
            "Flood", 
            "Rising waters that slow down...", 
            floodArt, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Flood"); } 
        ));
        disasterCards.Add(new Punishment(
            "Trash", 
            "Piled-up waste that pollutes...", 
            trashArt, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Trash"); } 
        ));
        disasterCards.Add(new Punishment(
            "Typhoon", 
            "Strong winds and heavy rain...", 
            typhoonArt, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Typhoon"); } 
        ));
        disasterCards.Add(new Punishment(
            "Volcanic Eruption", 
            "Explosive lava and ash clouds...", 
            volcanicEruptionArt, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Volcanic Eruption"); } 
        ));
        disasterCards.Add(new Punishment(
            "Pest", 
            "Infestation that destroys...", 
            pestArt, 
            (player) => { player.ChangeBamboo(-2); player.ChangeHardwood(-2); } 
        ));
        disasterCards.Add(new Punishment(
            "Sunog (Fire)", 
            "Sudden blaze that affects...", 
            sunogArt, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Sunog (Fire)"); } 
        ));
        disasterCards.Add(new Punishment(
            "COVID-19", 
            "A contagious virus that spreads...", 
            covid19Art, 
            (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "COVID-19"); } 
        ));
        disasterCards.Add(new Punishment(
            "Tuberculosis", 
            "A respiratory disease...", 
            tuberculosisArt, 
            (player) => { disasterManager.ApplyTuberculosisDebuff(player, -3f); } 
        ));
        disasterCards.Add(new Punishment(
            "Leptospirosis", 
            "A waterborne disease...", 
            leptospirosisArt, 
            (player) => { player.ChangeEnergy(-3); } 
        ));
        disasterCards.Add(new Punishment(
            "Hypertension", 
            "A health condition that causes...", 
            hypertensionArt, 
            (player) => { player.ChangeEnergy(-3); } 
        ));
        disasterCards.Add(new Punishment(
            "Dengue", 
            "A mosquito-borne illness...", 
            dengueArt, 
            (player) => { player.ChangeEnergy(-3); } 
        ));
        
        materialCards.Add(new Punishment(
            "Bamboo Found!", 
            "You collected 5 Bamboo.", 
            bambooArt, 
            (player) => { player.ChangeBamboo(5); } 
        ));
        materialCards.Add(new Punishment(
            "Nipa Leaves Found!", 
            "You collected 5 Nipa Leaves.", 
            nipaArt, 
            (player) => { player.ChangeNipa(5); } 
        ));
        materialCards.Add(new Punishment(
            "Hardwood Found!", 
            "You collected 5 Hardwood.", 
            hardwoodArt, 
            (player) => { player.ChangeHardwood(5); } 
        ));
        materialCards.Add(new Punishment(
            "Sawali Panel Found!", 
            "You collected 5 Sawali Panels.", 
            sawaliArt, 
            (player) => { player.ChangeSawali(5); } 
        ));
    }

    public List<Punishment> GetRandomDisasterCards(int count)
    {
        List<Punishment> shuffledDeck = new List<Punishment>(disasterCards);
        for (int i = 0; i < shuffledDeck.Count; i++) {
            Punishment temp = shuffledDeck[i];
            int randomIndex = Random.Range(i, shuffledDeck.Count);
            shuffledDeck[i] = shuffledDeck[randomIndex];
            shuffledDeck[randomIndex] = temp;
        }
        return shuffledDeck.Take(count).ToList();
    }
    
    public List<Punishment> GetRandomMaterialCards(int count)
    {
        List<Punishment> shuffledDeck = new List<Punishment>(materialCards);
        for (int i = 0; i < shuffledDeck.Count; i++) {
            Punishment temp = shuffledDeck[i];
            int randomIndex = Random.Range(i, shuffledDeck.Count);
            shuffledDeck[i] = shuffledDeck[randomIndex];
            shuffledDeck[randomIndex] = temp;
        }
        return shuffledDeck.Take(count).ToList();
    }
}