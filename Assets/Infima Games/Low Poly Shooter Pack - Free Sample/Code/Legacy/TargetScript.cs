using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour
{
    [Header("Customizable Options")]
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;

    [Header("Audio")]
    [SerializeField] private AudioClip upSound;
    [SerializeField] private AudioClip downSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Animations")]
    [SerializeField] private AnimationClip targetUp;
    [SerializeField] private AnimationClip targetDown;

    [Header("Bullet Layer")]
    [SerializeField] private LayerMask bulletLayer;

    private float randomTime;
    private bool routineStarted;
    private bool isHit;

    public bool IsHit { get => isHit; set => isHit = value; }

    private void Update()
    {
        if (!isHit || routineStarted) return;

        randomTime = Random.Range(minTime, maxTime);
        GetComponent<Animation>().clip = targetDown;
        GetComponent<Animation>().Play();

        audioSource.clip = downSound;
        audioSource.Play();

        StartCoroutine(DelayTimer());
        routineStarted = true;
    }

    private IEnumerator DelayTimer()
    {
        yield return new WaitForSeconds(randomTime);
        GetComponent<Animation>().clip = targetUp;
        GetComponent<Animation>().Play();

        audioSource.clip = upSound;
        audioSource.Play();

        isHit = false;
        routineStarted = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((bulletLayer.value & (1 << other.gameObject.layer)) != 0)
            isHit = true;
    }
}
