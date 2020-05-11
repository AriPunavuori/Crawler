using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoorScript : MonoBehaviour
{
	public static BossDoorScript Instance;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	IEnumerator Slide(float dist, float inTime)
	{
		float elapsedTime = 0;
		Vector3 fromPosition = transform.position;
		Vector3 toPosition = transform.position + new Vector3(dist, 0, 0);
		Debug.Log("from: " + fromPosition);
		Debug.Log("to: " + toPosition);
		while (transform.position.x > toPosition.x)
		{
			//Debug.Log(transform.position);
			transform.position = Vector3.Lerp(fromPosition, toPosition, (elapsedTime / inTime));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}



	public void SlideBossDoor()
	{
		//bc.enabled = false;
		AudioFW.Play("DoorOpen");
		//opened = true;
		//uim.SetInfoText(openerName + " opened " + gameObject.name, 2);
		//GameObject particles = Instantiate(doorOpenParticles, transform.GetChild(1).transform.position, transform.rotation, transform.GetChild(1));
		//Destroy(particles, 5);
		StartCoroutine(Slide(-3.3f, 1));
	}


}
