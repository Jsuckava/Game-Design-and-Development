using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class PunishmentDeck : MonoBehaviour
{
    [HideInInspector]
    public List<Punishment> deck = new List<Punishment>();
    public DisasterManager disasterManager; 
    public MinigameManager minigameManager;
    public BahayKuboTracker bahayKuboTracker; // Reference to Tracker
    
    [Header("Card Art - Disasters")]
    public Sprite elNinoArt, tsunamiArt, tornadoArt, floodArt, trashArt, typhoonArt, volcanicEruptionArt, landslideArt, earthquakeArt, pestArt, leptospirosisArt, sunogArt, covid19Art, tuberculosisArt, hypertensionArt, dengueArt;
    
    [Header("Card Art - Materials")]
    public Sprite bambooArt, nipaArt, hardwoodArt, sawaliArt;
    
    [Header("Card Art - Minigames")]
    public Sprite bugtongArt, xoxArt, matchaPickerArt;
    
    [Header("Card Art - Crops")]
    public Sprite talongArt, singkamasArt, maniArt, kundolArt, patolaArt, upoArt, kalabasaArt, labanosArt, MustasaArt, luyaArt, ampalayaArt, sitawArt, batawArt, pataniArt, sigarilyasArt, sibuyasArt, bawangArt, carrotsArt, patatasArt;
    public Sprite kamatisArt, blueberryArt, manggaArt, mansanasArt, pinaArt, orangeArt, mangostenArt, lemonArt, longanArt, loquatArt;

    [Header("Audio Clips")]
    public AudioClip soundElNino, soundTsunami, soundTornado, soundLandslide, soundEarthquake, soundFlood, soundTrash, soundTyphoon, soundVolcanic, soundPest, soundFire, soundCovid, soundDiseaseGeneric; 
    public AudioClip soundMaterialFound, soundCropHarvest, soundMinigameStart;

    private List<Punishment> disasterCards = new List<Punishment>();
    private List<Punishment> materialCards = new List<Punishment>();
    private List<Punishment> minigameCards = new List<Punishment>();
    private List<Punishment> cropCards = new List<Punishment>();

    void Awake()
    {
        if (disasterManager == null) disasterManager = FindFirstObjectByType<DisasterManager>();
        if (minigameManager == null) minigameManager = FindFirstObjectByType<MinigameManager>();
        if (bahayKuboTracker == null) bahayKuboTracker = FindFirstObjectByType<BahayKuboTracker>();
        
        InitializeDeck();
    }

    public float GetCropEnergyValue(string cropName)
    {
        float energy = 7f; 
        var card = cropCards.FirstOrDefault(c => c.itemType == cropName);
        if (card != null) energy = card.value;
        return energy;
    }

    private void TriggerDestruction(string partName)
    {
        if(bahayKuboTracker != null)
        {
            string msg = bahayKuboTracker.DestroyPart(partName);
            if(!string.IsNullOrEmpty(msg)) Debug.Log(msg);
        }
    }

    private void InitializeDeck()
    {        
        disasterCards.Add(new Punishment("Landslide", "Soil collapse! The Haligi is destroyed! (-5 Energy, -2 Bamboo/Hardwood)", landslideArt, false, (player) => { 
            player.ChangeBamboo(-2); player.ChangeHardwood(-2); 
            disasterManager.ApplyGlobalEnergyLoss(-5f, "Landslide");
            TriggerDestruction("Haligi"); 
        }, effectSound: soundLandslide));

        disasterCards.Add(new Punishment("Earthquake", "Ground shaking! The Pader cracked and fell! (-5 Energy)", earthquakeArt, false, (player) => { 
            disasterManager.ApplyGlobalEnergyLoss(-5f, "Earthquake"); 
            TriggerDestruction("Pader");
        }, effectSound: soundEarthquake));

        disasterCards.Add(new Punishment("Typhoon", "Strong winds blew the Bubong away! (-3 Energy)", typhoonArt, false, (player) => { 
            disasterManager.ApplyGlobalEnergyLoss(-3f, "Typhoon"); 
            TriggerDestruction("Bubong");
        }, effectSound: soundTyphoon));

        disasterCards.Add(new Punishment("Pest", "Termites ate the Sahig! (-2 Bamboo, -2 Hardwood)", pestArt, false, (player) => { 
            player.ChangeBamboo(-2); player.ChangeHardwood(-2); 
            TriggerDestruction("Sahig");
        }, effectSound: soundPest));

        disasterCards.Add(new Punishment("Sunog (Fire)", "Fire! The Pader burned down! (-3 Energy)", sunogArt, false, (player) => { 
            disasterManager.ApplyGlobalEnergyLoss(-3f, "Sunog (Fire)"); 
            TriggerDestruction("Pader");
        }, effectSound: soundFire));

        disasterCards.Add(new Punishment("El Niño", "A long period of extreme heat... (All players -5)", elNinoArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "El Niño"); }, effectSound: soundElNino));
        disasterCards.Add(new Punishment("Tsunami", "A giant sea wave... (All players -5)", tsunamiArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "Tsunami"); }, effectSound: soundTsunami));
        disasterCards.Add(new Punishment("Tornado", "A violent rotating windstorm... (All players -5)", tornadoArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-5f, "Tornado"); }, effectSound: soundTornado));
        disasterCards.Add(new Punishment("Flood", "Rising waters... (All players -3)", floodArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Flood"); }, effectSound: soundFlood));
        disasterCards.Add(new Punishment("Trash", "Piled-up waste... (All players -3)", trashArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Trash"); }, effectSound: soundTrash));
        disasterCards.Add(new Punishment("Volcanic Eruption", "Explosive lava... (All players -3)", volcanicEruptionArt, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "Volcanic Eruption"); }, effectSound: soundVolcanic));
        disasterCards.Add(new Punishment("COVID-19", "A contagious virus... (All players -3)", covid19Art, false, (player) => { disasterManager.ApplyGlobalEnergyLoss(-3f, "COVID-19"); }, effectSound: soundCovid));
        
        // Diseases
        disasterCards.Add(new Punishment("Tuberculosis", "Respiratory disease... (-3 Energy)", tuberculosisArt, false, (player) => { player.ChangeEnergy(-3f); }, effectSound: soundDiseaseGeneric));
        disasterCards.Add(new Punishment("Leptospirosis", "Waterborne disease... (-3 Energy)", leptospirosisArt, false, (player) => { player.ChangeEnergy(-3); }, effectSound: soundDiseaseGeneric));
        disasterCards.Add(new Punishment("Hypertension", "Health condition... (-3 Energy)", hypertensionArt, false, (player) => { player.ChangeEnergy(-3); }, effectSound: soundDiseaseGeneric));
        disasterCards.Add(new Punishment("Dengue", "Mosquito-borne illness... (-3 Energy)", dengueArt, false, (player) => { player.ChangeEnergy(-3); }, effectSound: soundDiseaseGeneric));

        // Materials
        AddMaterialsAndCrops(); 
        AddMinigames();
    }

    private void AddMaterialsAndCrops()
    {
        materialCards.Add(new Punishment("Bamboo Found!", "You collected 3 Bamboo.", bambooArt, false, (player) => { player.ChangeBamboo(3); }, itemType: "Bamboo", quantity: 3, effectSound: soundMaterialFound));
        materialCards.Add(new Punishment("Nipa Leaves Found!", "You collected 3 Nipa Leaves.", nipaArt, false, (player) => { player.ChangeNipa(3); }, itemType: "NipaLeaves", quantity: 3, effectSound: soundMaterialFound));
        materialCards.Add(new Punishment("Hardwood Found!", "You collected 2 Hardwood.", hardwoodArt, false, (player) => { player.ChangeHardwood(2); }, itemType: "Hardwood", quantity: 2, effectSound: soundMaterialFound));
        materialCards.Add(new Punishment("Sawali Panel Found!", "You collected 1 Sawali Panels.", sawaliArt, false, (player) => { player.ChangeSawali(1); }, itemType: "Sawali", quantity: 1, effectSound: soundMaterialFound));

        // +7 Energy
        cropCards.Add(new Punishment("Talong Harvest!", "+7 Energy", talongArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Talong", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Singkamas Harvest!", "+7 Energy", singkamasArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Singkamas", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Mani Harvest!", "+7 Energy", maniArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Mani", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Kundol Harvest!", "+7 Energy", kundolArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Kundol", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Patola Harvest!", "+7 Energy", patolaArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Patola", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Upo Harvest!", "+7 Energy", upoArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Upo", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Kalabasa Harvest!", "+7 Energy", kalabasaArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Kalabasa", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Labanos Harvest!", "+7 Energy", labanosArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Labanos", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Mustasa Harvest!", "+7 Energy", MustasaArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Mustasa", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Luya Harvest!", "+7 Energy", luyaArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Luya", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Ampalaya Harvest!", "+7 Energy", ampalayaArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Ampalaya", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Sitaw Harvest!", "+7 Energy", sitawArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Sitaw", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Bataw Harvest!", "+7 Energy", batawArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Bataw", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Patani Harvest!", "+7 Energy", pataniArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Patani", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Sigarilyas Harvest!", "+7 Energy", sigarilyasArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Sigarilyas", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Sibuyas Harvest!", "+7 Energy", sibuyasArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Sibuyas", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Bawang Harvest!", "+7 Energy", bawangArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Bawang", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Carrots Harvest!", "+7 Energy", carrotsArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Carrots", quantity: 1, value: 7f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Patatas Harvest!", "+7 Energy", patatasArt, false, (player) => { player.ChangeEnergy(7f); }, itemType: "Patatas", quantity: 1, value: 7f, effectSound: soundCropHarvest));

        // +5 Energy
        cropCards.Add(new Punishment("Kamatis Harvest!", "+5 Energy", kamatisArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Kamatis", quantity: 1, value: 5f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Blueberry Harvest!", "+5 Energy", blueberryArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Blueberry", quantity: 1, value: 5f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Mangga Harvest!", "+5 Energy", manggaArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Mangga", quantity: 1, value: 5f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Mansanas Harvest!", "+5 Energy", mansanasArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Mansanas", quantity: 1, value: 5f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Pina Harvest!", "+5 Energy", pinaArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Pina", quantity: 1, value: 5f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Orange Harvest!", "+5 Energy", orangeArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Orange", quantity: 1, value: 5f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Mangosten Harvest!", "+5 Energy", mangostenArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Mangosten", quantity: 1, value: 5f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Lemon Harvest!", "+5 Energy", lemonArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Lemon", quantity: 1, value: 5f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Longan Harvest!", "+5 Energy", longanArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Longan", quantity: 1, value: 5f, effectSound: soundCropHarvest));
        cropCards.Add(new Punishment("Loquat Harvest!", "+5 Energy", loquatArt, false, (player) => { player.ChangeEnergy(5f); }, itemType: "Loquat", quantity: 1, value: 5f, effectSound: soundCropHarvest));
    }

    private void AddMinigames()
    {
        minigameCards.Add(new Punishment("Bugtong Minigame", "Riddle time!", bugtongArt, true, (player) => { minigameManager.StartBugtongMinigame(player); }, itemType: "Minigame", quantity: 0, effectSound: soundMinigameStart));
        minigameCards.Add(new Punishment("TicTacToe Minigame", "Tic-Tac-Toe time!", xoxArt, true, (player) => { minigameManager.StartXoxMinigame(player); }, itemType: "Minigame", quantity: 0, effectSound: soundMinigameStart));
        minigameCards.Add(new Punishment("MatchPick Minigame", "Matching time!", matchaPickerArt, true, (player) => { minigameManager.StartMatchaPickerMinigame(player); }, itemType: "Minigame", quantity: 0, effectSound: soundMinigameStart));
    }

    public List<Punishment> GetRandomMiniGameCards(int count) { return GetRandomCards(minigameCards, count); }
    public List<Punishment> GetRandomDisasterCards(int count) { return GetRandomCards(disasterCards, count); }
    public List<Punishment> GetRandomMaterialCards(int count) { return GetRandomCards(materialCards, count); }
    public List<Punishment> GetRandomCropCards(int count) { return GetRandomCards(cropCards, count); }

    private List<Punishment> GetRandomCards(List<Punishment> sourceList, int count)
    {
        List<Punishment> shuffled = new List<Punishment>(sourceList);
        for (int i = 0; i < shuffled.Count; i++)
        {
            Punishment temp = shuffled[i];
            int rand = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[rand];
            shuffled[rand] = temp;
        }
        return shuffled.Take(count).ToList();
    }
}