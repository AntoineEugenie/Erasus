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
            Debug.Log($"✅ {_recipes.Count} recettes de potions chargées depuis le JSON !");
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
        Debug.Log($"✅ {_nameToPotionDict.Count} ItemData de potions enregistrées !");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            AddRecipeToInventory("cryolis", "elektra");
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
    
    private void AddRecipeToInventory(string plantA, string plantB)
    {
        // 1. Récupérer le nom de la potion (ex : "Potion1")
        string recipeName = GetRecipe(plantA, plantB);
            
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
    }
    
}
