using UnityEngine;

public class WorldBottom : MonoBehaviour
{
    // This method is called when another collider enters the trigger
    private void OnTriggerEnter(Collider malObject)
    {
        Destroy(malObject.gameObject);
    }
}