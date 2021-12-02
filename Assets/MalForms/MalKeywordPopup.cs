//The Keyword form that appears in the UI as a popup menu
//Created by James Vanderhyde, 1 December 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mal;

public class MalKeywordPopup : MalForm
{
    [SerializeField]
    public string value;

    void Awake()
    {
    }

    void Start()
    {
        this.UpdateColor();

        //Update the text
        TMP_Dropdown d = GetComponentInChildren<TMP_Dropdown>();
        int index = -1, i = 0;
        foreach (TMP_Dropdown.OptionData option in d.options)
        {
            if (option.text.Equals(this.value))
                index = i;
            i++;
        }
        if (index != -1)
            d.SetValueWithoutNotify(index);
        else
        {
            d.options.Add(new TMP_Dropdown.OptionData(this.value));
            d.SetValueWithoutNotify(d.options.Count-1);
        }
        d.RefreshShownValue();
    }

    public override types.MalVal read_form()
    {
        return types.MalKeyword.keyword(":"+value);
    }

    private void UpdateColor()
    {
        //Pick a color for this atom, based on the value
        Image im = GetComponent<Image>();
        if (im)
        {
            int hue = 0;
            foreach (char c in value.ToCharArray())
                hue += (int)c;
            im.color = Color.HSVToRGB((hue % 36) / 36f, 0.4f, 0.7f);
        }
    }

    public void ChangeValueFromItem()
    {
        TMP_Dropdown d = GetComponentInChildren<TMP_Dropdown>();
        this.value = d.options[d.value].text;
        this.UpdateColor();
    }

}
