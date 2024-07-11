//Part of the programming UI gallery that can add names
//Created by James Vanderhyde, 3 July 2024

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Mal;

public class NameShelf : MonoBehaviour
{
    [SerializeField]
    private GameObject symbolForm;
    private Dictionary<string,GameObject> nameList = new();
    private List<string> userDefs = new List<string>();
    private Vector3 formPosition = new Vector3(12,-200,0);

    void Start()
    {
        this.LoadNamesAndDefs();
    }

    public void AddToShelf(string name, string defcode)
    {
        if (!this.nameList.ContainsKey(name))
        {
            GameObject nameForm = GameObject.Instantiate(symbolForm,this.transform);
            nameForm.transform.localPosition = formPosition;
            formPosition.y -= 55;
            if (formPosition.y < -750)
            {
                formPosition.y += 750-165;
                formPosition.x += 200;
            }
            nameForm.GetComponent<MalSymbol>().SetSymbolName(name);
            this.nameList.Add(name,nameForm);
        }
        userDefs.Add(defcode);
    }

    public string SaveDefs()
    {
        StringBuilder sb = new StringBuilder();
        foreach (string defForm in this.userDefs)
        {
            sb.Append(defForm);
            sb.Append('\u001e');
        }
        return sb.ToString();
    }

    private void LoadNamesAndDefs()
    {
        SymbolEnvironment env = this.GetComponentInParent<DollhouseProgram>().GetComponentInChildren<SymbolEnvironment>();

        string[] defs = this.GetComponentInParent<SaveLoad>().GetDefs().Split('\u001e');
        foreach (string def in defs)
        {
            if (!string.IsNullOrWhiteSpace(def))
            {
                //Read and evaluate the string
                types.MalVal defCode = reader.read_str(def);
                types.MalVal defList = evaluator.eval_ast(defCode,env.environment);

                //Put the symbol in the shelf
                types.MalSymbol symbol = (defCode as types.MalList).rest().first() as types.MalSymbol;
                this.AddToShelf(symbol.name,def);
            }
        }
    }

}
