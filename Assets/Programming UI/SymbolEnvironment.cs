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
        //Create a new environment
        this.environment = new Mal.env.Environment(Mal.env.baseEnvironment, false);

        //Load the Dollhouse symbols
        this.environment.setAll(Dollhouse.entities.ns);
        this.environment.setAll(Dollhouse.control.ns);
        this.environment.setAll(Dollhouse.transformations.ns);
    }
}
