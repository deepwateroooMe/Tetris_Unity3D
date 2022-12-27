using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace deepwaterooo.tetris3d {
	public class BaseCubes : MonoBehaviour {

        [SerializeField]
        public GameObject [] _cubes;

        public GameObject [] cubes() {
            return _cubes;
        }
	}
}