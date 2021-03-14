using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//SCRIPT QUE GESTIONA LA INTELIGENCIA ARTIFICIAL DE LOS ANIMADORES.
public class AnimatorAI : MonoBehaviour
{
    //Capas necesarias para la detección correcta del jugador.
    public LayerMask playerAndObstaclesMask;
    public LayerMask playerMask;

    //Si el animador ha visto al jugador y lo está persiguiendo.
    bool alerted;

    //Si el jugador ha dado monedas al animador y está satisfecho.
    [HideInInspector] public bool satisfied;

    //Si esta yendo o volviendo al punto de patrulla.
    bool returning = false;

    //Si está bebiendo quieto en el cafetera un café.
    bool drinking = false;

    //Si el jugador está en su campo de visión.
    bool inFOV;

    //El jugador.
    GameObject player;

    //Script que maneja la destinación del Animador.
    AIDestinationSetter AIDestinationSetter;

    //Punto de patrulla
    Transform patrolPoint;
    //Punto de comienzo de la partida
    Transform originPoint;

    //Generadores de sonido.
    AudioSource[] ASs;

    //Sonidos.
    public List<AudioClip> closeSounds;
    public List<AudioClip> shoutsSounds;
    public List<AudioClip> satisfiedSounds;

    void Awake()
    {
        AIDestinationSetter = GetComponent<AIDestinationSetter>();
        player = GameObject.Find("Player");

        //Coje los puntos de patrulla.
        patrolPoint = transform.GetChild(0).transform;
        originPoint = transform.GetChild(1).transform;

        //Los hace invisibles.
        Destroy(transform.GetChild(0).GetComponent<SpriteRenderer>());
        Destroy(transform.GetChild(1).GetComponent<SpriteRenderer>());

        //Los quita como hijos.
        patrolPoint.parent = null;
        originPoint.parent = null;

        //Coge los generadores de sonido.
        ASs = GetComponents<AudioSource>();

        //Si el punto de patrulla está a más de un metro, ir hacia allí y volver infinitamente.
        if (Vector2.Distance(patrolPoint.position, transform.position) > 1f)
        {
            //Poner como destino el punto de patrulla.
            AIDestinationSetter.target = patrolPoint;
            //Hacer ruido de caminar.
            ASs[0].Play();
        }
        //En caso contrario quedarse quieto y hacer que las piernas se estén quietas.
        else {
            AIDestinationSetter.enabled = false;
            GetComponentInChildren<Animator>().SetBool("Walking", false);
        }

        //Aleatoriamente los animadores hacen ruídos. Esto activa un temporizador que se encarga de ello.
        StartCoroutine(randomSoundTimer());
    }

    void Update()
    {
        //Si el animador no ha visto aún al jugador peró sí que está en su campo de visión:
        if (alerted == false && inFOV == true)
        {
            //Dispara un rallo hacia el jugador y mirá si hay algo tapando su visión.
            Vector2 direction = player.transform.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, playerAndObstaclesMask);

            //Si el rallo ha dado directamente al jugador sin chocar con algún obstaculo:
            if (hit != false && hit.collider.gameObject.layer == 10)
            {
                //Alerta al animador.
                alerted = true;
                Alert();
            }
        }

        //Si el animador no ha sido alertado. 
        if (alerted == false)
        {
            //Si no está satisfecho.
            if (satisfied == false)
            {
                //Ir hacia el punto de patrulla y volver. Y comprobar las distancias para saber cuando dar media vuelta.
                if (returning == false)
                {
                    if (Vector2.Distance(transform.position, patrolPoint.position) < 1f)
                    {
                        returning = true;
                        AIDestinationSetter.target = originPoint;
                    }
                }
                else
                {
                    if (Vector2.Distance(transform.position, originPoint.position) < 1f)
                    {
                        returning = false;
                        AIDestinationSetter.target = patrolPoint;
                    }
                }
            }
            //Si esta satisfecho y no está bebiendo:
            else {
                //Mirar si ha llegado a la cafetera:
                if (Vector2.Distance(transform.position, AIDestinationSetter.target.position) < 1f && drinking == false)
                {
                    //En cuanto llegue a la cafetera beber un café y activar el temporizador de beber.
                    drinking = true;
                    StartCoroutine(waitAtCoffeeMachine());
                }
            }
        }
    }

    //Temporizador que aleatoriamente genera sónidos cada X rato. Infinitamente.
    IEnumerator randomSoundTimer() {
        yield return new WaitForSeconds(Random.Range(5, 20));
        if (alerted == false && satisfied == false)
        {
            ASs[1].clip = closeSounds[Random.Range(0, closeSounds.Count)];
            ASs[1].Play();
        }

        StartCoroutine(randomSoundTimer());
    }

    //Tiempo de espera en la máquina de café.
    IEnumerator waitAtCoffeeMachine() {
        //Bebe durante 15 segundos.
        yield return new WaitForSeconds(15f);

        //Deja de beber, vuelve a estar en el modo en el cual puede ver al jugador, y vuelve a su punto de origen para patrullar de nuevo.
        alerted = false;
        satisfied = false;
        drinking = false;
        returning = true;
        GetComponent<PolygonCollider2D>().enabled = true;

        AIDestinationSetter.target = originPoint;
        AIDestinationSetter.enabled = true;

        //Cambia la luz de visión de verde a rojo.
        GetComponentInChildren<Light2D>().color = new Color(1, 0, 0);
        GetComponentInChildren<Light2D>().enabled = true;
    }

    //Alertar al Animador.
    void Alert() {
        if (satisfied == false)
        {
            //Emite un sonido de que ha sido Alertado.
            ASs[1].clip = shoutsSounds[Random.Range(0, shoutsSounds.Count)];
            ASs[1].Play();

            //Se asegura que sus piernas estén haciendo una animación de caminar.
            GetComponentInChildren<AnimatorLegsScript>().anim.SetBool("Walking", true);

            //Se alerta y pone como objetivo la posición del jugador.
            alerted = true;
            AIDestinationSetter.target = player.transform;
            AIDestinationSetter.enabled = true;

            //Desactiva su luz para que sea más díficil de ver.
            GetComponentInChildren<Light2D>().enabled = false;
            GetComponent<PolygonCollider2D>().enabled = false;

            //Al cabo de un segundo comprueba si el jugador está cerca suyo para matarlo. (Esto sirve por si el jugador le ha venido por la espalda, es un poco raro pero funciona).
            StartCoroutine(closeQuartersKillCountdown());
        }
    }

    //Al cabo de un segundo comprueba si el jugador está cerca suyo para matarlo. (Esto sirve por si el jugador le ha venido por la espalda, es un poco raro pero funciona).
    IEnumerator closeQuartersKillCountdown() {
        yield return new WaitForSeconds(1f);
        if (satisfied == false && Physics2D.OverlapCircle(transform.position, GetComponent<CircleCollider2D>().radius, playerMask))
        {
            GameObject.Find("_DATABASE").GetComponent<Database>().RestartLevel();
        }
    }

    //Detecta que el jugador a entrado en su campo de visión.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si és el jugador:
        if (collision.gameObject.layer == 10)
        {
            //Y no está satisfecho.
            if (satisfied == false)
            {
                //Activa que está en su campo de visión y cuando pase al siguiente fotograma del juego disparará rayos al jugador para ver si lo tiene en su campo de visión.
                inFOV = true;

                //Si ya estaba alertado y el jugador a entrado en su Trigger significa que ha tocado al jugador y por lo tanto lo mata y reinicia el nivel:
                if (alerted == true)
                {
                    GameObject.Find("_DATABASE").GetComponent<Database>().RestartLevel();
                }
            }
        }
    }

    //Satisface al Animador, haciendo que ignore al jugador y se vaya a la cafetera.
    public void Satisfy() {
        //Emite un sonido de satisfacción.
        ASs[1].clip = satisfiedSounds[Random.Range(0, satisfiedSounds.Count)];
        ASs[1].Play();

        //Se desalerta por si ya estaba alertado y se satisface.
        satisfied = true;
        alerted = false;

        //Se dirige a la cafetera.
        AIDestinationSetter.target = GameObject.FindGameObjectWithTag("CoffeeMachine").transform;
        AIDestinationSetter.enabled = true;

        //Cambia el color de su luz a verde.
        GetComponentInChildren<Light2D>().color = new Color(0, 1, 0);
        GetComponentInChildren<Light2D>().enabled = true;

        GameObject.Find("_DATABASE").GetComponent<Database>().satisfiedEnemies++;
    }

    //Si el jugador sale del Trigger, este ya no estará en su campo de visión.
    private void OnTriggerExit2D(Collider2D collision)
    {
        inFOV = false;
    }
}
