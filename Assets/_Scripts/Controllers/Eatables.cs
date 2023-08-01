using UnityEngine;

public class Eatables : MonoBehaviour
{
    public int iD;
    public int points;
    public EatableType type;


    private void Awake()
    {
        gameObject.tag = "Eatables";
    }
}
