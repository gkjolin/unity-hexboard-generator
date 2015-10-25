using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Artwave.Board{
	/**
	 * jest to kontener przechowujący utworzone hexy. Daje uniwersalny dostęp do poszczególnych hexów czy ich grup
	 */ 
	public class BoardHex : MonoBehaviour {
		public bool drawGizmos = false;
		private HexListenerList listenerList;
		private Dictionary<Vector3, HexField> hexList;
		/**
		 * wartość określa czy board jest zbudowany
		 */ 
		private bool isReady = false;
		public void SetReady(){
			isReady = true;
		}
		public bool IsReady(){
			return isReady;
		}
		private bool isEven = true;
		private bool symmetricHorizontal = false;
		public void Init(){
			enabled = true;
			this.isReady = false;
			this.listenerList = new HexListenerList ();
			this.hexList = new Dictionary<Vector3, HexField>();
		}
		public void Reset(){
			isReady = false;
			this.hexList = new Dictionary<Vector3, HexField>();
		}
		private Vector2 hexSize;//rozmiar pojedyńczego hexa, przy pierwszym dodawanym hexie zostaje z niego rozmiar pobrany
		private Vector2 size; //rozmiar rzeczywisty planszy
		private int colNumber, rowNumber;//colNumber-x rowNumber-y 
		/**
		 * metoda pozwala przeliczyć rozmiar całej planszy
		 */ 
		public void Recalculate(){
//			int colNumber = Mathf.Abs (colMinCoordinate) + Mathf.Abs (colMaxCoordinate) + 1;//+1 ponieważ jakby narysować to na wykresie to w zakres wchodzi też ostatnia wartość jako pełna
//			int rowNumber = Mathf.Abs (rowMinCoordinate) + Mathf.Abs (rowMaxCoordinate) + 1;//+1 ponieważ jakby narysować to na wykresie to w zakres wchodzi też ostatnia wartość jako pełna
			if (rowNumber > 1 && symmetricHorizontal == false) { //jak symmetricHorizontal true to nie ma przesunięcia wynikającego z połowy hexa na szerokość
				this.size = new Vector2 (HexMath.CalculateMultiRowWidth (this.hexSize.x, colNumber), HexMath.CalculateColumnHeight (this.hexSize.y, rowNumber));
			} else {
				this.size = new Vector2 (HexMath.CalculateRowWidth (this.hexSize.x, colNumber), HexMath.CalculateColumnHeight (this.hexSize.y, rowNumber));
			}

		}
		void Update () {}
		public Vector2 GetSize(){
			return this.size;
		}
		public void Config(int colNumber, int rowNumber, Vector2 hexSize, bool isEven, bool symmetricHorizontal){
			this.hexSize = hexSize;
			this.isEven = isEven;
			this.colNumber = colNumber;
			this.rowNumber = rowNumber;
			this.symmetricHorizontal = symmetricHorizontal;
		}
//		private int colMinCoordinate, rowMinCoordinate, colMaxCoordinate, rowMaxCoordinate;
		public void Add(Vector3 coordinates, HexField hex){
//			Vector2 offsetCoordinates;
//			if (isEven) {
//				offsetCoordinates = HexMath.ConvertCubeToEvenROffsetCoordinate ((int)coordinates.x, (int)coordinates.y, (int)coordinates.z);
//			} else {
//				offsetCoordinates = HexMath.ConvertCubeToOddROffsetCoordinate ((int)coordinates.x, (int)coordinates.y, (int)coordinates.z);
//			}
//			if (this.hexList.Count == 0) {
//				colMinCoordinate = (int)offsetCoordinates.x;
//				colMaxCoordinate = (int)offsetCoordinates.x;
//				rowMinCoordinate = (int)offsetCoordinates.y;
//				rowMaxCoordinate = (int)offsetCoordinates.y;
//			}
//			if (offsetCoordinates.x < colMinCoordinate) {
//				colMinCoordinate = (int)offsetCoordinates.x;
//			} else if (offsetCoordinates.x > colMaxCoordinate) {
//				colMaxCoordinate = (int)offsetCoordinates.x;
//			}
//			if (offsetCoordinates.y < rowMinCoordinate) {
//				rowMinCoordinate = (int)offsetCoordinates.y;
//			} else if (offsetCoordinates.y > rowMaxCoordinate) {
//				rowMaxCoordinate = (int)offsetCoordinates.y;
//			}
			hex.SetCoordinates(coordinates);
			this.hexList.Add(coordinates , hex);
			hex.SetListenerList(this.listenerList);

		}
		public void AddListener(HexListenerInterface listener){
			this.listenerList.Add (listener);
		}
		public HexField GetHex(Vector3 coordinates){
			HexField hex;
			hexList.TryGetValue (coordinates, out hex);
			return hex;
		}
		public List<HexField> GetAll(){
			List<HexField> list = new List<HexField>();
			foreach(KeyValuePair<Vector3, HexField> entry in hexList){
				list.Add(entry.Value);
			}
			return list;
		}
		/**
		 * zwraca listę sąsiednich hexów, gdy range 1 to zwraca tylko pierwsze sąsiedztwo, gdy range 2, to kolejny pierścien dodatkowo
		 */ 
		public List<HexField> GetNeighbors(Vector3 centerCoordinates, int range = 1){
			List<HexField> list = new List<HexField>();
			List<Vector3> coordinatesList = HexMath.GetRange (centerCoordinates, range, 0);//by pominąć kliknięty hex i zwrócić sąsiedztwo
			foreach (Vector3 neigborCoordinates in coordinatesList) {
				HexField hex = this.GetHex(neigborCoordinates);
				if(hex != null){
					list.Add(hex);
				}
			}
			return list;
		}
		void OnDrawGizmos(){
			if (drawGizmos) {
				Gizmos.color = new Color (0, 1, 0, 0.5f);
				Gizmos.DrawCube (transform.position, new Vector3 (size.x, size.y, 0.5f));
			}
		}
		
	}
}
