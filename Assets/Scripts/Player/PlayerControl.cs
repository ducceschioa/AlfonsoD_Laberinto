using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

//ESTE SCRIPT SIRVE PARA EL CONTROL TOTAL DEL JUGADOR, MOVIMIENTO, INTERACCIONES, DISPARO.
public class PlayerControl : MonoBehaviour
{
    //Script que contiene botones del New Input System de Unity
    [HideInInspector] public Inputs controls;

    //Script que contiene toda la información del progreso de jugador
    [HideInInspector] public Database database;

    //Control de movimiento
    float topSpeed = 2f;
    float accelerationSpeed = 10f;
    float curSpeed;

    Vector3 moveDirection;
    Vector3 lastMoveDirection;

    Rigidbody2D rb;

    bool vertical;
    bool horizontal;

    [HideInInspector] public bool walking;
    bool playingWalkSound;

    //Script de control de animación de las piernas
    LegsScript LS;

    //Escopeta y monedas
    bool fireCoolingDown;

    public GameObject coin20;
    public GameObject coin10;
    GameObject coinEjector;

    //Capas con las que el jugador puede interactuar (Puertas, Botón de la luz).
    public LayerMask interactableObjects;

    void Awake()
    {
        database = GameObject.Find("_DATABASE").GetComponent<Database>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        controls = new Inputs();

        controls.Player.Vertical.started += _ => MovingVertical();
        controls.Player.Vertical.canceled += _ => ExitVertical();
        controls.Player.Horizontal.started += _ => MovingHorizontal();
        controls.Player.Horizontal.canceled += _ => ExitHorizontal();

        coinEjector = transform.Find("LightHolder").transform.Find("CoinEjector").gameObject;

        LS = GetComponentInChildren<LegsScript>();
    }


    //Disparo de la escopeta
    void Fire(bool leftClick) {
        if (database.paused == false && database.isMessageActive() == false && database.levelCompleted == false)
        {
            if (fireCoolingDown == false && database.coins > 0)
            {
                //Activar sonido disparo
                AudioSource[] ass = GetComponents<AudioSource>();
                ass[1].Play();

                //Retroceso creado sobre el jugador al disparar
                rb.velocity = Vector2.zero;
                Vector3 targetUp = GetComponentInChildren<PlayerLightScript>().lastRotation * Vector3.up;

                Vector2 force = (targetUp * -1) * new Vector2(1, 1);

                rb.AddForce(force, ForceMode2D.Impulse);

                fireCoolingDown = true;

                //Desactiva las luces de la linterna del jugador y activa las de la escopeta
                GameObject[] list = GameObject.FindGameObjectsWithTag("PlayerLights");

                for (int i = 0; i < list.Length; i++)
                {
                    list[i].GetComponent<Light2D>().enabled = false;
                }

                transform.Find("LightHolder").GetChild(0).GetChild(0).GetComponent<Light2D>().enabled = true;
                transform.Find("LightHolder").GetChild(0).GetChild(1).GetComponent<Light2D>().enabled = true;


                //Determina que tipo de disparo es (único o múltiple)
                if (leftClick == true)
                {
                    //Disparo único
                    database.coins -= 10;
                    database.IncreasePoints(0);

                    GameObject coin = Instantiate(coin10, coinEjector.transform.position, Quaternion.FromToRotation(coinEjector.transform.up, Vector3.up));
                    coin.GetComponent<Rigidbody2D>().AddForce(coinEjector.transform.up * 2f, ForceMode2D.Impulse);
                }
                else
                {
                    //Disparo múltiple, dispara tres monedas a la vez si hay suficiente dinero
                    int max = 3;
                    int counter = 0;
                    while (counter < max && database.coins > 0f)
                    {
                        database.coins -= 10;
                        counter++;

                        GameObject coin = Instantiate(coin10, coinEjector.transform.position, Quaternion.FromToRotation(coinEjector.transform.up, Vector3.up));
                        coin.GetComponent<Rigidbody2D>().AddForce(coinEjector.transform.up * Random.Range(2, 5), ForceMode2D.Impulse);
                    }

                    database.IncreasePoints(0);
                }

                //Activa un temporizador para cuando acabe el tiempo de disparo, se vuelvan a activar las luces
                StartCoroutine(FireTimer());
            }

            //En caso de que al jugador no le queden monedas se activa un sonido de cargador vacio
            else if (database.coins == 0)
            {
                AudioSource[] ass = GetComponents<AudioSource>();
                if (ass[2].isPlaying == false)
                {
                    ass[2].Play();
                }
            }
        }
    }

    //Activa un temporizador para cuando acabe el tiempo de disparo, se vuelvan a activar las luces
    IEnumerator FireTimer() {
        //Espera 0.025 segundos
        yield return new WaitForSeconds(0.025f);

        //Reactiva las luces y apaga las de la escopeta
        transform.Find("LightHolder").GetChild(0).GetChild(0).GetComponent<Light2D>().enabled = false;
        transform.Find("LightHolder").GetChild(0).GetChild(1).GetComponent<Light2D>().enabled = false;

        GameObject[] list = GameObject.FindGameObjectsWithTag("PlayerLights");

        if (database.GetComponent<Settings>().LightQuality() == true)
        {
            for (int i = 0; i < list.Length; i++)
            {
                list[i].GetComponent<Light2D>().enabled = true;
            }
        }
        else {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].name == "PlayerLight" || list[i].name == "Light")
                {
                    list[i].GetComponent<Light2D>().enabled = true;
                }
            }
        }

        //Espera un segundo para no dejar al jugador disparar muchas veces seguidas
        yield return new WaitForSeconds(1f);

        //Libera al jugador de no poder disparar
        fireCoolingDown = false;
    }

    void Update()
    {
        //Pulsa E para activar un objeto
        if (Input.GetKeyDown(KeyCode.E))
        {
            Use();
        }

        //Determina si haces un disparo único o un disparo múltiple
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Fire(true);
            }
            else
            {
                Fire(false);
            }
        }

        //Código que hace que el jugador se mueva cuando el jugador pulsa WASD. Si el jugador acaba de disparar el control de movimiento está bloqueado.
        if (fireCoolingDown == false && ( vertical == true || horizontal == true)) {

            walking = true;

            //Activa el sonido de andar
            if (playingWalkSound == false) {
                playingWalkSound = true;
                AudioSource[] ass = GetComponents<AudioSource>();
                ass[0].Play();
            }

            //Avisa al script de control de piernas que el jugador se esta moviendo.
            LS.Walk();

            //Aceleración del personaje.
            if (curSpeed < topSpeed) {
                if (curSpeed + accelerationSpeed * Time.deltaTime <= topSpeed) {
                    curSpeed += accelerationSpeed * Time.deltaTime;
                }
                else {
                    curSpeed = topSpeed;
                }
            }

            //Calculo de la dirección del jugador.
            moveDirection = new Vector3(controls.Player.Horizontal.ReadValue<float>(), controls.Player.Vertical.ReadValue<float>(), 0f).normalized;
            lastMoveDirection = moveDirection;
        }
        else {
            walking = false;

            //Desactiva el sonido de andar
            if (playingWalkSound == true)
            {
                playingWalkSound = false;
                AudioSource[] ass = GetComponents<AudioSource>();
                ass[0].Stop();
            }

            //Avisa al script de control de piernas que el jugador se ha parado
            LS.Idle();

            //Deaceleración del personaje.
            if (curSpeed > 0f) {
                if (curSpeed - accelerationSpeed * Time.deltaTime >= 0f) {
                    curSpeed -= accelerationSpeed * Time.deltaTime;
                }
                else {
                    curSpeed = 0f;
                }
            }

            //Mientras el jugador se desacelera se sigue moviendo en la última dirección que ha especificado.
            if (curSpeed > 0f) {
                moveDirection = lastMoveDirection;
            }
            else {
                moveDirection = Vector3.zero;
            }
        }

        //Si el jugador no acaba de disparar, mover personaje en la dirección marcada por teclado.
        if (fireCoolingDown == false)
        {
            //Asigna la dirección de movimiento del Rigidbody del jugador.
            rb.velocity = moveDirection * curSpeed;
        }
    }

    //Al inicio del script activa el Script de controles del New Input System de Unity.
    private void OnEnable()
    {
        controls.Enable();
    }

    //Activado cuando el jugador se esta moviendo en el eje de las Y
    void MovingVertical()
    {
        vertical = true;
    }

    //Activado cuando el jugador ya no se esta moviendo en el eje de las Y
    void ExitVertical()
    {
        vertical = false;
    }

    //Activado cuando el jugador se esta moviendo en el eje de las X
    void MovingHorizontal()
    {
        horizontal = true;
    }

    //Activado cuando el jugador ya no se esta moviendo en el eje de las X
    void ExitHorizontal()
    {
        horizontal = false;
    }

    //Utilizar objeto (Puertas, Botón de la luz).
    void Use()
    {
        //Mira si en un radio determinado hay algún objeto con el que se pueda interactuar. Estos objetos deben de formar parte de una de las capas especificadas como
        //interactuables.
        Collider2D collision = Physics2D.OverlapCircle(transform.position, GetComponent<CircleCollider2D>().radius, interactableObjects);

        //Si ha encontrado un objeto:
        if (collision != null)
        {
            //Si es un botón de la luz:
            if (collision.GetComponent<SwitchScript>() != null)
            {
                //Activar botón.
                collision.GetComponent<SwitchScript>().Activate();
            }
            //Si es una puerta:
            else if (collision.GetComponent<DoorScript>() != null) {
                //Abrir puerta.
                collision.GetComponent<DoorScript>().Activate();
            }
        }
    }

    //Cuando un objeto entra dentro del Trigger de detección de objetos del jugador:
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si forma parte de una de las capas interactuables.
        if (interactableObjects == (interactableObjects | (1 << collision.gameObject.layer)))
        {
            //Llamar a la base de datos y pedir que enseñe por pantalla al jugador que puede interactuar con este objeto.
            database.ShowInteractablePrompt();
        }
    }

    //Cuando un objeto sale del Trigger de detección de objetos del jugador:
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Si forma parte de una de las capas interactuables.
        if (interactableObjects == (interactableObjects | (1 << collision.gameObject.layer)))
        {
            //Llamar a la base de datos y pedir que quite de la pantalla que el jugador puede interactuar con este objeto.
            database.HideInteractablePrompt();
        }
    }
}
