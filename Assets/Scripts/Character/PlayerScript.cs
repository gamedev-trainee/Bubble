using UnityEngine;

namespace Bubble
{
    public class PlayerScript : MonoBehaviour
    {
        public static readonly int Layer_Brick = 6;
        public static readonly string IdleState = "idle_state";
        public static readonly string JumpState = "jump_state";
        public static readonly string BubbleState = "bubble_state";
        public static readonly int LifeBig = 2;
        public static readonly int LifeSmall = 1;
        public static readonly int MaxBubbleLife = 2;

        public Animator animator = null;
        public CircleCollider2D circleCollider = null;
        public GameObject shadow = null;
        public Animator shadowAnimator = null;
        public GameObject bubble = null;
        public Animator bubbleAnimator = null;
        public float speed = 1f;
        public float jumpSpeed = 1f;
        public float gravity = 1f;
        public float shadowRadius = 0f;
        public float bubbleRadius = 0f;

        private int m_moveDir = 0;

        private int m_jumpDir = 0;
        private float m_jumpSpeed = 0f;

        private bool m_isOnGround = false;

        private int m_life = 0;
        private int m_bubbleLife = 0;

        public float radius => m_bubbleLife > 0 ? bubbleRadius : shadowRadius;

        private void Start()
        {
            bubble.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 lastPos = transform.position;
            Vector3 pos = lastPos;
            updateMove(ref pos);
            updateJump(ref pos);
            updateGravity(ref pos);
            checkGround(lastPos, ref pos);
            checkWall(lastPos, ref pos);
            transform.position = pos;
        }

        public void moveLeft()
        {
            m_moveDir = -1;
        }

        public void stopMoveLeft()
        {
            if (m_moveDir == -1) stopMove();
        }

        public void moveRight()
        {
            m_moveDir = 1;
        }

        public void stopMoveRight()
        {
            if (m_moveDir == 1) stopMove();
        }

        public void stopMove()
        {
            m_moveDir = 0;
        }

        public void jump()
        {
            if (m_jumpDir != 0) return;
            m_jumpDir = 1;
            m_jumpSpeed = jumpSpeed;
            animator.SetInteger(JumpState, 1);
            m_isOnGround = false;
        }

        public void stopJump()
        {
            m_jumpDir = 0;
            animator.SetInteger(JumpState, 0);
        }

        public bool isOnGround()
        {
            return m_isOnGround;
        }

        protected void updateMove(ref Vector3 pos)
        {
            if (m_moveDir != 0)
            {
                pos.x += m_moveDir * speed * Time.deltaTime;
            }
        }

        protected void updateJump(ref Vector3 pos)
        {
            if (m_jumpDir == 0) return;
            pos.y += m_jumpSpeed * Time.deltaTime;
            m_jumpSpeed = m_jumpSpeed - gravity * Time.deltaTime;
            if (m_jumpDir > 0)
            {
                if (m_jumpSpeed < 0)
                {
                    m_jumpDir = -1;
                    animator.SetInteger(JumpState, -1);
                }
            }
        }

        protected void updateGravity(ref Vector3 pos)
        {
            if (m_jumpDir != 0) return;
            pos.y -= gravity * Time.deltaTime;
        }

        protected void checkGround(Vector3 lastPos, ref Vector3 pos)
        {
            if (m_jumpDir > 0) return;
            Vector2 start = new Vector2(lastPos.x, lastPos.y);
            float distance = Mathf.Abs(pos.y - lastPos.y);
            RaycastHit2D hit = Physics2D.CircleCast(start, radius, Vector2.down, distance, 1 << Layer_Brick);
            if (hit.collider != null)
            {
                if (hit.point.y > lastPos.y)
                {
                    pos.y = hit.point.y + radius;
                }
                else
                {
                    pos.y = lastPos.y;
                }
                m_jumpDir = 0;
                animator.SetInteger(JumpState, 0);
                m_isOnGround = true;
            }
            else
            {
                animator.SetInteger(JumpState, -1);
                m_isOnGround = false;
            }
        }

        protected void checkWall(Vector3 lastPos, ref Vector3 pos)
        {
            Vector2 start = new Vector2(lastPos.x, lastPos.y);
            Vector2 dir = new Vector2(pos.x - lastPos.x, 0f);
            float distance = Mathf.Abs(pos.x - lastPos.x);
            RaycastHit2D hit = Physics2D.CircleCast(start, radius, dir, distance, 1 << Layer_Brick);
            if (hit.collider != null)
            {
                if (dir.x > 0f)
                {
                    if (hit.point.x < lastPos.x)
                    {
                        pos.x = hit.point.x - radius;
                    }
                    else
                    {
                        pos.x = lastPos.x;
                    }
                }
                else if (dir.x < 0f)
                {
                    if (hit.point.x > lastPos.x)
                    {
                        pos.x = hit.point.x + radius;
                    }
                    else
                    {
                        pos.x = lastPos.x;
                    }
                }
                else
                {
                    pos.x = lastPos.x;
                }
            }
        }

        protected void setOnGround()
        {
            Vector3 pos = transform.position;
            Vector2 start = new Vector2(pos.x, pos.y + radius);
            RaycastHit2D hit = Physics2D.CircleCast(start, radius, Vector2.down, radius * 2, 1 << Layer_Brick);
            if (hit.collider != null)
            {
                if (hit.point.y > (pos.y - radius))
                {
                    pos.y = hit.point.y + radius;
                    transform.position = pos;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isAlive()) return;
            ObjectScript objectScript = collision.GetComponent<ObjectScript>();
            if (objectScript is ElementScript)
            {
                onHitElement(objectScript as ElementScript);
            }
            else if (objectScript is LightScript)
            {
                onHitLight(objectScript as LightScript);
            }
            else if (objectScript is TrapScript)
            {
                onHitTrap(objectScript as TrapScript);
                shadowAnimator.Play("beattack");
            }
        }

        public void onHitElement(ElementScript element)
        {
            GameObject.Destroy(element.gameObject);
            addBubbleLife(LifeBig);
        }

        protected void onHitLight(LightScript light)
        {
            //removeBubbleLife(LifeSmall);
            if (m_bubbleLife <= 0)
            {
                m_life--;
            }
        }

        protected void onHitTrap(TrapScript trap)
        {
            if (m_bubbleLife > 0)
            {
                removeBubbleLife(LifeBig);
            }
        }

        protected void addBubbleLife(int value)
        {
            if (!isAlive()) return;
            int oldBubbleLife = m_bubbleLife;
            m_bubbleLife += value;
            if (m_bubbleLife > MaxBubbleLife)
            {
                m_bubbleLife = MaxBubbleLife;
            }
            if (oldBubbleLife <= 0)
            {
                bubble.SetActive(true);
                circleCollider.radius = bubbleRadius;
                animator.SetFloat(IdleState, 1f);
                setOnGround();
            }
            bubbleAnimator.SetInteger(BubbleState, m_bubbleLife);
        }

        protected void removeBubbleLife(int value)
        {
            if (!isAlive()) return;
            m_bubbleLife -= value;
            if (m_bubbleLife <= 0)
            {
                m_bubbleLife = 0;
                bubble.SetActive(false);
                circleCollider.radius = shadowRadius;
                animator.SetFloat(IdleState, 0f);
            }
            bubbleAnimator.SetInteger(BubbleState, m_bubbleLife);
        }

        public bool isAlive()
        {
            return m_life > 0;
        }

        public void killSelf()
        {
            m_life = 0;
        }

        public void setLife(int value)
        {
            m_life = value;
        }
    }
}
