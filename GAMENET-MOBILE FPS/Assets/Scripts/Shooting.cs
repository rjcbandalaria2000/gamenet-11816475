using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.Assertions;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera Camera;
    public GameObject HitEffectPrefab;
    public float range = 200;
    public float damage = 25;
    public Coroutine RespawnCoroutine;
    public GameObject RespawnText;
    public GameObject KillNotificationPrefab;
    public int PlayerKills = 0;

    [Header("HP Related")]
    public float StartHealth = 100;
    public Image HealthBar;

    private float health;
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        health = StartHealth;
        HealthBar.fillAmount = health / StartHealth;
        animator = this.GetComponent<Animator>();
        RespawnText = GameObject.Find("Respawn Text");
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Fire()
    {
        RaycastHit hit;
        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log(hit.collider.gameObject.name);
            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point); // only use RPCTarget.All because entering players dont need to see the created hit effects
            if (hit.collider.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25, this.gameObject);
            }
        }
    }
    [PunRPC]
    public void TakeDamage(int damage, GameObject source, PhotonMessageInfo info) // PhotonMessageInfo 
    {
        this.health -= damage;
        this.HealthBar.fillAmount = health/StartHealth;
        if(health <= 0)
        {
            source.GetComponent<Shooting>().PlayerKills++;
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName); // Sender is the one calling the RPC (the one inflicting the damage)
            // Owner is the one killed  
            GameObject killNotification = Instantiate(KillNotificationPrefab);//PhotonNetwork.Instantiate(KillNotificationPrefab.name, killFeedUI.transform.position, Quaternion.identity
            killNotification.transform.SetParent(UIManager.Instance.KillFeedUI.transform);
            killNotification.transform.localScale = Vector3.one; 
            killNotification.transform.Find("KillerText").gameObject.GetComponent<TextMeshProUGUI>().text = info.Sender.NickName;
            killNotification.transform.Find("KilledText").gameObject.GetComponent<TextMeshProUGUI>().text = info.photonView.Owner.NickName;
            Destroy(killNotification, 3.0f);

            GameManager.Instance.CheckWinCondition();
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(HitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die() {

        if (photonView.IsMine)
        {
            animator.SetBool("IsDead", true);
            RespawnCoroutine = StartCoroutine(RespawnCountdown());
        }
    
    }

    IEnumerator RespawnCountdown()
    {
        Assert.IsNotNull(RespawnText, "Respawn text not found");
        float respawnTime = 5.0f;
        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;
            transform.GetComponent<PlayerMovementController>().enabled = false;
            RespawnText.GetComponent<TextMeshProUGUI>().text = "You are killed. Respawning in " + respawnTime.ToString(".00");
        }
        animator.SetBool("IsDead", false);
        RespawnText.GetComponent<TextMeshProUGUI>().text = "";
        //int randomPointX = Random.Range(-20, 20);
        //int randomPointZ = Random.Range(-20, 20);
        this.transform.position = SpawnManager.Instance.GetRandomSpawnPoint().position;//new Vector3(randomPointX, 0, randomPointZ);
        Debug.Log("Respawn");
        this.transform.GetComponent<PlayerMovementController>().enabled = true;
        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = StartHealth;
        HealthBar.fillAmount = health / StartHealth;
    }

    IEnumerator DisplayKillFeed(PhotonMessageInfo info)
    {
        yield return null; 
    }
}
