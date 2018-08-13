using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Buildings do the following:
// 1. Keeping track of population density
// 2. How many people are inside of it
// 3. Max Population for that building.
// 4. Hookups for water, power
// 5. How much water is being drawn, power is being draw, etc. (resource usage)
// 6. Goals it is associated with:
//    - Each morning, units go from one building to a list of possible destinations. 
// 

// Resources calculated with the following factors in mind:
// - Density
// - How many people are presently in the building
// - A floor on usage which is per zoning level

// Zoning and density are different
// - A building can exist within a zoned tile (TileType)
// - A building has a density (e.g. it has a ton of people).

public class BuildingTile {
    // The manager class associated with building state.
    public BuildingManager buildingManager;

    // Where the building is located in space.
    public Location location;

    // The building that this building tile represents.
    public Building building;
}

