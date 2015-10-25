using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
namespace Artwave.Board{
	public class BoardController : MonoBehaviour, HexListenerInterface, BuilderListenerInterface {
		#region BuilderListenerInterface implementation

		public void OnBuildStart (HexField patternHex, int colNumber, int rowNumber, Vector2 hexSize, bool isEven, bool symmetricHorizontal)
		{
		}
		public void OnBuildEnd ()
		{
			if (hexCollidersOnlyOnScreen) {
				CalculateHexColliders ();
			} else {
				List<HexField> hexList = this.board.GetAll();
				foreach(HexField hex in hexList){
					hex.circleCollider2D.enabled = true;
				}
			}
		}

		public void OnResetStart ()
		{
		}
		public void OnResetEnd ()
		{
		}
		public bool OnCreateHexStart (Vector3 coordinatesCube, Vector2 coordinatesOffset, bool isInverse)
		{
			return true;
		}
		public void OnCreateHexEnd (HexField hex)
		{
		}
		#endregion
		/**
		 * określa czułość przesuwania myszy czy dotyku. Dokładniej o ile procent szerokości ekranu musimy przesunąć input by był to move
		 */  
		[Tooltip("Określa czułość, kiedy przesunięcie. Wyrażone w procentach szerokości ekranu")]
		[Range(1f,20)]
		public float moveSensitiveDistancePercentage = 5f;
		/**
		 * czy system włącza collidery na hexach tylko w obrębie kamery
		 */ 
		public bool hexCollidersOnlyOnScreen = true;
		[Tooltip("Określa rozmiar poza ekranem który też jest brany jako część ekranu do wyświetlania colliderów.")]
		[Range(0,20)]
		public float hexCollidersScreenSize = 2f;
		public bool enabledClickEvents = true;
		public bool enabledDragEvents = true;
		public bool drawGizmos = false;
		/**
		 * jest to lista listenerów która nasłuchuje tego controllera. Jest to ciekawe, bo kontroler ten nasłuchuje z hexów, obrabia zdarzenia i wysyła do swoich nasłuchów
		 */ 
		private BoardListenerList listenerList;
		public Vector2 size;//rozmiar stołu na którym jest plansza; rozmiar w jakim może być plansza. jeśli plansza ma ten sam rozmiar lub mniejszy to się nie rusza w danej osi. Jeśli plansza ma większy rozmiar ro może się ruszać w danej osi
		private BoardHex board;
		public void Init(){
			enabled = true;
			this.listenerList = new BoardListenerList ();
		}
		/**
		 * dodajemy listener który nasłuchuje na informacje z kontrollera
		 */ 
		public void AddListener(BoardListenerInterface listener){
			this.listenerList.Add (listener);
		}
		public void SetBoard(BoardHex board){
			this.board = board;
			this.board.AddListener (this);
//			if (hexCollidersOnlyOnScreen) {
//				CalculateHexColliders();
//			}
		}
		/**
		 * na różnych użądzeniach mamy różną szerokość w pixelach. W celu normalizacji kiedy mamy dotyk a kiedy ruch, czułość wyrażamy w procentach
		 */ 
		public float normalizeMoveSensitiveDistance(float distanceWidthPercentage){
			return Screen.width / 100 * distanceWidthPercentage;
		}
		private Vector3 animationTarget;
		private bool isAnimation = true;
		private float animationSpeed;
		private bool animationEventBlock;
		public void AnimationGoToHex(HexField hex, float animationSpeed = 5, bool animationEventBlock = false){
			isAnimation = true;
			this.animationSpeed = animationSpeed;
			this.animationEventBlock = animationEventBlock;
			animationTarget = CalculateBoardNewPosition(-hex.transform.localPosition);
		}
		/**
		 *  ustawia centrum planszy w pozycji danego hexa
		 */ 
		public void GoToHex(HexField hex){
			isAnimation = false;
			this.board.transform.position = CalculateBoardNewPosition(-hex.transform.localPosition);
		}
		void Update () {
			this.CheckTouchInput ();
			if (isAnimation) {
				float step = animationSpeed * Time.deltaTime;
				this.board.transform.position = Vector3.MoveTowards (this.board.transform.position, animationTarget, step);
				if (hexCollidersOnlyOnScreen) {
					CalculateHexColliders ();
				}
				if(this.board.transform.position == animationTarget){
					isAnimation = false;
					this.listenerList.OnGoToHexAnimationFinish();
				}
			}

		}
		/**
		 * listę colliderów sprawdza czy są w zasięgu kamery. Jeśli tak to mają collidery jeśli nie to wyłączone zostają
		 */ 
		private void CalculateHexColliders(){
			Camera cam = Camera.main;
			float camHeight = 2f * cam.orthographicSize;
			float camWidth = camHeight * cam.aspect;
			float xMin = cam.transform.position.x - camWidth / 2 - hexCollidersScreenSize;
			float xMax = cam.transform.position.x + camWidth / 2 + hexCollidersScreenSize;
			float yMin = cam.transform.position.y - camHeight / 2 - hexCollidersScreenSize;
			float yMax = cam.transform.position.y + camHeight / 2 + hexCollidersScreenSize;
			List<HexField> hexList = this.board.GetAll();
			foreach(HexField hex in hexList){
				if(hex.transform.position.x > xMin && hex.transform.position.x < xMax && hex.transform.position.y > yMin && hex.transform.position.y < yMax){
					hex.circleCollider2D.enabled = true;
				} else {
					hex.circleCollider2D.enabled = false;
				}
			}
		}
		private bool isInputDrag = false;
		/**
		 * jest to relacja wduszonego punktu / pozycji hexa do origin board. Dzięki czemu w dowolnym miejscu od docelowego punktu odejmujemy tą relację
		 * i mamy poprawne położenie mapy. 
		 * Zastosowanie gdy chcemy by zaznaczony punkt był przesunięty w nowe miejsce - a na prawdę przesuwamy mapą
		 */ 
		private Vector3 relativePointParentVector;

		public void OnHexDown(HexField hex){
			if (isAnimation && animationEventBlock)return;//jeśli animacja zablokowała eventy
			if (enabledClickEvents) {
				this.listenerList.OnHexBoardDown (hex);
			}
		}
		public void OnHexUp(HexField hex){
			if (isInputDrag) {//jeśli jest drag to zatrzymuje informacje o hexUp
				return;
			}
			if (isAnimation && animationEventBlock)return;//jeśli animacja zablokowała eventy
			if (enabledClickEvents) {
				this.listenerList.OnHexBoardUp (hex);
			}
		}

		public void OnHexOver(HexField hex){
		}
		private Vector3  startPosition;

		/**
		 * flaga ustawiona na false zablkouje przesuwanie mapy. Ponowne wduszenie pointera odblokuje.
		 * Blokada następuje jeśli pointer trafi na Element UI - nie chcemy by jeżdżąc po popupie mapa pod spodem się przesuwała
		 */ 
		private bool stopMove = false;
		/**
		 * sprawdza czy jest wduszona myszka lub dotyk
		 */ 
		private void CheckTouchInput(){
			if (Input.touchCount > 0) {
				Touch touch = Input.GetTouch (0);
				if (touch.phase == TouchPhase.Began) {
					isInputDrag = false;
					stopMove = false;
					startPosition = touch.position;
					relativePointParentVector = CalculateInputPositionToCamera (touch.position, this.board.transform.position.z) - this.board.transform.position;
				} else if (touch.phase == TouchPhase.Moved && Vector3.Distance (startPosition, touch.position) > normalizeMoveSensitiveDistance(moveSensitiveDistancePercentage)) {
					if (stopMove)
						return;
					if (isAnimation && animationEventBlock)
						return;//jeśli animacja zablokowała eventy
					if (enabledDragEvents) {
						isInputDrag = true;
						this.listenerList.OnBoardDrag ();
						Vector3 newPosition = CalculateInputPositionToCamera (touch.position, this.board.transform.position.z) - relativePointParentVector;
						this.board.transform.position = CalculateBoardNewPosition (newPosition);
					}
				} else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) {
					isInputDrag = false;
					if (hexCollidersOnlyOnScreen) {
						CalculateHexColliders();
					}
				}
			} else if (Input.GetMouseButtonDown (0)) {
				isInputDrag = false;
				stopMove = false;
				startPosition = Input.mousePosition;
				relativePointParentVector = CalculateInputPositionToCamera (Input.mousePosition, this.board.transform.position.z) - this.board.transform.position;
			} else if (Input.GetMouseButton (0)) {
				if (stopMove)
					return;
				if (Vector3.Distance (startPosition, Input.mousePosition) > normalizeMoveSensitiveDistance(moveSensitiveDistancePercentage)) {//jeśli przesunięcie jest większe od 5 pikseli
					if (isAnimation && animationEventBlock)
						return;//jeśli animacja zablokowała eventy
					if (enabledDragEvents) {
						isInputDrag = true;
						this.listenerList.OnBoardDrag ();
						Vector3 newPosition = CalculateInputPositionToCamera (Input.mousePosition, this.board.transform.position.z) - relativePointParentVector;
						this.board.transform.position = CalculateBoardNewPosition (newPosition);
					}
				}
			} else if (Input.GetMouseButtonUp (0)) {
				if (hexCollidersOnlyOnScreen) {
					CalculateHexColliders();
				}
			} else {//żaden przycisk nie jest wduszony
				isInputDrag = false;
			}
			if (EventSystem.current.IsPointerOverGameObject ()) { //wskaźnik jest na elemencie UI więc nie przesuwamy dalej
				stopMove = true;
			}
			foreach(Touch t in Input.touches)			{
				if(EventSystem.current.IsPointerOverGameObject(t.fingerId)){
					stopMove = true;
					break;;
					
				}
			}
		}
		void OnDrawGizmos(){
			if (drawGizmos) {
				Gizmos.color = new Color (0, 0, 1, 0.5f);
				Gizmos.DrawCube (transform.position, new Vector3 (size.x, size.y, 0.1f));
				if(marginTop > 0 || marginBottom > 0 || marginLeft > 0 || marginRight > 0){
					Gizmos.color = new Color (1, 0, 0, 0.5f);
					Gizmos.DrawCube (new Vector3 (transform.position.x + marginLeft / 2 - marginRight /2, transform.position.y - marginTop /2 + marginBottom / 2, transform.position.z), new Vector3 (size.x - marginLeft - marginRight, size.y - marginTop - marginBottom, 0.1f));
				}
			}
		}
		/**
		 * przelicza na pozycję kamery, działa dobrze tylko dla ortogonalnej, przy perspektywicznej trzeba by użyć Raycast
		 * pozycję z ustawia na tą samą co kamera, dlatego mamy drugi parametr by ustawić odpowiednie z
		 */ 
		private Vector3 CalculateInputPositionToCamera(Vector3 inputPosition, float zPosition){
			Vector3 position = Camera.main.ScreenToWorldPoint(inputPosition);
			position = new Vector3 (position.x, position.y, zPosition);
			return position;
		}
		/**
		 * ustawia planszę w centrum stołu. Może to być przydatne na początku i potem jeśli zmieniamy opcje dragowania planszy
		 */ 
		public void CenterBoard(float offsetY = 0){
			this.board.transform.position = new Vector3 (transform.position.x, transform.position.y - offsetY, transform.position.z);
		}
		/**
		 * normalnie granicą draga mapy jest granica wrappera, możemy zrobić że w obrębie wrappera granica będzie przesunięta przez marginesy (przesuwając mapę możemy mieć pustą przestrzeń do wartości margina)
		 */
		public float marginTop = 0, marginBottom = 0, marginLeft = 0, marginRight = 0;
		/**
		 * marginy działają tylko gdy plansza większa od stołu, jeśli plansza trochę mniejsza to niezależnie jak duże są marginy plansza zostanie w danej osi unieruchomiona
		 * w przeciwnym razie mała plansza na stole z duzymi marginami będzie się  przesuwać po stole (czasem możemy to chcieć). Przy braku marginesów lub gdy board jest mniejszy od sumy stołu i marginesów nie będzie efektu
		 */ 
		public bool marginIfBoardBiggerThenScreen = false;
		/**
		 * blokuje drag jeśli plansza jest mniejsza od stołu lub tak by drag nie przsuwał pustego miejsca w obszar ekranu
		 * jest to pozycja w unitach
		 */ 
		private Vector3 CalculateBoardNewPosition(Vector3 newPosition){
			//testy wysokości
			if (board.GetSize ().y <= size.y - marginTop * (marginIfBoardBiggerThenScreen?0:1) - marginBottom * (marginIfBoardBiggerThenScreen?0:1)) {//sprawdzamy czy rozmiar planszy jest mniejszy od rozmiaru stołu, jeśli tak to blokujemy drag w pionie
				newPosition.y = this.board.transform.position.y;
			} else if (newPosition.y + board.GetSize ().y / 2 < transform.position.y + size.y / 2 - marginTop) { // plansza jest większa ale może dragować tylko do granicy plansza-stół, test dotyczy góry ekranu
				newPosition.y = transform.position.y + size.y / 2 - board.GetSize ().y / 2 - marginTop;
			} else if(newPosition.y - board.GetSize ().y / 2 > transform.position.y - size.y / 2 + marginBottom){//test dotyczy dołu ekranu
				newPosition.y = transform.position.y - size.y / 2 + board.GetSize ().y / 2 + marginBottom;
			}
			//testy szerokości
			if (board.GetSize ().x <= size.x - marginLeft * (marginIfBoardBiggerThenScreen?0:1) - marginRight * (marginIfBoardBiggerThenScreen?0:1)) {//sprawdzamy czy rozmiar planszy jest mniejszy od rozmiaru stołu, jeśli tak to blokujemy drag w poziomie
				newPosition.x = this.board.transform.position.x;
			} else if (newPosition.x + board.GetSize ().x / 2 < transform.position.x + size.x / 2 - marginRight) { // plansza jest większa ale może dragować tylko do granicy plansza-stół, test dotyczy lewej częsci ekranu
				newPosition.x = transform.position.x + size.x / 2 - board.GetSize ().x / 2 - marginRight;
			} else if(newPosition.x - board.GetSize ().x / 2 > transform.position.x - size.x / 2 + marginLeft){//test dotyczy prawej ekranu
				newPosition.x = transform.position.x - size.x / 2 + board.GetSize ().x / 2 + marginLeft;
			}
			return newPosition;
		}
	}
}
