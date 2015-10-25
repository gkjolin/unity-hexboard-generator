using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
namespace Artwave.Board{
	public class HexField : MonoBehaviour {
		/**
		 * zawiera nazwy grup w których powinna się ten hex znajdować, grupy przechowywane są w GroupController
		 */ 
		[HideInInspector]
		public List<string> groupListNames;
		/**
		 * hex zakłada że ta wysokość to wysokość którą ma przeskalować do rozmiaru podanego przez buildera
		 * jeśli dodana do hexa grafika będzie większa, wtedy będzie wychodziła poza zakres, proporcjonalnie,
		 * np jeśli ustawimy referenceHeight na 3, a grafikę damy 4.5 to będzie ona o 50% większa niezależnie od skali
		 */ 
		[Range(0.1f, 10.0f)]
		public float referenceHeight = 3;
		/**
		 *  wskaźnik na spriterenderera z grafiką będącą na potrzeby debugowania
		 */ 
		private SpriteRenderer spriteRenderer;
		public CircleCollider2D circleCollider2D;
		/**
		 * wysokość i szerokość hexa. Jest ona określana w builderze. Jest to rozmiar jaki jest przeznaczony na mapie dla tego hexa.
		 * Odpowiedzialność po stronie hexa by doskalował posiadane grafiki i swój obiekt. Inaczej będą miały pełny rozmiar, bo buildera to nie obchodzi.
		 */ 
		private Vector2 size;
		/**
		 * jest to referencja do dowolnego obiektu jaki można przypiąć do hexa aby zwiększyć zastosowanie hexa (np dodatkowe informacje czy dane, czy referencje)
		 * najlepiej jak moduł będzie dodany do hexa i będzie Komponentem
		 */ 
		public Object hexLogic;
		/**
		 * współrzędne hexa wyrażone w trzech wartościach x,y,z (Cube cordinates)
		 */ 
		private Vector3 coordinates;
		private HexListenerList listenerList;
		public void Init(){
			spriteRenderer = GetComponent<SpriteRenderer>();
			groupListNames = new List<string>();
			circleCollider2D = GetComponent<CircleCollider2D>();
		}
		public void SetSize(Vector2 size){
			this.size = size;
			float xScale =  size.x / HexMath.CalculateHexWidth(referenceHeight);
			float yScale =  size.y / referenceHeight;
			float hexScale = Mathf.Min (xScale,yScale);
			transform.localScale = new Vector3 (hexScale, hexScale, 1);
		}
		public virtual void SetPosition(Vector3 position){
			transform.localPosition = position;
		}
		public Vector3 GetCoordinates(){
			return coordinates;
		}
		public void SetCoordinates(Vector3 coordinates){
			this.coordinates = coordinates;
		}
		/**
		 * Rozmiar rzeczywisty hexa - uwzględnia już skalę jeśli został pomniejszony
		 */ 
		public Vector2 GetSize(){
			return size;
		}
		/**
		 * w hexie może być więcej listenerów, dlatego musi być lista, listę możemy przekazać. Pozwala to dla całej planszy ustawić jedną listę eventów
		 */ 
		public void SetListenerList(HexListenerList listenerList){

			this.listenerList = listenerList;
		}
		public void ShowColor(Color color){
			if (spriteRenderer && spriteRenderer.enabled) {
				spriteRenderer.color = color;
			}
		}
		void Update(){}
		/**
		 * 
		 * jeśli true to znaczy że wduszenie przycisku było nad elementem UI. A w takiej sytuacji nie chcemy odpalać eventa OnMouseUp
		 */ 
		private bool OnMouseDownUI = false;
		void OnMouseDown(){
			if (EventSystem.current.IsPointerOverGameObject ()) {
				OnMouseDownUI = true;
				return;
			}
			foreach(Touch t in Input.touches)			{
				if(EventSystem.current.IsPointerOverGameObject(t.fingerId)){
					OnMouseDownUI = true;
					return;

				}
			}
			if (this.listenerList != null) {
				OnMouseDownUI = false;
				this.listenerList.OnHexDown (this);
			}
		}
		void OnMouseUp(){
			if (OnMouseDownUI == true) {
				return;
			}
			if (EventSystem.current.IsPointerOverGameObject ()) {
				return;
			}
			foreach(Touch t in Input.touches)			{
				if(EventSystem.current.IsPointerOverGameObject(t.fingerId)){
					return;
				}
			}
				if (this.listenerList != null) {
					this.listenerList.OnHexUp (this);
				}	
		}
		void OnMouseOver(){
			if (this.listenerList != null) {
				this.listenerList.OnHexOver (this);
			}
		}
	}
}