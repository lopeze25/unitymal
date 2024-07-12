//Deletes names from the shelf when a define form is put in the trash
//Created by James Vanderhyde, 11 July 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashDefine : MonoBehaviour
{
    private NameShelf nameShelf;

    void Start()
    {
        this.nameShelf = this.GetComponentInParent<DollhouseProgramUI>().GetComponentInChildren<NameShelf>();
    }

    public void CheckDefinePutInTrash(Transform droppedObject)
    {
        MalDefForm defForm = droppedObject.GetComponent<MalDefForm>();
        if (defForm)
        {
            this.nameShelf.RemoveFromShelf(defForm.GetSymbolName());
        }
    }
}
