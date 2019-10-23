using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] float health = 200;
    [SerializeField] int scoreValue = 100;

    [Header("Projectile")]
    [SerializeField] GameObject enemyLazerPrefab;
    [SerializeField] float projectileSpeed = 10f;
    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 1f;
    [SerializeField] float maxTimeBetweenShots = 2f;

    [Header("VFX")]
    [SerializeField] GameObject explosionVFXPrefab;
    [SerializeField] float explosionVFXDuration = 1f;

    [Header("SoundFX")]
    [SerializeField] AudioClip deathSFX;
    [SerializeField][Range(0,1)] float deathSFXVolume = 0.75f;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] [Range(0, 1)] float shootSFXVolume = 0.5f;

    GameSession gameSession;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        gameSession = FindObjectOfType<GameSession>();
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) //Other refers to trigger collide with this gameObjects collider(Laser for this condition)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer)  { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();            
        }
    }

    private void Die()
    {        
        Destroy(gameObject);
        GameObject explosion = Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, explosionVFXDuration);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
        gameSession.AddToScore(scoreValue);
    }

    private void Fire()
    {
        GameObject enemyLazer = Instantiate(enemyLazerPrefab,
            transform.position, Quaternion.identity) as GameObject;
        enemyLazer.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
        AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);
    }
    
}
