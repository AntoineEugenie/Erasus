using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Manager;
using UnityEngine;

[SuppressMessage("ReSharper", "CheckNamespace")]
[SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
public class PotionManager : MonoBehaviour
{
    public ItemData[] potionsLibrary;
    public TextAsset recipesJson;
    public Inventory playerInventory;
    public ItemManager itemManager;
    public PlayerController playerController;
    private List<PotionRecipe> _recipes = new();
    private Dictionary<string, ItemData> _nameToPotionDict = new();
    

    private void Awake()
    {
        // Charger les recettes depuis le JSON
        if (recipesJson != null)
        {
            PotionRecipeList loaded = JsonUtility.FromJson<PotionRecipeList>(recipesJson.text);
            _recipes = loaded.recipes;
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun fichier JSON de recettes assigné !");
        }
        
        // Remplir le dictionnaire ItemData des potions
        foreach (ItemData potionData in potionsLibrary)
        {
            if (!_nameToPotionDict.ContainsKey(potionData.itemName))
            {
                _nameToPotionDict.Add(potionData.itemName, potionData);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            AddRecipeToInventory("Galéodia", "Héolia");
        }
    }
    
    // 🔹 Transmet la potion obtenue après le mélange de deux plantes
    public string GetRecipe(string plantA, string plantB)
    {
        string a = plantA.ToLower();
        string b = plantB.ToLower();
        
        foreach (var recipe in _recipes)
        {
            if (recipe == null || recipe.plantA == null || recipe.plantB == null)
                continue;

            // Si la recette correspond
            if (recipe.plantA.ToLower() == a && recipe.plantB.ToLower() == b)
            {
                return recipe.resultPotion;
            }
        }

        // Si on sort de la boucle → aucune recette trouvée
        return "Elixir of Socks";
    }
    
    public void AddRecipeToInventory(string plantA, string plantB)
    {
        // 1. Récupérer le nom de la potion (ex : "Potion1")
        string recipeName = GetRecipe(plantA, plantB);
        Debug.Log($"La recette de plante obtenue est {recipeName}");
            
        // 2. Demander à l'ItemManager de fournir le Prefab Item correspondant
        // Vérifiez que l'itemManager est correctement attaché
        itemManager = GameManager.instance.itemManager;
        Item itemPrefab = itemManager.GetItembyName(recipeName);
        

        if (itemPrefab != null)
        {
            // 3. Cloner/Instancier le Prefab Item pour le placer dans la scène
            
            playerInventory = GameManager.instance.playerController.playerInventory;
            playerInventory.Add(itemPrefab);
                
            Debug.Log($"✅ [GameManager] Potion '{recipeName}' ajoutée à l'inventaire");
        }
        else
        {
            Debug.LogError($"❌ [GameManager] Impossible d'instancier l'objet '{recipeName}'. " +
                           "Vérifiez que le Prefab est bien référencé dans l'ItemManager.");
        }
        
        foreach ( var slot in GameManager.instance.craftInventory.slots) // supprime tous les éléments des slots (actuellement 2 slots)
        {
            slot.RemoveItem();
        }
        
        GameManager.instance.inventoryManager.RefreshAllUIs();
    }
}
