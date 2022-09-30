using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace tetris3d {

public class Spells : MonoBehaviour {

    public GameObject[] spells; // may have many later on

    public void CastSpell(int currSpell) {
	//spells[currSpell].GetComponent<Animator>().SetTrigger("activated");

	// the above line does NOT work any more
	SceneManager.LoadScene("GameMenu");
    }
    
}
}