using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class ItemManager : MonoBehaviour
    {
        [Tooltip("Tableau contenant tous les objets (Items) du jeu.")]
        public Item[] items;

        // Dictionnaire pour lier rapidement le nom d'un objet à l'objet lui-même
        private Dictionary<string, Item> nameToItemDict = new Dictionary<string, Item>();

        private void Awake()
        {
            // Sécurité : On s'assure que le tableau n'est pas nul avant d'essayer de le lire
            if (items == null) return;

            // On parcourt chaque item de notre tableau
            foreach (Item item in items)
            {
                AddItem(item);
            }
        }

        private void AddItem(Item item)
        {
            // 1. Vérification : L'élément du tableau est-il vide dans l'Inspecteur Unity ?
            if (item == null)
            {
                Debug.LogWarning("Un emplacement dans le tableau 'items' de l'ItemManager est vide. Pense à le remplir ou à réduire la taille du tableau.");
                return; // On arrête d'exécuter la suite pour cet élément, car il est vide
            }

            // 2. Vérification : Les données (data) de cet item ont-elles été assignées ?
            if (item.data == null)
            {
                Debug.LogWarning($"Attention : L'item attaché à '{item.gameObject.name}' n'a pas de 'data' assigné dans l'Inspecteur !");
                return; // On arrête d'exécuter la suite, car on ne peut pas lire le nom
            }

            // 3. Ajout sécurisé : Si l'item et ses données existent, on l'ajoute au dictionnaire
            // On vérifie d'abord que le dictionnaire ne contient pas déjà ce nom pour éviter les doublons
            if (!nameToItemDict.ContainsKey(item.data.itemName))
            {
                nameToItemDict.Add(item.data.itemName, item);
            }
            else
            {
                Debug.LogWarning($"L'item avec le nom '{item.data.itemName}' existe déjà dans le dictionnaire ! Vérifie qu'il n'y a pas de doublons dans tes données.");
            }
        }

        public Item GetItembyName(string name)
        {
            // On vérifie si la clé existe avant d'essayer de la renvoyer
            if (nameToItemDict.ContainsKey(name))
            {
                return nameToItemDict[name];
            }

            // C'est toujours une bonne pratique de prévenir si une recherche échoue
            Debug.LogWarning($"Impossible de trouver l'item '{name}' dans le dictionnaire.");
            return null;
        }
    }
}