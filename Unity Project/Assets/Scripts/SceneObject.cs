using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneObject : MonoBehaviour
{

    public Texture2D[] textures;
    public int xOffset;
    public int yOffset;
    public bool isInteractible;


    public abstract bool OnInteract(PlayerController player);  //  Returns true if the object was interacted with.


}
