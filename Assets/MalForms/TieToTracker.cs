//A component on a symbol in the scope of a defining form
//Created by James Vanderhyde, 4 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TieToTracker : MonoBehaviour, IBeginDragHandler
{
    private SymbolTracker tracker;

    void Start()
    {
        this.tracker = GetComponentInParent<SymbolTracker>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        tracker.AddToListOfSymbolForms(this);
    }
}
