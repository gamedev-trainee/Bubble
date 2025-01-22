using UnityEngine;

namespace Bubble
{
    public class CameraScript : MonoBehaviour
    {
        public Transform follow = null;
        public Vector2 offset = Vector2.zero;
        public Vector2 min = Vector2.zero;
        public float speed = 1f;

        private Vector2 nextPos = Vector2.zero;

        private void LateUpdate()
        {
            if (follow == null) return;
            Vector3 followPos = follow.position;
            nextPos.x = followPos.x + offset.x;
            if (followPos.y > min.y)
            {
                nextPos.y = followPos.y + offset.y;
            }
            Vector3 pos = transform.position;
            if (pos.x < nextPos.x)
            {
                pos.x += speed * Time.deltaTime;
                if (pos.x > nextPos.x)
                {
                    pos.x = nextPos.x;
                }
            }
            else if (pos.x > nextPos.x)
            {
                pos.x -= speed * Time.deltaTime;
                if (pos.x < nextPos.x)
                {
                    pos.x = nextPos.x;
                }
            }
            if (pos.y < nextPos.y)
            {
                pos.y += speed * Time.deltaTime;
                if (pos.y > nextPos.y)
                {
                    pos.y = nextPos.y;
                }
            }
            else if (pos.y > nextPos.y)
            {
                pos.y -= speed * Time.deltaTime;
                if (pos.y < nextPos.y)
                {
                    pos.y = nextPos.y;
                }
            }
            transform.position = pos;
        }
    }
}
