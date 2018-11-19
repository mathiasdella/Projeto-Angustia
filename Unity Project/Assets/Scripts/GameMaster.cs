using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    /// <summary>
    /// Gets the conversion from pixels to world units.
    /// </summary>
    /// <value>The pixel to world ratio.</value>
    public static float PixelToWorld { get { return 1.80f / 64; } }

    /// <summary>
    /// Gets the conversion from world units to pixels.
    /// </summary>
    /// <value>The world to pixel ratio.</value>
    public static float WorldToPixel { get { return 64 / 1.80f; } }


    public PlayerController player;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();

            if (player == null)
            {
                Debug.LogError("Player not found!");
            }
        }
    }
}
