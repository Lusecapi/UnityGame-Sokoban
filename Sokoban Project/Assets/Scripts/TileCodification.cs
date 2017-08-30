using UnityEngine;
using System.Collections;

/// <summary>
/// Has all the info of the Tile like size, code
/// </summary>
public class TileCodification {

    private static float tileSize = 1.28f;

    public static float TileSize
    {
        get
        {
            return tileSize;
        }
    }

    public static string getTileCode(string tileName)
    {
        if (tileName == "Green Ground")
        {
            return "g00";
        }
        else
            if(tileName == "Grey Ground")
        {
            return "g01";
        }
        else
            if(tileName == "Brown Ground")
        {
            return "g02";
        }
        else
            if(tileName == "Brick Wall")
        {
            return "w00";
        }
        else
            if(tileName == "Red Wall")
        {
            return "w01";
        }
        else
            if(tileName == "Grey Wall")
        {
            return "w02";
        }
        else
            if(tileName == "Brown Wall")
        {
            return "w03";
        }
        else
            if(tileName == "Blue Crate")
        {
            return "c00";
        }
        else
            if(tileName == "Brown Crate")
        {
            return "c01";
        }
        else
            if(tileName == "Green Crate")
        {
            return "c02";
        }
        else
            if(tileName == "Grey Crate")
        {
            return "c03";
        }
        else
            if(tileName == "Red Crate")
        {
            return "c04";
        }
        else
            if(tileName == "Blue Crate Point")
        {
            return "cp00";
        }
        else
            if(tileName == "Brown Crate Point")
        {
            return "cp01";
        }
        else
            if(tileName == "Green Crate Point")
        {
            return "cp02";
        }
        else
            if(tileName == "Grey Crate Point")
        {
            return "cp03";
        }
        else
            if(tileName == "Red Crate Point")
        {
            return "cp04";
        }
        else
            if(tileName == "Spawn Point")
        {
            return "e00";
        }
        return null;
    }


    /// <summary>
    /// To get the specified tile name
    /// </summary>
    /// <param name="tileCode">The code of the Tile</param>
    /// <returns>The name of the tile</returns>
    public static string getTileName(string tileCode)
    {
        if (tileCode == "g00")
        {
            return "Green Ground";
        }
        else
            if (tileCode == "g01")
        {
            return "Grey Ground";
        }
        else
            if (tileCode == "g02")
        {
            return "Brown Ground";
        }
        else
            if (tileCode == "w00")
        {
            return "Brick Wall";
        }
        else
            if (tileCode == "w01")
        {
            return "Red Wall";
        }
        else
            if (tileCode == "w02")
        {
            return "Grey Wall";
        }
        else
            if (tileCode == "w03")
        {
            return "Brown Wall";
        }
        else
            if (tileCode == "c00")
        {
            return "Blue Crate";
        }
        else
            if (tileCode == "c01")
        {
            return "Brown Crate";
        }
        else
            if (tileCode == "c02")
        {
            return "Green Crate";
        }
        else
            if (tileCode == "c03")
        {
            return "Grey Crate";
        }
        else
            if (tileCode == "c04")
        {
            return "Red Crate";
        }
        else
            if (tileCode == "cp00")
        {
            return "Blue Crate Point";
        }
        else
            if (tileCode == "cp01")
        {
            return "Brown Crate Point";
        }
        else
            if (tileCode == "cp02")
        {
            return "Green Crate Point";
        }
        else
            if (tileCode == "cp03")
        {
            return "Grey Crate Point";
        }
        else
            if (tileCode == "cp04")
        {
            return "Red Crate Point";
        }
        else
            if (tileCode == "e00")
        {
            return "Spawn Point";
        }
        return null;
    }
}
