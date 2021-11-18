//A component adapter for the Mal environment
//Create by James Vanderhyde, 16 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolEnvironment : MonoBehaviour
{
    public Mal.env.Environment environment;

    void Awake()
    {
        this.environment = new Mal.env.Environment(Mal.env.baseEnvironment, false);
        this.environment.setAll(Dollhouse.control.ns);
        this.environment.setAll(Dollhouse.transformations.ns);
    }
}
