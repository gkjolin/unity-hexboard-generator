using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Artwave.Board{
	/**
	 * pozwala grupować hexy. Możemy np pogrupować hexy które mają mieć bombę, albo które są kolizjami na ruch. Każda grupa może zawierać dodatkową logikę
	 * Bez problemu możemy pobrać listę hexów spełniających to kryterium.
	 */ 
	public class GroupController : MonoBehaviour {
		/**
		 * zawiera wszystkie istniejące grupy
		 */ 
		private Dictionary<string, Group> groupList;
		public void Init(){
			enabled = true;
			groupList = new Dictionary<string, Group> ();
		}
		public void ClearAll(){
			foreach(KeyValuePair<string, Group> entry in groupList ){
				foreach(HexField hex in entry.Value.list){
					hex.groupListNames.Clear();//każdemu hexowi z grupy usuwamy informacje że należał do jakiejkolwiek grupy
				}
				entry.Value.list.Clear();//oczyszczamy daną grupę
			}
			//nie oczyszczamy słownika, zakładamy że nowa mapa musi mieć te same grupy
			//groupList.Clear ();//oczyszczamy słownik zawierający wszystkie grupy - żadna grupa nie powinna mieć powiązań do czegokolwiek
		}
		public void AddHex(string groupName, HexField hex){
			Group group;
			if (groupList.TryGetValue (groupName, out group)) {//znaczy że dana grupa już istnieje
				bool isNew = true;
				foreach(string actualHexGroupName in hex.groupListNames){
					if(actualHexGroupName == groupName){
						isNew = false;
						break;
					}
				}
				if(isNew == true){//dany hex nie jest w tej grupie
					group.list.Add(hex);
					hex.groupListNames.Add(groupName);
				}
			} else { // trzeba utworzyć grupę
				group= new Group();
				group.name = groupName;
				group.list.Add(hex);
				groupList.Add(groupName, group);
				hex.groupListNames.Add(groupName);
			}
		}
		void Update () {}
		/**
		 * usuwamy hexa z danej groupy
		 */ 
		public void RemoveHexFromGroup(string groupName, HexField hex){
			Group group;
			if (groupList.TryGetValue (groupName, out group)) {//znaczy że dana grupa już istnieje
				group.list.Remove(hex);
				hex.groupListNames.Remove(groupName);
			} else { // dana grupa nie istnieje więc hex w niej też nie istnieje
			}
		}
		/**
		 * zwraca grupę, jeśli grupa nie istnieje zwraca nową
		 */ 
		public Group GetGroup(string groupName){
			Group group;
			if (groupList.TryGetValue (groupName, out group)) {//znaczy że dana grupa już istnieje
				return group;
			} else { // trzeba utworzyć grupę
				group= new Group();
				group.name = groupName;
				groupList.Add(groupName, group);
				return group;
			}
		}
	}
}
