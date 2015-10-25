using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Artwave.Board{
	/**
	 * klasa rysuje planszę z hexami. Może być wywołane przez jakiś manager lub przez inspektora gdzie określimy parametry i metodę obliczania.
	 * Rysuje planszę w wersji "pointy topped" - punkt na górze i dole (R) (horyzontalny layout), alternatywna wersja (ten builder nie wspiera) to "flat tapped" - czyli u podstawy płaska powieżchnia (Q)(vertykalny layout) 
	 * W wariancie parzystym - czyli even-R  
	 */ 
	public class BoardHexBuilder : MonoBehaviour {
		private BuilderListenerList listenerList;
		public void Init(){
			enabled = true;
			this.listenerList = new BuilderListenerList ();
			hexPattern.Init();
		}
		/**
		 * dodajemy listener który nasłuchuje na informacje z kontrollera
		 */ 
		public void AddListener(BuilderListenerInterface listener){
			this.listenerList.Add (listener);
		}
		/**
		 * zawiera wzorcowy hex z którego będą zbudowane kolejne
		 */ 
		//tu zmiana przez zostawienie tylko jednego z nich
		public HexField hexPattern;
//		private HexField cloneHexPrefab;


		/**
		 * plansza jest typu pointy topped, a za pomocą poniższego parametru określamy czy jest parzysta czy niepayrzsta
		 */ 
		public bool isEven = true;
		/**
		 * Wszystkie tryby obliczania parametrów planszy, Niezależnie od trybu musi być zdefiniowana liczba hexów w pionie i poziomie
		 * BOARD_FULLSCREEN_CALC_HEXSIZE - rozmiar planszy się dopasuje do całego ekranu gry, na podstawie znanej liczby hexów obliczy ich rozmiar, tak by się zmieściły na całym ekranie
		 * FIXED_BOARD_CAL_HEXSIZE - określamy jaki ma być rozmiar planszy - wyrażone w unitach, jeśli np kamera ma 10 unitów wysokości a ustawimy 20 na wysokość to plansza będzie 2x większa, również obliczamy rozmiar hexów znając ich ilość
		 * FIXED_HEXSIZE_CALC_BOARD - znamy rozmiar hexów w unitach, znamy ilość, rozmiar planszy dopasuje się do tego rozmiaru
		 * 
		 */ 
		public enum CalculationTypes {BOARD_FULLSCREEN_CALC_HEXSIZE ,FIXED_BOARD_CAL_HEXSIZE, FIXED_HEXSIZE_CALC_BOARD};
		public CalculationTypes calculationType = CalculationTypes.BOARD_FULLSCREEN_CALC_HEXSIZE;
		/**
		 * liczba hexów jakie mają być w pionie i poziomie; colNumber-x rowNumber-y 
		 */ 
		public int colNumber, rowNumber;
		/**
		 * oznacza że w co drugim wierszu będzie o jeden mniej hex, przez co po prawej i lewej stronie będzie symetria
		 * JESZCZE NIE DZIAŁA
		 */ 
		public bool symmetricHorizontal = false;
		/**
		 * rozmiar tablicy
		 */ 
		public Vector2 recommendSize;
		/**
		 * rozmiar pojedyńczego hexa jaki ma być wyświetlony (szerokość obliczana jest z wysokości w=h*sqrt(3)/2)
		 */
		public float recommendHexHeight;
		private float hexWidth;
		/**
		 * do szybkiego budowania używając tylko parametrów określonych w inspektorze 
		 */
		public BoardHex Build(){
			switch (calculationType) {
			case CalculationTypes.BOARD_FULLSCREEN_CALC_HEXSIZE:
				return this.BuildCalcHexSize();
			case CalculationTypes.FIXED_BOARD_CAL_HEXSIZE:
				return this.BuildCalcHexSize();
			case CalculationTypes.FIXED_HEXSIZE_CALC_BOARD:
				return this.BuildCalcBoardSize();
			}
			return null;
		}
		void Update () {}
		private BoardHex board;
		public void SetBoard(BoardHex board){
			this.board = board;
		}
//		private void SetHexNumber(int newColNumber, int newRowNumber){
//			if (newColNumber <= 0 || newRowNumber <= 0) {
//				Debug.LogError("horizontalHexNumber i verticalHexNumber muszą być większe od 0");
//			}
//			this.colNumber = newColNumber;
//			this.rowNumber = newRowNumber;
//
//		}
		/**
		 * znamy ilość hexów i plansza ma zająć cały ekran. Odpowiada to trybowi BOARD_FULLSCREEN_CALC_HEXSIZE
		 */ 
		private BoardHex BuildFullScreenCalcHexSize () { 
//			this.SetHexNumber (newColNumber, newRowNumber);
			Camera cam = Camera.main;
			float camHeight = 2f * cam.orthographicSize;
			float camWidth = camHeight * cam.aspect;
			recommendSize.x = camWidth;
			recommendSize.y = camHeight;
			return BuildCalcHexSize ();
		}
		/**
		 * znamy ilość hexów i rozmiar planszy w unitach. Odpowiada to trybowi FIXED_BOARD_CAL_HEXSIZE
		 */ 
		private BoardHex BuildCalcHexSize () {
//			this.SetHexNumber (newColNumber, newRowNumber);
//			if (newRecommendWidth <= 0 || newRecommendHeight <= 0) {
//				Debug.LogError("boardUnitWidth i boardUnitHeight muszą być większe od 0");
//			}
//			recommendSize.x = newRecommendWidth;
//			recommendSize.y = newRecommendHeight;
			float bestHexWidth = HexMath.CalculateHexWidthInRow (recommendSize.x, colNumber);
			float bestHexHeight = HexMath.CalculateHexHeightInColumn(recommendSize.y, rowNumber);
			hexWidth = HexMath.CalculateHexWidth(bestHexHeight);
			if (hexWidth > bestHexWidth) { //szerokość obliczona z bestHexHeight jest za duża, przyjmujemy więc bestHexWidth jako referencję i obliczamy 
				hexWidth = bestHexWidth;
				recommendHexHeight = HexMath.CalculateHexHeight(hexWidth);
			} else { //prawidłowo obliczona została szerokość
				recommendHexHeight = bestHexHeight;
			}
			return this.BuildCalc ();
		}
		/**
		 * znamy ilość hexów i oczekiwany rozmiar hexów w unitach, obliczamy rozmiar planszy
		 */ 
		private BoardHex BuildCalcBoardSize(){
//			this.SetHexNumber (newColNumber, newRowNumber);
//			if (recommendHexHeight <= 0) {
//				Debug.LogError("hexHeight musi być większe od 0");
//			}
//			recommendHexHeight = newHexHeight;
			hexWidth = HexMath.CalculateHexWidth(recommendHexHeight);
			return this.BuildCalc ();
		}

		/**
		 * do rzeczywistej budowy potrzebujemy rozmiar  hexa i ilość hexów, nie potrzebujemy rozmiaru mapy
		 */ 
		private BoardHex BuildCalc(){
			if (board.IsReady () == true) {//znaczy że ta plansza jest już zbudowana
				return board;
			}
			hexPattern.gameObject.SetActive (false);
			float calcHeigh = HexMath.CalculateColumnHeight (recommendHexHeight, rowNumber);
			float halfHeight = calcHeigh / 2;//rowNumber / 2;
			float halfWidth = 0;
			if (rowNumber > 1 && symmetricHorizontal == false) // jak symmetricHorizontal true to nie ma przesunięcia wynikającego z połowy hexa na szerokość
			{
				halfWidth = HexMath.CalculateMultiRowWidth (hexWidth, colNumber) / 2;
			} else {
				halfWidth = HexMath.CalculateRowWidth (hexWidth, colNumber) / 2;
			}
			Transform hexObject;
			board.Config (colNumber, rowNumber, new Vector2(hexWidth, recommendHexHeight), isEven, symmetricHorizontal);
			this.listenerList.OnBuildStart (hexPattern, colNumber, rowNumber, new Vector2(hexWidth, recommendHexHeight), isEven, symmetricHorizontal);
			int inverse = 0; //ponieważ inverse nie jest hexem o y = 0 tylko jest zależny od rowNumber musimy mieć inną wartość inverse
			if (isEven == true && rowNumber % 2 == 1) {
				inverse = 0;
			} else if (isEven == true  && rowNumber % 2 == 0){
				inverse = 1;
			} else if (isEven == false && rowNumber % 2 == 1) {
				inverse = 1;
			} else if (isEven == false  && rowNumber % 2 == 0){
				inverse = 0;
			}
			for (int y = rowNumber -1; y >= 0 ; y--) 
			{
				for (int x = 0; x < colNumber; x ++) 
				{
					if(symmetricHorizontal && x == colNumber-1 && inverse == 0){
						break;
					}
					Vector3 newCoordinates;
					if(isEven){
						newCoordinates = HexMath.ConvertEvenROffsetToCubeCoordinate(x, y);
					} else {
						newCoordinates = HexMath.ConvertOddROffsetToCubeCoordinate(x, y);
					}
					bool listenerResult = this.listenerList.OnCreateHexStart (newCoordinates, new Vector2(x, y), (inverse==1)?true:false);
					if(listenerResult == false){
						continue;
					}
					hexObject = (Transform)Instantiate (hexPattern.transform, new Vector3 (0, 0, 0), Quaternion.identity);
					hexObject.gameObject.SetActive (true);
					hexObject.transform.parent = board.transform;
					HexField hexComponent = hexObject.GetComponent<HexField> ();
					hexComponent.Init();
					hexComponent.SetPosition(new Vector3(hexWidth * x + hexWidth - inverse * 0.5f * hexWidth - halfWidth, - 3 * (y) * recommendHexHeight  / 4 + halfHeight - recommendHexHeight / 2, 0));
					hexComponent.SetSize(new Vector2(hexWidth, recommendHexHeight));
					board.Add(newCoordinates , hexComponent);
					hexObject.name = "Hex " + hexComponent.GetCoordinates().ToString() + " " + x + " " + y;
					this.listenerList.OnCreateHexEnd (hexComponent);
				}
				inverse = (inverse==1)?0:1;//na wysokość co drugi wiersz ma przesunięcie - w ten sposób to oznaczamy
			}	

//			board.SetHexNumber (colNumber, rowNumber);
			board.Recalculate ();
			board.SetReady ();
			this.listenerList.OnBuildEnd ();
			return board;
		}
		/**
		 * zwraca wzorcowego hexa jaki będzie użyty do klonowania pozostałych hexów. Możemy więc zrobić na nim zmiany które potem będą widoczne na wszystkich hexach
		 */ 
		public HexField GetPatternHex(){
			return hexPattern;
		}
		public void Reset(){
			if (board.IsReady () == false) {//znaczy że ta plansza jest już zbudowana
				return;
			}
			this.listenerList.OnResetStart ();
			board.Reset ();
			foreach (Transform child in board.transform)
			{
				if(child.gameObject.GetInstanceID() == hexPattern.gameObject.GetInstanceID()){//znaczy że dane dziecko to pattern a z nim nic nie robimy
					continue;
				}
				Destroy(child.gameObject);
			}
			this.listenerList.OnResetEnd ();
		}
	}
}
