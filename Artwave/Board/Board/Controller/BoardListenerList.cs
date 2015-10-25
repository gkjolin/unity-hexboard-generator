using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Artwave.Board{
	/**
	 * jest to obiekt listenera przechowujący grupę listenerów, używamy jak zwykłego ale pod spodem wywołuje wszystkie na liście.
	 * W controlerze może być wiele listenerów, by dla eventa za każdym razem nie iterować po wszystkich to użyjemy tej klasy
	 * ma ona ten sam interfejs a iteracja idzie w jej wnętrzu
	 */ 
	public class BoardListenerList : BoardListenerInterface {
		private List<BoardListenerInterface> listenerList;
		public BoardListenerList(){
			listenerList = new List<BoardListenerInterface> ();
		}
		public void Add(BoardListenerInterface listener){
			foreach(BoardListenerInterface currListener in listenerList){
				if(currListener == listener){
					return;
				}
			}
			listenerList.Add (listener);
		}
		public void OnHexBoardDown(HexField hex){
			foreach (BoardListenerInterface listener in listenerList) {
				listener.OnHexBoardDown(hex);
			}
		}
		public void OnHexBoardUp(HexField hex){
			foreach (BoardListenerInterface listener in listenerList) {
				listener.OnHexBoardUp(hex);
			}
		}
		public void OnBoardDrag(){
			foreach (BoardListenerInterface listener in listenerList) {
				listener.OnBoardDrag();
			}
		}
		public void OnGoToHexAnimationFinish(){
			foreach (BoardListenerInterface listener in listenerList) {
				listener.OnGoToHexAnimationFinish();
			}
		}
	}
}
