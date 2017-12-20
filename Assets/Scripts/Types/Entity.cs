using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Alliance { friendly, hostile };

public class Entity : MonoBehaviour
{
    protected GameController gc;
    public GameObject attackReticle;
    public Animator animator;

    public Entity currentTarget;
    public Empty currentAttack;
    public string currentAnim;

    public bool gone, dead;

    public int id;

    public Alliance alliance;

    public int loc = 0;
    
    [System.Serializable]
    public struct Resource
    {
        public int trueMax, max, current;

        public void TrueReset()
        {
            current = max = trueMax;
        }

        public void Reset()
        {
            current = max;
        }
    }

    public Resource hp, mov;

    public GameObject healthBar;
    public float healthBarPos = 2.2f;

    [System.Serializable]
    public struct Stat
    {
        public int max, current;

        public void Reset()
        {
            current = max;
        }
    }

    public Stat atk, def, spd;

    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    public SpriteRenderer spRenderer;
    protected BoxCollider2D box2d;

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

        atk.Reset();
        def.Reset();
        spd.Reset();

        attackReticle = Instantiate(gc.attackReticle, transform, false);
        attackReticle.SetActive(false);

        healthBar = Instantiate(gc.healthBar, gc.mainCanvas.transform);
        healthBar.name = "HealthBar(" + name + id + ")";
    }

    public void Update()
    {
        if (gc.state == GameState.inBattle)
        {
            float x = -((gc.currentBattle.Count-1) * GameController.unitWidth / 2) + loc * GameController.unitWidth;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(x, GameController.floorY), 10 * Time.deltaTime);
        }
        else
        {
            //TODO: Program exploring AI
            transform.position = Vector3.MoveTowards(transform.position, Vector3.up * GameController.floorY, 10 * Time.deltaTime);
        }

        UpdateHealthBar();
    }

    protected void UpdateHealthBar()
    {
        healthBar.transform.position = transform.position + Vector3.up * healthBarPos;

        var health = healthBar.transform.Find("Health");
        health.localScale = new Vector3(hp.current / (float)hp.max, 1);

        var text = healthBar.transform.Find("Text").GetComponent<Text>();
        text.text = "HP: " + hp.current;
    }

    public virtual void Move()
    {

    }
    
    public virtual void Hurt(int dmg, Entity attacker = null)
    {
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

    public void Attack()
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

    protected bool IsAdjacent(Entity target)
    {
        return Mathf.Abs(loc - target.loc) <= 1;
    }
}
