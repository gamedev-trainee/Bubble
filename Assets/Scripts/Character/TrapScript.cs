using UnityEngine;

namespace Bubble
{
    public class TrapScript : ObjectScript
    {
        public enum TrapStates
        {
            Ready,
            Show,
            Stay,
            Hide,
        }

        public Animator animator = null;
        public string showAniName = string.Empty;
        public string hideAniName = string.Empty;
        public Collider2D hitRect = null;

        public Vector2 intervalRange = Vector2.zero;
        public Vector2 runningTimeRange = Vector2.zero;

        private TrapStates m_state = TrapStates.Ready;
        private float m_cd = 0f;
        private float m_runningTime = 0f;

        private void Start()
        {
            m_state = TrapStates.Ready;
            m_cd = Random.Range(intervalRange.x, intervalRange.y);
            m_runningTime = Random.Range(runningTimeRange.x, runningTimeRange.y);
            animator.Play(hideAniName);
            hitRect.enabled = false;
        }

        private void Update()
        {
            switch (m_state)
            {
                case TrapStates.Ready:
                    {
                        m_cd -= Time.deltaTime;
                        if (m_cd <= 0f)
                        {
                            m_state = TrapStates.Show;
                            animator.Play(showAniName);
                        }
                    }
                    break;
                case TrapStates.Show:
                    {
                        AnimatorStateInfo curState = animator.GetCurrentAnimatorStateInfo(0);
                        if (curState.IsName(showAniName))
                        {
                            if (curState.normalizedTime < 1f)
                            {
                                return;
                            }
                        }
                        m_state = TrapStates.Stay;
                        hitRect.enabled = true;
                    }
                    break;
                case TrapStates.Stay:
                    {
                        m_runningTime -= Time.deltaTime;
                        if (m_runningTime <= 0f)
                        {
                            hitRect.enabled = false;
                            m_state = TrapStates.Hide;
                            animator.Play(hideAniName);
                        }
                    }
                    break;
                case TrapStates.Hide:
                    {
                        AnimatorStateInfo curState = animator.GetCurrentAnimatorStateInfo(0);
                        if (curState.IsName(hideAniName))
                        {
                            if (curState.normalizedTime < 1f)
                            {
                                return;
                            }
                        }
                        m_state = TrapStates.Ready;
                        m_cd = Random.Range(intervalRange.x, intervalRange.y);
                        m_runningTime = Random.Range(runningTimeRange.x, runningTimeRange.y);
                    }
                    break;
            }
        }
    }
}
