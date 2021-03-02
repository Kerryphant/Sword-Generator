using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollapsibleMenu : MonoBehaviour
{
	public Button button;
	public GameObject panel;
	bool enabled = false;

    // Start is called before the first frame update
    void Start()
    {
		button = transform.GetComponent<Button>();
		button.onClick.AddListener(OnCollapseClick);
    }

	// Update is called once per frame
	void Update()
	{
		
	}

	void OnCollapseClick()
	{
		enabled = !enabled;
		panel.SetActive(enabled);
	}
}
