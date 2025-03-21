using DTT.AreaOfEffectRegions;
using EverScord;
using EverScord.Character;
using EverScord.Effects;
using EverScord.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossRPC : MonoBehaviour, IEnemy
{
    public Dictionary<string, float> clipDict = new();
    public BossData BossMonsterData => bossData;
    
    [SerializeField] private BossData bossData;
    [SerializeField] private GameObject laserPoint;
    [SerializeField] private GameObject projectorObj_Pattern4;
    [SerializeField] private GameObject projectorObj_Pattern5;
    private SRPLineRegionProjector projectorPattern4;
    private SRPArcRegionProjector projectorPattern5;

    [SerializeField] private GameObject fogPlane;
    [SerializeField] private GameObject safeZone;
    private GameObject jumpEffectObject;
    private ParticleSystem jumpEffect;

    private PhotonView photonView;
    private Animator animator;
    private CapsuleCollider hitBox;
    private UIMarker uiMarker;
    private NavMeshAgent bossNavMeshAgent;
    private BlinkEffect blinkEffect;

    private GameObject worldCanvas;

    //cur stat

    public float HP { get; private set; }
    public float MaxHP { get; private set; }
    public float BaseAttack { get; private set; }
    public float Defense { get; private set; }
    public float Speed { get; private set; }
    public int Phase { get; private set; }

    public BodyType EnemyBodyType => BodyType.FLESH;

    private bool isDead;

    public static Action OnBossDead = delegate { };

    private void Awake()
    {
        HP = bossData.MaxHP;
        MaxHP = bossData.MaxHP;
        Phase = 1;
        BaseAttack = bossData.BaseAttack1;
        Defense = bossData.Defense1;
        Speed = bossData.Speed1;

        hitBox = GetComponent<CapsuleCollider>();
        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        uiMarker = gameObject.AddComponent<UIMarker>();
        uiMarker.Initialize(PointMarkData.EType.BossMonster);
        bossNavMeshAgent = GetComponent<NavMeshAgent>();
        worldCanvas = GameObject.FindGameObjectWithTag("MonsterUI");

        blinkEffect = BlinkEffect.Create(this);
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            clipDict[clip.name] = clip.length;
        }
        SetProjectors();
    }

    private void OnEnable()
    {
        isDead = false;
        uiMarker.SetActivate(true);
    }

    private void OnDisable()
    {
        uiMarker.SetActivate(false);
    }

    private void Update()
    {
        uiMarker.UpdatePosition(transform.position);
        if (Input.GetKeyDown(KeyCode.F12))
        {
            HP -= 10000;
            Debug.Log($"CurHP = {HP}, {HP / MaxHP * 0.01f}");
        }
    }

    public void SetDebuff(CharacterControl attacker, EBossDebuff debuffState, float time, float value)
    {
        GameManager.Instance.DebuffSystem.SetDebuff(this, debuffState, attacker, time, value);
    }

    private void SetProjectors()
    {
        projectorPattern4 = projectorObj_Pattern4.GetComponent<SRPLineRegionProjector>();
        projectorPattern5 = projectorObj_Pattern5.GetComponent<SRPArcRegionProjector>();

        projectorObj_Pattern4.SetActive(false);
        projectorObj_Pattern5.SetActive(false);
    }

    public void PlayAnimation(string animationName)
    {
        photonView.RPC("SyncBossAnimation", RpcTarget.All, animationName);
    }    
    
    public void PlayAnimation(string animationName, float transitionDuration)
    {
        Debug.Log(animationName);
        photonView.RPC("SyncBossAnimation", RpcTarget.All, animationName, transitionDuration);
    }

    [PunRPC]
    public void SyncBossAnimation(string animationName)
    {
        if (animator == null || photonView == null)
        {
            photonView = GetComponent<PhotonView>();
            animator = GetComponent<Animator>();
        }
        animator.CrossFade(animationName, 0.25f, -1, 0);
    }

    [PunRPC]
    public void SyncBossAnimation(string animationName, float transitionDuration)
    {
        if (animator == null || photonView == null)
        {
            photonView = GetComponent<PhotonView>();
            animator = GetComponent<Animator>();
        }
        animator.CrossFade(animationName, transitionDuration, -1, 0);
    }

    public void PlaySound(string soundName, float volume = 1.0f)
    {
        photonView.RPC(nameof(SyncBossSound), RpcTarget.All, soundName, volume);
    }

    [PunRPC]
    private void SyncBossSound(string soundName, float volume)
    {
        SoundManager.Instance.PlaySound(soundName, volume);
    }

    public void StopSound(string soundName)
    {
        photonView.RPC(nameof(SyncBossStopSound), RpcTarget.All, soundName);
    }

    [PunRPC]
    private void SyncBossStopSound(string soundName)
    {
        SoundManager.Instance.StopSound(soundName);
    }

    public void PlayEffect(string effectName, Vector3 pos)
    {
        photonView.RPC("SyncEffect", RpcTarget.All, effectName, pos);
    }

    [PunRPC]
    public IEnumerator SyncEffect(string effectName, Vector3 pos)
    {
        GameObject go = ResourceManager.Instance.GetFromPool(effectName, pos, Quaternion.identity);
        ParticleSystem particleSystem = go.GetComponent<ParticleSystem>();
        particleSystem.Play();
        yield return new WaitForSeconds(particleSystem.main.duration);
        ResourceManager.Instance.ReturnToPool(go, effectName);
    }

    public void PlayJumpEffect()
    {
        photonView.RPC("SyncJumpEffect", RpcTarget.All);
        PlaySound("BossJump");
    }

    [PunRPC]
    private void SyncJumpEffect()
    {
        if (jumpEffect == null)
        {
            GameObject go = ResourceManager.Instance.GetAsset<GameObject>("BossJumpEffect");
            jumpEffectObject = Instantiate(go);
            jumpEffect = jumpEffectObject.GetComponent<ParticleSystem>();
        }
        jumpEffectObject.transform.position = transform.position;
        jumpEffect.Play();
    }

    public void FireBossProjectile(Vector3 position, Vector3 direction, float damage, float projectileSpeed)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterProjectile", direction, Quaternion.identity);
        MonsterProjectile mp = go.GetComponent<MonsterProjectile>();
        int id;
        if (mp.ID == -1)
        {
            id = GameManager.Instance.ProjectileController.GetIDNum();
            GameManager.Instance.ProjectileController.AddDict(id, mp);
        }
        else
            id = mp.ID;

        mp.Setup("BossProjectile", id, position, direction, BaseAttack, damage, projectileSpeed, true);
        photonView.RPC("SyncBossProjectile", RpcTarget.Others, id, position, direction, damage, projectileSpeed);
    }

    [PunRPC]
    public void SyncBossProjectile(int id, Vector3 position, Vector3 direction, float damage, float projectileSpeed)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterProjectile", direction, Quaternion.identity);
        MonsterProjectile bp = go.GetComponent<MonsterProjectile>();
        GameManager.Instance.ProjectileController.AddDict(id, bp);
        bp.Setup("BossProjectile", id, position, direction, BaseAttack, damage, projectileSpeed, true);
    }

    private IEnumerator FillAmountProjector(int projectorNum, float time)
    {
        float curTime = 0;

        switch (projectorNum)
        {
            case 4:
                {
                    projectorPattern4.FillProgress = 0;
                    float timeOffset = time - 0.2f;
                    while (true)
                    {
                        curTime += Time.deltaTime;
                        projectorPattern4.FillProgress = curTime / timeOffset;
                        projectorPattern4.UpdateProjectors();
                        yield return new WaitForSeconds(Time.deltaTime);
                        if (curTime > timeOffset)
                        {
                            yield break;
                        }
                    }
                }
            case 5:
                {
                    projectorPattern5.FillProgress = 0;
                    float timeOffset = time - 0.2f;
                    while (true)
                    {
                        curTime += Time.deltaTime;
                        projectorPattern5.FillProgress = curTime / timeOffset;
                        projectorPattern5.UpdateProjectors();
                        yield return new WaitForSeconds(Time.deltaTime);
                        if (curTime > timeOffset)
                        {
                            yield break;
                        }
                    }
                }
        }
    }

    public IEnumerator ProjectEnable(int patternNum, float projectTime)
    {
        photonView.RPC(nameof(SyncProjectorEnable), RpcTarget.All, patternNum, projectTime);
        yield return new WaitForSeconds(projectTime + 0.5f);
        photonView.RPC(nameof(SyncProjectorDisable), RpcTarget.All, patternNum);
    }

    [PunRPC]
    protected void SyncProjectorEnable(int patternNum, float projectTime)
    {
        switch (patternNum)
        {
            case 4:
                {
                    projectorObj_Pattern4.SetActive(true);
                    StartCoroutine(FillAmountProjector(patternNum, projectTime));
                    break;
                }
            case 5:
                {
                    projectorObj_Pattern5.SetActive(true);
                    StartCoroutine(FillAmountProjector(patternNum, projectTime));
                    break;
                }
        }
    }

    [PunRPC]
    protected void SyncProjectorDisable(int patternNum)
    {
        switch (patternNum)
        {
            case 4:
                {
                    projectorObj_Pattern4.SetActive(false);
                    break;
                }
            case 5:
                {
                    projectorObj_Pattern5.SetActive(false);
                    break;
                }
        }
    }

    public void SetActivePattern6(bool tf)
    {
        float damage = bossData.SkillDatas[4].SkillDamage;
        laserPoint.GetComponent<BossLaser>().SetDamage(damage, BaseAttack);
        photonView.RPC(nameof(SyncFog), RpcTarget.All, tf);
    }

    [PunRPC]
    private void SyncFog(bool tf)
    {
        if(!tf)
        {
            fogPlane.GetComponent<FogPlane>().DownFog();
        }
        else
            fogPlane.SetActive(tf);
        
        safeZone.SetActive(tf);
        if (tf) 
            safeZone.GetComponent<ParticleSystem>().Play();
    }

    public void SetPositionScaleP6_SafeZone(Vector3 pos, float size)
    {
        photonView.RPC(nameof(SyncPositionScaleP6_SafeZone), RpcTarget.All, pos, size);
    }

    [PunRPC]
    private void SyncPositionScaleP6_SafeZone(Vector3 pos, float size)
    {
        safeZone.transform.position = pos;
        safeZone.transform.localScale = new Vector3(size, 1, size);
    }

    public void MoveP6_SafeZone(float duration, Vector3 endPoint)
    {
        Vector3 startPoint = transform.position + transform.forward * 4;
        photonView.RPC(nameof(SyncP6_SafePosition), RpcTarget.All, duration, startPoint, endPoint);
    }

    [PunRPC]
    private void SyncP6_SafePosition(float duration, Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(MoveSafePos(duration, startPoint, endPoint));
    }

    private IEnumerator MoveSafePos(float duration, Vector3 startPoint, Vector3 endPoint)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            safeZone.transform.position = Vector3.Lerp(startPoint, endPoint, t / duration);
            yield return null;
        }
    }

    public void DecreaseHP(float damage, CharacterControl attacker)
    {
        photonView.RPC(nameof(SyncBossMonsterHP), RpcTarget.All, damage, attacker.CharacterPhotonView.ViewID);
    }

    [PunRPC]
    protected void SyncBossMonsterHP(float damage, int attackerID)
    {
        ReduceHP(damage, attackerID);
        GameObject damageObj = ResourceManager.Instance.GetFromPool("DamageText", transform.position, Quaternion.identity);
        damageObj.transform.SetParent(worldCanvas.transform);
        DamageTextUI damageUI = damageObj.GetComponent<DamageTextUI>();
        damageUI.DisplayDamage(transform, damage, 3);
        GameManager.Instance.LevelController.IncreaseBossProgress(this);
    }

    private void ReduceHP(float decrease, int attackerID)
    {
        HP -= decrease;
        if (HP < 0)
            HP = 0;

        CharacterControl attacker = GameManager.Instance.PlayerDict[attackerID];

        if (!isDead && attacker.CharacterPhotonView.IsMine && HP == 0 && Phase == 2)
        {
            isDead = true;
            attacker.IncreaseKillCount();
            if(PhotonNetwork.IsMasterClient)
            {
                OnBossDead?.Invoke();
            }
        }

        Debug.Log(decrease + " ������, ���� ü�� : " + HP);
    }

    public void PhaseUp()
    {
        photonView.RPC(nameof(SyncPhaseUp), RpcTarget.All);
    }

    [PunRPC]
    private void SyncPhaseUp()
    {
        HP = bossData.MaxHP_Phase2;
        MaxHP = bossData.MaxHP_Phase2;
        Phase++;
        BaseAttack = bossData.BaseAttack2;
        Defense = bossData.Defense2;
        Speed = bossData.Speed2;

        GameManager.Instance.LevelController.IncreaseBossProgress(this);
    }

    public bool IsUnderHP(float ratio)
    {
        if (HP > MaxHP / 100 * ratio)
        {
            return false;
        }
        return true;
    }

    public void LaserEnable(float enableTime)
    {
        laserPoint.GetComponent<BossLaser>().SetDamage(bossData.SkillDatas[4].SkillDamage, BaseAttack);
        photonView.RPC(nameof(SyncLaser), RpcTarget.All, enableTime);
    }

    [PunRPC]
    public IEnumerator SyncLaser(float enableTime)
    {
        laserPoint.SetActive(true);
        yield return new WaitForSeconds(enableTime);
        laserPoint.SetActive(false);
    }

    public void StunMonster(float stunTime)
    {
        return;
    }

    public void InstantiateStoneAttack(Vector3 pos, float width, float projectTime, string effectAddressableKey, float skillDamage, string soundName)
    {
        photonView.RPC(nameof(SyncStoneAttack), RpcTarget.All, pos, width, projectTime, effectAddressableKey, skillDamage, soundName);
    }

    [PunRPC]
    protected void SyncStoneAttack(Vector3 pos, float width, float projectTime, string addressableKey, float skillDamage, string soundName)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterAttack", pos, Quaternion.identity);
        MonsterAttack ma = go.GetComponent<MonsterAttack>();        
        ma.Setup(width, projectTime, addressableKey, BaseAttack, skillDamage, soundName);
    }

    public void InstantiateStoneAttack2(Vector3 pos, float width, float projectTime, string effectAddressableKey, float attackDamage)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("BossMonsterStoneAttack", pos, Quaternion.identity);
        PhotonView view = go.GetComponent<PhotonView>();
        BossMonsterStoneAttack ma = go.GetComponent<BossMonsterStoneAttack>();
        ma.Setup(width, projectTime, effectAddressableKey, attackDamage, BaseAttack);
        if (view.ViewID == 0)
        {
            if (PhotonNetwork.AllocateViewID(view))
            {
                int viewID = view.ViewID;
                photonView.RPC("SyncStoneAttack2", RpcTarget.Others, pos, width, projectTime, effectAddressableKey, attackDamage, viewID);
            }
        }
        else
        {
            photonView.RPC("SyncStoneAttack2", RpcTarget.Others, pos, width, projectTime, effectAddressableKey, attackDamage, null);
        }

    }

    [PunRPC]
    protected void SyncStoneAttack2(Vector3 pos, float width, float projectTime, string addressableKey, float attackDamage, int viewID)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("BossMonsterStoneAttack", pos, Quaternion.identity);
        PhotonView view = go.GetComponent<PhotonView>();
        if(view.ViewID == 0)
        {
            view.ViewID = viewID;
        }
        BossMonsterStoneAttack ma = go.GetComponent<BossMonsterStoneAttack>();
        ma.Setup(width, projectTime, addressableKey, attackDamage, BaseAttack);
    }

    public IEnumerator EnableShield()
    {
        hitBox.enabled = false;
        animator.speed = 0;
        GameObject go = ResourceManager.Instance.GetAsset<GameObject>("P15_Shield");
        GameObject shield = Instantiate(go);
        shield.transform.position = transform.position;
        photonView.RPC("SyncShield", RpcTarget.Others);
        BossShield bossShield = shield.GetComponent<BossShield>();
        bossShield.SetHP(MaxHP * 0.2f);
        yield return new WaitForSeconds(8f);
        if (bossShield.HP > 0)
        {
            PlayEffect("P15_Attack", transform.position);
            PlaySound("BossPattern15_Attack");
            foreach (CharacterControl player in GameManager.Instance.PlayerDict.Values)
            {
                player.DecreaseHP(bossData.SkillDatas[13].MaxHpBasedDamage, true);
            }
            yield return new WaitForSeconds(0.2f);
            PlaySound("BossPattern15_Attack");
        }
        animator.speed = 1;
        Destroy(shield);
        hitBox.enabled = true;
    }

    [PunRPC]
    protected IEnumerator SyncShield()
    {
        hitBox.enabled = false;
        animator.speed = 0;
        GameObject go = ResourceManager.Instance.GetAsset<GameObject>("P15_Shield");
        GameObject shield = Instantiate(go);
        BossShield bossShield = shield.GetComponent<BossShield>();
        bossShield.SetHP(MaxHP * 0.2f);
        shield.transform.position = transform.position;
        yield return new WaitForSeconds(8f);
        animator.speed = 1;
        Destroy(shield);
        hitBox.enabled = true;
    }

    public void Death()
    {
        GameOverControl gameOver = GameManager.Instance.GameOverController;

        if (PhotonNetwork.IsMasterClient)
            GameManager.Instance.StartCoroutine(gameOver.ShowGameover(true, 3f));

        photonView.RPC("SyncBossDeath", RpcTarget.All);
    }

    [PunRPC]
    private void SyncBossDeath()
    {
        //bossDebuffSystem.ClearActions();
        Destroy(gameObject);
    }

    public void TestDamage(GameObject sender, float value)
    {
        throw new System.NotImplementedException();
    }

    public BlinkEffect GetBlinkEffect()
    {
        return blinkEffect;
    }

    public float GetDefense()
    {
        return Defense;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return bossNavMeshAgent;
    }
}
