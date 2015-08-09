using UnityEngine;
using System.Collections;

public class Event {
	public enum TYPE {
		NONE = -1,
		ROCKET = 0,		// ロケット修理.
		FIRE,
		NUM,
	};
};


public class EventRoot : MonoBehaviour {

	public Event.TYPE getEventType(GameObject event_go)
	{
		Event.TYPE	type = Event.TYPE.NONE;
		if(event_go != null) {
			if(event_go.tag == "Rocket") {
				type = Event.TYPE.ROCKET;
			}

			//fire.
			if(event_go.tag == "Fire"){
				type = Event.TYPE.FIRE;
			}
		}
		return(type);
	}



	public bool	isEventIgnitable(Item.TYPE carried_item, GameObject event_go)
	{
		bool		ret  = false;
		Event.TYPE	type = Event.TYPE.NONE;
		if(event_go != null) {
			type = this.getEventType(event_go);
		}
		switch(type) {
		case Event.TYPE.ROCKET:
			if(carried_item == Item.TYPE.IRON) {
				ret = true;
			}
			if(carried_item == Item.TYPE.PLANT) {
				ret = true;
			}
			break;

		case Event.TYPE.FIRE:
			if(carried_item == Item.TYPE.APPLE){
				ret = true;
			}
			if(carried_item == Item.TYPE.PLANT){
				ret = true;
			}
			break;
		}
		return(ret);
	}


	public string getIgnitableMessage(GameObject event_go)
	{
		string	message = "";
		Event.TYPE	type    = Event.TYPE.NONE;
		if(event_go != null) {
			type = this.getEventType(event_go);
		}
		switch(type) {
		case Event.TYPE.ROCKET:
			message = "修理する";
			break;
		case Event.TYPE.FIRE:
			message = "火にくべる";
			break;
		}
		return(message);
	}


}
