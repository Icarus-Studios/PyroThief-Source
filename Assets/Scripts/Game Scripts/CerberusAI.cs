using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CerberusAI : MonoBehaviour
{
    [Header("Character Attributes:")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackDelay = 2f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float stunnedTime = 2f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float lavaFallDelay = 0.001f;
    [SerializeField] private BoxCollider2D swipeBox;
    //private GameObject swipeBoxObj;
    //private BoxCollider2D swipeBox;

    private ParticleSystem blood;
    public SpriteRenderer healthBar;
    public float attackRange = 1f;
    Vector3 localScale;
    private GameObject SFX;
    private Rigidbody2D target;
    private float minAtckDist = 2.5f;


    private new Animator animation;
    private new Rigidbody2D rb;
    private new BoxCollider2D box;
    //private new GameObject targetObj;
    private bool hitShield;
    public int phase = 1;
    private bool lavaFlow = false;
    public GameObject lavaPreFab;
    //private List<GameObject> activeLavaPrefabs = new List<GameObject>();
    private int tileNum = 1;
    private float colXStart = 98.5016f;
    private float colXMultiplier = 0.9899f;
    private StructData[] struct31List = new StructData[31];
    private GameObject[] tilesToDelete;
    private bool isAttacking;
    private bool isLavaReady = true;

    private int prevEnemiesActive;
    private bool enemiesDead = false;
    bool enabled = false;
    // Start is called before the first frame update
    void Start()
    {
        prevEnemiesActive = GameManager.Instance.enemiesActive;
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        //swipeBoxObj = GameObject.FindWithTag("swipeBox");
        //swipeBox = swipeBoxObj.GetComponent<BoxCollider2D>();
        target = GameObject.Find("Promethesus").GetComponent<Rigidbody2D>();
        //targetObj = GameObject.Find("Promethesus");
        animation = GetComponentInChildren<Animator>();
        blood = gameObject.GetComponentInChildren(typeof(ParticleSystem), true) as ParticleSystem;
        localScale = healthBar.transform.localScale;
        SFX = GameObject.Find("SFX");
        prepareLava();
    }

    // Update is called once per frame
    void Update()
    {
        localScale.x = health / maxHealth;
        healthBar.transform.localScale = new Vector3(localScale.x * 7, localScale.y * 4, localScale.z * 4);
    }

    private void FixedUpdate()
    {
        phase = CheckCurrentPhase();
        RunCurrentPhase(phase);
        Debug.Log("Current Health: " + health);
    }

    private int CheckCurrentPhase()
    {
        if(health >= 0.80 * maxHealth) { phase = 1; }
        else if(health < 0.80f * maxHealth && health >= 0.50f * maxHealth) { phase = 2; }
        else if(health < 0.50f * maxHealth && health >= 0.40f * maxHealth) { phase = 3; }
        else if (health < 0.40f * maxHealth && health >= 0f * maxHealth)
        {
            phase = 2;
        }

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
        animation.SetBool("IsIdle", true);
        swipeBox.enabled = false;
        float distanceToChar = ((Vector2)(target.transform.position - transform.position)).magnitude;
        //Debug.Log("Distance to Char:" + distanceToChar);
        if (distanceToChar < minAtckDist)
        {
            animation.SetBool("IsIdle", false);

            //Debug.Log("I should be attacking!");
            if (!isAttacking)
            {
                //Debug.Log("Attacking");
                animation.SetTrigger("Swipe");
                isAttacking = true;
                //StartCoroutine("attackAnimationDelay");
                Attack();
            }
            swipeBox.enabled = false;
        }
        //animation.SetBool("IsIdle", true);
   
        
    }

    IEnumerator Phase1SubDelay()
    {
        yield return new WaitForSeconds(0f);
        //Debug.Log("Lava should flow now!");
        if (!lavaFlow)
        {
            //Debug.Log("See! flowing");
            lavaFlow = true;
            StartCoroutine("releaseLava");
        }
    }
    private void Phase2()
    {
        Debug.Log("Phase2");
        animation.SetBool("IsIdle", true);
        animation.SetBool("IsHowl", false);
        swipeBox.enabled = false;
        float distanceToChar = ((Vector2)(target.transform.position - transform.position)).magnitude;
        //Debug.Log("Distance to Char:" + distanceToChar);
        if (distanceToChar < minAtckDist)
        {
            animation.SetBool("IsIdle", false);

            //Debug.Log("I should be attacking!");
            if (!isAttacking)
            {
                //Debug.Log("Attacking");
                animation.SetTrigger("Swipe");
                isAttacking = true;
                //StartCoroutine("attackAnimationDelay");
                Attack();
            }
            swipeBox.enabled = false;
        }
        //animation.SetBool("IsHowl", true);
        StartCoroutine("Phase1SubDelay");
    }

    private void Phase3()
    {
        animation.SetBool("IsIdle", false);
        animation.SetBool("IsHowl", true);
        //targetScript2 s2 = gobject.GetComponent<Script2>()
        PlayerController script = target.gameObject.GetComponent<PlayerController>();
        script.walkingSpeed = 3f;
        Debug.Log("Phase3");
        GameManager.Instance.enableBossSpawn();
        if(enabled == false)
        {
            GameManager.Instance.enemiesActive++;
            enabled = true;
        }
        tilesToDelete = GameObject.FindGameObjectsWithTag("Wave");

        foreach (GameObject tile in tilesToDelete)
        {
            Destroy(tile);
        }
        //animation.SetTrigger("Swipe");

        if (prevEnemiesActive == (GameManager.Instance.enemiesActive - 1))
        {
            enemiesDead = true;
        }
        else
        {

            enemiesDead = false;
        }

        //Debug.LogError("prev:" + prevEnemiesActive + " cu: " + GameManager.Instance.enemiesActive);
            
    }
    struct StructData
    {
        public int skip;
        public int xPos;
        public List<GameObject> activeLavaPrefabs;
        public bool isPattern;
        public int[] pattern;

        public StructData(int skip, int xPos, List<GameObject> activeLavaPrefabs, bool isPattern, int[] pattern)
        {
            this.skip = skip;
            this.xPos = xPos;
            this.activeLavaPrefabs = activeLavaPrefabs;
            this.isPattern = isPattern;
            this.pattern = pattern;
        }
    }

    private void prepareLava()
    {
        int[] whichColPattern = new int[] { 12, 13, 14, 15, 16, 17 ,18 };
        int[] patternCol12 = new int[1000];
        int[] patternCol13 = new int[1000];
        int[] patternCol14 = new int[1000];
        int[] patternCol15 = new int[1000];
        int[] patternCol16 = new int[1000];
        int[] patternCol17 = new int[1000];
        int[] patternCol18 = new int[1000];
        patternCol12[0] = 1;
        patternCol12[1] = 2;
        patternCol12[2] = 3;
        patternCol13[0] = 1;
        patternCol13[1] = 2;
        patternCol13[2] = 3;
        patternCol13[3] = 4;
        patternCol13[4] = 5;
        patternCol13[5] = 6;
        patternCol13[6] = 16;
        patternCol13[7] = 17;
        patternCol13[8] = 18;
        patternCol14[0] = 1;
        patternCol14[1] = 2;
        patternCol14[2] = 3;
        patternCol14[3] = 4;
        patternCol14[4] = 5;
        patternCol14[5] = 6;
        patternCol14[6] = 7;
        patternCol14[7] = 8;
        patternCol14[8] = 9;
        patternCol14[9] = 13;
        patternCol14[10] = 14;
        patternCol14[11] = 15;
        patternCol14[12] = 16;
        patternCol14[13] = 17;
        patternCol14[14] = 18;
        patternCol15[0] = 1;
        patternCol15[1] = 2;
        patternCol15[2] = 3;
        patternCol15[3] = 4;
        patternCol15[4] = 5;
        patternCol15[5] = 6;
        patternCol15[6] = 7;
        patternCol15[7] = 8;
        patternCol15[8] = 9;
        patternCol15[9] = 10;
        patternCol15[10] = 11;
        patternCol15[11] = 12;
        patternCol15[12] = 13;
        patternCol15[13] = 14;
        patternCol15[14] = 15;
        patternCol15[15] = 16;
        patternCol15[16] = 17;
        patternCol15[17] = 18;
        patternCol16[0] = 4;
        patternCol16[1] = 5;
        patternCol16[2] = 6;
        patternCol16[3] = 7;
        patternCol16[4] = 8;
        patternCol16[5] = 9;
        patternCol16[6] = 10;
        patternCol16[7] = 11;
        patternCol16[8] = 12;
        patternCol16[9] = 13;
        patternCol16[10] = 14;
        patternCol16[11] = 15;
        patternCol16[12] = 16;
        patternCol16[13] = 17;
        patternCol16[14] = 18;
        patternCol17[0] = 7;
        patternCol17[1] = 8;
        patternCol17[2] = 9;
        patternCol17[3] = 10;
        patternCol17[4] = 11;
        patternCol17[5] = 12;
        patternCol17[6] = 13;
        patternCol17[7] = 14;
        patternCol17[8] = 15;
        patternCol18[0] = 10;
        patternCol18[1] = 11;
        patternCol18[2] = 12;


        //col12
        for (int i = 3; i < 1000; ++i)
        {
            patternCol12[i] = patternCol12[i - 3] + 18;//patternCol14[i - 3] + 6;
        }
        //col13
        for (int i = 9; i < 1000; ++i)
        { 
            patternCol13[i] = patternCol13[i - 9] + 18;//patternCol14[i - 3] + 6;
        }
        //col14
        for (int i = 15; i < 1000; ++i)
        {
            patternCol14[i] = patternCol14[i - 15] + 18;//patternCol14[i - 3] + 6;
        }
        //col15
        for (int i = 18; i < 1000; ++i)
        {
            patternCol15[i] = patternCol15[i - 18] + 18;//patternCol14[i - 3] + 6;

        }
        //col16
        for (int i = 15; i < 1000; ++i)
        {
            patternCol16[i] = patternCol16[i - 15] + 18;//patternCol14[i - 3] + 6;
        }
        //col17
        for (int i = 9; i < 1000; ++i)
        {
            patternCol17[i] = patternCol17[i - 9] + 18;//patternCol14[i - 3] + 6;
        }
        //col18
        for (int i = 3; i < 1000; ++i)
        {
            patternCol18[i] = patternCol18[i - 3] + 18;//patternCol14[i - 3] + 6;
        }

        //for (int i = 0; i < 100; ++i)
        //{
        //    Debug.Log(patternCol16[i]);
        //}
        for (int columnStruct = 0; columnStruct < struct31List.Length; ++columnStruct)
        {
            StructData currentStruct = new StructData(1, columnStruct, new List<GameObject>(), false, new int[] {-1});

            if (whichColPattern.Contains(columnStruct) && columnStruct == 12)
            {
                currentStruct = new StructData(1, columnStruct, new List<GameObject>(), true, patternCol12);
            }
            else if (whichColPattern.Contains(columnStruct) && columnStruct == 13)
            {
                currentStruct = new StructData(1, columnStruct, new List<GameObject>(), true, patternCol13);
            }
            else if(whichColPattern.Contains(columnStruct) && columnStruct == 14)
            {
                currentStruct = new StructData(1, columnStruct, new List<GameObject>(), true, patternCol14);
            }
            else if(whichColPattern.Contains(columnStruct) && columnStruct == 15)
            {
                currentStruct = new StructData(1, columnStruct, new List<GameObject>(), true, patternCol15);

            }
            else if(whichColPattern.Contains(columnStruct) && columnStruct == 16)
            {
                currentStruct = new StructData(1, columnStruct, new List<GameObject>(), true, patternCol16);
            }
            else if (whichColPattern.Contains(columnStruct) && columnStruct == 17)
            {
                currentStruct = new StructData(1, columnStruct, new List<GameObject>(), true, patternCol17);
            }
            else if (whichColPattern.Contains(columnStruct) && columnStruct == 18)
            {
                currentStruct = new StructData(1, columnStruct, new List<GameObject>(), true, patternCol18);
            }

            struct31List[columnStruct] = currentStruct;
        }
    }
    IEnumerator releaseLava()
    {
        if(tileNum == 56 || (health < 0.50f * maxHealth))
        {
            tilesToDelete = GameObject.FindGameObjectsWithTag("Wave");

            foreach (GameObject tile in tilesToDelete)
            {
                Destroy(tile);
            }

            tileNum = 1;
            lavaFlow = false;
            prepareLava();
            StopCoroutine("releaseLava");
            StopAllCoroutines();
        }

        yield return new WaitForSeconds(.25f);
        //yield return new WaitForSeconds(lavaFallDelay);
        for (int col = 0; col < struct31List.Length; ++col)
        {
            bool foundBlankTile = false;

            for (int i = 0; i < struct31List[col].pattern.Length; ++i)
            {
                //Debug.Log("tileNum: " + tileNum);
                //Debug.Log("Column: " + col);
                //Debug.Log("Value: " + struct31List[col].pattern[i]);
                foundBlankTile = struct31List[col].pattern[i] == tileNum;
                //Debug.Log("foundBlankTile: " + foundBlankTile);
                if (foundBlankTile) { break ;}
            }
            //Debug.Log("foundBlankAgainTile: " + foundBlankTile);

            GameObject tile;

            if (!foundBlankTile)//!struct31List[col].isPattern
            {
                //Debug.Log("Tile made!");
                tile = Instantiate(lavaPreFab, new Vector3(colXStart + (colXMultiplier * struct31List[col].xPos), -14.644f, 14), Quaternion.identity) as GameObject;
                struct31List[col].activeLavaPrefabs.Add(tile);
            }
            else
            {
                //Debug.Log("Should be blank tile");
            }

            for (int activeTile = 0; activeTile < struct31List[col].activeLavaPrefabs.Count; ++activeTile)
            {
                tile = struct31List[col].activeLavaPrefabs[activeTile];
                if(tile != null)
                {
                    if (tile.transform.position.y + 29.71f < 0.01f)
                    {
                        Destroy(tile);
                        struct31List[col].activeLavaPrefabs.Remove(tile);
                        //Debug.Log("Destroy!");
                    }
                    else
                    {
                        tile.transform.position = new Vector2(tile.transform.position.x, tile.transform.position.y - 1f);
                    }
                }

            }
        }
        ++tileNum;

        lavaFlow = false;
    }

    void AttackComplete()
    {
        Debug.Log("In atckcomplete");
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(swipeBox.transform.position, swipeBox.size*1f, 0f); //swipeBox.size
        swipeBox.enabled = true;

        foreach (Collider2D hit in hitColliders)
        {
            //Debug.Log("Colliders: " + hit.transform.name);
            if (hit.CompareTag("Shield"))
            {
                if (hit.enabled)
                {
                    hitShield = true;
                }
                else
                {
                    hitShield = false;
                }
            }
        }
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player") && !hitShield)
            {
                //Debug.Log("Hitting player!");
                GameManager.Instance.updateHP(-attackDamage);
                target.AddForce(new Vector2(0, -1) * 500f);
            }
            else if (hit.CompareTag("Player") && hitShield)
            {
                //Debug.Log("Hitting shield!");
                CinemachineShake.Instance.ShakeCamera(10f, .3f);
                target.AddForce(new Vector2(0, -1) * 200f);
                hitShield = false;
            }
        }
        isAttacking = false;
    }

    public void Attack()
    {
        if (isAttacking)
        {
            Invoke("AttackComplete", attackDelay);
        }
    }

    public void takeDamage(int damage)
    {
        //Debug.Log("Enemy was attacked!");
        blood.Play();
        SFX.GetComponent<SFX>().PlayDamageSound();
        health -= damage;
        if (health <= 0)
        {
            // Debug.Log("Enemy died!");
            //dropItem();
            GameManager.Instance.enemiesActive--;
            Destroy(this.gameObject);
            StartCoroutine(Wait());
            GameManager.Instance.startEndCutscene();
        }
    }
    IEnumerator Wait()
    {

        yield return new WaitForSeconds(3f);

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name.ToString());
        //Debug.Log(health.ToString());

        if (collision.gameObject.name == "Bullet(Clone)")
        {
            takeDamage(PlayerController.Instance.turretDamage);
        }
        else if (collision.gameObject.tag == "Projectile")
        {
            takeDamage(PlayerController.Instance.attackDamage);
        }
    }
}
