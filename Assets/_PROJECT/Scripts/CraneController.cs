using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CraneController : MonoBehaviour
{
    public List<GameObject> rocketPartPrefabs;
    [SerializeField] private HingeJoint _hingeJoint;
    public float initialForce;
    public float heightGain;
    private Vector3 oldPartPosition;
    public List<RocketPart> rocketParts;
    public float explosionForce;
    public float explosionRadius;

    public bool gameOver;
    public LineRenderer lr;
    public Vector3[] lrPositions;
    public float highestPartHeight;

    public UIController ui;

    public CinemachineTargetGroup targetGroup;
    public CinemachineVirtualCamera targetCamera;


    public List<Material> randomMats;
    
    //Game Over UI
    public Button restartButton;
    public float revealDuration;
    public float revealEndTime;
    public TextMeshProUGUI gameOverScore;
    public TextMeshProUGUI alltimeScore;
    public CanvasGroup gameOverGroup;
    
    //Audio
    public AudioSource audioSource;
    public AudioClip spawnBlockClip;
    public AudioClip explodeClip;

    private int lastRandom;
    private void Start()
    {
        highestPartHeight = rocketParts[0].transform.position.y;
        lrPositions = new Vector3[2];
        lrPositions[0] = transform.position;
        oldPartPosition = transform.position + (Vector3.down * 2);
        GenerateNewPart();
    }

    private void Update()
    {
        if (_hingeJoint.connectedBody != null)
        {
            lrPositions[1] = _hingeJoint.connectedBody.position;
        }
        else
        {
            lrPositions[1] = transform.position;
        }

        lr.SetPositions(lrPositions);
    }

    public void PlayPlop()
    {
        audioSource.PlayOneShot(spawnBlockClip);
    }
    
    //Drop the current rocket part
    public void DropRocketpart(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && _hingeJoint.connectedBody != null)
        {
            
            oldPartPosition = _hingeJoint.connectedBody.position;

            _hingeJoint.connectedBody.GetComponent<RocketPart>().locked = false;
            
            _hingeJoint.connectedBody = null;
        }
    }

    public void GameOver()
    {
        
        if (gameOver) return;
        gameOver = true;
        
        audioSource.PlayOneShot(explodeClip);
        
        for (int i = 0; i < rocketParts.Count; i++)
        {
            Destroy(rocketParts[i].GetComponent<SpringJoint>());
        }
        
        for (int i = 0; i < rocketParts.Count; i++)
        {
            rocketParts[i].rb.AddExplosionForce(explosionForce, rocketParts[i/2].transform.position, explosionRadius);
        }

        targetCamera.m_Priority = 12;

        StartCoroutine(RevealEndOfGame());
    }

    public IEnumerator RevealEndOfGame()
    {
        gameOverGroup.gameObject.SetActive(true);
        revealEndTime = Time.time + revealDuration;
        gameOverScore.text = $"Height: {highestPartHeight:0.00}m";
        alltimeScore.text = $"Best: {PlayerPrefs.GetFloat("Highscore"):0.00}m";
        while (true)
        {
            float diff = revealEndTime - Time.time; 
            if (diff > 0)
            {
                gameOverGroup.alpha = 1 - diff;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }


    //Generate a new rocket part to attach to the crane
    public void GenerateNewPart()
    {
        if (gameOver) return;
        
        GameObject newPart = Instantiate(rocketPartPrefabs[Random.Range(0, rocketPartPrefabs.Count)], oldPartPosition, Quaternion.identity);
        RocketPart rocketPart = newPart.GetComponent<RocketPart>();
        Rigidbody rocketPartRb = newPart.GetComponent<Rigidbody>();
        MeshRenderer rocketRenderer = newPart.GetComponent<MeshRenderer>();

        
        
        int random = Random.Range(0, randomMats.Count);
        
        while (random == lastRandom)
        {
            random = Random.Range(0, randomMats.Count);
        }

        lastRandom = random;
        
        rocketRenderer.material = randomMats[random];
        
        _hingeJoint.connectedBody = rocketPartRb;
        rocketPart.locked = true;

        int rand = Random.Range(0, 2) * 2 - 1;
        
        rocketPartRb.AddForce(new Vector3((float) rand * initialForce, 0, 0));
        rocketPart.controller = this;

        for (int i = 0; i < rocketParts.Count; i++)
        {
            if (rocketParts[i].transform.position.y > highestPartHeight)
            {
                highestPartHeight = rocketParts[i].transform.position.y;
            }
        }

        ui.UpdateHeight(highestPartHeight);
        
        transform.position = new Vector3(transform.position.x, highestPartHeight + 5, transform.position.z);
        lrPositions[0] = transform.position;
    }
    
}
