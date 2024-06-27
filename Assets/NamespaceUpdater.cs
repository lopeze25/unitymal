using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NamespaceBlockManager : MonoBehaviour
{
    public Button generateButton;
    public TMP_InputField functionInputField;
    public TMP_InputField namespaceInputField;

    void Start()
    {
        generateButton.onClick.AddListener(UpdateNamespaceBlock);
    }

    void UpdateNamespaceBlock()
    {
        string functionName = functionInputField.text;
        // Valdiate the FunctionName(?)


        namespaceInputField.text = functionName;
    }
}