using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class TransformComponent : MonoBehaviour
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.position = transform.position;
        this.rotation = transform.eulerAngles;
        this.scale = transform.localScale;
    }
}
