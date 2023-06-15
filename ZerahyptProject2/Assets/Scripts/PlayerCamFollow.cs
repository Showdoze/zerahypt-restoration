using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public partial class PlayerCamFollow : MonoBehaviour
{
    public static bool HoldCam;
    public Transform PlayerCamera;
    public Transform LookTarget;
    public Transform Piri;
    public AudioListener mainListener;
    public AudioListener conListener;
    public AudioSource conScreenAudio;
    public GameObject ConIcon;
    public GameObject ConScreen;
    public bool conFar;
    public bool once;
    public float StabForce;
    public float Force;
    public float camDist;

    public virtual IEnumerator Start()
    {
        InvokeRepeating("Tick", 2, 0.25f);
        Force = 0.002f;
        Piri = PlayerInformation.instance.PiriPresence;
        camDist = 1f;

        if (HoldCam)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            yield return new WaitForSeconds(0.05f);
            transform.position = (PlayerInformation.instance.Pirizuka.position + (PlayerInformation.instance.Pirizuka.up * 3)) + (PlayerInformation.instance.Pirizuka.forward * 1.5f);
            GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
            yield return new WaitForSeconds(0.05f);
            transform.position = PlayerInformation.instance.Pirizuka.position + (PlayerInformation.instance.Pirizuka.up * 1000);
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public virtual void Update()
    {
        if (conFar && !once)
        {
            once = true;
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().loop = false;
            StartCoroutine(ConnectionBroken());
        }
        camDist = Vector3.Distance(transform.position, Piri.position);
    }

    public virtual void FixedUpdate()
    {
        if (!WorldInformation.FPMode)
        {
            GetComponent<Rigidbody>().AddForce((PlayerCamera.transform.position - transform.position) * Force);
        }
        Quaternion rotation = Quaternion.LookRotation(LookTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * StabForce);
        if (Force < 0.002f)
        {
            Force = Force + 5E-05f;
        }
    }

    public virtual void Tick()
    {
        if (!WorldInformation.UsingSnyf)
        {
            if (camDist > 512)
            {
                conFar = true;
            }
            else
            {
                if (camDist > 256)
                {
                    ConIcon.SetActive(true);
                    if (!conFar)
                    {
                        if (!GetComponent<AudioSource>().isPlaying)
                        {
                            GetComponent<AudioSource>().Play();
                        }
                    }
                    GetComponent<AudioSource>().loop = true;
                }
                else
                {
                    ConIcon.SetActive(false);
                    GetComponent<AudioSource>().loop = false;
                    if (camDist < 128)
                    {
                        conFar = false;
                        once = false;
                    }
                }
            }
        }
    }

    public virtual IEnumerator ConnectionBroken()
    {
        mainListener.enabled = false;
        conListener.enabled = true;
        ConScreen.SetActive(true);
        conScreenAudio.Play();
        yield return new WaitForSeconds(31);
        if (conFar)
        {
            WorldInformation.FPMode = false;
            HoldCam = false;
            SceneManager.LoadScene("PiriZerzek");
        }
        else
        {
            mainListener.enabled = true;
            conListener.enabled = false;
            ConScreen.SetActive(false);
        }
    }

    public virtual void OnCollisionStay(Collision col)
    {
        if (col.gameObject.name.Contains("Wa"))
        {
            Force = 0.0005f;
        }
    }

    public PlayerCamFollow()
    {
        StabForce = 6f;
        Force = 0.002f;
        camDist = 1;
    }

}