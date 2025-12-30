using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Animator animator;
    private bool shouldWait=false;
    public float timer;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//dashpos1
        {
            animator.SetTrigger("end");
            shouldWait = true;
        }
        if (shouldWait)
        { 
            float time= Time.deltaTime;
            timer-=time;
        }
        if (timer < 0)
        {
            SceneManager.LoadScene(1);
        }
    }
}
