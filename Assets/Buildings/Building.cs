using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    // The density of the tile. Can be thought of as the zoning for the building.
    public Density density;

    // The current population of the building.
    public int population;

    // The list of other buildings that this building is connected with as 
    // destinations for travel (e.g. commuting).
    public Building[] destinations;

    // The maximum population that the tile can sustain.
    public int PopulationCap
    {
        get
        {
            return -1; // Todo: Calculate this.
        }
    }
}