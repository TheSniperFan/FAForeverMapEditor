﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarkersList : MonoBehaviour {

	public			MapLuaParser		Scenario;
	public			CameraControler		KameraKontroler;

	public			RectTransform		Pivot;
	public			GameObject			ListPrefab;
	public			List<ListObject>	AllFields = new List<ListObject>();

	public			Sprite[]			Icons;

	void OnEnable(){
		GenerateList();

	}

	void OnDisable(){
		foreach(RectTransform child in Pivot){
			AllFields = new List<ListObject>();
			Destroy(child.gameObject);
		}
	}

	void GenerateList(){
		int count = 0;
		for(int i = 0; i < Scenario.ARMY_.Count; i++){
			GameObject newList = Instantiate(ListPrefab, Pivot.position, Quaternion.identity) as GameObject;
			newList.GetComponent<RectTransform>().SetParent(Pivot);
			newList.GetComponent<RectTransform>().localPosition = Vector3.up * -30 * count;
			newList.GetComponent<RectTransform>().sizeDelta = new Vector3(1, 30);
			AllFields.Add(newList.GetComponent<ListObject>());

			AllFields[count].ObjectName.text = Scenario.ARMY_[i].name;
			AllFields[count].Icon.sprite = Icons[0];
			AllFields[count].ConnectedGameObject = Scenario.ARMY_[i].Mark.gameObject;
			AllFields[count].KameraKontroler = KameraKontroler;

			count++;
		}

		for(int i = 0; i < Scenario.Mexes.Count; i++){
			GameObject newList = Instantiate(ListPrefab, Pivot.position, Quaternion.identity) as GameObject;
			newList.GetComponent<RectTransform>().SetParent(Pivot);
			newList.GetComponent<RectTransform>().localPosition = Vector3.up * -30 * count;
			newList.GetComponent<RectTransform>().sizeDelta = new Vector3(1, 30);

			AllFields.Add(newList.GetComponent<ListObject>());
			AllFields[count].ObjectName.text = Scenario.Mexes[i].name;
			AllFields[count].Icon.sprite = Icons[1];
			AllFields[count].ConnectedGameObject = Scenario.Mexes[i].Mark.gameObject;
			AllFields[count].KameraKontroler = KameraKontroler;

			count++;
		}

		for(int i = 0; i < Scenario.Hydros.Count; i++){
			GameObject newList = Instantiate(ListPrefab, Pivot.position, Quaternion.identity) as GameObject;
			newList.GetComponent<RectTransform>().SetParent(Pivot);
			newList.GetComponent<RectTransform>().localPosition = Vector3.up * -30 * count;
			newList.GetComponent<RectTransform>().sizeDelta = new Vector3(1, 30);
			AllFields.Add(newList.GetComponent<ListObject>());
			AllFields[count].ObjectName.text = Scenario.Hydros[i].name;
			AllFields[count].Icon.sprite = Icons[2];
			AllFields[count].ConnectedGameObject = Scenario.Hydros[i].Mark.gameObject;
			AllFields[count].KameraKontroler = KameraKontroler;

			count++;
		}

		for(int i = 0; i < Scenario.SiMarkers.Count; i++){
			GameObject newList = Instantiate(ListPrefab, Pivot.position, Quaternion.identity) as GameObject;
			newList.GetComponent<RectTransform>().SetParent(Pivot);
			newList.GetComponent<RectTransform>().localPosition = Vector3.up * -30 * count;
			newList.GetComponent<RectTransform>().sizeDelta = new Vector3(1, 30);
			AllFields.Add(newList.GetComponent<ListObject>());
			AllFields[count].ObjectName.text = Scenario.SiMarkers[i].name;
			AllFields[count].Icon.sprite = Icons[3];
			AllFields[count].ConnectedGameObject = Scenario.SiMarkers[i].Mark.gameObject;
			AllFields[count].KameraKontroler = KameraKontroler;

			count++;
		}

		Vector2 PivotRect = Pivot.sizeDelta ;
		PivotRect.y = 30 * count;
		Pivot.sizeDelta  = PivotRect;
	}
}