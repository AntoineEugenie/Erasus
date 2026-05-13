using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Quest
{
    public string questName;

    // NOUVEAU : TextArea agrandit la case de texte dans l'Inspecteur Unity
    [TextArea(2, 4)]
    public string description;

    // NOUVEAU : On demande directement ton composant Item (le Prefab) au lieu d'un texte
    public List<Item> acceptedItems;

    public bool isCompleted = false;
}

public class MailboxQuestSystem : MonoBehaviour, IRaycastable
{
    [Header("UI et Quêtes")]
    public GameObject questUIPanel;
    public List<Quest> availableQuests;

    [Header("Génération UI")]
    public GameObject questButtonPrefab;
    public Transform questListContainer;

    [Header("Animation d'envol")]
    public GameObject flyingSpritePrefab;
    public Transform spawnPoint;

    private void Start()
    {
        if (questUIPanel != null)
            questUIPanel.SetActive(false);
    }

    public void OpenQuestMenu()
    {
        questUIPanel.SetActive(true);
        RefreshQuestUI();
    }

    public void CloseQuestMenu()
    {
        questUIPanel.SetActive(false);
    }

    void RefreshQuestUI()
    {
        // 1. On nettoie l'ancienne liste
        foreach (Transform child in questListContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. On génère un texte cliquable pour chaque quête non terminée
        for (int i = 0; i < availableQuests.Count; i++)
        {
            Quest quest = availableQuests[i];

            if (quest.isCompleted) continue;

            GameObject newBtn = Instantiate(questButtonPrefab, questListContainer);

            // --- NOUVEAU : Construction du texte avec Description et nom du Prefab ---
            string texteQuete = $"<b>{quest.questName}</b>\n";

            // On ajoute la description si elle n'est pas vide
            if (!string.IsNullOrEmpty(quest.description))
            {
                texteQuete += $"<size=90%><i>{quest.description}</i></size>\n";
            }

            texteQuete += "<size=80%>Requis : ";

            for (int j = 0; j < quest.acceptedItems.Count; j++)
            {
                // On récupère le nom depuis la data du Prefab (comme dans ton Inventory.cs)
                if (quest.acceptedItems[j] != null && quest.acceptedItems[j].data != null)
                {
                    texteQuete += quest.acceptedItems[j].data.itemName;
                }

                if (j < quest.acceptedItems.Count - 1) texteQuete += ", ";
            }
            texteQuete += "</size>";

            TextMeshProUGUI btnText = newBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = texteQuete;
            }

            // 3. On relie le clic
            int questIndex = i;
            newBtn.GetComponent<Button>().onClick.AddListener(() => TryCompleteQuest(questIndex));
        }
    }

    public void TryCompleteQuest(int questIndex)
    {
        if (questIndex < 0 || questIndex >= availableQuests.Count) return;

        Quest selectedQuest = availableQuests[questIndex];

        if (selectedQuest.isCompleted) return;

        Inventory playerInventory = GameManager.instance.playerController.inventory;

        // NOUVEAU : On vérifie les items à partir des Prefabs
        foreach (Item itemPrefab in selectedQuest.acceptedItems)
        {
            if (itemPrefab == null || itemPrefab.data == null) continue;

            string itemNeeded = itemPrefab.data.itemName;

            for (int i = 0; i < playerInventory.slots.Count; i++)
            {
                Inventory.Slot slot = playerInventory.slots[i];

                if (slot.itemName == itemNeeded && slot.count > 0)
                {
                    playerInventory.Remove(i);
                    selectedQuest.isCompleted = true;

                    CloseQuestMenu();
                    TriggerFlyingAnimation();
                    return;
                }
            }
        }
        Debug.Log("Tu n'as pas les items requis pour cette quête.");
    }

    private void TriggerFlyingAnimation()
    {
        if (flyingSpritePrefab != null && spawnPoint != null)
        {
            Instantiate(flyingSpritePrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    public void OnHitByRaycast()
    {
        OpenQuestMenu();
    }
}