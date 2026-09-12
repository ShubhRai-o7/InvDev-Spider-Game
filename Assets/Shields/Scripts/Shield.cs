using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Shield : MonoBehaviour
{
    public Renderer _renderer;
    [SerializeField] AnimationCurve _DisplacementCurve;
    [SerializeField] float _DisplacementMagnitude;
    [SerializeField] float _LerpSpeed;
    [SerializeField] float _DisolveSpeed;
    [HideInInspector] public bool _shieldOn;
    Coroutine _disolveCoroutine;

    public GameObject legs;

    public Animator energyDecrease;

    [HideInInspector]public bool canShield = true;

    public static Shield instance;

    public Volume PostVolume;
    private ChromaticAberration aberration;
    private Vignette vignette;

    private float currentVignetteValue;
    private float currentAberrationValue;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        instance = this;

        PostVolume.profile.TryGet(out aberration);
        PostVolume.profile.TryGet(out vignette);
        currentVignetteValue = vignette.intensity.value;
        currentAberrationValue = aberration.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {

        

        if (Input.GetKey(KeyCode.F) && canShield && !SlowMotion.instance.isSlowed && SlowMotion.instance.currentEnergy > 0)
        {
            StartCoroutine(Coroutine_GenerateShield());
        }
        if(Input.GetKeyUp(KeyCode.F) && !canShield)
        {
            StopCoroutine(Coroutine_GenerateShield());
            StartCoroutine(Coroutine_DisolveShield());
        }

        if(_renderer.material.GetFloat("_Disolve") <= 0.4f)
        {
            gameObject.GetComponent<BoxCollider>().enabled = true;
            _shieldOn = true;
            PlayerController.instance.canMove = false;
        }
        else
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
            _shieldOn = false;
            PlayerController.instance.canMove = true;
        }
    }

    public void HitShield()
    {
        //_renderer.material.SetVector("_HitPos", hitPos);
        StopAllCoroutines();
        StartCoroutine(Coroutine_HitDisplacement());
    }

    public IEnumerator Coroutine_HitDisplacement()
    {
        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat("_DisplacementStrength", _DisplacementCurve.Evaluate(lerp) * _DisplacementMagnitude);
            lerp += Time.deltaTime*_LerpSpeed;
            yield return null;
        }
    }
    IEnumerator Coroutine_GenerateShield()
    {
        
        canShield = false;
        energyDecrease.SetBool("EnergyDeplete", true);
        float lerp = 0;
        
        while (lerp < 1)
        {
            _renderer.material.SetFloat("_Disolve", Mathf.Lerp(1, 0, lerp));
            lerp += Time.deltaTime * _DisolveSpeed;
            SlowMotion.instance.currentEnergy -= 10f * Time.deltaTime;
            //vignette.intensity.value += 0.2f;
            //aberration.intensity.value += 0.4f;

            yield return null;
        }
        while (Input.GetKey(KeyCode.F))
        {
            SlowMotion.instance.currentEnergy -= 10f * Time.deltaTime;

            if (SlowMotion.instance.currentEnergy <= 0)
            {
                StartCoroutine(Coroutine_DisolveShield());
                yield break;

            }
            yield return null;
        }     
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<SuicideBomber>().Explode();
        }
    }
    IEnumerator Coroutine_DisolveShield()
    {

        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat("_Disolve", Mathf.Lerp(0, 1, lerp));
            lerp += Time.deltaTime * _DisolveSpeed;
            yield return null;
        }
        
        canShield = true;
        energyDecrease.SetBool("EnergyDeplete", false);
    }
}
