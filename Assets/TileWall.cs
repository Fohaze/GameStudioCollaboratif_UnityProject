using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileWall : MonoBehaviour
{
    public GameObject walls_right;
    public GameObject walls_left;
    public GameObject walls_forward;
    public GameObject walls_backward;
    
    public void SetWalls(bool right, bool left, bool forward, bool backward){
        walls_right.SetActive(right);
        walls_left.SetActive(left);
        walls_forward.SetActive(forward);
        walls_backward.SetActive(backward);
    }
}
