using UnityEngine;

namespace Bubble
{
    public class ControlScript : MonoBehaviour
    {
        public ButtonScript buttonLeft = null;
        public ButtonScript buttonRight = null;
        public ButtonScript buttonUp = null;
        public Animator animator = null;

        public void show()
        {
            gameObject.SetActive(true);
            animator.Play("ui_show");
            animator.Update(0f);
        }

        public void hide()
        {
            animator.Play("ui_hide");
            animator.Update(0f);
        }

        public void open()
        {
            animator.Play("ui_open");
            animator.Update(0f);
        }

        public bool isOpened()
        {
            AnimatorStateInfo aniState = animator.GetCurrentAnimatorStateInfo(0);
            if (aniState.IsName("ui_open"))
            {
                return aniState.normalizedTime >= 1f;
            }
            return true;
        }

        public void close()
        {
            animator.Play("ui_close");
            animator.Update(0f);
        }

        public bool isClosed()
        {
            AnimatorStateInfo aniState = animator.GetCurrentAnimatorStateInfo(0);
            if (aniState.IsName("ui_close"))
            {
                return aniState.normalizedTime >= 1f;
            }
            return true;
        }
    }
}