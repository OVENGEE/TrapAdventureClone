using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private const string RAISE_PARAM = "Raise";

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" )
        {
            anim.SetTrigger(RAISE_PARAM);
        }
    }
}
