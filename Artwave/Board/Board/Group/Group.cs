using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Artwave.Board{
	public class Group{
		public string name;
		public List<HexField> list;
		/**
		 * jest to referencja do dowolnego obiektu jaki można przypiąć do grupy aby zwiększyć jej zastosowanie (np dodatkowe informacje czy dane, czy referencje)
		 */ 
		public Object hexLogic;
	}
}