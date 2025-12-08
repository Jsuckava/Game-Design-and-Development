using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class PunishmentDeck : MonoBehaviour
{
    [HideInInspector]
    public List<Punishment> deck = new List<Punishment>();
    public DisasterManager disasterManager; 
    public MinigameManager minigameManager;
    
    public Sprite elNinoArt, tsunamiArt, tornadoArt, floodArt, trashArt, typhoonArt, volcanicEruptionArt, landslideArt, earthquakeArt, pestArt, leptospirosisArt, sunogArt, covid19Art, tuberculosisArt, hypertensionArt, dengueArt;
    public Sprite bambooArt, nipaArt, hardwoodArt, sawaliArt;
    public Sprite bugtongArt, xoxArt, matchaPickerArt;
    public Sprite talongArt, singkamasArt, maniArt, kundolArt, patolaArt, upoArt, kalabasaArt, labanosArt, MustasaArt, luyaArt, ampalayaArt, sitawArt, batawArt, pataniArt, sigarilyasArt, sibuyasArt, bawangArt, carrotsArt, patatasArt;
    public Sprite kamatisArt, blueberryArt, manggaArt, mansanasArt, pinaArt, orangeArt, mangostenArt, lemonArt, longanArt, loquatArt;

    private List<Punishment> disasterCards = new List<Punishment>();
    private List<Punishment> materialCards = new List<Punishment>();
    private List<Punishment> minigameCards = new List<Punishment>();
    private List<Punishment> cropCards = new List<Punishment>();

    void Awake()
    {
        if (disasterManager == null) disasterManager = FindFirstObjectByType<DisasterManager>();
        if (minigameManager == null) minigameManager = FindFirstObjectByType<MinigameManager>();
        
        InitializeDeck();
    }

    // --- HELPER TO GET ENERGY VALUE ---
    public float GetCropEnergyValue(string cropName)
    {
        float energy = 7f; // Default fallback
        var card = cropCards.FirstOrDefault(c => c.itemType == cropName);
        if (card != null)
        {
            energy = card.value;
        }
        return energy;
    }

    private void InitializeDeck()
    {
        disasterCards.Add(new Punishment("El Niño", "A long period of extreme heat... (All players -5)", elNinoArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "El Niño"); }));
        disasterCards.Add(new Punishment("Tsunami", "A giant sea wave... (All players -5)", tsunamiArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "Tsunami"); }));
        disasterCards.Add(new Punishment("Tornado", "A violent rotating windstorm... (All players -5)", tornadoArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "Tornado"); }));
        disasterCards.Add(new Punishment("Landslide", "Sudden collapse of soil... (All players -5 and -2 Bamboo, -2 Hardwood)", landslideArt, false, (player) => { player.ChangeBamboo(-2); player.ChangeHardwood(-2); disasterManager.ApplyGlobalEnergyLoss(-5f, "Landslide"); }));
        disasterCards.Add(new Punishment("Earthquake", "Sudden shaking of the ground... (All players -5)", earthquakeArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "Earthquake"); }));
        disasterCards.Add(new Punishment("Flood", "Rising waters... (All players -3)", floodArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Flood"); }));
        disasterCards.Add(new Punishment("Trash", "Piled-up waste... (All players -3)", trashArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Trash"); }));
        disasterCards.Add(new Punishment("Typhoon", "Strong winds... (All players -3)", typhoonArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Typhoon"); }));
        disasterCards.Add(new Punishment("Volcanic Eruption", "Explosive lava... (All players -3)", volcanicEruptionArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Volcanic Eruption"); }));
        disasterCards.Add(new Punishment("Pest", "An Infestation... (-2 Bamboo, -2 Hardwood)", pestArt, false, (player) => { player.ChangeBamboo(-2); player.ChangeHardwood(-2); }));
        disasterCards.Add(new Punishment("Sunog (Fire)", "Sudden blaze... (All players -3)", sunogArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Sunog (Fire)"); }));
        disasterCards.Add(new Punishment("COVID-19", "A contagious virus... (All players -3)", covid19Art, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "COVID-19"); }));
        disasterCards.Add(new Punishment("Tuberculosis", "A respiratory disease... (Current player -3)", tuberculosisArt, false, (player) => { player.ChangeEnergy(-3f); }));
        disasterCards.Add(new Punishment("Leptospirosis", "A waterborne disease... (-3)", leptospirosisArt, false, (player) => { player.ChangeEnergy(-3); }));
        disasterCards.Add(new Punishment("Hypertension", "A health condition... (-3)", hypertensionArt, false, (player) => { player.ChangeEnergy(-3); }));
        disasterCards.Add(new Punishment("Dengue", "A mosquito-borne illness... (-3)", dengueArt, false, (player) => { player.ChangeEnergy(-3); }));

        materialCards.Add(new Punishment("Bamboo Found!", "You collected 3 Bamboo.", bambooArt, false, (player) => { player.ChangeBamboo(3); }, itemType: "Bamboo", quantity: 3));
        materialCards.Add(new Punishment("Nipa Leaves Found!", "You collected 3 Nipa Leaves.", nipaArt, false, (player) => { player.ChangeNipa(3); }, itemType: "NipaLeaves", quantity: 3));
        materialCards.Add(new Punishment("Hardwood Found!", "You collected 2 Hardwood.", hardwoodArt, false, (player) => { player.ChangeHardwood(2); }, itemType: "Hardwood", quantity: 2));
        materialCards.Add(new Punishment("Sawali Panel Found!", "You collected 1 Sawali Panels.", sawaliArt, false, (player) => { player.ChangeSawali(1); }, itemType: "Sawali", quantity: 1));

        // +7 Energy Crops
        cropCards.Add(new Punishment("Talong Harvest!", "You harvested Talong and gained +7 Energy!", talongArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Talong", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Singkamas Harvest!", "You harvested Singkamas and gained +7 Energy!", singkamasArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Singkamas", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Mani Harvest!", "You harvested Mani and gained +7 Energy!", maniArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Mani", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Kundol Harvest!", "You harvested Kundol and gained +7 Energy!", kundolArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Kundol", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Patola Harvest!", "You harvested Patola and gained +7 Energy!", patolaArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Patola", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Upo Harvest!", "You harvested Upo and gained +7 Energy!", upoArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Upo", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Kalabasa Harvest!", "You harvested Kalabasa and gained +7 Energy!", kalabasaArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Kalabasa", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Labanos Harvest!", "You harvested Labanos and gained +7 Energy!", labanosArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Labanos", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Mustasa Harvest!", "You harvested Mustasa and gained +7 Energy!", MustasaArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Mustasa", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Luya Harvest!", "You harvested Luya and gained +7 Energy!", luyaArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Luya", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Ampalaya Harvest!", "You harvested Ampalaya and gained +7 Energy!", ampalayaArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Ampalaya", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Sitaw Harvest!", "You harvested Sitaw and gained +7 Energy!", sitawArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Sitaw", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Bataw Harvest!", "You harvested Bataw and gained +7 Energy!", batawArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Bataw", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Patani Harvest!", "You harvested Patani and gained +7 Energy!", pataniArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Patani", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Sigarilyas Harvest!", "You harvested Sigarilyas and gained +7 Energy!", sigarilyasArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Sigarilyas", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Sibuyas Harvest!", "You harvested Sibuyas and gained +7 Energy!", sibuyasArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Sibuyas", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Bawang Harvest!", "You harvested Bawang and gained +7 Energy!", bawangArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Bawang", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Carrots Harvest!", "You harvested Carrots and gained +7 Energy!", carrotsArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Carrots", quantity: 1, value: 7f));
        cropCards.Add(new Punishment("Patatas Harvest!", "You harvested Patatas and gained +7 Energy!", patatasArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Patatas", quantity: 1, value: 7f));

        // +5 Energy Crops (Fixed values)
        cropCards.Add(new Punishment("Kamatis Harvest!", "You harvested Kamatis and gained +5 Energy!", kamatisArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Kamatis", quantity: 1, value: 5f));
        cropCards.Add(new Punishment("Blueberry Harvest!", "You harvested Blueberry and gained +5 Energy!", blueberryArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Blueberry", quantity: 1, value: 5f));
        cropCards.Add(new Punishment("Mangga Harvest!", "You harvested Mangga and gained +5 Energy!", manggaArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Mangga", quantity: 1, value: 5f));
        cropCards.Add(new Punishment("Mansanas Harvest!", "You harvested Mansanas and gained +5 Energy!", mansanasArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Mansanas", quantity: 1, value: 5f));
        cropCards.Add(new Punishment("Pina Harvest!", "You harvested Pina and gained +5 Energy!", pinaArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Pina", quantity: 1, value: 5f));
        cropCards.Add(new Punishment("Orange Harvest!", "You harvested Orange and gained +5 Energy!", orangeArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Orange", quantity: 1, value: 5f));
        cropCards.Add(new Punishment("Mangosten Harvest!", "You harvested Mangosten and gained +5 Energy!", mangostenArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Mangosten", quantity: 1, value: 5f));
        cropCards.Add(new Punishment("Lemon Harvest!", "You harvested Lemon and gained +5 Energy!", lemonArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Lemon", quantity: 1, value: 5f));
        cropCards.Add(new Punishment("Longan Harvest!", "You harvested Longan and gained +5 Energy!", longanArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Longan", quantity: 1, value: 5f));
        cropCards.Add(new Punishment("Loquat Harvest!", "You harvested Loquat and gained +5 Energy!", loquatArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Loquat", quantity: 1, value: 5f));
        
        minigameCards.Add(new Punishment("Bugtong Minigame", "Play a riddle minigame to win rewards.", bugtongArt, true, (player) => { minigameManager.StartBugtongMinigame(player); }, itemType: "Minigame", quantity: 0));
        minigameCards.Add(new Punishment("TicTacToe Minigame", "Play a tic-tac-toe minigame to win rewards.", xoxArt, true, (player) => { minigameManager.StartXoxMinigame(player); }, itemType: "Minigame", quantity: 0));
        minigameCards.Add(new Punishment("MatchPick Minigame", "Play a matching card picking minigame to win rewards.", matchaPickerArt, true, (player) => { minigameManager.StartMatchaPickerMinigame(player); }, itemType: "Minigame", quantity: 0));
    }

    public List<Punishment> GetRandomMiniGameCards(int count)
    {
        return GetRandomCards(minigameCards, count);
    }

    public List<Punishment> GetRandomDisasterCards(int count)
    {
        return GetRandomCards(disasterCards, count);
    }
    
    public List<Punishment> GetRandomMaterialCards(int count)
    {
        return GetRandomCards(materialCards, count);
    }
    
    public List<Punishment> GetRandomCropCards(int count)
    {
        return GetRandomCards(cropCards, count);
    }

    private List<Punishment> GetRandomCards(List<Punishment> sourceList, int count)
    {
        List<Punishment> shuffledDeck = new List<Punishment>(sourceList);
        for (int i = 0; i < shuffledDeck.Count; i++)
        {
            Punishment temp = shuffledDeck[i];
            int randomIndex = Random.Range(i, shuffledDeck.Count);
            shuffledDeck[i] = shuffledDeck[randomIndex];
            shuffledDeck[randomIndex] = temp;
        }
        return shuffledDeck.Take(count).ToList();
    }
}