using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bubble
{
    public class GameScript : MonoBehaviour
    {
        public enum GameStates
        {
            None,
            Ready,
            Running,
            Ending,
            End,
        }

        public CanvasScaler canvasScaler = null;
        public ControlScript control = null;
        //
        public GameObject playerResource = null;
        public GameObject brickResource = null;
        public GameObject elementResource = null;
        public GameObject lightResource = null;
        public GameObject trapResource = null;
        public float cellSize = 0f;
        public Vector2Int lightCounts = Vector2Int.zero;
        public float lightCountRoundIncrease = 0f;
        public Vector2Int trapCounts = Vector2Int.zero;
        public float trapCountRoundIncrease = 0f;
        //
        public CameraScript vCamera = null;
        //
        public float deadLine = 0f;
        public int startLife = 1;

        private PlayerScript m_player = null;
        private Transform m_brickRoot = null;
        private Transform m_lightRoot = null;
        private Transform m_trapRoot = null;
        private List<GameObject> m_bricks = new List<GameObject>();
        private List<GameObject> m_lights = new List<GameObject>();
        private List<GameObject> m_traps = new List<GameObject>();

        private int m_round = 0;

        private GameStates m_state = GameStates.None;

        private void Start()
        {
            m_state = GameStates.None;
            initMap();
            initElement();
            control.open();
        }

        private void Update()
        {
            switch (m_state)
            {
                case GameStates.None:
                    {
                        if (control.isOpened())
                        {
                            m_state = GameStates.Ready;
                            initPlayer();
                        }
                    }
                    break;
                case GameStates.Ready:
                    {
                        if (m_player.isOnGround())
                        {
                            m_state = GameStates.Running;
                            onPlayerShown();
                        }
                    }
                    break;
                case GameStates.Running:
                    {
                        updateMap();
                        checkPlayerLife();
                    }
                    break;
                case GameStates.Ending:
                    {
                        checkPlayerDeath();
                    }
                    break;
                case GameStates.End:
                    {
                        if (control.isClosed())
                        {
                            SceneManager.LoadScene("Entry");
                        }
                    }
                    break;
            }
        }

        protected void initMap()
        {
            m_brickRoot = createGameObject("__bricks__").transform;
            m_lightRoot = createGameObject("__lights__").transform;
            m_trapRoot = createGameObject("__traps__").transform;
            Vector3 sceneLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
            Vector3 sceneRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
            float vw = Mathf.Abs(sceneRight.x - sceneLeft.x);
            int count = Mathf.CeilToInt(vw / cellSize);
            for (int i = 0; i < count; i++)
            {
                initBrick(new Vector3(sceneLeft.x + i * cellSize, 0f, 0f));
            }
        }

        protected void initBrick(Vector3 position)
        {
            GameObject go = cloneResource(brickResource, m_brickRoot);
            go.transform.position = position;
            m_bricks.Add(go);
        }

        protected void initLight(Vector3 position)
        {
            GameObject go = cloneResource(lightResource, m_lightRoot);
            go.transform.position = position;
            m_lights.Add(go);
        }

        protected void initTrap(Vector3 position)
        {
            GameObject go = cloneResource(trapResource, m_trapRoot);
            go.transform.position = position;
            m_traps.Add(go);
        }

        protected void initElement()
        {
            GameObject elementrGo = cloneResource(elementResource);
            int elementBornIndex = m_bricks.Count - m_bricks.Count / 3;
            elementrGo.transform.position = m_bricks[elementBornIndex].transform.position + new Vector3(0f, cellSize, 0f);
        }

        protected void initElement(Vector3 position)
        {
            GameObject elementrGo = cloneResource(elementResource);
            elementrGo.transform.position = position;
        }

        protected void initPlayer()
        {
            GameObject go = cloneResource(playerResource);
            m_player = go.GetComponent<PlayerScript>();
            int playerBornIndex = m_bricks.Count / 2;
            Vector3 sceneTop = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f));
            m_player.transform.position = m_bricks[playerBornIndex].transform.position + new Vector3(0f, sceneTop.y + cellSize, 0f);
            m_player.setLife(startLife);
        }

        protected GameObject createGameObject(string name)
        {
            GameObject go = new GameObject(name);
            go.name = name;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
            return go;
        }

        protected GameObject cloneResource(GameObject resource, Transform parent = null)
        {
            GameObject go = GameObject.Instantiate(resource);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
            return go;
        }

        protected void onPlayerShown()
        {
            initControl();
            initCamera();
        }

        protected void initControl()
        {
            control.show();
            control.buttonLeft.onButtonPressed = onButtonLeftPressed;
            control.buttonLeft.onButtonReleased = onButtonLeftReleased;
            control.buttonRight.onButtonPressed = onButtonRightPressed;
            control.buttonRight.onButtonReleased = onButtonRightReleased;
            control.buttonUp.onButtonReleased = onButtonUpReleased;
        }

        protected void onButtonLeftPressed()
        {
            m_player.moveLeft();
        }

        protected void onButtonLeftReleased()
        {
            m_player.stopMoveLeft();
        }

        protected void onButtonRightPressed()
        {
            m_player.moveRight();
        }

        protected void onButtonRightReleased()
        {
            m_player.stopMoveRight();
        }

        protected void onButtonUpReleased()
        {
            m_player.jump();
        }

        protected void initCamera()
        {
            vCamera.follow = m_player.transform;
        }

        protected void checkPlayerLife()
        {
            if (m_player.transform.position.y < deadLine)
            {
                m_player.killSelf();
            }
            if (!m_player.isAlive())
            {
                m_state = GameStates.Ending;
                m_player.animator.Play("death");
                m_player.animator.Update(0f);
            }
        }

        protected void checkPlayerDeath()
        {
            AnimatorStateInfo aniState = m_player.animator.GetCurrentAnimatorStateInfo(0);
            if (aniState.IsName("death"))
            {
                if (aniState.normalizedTime < 1f)
                {
                    return;
                }
            }
            m_state = GameStates.End;
            control.close();
        }

        protected void updateMap()
        {
            Vector3 sceneLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
            Vector3 sceneRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
            float vw = Mathf.Abs(sceneRight.x - sceneLeft.x);
            int count = Mathf.CeilToInt(vw / cellSize);
            float removeLeft = sceneLeft.x - cellSize * (count / 3);
            removeObjects(m_bricks, removeLeft);
            removeObjects(m_lights, removeLeft);
            removeObjects(m_traps, removeLeft);
            float generateRight = sceneRight.x + cellSize * (count / 3);
            if (m_bricks[m_bricks.Count - 1].transform.position.x <= generateRight)
            {
                float vx = m_bricks[m_bricks.Count - 1].transform.position.x + cellSize;
                float vy = m_bricks[m_bricks.Count - 1].transform.position.y;
                generateMap(vx, vy, count);
            }
        }

        protected void removeObjects(List<GameObject> list, float removeLeft)
        {
            if (list.Count <= 0)
            {
                return;
            }
            if (list[0].transform.position.x > removeLeft)
            {
                return;
            }
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (list[i].transform.position.x <= removeLeft)
                {
                    GameObject.Destroy(list[i].gameObject);
                    list.RemoveAt(0);
                    i--;
                    count--;
                }
            }
        }

        protected void generateMap(float vx, float vy, int count)
        {
            for (int i = 0; i < count; i++)
            {
                initBrick(new Vector3(vx + i * cellSize, 0f, 0f));
            }
            List<int> indexesList = new List<int>();
            fillIndexesList(indexesList, count);
            generateLight(indexesList, vx, vy, count);
            generateTrap(indexesList, vx, vy, count);
            generateElement(vx, vy, count);
            m_round++;
        }

        protected void fillIndexesList(List<int> indexesList, int count)
        {
            if (indexesList.Count > 0) return;
            for (int i = 0; i < count; i++)
            {
                indexesList.Add(i);
            }
        }

        protected void generateLight(List<int> indexesList, float vx, float vy, int count)
        {
            Vector3 sceneTop = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f));
            int generateCount = Random.Range(lightCounts.x, lightCounts.y + (int)(m_round * lightCountRoundIncrease) + 1);
            int generateIndex;
            for (int i = 0; i < generateCount; i++)
            {
                if (indexesList.Count <= 0) break;
                generateIndex = randomInList(indexesList, true);
                initLight(new Vector3(vx + generateIndex * cellSize, vy + sceneTop.y + cellSize, 0f));
            }
        }

        protected void generateTrap(List<int> indexesList, float vx, float vy, int count)
        {
            int generateCount = Random.Range(trapCounts.x, trapCounts.y + (int)(m_round * trapCountRoundIncrease) + 1);
            int generateIndex;
            for (int i = 0; i < generateCount; i++)
            {
                if (indexesList.Count <= 0) break;
                generateIndex = randomInList(indexesList, true);
                initTrap(new Vector3(vx + generateIndex * cellSize, vy + cellSize, 0f));
            }
        }

        protected void generateElement(float vx, float vy, int count)
        {
            if (m_round % 2 == 0) return;
            int generateIndex = Random.Range(0, count);
            initElement(new Vector3(vx + generateIndex * cellSize, vy + cellSize, 0f));
        }

        protected int randomInList(List<int> list, bool removeResult)
        {
            int index = Random.Range(0, list.Count);
            int result = list[index];
            if (removeResult)
            {
                list.RemoveAt(index);
            }
            return result;
        }
    }
}
