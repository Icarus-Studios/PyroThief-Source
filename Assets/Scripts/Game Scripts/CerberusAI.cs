using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CerberusAI : MonoBehaviour
{
    [Header("Character Attributes:")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackDelay = 2f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float stunnedTime = 2f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float lavaFallDelay = 1.5f;


    private ParticleSystem blood;
    public SpriteRenderer healthBar;
    public SpriteRenderer lavaWave;
    public float attackRange = 1f;
    Vector3 localScale;
    private GameObject SFX;

    private new Animator animation;
    private new Rigidbody2D rb;
    private new BoxCollider2D box;
    //private new GameObject targetObj;
    private bool hitShield;
    public int phase = 1;
    private bool lavaFlow = false;
    public GameObject lavaPreFab;
    private GameObject[] lavaTiles;
    private List<List<GameObject>> listColumns = new List<List<GameObject>>(30);
    private List<GameObject> firstlist = new List<GameObject>();
    private int numTiles = 0;
    private int colNum = 0;
    private float colXStart = 98.5016f;
    private float colXMultiplier = 0.9899f;
    colPara[] paraList;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        //targetObj = GameObject.Find("Promethesus");
        animation = GetComponentInChildren<Animator>();
        blood = gameObject.GetComponentInChildren(typeof(ParticleSystem), true) as ParticleSystem;
        localScale = healthBar.transform.localScale;
        SFX = GameObject.Find("SFX");
        paraList = prepareLava();
    }

    // Update is called once per frame
    void Update()
    {
        localScale.x = health / 100;
        healthBar.transform.localScale = 4*localScale;
    }

    private void FixedUpdate()
    {
        phase = CheckCurrentPhase();
        //Debug.Log("Phase:" + phase);
        RunCurrentPhase(phase);
        //Debug.Log("Health: " + health);
        //animation.SetBool("IsStun", true);
        //animation.SetBool("IsHowl", true);
        //animation.SetTrigger("Swipe");
        //animation.SetTrigger("IsIdle",true);
    }

    private int CheckCurrentPhase()
    {
        if(health >= 0.80 * maxHealth) { phase = 1; }
        else if(health < 0.80f * maxHealth && health >= 0.25f * maxHealth) { phase = 2; }
        else if(health < 0.25f * maxHealth) { phase = 3; }

        return phase;
    }
    private void RunCurrentPhase(int phase)
    {
        if(phase == 1) { Phase1(); }
        else if(phase == 2) { Phase2();  }
        else if(phase == 3) { Phase3(); }
    }

    private void Phase1()
    {
        animation.SetBool("IsStun", true);
        if (!lavaFlow)
        {
            //releaseLava();
            lavaFlow = true;
            StartCoroutine("releaseLava");
        }
    }

    private void Phase2()
    {
        animation.SetBool("IsHowl", true);
    }

    private void Phase3()
    {
        animation.SetTrigger("Swipe");
    }

    private colPara[] prepareLava()
    {
        colPara[] colParaList = new colPara[1];
        for(int colParaIt = 0; colParaIt < colParaList.Length; ++colParaIt)
        {
            colPara colParaCurrent;
            colParaCurrent.numTiles = numTiles;
            colParaCurrent.skip = 1;
            colParaCurrent.xPos = colParaIt;
            colParaList[colParaIt] = colParaCurrent;
            //Debug.Log("numTiles: " + numTiles);
            //Debug.Log("xPos:" + colParaIt);
        }

        return colParaList;
    }
    //IEnumerator releaseLava()
    //{
    //    yield return new WaitForSeconds(lavaFallDelay);

    //    for (int col = 0; col < paraList.Length; ++col)
    //    {
    //        //StartCoroutine("lavaCol25", paraList[col]);
    //        lavaCol25(paraList[col]);
    //    }
    //    //array3 = new int[] { 1, 3, 5, 7, 9 };
    //    ++numTiles;
    //}
    IEnumerator releaseLava()
    {
        yield return new WaitForSeconds(lavaFallDelay);
        for (int col = 0; col < paraList.Length; ++col)
        {
            firstlist.Add(Instantiate(lavaPreFab, new Vector3(colXStart + (colXMultiplier * paraList[col].xPos), -15.644f, 14), Quaternion.identity) as GameObject);
            GameObject tile = firstlist[col];
            Debug.Log("Tile pos: " + tile.transform.position);


            if (tile.transform.position.y + 29.71f < 0.01)
            {
                Destroy(tile);
                firstlist.Remove(tile);
                Debug.Log("Destroy!");
            }
            else
            {
                tile.transform.position = new Vector2(tile.transform.position.x, tile.transform.position.y - 1f);
            }
            
        }
        //array3 = new int[] { 1, 3, 5, 7, 9 };
        ++numTiles;
        lavaFlow = false;
    }

    struct colPara
    {
        public int numTiles;
        //public int[] skip;
        public int skip;
        public int xPos;
    }

    private void lavaCol25(colPara data)
    {
        //yield return new WaitForSeconds(lavaFallDelay);
        Debug.Log("Making new tile!");
        //Debug.Log("numtiles:" + numTiles);
        //firstlist.Insert(numTiles, Instantiate(lavaPreFab, new Vector3(122.27f, -14.71f, 14), Quaternion.identity) as GameObject);
        //if(numTiles % 3 == 0)
        //{
        //    firstlist.Add(Instantiate(lavaPreFab, new Vector3(122.27f, -14.71f, 14), Quaternion.identity) as GameObject);
        //}
        Debug.Log("Where xpos:" + data.xPos);
        Debug.Log("Where colXMultiplier:" + colXMultiplier);
        Debug.Log("Where colXMultiplier * data.xPos" + colXMultiplier * data.xPos);
        Debug.Log("Where colXStart:" + colXStart);
        Debug.Log("Where (colXMultiplier * data.xPos) + colXStart:" + ((colXMultiplier * data.xPos) + colXStart));
        Debug.Log("Moving instance to this X: " + ((colXMultiplier * data.xPos) + colXStart));
        //if (data.numTiles % data.skip == 0)
        //{
        //    firstlist.Add(Instantiate(lavaPreFab, new Vector3(colXStart+(colXMultiplier*data.xPos), -14.71f, 14), Quaternion.identity) as GameObject);
        //}
        firstlist.Add(Instantiate(lavaPreFab, new Vector3(colXStart + (colXMultiplier * data.xPos), -15.644f, 14), Quaternion.identity) as GameObject);

        Debug.Log("Lava tile made");
        //numTiles += 1;
        for(int i = 0; i < firstlist.Count; ++i)// (GameObject tile in firstlist)
        {
            Debug.Log("List size: " + firstlist.Count);
            Debug.Log("i:" + i);
            GameObject tile = firstlist[i];
            //Debug.Log(tile.transform.position.y);
            Debug.Log("Tile Position: " + tile.transform.position);

            if (tile.transform.position.y + 30.71f < 0.01)
            { 
                Destroy(tile); 
                firstlist.Remove(tile); 
                Debug.Log("Destroy!"); 
            }
            else
            {
                tile.transform.position = new Vector2(tile.transform.position.x, tile.transform.position.y - 1f);
            }
            Debug.Log("Tile Position after -1: " + tile.transform.position);
            //Debug.Log("Made it");
        }
        lavaFlow = false;
    }
}
