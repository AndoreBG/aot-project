using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptBarrier : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private BoxCollider2D collider;

    private void Update()
    {
        if(collider.IsTouching(player.GetComponent<CapsuleCollider2D>()))
        {
            LoadScene();
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene("Final");
    }
}
