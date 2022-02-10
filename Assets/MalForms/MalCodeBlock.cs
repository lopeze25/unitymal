//A block of typed MAL code to execute
//Created by James Vanderhyde, 9 February 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;
using System.Text;

public class MalCodeBlock : MalForm
{
    public override types.MalVal read_form()
    {
        string code = this.transform.GetChild(0).GetComponentInChildren<TMPro.TMP_InputField>().text;
        types.MalVal codeBlock = reader.read_str("(code-block " + code + ")");

        types.MalList highlight = new types.MalList();
        highlight.cons(codeBlock);
        highlight.cons(new types.MalObjectReference(this.gameObject)); //line of code to highlight
        highlight.cons(new Highlight());

        return highlight;
    }

    public override void setChildForms(List<MalForm> children)
    {
        StringBuilder sb = new StringBuilder();
        foreach (MalForm child in children)
        {
            string childCode = printer.pr_str(Highlight.removeHighlights(child.read_form()));
            sb.Append(childCode);
            sb.Append(System.Environment.NewLine);

            child.transform.SetParent(null);
            GameObject.Destroy(child.gameObject);
        }
        this.transform.GetChild(0).GetComponentInChildren<TMPro.TMP_InputField>().text = sb.ToString();
    }
}
