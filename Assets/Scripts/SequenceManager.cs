using System.Collections;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    // Define the actions that can be performed in the sequence
    public Animator animator;
    public AudioSource audioSource;
    public Camera camera;
    public enum SequenceActionType
    {
        Animation,
        Audio,
        GameObject,
        CameraMovement
    }

    // Define a data structure to hold information about each sequence action
    public class SequenceAction
    {
        public SequenceActionType type;
        public AnimationClip animationClip;
        public AudioClip audioClip;
        public GameObject targetGameObject;
        public bool waitForCompletion;
        public Transform cameraStartPosition;
        public Transform cameraEndPosition;
        public float cameraMovementSpeed;
    }

    // Define the sequence by populating a list of sequence actions
    public SequenceAction[] sequence = new SequenceAction[]
    {
        // Define your sequence actions here
    };

    private int currentActionIndex = 0;
    private bool isSequenceRunning = false;

    private void Start()
    {
        // Start the sequence when the game starts
        StartSequence();
    }

    private void StartSequence()
    {
        if (!isSequenceRunning)
        {
            StartCoroutine(RunSequence());
        }
    }

    private IEnumerator RunSequence()
    {
        isSequenceRunning = true;

        while (currentActionIndex < sequence.Length)
        {
            SequenceAction currentAction = sequence[currentActionIndex];

            switch (currentAction.type)
            {
                case SequenceActionType.Animation:
                    yield return StartCoroutine(PlayAnimation(currentAction.animationClip, currentAction.waitForCompletion));
                    break;

                case SequenceActionType.Audio:
                    yield return StartCoroutine(PlayAudio(currentAction.audioClip, currentAction.waitForCompletion));
                    break;

                case SequenceActionType.GameObject:
                    SetGameObjectActive(currentAction.targetGameObject, true);
                    break;

                case SequenceActionType.CameraMovement:
                    yield return StartCoroutine(MoveCamera(currentAction.cameraStartPosition, currentAction.cameraEndPosition, currentAction.cameraMovementSpeed));
                    break;
            }

            currentActionIndex++;
        }

        isSequenceRunning = false;
    }

    private IEnumerator PlayAnimation(AnimationClip animationClip, bool waitForCompletion)
    {
        //Animator animator = /* Get the animator component from the appropriate game object */;
        animator.Play(animationClip.name);

        if (waitForCompletion)
        {
            yield return new WaitForSeconds(animationClip.length);
        }
    }

    private IEnumerator PlayAudio(AudioClip audioClip, bool waitForCompletion)
    {
        //AudioSource audioSource = /* Get the audio source component from the appropriate game object */;
        audioSource.clip = audioClip;
        audioSource.Play();

        if (waitForCompletion)
        {
            yield return new WaitForSeconds(audioClip.length);
        }
    }

    private void SetGameObjectActive(GameObject gameObject, bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    private IEnumerator MoveCamera(Transform startPosition, Transform endPosition, float speed)
    {
        //Camera mainCamera = /* Get the main camera component or any other appropriate camera */;
        float startTime = Time.time;

        while (Time.time - startTime <= Vector3.Distance(startPosition.position, endPosition.position) / speed)
        {
            float t = (Time.time - startTime) * speed / Vector3.Distance(startPosition.position, endPosition.position);
            camera.transform.position = Vector3.Lerp(startPosition.position, endPosition.position, t);
            yield return null;
        }
    }
}