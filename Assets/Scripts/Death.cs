using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Death : MonoBehaviour
{
    public Transform respawnPoint;  // El GameObject vac�o que define el punto de respawn
    public Canvas canvas;           // El Canvas que contiene la pantalla negra
    public Image blackScreen;       // La Image negra que cubrir� la pantalla
    // VARIABLE NUEVA: Vi�etas rojas que se mostrar�n al recibir da�o.
    public Image redVignette;
    public AudioSource audioSource; // El componente AudioSource que reproducir� el sonido
    public AudioClip[] deathSounds; // Los clips de sonido que se reproducir�n al chocar
    public float transitionDuration = 2f;  // Duraci�n de la transici�n en segundos

    private int collisionCount = 0; // Contador de cu�ntas veces ha chocado el jugador

    // Esta funci�n se llama cuando un objeto entra en contacto con este objeto
    private void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto que colision� es el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            // LLAMADA NUEVA: Activar el efecto de c�mara al recibir da�o.
            PlayerCam playerCam = collision.gameObject.GetComponent<PlayerCam>();
            if (playerCam == null)
            {
                // Si no se encuentra en los hijos, intenta buscar en la escena
                playerCam = GameObject.FindObjectOfType<PlayerCam>();
            }
            if (playerCam != null)
            {
                Debug.Log("Triggering damage shake");
                playerCam.TriggerDamageShake();
            }
            else
            {
                Debug.LogWarning("PlayerCam no se encontr� en el jugador ni en la escena.");
            }

            // LLAMADA NUEVA: Mostrar vi�etas rojas alrededor de la pantalla.
            if (redVignette != null)
            {
                StartCoroutine(RedVignetteEffect());
            }

            // Incrementa el contador de colisiones
            collisionCount++;

            // Reproduce el sonido correspondiente seg�n el n�mero de colisiones
            if (audioSource != null && deathSounds.Length >= collisionCount)
            {
                AudioClip soundToPlay = deathSounds[collisionCount - 1];

                // Reproduce el primer y segundo sonido en bucle
                if (collisionCount == 1 || collisionCount == 2)
                {
                    audioSource.clip = soundToPlay;
                    audioSource.loop = true;
                    audioSource.Play();
                }
                // Reproduce el tercer sonido sin bucle
                else if (collisionCount == 3)
                {
                    audioSource.PlayOneShot(soundToPlay);
                    audioSource.loop = false; // Asegura que no se repita
                }
            }

            // Si el jugador ha chocado 3 veces, inicia la secuencia de muerte
            if (collisionCount == 3)
            {
                // Inicia la secuencia de muerte (transici�n)
                StartCoroutine(DeathSequence(collision.gameObject));
            }
        }
    }

    private IEnumerator DeathSequence(GameObject player)
    {
        // Activa la pantalla negra (si no est� activada ya)
        blackScreen.gameObject.SetActive(true);

        // Hace la pantalla opaca (de 0 a 1) durante la duraci�n de la transici�n
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            // Cambia la opacidad sin afectar el sprite
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.Lerp(0f, 1f, elapsedTime / transitionDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1f); // Asegura que est� completamente opaco

        // Teletransporta al jugador a la posici�n del punto de respawn
        player.transform.position = respawnPoint.position;

        // Espera unos segundos en la pantalla negra antes de desaparecer
        yield return new WaitForSeconds(1f);

        // Hace que la pantalla negra se desvanezca (de 1 a 0)
        elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            // Cambia la opacidad sin afectar el sprite
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.Lerp(1f, 0f, elapsedTime / transitionDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0f); // Asegura que est� completamente transparente

        // Desactiva la pantalla negra
        blackScreen.gameObject.SetActive(false);

        // Reinicia el contador de colisiones
        collisionCount = 0;
    }

    // M�TODO NUEVO: Coroutine para mostrar y desvanecer las vi�etas rojas.
    private IEnumerator RedVignetteEffect()
    {
        redVignette.gameObject.SetActive(true);
        Color initialColor = redVignette.color;
        initialColor.a = 1f;
        redVignette.color = initialColor;
        float duration = 0.3f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            Color newColor = redVignette.color;
            newColor.a = alpha;
            redVignette.color = newColor;
            yield return null;
        }
        Color finalColor = redVignette.color;
        finalColor.a = 0f;
        redVignette.color = finalColor;
        redVignette.gameObject.SetActive(false);
    }
}