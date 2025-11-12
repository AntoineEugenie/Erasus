using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{
    public ItemData[] potionsLibrary;
    public TextAsset recipesJson;
    private Inventory playerInventory;
    private List<PotionRecipe> recipes = new();
    private Dictionary<string, ItemData> nameToPotionDict = new();

    private void Awake()
    {
        Debug.LogWarning("awake de potion manager enclenché");

        // Charger les potions disponibles dans la bibliothèque
        foreach (ItemData potion in potionsLibrary)
        {
            if (potion == null || string.IsNullOrEmpty(potion.itemName)) continue;
            nameToPotionDict[potion.itemName.ToLower()] = potion;
        }

        // Charger les recettes depuis le JSON
        if (recipesJson != null)
        {
            PotionRecipeList loaded = JsonUtility.FromJson<PotionRecipeList>(recipesJson.text);
            recipes = loaded.recipes;
            Debug.Log($"✅ {recipes.Count} recettes de potions chargées depuis le JSON !");
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun fichier JSON de recettes assigné !");
        }
    }

    // 🔹 Transmet le potion obtenue après le mélange de deux plantes
    public string GetRecipe(string plantA, string plantB)
    {
        string a = plantA.ToLower();
        string b = plantB.ToLower();

        foreach (var recipe in recipes)
        {
            if (recipe.plantA.ToLower() == a && recipe.plantB.ToLower() == b)
                return recipe.resultPotion;
        }
        return "Elixir of socks";
    }

    // 🔹 Ajoute la potion à l’inventaire
    public void AddPotionToInventory(string potionName)
    {
        if (!nameToPotionDict.TryGetValue(potionName.ToLower(), out ItemData data))
        {
            Debug.LogWarning($"Potion '{potionName}' introuvable dans la bibliothèque !");
            return;
        }

        // Crée un nouvel objet potion
        GameObject potionObj = new GameObject(potionName);
        Item newPotion = potionObj.AddComponent<Item>();
        newPotion.data = data;
        newPotion.amount = 1;

        playerInventory.Add(newPotion);
    }
}
