using UnityEngine;
using System.Collections;

namespace tetris3d {
    
    public class UnscaledTimeParticle : MonoBehaviour {
        
        private ParticleSystem particles;
        
        void Start () {
            particles = GetComponent<ParticleSystem>();
        }
        
        void Update() {
            if (Time.timeScale < 0.01f) {
                 particles.Simulate(Time.unscaledDeltaTime, true, false);
            }
        }
    }    
}
