using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Alliance { friendly, hostile };

public class Entity : MonoBehaviour
{
    protected GameController gc;

    [HideInInspector] public GameObject attackReticle;
    [HideInInspector] public Animator animator;

    [HideInInspector] public SpriteRenderer spRenderer;
    protected BoxCollider2D box2d;

    [HideInInspector] public Entity currentTarget;
    public Empty currentAttack;
    [HideInInspector] public string currentAnim;

    public bool gone, dead;

    public bool blockingLeft, blockingRight, lockedLeft, lockedRight;

    public int id;

    public Alliance alliance;

    public int loc;
    public int LocNormal
    {
        get
        {
            int normal = gc.currentBattle.UnitCountNormal / 2;

            return (gc.currentMap.GetTile().things.Length / 2) + (loc - normal);
        }
    }
    
    [System.Serializable]
    public struct Resource
    {
        public int trueMax;
        
        public int max;
        public int current;

        public void TrueReset()
        {
            current = max = trueMax;
        }

        public void Reset()
        {
            current = max;
        }
    }

    public Resource hp, mov, lust;

    private GameObject healthBar;

    private Transform healthDisplay;
    private List<GameObject> movPips = new List<GameObject>();
    private Text healthText;

    public float healthBarPos = 2.2f;

    [System.Serializable]
    public struct Stat
    {
        public int max;
        public int current;

        public void Reset()
        {
            current = max;
        }
    }

    public Stat atk, def, spd, movRegen, resFire, resSound, resSalt, resSilver;

    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    protected Vector2 baseScale;

    private Vector2 targetPos;

    protected StatusEffect DOT;

    public void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        spRenderer = GetComponent<SpriteRenderer>();
        box2d = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        baseScale = transform.localScale;

        id = GameController.newID;

        hp.TrueReset();
        mov.TrueReset();
        lust.TrueReset();

        atk.Reset();
        def.Reset();
        spd.Reset();
        movRegen.Reset();

        resFire.Reset();
        resSound.Reset();
        resSalt.Reset();
        resSilver.Reset();

        attackReticle = Instantiate(gc.attackReticle, transform, false);
        attackReticle.SetActive(false);

        healthBar = Instantiate(gc.healthBar, gc.mainCanvas.transform);
        healthBar.name = "HealthBar";

        healthDisplay = healthBar.transform.FindChild("Health");
        healthText = healthBar.transform.FindChild("Text").GetComponent<Text>();

        for (int i = 0; i < 99; i++)
        {
            var movPip = Instantiate(gc.movPip, healthBar.transform.Find("MovPips"));

            if (i >= mov.current)
            {
                movPip.SetActive(false);
            }

            movPips.Add(movPip);
        }
    }

    public void Update()
    {
        if (gc.state == GameState.inBattle)
        {
            float x = -((gc.currentBattle.UnitCountNormal - 1) * GameController.unitWidth / 2) + (loc * GameController.unitWidth);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(x, GameController.floorY), 10 * Time.deltaTime);
        }
        else
        {
            //TODO: Program exploring AI
            transform.position = Vector3.MoveTowards(transform.position, Vector3.up * GameController.floorY, 10 * Time.deltaTime);
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.transform.position = transform.position + Vector3.up * healthBarPos;
        
        healthDisplay.localScale = new Vector3(hp.current / (float)hp.max, 1);
        
        healthText.text = "HP: " + hp.current;

        for (int i = 0; i < movPips.Count; i++)
        {
            var movPip = movPips[i];

            if (movPip.activeSelf)
            {
                movPip.transform.position = healthBar.transform.position;
                movPip.transform.position -= new Vector3(-gc.movPipRect.width * i - gc.movPipRect.x, gc.movPipRect.y);

            }

            if (i < mov.current)
            {
                movPip.SetActive(true);
            }
            else
            {
                movPip.SetActive(false);
            }
        }
    }

    public void OnDestroy()
    {
        Destroy(healthBar);
    }

    public virtual void Move()
    {
        Advance();
    }
    
    public Stat GetResistance(string res)
    {
        switch (res)
        {
            default:
                Debug.LogError("Bad resistance " + res);
                return def;

            case "normal":
                return def;

            case "fire":
                return resFire;

            case "sound":
                return resSound;

            case "salt":
                return resSalt;

            case "silver":
                return resSilver;
        }
    }

    public virtual void Hurt(int dmg, string dmgType = "normal", Entity attacker = null)
    {
        animator.Play("Hurt");

        int res = GetResistance(dmgType).current;

        dmg = (int) Mathf.Round((100f - res) / 100 * dmg);

        hp.current = Mathf.Clamp(hp.current - dmg, 0, hp.max);
        if (hp.current <= 0)
        {
            dead = true;
        }

        if (attacker != null)
        {
            //TODO
        }
    }

    public void SetAttack(Empty newAttack, string newAnim)
    {
        currentAttack = newAttack;
        currentAnim = newAnim;
    }

    public void PlayAttack()
    {
        animator.Play(currentAnim);
        // animation runs currentAttack func
    }

    protected void NullAttack()
    {
        currentAttack = null;
        currentAnim = null;
        currentTarget = null;
    }

    public void RunAttack()
    {
        currentAttack();

        NullAttack();
    }

    public void Advance()
    {
        gc.currentBattle.Advance();
    }

    protected void OnMouseEnter()
    {
        if (gc.currentBattle.state == BattleState.playerTurn)
        {
            gc.currentBattle.AttackPreview(this);

            FaceMe(gc.player);
        }
    }

    protected void OnMouseDown()
    {
        if (gc.currentBattle.state == BattleState.playerTurn
            && gc.currentBattle.currentPlayerAttack != null
            && attackReticle.activeSelf)
        {
            gc.player.PlayAttack(this);
            gc.currentBattle.NullCurrentAttackPreview();
        }
    }

    protected void FaceMe(Entity target)
    {
        if (target.loc < loc)
        {
            target.spRenderer.flipX = false;
        }
        else if (target.loc > loc)
        {
            target.spRenderer.flipX = true;
        }
    }

    protected void FaceTarget(Entity target)
    {
        if (target.loc < loc)
        {
            spRenderer.flipX = true;
        }
        else if (target.loc > loc)
        {
            spRenderer.flipX = false;
        }
    }

    protected List<Entity> GetAdjacentEnemies()
    {
        List<Entity> adjacent = new List<Entity>();

        if (loc > 0 && gc.currentBattle.arena[loc - 1].alliance != alliance)
        {
            adjacent.Add(gc.currentBattle.arena[loc - 1]);
        }

        if (loc < gc.currentBattle.arena.Count - 1 && gc.currentBattle.arena[loc + 1].alliance != alliance)
        {
            adjacent.Add(gc.currentBattle.arena[loc + 1]);
        }

        return adjacent;
    }

    protected bool IsAdjacent(Entity target)
    {
        return Mathf.Abs(loc - target.loc) <= 1;
    }


    // Attack Prefs
    public void HurtTarget()
    {
        currentTarget.Hurt(atk.current, "normal",  this);
    }
}
