using System;
using System.Collection.Generic;
using UnityEngine;

//functios para sa inventory
//check comments para sa future documentation
//ask yuehan or patrick if there is question or confusion
[CreateAssetMenu(menuName = "Kubo")]
public class InventorySystem : ScriptableObject {
     public Action<BahayKuboParts> OnPartAdded;   //fired whenever a new event or new unique parts sa bahay kubo

      [SerializeField] private List<BahayKuboPart> owned = new();  //store parts, yung mga meron na

     //check if player already has the acquired  part
     public bool Has(BahayKuboParts part) => owned.Contains(part);
     public IReadOnlyList<BahayKuboParts> All => owned; //read only view, good for saving

     //add the acquired part if the player doesnt have it yet
     public bool Add(BahayKuboParts part) {
          if (owned.Contains(part)) return false; //dont give parts
          owned.Add(part);
          OnPartAdded?.Invoke(part); //fire the remote or whatever it is, dunno man kayo na bahala
          return true;
     }

     //check if player has complete the build
     public bool IsComplete() {
          foreach (BahayKuboParts p in Enum.GetValues(typeof(BahayKuboParts)))
               if (!owned.Contains(p)) return false;
          return true;
     }

     //funcion for clearing the inventory, like for example pag gagawa ng new round
     public void clear() => owned.Clear();

}
