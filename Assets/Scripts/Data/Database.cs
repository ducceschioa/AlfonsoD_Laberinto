using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//ESTE SCRIPT GESTIONA LA INFORMACIÓN QUE SE MANTIENE ENTRE NIVELES PARA GESTIONAR EL JUEGO.
public class Database : MonoBehaviour
{
    //Nivel actual
    //Menu Principal = 0.
    //Nivel 1 = 1.
    //Nivel 2 = 2.
    //Nivel 3 = 3.
    //Pantalla de Victoria = 4.
    [HideInInspector] public int currentLevel = 0;

    //Vidas
    [HideInInspector] public int lifes;

    //Dinero que posee el jugador
    [HideInInspector] public int coins = 0;

    //Booleano que determina si el juego está pausado
    public bool paused;

    //El jugador esta cambiando de nivel
    [HideInInspector] public bool changingLevel = false;

    //El jugador a silenciado la música
    bool mutedAudio;
    //El volumen de la música esta bajando
    bool decreaseVolume;

    //Imagenes del Sprite del silencio de volumen de la música
    public Sprite muteVolume;
    public Sprite normalVolume;

    //Contador en pantalla del dinero del jugador.
    TMPro.TMP_Text pointsCounter;

    //Elementos de la interfaz
    GameObject pointCounterHolder; //Contenedor del texto del dinero del jugador.
    GameObject PressE; //Contenedor del mensaje en pantalla (Pulsa E para interactuar).
    Image screenfade; //Imagen negra que sirve para hacer transiciones entre un nivel y otro.
    GameObject muteAudio; //Contenedor de la imagen que enseña si la música esta silenciada o no.
    GameObject blueKey; //Contenedor que enseña si el jugador posee la llave azul.
    GameObject redKey; //Contenedor que enseña si el jugador posee la llave roja.
    GameObject message; //Contenedor del mensaje del Manel al inicio de cada nivel.
    GameObject pauseText; //Texto de pausa
    GameObject returnToMenuButton; //Botón de volver al menú principal.
    GameObject lifeCounterHolder; //Contenedor del contador y imagen de las vidas.
    GameObject lifeCounter; //Contador de vidas.

    //Objetos de la pantalla final de cada nivel
    GameObject levelSummaryBackground, levelSummaryTitle, levelSummaryContinue, levelSummaryCoins, levelSummaryEnemies;

    //Una vez ya se ha enseñado un mensaje salta este bloqueador para que no se enseñe más. Sirve por si el jugador muere
    //y se reinicia el nivel.
    [HideInInspector] public bool blockMessage;

    //Lista de las llaves que posee el jugador.
    [HideInInspector] public List<string> keysOwned;

    //Booleano que determina si el nivel esta siendo reiniciado.
    [HideInInspector] public bool restarting;

    //Generadores de sonido de la base de datos (Muerte del jugador, Música, Sonidos de interacción con el menu).
    AudioSource[] ASs;

    //Canciones del juego.
    public List<AudioClip> musicList;

    //Monedas en este nivel
    int currentLevelCoins;

    //Nivel completado.
    [HideInInspector] public bool levelCompleted;

    //Veces en este nivel que un enemigo haya sido sobornado
    [HideInInspector] public int satisfiedEnemies;

    //Contador del tiempo transcurrido de un nivel
    float levelTimer;
    GameObject levelSummaryTime;

    private void Awake() {
        //El jugador comienza con 10 céntimos.
        coins = 10;

        //Fuerzo por si acaso el número de vidas
        lifes = 5;

        //Dice a Unity de hacer que este script se mantenga entre nivel y nivel.
        DontDestroyOnLoad(gameObject);

        //Encuentra y asigna elementos de la interfaz:
        PressE = GameObject.Find("PressE");
        screenfade = GameObject.Find("Screenfade").GetComponent<Image>();
        pointCounterHolder = GameObject.Find("PointCounterHolder");
        pointsCounter = GameObject.Find("PointCounter").GetComponent<TMPro.TMP_Text>();
        muteAudio = GameObject.Find("MuteAudio");
        blueKey = GameObject.Find("BlueKey");
        redKey = GameObject.Find("RedKey");
        message = GameObject.Find("MessageHolder");
        pauseText = GameObject.Find("Pause");
        returnToMenuButton = GameObject.Find("ReturnToMenu");
        lifeCounterHolder = GameObject.Find("LifeCounterHolder");
        lifeCounter = GameObject.Find("LifeCounter");
        levelSummaryBackground = GameObject.Find("LevelSummaryBackground");
        levelSummaryTitle = GameObject.Find("LevelSummaryTitle");
        levelSummaryContinue = GameObject.Find("LevelSummaryContinue");
        levelSummaryCoins = GameObject.Find("LevelSummaryCoins");
        levelSummaryEnemies = GameObject.Find("LevelSummaryEnemies");
        levelSummaryTime = GameObject.Find("LevelSummaryTime");

        //Desactiva elementos de la interfaz que solo se usen durante el juego.
        PressE.SetActive(false);
        pointCounterHolder.SetActive(false);
        blueKey.SetActive(false);
        redKey.SetActive(false);
        message.SetActive(false);
        pauseText.SetActive(false);
        returnToMenuButton.SetActive(false);
        lifeCounterHolder.SetActive(false);
        levelSummaryBackground.SetActive(false);
        levelSummaryTitle.SetActive(false);
        levelSummaryCoins.SetActive(false);
        levelSummaryContinue.SetActive(false);
        levelSummaryEnemies.SetActive(false);
        levelSummaryTime.SetActive(false);

        //Hace que la pantalla en negro desaparezca enseñando el menu principal.
        screenfade.CrossFadeAlpha(0f, 1f, false);

        //Inicializa la lista de llaves.
        keysOwned = new List<string>();

        //Activa la música el menu principal.
        ASs = GetComponents<AudioSource>();
        ASs[1].Play();

    }

    private void Update()
    {

        //Contador del tiempo transcurrido
        levelTimer += Time.deltaTime;

        //Comprueba que el jugador le de al espacio para cerrar el mensaje del Manel.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Reactiva el paso del tiempo.
            Time.timeScale = 1;
            //Activa el sonido de interacción con el menu.
            PlayMenuSound();
            //Esconde el mensaje.
            message.SetActive(false);
        }

        //Al pulsar M se silencia o desilencia la música.
        if (Input.GetKeyDown(KeyCode.M))
        {
            MuteAudio();
        }

        //Entre nivel y nivel se usa esto para hacer un silencio gradual de la música para que no sea muy brusco.
        if (decreaseVolume == true)
        {
            if (ASs[1].volume > 0f)
            {
                ASs[1].volume -= Time.deltaTime;
            }
        }

        //Activar - Desactivar pausa
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Pause();

        }
    }

    //Añadir llave a la lista de llaves que tiene el jugador.
    public void AddKey(string type) {
        if (type == "Red")
        {
            //Enseña por pantalla que el jugador posee la llave.
            redKey.SetActive(true);
            keysOwned.Add("Red");
        }
        else if (type == "Blue"){
            //Enseña por pantalla que el jugador posee la llave.
            blueKey.SetActive(true);
            keysOwned.Add("Blue");
        }
    }

    //Comprueba si el jugador posee esta llave y devuelve la respuesta con un booleano. Esto sirve cuando el jugador está intentando abrir una puerta.
    public bool IsKeyOwned(string key)
    {
        bool found = false;
        for (int i = 0; i < keysOwned.Count && found == false; i++)
        {
            if (keysOwned[i] == key)
            {
                found = true;
            }
        }
        return found;
    }

    //Enseña al jugador por pantalla que si pulsa E puede interactuar con el objeto.
    public void ShowInteractablePrompt() {
        if (blockMessage == false)
        {
            PressE.SetActive(true);
        }
    }

    //Esconde al jugador por pantalla que si pulsa E puede interactuar con el objeto.
    public void HideInteractablePrompt() {
        PressE.SetActive(false);
    }

    //Silencia la musica y cambia el icono en pantalla de el volumen de la música.
    public void MuteAudio() {
        if (mutedAudio == true)
        {
            muteAudio.GetComponent<Image>().sprite = normalVolume;
            mutedAudio = false;
            ASs[1].volume = 0.1f;
        }
        else {
            muteAudio.GetComponent<Image>().sprite = muteVolume;
            mutedAudio = true;
            ASs[1].volume = 0f;
        }
    }

    //Añadir dinero a la cantidad actual.
    public void IncreasePoints(int points) {
        coins += points;

        pointsCounter.text = coins.ToString();
    }

    //Update lifes
    public void UpdateLifes()
    {
        lifeCounter.GetComponent<TMPro.TMP_Text>().text = ""+lifes;
    }

    //Cargar el siguiente nivel.
    public void NextLevel() {
        if (changingLevel == false && restarting == false)
        {
            changingLevel = true;
            //Activa la cuenta atrás del comienzo del siguiente nivel.
            StartCoroutine(LoadLevelTimer());
        }
    }

    //Cuenta atras de la carga de nivel.
    IEnumerator LoadLevelTimer()
    {
        //Pone la pantalla en negro.
        screenfade.color = new Color(0, 0, 0);
        screenfade.CrossFadeAlpha(1f, 1f, true);
        //Quita el volumen de la música.
        decreaseVolume = true;
        //Espera 2 segundos.
        yield return new WaitForSecondsRealtime(2f);

        //Quita todas las llaves al jugador.
        redKey.SetActive(false);
        blueKey.SetActive(false);
        keysOwned.Clear();

        //Cambia el número de nivel en el que nos encontramos.
        currentLevel++;

        //Para el tiempo.
        Time.timeScale = 0f;

        //Carga el siguiente nivel, cambia el texto del siguiente mensaje del Manel y pone la música de ese nivel.
        switch (currentLevel)
        {
            case 1:
                ASs[1].clip = musicList[0];

                message.transform.Find("Message").GetComponent<TMPro.TMP_Text>().text = level1Message;
                SceneManager.LoadScene("Level1");
                break;
            case 2:
                ASs[1].clip = musicList[1];

                message.transform.Find("Message").GetComponent<TMPro.TMP_Text>().text = level2Message;
                SceneManager.LoadScene("Level2");
                break;
            case 3:
                ASs[1].clip = musicList[2];

                message.transform.Find("Message").GetComponent<TMPro.TMP_Text>().text = level3Message;
                SceneManager.LoadScene("Level3");
                break;
            case 4:
                ASs[1].clip = musicList[3];

                SceneManager.LoadScene("Victory");
                break;
        }
    }

    //Se ejecuta cuando se acaba de cargar un nivel.
    private void OnLevelWasLoaded(int level)
    {
        levelTimer = 0f;

        levelCompleted = false;

        if (currentLevel != 0 && lifes > -1)
        {
            //Permite que el volumen de la música vuelva.
            decreaseVolume = false;

            //Activa la canción.
            if (restarting == false)
            {
                ASs[1].Play();
            }

            //Dependiendo de si el jugador a desactivado la música o no, reactiva el volumen de la música.
            if (restarting == false && mutedAudio == false)
            {
                ASs[1].volume = 0.1f;
            }

            //Ya no se está cambiando de nivel.
            changingLevel = false;
            restarting = false;

            //Reset del contador de enemigos sobornados.
            satisfiedEnemies = 0;

            //Si el nivel no es la pantalla de victoria:
            if (currentLevel != 4)
            {
                //Reactivar controles del jugador
                GameObject.Find("Player").GetComponent<PlayerControl>().controls.Enable();

                //Cuantas monedas hay en este nivel?
                currentLevelCoins = GameObject.FindGameObjectsWithTag("Coin").Length;

                //Se asegura que las luces del juego esten ajustadas según la configuración del jugador.
                UpdateLightsQuality();

                //Activa un temporizador que hará que al cabo de 2 segundos la pantalla en negro desaparezca enseñando el juego de nuevo.
                StartCoroutine(waitForScreenfade(2));

                //Reactiva el contador de monedas.
                pointCounterHolder.SetActive(true);

                //Reactiva el contador de vidas.
                lifeCounterHolder.SetActive(true);

                //Actualiza el contador de vidas.
                UpdateLifes();

                //Enseña el mensaje del Manel.
                message.SetActive(true);

                //Enseña el botón del silencio de volumen del Manel.
                muteAudio.SetActive(true);

                //Desactiva el bloqueo del mensaje.
                blockMessage = false;

                //Se asegura de que el texto del dinero disponible se actualice.
                IncreasePoints(0);
            }
            //Si es la pantalla de victoria:
            else
            {
                //Fuerza la activación de la música.
                ASs[1].volume = 0.1f;
                ASs[1].Play();

                //Pone una cuenta atrás de 6 segundos para hacer desaparecer la pantalla en negro.
                StartCoroutine(waitForScreenfade(6));

                //Reactiva el paso del tiempo.
                Time.timeScale = 1f;

                //Desactiva el botón de silenciar la música, el contador de dinero y el contador de vidas.
                muteAudio.SetActive(false);
                pointCounterHolder.SetActive(false);
                lifeCounterHolder.SetActive(false);
            }
        }
        else
        {
            //Activa un temporizador que hará que al cabo de 2 segundos la pantalla en negro desaparezca enseñando el juego de nuevo.
            StartCoroutine(waitForScreenfade(2));
        }
    }

    //Reiniciar un nivel.
    public void RestartLevel() {
        if (changingLevel == false)
        {
            lifes--;
            //Cambia el color de la capa que cubre toda la pantalla invisible por un color rojo.
            screenfade.color = new Color(1, 0, 0);
            //Para el tiempo para que el juego no siga funcionando cuando el jugador ya ha muerto.
            Time.timeScale = 0f;
            //Hace que la capa de color rojo se vuelva visible y tape todo el juego en el siguiente segundo.
            screenfade.CrossFadeAlpha(1, 1, true);
            //Activa la cuenta atrás del reinicio del nivel.
            StartCoroutine(RestartLevelTimer());
        }
    }

    //Cuenta atrás del reinicio del nivel.
    IEnumerator RestartLevelTimer() {

        restarting = true;

        //Busca y pone en una lista todos los generadores de sonido del juego.
        AudioSource[] AllASs = FindObjectsOfType<AudioSource>();

        //Silencia todos los sonidos del juego excepto los generados por la base de datos (Música, Muerte del jugador, Sonidos de la interfaz).
        for (int i = 0; i < AllASs.Length; i++) {
            if (AllASs[i] != ASs[0] && AllASs[i] != ASs[1] && AllASs[i] != ASs[2]) {
                AllASs[i].volume = 0;
            }
        }

        //Activa el sonido de muerte del jugador.
        ASs[0].Play();

        //Espera 2 segundos.
        yield return new WaitForSecondsRealtime(2f);

        //Hace que la tela roja que cubre la pantalla se vuelva de color negra en el siguiente segundo.
        screenfade.CrossFadeColor(new Color(0, 0, 0), 1, true, false);
        yield return new WaitForSecondsRealtime(2f);

        //Quita todas las llaves del jugador.
        redKey.SetActive(false);
        blueKey.SetActive(false);
        keysOwned.Clear();

        //Quita todo el dinero del jugador y le deja con 10 centimos.
        coins = 10;

        if (lifes > -1)
        {
            //Según el nivel en el que está el jugador carga un nivel o otro.
            switch (currentLevel)
            {
                case 1:
                    SceneManager.LoadScene("Level1");
                    break;
                case 2:
                    SceneManager.LoadScene("Level2");
                    break;
                case 3:
                    SceneManager.LoadScene("Level3");
                    break;
            }
        }
        else
        {
            SceneManager.LoadScene("Defeat");
        }
    }

    //Temporizador para desactivar pantalla en negro.
    IEnumerator waitForScreenfade(float timer)
    {
        yield return new WaitForSecondsRealtime(timer);
        screenfade.CrossFadeAlpha(0f, 1f, true);
        yield return new WaitForSecondsRealtime(2f);
        screenfade.CrossFadeColor(new Color(1, 0, 0), 1, true, false);
    }

    //Actualiza la calidad y cantidad de luces en el nivel para reflejar las preferencias del jugador.
    void UpdateLightsQuality() {
        //Si la calidad de luces está puesta en baja:
        if (GetComponent<Settings>().LightQuality() == false)
        {
            GameObject postProcessing = GameObject.Find("PostProcessing");
            postProcessing.GetComponent<Volume>().profile.components[0].active = false;

            //Encuentra todas las luces del jugador y de los monitores.
            GameObject[] playerLights = GameObject.FindGameObjectsWithTag("PlayerLights");
            GameObject[] monitorLights = GameObject.FindGameObjectsWithTag("Monitor");

            //Desactiva todas las luces del jugador excepto las esenciales.
            for (int i = 0; i < playerLights.Length; i++)
            {
                if (playerLights[i].name != "PlayerLight" && playerLights[i].name != "Light")
                {
                    playerLights[i].GetComponent<Light2D>().enabled = false;
                }
            }

            //Desactiva todas las luces de los monitores.
            for (int i = 0; i < monitorLights.Length; i++)
            {
                monitorLights[i].GetComponentInChildren<Light2D>().enabled = false;
            }
        }
    }

    //Pausar o reanudar el juego
    public void Pause()
    {
        if (currentLevel != 0 && lifes > -1 && changingLevel == false && restarting == false && currentLevel != 4)
        {
            if (paused == false)
            {
                if (Time.timeScale != 0f)
                {
                    Time.timeScale = 0f;
                    pauseText.SetActive(true);
                    returnToMenuButton.SetActive(true);
                    GameObject postProcessing = GameObject.Find("PostProcessing");
                    ColorAdjustments ca = (ColorAdjustments)postProcessing.GetComponent<Volume>().profile.components[1];
                    VolumeParameter<float> value = new MinFloatParameter(-100f, 0, true);
                    ca.saturation.SetValue(value);
                    GameObject.Find("Player").GetComponent<PlayerControl>().controls.Disable();
                    paused = true;
                }
            }
            else
            {
                Time.timeScale = 1f;
                pauseText.SetActive(false);
                returnToMenuButton.SetActive(false);
                GameObject postProcessing = GameObject.Find("PostProcessing");
                ColorAdjustments ca = (ColorAdjustments)postProcessing.GetComponent<Volume>().profile.components[1];
                VolumeParameter<float> value = new MinFloatParameter(15, 0, true);
                ca.saturation.SetValue(value);
                GameObject.Find("Player").GetComponent<PlayerControl>().controls.Enable();
                paused = false;
            }
        }
    }

    //Activa la pantalla final del nivel
    public void ActivatedSwitch()
    {
        levelCompleted = true;

        Time.timeScale = 0f;

        GameObject.Find("Player").GetComponent<PlayerControl>().controls.Disable();

        int monedasRecogidas = currentLevelCoins - GameObject.FindGameObjectsWithTag("Coin").Length;

        //En el caso que el jugador se deje monedas en el suelo que haya disparado es posible que el valor sea negativo.
        if(monedasRecogidas < 0)
        {
            monedasRecogidas = 0;
        }

        levelSummaryCoins.GetComponent<TMPro.TMP_Text>().text = "Monedas recogidas: " + "\n" + monedasRecogidas + " / " + currentLevelCoins;
        levelSummaryEnemies.GetComponent<TMPro.TMP_Text>().text = "Enemigos sobornados: " + "\n" + satisfiedEnemies;

        levelSummaryTime.GetComponent<TMPro.TMP_Text>().text = "Tiempo transcurrido: " + "\n" + (int)levelTimer + " segundos";

        levelSummaryBackground.SetActive(true);
        levelSummaryTitle.SetActive(true);
        levelSummaryCoins.SetActive(true);
        levelSummaryContinue.SetActive(true);
        levelSummaryEnemies.SetActive(true);
        levelSummaryTime.SetActive(true);
    }

    //Cierra la pantalla final del nivel y prosigue al siguiente
    public void Continue()
    {
        levelSummaryBackground.SetActive(false);
        levelSummaryTitle.SetActive(false);
        levelSummaryCoins.SetActive(false);
        levelSummaryContinue.SetActive(false);
        levelSummaryEnemies.SetActive(false);
        levelSummaryTime.SetActive(false);

        NextLevel();
    }

    //Volver al menu principal desde el menú de pausa
    public void ReturnToMenu()
    {
        Destroy(gameObject);
        Destroy(GameObject.Find("GUI"));
        SceneManager.LoadScene("MainMenu");
    }

    //Comprueba que aún se esté enseñando por pantalla el mensaje del Manel.
    public bool isMessageActive() {
        return message.activeSelf;
    }

    //Activar sonido de interacción de menu.
    public void PlayMenuSound() {
        ASs[2].Play();
    }

    //Mensajes del Manel.
    string level1Message = "Los alumnos de Animación tienen demasiados monitores encendidos y se ha ido la luz." + "\n \n" +
        "Tu objetivo es descubrir su posición y apagarlos." + "\n \n" + "Es posible que te encuentres con animadores por el camino." + "\n \n" +
         "Utiliza tu rifle para dispersarlos. El rifle dispara monedas de 10 céntimos." + "\n \n" +
        "Los alumnos cogerán el dinero y se irán corriendo a la máquina de café a pasar el rato. Cuidado, porque no se quedarán allí para siempre.";

    string level2Message = "Este es el segundo piso, los animadores deben de haber escondido sus monitores aquí para dejar los ordenadores más tiempo renderizando."+"\n \n"+
        "Es posible que te encuentres algunas puertas cerradas con llave, las llaves abren las puertas que sean del mismo color que ellas."+ "\n \n"+
        "Consejo: con el click derecho en vez de disparar una moneda disparas hasta tres. Esto es útil cuando más de un animador te ataca a la vez."+"\n \n" +
        "Acuerdate de apagar la pantalla de tu móvil también.";

    string level3Message = "Los alumnos de animación se han juntado todos en este piso y han encendido todos los monitores a la vez."+"\n \n"+"Preparate porque va a ser duro."+"\n \n"+
        "Acaba con esto de una vez por todas. No dejes ni un solo monitor en pie.";
}
