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
    public float height;

    // The building that this building tile represents.
    public Building building;


    public BuildingType type;
    public Density density;

    System.Random rnd = new System.Random();

    public float GetHeight() {
        switch(density) {
            case Density.SubUrban:
                return height + ((float)rnd.NextDouble() / 3.0f);
            case Density.Urban:
                return height + 1f + ((float)rnd.NextDouble() / 3.0f);
            default:
                return height - 0.75f + ((float)rnd.NextDouble() / 3.0f);
        }
    }
    public Material GetMaterial() {
        switch (type) {
        case BuildingType.Residential:
            switch(density) {
            case Density.Rural:
                return buildingManager.data.lowDensityResidential;
            case Density.SubUrban:
                return buildingManager.data.mediumDensityResidential;
            case Density.Urban:
                return buildingManager.data.highDensityResidential;
            }
            break;
        case BuildingType.Commerce:
            switch(density) {
            case Density.Rural:
                return buildingManager.data.lowDensityCommercial;
            case Density.SubUrban:
                return buildingManager.data.mediumDensityCommercial;
            case Density.Urban:
                return buildingManager.data.highDensityCommercial;
            }
            break;
        case BuildingType.Industrial:
            switch(density) {
            case Density.Rural:
                return buildingManager.data.lowDensityIndustrial;
            case Density.SubUrban:
                return buildingManager.data.mediumDensityIndustrial;
            case Density.Urban:
                return buildingManager.data.highDensityIndustrial;
            }
            break;
        }
        return null;
    }

    public void maybeMakeACar(List<GameObject> buildings) {
        if (GameManager.Instance.RoadManager.data.enableCarSim && rnd.NextDouble() <= .001) {
            var x = (rnd.Next() % 5) + rnd.Next() % 2 + location.row - 3;
            var y = (rnd.Next() % 5) + rnd.Next() % 2 + location.col - 3;
            if (x > 0 && x < 50 && y > 0 && y < 75) {
                var s = GameManager.Instance.RoadManager.tiles[location.row, location.col];
                var e = GameManager.Instance.RoadManager.tiles[y, x];
                GameManager.Instance.RoadManager.makeACarGo(s, e);
            }
        }
    }
}

