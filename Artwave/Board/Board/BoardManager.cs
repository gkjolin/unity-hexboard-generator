using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Artwave.Board{
	public class BoardManager : MonoBehaviour {
		/**
		 *  czy ma samodzielnie zarządzać planszą
		 */ 
		public bool selfDraw = false;

		public BoardHexBuilder builder;
		public BoardController boardController;
		public BoardHex board;
		public GroupController groupController;
		private bool isInit = false;
		public void Init(){
			if (isInit == false) {
				isInit = true;
				board.Init();
				boardController.Init();
				groupController.Init();
				boardController.SetBoard (board);
				builder.Init();
				builder.SetBoard (board);
				AddBuildListener(boardController);
			}
		}
		void Start () {
			if (selfDraw) {//żeby nie rysował gdy builder sam siebie obsługuje
				Init ();
				this.SetTableSizeFullScreen ();
				builder.Build ();
			}
		}
 		/**
		 * ustawia full screen mapę, możemy jednak chcieć na dole lub górze mieć jakieś elementy interfejsu, więc origin stołu musi być w trochę innym miejscu
		 * dając top i bottom, właściwie otrzymamy podobny efekt co przy marginesach kontrolera. Oba efekty się sumują
		 * marginy i paddingi pozwalają dać w danym boku np menu, i możliwość przesuwania tak mapą by to menu jej nie przysłaniało
		 * 
		 * Nie zmienia boarda, board ma parametry jakie są ustawione na scenie
		 */
		public void SetTableSizeFullScreen(float offsetTop = 0 , float offsetBottom = 0){
			Camera cam = Camera.main;
			float camHeight = 2f * cam.orthographicSize;
			float camWidth = camHeight * cam.aspect;
			this.boardController.size = new Vector2(camWidth, camHeight - offsetTop - offsetBottom);
			this.transform.position = new Vector3 (0, (offsetBottom - offsetTop) / 2, 0);
			this.boardController.CenterBoard ();
		}
		public void SetTableSize(float width, float height){
			this.boardController.size = new Vector2(width, height);
			this.boardController.CenterBoard ();
		}
		/**
		 * ustawia rozmiar planszy względem stołu tak  by, plansza na wysokość zajmuje tyle co stół więc nie będzie scrolla na wysokość
		 * na szerokość będzie zajmowała tyle ile jest potrzeba na narysowanie. Prawdopodobnie pojawi się scroll
		 */ 
		public void SetBoardSizeHorizontalScroll(){
			this.builder.recommendSize.y = boardController.size.y;
			this.builder.recommendSize.x = 10000;
		}
		/**
		 * ustawia rozmiar planszy względem stołu tak  by, plansza na szerokość zajmuje tyle co stół więc nie będzie scrolla na szerokość
		 * na wysokość będzie zajmowała tyle ile jest potrzeba na narysowanie. Prawdopodobnie pojawi się scroll
		 */ 
		public void SetBoardSizeVerticalScroll(){
			this.builder.recommendSize.y = 10000;
			this.builder.recommendSize.x = boardController.size.x;
		}
		/**
		 * ustawia rozmiar planszy względem stołu tak  by, plansza na szerokość i wysokość zajmie tyle co stół więc nie będzie scrolla na szerokość i wysokość
		 */ 
		public void SetBoardSizeNoScroll(){
			this.builder.recommendSize.y = boardController.size.y;
			this.builder.recommendSize.x = boardController.size.x;
		}

		public void Build(int colHexNumber=0, int rowHexNumber=0){
			if (selfDraw == true) {//blokujemy rysowanie ponieważ jest włączony tryb samorysujący
				Debug.LogError ("BoardManager is in autobuild mode, therefore there is no posibility to generate board by api");
			}
			this.boardController.CenterBoard ();
			builder.colNumber = colHexNumber > 0 ? colHexNumber : this.builder.colNumber;
			builder.rowNumber = rowHexNumber>0?rowHexNumber:this.builder.rowNumber;
			builder.Build ();
		}
		public void Reset(){
			groupController.ClearAll ();
			builder.Reset();
		}
		public void AddBoardListener(BoardListenerInterface listener){
			boardController.AddListener (listener);
		}
		public void AddBuildListener(BuilderListenerInterface listener){
			builder.AddListener (listener);
		}
		public HexField GetHex(Vector3 coordinates){
			return board.GetHex (coordinates);
		}
		public List<HexField> GetAllHex(){
			return board.GetAll ();
		}
		public List<HexField> GetNeigbors(Vector3 centerCoordinates, int range = 1){
			return board.GetNeighbors (centerCoordinates, range);
		}
		public HexField GetPatternHex(){
			return this.builder.GetPatternHex ();

		}
		/**
		 * ustawia centrum planszy w pozycji danego hexa w sposób animowany
		 * animationEventBlock jeśli true to blokuje dotyk i ręczne przesuwanie mapy
		 * w przeciwnym razie gracz może próbować wymusić inny ruch
		 */ 
		public void AnimationGoToHex(HexField hex, float animationSpeed = 3, bool animationEventBlock = false){
			boardController.AnimationGoToHex (hex, animationSpeed, animationEventBlock);
		}
		/**
		 *  ustawia centrum planszy w pozycji danego hexa
		 */ 
		public void GoToHex(HexField hex){
			boardController.GoToHex (hex);
		}
		/**
		 * pozwala dodać hex do grupy
		 */ 
		public void AddHexToGroup(string groupName, HexField hex){
			groupController.AddHex (groupName, hex);
		}
		/**
		 * zwraca grupę na podstawie nazwy
		 */ 
		public Group GetGroup(string groupName){
			return groupController.GetGroup (groupName);
		}
	}
}
