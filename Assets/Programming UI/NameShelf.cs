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
    private MalSymbol symbolFormPrefab;
    [SerializeField]
    private MalUserFunctionCall functionCallFormPrefab;
    private Dictionary<string,GameObject> nameList = new();
    private Dictionary<string,List<string>> userDefs = new();
    private Vector3 formPosition = new Vector3(12,-200,0);
    private SymbolEnvironment symbolEnv;

    void Start()
    {
        this.symbolEnv = this.GetComponentInParent<DollhouseProgram>().GetComponentInChildren<SymbolEnvironment>();
        this.LoadNamesAndDefs();
    }

    public void AddToShelf(string name, string defcode)
    {
        types.MalVal symbolValue = evaluator.eval_ast(new types.MalSymbol(name),this.symbolEnv.environment);

        //If it's not on the shelf, add it to the shelf
        if (!this.nameList.ContainsKey(name))
        {
            if (symbolValue is types.FuncClosure)
            {
                //Create and position the symbol
                MalUserFunctionCall nameForm = GameObject.Instantiate(functionCallFormPrefab,this.transform);
                nameForm.transform.localPosition = formPosition;
                nameForm.SetNameAndParameters(name, (symbolValue as types.FuncClosure).unboundSymbols);
                this.nameList.Add(name,nameForm.gameObject);

                //Set up for next time
                formPosition.y -= 105;
            }
            else
            {
                //Create and position the symbol
                MalSymbol nameForm = GameObject.Instantiate(symbolFormPrefab,this.transform);
                nameForm.transform.localPosition = formPosition;
                nameForm.SetSymbolName(name);
                this.nameList.Add(name,nameForm.gameObject);

                //Set up for next time
                formPosition.y -= 55;
            }

            //Wrap to next column
            if (formPosition.y < -750)
            {
                formPosition.y += 750-165;
                formPosition.x += 200;
            }
        }

        //Add it to the record of defines (if it's not identical to the current value)
        if (!userDefs.ContainsKey(name))
            userDefs.Add(name, new List<string>());
        if (userDefs[name].Count==0 || !userDefs[name][^1].Equals(defcode))
            userDefs[name].Add(defcode);
    }

    public void RemoveFromShelf(string name)
    {
        if (this.nameList.ContainsKey(name))
        {
            GameObject nameForm = this.nameList[name];
            GameObject.Destroy(nameForm);
            this.nameList.Remove(name);
        }
    }

    public string SaveDefs()
    {
        StringBuilder sb = new StringBuilder();
        foreach ((string name, GameObject form) in this.nameList)
        {
            sb.Append(name);
            sb.Append('\u001e');
        }
        sb.Append('\u001d');
        foreach ((string name, List<string> defs) in this.userDefs)
        {
            sb.Append(name);
            foreach (string def in defs)
            {
                sb.Append('\u001f');
                sb.Append(def);
            }
            sb.Append('\u001e');
        }
        return sb.ToString();
    }

    private void LoadNamesAndDefs()
    {
        string[] namenamedefs = this.GetComponentInParent<SaveLoad>().GetDefs().Split('\u001d');
        HashSet<string> namesInUse = new(namenamedefs[0].Split('\u001e'));
        string[] namedefs = namenamedefs[1].Split('\u001e');
        foreach (string namedef in namedefs)
        {
            if (!string.IsNullOrWhiteSpace(namedef))
            {
                string[] defs = namedef.Split('\u001f');
                string name = defs[0];
                string def = defs[^1];

                //Save the old definitions
                this.userDefs.Add(name, new List<string>());
                for (int i=1; i<defs.Length; i++)
                    this.userDefs[name].Add(defs[i]);

                if (namesInUse.Contains(name))
                {
                    //Read and evaluate the string
                    types.MalVal defCode = reader.read_str(def);
                    types.MalVal defList = evaluator.eval_ast(defCode,this.symbolEnv.environment);

                    //Put the symbol and the newest definition on the shelf
                    this.AddToShelf(name,def);
                }
            }
        }
    }
}
