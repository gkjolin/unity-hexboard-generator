using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Artwave.Board{
	/**
	 * jest to obiekt listenera przechowujący grupę listenerów, używamy jak zwykłego ale pod spodem wywołuje wszystkie na liście.
	 * W hexie może być wiele listenerów, by dla eventa za każdym razem nie iterować po wszystkich to użyjemy tej klasy
	 * ma ona ten sam interfejs a iteracja idzie w jej wnętrzu
	 */ 
	public class HexListenerList : HexListenerInterface {
		private List<HexListenerInterface> listenerList;
		public HexListenerList(){
			listenerList = new List<HexListenerInterface> ();
		}
		public void Add(HexListenerInterface listener){
			listenerList.Add (listener);
		}
		public void OnHexDown(HexField hex){
			foreach (HexListenerInterface listener in listenerList) {
				listener.OnHexDown(hex);
			}
		}
		public void OnHexUp(HexField hex){
			foreach (HexListenerInterface listener in listenerList) {
				listener.OnHexUp(hex);
			}
		}
		public void OnHexOver(HexField hex){
			foreach (HexListenerInterface listener in listenerList) {
				listener.OnHexOver(hex);
			}
		}
//		public void OnHexExit(AbstractHex hex){
//			foreach (HexListenerInterface listener in listenerList) {
//				listener.OnHexExit(hex);
//			}
//		}
	}
}
