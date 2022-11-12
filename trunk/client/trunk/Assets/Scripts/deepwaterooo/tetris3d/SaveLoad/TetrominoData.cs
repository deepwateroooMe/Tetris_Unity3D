using Framework.MVVM;
using System.Text;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    [System.Serializable]
    public class TetrominoData {
        private const string TAG = "TetrominoData";

        public string name { get; set; }
        public string type { get; set; }
        public int color { get; set; }
        
        public SerializedTransform transform { get; set; }
		public MinoDataCollection<TetrominoData, MinoData> children;

        public TetrominoData(Transform parentTrans, string type, string name, int colortmp) {
            Debug.Log(TAG + " TetrominoData" + " type: " + type);
            this.name = name;
            this.type = type;
            this.color = colortmp;
            transform = new SerializedTransform(parentTrans);
            children = new MinoDataCollection<TetrominoData, MinoData>(this);
// 这里根据现在的逻辑需求,是需要添加颜色信息的            
            foreach (Transform mino in parentTrans) {
                if (mino.CompareTag("mino")) {
                    MinoData minoDataItem = new MinoData(mino,
                                                         new StringBuilder("mino" + type.Substring(9, 1)).ToString(),
                                                         colortmp); // TetrominoX ==> minoX
                    children.Add(minoDataItem);
                }
            }
        }
        
        public TetrominoData(Transform parentTrans) {
            transform = new SerializedTransform(parentTrans);
            children = new MinoDataCollection<TetrominoData, MinoData>(this);
            foreach (Transform mino in parentTrans) {
                if (mino.CompareTag("mino")) {
                    MinoData minoDataItem = new MinoData(mino);
                    children.Add(minoDataItem);
                }
            }
        }
    
        public void print() {
            Debug.Log(TAG + ": Parent TetrominoData: "); 
            this.transform.print();
            foreach (var minoData in children) {
                Debug.Log(TAG + " minoData.idx: " + minoData.idx); 
                minoData.print();
            }
        }
    }
}