﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class Connection : MonoBehaviour {
	private RectTransform rectTransform;
	public void SetPosition(Vector2 start, Vector2 end) {
		float xScale = 1f / transform.parent.localScale.x;

		transform.localScale = new Vector3(xScale, 2, 2);
		
		if (!rectTransform) rectTransform = (RectTransform) transform;
		transform.position = (start + end) * 0.5f;

		float r = Mathf.Atan2(start.y - end.y, start.x - end.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0, 0, r);
		rectTransform.sizeDelta = new Vector2(Vector2.Distance(start, end), rectTransform.sizeDelta.y);
	}
}