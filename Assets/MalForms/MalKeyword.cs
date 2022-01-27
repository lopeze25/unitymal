//The keyword form that appears in the UI
//Created by James Vanderhyde, 27 January 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mal;

public class MalKeyword : MalForm
{
    [SerializeField]
    private string keywordName;

    void Start()
    {
        //Pick a color for this atom, based on the value
        Image im = GetComponent<Image>();
        if (im)
        {
            int hue = 0;
            foreach (char c in this.keywordName.ToCharArray())
                hue += (int)c;
            im.color = Color.HSVToRGB((hue % 36) / 36f, 0.4f, 0.7f);
        }

        //Update the text
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = this.keywordName;
    }

    public string GetKeywordName()
    {
        return this.keywordName;
    }

    public void SetKeywordName(string keywordName)
    {
        this.keywordName = keywordName;
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = keywordName;

        //Tell the block to resize itself
        ContentSizeFitter fitter = text.GetComponent<ContentSizeFitter>();
        if (fitter)
        {
            fitter.SetLayoutHorizontal();
            fitter.SetLayoutVertical();
        }
        ListManagement lm = GetComponentInParent<ListManagement>();
        if (lm)
            lm.AddToList(this.gameObject);
    }

    public override types.MalVal read_form()
    {
        return new types.MalKeyword(this.keywordName);
    }

}
