using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextKill : MonoBehaviour {

    private bool isDead = false;

	// Use this for initialization
	void Update () {
        if (!isDead) { StartCoroutine(kill()); isDead = true; }
    }

    public IEnumerator kill()
    {
        yield return new WaitForSeconds(2);
        isDead = false;
        gameObject.SetActive(false);
    }
}
