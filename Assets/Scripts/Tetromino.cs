using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    private float fall = 0;
    public float fallSpeed = 1;

    public bool allowRotation = true;
    public bool limitRotation = false;
    public string prefabName;

    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;

    private float continuousVerticalSpeed = 0.05f;
    private float continuousHorizontalSpeed = 0.1f;
    private float buttonDownWaitMax = 0f;


    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimer = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    public int individualScore = 100;

    private AudioSource audioSource;

    private float individualScoreTime;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckUserInput();
    }

    void UpdateIndividualScore()
    {
        if (individualScoreTime < 1)
        {
            individualScoreTime += Time.deltaTime;
        }
        else
        {
            individualScoreTime = 0;

            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }

    void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.DownArrow))
        {
            movedImmediateVertical = false;
            movedImmediateVertical = false;

            horizontalTimer = 0;
            verticalTimer = 0;
            buttonDownWaitTimer = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (movedImmediateHorizontal)
            {
                if (buttonDownWaitTimer < buttonDownWaitMax)
                {
                    buttonDownWaitTimer += Time.deltaTime;
                    return;
                }

                if (horizontalTimer < continuousHorizontalSpeed)
                {
                    horizontalTimer += Time.deltaTime;
                    return;
                }
            }


            if (!movedImmediateHorizontal)
                movedImmediateHorizontal = true;

            horizontalTimer = 0;

            transform.position += new Vector3(1, 0, 0);
            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
                PlayMoveAudio();
            }
            else
            {
                transform.position += new Vector3(-1, 0, 0);
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (movedImmediateHorizontal)
            {
                if (buttonDownWaitTimer < buttonDownWaitMax)
                {
                    buttonDownWaitTimer += Time.deltaTime;
                    return;
                }

                if (horizontalTimer < continuousHorizontalSpeed)
                {
                    horizontalTimer += Time.deltaTime;
                    return;
                }
            }

            if (!movedImmediateHorizontal)
                movedImmediateHorizontal = true;

            horizontalTimer = 0;

            transform.position += new Vector3(-1, 0, 0);

            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
                PlayMoveAudio();
            }
            else
            {
                transform.position += new Vector3(1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (allowRotation)
            {
                if (limitRotation)
                {
                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }

                if (CheckIsValidPosition())
                {
                    FindObjectOfType<Game>().UpdateGrid(this);

                    PlayRotateAudio();

                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        PlayMoveAudio();
                    }
                }
                else
                {
                    if (limitRotation)
                    {
                        if (transform.rotation.eulerAngles.z >= 90)
                        {
                            transform.Rotate(0, 0, -90);
                        }
                        else
                        {
                            transform.Rotate(0, 0, 90);
                        }
                    }
                    else
                    {
                        transform.Rotate(0, 0, -90);
                    }
                }
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            if (movedImmediateVertical)
            {
                if (buttonDownWaitTimer < buttonDownWaitMax)
                {
                    buttonDownWaitTimer += Time.deltaTime;
                    return;
                }

                if (verticalTimer < continuousVerticalSpeed)
                {
                    verticalTimer += Time.deltaTime;
                    return;
                }
            }

            if (!movedImmediateVertical)
                movedImmediateVertical = true;

            verticalTimer = 0;

            transform.position += new Vector3(0, -1, 0);
            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);

                FindObjectOfType<Game>().DeleteRow();


                if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
                {
                    FindObjectOfType<Game>().GameOver();
                }

                PlayLandAudio();

                Game.currentScore += individualScore;

                enabled = false;

                FindObjectOfType<Game>().SpawnNextTetromino();
            }

            fall = Time.time;
        }
    }


    void PlayMoveAudio()
    {
        audioSource.PlayOneShot(moveSound);
    }

    void PlayRotateAudio()
    {
        audioSource.PlayOneShot(rotateSound);
    }

    void PlayLandAudio()
    {
        audioSource.PlayOneShot(landSound);
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);

            if (FindObjectOfType<Game>().CheckInsideGrid(pos) == false)
            {
                return false;
            }

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null &&
                FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }

        return true;
    }
}