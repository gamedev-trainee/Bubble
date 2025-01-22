using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bubble
{
    public class EntryScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SceneManager.LoadScene("Stage");
        }
    }
}
