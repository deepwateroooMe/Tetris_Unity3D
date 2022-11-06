using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    public class UnscaledTimeParticle : MonoBehaviour {
        private const string TAG = "UnscaledTimeParticle";     

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
